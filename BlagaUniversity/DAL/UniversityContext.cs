using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using BlagaUniversity.Models;

namespace BlagaUniversity.DAL
{
    public class UniversityContext : DbContext
    {
        public UniversityContext() : base("UniversityContext")
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}