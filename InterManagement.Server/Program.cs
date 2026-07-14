// InterManagement.Server/Program.cs
using InternManagement.Infrastructure;
using InternManagement.Infrastructure.Data;
using InterManagement.Application;
using InterManagement.Domain.Exceptions;
using InterManagement.Server.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Services ───────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// ── 2. Compression ────────────────────────────
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// ── 3. CORS ───────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── 4. Branchement des couches ────────────────
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

// ── 5. Identity ────────────────────────────────
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ── 6. AuthService ─────────────────────────────
builder.Services.AddScoped<AuthService>();

// ── 7. Construction ────────────────────────────
var app = builder.Build();

// ── 8. Initialisation des rôles au démarrage ──
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    await authService.EnsureRolesExistAsync();
}

// ── 9. Middleware exceptions ───────────────────
// Convertit chaque DomainException métier (Trainee/Mentor/Admin/
// Phase/Assignment/Feedback/WeeklyFollowUp/Week introuvable, déjà
// existant, etc.) en code HTTP approprié plutôt qu'un 500 générique.
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            TraineeNotFoundException             => 404,
            TraineeAlreadyExistsException        => 409,
            TraineeNotActiveException            => 400,
            TraineeAlreadyAssignedException      => 400,
            MentorNotFoundException              => 404,
            MentorAlreadyExistsException         => 409,
            MentorNotActiveException             => 400,
            AdminNotFoundException               => 404,
            AdminAlreadyExistsException          => 409,
            AdminNotActiveException              => 400,
            PhaseNotFoundException               => 404,
            PhaseAlreadyCompletedException       => 400,
            PhaseCancelledException              => 400,
            AssignmentNotFoundException          => 404,
            AssignmentAlreadyExistsException     => 409,
            AssignmentNotActiveException         => 400,
            FeedbackNotFoundException            => 404,
            WeeklyFollowUpNotFoundException      => 404,
            WeeklyFollowUpAlreadyExistsException => 409,
            WeeklyFollowUpAlreadyDoneException   => 400,
            WeekNotFoundException                => 404,
            WeekAlreadyExistsException           => 409,
            DomainException                      => 400,
            _                                    => 500
        };

        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message
        });
    });
});

// ── 10. Pipeline HTTP ──────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression(); // doit être en premier dans le pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── 11. Lancement ──────────────────────────────
app.Run();
