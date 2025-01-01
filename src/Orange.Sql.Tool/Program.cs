using Cocona;
using ConsoleTables;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orange.Sql.Tool.Database;
using Orange.Sql.Tool.Users;
using Serilog;
using System.Text.Json;
using FluentValidation;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = CoconaApp.CreateBuilder();

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Host.UseSerilog((hostContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostContext.Configuration)
);

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqlConnectionFactory(builder.Configuration["OrangeSql:ConnectionString"]!));

builder.Services.AddSingleton<IUserInfoService, UserInfoService>();

builder.Services.AddScoped<IValidator<UserInfo>, UserInfoValidator>();

var app = builder.Build();

// app.AddCommand("create", () => Console.WriteLine("Creating a new user...")).WithDescription("Create a new UserInfo");
app.AddCommand("get",  async (Guid userId, IUserInfoService userInfoService) =>
{
    var user = await userInfoService.GetUserByIdAsync(userId);
    if (user == null)
    {
        Console.WriteLine("User not found");
        return;
    }
    else
    {
        var table = ConsoleTable.From<UserInfo>([user]);
        Console.WriteLine(table);
    }
    
}).WithDescription("Get UserInfo by Id");

app.AddCommand("list-by-date-created", async (IUserInfoService userInfoService, SortDirection sort) =>
{
    var users = await userInfoService.GetAllUsersSortedByDateCreatedAsync(sort);
    var table = ConsoleTable.From(users.AsList()).ToString();
    Console.WriteLine(table);

}).WithDescription("Sort users by date created");

app.AddCommand("list-by-lastname", async (IUserInfoService userInfoService, SortDirection sort) =>
{
    var users = await userInfoService.GetAllUsersSortedByLastNameAsync(sort);
    var table = ConsoleTable.From(users.AsList()).ToString();
    Console.WriteLine(table);

}).WithDescription("Sort users by lastname");

app.AddCommand("delete", async (Guid userId, IUserInfoService userInfoService) =>
{
    var success = await userInfoService.DeleteUserByIdAsync(userId);
    Console.WriteLine(!success ? "User not found" : $"User {userId} successfully deleted");
}).WithDescription("Delete a user");

// app.AddCommand("update", () => Console.WriteLine("Updating user...")).WithDescription("Update a user");
app.AddCommand("version", () => Console.WriteLine("App version...")).WithDescription("App Version");

app.AddCommand("create-from-file", async (string filePath, IUserInfoService userInfoService) =>
{
    try
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' was not found.");
        }
        
        var jsonData = File.ReadAllText(filePath); 
        var user = JsonSerializer.Deserialize<UserInfo>(jsonData);
        
        if (user == null)
        {
            throw new InvalidOperationException("The JSON file does not contain a valid UserInfo object.");
        }
        
        var result = await userInfoService.CreateUserAsync(user);

        if (result.IsSuccess)
        {
            var userInfo = result.GetValueOrDefault();
            Console.WriteLine("User created successfully!");
            Console.WriteLine($"ID: {userInfo.UserId}, Name: {userInfo.FirstName} {userInfo.LastName}");
        }
        else
        {
            Console.WriteLine("Failed to create user:");
            var error = result.GetErrorOrDefault();
            Console.WriteLine($"Error occurred: {error.ToString()}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}).WithDescription("Create a new user from a local JSON file.");

app.AddCommand("update-from-file", async (string filePath, IUserInfoService userInfoService) =>
{
    try
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file '{filePath}' was not found.");
        }
        
        var jsonData = File.ReadAllText(filePath); 
        var user = JsonSerializer.Deserialize<UserInfo>(jsonData);
        
        if (user == null)
        {
            throw new InvalidOperationException("The JSON file does not contain a valid UserInfo object.");
        }
        
        var result = await userInfoService.UpdateUserAsync(user);

        if (result.IsSuccess)
        {
            var userInfo = result.GetValueOrDefault();
            Console.WriteLine("User updated successfully!");
            if (userInfo != null)
                Console.WriteLine($"ID: {userInfo.UserId}, Name: {userInfo.FirstName} {userInfo.LastName}");
        }
        else
        {
            Console.WriteLine("Failed to create user:");
            var error = result.GetErrorOrDefault();
            Console.WriteLine($"Error occurred: {error.ToString()}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}).WithDescription("Update an existing user from a local JSON file.");

app.Run();