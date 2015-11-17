namespace BlagaUniversity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLocationToDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Department", "Location", c => c.String(maxLength: 50));
            AlterStoredProcedure(
                "dbo.Department_Insert",
                p => new
                    {
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        Location = p.String(maxLength: 50),
                        InstructorID = p.Int(),
                    },
                body:
                    @"INSERT [dbo].[Department]([Name], [Budget], [StartDate], [Location], [InstructorID])
                      VALUES (@Name, @Budget, @StartDate, @Location, @InstructorID)
                      
                      DECLARE @DepartmentID int
                      SELECT @DepartmentID = [DepartmentID]
                      FROM [dbo].[Department]
                      WHERE @@ROWCOUNT > 0 AND [DepartmentID] = scope_identity()
                      
                      SELECT t0.[DepartmentID]
                      FROM [dbo].[Department] AS t0
                      WHERE @@ROWCOUNT > 0 AND t0.[DepartmentID] = @DepartmentID"
            );
            
            AlterStoredProcedure(
                "dbo.Department_Update",
                p => new
                    {
                        DepartmentID = p.Int(),
                        Name = p.String(maxLength: 50),
                        Budget = p.Decimal(precision: 19, scale: 4, storeType: "money"),
                        StartDate = p.DateTime(),
                        Location = p.String(maxLength: 50),
                        InstructorID = p.Int(),
                    },
                body:
                    @"UPDATE [dbo].[Department]
                      SET [Name] = @Name, [Budget] = @Budget, [StartDate] = @StartDate, [Location] = @Location, [InstructorID] = @InstructorID
                      WHERE ([DepartmentID] = @DepartmentID)"
            );
            
        }
        
        public override void Down()
        {
            DropColumn("dbo.Department", "Location");
            throw new NotSupportedException("Scaffolding create or alter procedure operations is not supported in down methods.");
        }
    }
}
