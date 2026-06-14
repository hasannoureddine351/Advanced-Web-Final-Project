namespace UniSolve.Api.Models.Entities;

public class Problem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? BookReference { get; set; }
    public string? ProblemNumber { get; set; }
    public int SubjectId { get; set; }
    public int AuthorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Subject Subject { get; set; } = null!;
    public User Author { get; set; } = null!;
    public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
}
