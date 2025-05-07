using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Models; // model sınıfının namespace’i
using RazorPagesProject.Data;

namespace RazorPagesProject.Pages.Classes
{
    public class CreateModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public CreateModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class Class { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.ClassInformationTable.Add(Class);
            _context.SaveChanges();
            return RedirectToPage("./Index");
        }
    }
}