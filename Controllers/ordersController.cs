using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Controllers
{
    public class ordersController : Controller
    {
        private readonly MyStoreContext _context;

        public ordersController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
              return _context.orders != null ? 
                          View(await _context.orders.ToListAsync()) :
                          Problem("Entity set 'MyStoreContext.orders'  is null.");
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        public async Task<IActionResult> Create(int? id)
        {
            var ball = await _context.ball.FindAsync(id);

            return View(ball);
        }


        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ballld, int quantity)
        {
            orders order = new orders();
            order.ballld = ballld;
            order.quantity = quantity;

            order.userid = Convert.ToInt32(HttpContext.Session.GetString("userid")); ;
            order.orderdate = DateTime.Today;

            _context.Add(order);
            await _context.SaveChangesAsync();

            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Asus\\Documents\\mynewdb.mdf;Integrated Security=True;Connect Timeout=30");
            string sql;
            sql = "UPDATE ball SET ballquantity = ballquantity - " + order.quantity + " WHERE id = " + order.ballld;
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();



            return RedirectToAction(nameof(myorders));

        }
        public async Task<IActionResult> myorders()
        {

            int userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            var orItems = await _context.orders.FromSqlRaw("select *  from orders where  userid = '" + userid + "'  ").ToListAsync();
            return View(orItems);

        }

        public async Task<IActionResult> customerOrders(int? id)
        {


            var orItems = await _context.orders.FromSqlRaw("select *  from orders where  userid = '" + id + "'  ").ToListAsync();
            return View(orItems);

        }





        public async Task<IActionResult> customerreport()
        {
            var orItems = await _context.report.FromSqlRaw("select usersaccounts.id as Id, name as customername, sum (quantity * Price)  as total from ball, orders,usersaccounts  where usersaccounts.id = orders.userid  and orders.ballld= ball.Id group by name,usersaccounts.id ").ToListAsync();
            return View(orItems);

        }



        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ballld,userid,quantity,orderdate")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
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
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.orders == null)
            {
                return Problem("Entity set 'MyStoreContext.orders'  is null.");
            }
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
          return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
