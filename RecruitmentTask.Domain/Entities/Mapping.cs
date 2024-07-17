namespace RecruitmentTask.Domain.Entities;

public class Mapping
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = null!;
    public Exchange? Exchange { get; set; }
    public Guid? ExchangeId { get; set; }
    public int DefaultOrderSize { get; set; }
    public Provider Provider { get; set; } = null!;
    public Guid ProviderId { get; set; }
}
