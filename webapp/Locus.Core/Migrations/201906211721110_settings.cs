namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        UserName = c.String(nullable: false, maxLength: 70),
                        UIMode = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.UserName);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Setting");
        }
    }
}
