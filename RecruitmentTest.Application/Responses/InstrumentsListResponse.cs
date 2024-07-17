namespace RecruitmentTest.Application.Responses
{
    public class InstrumentsListResponse
    {
        public PagingResponse Paging { get; set; } = null!;
        public List<InstrumentResponse>? Data { get; set; }
    }

    public class PagingResponse
    {
        public int Page { get; set; }
        public int Pages { get; set; }
        public int Items { get; set; }
    }

    public class InstrumentResponse
    {
        public Guid Id { get; set; }
        public string? Symbol { get; set; }
        public string? Kind { get; set; }
        public string? Description { get; set; }
        public decimal TickSize { get; set; }
        public string? Currency { get; set; }
        public string? BaseCurrency { get; set; }
        public Dictionary<string, MappingResponse> Mappings { get; set; } = [];
    }

    public class MappingResponse
    {
        public string? Symbol { get; set; }
        public string? Exchange { get; set; }
        public int DefaultOrderSize { get; set; }
    }
}
