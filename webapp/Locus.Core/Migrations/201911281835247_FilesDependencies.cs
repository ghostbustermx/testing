namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FilesDependencies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FilesDependencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExecutionId = c.Int(nullable: false),
                        Path = c.String(),
                        Type = c.String(),
                        Name = c.String(),
                        GroupId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FilesDependencies");
        }
    }
}
