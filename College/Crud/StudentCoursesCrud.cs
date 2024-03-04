using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Crud
{
    public static class StudentCoursesCrud
    {
        private static readonly string _connectionString;

        static StudentCoursesCrud()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("defaultCrud");
        }
        public static List<StudentCourse> GetStudentCourses()
        {
            var courses = new List<StudentCourse>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetStudentCourses";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        courses.Add(new StudentCourse
                        {
                            CourseId = reader.GetInt32("CoursesId"),
                            StudentId = reader.GetInt32("StudentsId"),
                            CourseName = reader.GetString("CourseName"),
                            StudentName = reader.GetString("StudentName"),
                        });
                    }
                }
            }

            return courses;
        }
        public static List<Course> GetStudentCoursesByStdId(int id)
        {
            var Stdcourses = GetStudentCourses();
            Stdcourses = Stdcourses.Where(c => c.StudentId == id).ToList();

            var course = new List<Course>();
            foreach (var c in Stdcourses)
            {
                course.Add(new Course
                {
                    Id = c.CourseId,
                    Name = c.CourseName
                });
            }
            return course;
        }

    }
}
