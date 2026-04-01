var builder = DistributedApplication.CreateBuilder(args);

var apiBackend = builder.AddProject<Projects.BlazorEnterpriseStarter_Server>("server")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.BlazorEnterpriseStarter_App>("app")
    .WithExternalHttpEndpoints()
    .WithReference(apiBackend)
    .WaitFor(apiBackend)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
