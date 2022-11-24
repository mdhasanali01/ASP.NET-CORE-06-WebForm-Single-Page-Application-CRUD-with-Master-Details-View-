using SPA_BookRepository_CRUD_Mid_09.Models;

namespace SPA_BookRepository_CRUD_Mid_09.ViewModels
{
    public class DataViewModel
    {
        public int SelectedAuthorId { get; set; }
        public IEnumerable<Publisher> Publishers { get; set; }
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<AuthorBook> AuthorBooks { get; set; }
    }
}
