using DragonBallLibrary.ApiService.Data;
using DragonBallLibrary.ApiService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();


// Add Azure App Configuration
// In production, this would connect to actual Azure App Configuration
// builder.Configuration.AddAzureAppConfiguration(options =>
// {
//     options.Connect(connectionString)
//            .UseFeatureFlags();
// });

// Add Azure Key Vault
// In production, this would connect to actual Azure Key Vault
// builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

// Add Entity Framework with SQL Server (using InMemory for demonstration)
builder.Services.AddDbContext<DragonBallContext>(options =>
{
    // In production, use SQL Server:
    // options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    
    // For demonstration, using InMemory database
    options.UseInMemoryDatabase("DragonBallDb");
});

// Add services to the container.
builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Dragon Ball Character Library API", 
        Version = "v1",
        Description = "API for managing Dragon Ball characters with Azure services integration"
    });
});

// Register Azure services
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DragonBallContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dragon Ball Character Library API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");

app.UseAuthorization();

// Dragon Ball Characters endpoints using Entity Framework
app.MapGet("/api/characters", async (DragonBallContext context) =>
{
    var characters = await context.Characters.ToListAsync();
    return Results.Ok(characters);
})
.WithName("GetCharacters")
.WithTags("Characters");

app.MapGet("/api/characters/{id:int}", async (int id, DragonBallContext context) =>
{
    var character = await context.Characters.FindAsync(id);
    return character is not null ? Results.Ok(character) : Results.NotFound();
})
.WithName("GetCharacter")
.WithTags("Characters");

app.MapPost("/api/characters", async (CreateCharacterRequest request, DragonBallContext context, IBlobStorageService blobService) =>
{
    // Get the image URL from blob storage
    var imageUrl = await blobService.GetCharacterImageUrlAsync(request.Name);
    
    var character = new DragonBallCharacter(
        0, // EF will generate ID
        request.Name,
        request.Race,
        request.Planet,
        request.Transformation,
        request.Technique,
        imageUrl
    );
    
    context.Characters.Add(character);
    await context.SaveChangesAsync();
    
    return Results.Created($"/api/characters/{character.Id}", character);
})
.WithName("CreateCharacter")
.WithTags("Characters");

app.MapPut("/api/characters/{id:int}", async (int id, UpdateCharacterRequest request, DragonBallContext context, IBlobStorageService blobService) =>
{
    var character = await context.Characters.FindAsync(id);
    if (character is null)
        return Results.NotFound();

    // Get the image URL from blob storage if not provided
    var imageUrl = request.ImageUrl ?? await blobService.GetCharacterImageUrlAsync(request.Name);

    // Update properties (using reflection or manual assignment)
    var updatedCharacter = character with
    {
        Name = request.Name,
        Race = request.Race,
        Planet = request.Planet,
        Transformation = request.Transformation,
        Technique = request.Technique,
        ImageUrl = imageUrl
    };

    context.Entry(character).CurrentValues.SetValues(updatedCharacter);
    await context.SaveChangesAsync();
    
    return Results.Ok(updatedCharacter);
})
.WithName("UpdateCharacter")
.WithTags("Characters");

app.MapDelete("/api/characters/{id:int}", async (int id, DragonBallContext context, IBlobStorageService blobService) =>
{
    var character = await context.Characters.FindAsync(id);
    if (character is null)
        return Results.NotFound();

    context.Characters.Remove(character);
    await context.SaveChangesAsync();
    
    // Clean up associated blob storage
    _ = Task.Run(async () =>
    {
        try
        {
            await blobService.DeleteCharacterImageAsync(character.Name);
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Failed to cleanup blob storage for character {CharacterName}", character.Name);
        }
    });
    
    return Results.NoContent();
})
.WithName("DeleteCharacter")
.WithTags("Characters");

// Configuration endpoints using Azure App Configuration
app.MapGet("/api/config/{key}", async (string key, IConfigurationService configService) =>
{
    try
    {
        var value = await configService.GetSettingAsync(key);
        return Results.Ok(new { Key = key, Value = value });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error retrieving configuration: {ex.Message}");
    }
})
.WithName("GetConfiguration")
.WithTags("Configuration");

// Health check endpoint
app.MapGet("/health", async (DragonBallContext context) =>
{
    try
    {
        // Check database connectivity
        await context.Database.CanConnectAsync();
        
        return Results.Ok(new { 
            Status = "Healthy", 
            Timestamp = DateTime.UtcNow,
            Services = new { 
                Database = "Connected", 
                AzureAppConfiguration = "Simulated",
                AzureKeyVault = "Simulated",
                AzureBlobStorage = "Simulated"
            }
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Health check failed: {ex.Message}");
    }
})
.WithName("HealthCheck")
.WithTags("Health");

app.MapDefaultEndpoints();

app.Run();

public record DragonBallCharacter(
    int Id,
    string Name,
    string Race,
    string Planet,
    string Transformation,
    string Technique,
    string? ImageUrl = null
);

public record CreateCharacterRequest(
    string Name,
    string Race,
    string Planet,
    string Transformation,
    string Technique,
    string? ImageUrl = null
);

public record UpdateCharacterRequest(
    string Name,
    string Race,
    string Planet,
    string Transformation,
    string Technique,
    string? ImageUrl = null
);
