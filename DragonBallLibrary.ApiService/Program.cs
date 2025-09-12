var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");

app.UseAuthorization();

// Dragon Ball Characters endpoints
var characters = new List<DragonBallCharacter>
{
    new(1, "Goku", "Saiyan", "Earth", "Ultra Instinct", "Kamehameha"),
    new(2, "Vegeta", "Saiyan", "Vegeta", "Super Saiyan Blue Evolution", "Final Flash"),
    new(3, "Piccolo", "Namekian", "Namek", "Orange Piccolo", "Special Beam Cannon"),
    new(4, "Gohan", "Half-Saiyan", "Earth", "Beast", "Masenko"),
    new(5, "Frieza", "Frost Demon", "Unknown", "Black Frieza", "Death Ball")
};

app.MapGet("/api/characters", () => Results.Ok(characters))
    .WithName("GetCharacters");

app.MapGet("/api/characters/{id:int}", (int id) =>
{
    var character = characters.FirstOrDefault(c => c.Id == id);
    return character is not null ? Results.Ok(character) : Results.NotFound();
})
.WithName("GetCharacter");

app.MapPost("/api/characters", (CreateCharacterRequest request) =>
{
    var newId = characters.Max(c => c.Id) + 1;
    var character = new DragonBallCharacter(
        newId,
        request.Name,
        request.Race,
        request.Planet,
        request.Transformation,
        request.Technique
    );
    characters.Add(character);
    return Results.Created($"/api/characters/{newId}", character);
})
.WithName("CreateCharacter");

app.MapPut("/api/characters/{id:int}", (int id, UpdateCharacterRequest request) =>
{
    var character = characters.FirstOrDefault(c => c.Id == id);
    if (character is null)
        return Results.NotFound();

    var updatedCharacter = character with
    {
        Name = request.Name,
        Race = request.Race,
        Planet = request.Planet,
        Transformation = request.Transformation,
        Technique = request.Technique
    };

    var index = characters.IndexOf(character);
    characters[index] = updatedCharacter;
    
    return Results.Ok(updatedCharacter);
})
.WithName("UpdateCharacter");

app.MapDelete("/api/characters/{id:int}", (int id) =>
{
    var character = characters.FirstOrDefault(c => c.Id == id);
    if (character is null)
        return Results.NotFound();

    characters.Remove(character);
    return Results.NoContent();
})
.WithName("DeleteCharacter");

app.MapDefaultEndpoints();

app.Run();

public record DragonBallCharacter(
    int Id,
    string Name,
    string Race,
    string Planet,
    string Transformation,
    string Technique
);

public record CreateCharacterRequest(
    string Name,
    string Race,
    string Planet,
    string Transformation,
    string Technique
);

public record UpdateCharacterRequest(
    string Name,
    string Race,
    string Planet,
    string Transformation,
    string Technique
);
