using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BlagaUniversity.DAL;
using BlagaUniversity.Models;

namespace BlagaUniversity.Controllers
{
    public class CourseController : Controller
    {
        private readonly UniversityContext _universityContext = new UniversityContext();

        // GET: Course
        public ActionResult Index()
        {
            var courses = _universityContext.Courses.Include(c => c.Department);
            return View(courses.ToList());
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = _universityContext.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            PopulateDepartmentDropDownList();
            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
            if (ModelState.IsValid)
            {
                _universityContext.Courses.Add(course);
                _universityContext.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(_universityContext.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = _universityContext.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            PopulateDepartmentDropDownList(course.DepartmentID);
            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditCourse(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var courseToUpdate = _universityContext.Courses.Find(id);
            if (TryUpdateModel(courseToUpdate, "", new[] { "CourseID", "Credits", "DepartmentID" }))
            {
                try
                {
                    _universityContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Something went wrong.");
                }
            }

            PopulateDepartmentDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = _universityContext.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = _universityContext.Courses.Find(id);
            _universityContext.Courses.Remove(course);
            _universityContext.SaveChanges();
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

        private void PopulateDepartmentDropDownList(object selectedDepartmentID = null)
        {
            var departmentQuery = from d in _universityContext.Departments orderby d.Name select d;
            ViewBag.DepartmentID = new SelectList(departmentQuery, "DepartmentID", "Name", selectedDepartmentID);
        }
    }
}
