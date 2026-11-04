using Biblioteca.Data;
using Intalnire1.Api;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LibraryContext") ?? throw new InvalidOperationException("Connection string 'LibraryContext' not found.");

// OpenAIClient pentru GitHubModels (GPT-4o-mini) - vezi
var token = builder.Configuration["GitHubModels:Token"]
    ?? throw new InvalidOperationException("Lipsește GitHubModels:Token (User Secrets).");
var endpoint = new Uri("https://models.github.ai/inference");

IChatClient chat = new OpenAIClient(new ApiKeyCredential(token),
        new OpenAIClientOptions { Endpoint = endpoint })
    .GetChatClient("openai/gpt-4o-mini")
    .AsIChatClient();



builder.Services.AddDbContext<LibraryContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.Services.AddChatClient(chat);

var app = builder.Build();

app.MapLibraryApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    db.Database.Migrate();          // aplică migrările (înlocuiește Update-Database)
    LibrarySeed.Seed(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // /openapi/v1.json (OpenAPI 3.1 în .NET 10)
    app.MapScalarApiReference();    // /scalar
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "start",
    pattern: "start",
    defaults: new { controller = "Home", action = "Index" })
    .WithStaticAssets();

app.Run();
