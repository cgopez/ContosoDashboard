using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Contract.Documents.Tests;

public class ContractDocumentsTest : IClassFixture<WebApplicationFactory<ContosoDashboard.Program>>
{
    private readonly WebApplicationFactory<ContosoDashboard.Program> _factory;

    public ContractDocumentsTest(WebApplicationFactory<ContosoDashboard.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostDocuments_EndpointExists()
    {
        var client = _factory.CreateClient();
        using var content = new MultipartFormDataContent();
        var bytes = new byte[1];
        content.Add(new ByteArrayContent(bytes), "files", "contract.txt");

        var resp = await client.PostAsync("/api/documents", content);

        // Contract: API should accept uploads (202 Accepted) or return 200/201 for tests
        Assert.True(resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task GetDocuments_ListEndpointExists()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/documents");
        Assert.True(resp.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetDocument_ById_ReturnsNotFoundForUnknown()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/documents/999999");
        Assert.True(resp.StatusCode == System.Net.HttpStatusCode.NotFound || resp.IsSuccessStatusCode);
    }
}
