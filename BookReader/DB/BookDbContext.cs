using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Add migration
 * Add-Migration Initial
 * 
 * Update Database
 * Update-Database
 * 
 * Remove migrations
 * Update-Database -TargetMigration:0
 */

namespace BookReader.DB
{
    public class BookDbContext : DbContext
    {
        public BookDbContext() : base("name=BookDb")
        {
        }

        public DbSet<Language> Languages { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookTranslation> BookTranslations { get; set; }
        public DbSet<Tradition> Traditions { get; set; }
        public DbSet<TraditionTranslation> TraditionTranslations { get; set; }
        public DbSet<TraditionReference> TraditionReferences { get; set; }
        public DbSet<TraditionReferenceTranslation> TraditionReferenceTranslations { get; set; }

    }
}
