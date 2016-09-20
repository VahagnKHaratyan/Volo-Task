using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using BookStore.Entity;
using PagedList;
using System.Web;
using System.IO;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        public dynamic ViewBagProperty
        {
            get;
            set;
        }
        private DatabaseEntities db = new DatabaseEntities();

        // GET: Books
        public async Task<ActionResult> Index(string searchString, string sortOption, int page = 1, string pageType = "image")
        {
            
            int pageSize = 3;
            var books = db.Books.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {

                books = db.Books.Where(p => p.title.ToLower().Contains(searchString));
            }
            switch (sortOption)
            {
                case "title_acs":
                    books = books.OrderBy(p => p.title);
                    break;
                default:
                    books = books.OrderBy(p => p.id);
                    break;

            }
            if (pageType == "image")
            {
                ViewBag.pageType = "image";
            }
            else {
                ViewBag.pageType = "table";
            }


            return Request.IsAjaxRequest()
                ? pageType =="image" ? (ActionResult)PartialView("_ImageList", books.ToPagedList(page, pageSize)) : (ActionResult)PartialView("_BookList", books.ToPagedList(page, pageSize))
                : View(books.ToPagedList(page, pageSize));
                //return View(await db.Books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = await db.Books.FindAsync(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,title,price,page_count,book_description,picture")] Books books, HttpPostedFileBase upload)
        {
         
             if (ModelState.IsValid)
            {
                if (upload != null)
                {
                    var supportedTypes = new[] { "jpg", "jpeg", "png" };
                    var fileExt = System.IO.Path.GetExtension(upload.FileName).Substring(1);

                    if (!supportedTypes.Contains(fileExt))
                    {
                        return RedirectToAction("Index");
                        // ModelState.AddModelError("photo", "Invalid type. Only the following types (jpg, jpeg, png) are supported.");

                    }

                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
                    string path = Path.Combine(Server.MapPath("~/Upload"), filename);//("~/App_Data/Images"), filename);
                    upload.SaveAs(path);

                    books.picture = filename;
                }
                else
                {
                    books.picture = "default.jpg";
                }


                db.Books.Add(books);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(books);
        }

        // GET: Books/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = await db.Books.FindAsync(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,title,price,page_count,book_description,picture")] Books books)
        {
            if (ModelState.IsValid)
            {
                db.Entry(books).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(books);
        }

        // GET: Books/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Books books = await db.Books.FindAsync(id);
            if (books == null)
            {
                return HttpNotFound();
            }
            return View(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Books books = await db.Books.FindAsync(id);
            db.Books.Remove(books);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
