namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testenvironment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestEnvironment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Server = c.String(nullable: false, maxLength: 100),
                        Processor = c.String(nullable: false, maxLength: 100),
                        RAM = c.String(nullable: false, maxLength: 100),
                        HardDisk = c.String(nullable: false, maxLength: 100),
                        OS = c.String(nullable: false, maxLength: 100),
                        ServerSoftwareDevs = c.String(),
                        ServerSoftwareTest = c.String(),
                        Database = c.String(maxLength: 100),
                        URL = c.String(),
                        SiteType = c.String(nullable: false, maxLength: 100),
                        Notes = c.String(),
                        Creator = c.String(nullable: false, maxLength: 100),
                        Last_Editor = c.String(nullable: false, maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TestEnvironment");
        }
    }
}
