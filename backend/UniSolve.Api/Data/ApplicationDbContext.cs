using Microsoft.EntityFrameworkCore;
using UniSolve.Api.Models.Entities;

namespace UniSolve.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Problem> Problems => Set<Problem>();
    public DbSet<Solution> Solutions => Set<Solution>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasIndex(s => s.Code).IsUnique();
        });

        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasOne(p => p.Subject)
                .WithMany(s => s.Problems)
                .HasForeignKey(p => p.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Author)
                .WithMany(u => u.Problems)
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Solution>(entity =>
        {
            entity.HasOne(s => s.Problem)
                .WithMany(p => p.Solutions)
                .HasForeignKey(s => s.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Author)
                .WithMany(u => u.Solutions)
                .HasForeignKey(s => s.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasIndex(v => new { v.SolutionId, v.UserId }).IsUnique();

            entity.HasOne(v => v.Solution)
                .WithMany(s => s.Votes)
                .HasForeignKey(v => v.SolutionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
