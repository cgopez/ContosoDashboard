using System.Net.Http;

namespace ContosoDashboard.Services;

public class DocumentService
{
    private readonly HttpClient _http;

    public DocumentService(HttpClient http)
    {
        _http = http;
    }

    public Task<HttpResponseMessage> UploadAsync(MultipartFormDataContent content)
    {
        return _http.PostAsync("/api/documents", content);
    }

    public Task<HttpResponseMessage> GetListAsync(string query = "")
    {
        return _http.GetAsync($"/api/documents{query}");
    }
}
