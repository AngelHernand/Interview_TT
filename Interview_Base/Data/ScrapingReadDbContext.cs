using Microsoft.EntityFrameworkCore;

namespace Interview_Base.Data;

/// <summary>
/// DbContext de solo lectura que apunta a la base InterviewSimulator del Scraping_module.
/// Mapea únicamente las tablas necesarias para leer preguntas behavioral.
/// </summary>
public class ScrapingReadDbContext : DbContext
{
    public ScrapingReadDbContext(DbContextOptions<ScrapingReadDbContext> options)
        : base(options) { }

    public DbSet<BehavioralQuestionEntity> BehavioralQuestions { get; set; }
    public DbSet<EvaluationCriteriaEntity> EvaluationCriteria { get; set; }
    public DbSet<RedFlagEntity> RedFlags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BehavioralQuestionEntity>(entity =>
        {
            entity.ToTable("BehavioralQuestions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionText).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Competency).HasMaxLength(200).IsRequired();
            entity.Property(e => e.EvaluationMethod).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Difficulty).HasMaxLength(20).IsRequired();

            entity.HasMany(e => e.EvaluationCriteria)
                  .WithOne()
                  .HasForeignKey(e => e.BehavioralQuestionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.RedFlags)
                  .WithOne()
                  .HasForeignKey(e => e.BehavioralQuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EvaluationCriteriaEntity>(entity =>
        {
            entity.ToTable("EvaluationCriteria");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CriteriaText).IsRequired();
        });

        modelBuilder.Entity<RedFlagEntity>(entity =>
        {
            entity.ToTable("RedFlags");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FlagText).IsRequired();
        });
    }
}

// Entidades locales mapeadas a las tablas del Scraping_module (solo lectura)

public class BehavioralQuestionEntity
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Competency { get; set; } = string.Empty;
    public string EvaluationMethod { get; set; } = "STAR";
    public string Difficulty { get; set; } = "Junior";
    public bool IsActive { get; set; }
    public ICollection<EvaluationCriteriaEntity> EvaluationCriteria { get; set; } = new List<EvaluationCriteriaEntity>();
    public ICollection<RedFlagEntity> RedFlags { get; set; } = new List<RedFlagEntity>();
}

public class EvaluationCriteriaEntity
{
    public int Id { get; set; }
    public int BehavioralQuestionId { get; set; }
    public string CriteriaText { get; set; } = string.Empty;
    public int Weight { get; set; }
    public int OrderIndex { get; set; }
}

public class RedFlagEntity
{
    public int Id { get; set; }
    public int BehavioralQuestionId { get; set; }
    public string FlagText { get; set; } = string.Empty;
}
