namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedversionTestExecution : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestExecution", "Version", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestExecution", "Version");
        }
    }
}
