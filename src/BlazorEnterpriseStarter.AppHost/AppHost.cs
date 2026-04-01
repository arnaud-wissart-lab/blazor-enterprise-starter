var builder = DistributedApplication.CreateBuilder(args);

var serveurApi = builder.AddProject<Projects.BlazorEnterpriseStarter_Server>("server")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.BlazorEnterpriseStarter_App>("app")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(serveurApi)
    .WaitFor(serveurApi);

builder.Build().Run();
