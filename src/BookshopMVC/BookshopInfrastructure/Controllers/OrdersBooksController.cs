using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookshopDomain.Model;
using BookshopInfrastructure;

namespace BookshopInfrastructure.Controllers
{
    public class OrdersBooksController : Controller
    {
        private readonly DbbookshopContext _context;

        public OrdersBooksController(DbbookshopContext context)
        {
            _context = context;
        }

        // GET: OrdersBooks
        public async Task<IActionResult> Index()
        {
            var dbbookshopContext = _context.OrdersBooks.Include(o => o.Book).Include(o => o.Order);
            return View(await dbbookshopContext.ToListAsync());
        }

        // GET: OrdersBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersBook = await _context.OrdersBooks
                .Include(o => o.Book)
                .Include(o => o.Order).ThenInclude(c => c.Customer)
                .Include(o => o.Order).ThenInclude(a => a.Address)
                .Include(o => o.Order).ThenInclude(s => s.Status)
                .Include(o => o.Order).ThenInclude(p => p.PaymentMethod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordersBook == null)
            {
                return NotFound();
            }

            return View(ordersBook);
        }

        // GET: OrdersBooks/Create
        public IActionResult Create(int Id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == Id);
            
            ViewData["Book"] = book;
            ViewData["BookId"] = new SelectList(new List<Book> { book }, "Id", "Title");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            ViewData["AdressId"] = new SelectList(_context.Addresses, "Id", "Address1");
            ViewData["CustomerId"] = new SelectList(_context.Addresses, "Id", "FullName");
            return View();
        }

        // POST: OrdersBooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,BookId,BookQuantity,Id")] OrdersBook ordersBook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordersBook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", ordersBook.BookId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", ordersBook.OrderId);
            return View(ordersBook);
        }

        // GET: OrdersBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersBook = await _context.OrdersBooks.FindAsync(id);
            if (ordersBook == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Description", ordersBook.BookId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", ordersBook.OrderId);
            return View(ordersBook);
        }

        // POST: OrdersBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,BookId,BookQuantity,Id")] OrdersBook ordersBook)
        {
            if (id != ordersBook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordersBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersBookExists(ordersBook.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Description", ordersBook.BookId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", ordersBook.OrderId);
            return View(ordersBook);
        }

        // GET: OrdersBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordersBook = await _context.OrdersBooks
                .Include(o => o.Book)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordersBook == null)
            {
                return NotFound();
            }

            return View(ordersBook);
        }

        // POST: OrdersBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordersBook = await _context.OrdersBooks.FindAsync(id);
            if (ordersBook != null)
            {
                _context.OrdersBooks.Remove(ordersBook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersBookExists(int id)
        {
            return _context.OrdersBooks.Any(e => e.Id == id);
        }
    }
}
