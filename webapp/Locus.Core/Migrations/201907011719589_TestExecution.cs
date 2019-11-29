namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestExecution : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestExecution",
                c => new
                    {
                        Test_Execution_Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Creator = c.String(),
                        Name = c.String(),
                        State = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Test_Execution_Id);
            
            AddColumn("dbo.TestResult", "Test_Execution_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestResult", "Test_Execution_Id");
            DropTable("dbo.TestExecution");
        }
    }
}
