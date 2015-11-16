using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlagaUniversity.Models
{
    public class OfficeAssignment
    {
        [Key]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }

        [StringLength(50)]
        [DisplayName("Office Location")]
        public string Location { get; set; }

        public virtual Instructor Instructor { get; set; }
    }
}