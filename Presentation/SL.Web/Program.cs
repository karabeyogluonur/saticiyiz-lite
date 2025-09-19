using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using SL.Application.Interfaces.Repositories.UnitOfWork;
using SL.Domain.Entities;
using SL.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBaseServices();
builder.Services.AddLayerServices();
builder.Services.AddAuthServices();




var app = builder.Build();

// Add middlewares
app.DatabaseSeed();
app.AddDevelopmentBuilder();
app.AddBaseBuilder();
app.AddRouteBuilder();
app.AddAuthBuilder();
app.UseSpecialRoute();
app.Run();

