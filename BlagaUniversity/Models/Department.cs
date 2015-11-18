using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlagaUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int? InstructorID { get; set; }

        public Instructor Administrator { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}