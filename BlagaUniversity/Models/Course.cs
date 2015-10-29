using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlagaUniversity.Models
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollements { get; set; }
    }
}