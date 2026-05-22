using Blazored.LocalStorage;
using Web.Components;
using IRepository;
using IServices;
using Repository;
using Services;
using Web.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Import;
using Domain.Import.Interfaces;
using Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();

builder.Services.AddSingleton<ITeamRepository, TeamRepository>();
builder.Services.AddSingleton<ITeamService, TeamService>();

builder.Services.AddSingleton<IStadiumRepository, StadiumRepository>();
builder.Services.AddSingleton<IStadiumService, StadiumService>();

builder.Services.AddSingleton<IMatchRepository, MatchRepository>();
builder.Services.AddSingleton<IMatchService, MatchService>();
builder.Services.AddSingleton<IDateTimeMatch, SystemDateTimeMatch>();

builder.Services.AddSingleton<IFixtureRepository, FixtureRepository>();
builder.Services.AddSingleton<IFixtureService, FixtureService>();
builder.Services.AddSingleton<FixtureGenerator>();

builder.Services.AddSingleton<ILogRepository, LogRepository>();
builder.Services.AddSingleton<ILogService, LogService>();

builder.Services.AddScoped<ICsvParser, CsvParser>();
builder.Services.AddScoped<ITeamImportValidator, TeamImportValidator>();
builder.Services.AddScoped<ITeamFactory, TeamFactory>();
builder.Services.AddScoped<TeamImporter>();
builder.Services.AddScoped<IImportService, ImportService>();

builder.Services.AddSingleton<TournamentConfig>();
builder.Services.AddSingleton<IConfigService, ConfigService>();
builder.Services.AddSingleton<ISimulationService, CrucesSimulationService>();

builder.Services.AddSingleton<ICrucesRepository, CrucesRepository>();
builder.Services.AddSingleton<ICrucesGeneratorService, CrucesGeneratorService>();
builder.Services.AddSingleton<ICruceService, CruceService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var teamRepo = scope.ServiceProvider.GetRequiredService<ITeamRepository>();
    var stadiumRepo = scope.ServiceProvider.GetRequiredService<IStadiumRepository>();

    var uruguay = new Team("Uruguay", new Confederation("CONMEBOL"), 300);
    var argentina = new Team("Argentina", new Confederation("CONMEBOL"), 301);
    var brasil = new Team("Brasil", new Confederation("CONMEBOL"), 400);

    teamRepo.Add(uruguay);
    teamRepo.Add(argentina);
    teamRepo.Add(brasil);

    stadiumRepo.AddStadium(new Stadium("Centenario", "Montevideo", null, 60000));
    stadiumRepo.AddStadium(new Stadium("Maracaná", "Rio", null, 70000));
    stadiumRepo.AddStadium(new Stadium("Campeón del Siglo", "Montevideo", "Estadio de Peñarol.", 40000));
    stadiumRepo.AddStadium(new Stadium("Belvedere", "Montevideo", "Estadio de Liverpool Fútbol Club.", 20000));

    var user = new User(
        "Federica",
        "Paz",
        "fede@gmail.com",
        new DateTime(1990, 5, 20),
        "Password123!",
        [UserRole.SystemAdministrator, UserRole.Editor]);

    userRepo.AddUser(user);
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