var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

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
builder.Services.AddScoped<IOutputBindingService, OutputBindingService>();

// Add CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
        //policy.WithOrigins("http://localhost:3000", "https://localhost:3001")
        //      .AllowAnyHeader()
        //      .AllowAnyMethod()
        //      .AllowCredentials();
    });
});

// Create Dapr client
var client = new DaprClientBuilder().Build();
builder.Configuration.AddDaprSecretStore(ComponentNames.SecretComponentName, client, TimeSpan.FromSeconds(20));
builder.Configuration.AddDaprConfigurationStore(ComponentNames.ConfigComponentName, new List<string>(), client, TimeSpan.FromSeconds(20));

// Add Entity Framework with SQL Server (using InMemory for demonstration)
builder.Services.AddDbContext<DragonBallContext>(options =>
{
    // In production, use SQL Server:
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // For demonstration, using InMemory database
    //options.UseInMemoryDatabase("DragonBallDb");
});

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DragonBallContext>();
    await context.Database.EnsureCreatedAsync();
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

app.MapPost("/api/characters", async (CreateCharacterRequest request, DragonBallContext context, 
                                            IBlobStorageService blobService, IOutputBindingService bindingService) =>
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

    var message = new MessageModel("New character created.");
    await bindingService.PublishMessageAsync<MessageModel>(message, ComponentNames.QueueComponentName, CancellationToken.None);

    return Results.Created($"/api/characters/{character.Id}", character);
})
.WithName("CreateCharacter")
.WithTags("Characters");

app.MapPut("/api/characters/{id:int}", async (int id, UpdateCharacterRequest request, DragonBallContext context,
                                                    IBlobStorageService blobService, IOutputBindingService bindingService) =>
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

    var message = new MessageModel($"Character {id} updated.");
    await bindingService.PublishMessageAsync<MessageModel>(message, ComponentNames.QueueComponentName, CancellationToken.None);

    return Results.Ok(updatedCharacter);
})
.WithName("UpdateCharacter")
.WithTags("Characters");

app.MapDelete("/api/characters/{id:int}", async (int id, DragonBallContext context, 
                                                        IBlobStorageService blobService, IOutputBindingService bindingService) =>
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

    var message = new MessageModel($"Character {id} deleted.");
    await bindingService.PublishMessageAsync<MessageModel>(message, ComponentNames.QueueComponentName, CancellationToken.None);

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

await app.RunAsync();

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
