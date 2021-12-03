namespace BookReader.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 500),
                        Author = c.String(nullable: false, maxLength: 500),
                        PrimaryLanguageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.PrimaryLanguageId, cascadeDelete: true)
                .Index(t => t.PrimaryLanguageId);
            
            CreateTable(
                "dbo.BookTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 500),
                        Author = c.String(nullable: false, maxLength: 500),
                        Book_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId, cascadeDelete: true)
                .ForeignKey("dbo.Books", t => t.Book_Id)
                .Index(t => t.LanguageId)
                .Index(t => t.Book_Id);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TraditionReferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TraditionId = c.Int(nullable: false),
                        Reference = c.String(nullable: false, maxLength: 3000),
                        Book = c.String(),
                        Volume = c.String(),
                        Page = c.String(),
                        Hadith = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Traditions", t => t.TraditionId, cascadeDelete: true)
                .Index(t => t.TraditionId);
            
            CreateTable(
                "dbo.TraditionReferenceTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TraditionId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        Reference = c.String(nullable: false, maxLength: 3000),
                        TraditionReference_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId, cascadeDelete: true)
                .ForeignKey("dbo.Traditions", t => t.TraditionId, cascadeDelete: true)
                .ForeignKey("dbo.TraditionReferences", t => t.TraditionReference_Id)
                .Index(t => t.TraditionId)
                .Index(t => t.LanguageId)
                .Index(t => t.TraditionReference_Id);
            
            CreateTable(
                "dbo.Traditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BookId = c.Int(nullable: false),
                        Volume = c.Int(),
                        Section = c.String(maxLength: 2000),
                        Chapter = c.String(maxLength: 2000),
                        Page = c.Int(),
                        EndPage = c.Int(),
                        TraditionNo = c.Int(nullable: false),
                        Title = c.String(maxLength: 2000),
                        Text = c.String(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TraditionTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LanguageId = c.Int(nullable: false),
                        TraditionId = c.Int(nullable: false),
                        Section = c.String(maxLength: 2000),
                        Chapter = c.String(maxLength: 2000),
                        Title = c.String(maxLength: 2000),
                        Text = c.String(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Languages", t => t.LanguageId, cascadeDelete: true)
                .ForeignKey("dbo.Traditions", t => t.TraditionId, cascadeDelete: true)
                .Index(t => t.LanguageId)
                .Index(t => t.TraditionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TraditionReferenceTranslations", "TraditionReference_Id", "dbo.TraditionReferences");
            DropForeignKey("dbo.TraditionReferenceTranslations", "TraditionId", "dbo.Traditions");
            DropForeignKey("dbo.TraditionTranslations", "TraditionId", "dbo.Traditions");
            DropForeignKey("dbo.TraditionTranslations", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.TraditionReferences", "TraditionId", "dbo.Traditions");
            DropForeignKey("dbo.TraditionReferenceTranslations", "LanguageId", "dbo.Languages");
            DropForeignKey("dbo.Books", "PrimaryLanguageId", "dbo.Languages");
            DropForeignKey("dbo.BookTranslations", "Book_Id", "dbo.Books");
            DropForeignKey("dbo.BookTranslations", "LanguageId", "dbo.Languages");
            DropIndex("dbo.TraditionTranslations", new[] { "TraditionId" });
            DropIndex("dbo.TraditionTranslations", new[] { "LanguageId" });
            DropIndex("dbo.TraditionReferenceTranslations", new[] { "TraditionReference_Id" });
            DropIndex("dbo.TraditionReferenceTranslations", new[] { "LanguageId" });
            DropIndex("dbo.TraditionReferenceTranslations", new[] { "TraditionId" });
            DropIndex("dbo.TraditionReferences", new[] { "TraditionId" });
            DropIndex("dbo.BookTranslations", new[] { "Book_Id" });
            DropIndex("dbo.BookTranslations", new[] { "LanguageId" });
            DropIndex("dbo.Books", new[] { "PrimaryLanguageId" });
            DropTable("dbo.TraditionTranslations");
            DropTable("dbo.Traditions");
            DropTable("dbo.TraditionReferenceTranslations");
            DropTable("dbo.TraditionReferences");
            DropTable("dbo.Languages");
            DropTable("dbo.BookTranslations");
            DropTable("dbo.Books");
        }
    }
}
