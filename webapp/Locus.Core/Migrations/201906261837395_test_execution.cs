namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test_execution : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExecutionGroup",
                c => new
                    {
                        Execution_Group_Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Creation_Date = c.DateTime(),
                        Creator = c.String(nullable: false),
                        LastEditor = c.String(),
                        lastEditDate = c.DateTime(),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Execution_Group_Id);
            
            CreateTable(
                "dbo.ExecutionTestEvidence",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Execution_Group_Id = c.Int(nullable: false),
                        Tc_Id = c.Int(),
                        Tp_Id = c.Int(),
                        Ts_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.ExecutionGroup", t => t.Execution_Group_Id)
                .Index(t => t.Execution_Group_Id);
            
            CreateTable(
                "dbo.TestResult",
                c => new
                    {
                        Test_Result_Id = c.Int(nullable: false, identity: true),
                        Execution_Group_Id = c.Int(nullable: false),
                        Execution_Result = c.String(),
                        Execution_Date = c.DateTime(nullable: false),
                        Status = c.String(),
                        Tester = c.String(),
                        Test_Case_Id = c.Int(),
                        Test_Scenario_Id = c.Int(),
                        Test_Procedure_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Test_Result_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExecutionTestEvidence", "Execution_Group_Id", "dbo.ExecutionGroup");
            DropIndex("dbo.ExecutionTestEvidence", new[] { "Execution_Group_Id" });
            DropTable("dbo.TestResult");
            DropTable("dbo.ExecutionTestEvidence");
            DropTable("dbo.ExecutionGroup");
        }
    }
}
