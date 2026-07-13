// InterManagement.Client/Program.cs
// Version mise à jour avec Session pour l'authentification

using InterManagement.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ── Session 
// Stocke les infos de l'utilisateur connecté
// (UserRole, UserEmail, EntityId)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8); // Session de 8 heures
    options.Cookie.HttpOnly = true;              // Inaccessible en JS
    options.Cookie.IsEssential = true;           // Obligatoire même sans consentement
});

// ── API base URL 
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5099/api/";

// ── Services HTTP existants 
builder.Services.AddHttpClient<IPhaseApiService, PhaseApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IWeeklyFollowUpApiService, WeeklyFollowUpApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IFeedbackApiService, FeedbackApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IWeekApiService, WeekApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<ITraineeApiService, TraineeApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IMentorApiService, MentorApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IAssignmentApiService, AssignmentApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IAdminApiService, AdminApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IDashboardApiService, DashboardApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


builder.Services.AddHttpClient<IAssignmentApiService, AssignmentApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


builder.Services.AddHttpClient<IImportedFollowUpApiService, ImportedFollowUpApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});


builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();        // ← AJOUTÉ : doit être AVANT UseAuthorization
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
