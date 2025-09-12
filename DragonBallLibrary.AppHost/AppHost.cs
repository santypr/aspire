var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.DragonBallLibrary_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
