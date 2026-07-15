using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/", () => Results.Redirect("Scalar/#tag/gym-boocontrollers"));

app.Run();