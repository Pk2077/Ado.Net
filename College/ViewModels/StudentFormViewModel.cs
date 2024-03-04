using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class StudentFormViewModel
    {
        public Student Student { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}
