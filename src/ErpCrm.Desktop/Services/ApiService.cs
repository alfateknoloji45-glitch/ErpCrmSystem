using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ErpCrm.Desktop.Services;

/// <summary>
/// API iletişim servisi
/// Tüm HTTP isteklerini yönetir
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;
    private string? _authToken;
    private int _tenantId;

    /// <summary>
    /// API base URL (geliştirme için)
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5000";

    /// <summary>
    /// Yeni ApiService oluşturur
    /// </summary>
    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Kimlik doğrulama bilgilerini ayarlar
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="tenantId">Firma ID</param>
    public void SetAuth(string token, int tenantId)
    {
        _authToken = token;
        _tenantId = tenantId;

        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        // X-Tenant-Id header'ını ekle
        if (_httpClient.DefaultRequestHeaders.Contains("X-Tenant-Id"))
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Tenant-Id");
        }
        _httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId.ToString());
    }

    /// <summary>
    /// Kimlik doğrulama bilgilerini temizler
    /// </summary>
    public void ClearAuth()
    {
        _authToken = null;
        _tenantId = 0;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        if (_httpClient.DefaultRequestHeaders.Contains("X-Tenant-Id"))
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Tenant-Id");
        }
    }

    /// <summary>
    /// HTTP GET isteği yapar
    /// </summary>
    /// <typeparam name="T">Dönüş tipi</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <returns>Yanıt verisi</returns>
    public async Task<T?> GetAsync<T>(string endpoint) where T : class
    {
        try
        {
            var url = $"{BaseUrl}/api/{endpoint}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// HTTP POST isteği yapar
    /// </summary>
    /// <typeparam name="TRequest">İstek tipi</typeparam>
    /// <typeparam name="TResponse">Yanıt tipi</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="data">Gönderilecek veri</param>
    /// <returns>Yanıt verisi</returns>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) 
        where TResponse : class
    {
        try
        {
            var url = $"{BaseUrl}/api/{endpoint}";
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// HTTP PUT isteği yapar
    /// </summary>
    /// <typeparam name="TRequest">İstek tipi</typeparam>
    /// <typeparam name="TResponse">Yanıt tipi</typeparam>
    /// <param name="endpoint">API endpoint</param>
    /// <param name="data">Gönderilecek veri</param>
    /// <returns>Yanıt verisi</returns>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) 
        where TResponse : class
    {
        try
        {
            var url = $"{BaseUrl}/api/{endpoint}";
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>();
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// HTTP DELETE isteği yapar
    /// </summary>
    /// <param name="endpoint">API endpoint</param>
    /// <returns>Başarılı ise true</returns>
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var url = $"{BaseUrl}/api/{endpoint}";
            var response = await _httpClient.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Login isteği yapar
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="password">Şifre</param>
    /// <returns>Login yanıtı</returns>
    public async Task<Models.LoginResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new Models.LoginRequest
            {
                Email = email,
                Password = password
            };

            return await PostAsync<Models.LoginRequest, Models.LoginResponse>("auth/login", request);
        }
        catch (Exception)
        {
            return null;
        }
    }
}