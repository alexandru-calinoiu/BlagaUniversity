using System.Linq;
using System.Web.Mvc;
using BlagaUniversity.DAL;
using BlagaUniversity.ViewModels;

namespace BlagaUniversity.Controllers
{
    public class HomeController : Controller
    {
        private readonly UniversityContext _universityContext = new UniversityContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var enrollmentDateGroups = from student in _universityContext.Students
                group student by student.EnrollmentDate
                into dateGroup
                select new EnrollmentDateGroup()
                {
                    EnrollmentDate = dateGroup.Key,
                    StudentCount = dateGroup.Count()
                };

            return View(enrollmentDateGroups.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _universityContext.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}