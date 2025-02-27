var builder = DistributedApplication.CreateBuilder(args);

var userApi = builder.AddProject<Projects.RPTA_UserApi>("rpta-userapi");

var productApi = builder.AddProject<Projects.RPTA_ProductApi>("rpta-productapi");

var proxy = builder.AddProject<Projects.RPTA_ReverseProxy>("rpta-reverseproxy")
                .WithReference(productApi)
                .WithReference(userApi)
                .WithExternalHttpEndpoints();

builder.Build().Run();
