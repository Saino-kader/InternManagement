using InternManagement.Infrastructure;
using InterManagement.Application;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

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

// ── 3. Branchement des couches ────────────────
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

// ── 4. Construction ───────────────────────────
var app = builder.Build();

// ── 5. Middleware exceptions ──────────────────
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

            InternFileNotFoundException       => 404,  // ← InternFile
            InternFileAlreadyExistsException  => 409,  // ← InternFile
            InternFileTypeNotAllowedException => 400,  // ← InternFile

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
























/*
using Swashbuckle.AspNetCore;
using InternManagement.Infrastructure;
using InterManagement.Application;
using InterManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
// ... le reste du code



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


*/