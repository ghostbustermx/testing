namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Backup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 70),
                        Status = c.Boolean(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        Message = c.String(maxLength: 200),
                        GeneratedBy = c.String(nullable: false),
                        Creation_Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 70),
                        Status = c.Boolean(nullable: false),
                        Description = c.String(),
                        Image = c.String(maxLength: 50),
                        Axosoft_Project_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Requirement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Project_Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(),
                        Developer_Assigned = c.String(maxLength: 50),
                        Tester_Assigned = c.String(maxLength: 50),
                        Axosoft_Task_Id = c.Int(),
                        Status = c.Boolean(nullable: false),
                        req_number = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.Project_Id)
                .Index(t => t.Project_Id);
            
            CreateTable(
                "dbo.RequirementsTest",
                c => new
                    {
                        Requirement_Id = c.Int(nullable: false),
                        Test_Case_Id = c.Int(),
                        Test_Scenario_Id = c.Int(),
                        Test_Procedure_Id = c.Int(),
                    })
                .ForeignKey("dbo.TestCase", t => t.Test_Case_Id)
                .ForeignKey("dbo.TestProcedure", t => t.Test_Procedure_Id)
                .ForeignKey("dbo.TestScenario", t => t.Test_Scenario_Id)
                .ForeignKey("dbo.Requirement", t => t.Requirement_Id)
                .Index(t => t.Requirement_Id)
                .Index(t => t.Test_Case_Id)
                .Index(t => t.Test_Scenario_Id)
                .Index(t => t.Test_Procedure_Id);
            
            CreateTable(
                "dbo.TestCase",
                c => new
                    {
                        Test_Case_Id = c.Int(nullable: false, identity: true),
                        tc_number = c.String(maxLength: 20),
                        Test_Priority = c.String(nullable: false, maxLength: 20),
                        Title = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false),
                        Preconditions = c.String(nullable: false),
                        Test_Case_Creator = c.String(nullable: false, maxLength: 50),
                        Expected_Result = c.String(nullable: false),
                        Creation_Date = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Test_Case_Id);
            
            CreateTable(
                "dbo.Steps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Test_Case_Id = c.Int(),
                        Test_Procedure_Id = c.Int(),
                        Test_Scenario_Id = c.Int(),
                        number_steps = c.Int(nullable: false),
                        action = c.String(nullable: false),
                        creation_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestCase", t => t.Test_Case_Id)
                .ForeignKey("dbo.TestProcedure", t => t.Test_Procedure_Id)
                .ForeignKey("dbo.TestScenario", t => t.Test_Scenario_Id)
                .Index(t => t.Test_Case_Id)
                .Index(t => t.Test_Procedure_Id)
                .Index(t => t.Test_Scenario_Id);
            
            CreateTable(
                "dbo.TestProcedure",
                c => new
                    {
                        Test_Procedure_Id = c.Int(nullable: false, identity: true),
                        tp_number = c.String(maxLength: 20),
                        Test_Case_Id = c.Int(),
                        Test_Priority = c.String(nullable: false, maxLength: 20),
                        Title = c.String(maxLength: 150),
                        Description = c.String(),
                        Test_Procedure_Creator = c.String(nullable: false, maxLength: 50),
                        Expected_Result = c.String(nullable: false),
                        Creation_Date = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                        Preconditions = c.String(),
                    })
                .PrimaryKey(t => t.Test_Procedure_Id)
                .ForeignKey("dbo.TestCase", t => t.Test_Case_Id)
                .Index(t => t.Test_Case_Id);
            
            CreateTable(
                "dbo.TestTags",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Test_Procedure_Id = c.Int(),
                        Test_Case_Id = c.Int(),
                        Test_Scenario_Id = c.Int(),
                        Test_Suplemental_Id = c.Int(),
                    })
                .ForeignKey("dbo.tag", t => t.Tag_Id)
                .ForeignKey("dbo.TestCase", t => t.Test_Case_Id)
                .ForeignKey("dbo.TestProcedure", t => t.Test_Procedure_Id)
                .ForeignKey("dbo.TestScenario", t => t.Test_Scenario_Id)
                .Index(t => t.Tag_Id)
                .Index(t => t.Test_Procedure_Id)
                .Index(t => t.Test_Case_Id)
                .Index(t => t.Test_Scenario_Id);
            
            CreateTable(
                "dbo.tag",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 150),
                        Project_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Project", t => t.Project_Id)
                .Index(t => t.Project_Id);
            
            CreateTable(
                "dbo.TestScenario",
                c => new
                    {
                        Test_Scenario_Id = c.Int(nullable: false, identity: true),
                        ts_number = c.String(maxLength: 20),
                        Test_Priority = c.String(nullable: false, maxLength: 20),
                        Title = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false),
                        Preconditions = c.String(nullable: false),
                        Note = c.String(),
                        Test_Scenario_Creator = c.String(nullable: false, maxLength: 50),
                        Expected_Result = c.String(nullable: false),
                        Creation_Date = c.DateTime(),
                        Status = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Test_Scenario_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tag", "Project_Id", "dbo.Project");
            DropForeignKey("dbo.Requirement", "Project_Id", "dbo.Project");
            DropForeignKey("dbo.RequirementsTest", "Requirement_Id", "dbo.Requirement");
            DropForeignKey("dbo.TestProcedure", "Test_Case_Id", "dbo.TestCase");
            DropForeignKey("dbo.TestTags", "Test_Scenario_Id", "dbo.TestScenario");
            DropForeignKey("dbo.Steps", "Test_Scenario_Id", "dbo.TestScenario");
            DropForeignKey("dbo.RequirementsTest", "Test_Scenario_Id", "dbo.TestScenario");
            DropForeignKey("dbo.TestTags", "Test_Procedure_Id", "dbo.TestProcedure");
            DropForeignKey("dbo.TestTags", "Test_Case_Id", "dbo.TestCase");
            DropForeignKey("dbo.TestTags", "Tag_Id", "dbo.tag");
            DropForeignKey("dbo.Steps", "Test_Procedure_Id", "dbo.TestProcedure");
            DropForeignKey("dbo.RequirementsTest", "Test_Procedure_Id", "dbo.TestProcedure");
            DropForeignKey("dbo.Steps", "Test_Case_Id", "dbo.TestCase");
            DropForeignKey("dbo.RequirementsTest", "Test_Case_Id", "dbo.TestCase");
            DropIndex("dbo.tag", new[] { "Project_Id" });
            DropIndex("dbo.TestTags", new[] { "Test_Scenario_Id" });
            DropIndex("dbo.TestTags", new[] { "Test_Case_Id" });
            DropIndex("dbo.TestTags", new[] { "Test_Procedure_Id" });
            DropIndex("dbo.TestTags", new[] { "Tag_Id" });
            DropIndex("dbo.TestProcedure", new[] { "Test_Case_Id" });
            DropIndex("dbo.Steps", new[] { "Test_Scenario_Id" });
            DropIndex("dbo.Steps", new[] { "Test_Procedure_Id" });
            DropIndex("dbo.Steps", new[] { "Test_Case_Id" });
            DropIndex("dbo.RequirementsTest", new[] { "Test_Procedure_Id" });
            DropIndex("dbo.RequirementsTest", new[] { "Test_Scenario_Id" });
            DropIndex("dbo.RequirementsTest", new[] { "Test_Case_Id" });
            DropIndex("dbo.RequirementsTest", new[] { "Requirement_Id" });
            DropIndex("dbo.Requirement", new[] { "Project_Id" });
            DropTable("dbo.TestScenario");
            DropTable("dbo.tag");
            DropTable("dbo.TestTags");
            DropTable("dbo.TestProcedure");
            DropTable("dbo.Steps");
            DropTable("dbo.TestCase");
            DropTable("dbo.RequirementsTest");
            DropTable("dbo.Requirement");
            DropTable("dbo.Project");
            DropTable("dbo.Backup");
        }
    }
}
