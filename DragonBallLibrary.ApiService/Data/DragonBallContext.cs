namespace DragonBallLibrary.ApiService.Data;

public class DragonBallContext : DbContext
{
    public DragonBallContext(DbContextOptions<DragonBallContext> options) : base(options)
    {
    }

    public DbSet<DragonBallCharacter> Characters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data with Azure Blob Storage URLs in the proper container structure
        modelBuilder.Entity<DragonBallCharacter>().HasData(
            //new DragonBallCharacter(1, "Goku", "Saiyan", "Earth", "Ultra Instinct", "Kamehameha", "https://storage5gvqxy7gckwwa.blob.core.windows.net/characters/goku/goku.jpg"),
            new DragonBallCharacter(1, "Goku", "Saiyan", "Earth", "Ultra Instinct", "Kamehameha", "http://127.0.0.1:54229/devstoreaccount1/characters/goku/goku.jpg"),
            new DragonBallCharacter(2, "Vegeta", "Saiyan", "Vegeta", "Super Saiyan Blue Evolution", "Final Flash", "https://storage5gvqxy7gckwwa.blob.core.windows.net/characters/vegeta/vegeta.jpg"),
            new DragonBallCharacter(3, "Piccolo", "Namekian", "Namek", "Orange Piccolo", "Special Beam Cannon", "https://storage5gvqxy7gckwwa.blob.core.windows.net/characters/piccolo/piccolo.jpg"),
            new DragonBallCharacter(4, "Gohan", "Half-Saiyan", "Earth", "Beast", "Masenko", "https://storage5gvqxy7gckwwa.blob.core.windows.net/characters/gohan/gohan.jpg"),
            new DragonBallCharacter(5, "Frieza", "Frost Demon", "Unknown", "Black Frieza", "Death Ball", "https://storage5gvqxy7gckwwa.blob.core.windows.net/characters/frieza/frieza.jpg")
        );
    }
}