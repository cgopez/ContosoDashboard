using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DocumentFeature.Tests;

public class DocumentUploadTests : IClassFixture<WebApplicationFactory<ContosoDashboard.Program>>
{
    private readonly WebApplicationFactory<ContosoDashboard.Program> _factory;

    public DocumentUploadTests(WebApplicationFactory<ContosoDashboard.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Upload_ReturnsAccepted_ForValidFile()
    {
        var client = _factory.CreateClient();

        using var content = new MultipartFormDataContent();
        var bytes = new byte[10];
        content.Add(new ByteArrayContent(bytes), "files", "test.txt");

        var resp = await client.PostAsync("/api/documents", content);

        Assert.True(resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.Accepted);
    }
}
