namespace Locus.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupIdOnTestExecution : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestExecution", "Execution_Group_Id", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TestExecution", "Execution_Group_Id");
        }
    }
}
