using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdentityServer4Authentication.Data;
using IdentityServer4Authentication.Models;

namespace IdentityServer4Authentication.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUsersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApplicationUser.ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        [Route("/ApplicationUsers/Details/{user_name}")]
        public async Task<IActionResult> Details(string user_name)
        {

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.NormalizedUserName == user_name.ToUpper());
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AccessFailedCount,ConcurrencyStamp,Email,EmailConfirmed,LockoutEnabled,LockoutEnd,NormalizedEmail,NormalizedUserName,OfficeNumber,Group,PasswordHash,PhoneNumber,PhoneNumberConfirmed,SecurityStamp,TwoFactorEnabled,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        [Route("/ApplicationUsers/Edit/{user_name}")]
        public async Task<IActionResult> Edit(string user_name)
        {

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.NormalizedUserName == user_name.ToUpper());
            if (applicationUser == null)
            {
                return NotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/ApplicationUsers/Edit/{user_name}")]
        public async Task<IActionResult> Edit(string user_name, [Bind("Id,AccessFailedCount,ConcurrencyStamp,Email,EmailConfirmed,LockoutEnabled,LockoutEnd,NormalizedEmail,NormalizedUserName,OfficeNumber,Group,PasswordHash,PhoneNumber,PhoneNumberConfirmed,SecurityStamp,TwoFactorEnabled,UserName")] ApplicationUser applicationUser)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.NormalizedUserName))
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
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        [Route("/ApplicationUsers/Delete/{user_name}")]
        public async Task<IActionResult> Delete(string user_name)
        {

            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.NormalizedUserName == user_name.ToUpper());
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("/ApplicationUsers/Delete/{user_name}")]
        public async Task<IActionResult> DeleteConfirmed(string user_name)
        {
            var applicationUser = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.NormalizedUserName == user_name.ToUpper());
            _context.ApplicationUser.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ApplicationUserExists(string user_name)
        {
            return _context.ApplicationUser.Any(e => e.NormalizedUserName == user_name.ToUpper());
        }
    }
}
