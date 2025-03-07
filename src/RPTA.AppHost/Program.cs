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

var reverseProxy = builder.AddYarp("reverse-proxy")
    .WithEndpoint(port: 8001, scheme: "http")
    .WithReference(userApi)
    .WithReference(productApi)
    .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();
