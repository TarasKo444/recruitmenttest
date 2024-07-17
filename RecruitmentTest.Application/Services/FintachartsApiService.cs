using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using RecruitmentTest.Application.Responses;
using RecruitmentTest.Common;

namespace RecruitmentTest.Application.Services;

public class FintachartsApiService
{
    private readonly IOptions<ApiOptions> _options;
    private readonly AuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;

    public FintachartsApiService(AuthService authService, IHttpClientFactory httpClientFactory, IOptions<ApiOptions> options)
    {
        _authService = authService;
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public async Task<KindsListResponse> GetKindsAsync()
    {
        var credentials = await _authService.GetCredentialsAsync();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            new Uri($"{_options.Value.URI}/api/instruments/v1/instruments/kinds/"));

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.AccessToken);
        
        var response = await SendRequestAsync(httpRequestMessage);
        var kinds = await response.Content.ReadFromJsonAsync<KindsListResponse>();

        return kinds!;
    }
    
    public async Task<ExchangeListResponse> GetExchangesAsync()
    {
        var credentials = await _authService.GetCredentialsAsync();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            new Uri($"{_options.Value.URI}/api/instruments/v1/exchanges"));

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.AccessToken);
        
        var response = await SendRequestAsync(httpRequestMessage);
        var exchanges = await response.Content.ReadFromJsonAsync<ExchangeListResponse>();

        return exchanges!;
    }
    
    public async Task<ProvidersListResponse> GetProvidersAsync()
    {
        var credentials = await _authService.GetCredentialsAsync();
        
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            $"{_options.Value.URI}/api/instruments/v1/providers");

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.AccessToken);
        
        var response = await SendRequestAsync(httpRequestMessage);
        var providers = await response.Content.ReadFromJsonAsync<ProvidersListResponse>();

        return providers!;
    }

    public async Task<InstrumentsListResponse> GetInstrumentsAsync(string? provider, string? kind, string? symbol, int? page)
    {
        var credentials = await _authService.GetCredentialsAsync();

        var queryParams = new List<string>();

        if(!string.IsNullOrEmpty(provider)) queryParams.Add($"provider={provider}");
        if(!string.IsNullOrEmpty(kind)) queryParams.Add($"kind={kind}");
        if (!string.IsNullOrEmpty(symbol)) queryParams.Add($"symbol={symbol}");

        if (page.HasValue) queryParams.Add($"page={page.Value}");

        var queryString = string.Join("&", queryParams);
        var requestUri = $"{_options.Value.URI}/api/instruments/v1/instruments?{queryString}";

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);


        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.AccessToken);
        
        var response = await SendRequestAsync(httpRequestMessage);
        var instruments = await response.Content.ReadFromJsonAsync<InstrumentsListResponse>();

        return instruments!;
    }

    public async Task<CountBackListResponse> GetCountBackAsync(string instrumentId, string provider, int interval, string periodicity, int barsCount)
    {
        var credentials = await _authService.GetCredentialsAsync();

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            $"{_options.Value.URI}/api/bars/v1/bars/count-back?instrumentId={instrumentId}&provider={provider}&interval={interval}&periodicity={periodicity}&barsCount={barsCount}");

        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.AccessToken);

        var response = await SendRequestAsync(httpRequestMessage);
        var countBack = await response.Content.ReadFromJsonAsync<CountBackListResponse>();

        return countBack!;
    }

    public async Task<CountBackListResponse> GetDateRangeAsync(string instrumentId, string provider, int interval, string periodicity, string startDate, string? endDate)
    {
        try
        {
            var credentials = await _authService.GetCredentialsAsync();

            var queryParams = new List<string>
        {
            $"instrumentId={instrumentId}",
            $"provider={provider}",
            $"interval={interval}",
            $"periodicity={periodicity}",
            $"startDate={startDate}"
        };

            if (!string.IsNullOrEmpty(endDate))
            {
                queryParams.Add($"endDate={endDate}");
            }

            var queryString = string.Join("&", queryParams);
            var requestUri = $"{_options.Value.URI}/api/bars/v1/bars/date-range?{queryString}";

            // Add logging to debug the constructed URL
            Console.WriteLine($"Request URI: {requestUri}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.AccessToken);

            var response = await SendRequestAsync(httpRequestMessage);
            var dateRange = await response.Content.ReadFromJsonAsync<CountBackListResponse>();

            return dateRange!;
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage httpRequestMessage)
    {
        using var client = _httpClientFactory.CreateClient();
        var response = await client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        return response;
    }


}

public class ExchangeListResponse
{
    public Dictionary<string, List<string>> Data { get; set; } = null!;
}