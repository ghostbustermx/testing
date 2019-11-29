namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class runner_version : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Runners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Identifier = c.String(nullable: false, maxLength: 100),
                        OS = c.String(nullable: false, maxLength: 100),
                        IsConnected = c.Boolean(),
                        Status = c.Boolean(nullable: false),
                        MAC = c.String(nullable: false, maxLength: 25),
                        Description = c.String(nullable: false, maxLength: 200),
                        IPAddress = c.String(maxLength: 20),
                        Tags = c.String(maxLength: 200),
                        Creation_Date = c.DateTime(nullable: false),
                        Last_Connection_Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Scripts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Extension = c.String(nullable: false, maxLength: 10),
                        ScriptsGroup_Id = c.Int(nullable: false),
                        Path = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScriptsGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        projectId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Release = c.String(nullable: false, maxLength: 100),
                        Creator = c.String(nullable: false, maxLength: 50),
                        Last_Editor = c.String(maxLength: 50),
                        Creation_Date = c.DateTime(nullable: false),
                        Priority = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ExecutionGroup", "RunnerId", c => c.Int());
            AddColumn("dbo.ExecutionGroup", "isAutomated", c => c.Boolean(nullable: false));
            AddColumn("dbo.ExecutionTestEvidence", "Ta_Id", c => c.Int());
            AddColumn("dbo.TestProcedure", "Script_Id", c => c.Int());
            AlterColumn("dbo.ExecutionGroup", "TestEnvironmentId", c => c.Int());
            AlterColumn("dbo.TestProcedure", "Expected_Result", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TestProcedure", "Expected_Result", c => c.String(nullable: false));
            AlterColumn("dbo.ExecutionGroup", "TestEnvironmentId", c => c.Int(nullable: false));
            DropColumn("dbo.TestProcedure", "Script_Id");
            DropColumn("dbo.ExecutionTestEvidence", "Ta_Id");
            DropColumn("dbo.ExecutionGroup", "isAutomated");
            DropColumn("dbo.ExecutionGroup", "RunnerId");
            DropTable("dbo.ScriptsGroups");
            DropTable("dbo.Scripts");
            DropTable("dbo.Runners");
        }
    }
}
