using EduTrackOne.Application.Classes.Commands.CreateClasse;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Classes;
using EduTrackOne.Persistence;
using EduTrackOne.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using MediatR;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Remplace les variables d’environnement dans la chaîne de connexion
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    .Replace("${SA_PASSWORD}", Environment.GetEnvironmentVariable("SA_PASSWORD"));

builder.Services.AddDbContext<EduTrackOneDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null)
    )
    );


// Add services to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClasseRepository, ClasseRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblyContaining<CreateClasseHandler>();
});
var app = builder.Build();

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
