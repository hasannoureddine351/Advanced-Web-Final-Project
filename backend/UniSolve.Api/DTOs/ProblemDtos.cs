using System.ComponentModel.DataAnnotations;

namespace UniSolve.Api.DTOs;

public class ProblemListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? BookReference { get; set; }
    public string? ProblemNumber { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string AuthorUsername { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public int SolutionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProblemDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? BookReference { get; set; }
    public string? ProblemNumber { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<SolutionDto> Solutions { get; set; } = new();
}

public class CreateProblemRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? BookReference { get; set; }

    [MaxLength(50)]
    public string? ProblemNumber { get; set; }

    [Required]
    public int SubjectId { get; set; }
}

public class UpdateProblemRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? BookReference { get; set; }

    [MaxLength(50)]
    public string? ProblemNumber { get; set; }

    [Required]
    public int SubjectId { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
