using System.ComponentModel.DataAnnotations;

namespace UniSolve.Api.DTOs;

public class SolutionDto
{
    public int Id { get; set; }
    public int ProblemId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int? UserVote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSolutionRequest
{
    [Required, MinLength(10)]
    public string Content { get; set; } = string.Empty;
}

public class UpdateSolutionRequest
{
    [Required, MinLength(10)]
    public string Content { get; set; } = string.Empty;
}

public class VoteRequest
{
    [Required]
    [Range(-1, 1)]
    public int Value { get; set; }
}
