using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orange.Sql.Tool.Database;
using Orange.Sql.Tool.Users;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = CoconaApp.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration["OrangeSql:ConnectionString"]!));

builder.Services.AddSingleton<IUserInfoService, UserInfoService>();

builder.Host.UseSerilog((hostContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostContext.Configuration)
);

var app = builder.Build();

app.AddCommands<UserInfoCommands>();

app.Run();