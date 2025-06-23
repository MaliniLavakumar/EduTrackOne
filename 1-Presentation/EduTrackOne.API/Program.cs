using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Classes.CreateClasse;
using EduTrackOne.Application.Classes.DeleteClasse;
using EduTrackOne.Application.Classes.GetClassesByEnseignantPrincipal;
using EduTrackOne.Application.Classes.RemoveInscription;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.Eleves.CreateEleve;
using EduTrackOne.Application.Eleves.UpdateEleve;
using EduTrackOne.Application.EnseignantsPrincipaux.CreateEnseignantPrincipal;
using EduTrackOne.Application.Inscriptions.AddNotesForClasse;
using EduTrackOne.Application.Inscriptions.AddPresencesForClasse;
using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;
using EduTrackOne.Application.Inscriptions.Services;
using EduTrackOne.Application.Inscriptions.UpdateNote;
using EduTrackOne.Application.Inscriptions.UpdatePresence;
using EduTrackOne.Application.Matieres.CreateMatiere;
using EduTrackOne.Application.Matieres.GetAllMatieres;
using EduTrackOne.Application.Utilisateurs.ChangePassword;
using EduTrackOne.Application.Utilisateurs.CreateUser;
using EduTrackOne.Application.Utilisateurs.DeleteUser;
using EduTrackOne.Application.Utilisateurs.UpdateUser;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.EnseignantsPrincipaux;
using EduTrackOne.Domain.Inscriptions;
using EduTrackOne.Domain.Matieres;
using EduTrackOne.Domain.Notes;
using EduTrackOne.Domain.Presences;
using EduTrackOne.Domain.Utilisateurs;
using EduTrackOne.Infrastructure.Identity;
using EduTrackOne.Infrastructure.Services;
using EduTrackOne.Persistence;
using EduTrackOne.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.Channels;


var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Initialiser Log.Logger AVANT la création du host final pour capter les logs startup
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Remplacer le logger par défaut d’ASP.NET Core par Serilog
builder.Host.UseSerilog();


// Configuration du DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    

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
builder.Services.AddScoped<IPasswordHasher<Utilisateur>, PasswordHasher<Utilisateur>>();
builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.IterationCount = 100_000;
});

// Cookie authentication (pour MVC)
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Utilisateur/Login";
    opts.Cookie.Name = "EduTrackOne.Identity";
    opts.Cookie.HttpOnly = true;
    opts.ExpireTimeSpan = TimeSpan.FromHours(1);
    opts.SlidingExpiration = false;
    opts.Cookie.SameSite = SameSiteMode.Lax;
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
builder.Services.AddControllersWithViews(options =>
{
    // Configuration MVC : ajout du filtre global no-cache
    options.Filters.Add(new ResponseCacheAttribute
    {
        NoStore = true,
        Location = ResponseCacheLocation.None
    });
})
.AddJsonOptions(o =>
{
    // Configuration JSON : convertisseur d’énumérations en string
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
builder.Services.AddValidatorsFromAssemblyContaining<GetAllMatieresQueryHandler>();
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
    var logger = services.GetRequiredService<ILogger<Program>>();
   
   
    
    try
    {
        logger.LogInformation("Démarrage du seed Identity...");
        await IdentityDataInitializer.SeedUsersAsync(services);
        logger.LogInformation("Seed Identity terminé.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erreur pendant le seed Identity. L’application continue.");
    }
}

app.MapGet("/health", () => Results.Ok("API OK"));

RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");



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
