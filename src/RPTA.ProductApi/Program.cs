using Microsoft.EntityFrameworkCore;
using RPTA.ProductApi.Data;
using RPTA.ProductApi.Models;
using RPTA.ProductApi.Request;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RPTA.ProductApi", Version = "v1" });
});

builder.AddSqlServerDbContext<ProductDbContext>(
    connectionName: "productdb");

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Create database if not exists
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

// Define API endpoints
// GET all products
app.MapGet("/products", async (ProductDbContext db) =>
    await db.Products.ToListAsync())
.WithName("GetAllProducts")
.Produces<List<Product>>(StatusCodes.Status200OK);

// GET product by id
app.MapGet("/products/{id}", async (int id, ProductDbContext db) =>
    await db.Products.FindAsync(id) is Product product
        ? Results.Ok(product)
        : Results.NotFound())
.WithName("GetProductById")
.Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

// POST new product
app.MapPost("/products", async (ProductRequest product, ProductDbContext db) =>
{
    var productDb = new Product
    {
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Stock = product.Stock
    };
    db.Products.Add(productDb);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{productDb.Id}", productDb);
})
.WithName("CreateProduct")
.Produces<Product>(StatusCodes.Status201Created);

// PUT update product
app.MapPut("/products/{id}", async (int id, ProductRequest updatedProduct, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    product.Name = updatedProduct.Name;
    product.Description = updatedProduct.Description;
    product.Price = updatedProduct.Price;
    product.Stock = updatedProduct.Stock;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateProduct")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

// DELETE product
app.MapDelete("/products/{id}", async (int id, ProductDbContext db) =>
{
    var product = await db.Products.FindAsync(id);
    if (product == null) return Results.NotFound();

    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteProduct")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound);

app.Run();