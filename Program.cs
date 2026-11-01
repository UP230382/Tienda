
using Microsoft.OpenApi;
using ProductosAPI.Interfaces;
using ProductosAPI.Models;
using ProductosAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProductosAPI",
        Version = "v1",
        Description = "API REST para la gestión de productos"
    });
});

// 2. Registrar el repositorio
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

// 3. Habilitar Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductosAPI v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

// CONSULTAR TODOS
app.MapGet("/api/productos", (IProductoRepository repo) =>
{
    var productos = repo.ObtenerTodos();
    return Results.Ok(productos);
})
.WithName("ObtenerProductos")
.WithSummary("Obtiene todos los productos")
.Produces<List<Producto>>(200);

// BUSCAR POR ID

app.MapGet("/api/productos/{id}", (int id, IProductoRepository repo) =>
{
    var producto = repo.ObtenerPorId(id);

    if (producto == null)
        return Results.NotFound("Producto no encontrado");

    return Results.Ok(producto);
})
.WithName("ObtenerProductoPorId")
.WithSummary("Obtiene un producto por su ID")
.Produces<Producto>(200)
.Produces(404);

// INSERTAR

app.MapPost("/api/productos", (Producto producto, IProductoRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(producto.Nombre) ||
        producto.Precio < 0)
    {
        return Results.BadRequest("Datos inválidos");
    }

    int id = repo.Insertar(producto);

    return Results.Created(
        $"/api/productos/{id}",
        repo.ObtenerPorId(id)
    );
})
.WithName("CrearProducto")
.WithSummary("Crea un nuevo producto")
.Accepts<Producto>("application/json")
.Produces<Producto>(201)
.Produces(400);

//ACTUALIZAR

app.MapPut("/api/productos/{id}", (int id, Producto producto, IProductoRepository repo) =>
{
    if (id != producto.Id)
        return Results.BadRequest("El ID no coincide");

    if (string.IsNullOrWhiteSpace(producto.Nombre) ||
        producto.Precio < 0)
    {
        return Results.BadRequest("Datos inválidos");
    }

    bool actualizado = repo.Actualizar(producto);

    return actualizado
        ? Results.Ok("Producto actualizado correctamente")
        : Results.NotFound("Producto no encontrado");
})
.WithName("ActualizarProducto")
.WithSummary("Actualiza un producto existente")
.Accepts<Producto>("application/json")
.Produces(200)
.Produces(400)
.Produces(404);

//ELIMINAR

app.MapDelete("/api/productos/{id}", (int id, IProductoRepository repo) =>
{
    bool eliminado = repo.Eliminar(id);

    return eliminado
        ? Results.Ok("Producto eliminado correctamente")
        : Results.NotFound("Producto no encontrado");
})
.WithName("EliminarProducto")
.WithSummary("Elimina un producto por su ID")
.Produces(200)
.Produces(404);

app.Run();
