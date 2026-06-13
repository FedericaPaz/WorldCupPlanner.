using Blazored.LocalStorage;
using Domain.Calculators;
using Domain.Entities;
using Web.Components;
using IRepository;
using IServices;
using Repository;
using Services;
using Web.Services;
using Domain.Import;
using Domain.Import.Interfaces;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WorldCupPlannerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SessionService>();

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();

builder.Services.AddScoped<IStadiumRepository, StadiumRepository>();
builder.Services.AddScoped<IStadiumService, StadiumService>();

builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddSingleton<IDateTimeMatch, SystemDateTimeMatch>();

builder.Services.AddScoped<IFixtureRepository, FixtureRepository>();
builder.Services.AddScoped<IFixtureService, FixtureService>();
builder.Services.AddScoped<FixtureGenerator>();

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
builder.Services.AddScoped<ISimulationService, CrucesSimulationService>();

builder.Services.AddScoped<ICrucesRepository, CrucesRepository>();
builder.Services.AddScoped<ICrucesGeneratorService, CrucesGeneratorService>();
builder.Services.AddScoped<ICruceService, CruceService>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IExportService, ExportService>();

builder.Services.AddScoped<RankingCalculator>();
builder.Services.AddScoped<IRankingService, RankingService>();

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