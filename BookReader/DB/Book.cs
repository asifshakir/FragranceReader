using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReader.DB
{
    public class Language
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Author { get; set; }
        public int PrimaryLanguageId { get; set; }
        public virtual Language PrimaryLanguage { get; set; }
        public virtual ICollection<BookTranslation> BookTranslations { get; set; }
    }

    public class BookTranslation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Author { get; set; }
    }

    public class Tradition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int BookId { get; set; }
        public int? Volume { get; set; }
        [MaxLength(2000)]
        public string Section { get; set; }
        [MaxLength(2000)]
        public string Chapter { get; set; }
        public int? Page { get; set; }
        public int? EndPage { get; set; }
        public int TraditionNo { get; set; }
        [MaxLength(2000)]
        public string Title { get; set; }
        [Required]
        [MaxLength(64000)]
        public string Text { get; set; }
        [MaxLength(64000)]
        public string Notes { get; set; }
        public virtual ICollection<TraditionReference> TraditionReferences { get; set; }
        public virtual ICollection<TraditionTranslation> TraditionTranslations { get; set; }
    }

    public class TraditionTranslation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
        public int TraditionId { get; set; }
        public virtual Tradition Tradition { get; set; }
        [MaxLength(2000)]
        public string Section { get; set; }
        [MaxLength(2000)]
        public string Chapter { get; set; }
        [MaxLength(2000)]
        public string Title { get; set; }
        [Required]
        [MaxLength(64000)]
        public string Text { get; set; }
        [MaxLength(64000)]
        public string Notes { get; set; }
    }

    public class TraditionReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TraditionId { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Reference { get; set; }
        public string Book { get; set; }
        public string Volume { get; set; }
        public string Page { get; set; }
        public string Hadith { get; set; }
        public virtual ICollection<TraditionReferenceTranslation> TraditionReferenceTranslations { get; set; }
    }

    public class TraditionReferenceTranslation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TraditionId { get; set; }
        public virtual Tradition Tradition { get; set; }
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }
        [Required]
        [MaxLength(3000)]
        public string Reference { get; set; }
    }

}
