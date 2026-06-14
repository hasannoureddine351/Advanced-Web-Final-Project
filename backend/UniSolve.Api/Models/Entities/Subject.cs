namespace UniSolve.Api.Models.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
}
