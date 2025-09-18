using SL.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBaseServices();
builder.Services.AddLayerServices();

var app = builder.Build();

// Add middlewares
app.DatabaseSeed();
app.AddDevelopmentBuilder();
app.AddBaseBuilder();
app.AddRouteBuilder();
app.AddAuthBuilder();
app.UseSpecialRoute();

app.Run();

