namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOnSteps : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Steps", "type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Steps", "type");
        }
    }
}
