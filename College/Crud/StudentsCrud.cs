using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.Models;

namespace WebApplication1.Crud
{
    public static class StudentsCrud
    {
        private static readonly string _connectionString;

        static StudentsCrud()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("defaultCrud");

        }

        public static List<Student> GetStudents()
        {
            var students = new List<Student>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetStudentData";

                    var reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        students.Add(new Student()
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("StudentName"),
                            DepartmentId = reader.GetInt32("DepartmentId"),
                            Department = DepartmentsCrud.GetDepartmentById(reader.GetInt32("DepartmentId")),
                            Courses = StudentCoursesCrud.GetStudentCoursesByStdId(reader.GetInt32("Id"))
                        });
                    }
                    return students;
                }
            }
        }
        public static Student GetStudentById(int id)
        {
            Student student = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "GetStudentById";
                    command.Parameters.AddWithValue("@StudentId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32("StudentsId"),
                                Name = reader.GetString("StudentName"),
                                DepartmentId = reader.GetInt32("DepartmentId"),
                                Department = DepartmentsCrud.GetDepartmentById(reader.GetInt32("DepartmentId")),
                                Courses = StudentCoursesCrud.GetStudentCoursesByStdId(reader.GetInt32("StudentsId"))
                            };
                        }
                    }
                }
            }

            return student;
        }
        public static void InsertStudentsWithCourses(Student student,int[] CourseIds)
        {
            string courseIds;
            if (CourseIds != null)
            {
                 courseIds = string.Join(",", CourseIds);
            }
            else
            {
                courseIds = string.Empty;
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                { if(student.Id == 0)
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "InsertStudentWithCourses";

                        command.Parameters.AddWithValue("@DepartmentId", student.DepartmentId);
                        command.Parameters.AddWithValue("@Name", student.Name);
                        command.Parameters.AddWithValue("@CourseIds", courseIds);

                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "UpdateStudentWithCourses";

                        command.Parameters.AddWithValue("@DepartmentId", student.DepartmentId);
                        command.Parameters.AddWithValue("@Name", student.Name);
                        command.Parameters.AddWithValue("@CourseIds", courseIds);
                        command.Parameters.AddWithValue("@StudentId", student.Id);

                        command.ExecuteNonQuery();
                    }
                    
                }
            }
        }

        public static void DeleteStudentWithCourses(int StdId, List<Course> Courses)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    if (StdId != 0)
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "DeleteStudent";
                        command.Parameters.AddWithValue("@StudentId", StdId);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "DeleteStudentCourse";
                        foreach (Course course in Courses)
                        {
                            command.Parameters.AddWithValue("@StudentId", StdId);
                            command.Parameters.AddWithValue("@CourseId", course.Id);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
                }
            }
        }

    }
}
