var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EduTrackOne_API>("edutrackone-api");

builder.Build().Run();
