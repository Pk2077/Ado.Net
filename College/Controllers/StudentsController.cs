using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Crud;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class StudentsController : Controller
    {
        private readonly Context context;

        public StudentsController(Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            var students = StudentsCrud.GetStudents();
            return View(students);
        }
        public IActionResult New()
        {
            var courses = CoursesCrud.GetCourses();
            var depts = DepartmentsCrud.GetDepartments();
            var viewModel = new StudentFormViewModel
            {
                Courses = courses,
                Student = new Student(),
                Departments = depts
            };
            return View("StudentForm",viewModel);
        }
        [HttpPost]
        public IActionResult Save(StudentFormViewModel model, int[] selectedCourses)
        {
            if(model.Student.Id == 0)
            {
                var student = model.Student;
                StudentsCrud.InsertStudentsWithCourses(student, selectedCourses);

                return RedirectToAction("Index", "Students");
            }
            else
            {
                var StudentInDb = StudentsCrud.GetStudentById(model.Student.Id);
                if (StudentInDb == null)
                {
                    return NotFound();
                }
                StudentInDb.Id = model.Student.Id;
                StudentInDb.Name = model.Student.Name;
                StudentInDb.DepartmentId = model.Student.DepartmentId;
                StudentInDb.Courses.Clear();

                if (selectedCourses != null)
                {
                        var course = StudentCoursesCrud.GetStudentCoursesByStdId(StudentInDb.Id);
                        if (course != null)
                        {
                            StudentInDb.Courses = course;
                        }
                }
                StudentsCrud.InsertStudentsWithCourses(StudentInDb, selectedCourses);

                return RedirectToAction("Index", "Students");
            }

        }

        public IActionResult Edit(int id)
        {
            var student = StudentsCrud.GetStudentById(id);
            if (student == null)
            {
                return NotFound();
            }

            var viewModel = new StudentFormViewModel
            {
                Student = student,
                Courses = CoursesCrud.GetCourses(),
                Departments =DepartmentsCrud.GetDepartments()
            };

            return View(viewModel);
        }


        public IActionResult Details(int id)
        {
            var student = StudentsCrud.GetStudentById(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }
        public IActionResult Delete(int id)
        {
            var student = StudentsCrud.GetStudentById(id);
            StudentsCrud.DeleteStudentWithCourses(student.Id,student.Courses);

            return RedirectToAction("Index", "Students");
        }
    }
}
