namespace UniSolve.Api.Models.Entities;

public class Vote
{
    public int Id { get; set; }
    public int SolutionId { get; set; }
    public int UserId { get; set; }
    public int Value { get; set; }

    public Solution Solution { get; set; } = null!;
    public User User { get; set; } = null!;
}
