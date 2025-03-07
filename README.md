
# Reverse Proxy Study with YARP and Aspire

This project is a study of implementing a reverse proxy using YARP (Yet Another Reverse Proxy) and Aspire. The system is designed to manage user and product APIs, routing requests through a reverse proxy.

## Technologies Used

- **.NET 9**
- **YARP (Yet Another Reverse Proxy)**
- **Aspire**

## Project Structure

- **RPTA.AppHost**: The main project hosting the reverse proxy and API services.
- **RPTA_UserApi**: User management API.
- **RPTA_ProductApi**: Product management API.

## Configuration

The reverse proxy and APIs are configured in the `Program.cs` file:


```
var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver-db")
    .WithLifetime(ContainerLifetime.Persistent);

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

```

## API Endpoints

The following endpoints are available through the reverse proxy:

### Products API

- `GET /api/products`: Retrieve all products.
- `GET /api/products/{productId}`: Retrieve a specific product by ID.
- `POST /api/products`: Create a new product.
- `PUT /api/products/{productId}`: Update an existing product.
- `DELETE /api/products/{productId}`: Delete a product.

### Users API

- `GET /api/Users`: Retrieve all users.
- `GET /api/Users/{UserId}`: Retrieve a specific user by ID.
- `POST /api/Users`: Create a new user.
- `PUT /api/Users/{UserId}`: Update an existing user.
- `DELETE /api/Users/{UserId}`: Delete a user.
- `DELETE /api/Users/{UserId}/hard`: Hard delete a user.

## Running the Project

To run the project, use the following command:


```
dotnet run --project src/RPTA.AppHost

```

## Contributions

This project was developed as a study to demonstrate the use of YARP and Aspire in building a reverse proxy system. Contributions and feedback are welcome.

## Contact

For any inquiries, please contact Victor Ochoa at 'victor20054@gmail.com'.
