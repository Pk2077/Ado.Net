using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly Context context;

        public DepartmentsController(Context context )
        {
            this.context = context;
        }
        public IActionResult Index()
        {
           var depts =  this.context.Departments.ToList();
            return View(depts);
        }
    }
}
