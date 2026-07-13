// InterManagement.Server/Program.cs
// Version mise à jour avec authentification complète

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

// ── 2. COMPRESSION ────────────────────────────
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

// ── 5. Identity ───────────────────────────────
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ── 6. AuthService ────────────────────────────
builder.Services.AddScoped<AuthService>();

// ── 7. Construction ───────────────────────────
var app = builder.Build();

// ── 8. Initialisation des rôles au démarrage ──
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    await authService.EnsureRolesExistAsync();
}

// ── 9. Middleware exceptions ──────────────────
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

// ── 10. Pipeline HTTP ─────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression(); // ← EN PREMIER dans le pipeline
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── 11. Lancement ─────────────────────────────
app.Run();


/*using InternManagement.Infrastructure;
using InternManagement.Infrastructure.Data;
using InterManagement.Application;
using InterManagement.Domain.Exceptions;
using InterManagement.Server.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Services ───────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// ── 2. CORS ───────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── 3. Branchement des couches ────────────────
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

// ── 4. Identity ──────────────────────────────
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ── 5. AuthService (notre service custom) ─────
builder.Services.AddScoped<AuthService>();

// ── 6. Construction ───────────────────────────
var app = builder.Build();

// ── 7. Initialisation des rôles au démarrage ──
// Crée les rôles Admin/Trainee/Mentor s'ils n'existent pas encore

using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    await authService.EnsureRolesExistAsync();
}

// ── 8. Middleware exceptions ──────────────────
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



// ── 9. Pipeline HTTP ──────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication(); // ← AJOUTÉ : doit être AVANT UseAuthorization
app.UseAuthorization();
app.MapControllers();

// ── 10. Lancement ─────────────────────────────
app.Run();

*/


























/*

using InternManagement.Infrastructure;
using InterManagement.Application;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using InternManagement.Infrastructure.Data;  

var builder = WebApplication.CreateBuilder(args);

// ── 1. Services ───────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

// ── 2. CORS ───────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── 3. Branchement des couches 
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

// ── Identity 
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ── 4. Construction 
var app = builder.Build();

// ── 5. Middleware exceptions 
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerFeature>()?.Error;

        context.Response.ContentType = "application/json";



        // Les exceptions Mentor dans le switch
        context.Response.StatusCode = exception switch
        {
            TraineeNotFoundException      => 404,
            TraineeAlreadyExistsException => 409,
            TraineeNotActiveException     => 400,
            TraineeAlreadyAssignedException => 400,

            MentorNotFoundException       => 404,  // ← Mentor
            MentorAlreadyExistsException  => 409,  // ← Mentor
            MentorNotActiveException      => 400,  // ← Mentor

            AdminNotFoundException        => 404,  // ← Admin
            AdminAlreadyExistsException   => 409,  // ← Admin
            AdminNotActiveException       => 400,  // ← Admin

            PhaseNotFoundException         => 404,  // ← ajouter
            PhaseAlreadyCompletedException => 400,  // ← ajouter
            PhaseCancelledException        => 400,  // ← ajouter

            AssignmentNotFoundException       => 404,  // ← Assignment
            AssignmentAlreadyExistsException  => 409,  // ← Assignment
            AssignmentNotActiveException      => 400,  // ← Assignment

            FeedbackNotFoundException          => 404,  // ← Feedback

            WeeklyFollowUpNotFoundException      => 404,  // ← WeeklyFollowUp
            WeeklyFollowUpAlreadyExistsException => 409,  // ← WeeklyFollowUp
            WeeklyFollowUpAlreadyDoneException   => 400,  // ← WeeklyFollowUp

            WeekNotFoundException      => 404,  // ← Week
            WeekAlreadyExistsException => 409,  // ← Week

            DomainException               => 400,
            _                             => 500
        };

        await context.Response.WriteAsJsonAsync(new
        {
            error = exception?.Message
        });
    });
});

// ── 6. Pipeline HTTP ──────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// ── 7. Lancement ──────────────────────────────
app.Run();





*/




















//admin@gabera.com  et  Admin@2025





