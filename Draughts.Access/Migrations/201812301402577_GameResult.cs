namespace Draughts.Access.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameResult : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameResults",
                c => new
                    {
                        GameResultId = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        Network_NeuralNetworkId = c.Int(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.GameResultId)
                .ForeignKey("dbo.NeuralNetworks", t => t.Network_NeuralNetworkId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Network_NeuralNetworkId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.NeuralNetworks",
                c => new
                    {
                        NeuralNetworkId = c.Int(nullable: false, identity: true),
                        GenePoolSize = c.Int(nullable: false),
                        Generation = c.Int(nullable: false),
                        Network = c.String(),
                    })
                .PrimaryKey(t => t.NeuralNetworkId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameResults", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.GameResults", "Network_NeuralNetworkId", "dbo.NeuralNetworks");
            DropIndex("dbo.GameResults", new[] { "User_Id" });
            DropIndex("dbo.GameResults", new[] { "Network_NeuralNetworkId" });
            DropTable("dbo.NeuralNetworks");
            DropTable("dbo.GameResults");
        }
    }
}
