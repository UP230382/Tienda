using MySqlConnector;
using ProductosAPI.Interfaces;
using ProductosAPI.Models;

namespace ProductosAPI.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("MySQL")!;
        }

        // CONSULTAR TODOS
        public List<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql = "SELECT * FROM Productos";

            using var command = new MySqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Descripcion = reader.GetString("Descripcion"),
                    Precio = reader.GetDecimal("Precio")
                });
            }

            return productos;
        }

        // BUSCAR POR ID
        public Producto? ObtenerPorId(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql = "SELECT * FROM Productos WHERE Id = @Id";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Producto
                {
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.GetString("Nombre"),
                    Descripcion = reader.GetString("Descripcion"),
                    Precio = reader.GetDecimal("Precio")
                };
            }

            return null;
        }

        // INSERTAR
        public int Insertar(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql = @"INSERT INTO Productos
                (Nombre, Descripcion, Precio)
                VALUES (@Nombre, @Descripcion, @Precio)";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Nombre", producto.Nombre);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("@Precio", producto.Precio);

            command.ExecuteNonQuery();

            return (int)command.LastInsertedId;
        }

        // ACTUALIZAR
        public bool Actualizar(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql = @"UPDATE Productos
                SET Nombre = @Nombre,
                    Descripcion = @Descripcion,
                    Precio = @Precio
                WHERE Id = @Id";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", producto.Id);
            command.Parameters.AddWithValue("@Nombre", producto.Nombre);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("@Precio", producto.Precio);

            return command.ExecuteNonQuery() > 0;
        }

        // ELIMINAR
        public bool Eliminar(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string sql = "DELETE FROM Productos WHERE Id = @Id";

            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }
    }
}