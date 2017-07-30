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
using JiraMonitor.Data.DbModel;
using JiraMonitor.Model;

namespace JiraMonitor.Controllers
{
    public class UserSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;

        public UserSettingsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();

            var emailNotificationSettings = await _context.EmailNotificationSettings.Where(m => m.ApplicationUserId == user.Id).SingleOrDefaultAsync();
            if (emailNotificationSettings == null)
            {
                //create if not exist
                emailNotificationSettings = new EmailNotificationSettings();
                emailNotificationSettings.ApplicationUserId = user.Id;


                emailNotificationSettings.SmtpPort = "25";
                
               _context.Add(emailNotificationSettings);
                await _context.SaveChangesAsync();
                
            }

            var jiraSettings = await _context.JiraSettings.Where(m => m.ApplicationUserId == user.Id).SingleOrDefaultAsync();

            if (jiraSettings == null)
            {
                //create if not exist
                jiraSettings = new JiraSettings();
                jiraSettings.ApplicationUserId = user.Id;

                jiraSettings.Url = "*.attlassian.net";
                _context.Add(jiraSettings);
                await _context.SaveChangesAsync();

            }

            var userSettingsViewModel = new UserSettingsViewModel();

            userSettingsViewModel.emailNotificationSettings = emailNotificationSettings;
            userSettingsViewModel.jiraSettings = jiraSettings;

            return View(userSettingsViewModel);
        }

        // POST: EmailNotificationSettings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("emailNotificationSettings, jiraSettings")] UserSettingsViewModel userSettingsViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {                    
                    _context.Update(userSettingsViewModel.emailNotificationSettings);
                    _context.Update(userSettingsViewModel.jiraSettings);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailNotificationSettingsExists(userSettingsViewModel.emailNotificationSettings.ID))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", userSettingsViewModel.emailNotificationSettings.ApplicationUserId);
            return View(userSettingsViewModel);
        }

       

        private bool EmailNotificationSettingsExists(int id)
        {
            return _context.EmailNotificationSettings.Any(e => e.ID == id);
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }
}
