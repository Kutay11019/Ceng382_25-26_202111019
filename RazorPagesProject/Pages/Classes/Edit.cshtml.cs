using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesProject.Data;
using RazorPagesProject.Models;

namespace RazorPagesProject.Pages.Classes
{
    public class EditModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public EditModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class Class { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Class = await _context.Classes.FindAsync(id); // DÜZENLENDİ

            if (Class == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Classes.Any(e => e.Id == Class.Id)) // DÜZENLENDİ
                    return NotFound();
                else
                    throw;
            }

            return RedirectToPage("./Index");
        }
    }
}