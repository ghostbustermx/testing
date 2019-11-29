namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Status_TPSTP : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Test_Procedure_Test_Suplemental", "Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Test_Procedure_Test_Suplemental", "Status");
        }
    }
}
