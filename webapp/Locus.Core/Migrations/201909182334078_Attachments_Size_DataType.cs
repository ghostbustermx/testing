namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Attachments_Size_DataType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Attachment", "Size", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Attachment", "Size", c => c.Single(nullable: false));
        }
    }
}
