using Web.Components;
using IRepository;
using IServices;
<<<<<<< HEAD
using Repository;
using Services;
=======
using Services;
using Repository;
using Blazored.LocalStorage;

>>>>>>> feature/entidad-estadio

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

<<<<<<< HEAD
builder.Services.AddSingleton<ITeamRepository, TeamRepository>();
builder.Services.AddSingleton<ITeamService, TeamService>();
=======
builder.Services.AddSingleton<IStadiumRepository, StadiumRepository>();
builder.Services.AddSingleton<IStadiumService, StadiumService>();
builder.Services.AddBlazoredLocalStorage();
>>>>>>> feature/entidad-estadio

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();