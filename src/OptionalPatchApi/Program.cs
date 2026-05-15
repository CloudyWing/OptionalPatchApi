using CloudyWing.OptionalPatchApi.OpenApi;
using CloudyWing.OptionalPatchApi.OptionalValues;
using CloudyWing.OptionalPatchApi.Products;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options => {
        options.ModelBinderProviders.Insert(0, new OptionalValueModelBinderProvider());
        options.ModelValidatorProviders.Insert(0, new OptionalValueModelValidatorProvider());
    })
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new OptionalValueJsonConverterFactory());
    });

builder.Services.AddSingleton<IProductStore, InMemoryProductStore>();
builder.Services.AddOpenApi(options => {
    options.AddSchemaTransformer<OptionalValueSchemaTransformer>();
    options.AddSchemaTransformer<JsonPropertyNameSchemaTransformer>();
    options.AddOperationTransformer<OptionalValueOperationTransformer>();
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
