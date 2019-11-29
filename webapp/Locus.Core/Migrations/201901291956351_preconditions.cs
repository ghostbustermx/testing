namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preconditions : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TestProcedure", "Preconditions");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestProcedure", "Preconditions", c => c.String());
        }
    }
}
