namespace UniSolve.Api.Models.Entities;

public class Solution
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Problem Problem { get; set; } = null!;
    public User Author { get; set; } = null!;
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
