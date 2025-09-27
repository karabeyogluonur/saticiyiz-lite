using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Serilog;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Domain.Entities;
using SL.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfigurations();
builder.Services.AddBaseServices();
builder.Services.AddLayerServices();
builder.Services.AddAuthServices();
builder.Services.AddLogging();


var app = builder.Build();

// Add middlewares

app.DatabaseSeedAsync();
app.AddDevelopmentBuilder();
app.AddBaseBuilder();
app.AddRouteBuilder();
app.AddAuthBuilder();
app.UseSpecialRoute();
app.UseSerilogRequestLogging();
app.Run();

