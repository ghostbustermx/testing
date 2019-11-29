namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestEnvironment", "ProjectId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestEnvironment", "ProjectId");
        }
    }
}
