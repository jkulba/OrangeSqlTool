using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Orange.Sql.Tool.Users;
using Serilog;

var builder = CoconaApp.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddSingleton<IUserInfoRepository, UserInfoRepository>();

builder.Host.UseSerilog((hostContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostContext.Configuration)
);

var app = builder.Build();