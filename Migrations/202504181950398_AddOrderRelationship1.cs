namespace OnlineMusicStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrderRelationship1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "UserId", c => c.Int(nullable: false));
        }
    }
}
