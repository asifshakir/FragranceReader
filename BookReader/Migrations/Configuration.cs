namespace BookReader.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookReader.DB.BookDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BookReader.DB.BookDbContext context)
        {
            context.Books.AddOrUpdate(x => x.Id,
                new DB.Book
                {
                    Id = 1,
                    PrimaryLanguageId = 2,
                    Name = "Fragrance of Mastership",
                    Author = "Dr. Shabeeb Rizvi"
                }
            );

            context.BookTranslations.AddOrUpdate(x => x.Id,
                new DB.BookTranslation
                {
                    Id = 1,
                    LanguageId = 3,
                    Name = "شمیم ولایت علی ابن ابی طالب علیہما السلام",
                    Author = "ڈاکٹر شبیب رضوی"
                }
            );

            context.Languages.AddOrUpdate( x => x.Id,
                new DB.Language
                {
                    Id = 1,
                    Name = "Arabic"
                },
                new DB.Language
                {
                    Id = 2,
                    Name = "English"
                },
                new DB.Language
                {
                    Id = 3,
                    Name = "Urdu"
                }
            );
        }
    }
}
