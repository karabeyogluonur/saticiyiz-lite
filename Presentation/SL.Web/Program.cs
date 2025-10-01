using System.Security.Claims;
using Serilog;
using SL.Web.Mvc;
using SL.Web.Mvc.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogConfigurations();
builder.Services.AddBaseServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddFactoryServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLayerServices();
builder.Services.AddAuthServices();

builder.Services.AddLogging();


var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.DatabaseSeedAsync();

app.AddDevelopmentBuilder();
app.AddBaseBuilder();
app.AddRouteBuilder();
app.AddAuthBuilder();
app.UseSpecialRoute();
app.UseSerilogRequestLogging();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.Run();

