namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChangeLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        Version = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        User = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Test_ChangeLog",
                c => new
                    {
                        Change_Log_Id = c.Int(nullable: false),
                        Project_Id = c.Int(),
                        Requirement_Id = c.Int(),
                        Test_Case_Id = c.Int(),
                        Test_Scenario_Id = c.Int(),
                        Test_Procedure_Id = c.Int(),
                        Test_Suplemental_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Change_Log_Id)
                .ForeignKey("dbo.ChangeLog", t => t.Change_Log_Id)
                .ForeignKey("dbo.Project", t => t.Project_Id)
                .ForeignKey("dbo.Requirement", t => t.Requirement_Id)
                .ForeignKey("dbo.TestCase", t => t.Test_Case_Id)
                .ForeignKey("dbo.TestProcedure", t => t.Test_Procedure_Id)
                .ForeignKey("dbo.TestScenario", t => t.Test_Scenario_Id)
                .ForeignKey("dbo.TestSuplemental", t => t.Test_Suplemental_Id)
                .Index(t => t.Change_Log_Id)
                .Index(t => t.Project_Id)
                .Index(t => t.Requirement_Id)
                .Index(t => t.Test_Case_Id)
                .Index(t => t.Test_Scenario_Id)
                .Index(t => t.Test_Procedure_Id)
                .Index(t => t.Test_Suplemental_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Test_ChangeLog", "Test_Suplemental_Id", "dbo.TestSuplemental");
            DropForeignKey("dbo.Test_ChangeLog", "Test_Scenario_Id", "dbo.TestScenario");
            DropForeignKey("dbo.Test_ChangeLog", "Test_Procedure_Id", "dbo.TestProcedure");
            DropForeignKey("dbo.Test_ChangeLog", "Test_Case_Id", "dbo.TestCase");
            DropForeignKey("dbo.Test_ChangeLog", "Requirement_Id", "dbo.Requirement");
            DropForeignKey("dbo.Test_ChangeLog", "Project_Id", "dbo.Project");
            DropForeignKey("dbo.Test_ChangeLog", "Change_Log_Id", "dbo.ChangeLog");
            DropIndex("dbo.Test_ChangeLog", new[] { "Test_Suplemental_Id" });
            DropIndex("dbo.Test_ChangeLog", new[] { "Test_Procedure_Id" });
            DropIndex("dbo.Test_ChangeLog", new[] { "Test_Scenario_Id" });
            DropIndex("dbo.Test_ChangeLog", new[] { "Test_Case_Id" });
            DropIndex("dbo.Test_ChangeLog", new[] { "Requirement_Id" });
            DropIndex("dbo.Test_ChangeLog", new[] { "Project_Id" });
            DropIndex("dbo.Test_ChangeLog", new[] { "Change_Log_Id" });
            DropTable("dbo.Test_ChangeLog");
            DropTable("dbo.ChangeLog");
        }
    }
}
