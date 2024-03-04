using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Crud
{
    public static class DepartmentsCrud
    {
        private static readonly string _connectionString;

        static DepartmentsCrud()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("defaultCrud");
        }

        public static List<Department> GetDepartments()
        {
            var depts = new List<Department>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetDepartments";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        depts.Add(new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }
                }
            }

            return depts;
        }

        public static Department GetDepartmentById(int id)
        {
            var depts = GetDepartments();
            var department = depts.Find(x => x.Id == id);
            return department;
        }
    }
}
