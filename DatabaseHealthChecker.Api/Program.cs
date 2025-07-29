using DatabaseHealthChecker.Common.Models;
using DatabaseHealthChecker.Common.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<FileLoggerService>();
builder.Services.AddHostedService<DatabaseHealthCheckerService>();

var app = builder.Build();

app.Run();