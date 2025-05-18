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
using Microsoft.Data.SqlClient;
using EduTrackOne.Application.Classes.RemoveInscription;
using EduTrackOne.Application.Matieres.CreateMatiere;
using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;
using EduTrackOne.Domain.Matieres;
using EduTrackOne.Application.Inscriptions.Services;
using EduTrackOne.Application.Inscriptions.AddNotesForClasse;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using EduTrackOne.Application.Inscriptions.AddPresencesForClasse;
using EduTrackOne.Application.Inscriptions.UpdateNote;
using EduTrackOne.Application.Inscriptions.UpdatePresence;
using EduTrackOne.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using EduTrackOne.Infrastructure.Services;
using EduTrackOne.Domain.Utilisateurs;
using EduTrackOne.Application.Utilisateurs.CreateUser;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using EduTrackOne.Application.Utilisateurs.UpdateUser;
using EduTrackOne.Application.Utilisateurs.ChangePassword;
using EduTrackOne.Application.Utilisateurs.DeleteUser;
using EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal;
using Microsoft.AspNetCore.Identity;
using EduTrackOne.Infrastructure.Identity;


var builder = WebApplication.CreateBuilder(args);


// Configuration du DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("${SA_PASSWORD}", Environment.GetEnvironmentVariable("SA_PASSWORD"));

builder.Services.AddDbContext<EduTrackOneDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null))
);

//Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
   
})
.AddEntityFrameworkStores<EduTrackOneDbContext>()
.AddDefaultTokenProviders();

// Cookie authentication (pour MVC)
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Utilisateur/Login";
    opts.Cookie.Name = "EduTrackOne.Identity";
    opts.Cookie.HttpOnly = true;
    opts.ExpireTimeSpan = TimeSpan.FromHours(1);
});

// Authorization
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Inscription des services de persistance et métier
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClasseRepository, ClasseRepository>();
builder.Services.AddScoped<IEnseignantPrincipalRepository, EnseignantPrincipalRepository>();
builder.Services.AddScoped<IEleveRepository, EleveRepository>();
builder.Services.AddScoped<IInscriptionRepository, InscriptionRepository>();
builder.Services.AddScoped<IMatiereRepository, MatiereRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IInscriptionManager, InscriptionManager>();
builder.Services.AddScoped<IPresenceRepository, PresenceRepository>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();

// Activation de l’auto-validation FluentValidation
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o => {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CreateClasseCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEnseignantPrincipalCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEleveCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateEleveCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddInscriptionCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RemoveInscriptionCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMatiereCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetInscriptionsByClasseQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddNotesForClasseCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddPresencesForClasseCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateNoteCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdatePresenceCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordCommandValidator>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<CreateClasseCommandHandler>()
       .RegisterServicesFromAssemblyContaining<DeleteClasseCommandHandler>()
       .RegisterServicesFromAssemblyContaining<CreateEnseignantPrincipalCommandHandler>()
       .RegisterServicesFromAssemblyContaining<CreateEleveCommandHandler>()
       .RegisterServicesFromAssemblyContaining<UpdateEleveCommandHandler>()
       .RegisterServicesFromAssemblyContaining<AddInscriptionCommandHandler>()
       .RegisterServicesFromAssemblyContaining<RemoveInscriptionCommandHandler>()
       .RegisterServicesFromAssemblyContaining<CreateMatiereCommandHandler>()
       .RegisterServicesFromAssemblyContaining<AddNotesForClasseCommandHandler>()
       .RegisterServicesFromAssemblyContaining<AddPresencesForClasseCommandHandler>()
       .RegisterServicesFromAssemblyContaining<UpdateNoteCommandHandler>()
       .RegisterServicesFromAssemblyContaining<UpdatePresenceCommandHandler>()
       .RegisterServicesFromAssemblyContaining<CreateUserCommandHandler>()
       .RegisterServicesFromAssemblyContaining<UpdateUserCommandHandler>()
       .RegisterServicesFromAssemblyContaining<ChangePasswordCommandHandler>()
       .RegisterServicesFromAssemblyContaining<DeleteUserCommandHandler>()
       .RegisterServicesFromAssemblyContaining<GetClassesByEnseignantPrincipalQueryHandler>());


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentityDataInitializer.SeedUsersAsync(services);
}

//app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Utilisateur}/{action=Login}/{id?}"
).WithStaticAssets();

app.Run();
