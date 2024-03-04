using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Crud
{
    public static class CoursesCrud
    {
        private static readonly string _connectionString;

        static CoursesCrud()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("defaultCrud");
        }
        public static List<Course> GetCourses()
        {
            var courses = new List<Course>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetCourses";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name")
                        });
                    }
                }
            }

            return courses;
        }

        public static Course GetCourseById(int id)
        {
            var courses = GetCourses();
            var course = courses.Find(x => x.Id == id);
            return course;
        }
    }
}
