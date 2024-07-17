namespace RecruitmentTask.Domain.Entities;

public class Instrument
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal TickSize { get; set; }
    public string Currency { get; set; } = null!;
    public string? BaseCurrency { get; set; } = null;

    public ICollection<Mapping> Mappings { get; set; } = [];

    public Kind Kind { get; set; } = null!;
    public Guid KindId { get; set; }
}
