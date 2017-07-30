using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JiraMonitor.Data;
using JiraMonitor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using JiraMonitor.Data.DbModel;

namespace JiraMonitor.Controllers
{
    public class JqlQueriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JqlQueriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JqlQueries
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var applicationDbContext = _context.JqlQuery;
            return View(await applicationDbContext.Where(j => j.ApplicationUserId == user.Id).ToListAsync());
        }

        // GET: JqlQueries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jqlQuery = await _context.JqlQuery.SingleOrDefaultAsync(m => m.ID == id);
            if (jqlQuery == null)
            {
                return NotFound();
            }

            return View(jqlQuery);
        }

        // GET: JqlQueries/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: JqlQueries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,Query,PollingInterval")] JqlQuery jqlQuery)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {

            }

            jqlQuery.ApplicationUserId = user.Id;

            if (ModelState.IsValid)
            {
                _context.Add(jqlQuery);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jqlQuery.ApplicationUserId);
            return View(jqlQuery);
        }

        // GET: JqlQueries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jqlQuery = await _context.JqlQuery.SingleOrDefaultAsync(m => m.ID == id);
            if (jqlQuery == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jqlQuery.ApplicationUserId);
            return View(jqlQuery);
        }

        // POST: JqlQueries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ApplicationUserId,Query,PollingInterval,Remove")] JqlQuery jqlQuery)
        {
            if (id != jqlQuery.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jqlQuery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JqlQueryExists(jqlQuery.ID))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", jqlQuery.ApplicationUserId);
            return View(jqlQuery);
        }

        // GET: JqlQueries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jqlQuery = await _context.JqlQuery                
                .SingleOrDefaultAsync(m => m.ID == id);
            if (jqlQuery == null)
            {
                return NotFound();
            }

            return View(jqlQuery);
        }

        // POST: JqlQueries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jqlQuery = await _context.JqlQuery.SingleOrDefaultAsync(m => m.ID == id);
            _context.JqlQuery.Remove(jqlQuery);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool JqlQueryExists(int id)
        {
            return _context.JqlQuery.Any(e => e.ID == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
