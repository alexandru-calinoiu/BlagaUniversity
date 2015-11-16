namespace BlagaUniversity.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCourseForeingKey : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.Course", "DepartmentID", "dbo.Department", "DepartmentID", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Course", "DepartmentID", "dbo.Department");
        }
    }
}
