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
    public class OrdersController : Controller
    {
        private readonly DbbookshopContext _context;

        public OrdersController(DbbookshopContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var dbbookshopContext = _context.Orders.Include(o => o.Address).Include(o => o.Customer).Include(o => o.PaymentMethod).Include(o => o.Status);
            return View(await dbbookshopContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPurchase(int orderId)
        {
            var bookCount = await _context.OrdersBooks.Where(o => o.OrderId == orderId).CountAsync();
            if (bookCount > 0)
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                    return NotFound();
                else
                {
                    order.CreationDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Status)
                .Include(o => o.OrdersBooks)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Address1");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Email");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,AddressId,StatusId,PaymentMethodId,CreationDate,DispatchDate,ArrivalDate,Id")] Order order, int[] books)
        {
            _context.Add(order);
            await _context.SaveChangesAsync();
                
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Address1", order.AddressId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Email", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", order.PaymentMethodId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", order.StatusId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", order.OrdersBooks.Select(b => b.BookId));
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Address1", order.AddressId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Email", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", order.PaymentMethodId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", order.StatusId);

            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,AddressId,StatusId,PaymentMethodId,CreationDate,DispatchDate,ArrivalDate,Id")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "Id", "Address1", order.AddressId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Email", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "Id", "Name", order.PaymentMethodId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", order.StatusId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Address)
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
