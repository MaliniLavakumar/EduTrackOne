using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using EduTrackOne.Persistence;
using EduTrackOne.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EduTrackOne.Application.Classes.DeleteClasse;
using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Application.Eleves.UpdateEleve;
using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Domain.Inscriptions;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Configuration du DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("${SA_PASSWORD}", Environment.GetEnvironmentVariable("SA_PASSWORD"));

builder.Services.AddDbContext<EduTrackOneDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null))
);

// Inscription des services de persistance et métier
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClasseRepository, ClasseRepository>();
builder.Services.AddScoped<IEnseignantPrincipalRepository, EnseignantPrincipalRepository>();
builder.Services.AddScoped<IEleveRepository, EleveRepository>();
builder.Services.AddScoped<IInscriptionRepository, InscriptionRepository>();


// Activation de l’auto-validation FluentValidation
builder.Services.AddControllersWithViews()
  .AddJsonOptions(options =>
  {
      options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
  });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CreateClasseCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEnseignantPrincipalCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEleveCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateEleveCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddInscriptionCommandValidator>();
// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CreateClasseCommandHandler>()
       .RegisterServicesFromAssemblyContaining<DeleteClasseCommandHandler>()
       .RegisterServicesFromAssemblyContaining<CreateEnseignantPrincipalCommandHandler>()
       .RegisterServicesFromAssemblyContaining<CreateEleveCommandHandler>()
       .RegisterServicesFromAssemblyContaining<UpdateEleveCommandHandler>()
       .RegisterServicesFromAssemblyContaining<AddInscriptionCommandHandler>()
);

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.Run();
