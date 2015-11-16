namespace BlagaUniversity.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveEndDateFromDepartment : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Department", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Department", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
