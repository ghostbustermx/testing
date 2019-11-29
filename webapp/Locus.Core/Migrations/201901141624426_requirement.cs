namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requirement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Requirement", "Acceptance_Criteria", c => c.String());
            AddColumn("dbo.Requirement", "Release", c => c.String());
            AlterColumn("dbo.Requirement", "Axosoft_Task_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Requirement", "Axosoft_Task_Id", c => c.Int());
            DropColumn("dbo.Requirement", "Release");
            DropColumn("dbo.Requirement", "Acceptance_Criteria");
        }
    }
}
