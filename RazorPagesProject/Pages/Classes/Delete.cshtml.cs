using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Data;
using RazorPagesProject.Models;

namespace RazorPagesProject.Pages.Classes
{
    public class DeleteModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public DeleteModel(SchoolDbContext context)
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
            if (Class == null)
                return NotFound();

            var classToDelete = await _context.Classes.FindAsync(Class.Id); // DÜZENLENDİ

            if (classToDelete != null)
            {
                _context.Classes.Remove(classToDelete); // DÜZENLENDİ
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}