using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoCandies.Data.Models;

namespace ContosoCandies.Controllers
{
    public class CandiesController : Controller
    {
        private readonly ContosoCandiesContext _context;

        public CandiesController(ContosoCandiesContext context)
        {
            _context = context;    
        }

        // GET: Candies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Candies.ToListAsync());
        }

        // GET: Candies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candies = await _context.Candies
                .SingleOrDefaultAsync(m => m.Id == id);
            if (candies == null)
            {
                return NotFound();
            }

            return View(candies);
        }

        // GET: Candies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Candies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl,Price")] Candies candies)
        {
            if (ModelState.IsValid)
            {
                _context.Add(candies);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(candies);
        }

        // GET: Candies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candies = await _context.Candies.SingleOrDefaultAsync(m => m.Id == id);
            if (candies == null)
            {
                return NotFound();
            }
            return View(candies);
        }

        // POST: Candies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl,Price")] Candies candies)
        {
            if (id != candies.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(candies);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandiesExists(candies.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(candies);
        }

        // GET: Candies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candies = await _context.Candies
                .SingleOrDefaultAsync(m => m.Id == id);
            if (candies == null)
            {
                return NotFound();
            }

            return View(candies);
        }

        // POST: Candies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candies = await _context.Candies.SingleOrDefaultAsync(m => m.Id == id);
            _context.Candies.Remove(candies);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CandiesExists(int id)
        {
            return _context.Candies.Any(e => e.Id == id);
        }
    }
}
