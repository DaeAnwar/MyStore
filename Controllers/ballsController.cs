using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyStore.Data;
using MyStore.Models;

namespace MyStore.Controllers
{
    public class ballsController : Controller
    {
        private readonly MyStoreContext _context;

        public ballsController(MyStoreContext context)
        {
            _context = context;
        }

        // GET: balls
        public async Task<IActionResult> Index()
        {
              return _context.ball != null ? 
                          View(await _context.ball.ToListAsync()) :
                          Problem("Entity set 'MyStoreContext.ball'  is null.");
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Search)
        {
           
            ViewData["GetBallsfiltred"] = Search;
            var empquery = from x in _context.ball select x;
            if (!String.IsNullOrEmpty(Search))
            {
                empquery = empquery.Where(x => x.title.Contains(Search));

            }
            return View(await empquery.AsNoTracking().ToListAsync());
        }


        public async Task<IActionResult> catalogue()
        {
            return View(await _context.ball.ToListAsync());
        }


        // GET: balls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ball == null)
            {
                return NotFound();
            }

            var ball = await _context.ball
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ball == null)
            {
                return NotFound();
            }

            return View(ball);
        }

        // GET: balls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: balls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,title,info,ballquantity,price,cataid")] ball ball)
        {

            if (file != null)
            {
                string filename = file.FileName;
                //  string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                ball.imgfile = filename;
            }

            _context.Add(ball);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: balls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ball == null)
            {
                return NotFound();
            }

            var ball = await _context.ball.FindAsync(id);
            if (ball == null)
            {
                return NotFound();
            }
            return View(ball);
        }

        // POST: balls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile file, [Bind("Id,title,info,ballquantity,price,cataid,imgfile")] ball ball)
        {
            if (id != ball.Id)
            {
                return NotFound();
            }


            try
            {
                if (file != null)
                {
                    string filename = file.FileName;
                    //  string  ext = Path.GetExtension(file.FileName);
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    ball.imgfile = filename;
                }

                _context.Update(ball);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ballExists(ball.Id))
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
       


        // GET: balls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ball == null)
            {
                return NotFound();
            }

            var ball = await _context.ball
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ball == null)
            {
                return NotFound();
            }

            return View(ball);
        }

        // POST: balls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ball == null)
            {
                return Problem("Entity set 'MyStoreContext.ball'  is null.");
            }
            var ball = await _context.ball.FindAsync(id);
            if (ball != null)
            {
                _context.ball.Remove(ball);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ballExists(int id)
        {
          return (_context.ball?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
