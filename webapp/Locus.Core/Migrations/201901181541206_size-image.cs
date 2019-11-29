namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sizeimage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Project", "Image", c => c.String(maxLength: 90));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Project", "Image", c => c.String(maxLength: 50));
        }
    }
}
