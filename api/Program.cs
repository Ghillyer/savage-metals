using System.Text.Json.Serialization;
using SavageMetals.Api;
using SavageMetals.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();
builder.Services.AddSavageMetalsServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();
app.UseCors(CorsSettings.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();