using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Data;
using RazorPagesProject.Models;

namespace RazorPagesProject.Pages.Classes
{
    public class DetailsModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public DetailsModel(SchoolDbContext context)
        {
            _context = context;
        }

        public Class Class { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Class = await _context.ClassInformationTable.FindAsync(id);

            if (Class == null)
                return NotFound();

            return Page();
        }
    }
}