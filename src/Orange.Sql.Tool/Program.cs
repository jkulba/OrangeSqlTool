using Cocona;
using ConsoleTables;
using Dapper;
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

// app.AddCommand("weather", async (string city, IWeatherService weatherService) =>
// {
//     var weather = await weatherService.GetWeatherForCityAsync(city);
//     Console.WriteLine(JsonSerializer.Serialize(weather, new JsonSerializerOptions { WriteIndented = true }));
// });


app.AddCommand("create", () => Console.WriteLine("Creating a new user...")).WithDescription("Create a new UserInfo");
app.AddCommand("get", () => Console.WriteLine("Getting user By Id...")).WithDescription("Get UserInfo by Id");
// app.AddCommand("list", () => Console.WriteLine("List all users...")).WithDescription("List all users");

app.AddCommand("list", async (IUserInfoService userInfoService) =>
{
    var users = await userInfoService.GetAllUsersAsync();
    var table = ConsoleTable.From(users.AsList()).ToString();
    Console.WriteLine(table);
    
}).WithDescription("List all users");

app.AddCommand("delete", () => Console.WriteLine("Deleting user...")).WithDescription("Delete a user");
app.AddCommand("update", () => Console.WriteLine("Updating user...")).WithDescription("Update a user");
app.AddCommand("version", () => Console.WriteLine("App version...")).WithDescription("App Version");

app.Run();