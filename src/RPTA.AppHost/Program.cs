var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgresdb")
    .WithPgWeb()
    .WithLifetime(ContainerLifetime.Persistent); ;

var productDb = postgres
    .AddDatabase("productdb");

var userApi = builder.AddProject<Projects.RPTA_UserApi>("rpta-userapi");

var productApi = builder.AddProject<Projects.RPTA_ProductApi>("rpta-productapi")
    .WithReference(productDb)
    .WaitFor(productDb);

var proxy = builder.AddProject<Projects.RPTA_ReverseProxy>("rpta-reverseproxy")
    .WithReference(productApi)
    .WaitFor(productApi)
    .WithReference(userApi)
    .WaitFor(userApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
