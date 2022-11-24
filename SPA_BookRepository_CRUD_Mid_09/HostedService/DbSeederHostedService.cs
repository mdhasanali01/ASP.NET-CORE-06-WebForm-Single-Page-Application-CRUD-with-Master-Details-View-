using SPA_BookRepository_CRUD_Mid_09.Models;

namespace SPA_BookRepository_CRUD_Mid_09.HostedService
{
    public class DbSeederHostedService : IHostedService
    {
        IServiceProvider serviceProvider;
        public DbSeederHostedService(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {

                var db = scope.ServiceProvider.GetRequiredService<BookDbContext>();

                await SeedDbAsync(db);

            }
        }
        public async Task SeedDbAsync(BookDbContext db)
        {
            await db.Database.EnsureCreatedAsync();
            if (!db.Publishers.Any())
            {
                var c1 = new Publisher { PublisherName = "Microsoft Corporation", Address = "Calefornia, USA", Email = "microsoft@gmail.com" };
                var c2 = new Publisher { PublisherName = "Bertelsmann Publisher", Address = "Brooklyn, NY", Email = "bertelsmann@gmail.com" };
                var c3 = new Publisher { PublisherName = "Dzanc Books", Address = "Adilete, Australia", Email = "dzane@gmail.com" };
                var c4 = new Publisher { PublisherName = "Hanging Loose Press", Address = "Brooklyn, NY", Email = "hanging@gmail.com" };
                await db.Publishers.AddAsync(c1);
                await db.Publishers.AddAsync(c2);
                await db.Publishers.AddAsync(c3);
                await db.Publishers.AddAsync(c4);
                var p1 = new Book { BookName = "SQL Database", PublishDate = DateTime.Now.AddDays(-5 * 30), Price =2000M,Available=false, Picture = "1.jpg",Topic=Topic.DataBase,Publisher=c1 };
                var p2 = new Book { BookName = "HTML5 & CSS3", PublishDate = DateTime.Now.AddDays(-2 * 30), Price = 2500M, Available = true, Picture = "2.jpg", Topic = Topic.Programming, Publisher = c2 };
                var p3 = new Book { BookName = "MVC Core", PublishDate = DateTime.Now.AddDays(-4 * 30), Price = 3000M, Available = false, Picture = "3.png", Topic = Topic.Geograpy, Publisher = c1 };
                var p4 = new Book { BookName = "C# Programing", PublishDate = DateTime.Now.AddDays(-5 * 30), Price = 2700M, Available = true, Picture = "4.jpg", Topic = Topic.Programming, Publisher = c3 };
                await db.Books.AddAsync(p1);
                await db.Books.AddAsync(p2);
                await db.Books.AddAsync(p3);
                await db.Books.AddAsync(p4);
                var o1 = new Author { AuthorName="Jhon Sharp",Address="California, USA" };
                var o2 = new Author { AuthorName = "Jhon Charles", Address = "Sydney, Austrelia" };
                o1.AuthorBooks.Add(new AuthorBook { Author = o1, Book = p1, Quantity = 2 });
                o1.AuthorBooks.Add(new AuthorBook { Author = o1, Book = p2, Quantity = 2 });
                o2.AuthorBooks.Add(new AuthorBook { Author = o2, Book = p3, Quantity = 2 });
                await db.Authors.AddAsync(o1);
                await db.Authors.AddAsync(o2);
                await db.SaveChangesAsync();
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
