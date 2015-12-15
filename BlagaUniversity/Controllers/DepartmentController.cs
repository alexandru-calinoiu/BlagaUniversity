using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using BlagaUniversity.DAL;
using BlagaUniversity.Models;

namespace BlagaUniversity.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly UniversityContext _universityContext = new UniversityContext();

        // GET: Department
        public async Task<ActionResult> Index(int? page)
        {
            var departments = _universityContext.Departments.Include(d => d.Administrator);
            return View(await departments.ToListAsync());
        }

        // GET: Department/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var department = await FetchDepartment(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            ViewBag.InstructorID = new SelectList(_universityContext.Instructors, "ID", "FullName");
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentID,Name,Budget,StartDate,InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                _universityContext.Departments.Add(department);
                await _universityContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(_universityContext.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Department/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await _universityContext.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstructorID = new SelectList(_universityContext.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDepartment(int? id, byte[] rowVersion)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var departmentToUpdate = await _universityContext.Departments.FindAsync(id);
            var includeProperties = new[] { "DepartmentID", "Name", "Budget", "StartDate", "InstructorID", "RowVersion" };

            if (departmentToUpdate == null)
            {
                departmentToUpdate = new Department();
                TryUpdateModel(departmentToUpdate, includeProperties);
                ModelState.AddModelError(string.Empty, "Unable to save change the Department has already been deleted by another user.");
            }
            else if (TryUpdateModel(departmentToUpdate, includeProperties))
            {
                try
                {
                    _universityContext.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await _universityContext.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Department)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save change the Department has already been deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", "Current value: " + databaseValues.Name);
                        }

                        ModelState.AddModelError(string.Empty, "The record was already edit by some other user");
                        departmentToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Something went wrong.");
                }
            }

            ViewBag.InstructorID = new SelectList(_universityContext.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        // GET: Department/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await FetchDepartment(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = await _universityContext.Departments.FindAsync(id);
            _universityContext.Departments.Remove(department);
            await _universityContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _universityContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private async Task<Department> FetchDepartment(int? id)
        {
            var department = await _universityContext
                .Departments
                .Include(d => d.Administrator)
                .SingleAsync(d => d.DepartmentID == id);
            return department;
        }
    }
}
