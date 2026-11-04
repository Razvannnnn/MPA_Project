using Biblioteca.Data;
using Intalnire1.Api;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("LibraryContext") ?? throw new InvalidOperationException("Connection string 'LibraryContext' not found.");

// OpenAIClient pentru GitHubModels
//var token = builder.Configuration["GitHubModels:Token"]
//    ?? throw new InvalidOperationException("Lipsește GitHubModels:Token (User Secrets).");
//var endpoint = new Uri("https://models.github.ai/inference");

//IChatClient chat = new OpenAIClient(new ApiKeyCredential(token),
//        new OpenAIClientOptions { Endpoint = endpoint })
//    .GetChatClient("openai/")
//    .AsIChatClient();


builder.Services.AddDbContext<LibraryContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidation();
builder.Services.AddOpenApi();
//builder.Services.AddChatClient(chat);
builder.Services.AddHttpClient<OllamaService>();

var app = builder.Build();

app.MapLibraryApi();

app.MapPost("/api/v2/books/{id:int}/pitch", async (int id, LibraryContext db, OllamaService ollama) =>
{
    var b = await db.Book.Include(x => x.Author).Include(x => x.Genre)
                         .FirstOrDefaultAsync(x => x.ID == id);
    if (b is null) return Results.NotFound();

    var prompt = "Ești bibliotecar. Răspunde în română, în cel mult 2 propoziții. " +
                 $"Recomandă cartea „{b.Title}” de {b.Author?.FullName} " +
                 $"({b.Genre?.Name}). Descriere: {b.Description}";

    var pitch = await ollama.AskAsync(prompt);
    return Results.Ok(new { b.Title, Pitch = pitch });
});

app.MapGet("/api/v2/ask", async (string q, OllamaService ollama) =>
    await ollama.AskAsync(q));


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
