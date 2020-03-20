namespace CityStations.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastCommit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contents",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContentType = c.Int(nullable: false),
                        TimeOut = c.Int(nullable: false),
                        InnerContent = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InformationTables",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        WidthWithModule = c.Int(nullable: false),
                        HeightWithModule = c.Int(nullable: false),
                        RowCount = c.Int(nullable: false),
                        ModuleType_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModuleTypes", t => t.ModuleType_Id)
                .Index(t => t.ModuleType_Id);
            
            CreateTable(
                "dbo.ModuleTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        WidthPx = c.Int(nullable: false),
                        HeightPx = c.Int(nullable: false),
                        CssClass = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        EventType = c.Int(nullable: false),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                        Initiator = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StationModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        IdForRnis = c.String(),
                        Name = c.String(),
                        NameForRnis = c.String(),
                        Description = c.String(),
                        Lat = c.Double(nullable: false),
                        Lng = c.Double(nullable: false),
                        Type = c.Boolean(nullable: false),
                        Active = c.Boolean(nullable: false),
                        AccessCode = c.String(),
                        InformationTable_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InformationTables", t => t.InformationTable_Id)
                .Index(t => t.InformationTable_Id);
            
            CreateTable(
                "dbo.InformationTables_Contents",
                c => new
                    {
                        InformationTable_Id = c.String(nullable: false, maxLength: 128),
                        Content_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.InformationTable_Id, t.Content_Id })
                .ForeignKey("dbo.InformationTables", t => t.InformationTable_Id, cascadeDelete: true)
                .ForeignKey("dbo.Contents", t => t.Content_Id, cascadeDelete: true)
                .Index(t => t.InformationTable_Id)
                .Index(t => t.Content_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StationModels", "InformationTable_Id", "dbo.InformationTables");
            DropForeignKey("dbo.InformationTables", "ModuleType_Id", "dbo.ModuleTypes");
            DropForeignKey("dbo.InformationTables_Contents", "Content_Id", "dbo.Contents");
            DropForeignKey("dbo.InformationTables_Contents", "InformationTable_Id", "dbo.InformationTables");
            DropIndex("dbo.InformationTables_Contents", new[] { "Content_Id" });
            DropIndex("dbo.InformationTables_Contents", new[] { "InformationTable_Id" });
            DropIndex("dbo.StationModels", new[] { "InformationTable_Id" });
            DropIndex("dbo.InformationTables", new[] { "ModuleType_Id" });
            DropTable("dbo.InformationTables_Contents");
            DropTable("dbo.StationModels");
            DropTable("dbo.Events");
            DropTable("dbo.ModuleTypes");
            DropTable("dbo.InformationTables");
            DropTable("dbo.Contents");
        }
    }
}
