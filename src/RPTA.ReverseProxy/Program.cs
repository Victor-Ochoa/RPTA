using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Yarp.ReverseProxy.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Yarp.ReverseProxy.Swagger.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RPTA API Gateway",
        Version = "v1",
        Description = "API Gateway para os servi�os RPTA"
    });
});

var configuration = builder.Configuration.GetSection("ReverseProxy");

builder.Services.AddReverseProxy()
    .LoadFromConfig(configuration)
    .AddSwagger(configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPTA API Gateway v1");
        c.RoutePrefix = "swagger";
    });
}

// Defina uma rota inicial simples para a raiz
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

// Configure YARP middleware
app.MapReverseProxy();

app.Run();