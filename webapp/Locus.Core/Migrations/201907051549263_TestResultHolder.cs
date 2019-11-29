namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestResultHolder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TestResult", "Execution_Group_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestResult", "Execution_Group_Id", c => c.Int(nullable: false));
        }
    }
}
