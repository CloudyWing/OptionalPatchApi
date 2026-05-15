using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CloudyWing.OptionalPatchApi.Tests;

[TestFixture]
internal sealed class ProductPatchApiTests {
    private WebApplicationFactory<Program>? factory;

    [SetUp]
    public void SetUp() {
        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Development"));
    }

    [TearDown]
    public void TearDown() {
        factory?.Dispose();
    }

    [Test]
    public async Task PatchBody_WhenNoFieldsSupplied_DoesNotChangeProductAndSkipsRequiredValidationAsync() {
        HttpClient client = CreateClient();
        using StringContent content = CreateJsonContent("{}");

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/body", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        using (Assert.EnterMultipleScope()) {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(GetChangedFields(document.RootElement), Is.Empty);
            Assert.That(GetProduct(document.RootElement).GetProperty("name").GetString(), Is.EqualTo("Notebook Pro"));
            Assert.That(GetProduct(document.RootElement).GetProperty("description").GetString(), Is.EqualTo("Initial description"));
            Assert.That(GetProduct(document.RootElement).GetProperty("price").GetDecimal(), Is.EqualTo(42000m));
        }
    }

    [Test]
    public async Task PatchBody_WhenRequiredNameOmitted_UpdatesOnlySuppliedFieldsAsync() {
        HttpClient client = CreateClient();
        using StringContent content = CreateJsonContent("""{ "description": "Updated description" }""");

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/body", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        using (Assert.EnterMultipleScope()) {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(GetChangedFields(document.RootElement), Is.EquivalentTo(new[] { "Description" }));
            Assert.That(GetProduct(document.RootElement).GetProperty("name").GetString(), Is.EqualTo("Notebook Pro"));
            Assert.That(GetProduct(document.RootElement).GetProperty("description").GetString(), Is.EqualTo("Updated description"));
        }
    }

    [Test]
    public async Task PatchBody_WhenNameSupplied_UpdatesOnlyNameAsync() {
        HttpClient client = CreateClient();
        using StringContent content = CreateJsonContent("""{ "name": "Notebook Air" }""");

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/body", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        using (Assert.EnterMultipleScope()) {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(GetChangedFields(document.RootElement), Is.EquivalentTo(new[] { "Name" }));
            Assert.That(GetProduct(document.RootElement).GetProperty("name").GetString(), Is.EqualTo("Notebook Air"));
            Assert.That(GetProduct(document.RootElement).GetProperty("description").GetString(), Is.EqualTo("Initial description"));
            Assert.That(GetProduct(document.RootElement).GetProperty("price").GetDecimal(), Is.EqualTo(42000m));
        }
    }

    [Test]
    public async Task PatchBody_WhenNullableFieldSuppliedAsNull_ClearsDescriptionAsync() {
        HttpClient client = CreateClient();
        using StringContent content = CreateJsonContent("""{ "description": null }""");

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/body", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        using (Assert.EnterMultipleScope()) {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(GetChangedFields(document.RootElement), Is.EquivalentTo(new[] { "Description" }));
            Assert.That(GetProduct(document.RootElement).GetProperty("description").ValueKind, Is.EqualTo(JsonValueKind.Null));
        }
    }

    [TestCase("""{ "name": null }""", "Name")]
    [TestCase("""{ "name": "" }""", "Name")]
    [TestCase("""{ "price": 0 }""", "Price")]
    public async Task PatchBody_WhenSuppliedFieldViolatesValidation_ReturnsValidationProblemAsync(
        string requestJson,
        string fieldName
    ) {
        HttpClient client = CreateClient();
        using StringContent content = CreateJsonContent(requestJson);

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/body", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        AssertValidationProblem(response, document.RootElement, fieldName);
    }

    [Test]
    public async Task PatchForm_WhenRequiredNameOmitted_UpdatesOnlySuppliedFieldsAsync() {
        HttpClient client = CreateClient();
        using FormUrlEncodedContent content = new(new Dictionary<string, string> {
            [ "price" ] = "39999.5",
            [ "discontinuedOn" ] = "2026-01-31"
        });

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/form", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        using (Assert.EnterMultipleScope()) {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(GetChangedFields(document.RootElement), Is.EquivalentTo(new[] { "Price", "DiscontinuedOn" }));
            Assert.That(GetProduct(document.RootElement).GetProperty("name").GetString(), Is.EqualTo("Notebook Pro"));
            Assert.That(GetProduct(document.RootElement).GetProperty("price").GetDecimal(), Is.EqualTo(39999.5m));
            Assert.That(GetProduct(document.RootElement).GetProperty("discontinuedOn").GetString(), Is.EqualTo("2026-01-31"));
        }
    }

    [Test]
    public async Task PatchForm_WhenNullableDateFieldSuppliedAsEmpty_ClearsDiscontinuedOnAsync() {
        HttpClient client = CreateClient();
        using FormUrlEncodedContent initialContent = new(new Dictionary<string, string> {
            [ "discontinuedOn" ] = "2026-01-31"
        });

        using HttpResponseMessage initialResponse = await client.PatchAsync("/api/products/1/form", initialContent);
        using FormUrlEncodedContent clearContent = new(new Dictionary<string, string> {
            [ "discontinuedOn" ] = ""
        });

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/form", clearContent);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        using (Assert.EnterMultipleScope()) {
            Assert.That(initialResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(GetChangedFields(document.RootElement), Is.EquivalentTo(new[] { "DiscontinuedOn" }));
            Assert.That(GetProduct(document.RootElement).GetProperty("discontinuedOn").ValueKind, Is.EqualTo(JsonValueKind.Null));
        }
    }

    [TestCase("name", "", "Name")]
    [TestCase("price", "0", "Price")]
    [TestCase("price", "abc", "Price")]
    public async Task PatchForm_WhenSuppliedFieldViolatesBindingOrValidation_ReturnsValidationProblemAsync(
        string field,
        string value,
        string fieldName
    ) {
        HttpClient client = CreateClient();
        using FormUrlEncodedContent content = new(new Dictionary<string, string> {
            [ field ] = value
        });

        using HttpResponseMessage response = await client.PatchAsync("/api/products/1/form", content);
        using JsonDocument document = await ReadJsonDocumentAsync(response);

        AssertValidationProblem(response, document.RootElement, fieldName);
    }

    [Test]
    public async Task OpenApiDocument_WhenGenerated_DescribesPatchFieldsWithoutOptionalValueWrapperAsync() {
        HttpClient client = CreateClient();

        string openApiJson = await client.GetStringAsync("/openapi/v1.json");
        using JsonDocument document = JsonDocument.Parse(openApiJson);
        JsonElement root = document.RootElement;
        IReadOnlyCollection<string> bodyFields = GetRequestBodySchemaProperties(
            root, "/api/products/{id}/body", "application/json"
        );
        IReadOnlyCollection<string> formFields = GetRequestBodySchemaProperties(
            root, "/api/products/{id}/form", "application/x-www-form-urlencoded"
        );

        using (Assert.EnterMultipleScope()) {
            AssertPatchFieldSchema(bodyFields);
            AssertPatchFieldSchema(formFields);
            Assert.That(FindPropertyNames(root, "hasValue"), Is.Empty);
            Assert.That(FindPropertyNames(root, "value"), Is.Empty);
            Assert.That(openApiJson, Does.Not.Contain(".HasValue"));
            Assert.That(openApiJson, Does.Not.Contain(".Value"));
        }
    }

    private HttpClient CreateClient() {
        return factory?.CreateClient() ?? throw new InvalidOperationException("Factory is not initialized.");
    }

    private static async Task<JsonDocument> ReadJsonDocumentAsync(HttpResponseMessage response) {
        JsonDocument? document = await response.Content.ReadFromJsonAsync<JsonDocument>();
        return document ?? throw new InvalidOperationException("Response body is not a JSON document.");
    }

    private static StringContent CreateJsonContent(string json) {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static JsonElement GetProduct(JsonElement root) {
        return root.GetProperty("product");
    }

    private static IReadOnlyList<string?> GetChangedFields(JsonElement root) {
        return root.GetProperty("changedFields")
            .EnumerateArray()
            .Select(field => field.GetString())
            .ToArray();
    }

    private static void AssertValidationProblem(
        HttpResponseMessage response,
        JsonElement problemDetails,
        string fieldName
    ) {
        using (Assert.EnterMultipleScope()) {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(problemDetails.GetProperty("status").GetInt32(), Is.EqualTo((int)HttpStatusCode.BadRequest));
            Assert.That(problemDetails.TryGetProperty("errors", out JsonElement _), Is.True);
            Assert.That(HasValidationError(problemDetails.GetProperty("errors"), fieldName), Is.True);
        }
    }

    private static bool HasValidationError(JsonElement errors, string fieldName) {
        foreach (JsonProperty error in errors.EnumerateObject()) {
            if (error.Name.Contains(fieldName, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyCollection<string> GetRequestBodySchemaProperties(
        JsonElement openApi,
        string path,
        string contentType
    ) {
        JsonElement schema = openApi
            .GetProperty("paths")
            .GetProperty(path)
            .GetProperty("patch")
            .GetProperty("requestBody")
            .GetProperty("content")
            .GetProperty(contentType)
            .GetProperty("schema");
        JsonElement resolvedSchema = ResolveSchema(openApi, schema);

        return resolvedSchema
            .GetProperty("properties")
            .EnumerateObject()
            .Select(property => property.Name)
            .ToArray();
    }

    private static JsonElement ResolveSchema(JsonElement openApi, JsonElement schema) {
        if (!schema.TryGetProperty("$ref", out JsonElement reference)) {
            return schema;
        }

        string referencePath = reference.GetString() ?? throw new InvalidOperationException("Schema reference is empty.");
        string[] segments = referencePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        JsonElement current = openApi;

        foreach (string segment in segments.Skip(1)) {
            current = current.GetProperty(segment);
        }

        return current;
    }

    private static void AssertPatchFieldSchema(IReadOnlyCollection<string> fields) {
        Assert.That(fields, Does.Contain("name"));
        Assert.That(fields, Does.Contain("description"));
        Assert.That(fields, Does.Contain("price"));
        Assert.That(fields, Does.Contain("discontinuedOn"));
    }

    private static IReadOnlyList<string> FindPropertyNames(JsonElement element, string propertyName) {
        List<string> matches = [];
        CollectPropertyNames(element, propertyName, matches);
        return matches;
    }

    private static void CollectPropertyNames(JsonElement element, string propertyName, ICollection<string> matches) {
        if (element.ValueKind == JsonValueKind.Object) {
            foreach (JsonProperty property in element.EnumerateObject()) {
                if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) {
                    matches.Add(property.Name);
                }

                CollectPropertyNames(property.Value, propertyName, matches);
            }

            return;
        }

        if (element.ValueKind == JsonValueKind.Array) {
            foreach (JsonElement item in element.EnumerateArray()) {
                CollectPropertyNames(item, propertyName, matches);
            }
        }
    }
}
