using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BlagaUniversity.DAL;
using BlagaUniversity.Models;
using BlagaUniversity.ViewModels;
using WebGrease.Css.Extensions;

namespace BlagaUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private readonly UniversityContext _universityContext = new UniversityContext();

        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData
            {
                Instructors = _universityContext.Instructors
                    .Include(i => i.OfficeAssignment)
                    .Include(i => i.Courses.Select(c => c.Department))
                    .OrderBy(i => i.LastName)
            };

            if (id.HasValue)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.SelectedInstructor = viewModel.Instructors.First(i => i.ID == id);
                viewModel.Courses = viewModel.SelectedInstructor.Courses;
            }

            if (courseID.HasValue)
            {
                // Explicit loading
                ViewBag.courseID = courseID.Value;
                viewModel.SelectedCourse = viewModel.Courses.First(c => c.CourseID == courseID);
                _universityContext.Entry(viewModel.SelectedCourse).Collection(c => c.Enrollements).Load();
                viewModel.SelectedCourse.Enrollements.ForEach(
                    e => _universityContext.Entry(e).Reference(le => le.Student).Load());
                viewModel.Enrollments = viewModel.SelectedCourse.Enrollements;
            }

            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = _universityContext.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();
            PopulateAssigmentCourseData(instructor);

            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,LastName,FirstMidName,HireDate,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            UpdateInstructorCourses(instructor, selectedCourses);

            if (ModelState.IsValid)
            {
                _universityContext.Instructors.Add(instructor);
                _universityContext.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssigmentCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructor = FetchInstructor(id);
            PopulateAssigmentCourseData(instructor);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCourses)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var instructorToUpdate = FetchInstructor(id);

            if (TryUpdateModel(instructorToUpdate, "", new[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }

                UpdateInstructorCourses(instructorToUpdate, selectedCourses);

                try
                {
                    _universityContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Something went wrong");
                }
            }

            return View(instructorToUpdate);
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = _universityContext.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = FetchInstructor(id);
            _universityContext.Instructors.Remove(instructor);

            foreach (var instructorsDepartment in _universityContext.Departments.Where(d => d.InstructorID == instructor.ID))
            {
                instructorsDepartment.InstructorID = null;
            }

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

        private Instructor FetchInstructor(int? id)
        {
            Instructor instructor = _universityContext.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Single(i => i.ID == id);

            return instructor;
        }

        private void PopulateAssigmentCourseData(Instructor instructor)
        {
            var instructorCoursesIds = instructor.Courses.Select(c => c.CourseID);

            var assignedCourseData = from c in _universityContext.Courses
                                     select
                                         new AssignedCourseData()
                                         {
                                             CourseID = c.CourseID,
                                             Title = c.Title,
                                             Assigned = instructorCoursesIds.Contains(c.CourseID)
                                         };


            ViewBag.Courses = assignedCourseData.ToList();
        }

        private void UpdateInstructorCourses(Instructor instructorToUpdate, string[] selectedCoursesIds)
        {
            if (selectedCoursesIds == null || !selectedCoursesIds.Any())
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            instructorToUpdate.Courses = instructorToUpdate.Courses ?? new List<Course>();

            var parsedIds = selectedCoursesIds.Select(int.Parse);
            var instructorCoursesIds = instructorToUpdate.Courses.Select(c => c.CourseID).ToList();
            var selectedCourses = _universityContext.Courses.Where(c => parsedIds.Contains(c.CourseID));

            // remove courses
            foreach (var courseToDelete in instructorCoursesIds.Where(courseId => !selectedCourses.Any(c => c.CourseID == courseId)))
            {
                instructorToUpdate.Courses.Remove(instructorToUpdate.Courses.Single(c => c.CourseID == courseToDelete));
            }

            // add courses
            foreach (var course in selectedCourses.Where(c => instructorCoursesIds.All(courseId => c.CourseID != courseId)))
            {
                instructorToUpdate.Courses.Add(course);
            }
        }
    }
}
