using Web.Components;
using IRepository;
using IServices;
using Services;
using Repository;
using Blazored.LocalStorage;
<<<<<<< HEAD
=======

>>>>>>> feature/entidad-partido

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

<<<<<<< HEAD
// Repositorios y servicios
builder.Services.AddSingleton<ITeamRepository, TeamRepository>();
builder.Services.AddSingleton<ITeamService, TeamService>();

builder.Services.AddSingleton<IStadiumRepository, StadiumRepository>();
builder.Services.AddSingleton<IStadiumService, StadiumService>();

// Local storage
=======
builder.Services.AddSingleton<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();

>>>>>>> feature/entidad-partido
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();