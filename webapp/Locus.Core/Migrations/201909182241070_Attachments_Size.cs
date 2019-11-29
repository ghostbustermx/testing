namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Attachments_Size : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attachment", "Size", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attachment", "Size");
        }
    }
}
