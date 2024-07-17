
namespace RecruitmentTest.Application.Responses
{
    public class CountBackListResponse
    {
        public List<CountBackResponse>? Data { get; set; }
    }

    public class CountBackResponse
    {
        public DateTime? t { get; set; }
        public decimal? o { get; set; }
        public decimal? h { get; set; }
        public decimal? l { get; set; }
        public decimal? c { get; set; }
        public int? v { get; set; }
    }
}

