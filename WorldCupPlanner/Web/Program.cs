using Blazored.LocalStorage;
using Domain.Calculators;
using Domain.Entities;
using Domain.Import;
using Domain.Import.Interfaces;
using Domain.Interfaces;
using IRepository;
using IServices;
using IServices.CrucesInterface;
using IServices.FixtureInterfaces;
using IServices.MatchInterfaces;
using IServices.TeamInterfaces;
using IServices.UserInterfaces;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using Web.Components;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WorldCupPlannerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserReadRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserQueryService, UserService>();
builder.Services.AddScoped<IUserEditService, UserService>();
builder.Services.AddScoped<SessionService>();

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamQueryService, TeamService>();
builder.Services.AddScoped<ITeamEditService, TeamService>();

builder.Services.AddScoped<IStadiumRepository, StadiumRepository>();
builder.Services.AddScoped<IStadiumService, StadiumService>();

builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchQueryService, MatchService>();
builder.Services.AddScoped<IMatchEditService, MatchService>();
builder.Services.AddScoped<IMatchCreateService, MatchService>();
builder.Services.AddSingleton<IDateTimeMatch, SystemDateTimeMatch>();

builder.Services.AddScoped<IFixtureRepository, FixtureRepository>();
builder.Services.AddScoped<IFixtureQueryService, FixtureService>();
builder.Services.AddScoped<IFixtureEditService, FixtureService>();

builder.Services.AddScoped<IGroupRepository, GroupRepository>();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddScoped<ICsvParser, CsvParser>();
builder.Services.AddScoped<ITeamImportValidator, TeamImportValidator>();
builder.Services.AddScoped<ITeamFactory, TeamFactory>();
builder.Services.AddScoped<TeamImporter>();
builder.Services.AddScoped<IImportService, ImportService>();

builder.Services.AddScoped<TournamentConfig>();
builder.Services.AddScoped<IConfigService, ConfigService>();
builder.Services.AddScoped<ISimulationService, SimulationService>();

builder.Services.AddScoped<ICrucesRepository, CrucesRepository>();
builder.Services.AddScoped<ICruceQueryService, CruceService>();
builder.Services.AddScoped<ICruceEditService, CruceService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IExportService, ExportService>();

builder.Services.AddScoped<IRankingService, RankingService>();

builder.Services.AddScoped<Services.Simulation.ISimulationEngineFactory, Services.Simulation.DefaultSimulationEngineFactory>();

builder.Services.AddScoped<IRankingCalculator, RankingCalculator>();

var app = builder.Build();

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