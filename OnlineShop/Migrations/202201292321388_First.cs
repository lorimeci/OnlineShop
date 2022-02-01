namespace OnlineShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CategoryID = c.Int(nullable: false),
                        ProducerId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemArtUrl = c.String(),
                    })
                .PrimaryKey(t => t.ItemId)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Producers", t => t.ProducerId, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.ProducerId);
            
            CreateTable(
                "dbo.Producers",
                c => new
                    {
                        ProducerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ProducerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Items", "ProducerId", "dbo.Producers");
            DropForeignKey("dbo.Items", "CategoryID", "dbo.Categories");
            DropIndex("dbo.Items", new[] { "ProducerId" });
            DropIndex("dbo.Items", new[] { "CategoryID" });
            DropTable("dbo.Producers");
            DropTable("dbo.Items");
            DropTable("dbo.Categories");
        }
    }
}
