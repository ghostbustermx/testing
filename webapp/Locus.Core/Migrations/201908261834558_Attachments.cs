namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Attachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Extention = c.String(),
                        Requirement_Id = c.Int(nullable: false),
                        TestSupplemental_Id = c.Int(nullable: false),
                        Test_Case_Id = c.Int(nullable: false),
                        Test_Procedure_Id = c.Int(nullable: false),
                        Test_Scenario_Id = c.Int(nullable: false),
                        Test_Result_Id = c.Int(nullable: false),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Attachment");
        }
    }
}
