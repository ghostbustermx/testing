namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestResultHolderandTitles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestResult", "Identifier_number", c => c.String());
            AddColumn("dbo.TestResult", "Title", c => c.String(maxLength: 500));
            AddColumn("dbo.TestResult", "PhotoUrl", c => c.String(maxLength: 500));
            AddColumn("dbo.TestResult", "IsTaken", c => c.Boolean(nullable: false));
            AddColumn("dbo.TestResult", "CurrentHolder", c => c.String());
            AlterColumn("dbo.TestResult", "Execution_Date", c => c.DateTime());
        }

        public override void Down()
        {
            AlterColumn("dbo.TestResult", "Execution_Date", c => c.DateTime(nullable: false));
            DropColumn("dbo.TestResult", "Title");
            DropColumn("dbo.TestResult", "PhotoUrl");
            DropColumn("dbo.TestResult", "IsTaken");
            DropColumn("dbo.TestResult", "CurrentHolder");
            DropColumn("dbo.TestResult", "Identifier_number");

        }
    }
}
