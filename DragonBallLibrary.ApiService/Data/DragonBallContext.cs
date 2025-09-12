using Microsoft.EntityFrameworkCore;

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

        // Seed data
        modelBuilder.Entity<DragonBallCharacter>().HasData(
            new DragonBallCharacter(1, "Goku", "Saiyan", "Earth", "Ultra Instinct", "Kamehameha"),
            new DragonBallCharacter(2, "Vegeta", "Saiyan", "Vegeta", "Super Saiyan Blue Evolution", "Final Flash"),
            new DragonBallCharacter(3, "Piccolo", "Namekian", "Namek", "Orange Piccolo", "Special Beam Cannon"),
            new DragonBallCharacter(4, "Gohan", "Half-Saiyan", "Earth", "Beast", "Masenko"),
            new DragonBallCharacter(5, "Frieza", "Frost Demon", "Unknown", "Black Frieza", "Death Ball")
        );
    }
}