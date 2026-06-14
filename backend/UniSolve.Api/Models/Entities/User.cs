using UniSolve.Api.Models.Enums;

namespace UniSolve.Api.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
