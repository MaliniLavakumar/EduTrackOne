using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;


namespace EduTrackOne.Persistence
{
    public class DesignTimeDbContextFactory
        : IDesignTimeDbContextFactory<EduTrackOneDbContext>
    {
        public EduTrackOneDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            var solutionRoot = Path.GetFullPath(Path.Combine(basePath, "../../")); // <- racine solution
            DotNetEnv.Env.Load(Path.Combine(solutionRoot, ".env"));


            var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .Build();

            var connectionString = configuration
                .GetConnectionString("DefaultConnection")
                .Replace("${SA_PASSWORD}",
                         Environment.GetEnvironmentVariable("SA_PASSWORD"));

            var optionsBuilder = new DbContextOptionsBuilder<EduTrackOneDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new EduTrackOneDbContext(optionsBuilder.Options);
        }
    }

}

