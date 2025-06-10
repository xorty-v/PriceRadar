using System.Diagnostics;
using PriceRadar.Api.Workers;
using PriceRadar.Application;
using PriceRadar.Application.Abstractions;
using PriceRadar.Infrastructure;
using PriceRadar.Parsers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddApplication();
services.AddInfrastructure(configuration);
services.AddParsers();
services.AddControllers();

services.AddHostedService<ChromiumInitializerService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/run-parsers", async (IParserService parserService) =>
{
    var stopwatch = Stopwatch.StartNew();

    await parserService.RunAllParsers();

    stopwatch.Stop();

    return Results.Ok($"Parsing completed in {stopwatch.Elapsed:hh\\:mm\\:ss}.");
});

app.MapControllers();

app.Run();