
// File: src/BlazorAdmin/Services/ApiService.cs
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AjuaBank.Shared.DTOs;

namespace BlazorAdmin.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private string? _token;

    public ApiService(HttpClient http)
    {
        _http = http;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    public async Task<bool> LoginAsync(string email, string password)
    {
        var request = new LoginRequest(email, password);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("/api/auth/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _token = authResponse?.Token;
            return true;
        }

        return false;
    }

    public async Task<bool> RegisterAsync(string email, string password, string fullName)
    {
        var request = new RegisterRequest(email, password, fullName);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("/api/auth/register", content);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _token = authResponse?.Token;
            return true;
        }

        return false;
    }

    public async Task<List<TransactionResponse>> GetTransactionsAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/transactions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _http.SendAsync(request);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TransactionResponse>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<TransactionResponse>();
        }

        return new List<TransactionResponse>();
    }

    public async Task<bool> CreateTransactionAsync(string type, decimal amount, string? toAccount, string? description)
    {
        var request = new CreateTransactionRequest(type, amount, toAccount, description);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/transactions")
        {
            Content = content
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

        var response = await _http.SendAsync(httpRequest);
        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        _token = null;
    }
}