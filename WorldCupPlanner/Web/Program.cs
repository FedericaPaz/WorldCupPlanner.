using Web.Components;
using IRepository;
using IServices;
using Services;
using Repository;
using Domain;
using Blazored.LocalStorage;
using DomainTests.Stubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<ITeamRepository, TeamRepository>();
builder.Services.AddSingleton<ITeamService, TeamService>();

builder.Services.AddSingleton<IStadiumRepository, StadiumRepository>();
builder.Services.AddSingleton<IStadiumService, StadiumService>();

builder.Services.AddSingleton<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddSingleton<IDateTimeMatch, SystemDateTimeMatch>();

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var teamRepo = scope.ServiceProvider.GetRequiredService<ITeamRepository>();
    var stadiumRepo = scope.ServiceProvider.GetRequiredService<IStadiumRepository>();
    var matchRepo = scope.ServiceProvider.GetRequiredService<IMatchRepository>();
    var clock = scope.ServiceProvider.GetRequiredService<IDateTimeMatch>();

    // Teams
    var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 300);
    var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 301);
    var brasil = new Team("Brasil", new Confederation("CONMEBOL"), 400);

    teamRepo.Add(uruguay);
    teamRepo.Add(argentina);
    teamRepo.Add(brasil);

    // Stadiums
    var centenario = new Stadium("Centenario", "Montevideo", null, 60000);
    var maracana = new Stadium("Maracaná", "Rio", null, 70000);

    stadiumRepo.AddStadium(centenario);
    stadiumRepo.AddStadium(maracana);

    // Matches
    var match1 = new Match(clock.Now.AddDays(1), centenario, uruguay, argentina, 'A', clock);
    var match2 = new Match(clock.Now.AddDays(2), maracana, brasil, uruguay, 'B', clock);

    matchRepo.AddMatch(match1);
    matchRepo.AddMatch(match2);
}

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