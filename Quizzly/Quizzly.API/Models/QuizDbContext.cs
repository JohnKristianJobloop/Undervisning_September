using Microsoft.EntityFrameworkCore;

namespace Quizzly.API.Models;

// DbContext er EF Core sin inngang til databasen: den vet hvilke tabeller vi har og hvordan de ser ut.
public sealed class QuizDbContext (DbContextOptions<QuizDbContext> options): DbContext(options)
{
    // Tabellen med spørsmål. Spørringer mot denne blir oversatt til SQL.
    public DbSet<QuestionEntity> Questions => Set<QuestionEntity>();

    // Her beskriver vi tabellen: navn, påkrevde felt og indekser.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuestionEntity>(question =>
        {
            question.ToTable("Questions");

            question.Property(q => q.Text).IsRequired();
            question.Property(q => q.Category).IsRequired();

            // Unik indeks: databasen selv nekter to spørsmål med samme (normaliserte) tekst.
            question.HasIndex(q => q.NormalizedText).IsUnique();
        });
    }
}
