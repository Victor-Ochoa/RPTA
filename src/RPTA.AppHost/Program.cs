var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver-db")
    .WithLifetime(ContainerLifetime.Persistent); ;

var userDb = sqlServer
    .AddDatabase("userdb");
var userApi = builder.AddProject<Projects.RPTA_UserApi>("rpta-userapi")
    .WithReference(userDb)
    .WaitFor(userDb);

var productDb = sqlServer
    .AddDatabase("productdb");
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
