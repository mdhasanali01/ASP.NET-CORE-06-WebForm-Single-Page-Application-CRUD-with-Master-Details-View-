using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPA_BookRepository_CRUD_Mid_09.Models;
using SPA_BookRepository_CRUD_Mid_09.ViewModels;

namespace SPA_BookRepository_CRUD_Mid_09.Controllers
{
    public class SPAController : Controller
    {

        BookDbContext db;
        IWebHostEnvironment env;
        public SPAController(BookDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }
        public async Task<IActionResult> Index()
        {
            var id = 0;
            if (db.Authors.Any())
            {
                id = db.Authors.ToList()[0].AuthorId;
            }

            DataViewModel data = new DataViewModel();
            data.SelectedAuthorId = id;
            data.Publishers = await db.Publishers.ToListAsync();
            data.Books = await db.Books.ToListAsync();
            data.Authors = await db.Authors.ToListAsync();
            data.AuthorBooks = await db.AuthorBooks.Where(oi => oi.AuthorId == id).ToListAsync();


            return View(data);
        }
        public async Task<IActionResult> GetPublishers()
        {
            return PartialView("_PublisherTable", await db.Publishers.ToListAsync());
        }
        public IActionResult CreatePublisher()
        {
            return PartialView("_CreatePublisher");
        }
        [HttpPost]
        public async Task<IActionResult> CreatePublisher(Publisher c)
        {
            if (ModelState.IsValid)
            {
                await db.Publishers.AddAsync(c);
                await db.SaveChangesAsync();
                return Json(c);
            }
            return BadRequest("Unexpected error");
        }
        public async Task<IActionResult> EditPublisher(int id)
        {
            var data = await db.Publishers.FirstOrDefaultAsync(c => c.PublisherId == id);
            return PartialView("_EditPublisher", data);
        }
        [HttpPost]
        public async Task<IActionResult> EditPublisher(Publisher c)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(c);
            }
            return BadRequest("Unexpected error");
        }
        [HttpPost]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            if (!await db.Books.AnyAsync(o => o.PublisherId == id))
            {
                var o = new Publisher { PublisherId = id };
                db.Entry(o).State = EntityState.Deleted;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true, message = "Data deleted" });
            }
            return Json(new { success = false, message = "Cannot delete, item has related child." });
        }
        public async Task <IActionResult> CreateBook()
        {
            ViewData["Publishers"] = await db.Publishers.ToListAsync();
            return PartialView("_CreateBook");
        }
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookInputModel p)
        {
            if (ModelState.IsValid)
            {
                var book = new Book { BookName = p.BookName,PublishDate=p.PublishDate, Price = p.Price,Available=p.Available,Topic=p.Topic,PublisherId=p.PublisherId };
                string fileName = Guid.NewGuid() + Path.GetExtension(p.Picture.FileName);
                string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
                var fs = new FileStream(savePath, FileMode.Create);
                p.Picture.CopyTo(fs);
                fs.Close();
                book.Picture = fileName;
                await db.Books.AddAsync(book);
                
                await db.SaveChangesAsync();
                var x = GetBook(book.BookId);
                return Json(x);


            }
            return BadRequest("Falied to insert product");
        }
        public async Task<IActionResult> EditBook(int id)
        {
            
            ViewData["Publishers"] = await db.Publishers.ToListAsync();
            var data = await db.Books.FirstAsync(x => x.BookId == id);
            ViewData["CurrentPic"] = data.Picture;
            return PartialView("_EditBook", new BookEditModel { BookId = data.BookId, BookName = data.BookName,PublishDate=data.PublishDate, Price = data.Price, Available = data.Available,Topic=data.Topic,PublisherId=data.PublisherId });
        }
        [HttpPost]
        public async Task< ActionResult> EditBook(BookEditModel b)
        {
            
            if (ModelState.IsValid)
            {
                var book = await db.Books.FirstAsync(x => x.BookId == b.BookId);
                book.BookName = b.BookName;
                book.PublishDate = b.PublishDate;
                book.Price = b.Price;
                book.Available = b.Available;
                book.Topic = b.Topic;
                book.PublisherId = b.PublisherId;
                if (b.Picture != null)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(b.Picture.FileName);
                    string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
                    var fs = new FileStream(savePath, FileMode.Create);
                    b.Picture.CopyTo(fs);
                    fs.Close();
                    book.Picture = fileName;
                }

                await db.SaveChangesAsync();
                var x = GetBook(book.BookId);
                return Json(book);
            }
            return BadRequest();
        }
       private Book? GetBook(int id)
        {
            return db.Books.Include(x=>x.Publisher).FirstOrDefault(x=>x.BookId == id);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (!await db.AuthorBooks.AnyAsync(o => o.BookId == id))
            {
                var o = new Book { BookId = id };
                db.Entry(o).State = EntityState.Deleted;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true, message = "Data deleted" });
            }
            return Json(new { success = false, message = "Cannot delete, item has related child." });
        }
        public async Task<IActionResult> CreateAuthor()
        {
            ViewData["Books"] = await db.Books.ToListAsync();
            return PartialView("_CreateAuthor");
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuthor(Author a, int[] BookId)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < BookId.Length; i++)
                {
                    a.AuthorBooks.Add(new AuthorBook { BookId = BookId[i]});
                }
                await db.Authors.AddAsync(a);

                await db.SaveChangesAsync();


                var ord = await GetAuthor(a.AuthorId);
                return Json(ord);
            }
            return BadRequest();
        }
        public async Task<IActionResult> EditAuthor(int id)
        {
            ViewData["Books"] = await db.Books.ToListAsync();
            
            var data = await db.Authors
                .Include(x => x.AuthorBooks).ThenInclude(x => x.Book)
                .FirstOrDefaultAsync(x => x.AuthorId == id);
            return PartialView("_EditAuthor", data);

        }
        [HttpPost]
        public async Task<IActionResult> EditAuthor(Author o)
        {
            if (ModelState.IsValid)
            {
                var existing = await db.Authors.FirstAsync(x => x.AuthorId == o.AuthorId);
                existing.AuthorName = o.AuthorName;
                existing.Address = o.Address;
                

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return Json(existing);
            }

            return BadRequest();
        }
        private async Task<Author?> GetAuthor(int id)
        {
            var o = await db.Authors.FirstOrDefaultAsync(x => x.AuthorId == id);
            return o;
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var o = new Author { AuthorId = id };
            db.Entry(o).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Data deleted" });
        }
        public async Task<IActionResult> CreateItem()
        {
            ViewData["Books"] = await db.Books.ToListAsync();
            return PartialView("_CreateItem");
        }
        public async Task<IActionResult> CreateAuthorBook(int id)
        {
            ViewData["AuthorId"] = id;
            ViewData["Books"] = await db.Books.ToListAsync();
            return PartialView("_CreateAuthorBook");
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuthorBook(AuthorBook oi)
        {
            if (ModelState.IsValid)
            {
                await db.AuthorBooks.AddAsync(oi);
                await db.SaveChangesAsync();
                var o = await GetAuthorBook(oi.AuthorId, oi.BookId);
                return Json(o);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAuthorBook([FromQuery] int aid, [FromQuery] int bid)
        {

            var o = new AuthorBook { BookId = bid, AuthorId = aid };
            db.Entry(o).State = EntityState.Deleted;

            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Data deleted" });

        }
        private async Task<AuthorBook> GetAuthorBook(int oid, int pid)
        {
            var oi = await db.AuthorBooks
                .Include(o => o.Author)
                .Include(o => o.Book)
                .FirstAsync(x => x.AuthorId == oid && x.BookId == pid);
            return oi;
        }
        public async Task<IActionResult> GetSelectedAuthorBooks(int id)
        {

            var AuthorBooks = await db.AuthorBooks.Include(x => x.Book).Where(oi => oi.AuthorId == id).ToListAsync();
            return PartialView("_AuthorBookTable", AuthorBooks);
        }
    }
}
