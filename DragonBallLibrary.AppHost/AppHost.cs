using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add Azure services (for production deployment)
// Note: These are configured for actual Azure deployment
// For local development, we'll use simpler alternatives

// Add SQL Server (using container for local development)
var sqlServerName = builder.AddParameter("sql-server-name");
var rgName = builder.AddParameter("resource-group-name");
var sql = builder.AddAzureSqlServer("sql-dragonball")
    .AsExisting(sqlServerName, rgName);

//var sql = builder.AddSqlServer("sql")
//    .WithLifetime(ContainerLifetime.Persistent)
//    .AddDatabase("dragonballdb");

// Add Azure Storage emulator
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(
        azurite =>
        {
            azurite.WithBlobPort(27000)
                    .WithQueuePort(27001)
                    .WithTablePort(27002);
            azurite.WithLifetime(ContainerLifetime.Persistent);
        });

var blobStorage = storage.AddBlobs("character-images");

// Add API service with dependencies
var apiService = builder.AddProject<Projects.DragonBallLibrary_ApiService>("apiservice")
    .WithReference(sql)
    .WithReference(blobStorage)
    .WithHttpHealthCheck("/health")
    .WithEnvironment("AZURE_CLIENT_ID", () => "development-client-id")
    .WithEnvironment("AZURE_CLIENT_SECRET", () => "development-client-secret")
    .WithEnvironment("AZURE_TENANT_ID", () => "development-tenant-id");

// For the React frontend, we'll reference it by URL since it's a separate Node.js app
// In production, this would be containerized and added as a container resource

builder.Build().Run();
