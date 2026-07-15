// InterManagement.Client/Program.cs
using InterManagement.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ── Session ────────────────────────────────────────
// Stocke les infos de l'utilisateur connecté (UserRole, UserEmail, EntityId)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── API base URL ───────────────────────────────────
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5099/api/";

// Délai raisonnable pour tous les appels au Server : plus court que les
// 100s par défaut de HttpClient, pour qu'un appel qui échoue (Server
// injoignable, requête bloquée...) échoue vite et affiche une erreur
// claire plutôt que de laisser l'utilisateur face à une page qui
// semble bloquée pendant plus d'une minute.
void ConfigureApiClient(HttpClient client)
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
}

// ── Services HTTP typés vers l'API Server ──────────
builder.Services.AddHttpClient<IPhaseApiService, PhaseApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IWeeklyFollowUpApiService, WeeklyFollowUpApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IFeedbackApiService, FeedbackApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IWeekApiService, WeekApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<ITraineeApiService, TraineeApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IMentorApiService, MentorApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IAssignmentApiService, AssignmentApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IAdminApiService, AdminApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IDashboardApiService, DashboardApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IImportedFollowUpApiService, ImportedFollowUpApiService>(ConfigureApiClient);
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(ConfigureApiClient);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession(); // Doit être avant UseAuthorization : les contrôleurs lisent le rôle en session
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
