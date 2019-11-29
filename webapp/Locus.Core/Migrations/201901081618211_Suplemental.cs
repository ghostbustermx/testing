namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Suplemental : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Test_Procedure_Test_Suplemental",
                c => new
                    {
                        Test_Suplemental_Id = c.Int(nullable: false),
                        Test_Procedure_Id = c.Int(),
                        Test_Scenario_Id = c.Int(),
                    })
                .ForeignKey("dbo.TestProcedure", t => t.Test_Procedure_Id)
                .ForeignKey("dbo.TestScenario", t => t.Test_Scenario_Id)
                .ForeignKey("dbo.TestSuplemental", t => t.Test_Suplemental_Id)
                .Index(t => t.Test_Suplemental_Id)
                .Index(t => t.Test_Procedure_Id)
                .Index(t => t.Test_Scenario_Id);
            
            CreateTable(
                "dbo.TestSuplemental",
                c => new
                    {
                        Test_Suplemental_Id = c.Int(nullable: false, identity: true),
                        stp_number = c.String(maxLength: 50),
                        Title = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false),
                        Test_Procedure_Creator = c.String(nullable: false, maxLength: 50),
                        Creation_Date = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                        Project_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Test_Suplemental_Id);
            
            AddColumn("dbo.Steps", "Test_Suplemental_Id", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Test_Procedure_Test_Suplemental", "Test_Suplemental_Id", "dbo.TestSuplemental");
            DropForeignKey("dbo.Test_Procedure_Test_Suplemental", "Test_Scenario_Id", "dbo.TestScenario");
            DropForeignKey("dbo.Test_Procedure_Test_Suplemental", "Test_Procedure_Id", "dbo.TestProcedure");
            DropIndex("dbo.Test_Procedure_Test_Suplemental", new[] { "Test_Scenario_Id" });
            DropIndex("dbo.Test_Procedure_Test_Suplemental", new[] { "Test_Procedure_Id" });
            DropIndex("dbo.Test_Procedure_Test_Suplemental", new[] { "Test_Suplemental_Id" });
            DropColumn("dbo.Steps", "Test_Suplemental_Id");
            DropTable("dbo.TestSuplemental");
            DropTable("dbo.Test_Procedure_Test_Suplemental");
        }
    }
}
