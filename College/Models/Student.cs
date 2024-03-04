namespace WebApplication1.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Department Department { get; set; }
        public List<Course> Courses { get; set; }
        public int DepartmentId { get; set; }

        public Student()
        {
            Courses = new List<Course>();
        }
    }
}
