using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("FileStore", c =>
{
    c.BaseAddress = new Uri("http://filestore:80");
});
builder.Services.AddHttpClient("FileAnalysis", c =>
{
    c.BaseAddress = new Uri("http://analysis:80");
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();