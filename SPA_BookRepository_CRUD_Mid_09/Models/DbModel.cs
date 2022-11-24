using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPA_BookRepository_CRUD_Mid_09.Models
{
    public enum Topic { Geograpy=1, Programming, DataBase, Novel}
    public class Publisher
    {
        public int PublisherId { get; set; }
        [Required, StringLength(50), Display(Name = "Publisher Name")]
        public string PublisherName { get; set; }
        [Required, StringLength(50), Display(Name = "Address")]
        public string Address { get; set; }
        [Required, StringLength(50), EmailAddress()]
        public string Email { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
    public class Book
    {
        public int BookId { get; set; }
        [Required, StringLength(50), Display(Name = "Book Name")]
        public string BookName { get; set; }
        [Required, System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date"), Display(Name = "Publish Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublishDate { get; set; }
        [Required, Display(Name = "Price"), DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        public bool Available { get; set; }
        [Required, StringLength(200)]
        public string Picture { get; set; }
        [Required, EnumDataType(typeof(Topic))]
        public Topic Topic { get; set; }
        public int PublisherId { get; set; }
        [ForeignKey("PublisherId")]
        public Publisher Publisher { get; set; }
        public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
    }
    public class Author
    {
        public int AuthorId { get; set; }
        [Required, StringLength(50), Display(Name = "Author Name")]
        public string AuthorName { get; set; }
        [Required, StringLength(50), Display(Name = "Address")]
        public string Address { get; set; }
        public virtual ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
    }
    public class AuthorBook
    {
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public virtual Author? Author { get; set; }
        public virtual Book? Book { get; set; } 
    }
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorBook>().HasKey(o => new { o.AuthorId, o.BookId });
        }
    }
}
