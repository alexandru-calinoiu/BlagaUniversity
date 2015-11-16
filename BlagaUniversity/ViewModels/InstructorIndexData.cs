using System.Collections.Generic;
using BlagaUniversity.Models;

namespace BlagaUniversity.ViewModels
{
    public class InstructorIndexData
    {
        public Instructor SelectedInstructor { get; set; }
        public Course SelectedCourse { get; set; }
        public IEnumerable<Instructor> Instructors { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Enrollment> Enrollments { get; set; }
    }
}