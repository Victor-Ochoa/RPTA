using Microsoft.EntityFrameworkCore;
using RPTA.UserApi.Data;
using RPTA.UserApi.Models;
using RPTA.UserApi.Request;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RPTA.UserApi", Version = "v1" });
});

builder.AddSqlServerDbContext<UserDbContext>(
    connectionName: "userdb");

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    app.UseSwagger();
    app.UseSwaggerUI();

    // Create database if not exists
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

// Define API endpoints
// GET all users
app.MapGet("/users", async (UserDbContext db) =>
    await db.Users.ToListAsync())
.WithName("GetAllUsers")
.Produces<List<User>>(StatusCodes.Status200OK);

// GET user by id
app.MapGet("/users/{id}", async (int id, UserDbContext db) =>
    await db.Users.FindAsync(id) is User user
        ? Results.Ok(user)
        : Results.NotFound())
.WithName("GetUserById")
.Produces<User>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// POST new user
app.MapPost("/users", async (UserCreate user, UserDbContext db) =>
{
    // Simple email validation
    if (string.IsNullOrEmpty(user.Email) || !user.Email.Contains("@"))
    {
        return Results.BadRequest("Valid email is required");
    }

    // Check if email already exists
    if (await db.Users.AnyAsync(u => u.Email == user.Email))
    {
        return Results.Conflict("A user with this email already exists");
    }

    var userDb = new User
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email
    };

    db.Users.Add(userDb);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{userDb.Id}", userDb);
})
.WithName("CreateUser")
.Produces<User>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status409Conflict);

// PUT update user
app.MapPut("/users/{id}", async (int id, UserUpdate updatedUser, UserDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound();

    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateUser")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status409Conflict);

// DELETE user
app.MapDelete("/users/{id}", async (int id, UserDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound();

    // Soft delete - just mark as inactive
    user.Active = false;
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeactivateUser")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// Hard DELETE user (for admins)
app.MapDelete("/users/{id}/hard", async (int id, UserDbContext db) =>
{
    var user = await db.Users.FindAsync(id);
    if (user == null) return Results.NotFound();

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteUser")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();