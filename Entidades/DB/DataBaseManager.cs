using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using Entidades.Excepciones;
using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Interfaces;

namespace Entidades.DataBase
{
    public static class DataBaseManager
    {
        private static SqlConnection connection;
        private static string stringConnection;


        static DataBaseManager()
        {
            DataBaseManager.stringConnection = "Server=DESKTOP-JGCLFR3;Database=20230622SP;Trusted_Connection=True;";
            connection = new SqlConnection(stringConnection);
        }

        public static bool GuardarTicket<T>(string nombreEmpleado, T comida) where T : IComestible, new()
        {
            try
            {
                using(connection = new SqlConnection(stringConnection))
                {
                    string query = "INSERT INTO tickets (empleado, ticket) VALUES (@empleado,@ticket);";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@empleado", nombreEmpleado);
                    command.Parameters.AddWithValue("@ticket", comida.Ticket);

                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
            }
            catch (DataBaseManagerException ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt", true);
                throw new DataBaseManagerException("No se pudo escribir el ticket en la base", ex);
            }
        }

        public static string GetImagenComida(string tipo)
        {
            try
            {
                using (connection = new SqlConnection(stringConnection))
                {
                    string query = "SELECT * FROM comidas WHERE tipo_comida = @tipo";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@tipo", tipo);

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        throw new ComidaInvalidaException("Comida no encontrada");
                    }

                    if (reader.Read())
                    {
                        return reader["imagen"].ToString();
                    }
                }
                throw new ComidaInvalidaException("Comida no encontrada");
            }
            catch (ComidaInvalidaException ex)
            {
                FileManager.Guardar(ex.Message, "logs.txt", false);
                throw;
            }
        }
    }
}
