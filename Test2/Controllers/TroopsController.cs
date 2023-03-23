using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Test2.Data;
using Test2.Models;

namespace Test2.Controllers
{
    public class TroopsController : Controller
    {
        private readonly ScoutTroopContext _context;

        public TroopsController(ScoutTroopContext context)
        {
            _context = context;
        }

        // GET: Troops
        public async Task<IActionResult> Index()
        {
              return View(await _context.Troops.AsNoTracking().ToListAsync());
        }

        // GET: Troops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Troops == null)
            {
                return NotFound();
            }

            var troop = await _context.Troops
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (troop == null)
            {
                return NotFound();
            }

            return View(troop);
        }

        // GET: Troops/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Troops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,TroopName,TroopNumber,TroopBudget")] Troop troop)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(troop);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                {
                    ModelState.AddModelError("TroopNumber", "Unable to save changes. Remember, you cannot have duplicate Troop Numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(troop);
        }

        // GET: Troops/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Troops == null)
            {
                return NotFound();
            }

            var troop = await _context.Troops.FindAsync(id);
            if (troop == null)
            {
                return NotFound();
            }
            return View(troop);
        }

        // POST: Troops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            //Get the Troop to update
            var troopToUpdate = await _context.Troops
                .FirstOrDefaultAsync(m => m.ID == id);

            //Check that you got it or exit with a not found error
            if (troopToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Troop>(troopToUpdate, "",
                p => p.TroopName, p => p.TroopNumber, p => p.TroopBudget))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TroopExists(troopToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed"))
                    {
                        ModelState.AddModelError("TroopNumber", "Unable to save changes. Remember, you cannot have duplicate Troop Numbers.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
            return View(troopToUpdate);
        }

        // GET: Troops/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Troops == null)
            {
                return NotFound();
            }

            var troop = await _context.Troops
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (troop == null)
            {
                return NotFound();
            }

            return View(troop);
        }

        // POST: Troops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Troops == null)
            {
                return Problem("Entity set 'ScoutTroopContext.Troops'  is null.");
            }
            var troop = await _context.Troops.FindAsync(id);
            try
            {
                if (troop != null)
                {
                    _context.Troops.Remove(troop);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Troop. Remember, you cannot delete a Troop that has Scouts assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(troop);
        }

        private bool TroopExists(int id)
        {
          return _context.Troops.Any(e => e.ID == id);
        }
    }
}
