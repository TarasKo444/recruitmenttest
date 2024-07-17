namespace RecruitmentTask.Domain.Entities;

public class Provider
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Exchange> Exchanges { get; set; } = [];
}
