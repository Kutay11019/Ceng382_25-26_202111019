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
            Class = await _context.ClassInformationTable.FindAsync(id);

            if (Class == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Class == null)
                return NotFound();

            var classToDelete = await _context.ClassInformationTable.FindAsync(Class.Id);

            if (classToDelete != null)
            {
                _context.ClassInformationTable.Remove(classToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}