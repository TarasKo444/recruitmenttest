using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using RecruitmentTest.Common;

namespace RecruitmentTest.Application.Services;

public class AuthService
{
    private readonly IOptions<ApiOptions> _options;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthService(IOptions<ApiOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AuthResponse> GetCredentialsAsync()
    {
        using var client = _httpClientFactory.CreateClient();
        
        var content = new FormUrlEncodedContent([
            new("username", _options.Value.USERNAME),
            new("password", _options.Value.PASSWORD),
            new("grant_type", "password"),
            new("client_id", "app-cli"),
        ]);
        
        var response = await client.PostAsync(
            $"{_options.Value.URI}/identity/realms/fintatech/protocol/openid-connect/token",
            content);
        

        response.EnsureSuccessStatusCode();
        
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!;
    }
}

public class AuthResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}