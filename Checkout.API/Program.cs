using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("PaymentLinkClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5006");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient("CatalogClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5004");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();