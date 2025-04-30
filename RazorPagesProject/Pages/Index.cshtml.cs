using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace RazorPagesProject.Pages
{
    public class IndexModel : PageModel
    {
        public List<ClassInformationTable> DisplayedClasses { get; set; } = new List<ClassInformationTable>();
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        [BindProperty(SupportsGet = true)]
        public string? FilterText { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty]
        public string SelectedColumns { get; set; }

        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        public void OnGet()
        {
            // 🔐 Giriş kontrolü
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieToken = Request.Cookies["token"];
            var cookieUsername = Request.Cookies["username"];
            var cookieSessionId = Request.Cookies["session_id"];

            if (sessionToken == null || cookieToken == null ||
                sessionToken != cookieToken || sessionUsername != cookieUsername ||
                sessionId != cookieSessionId)
            {
                Response.Redirect("/Login");
                return;
            }

            // Sahte veri ekleniyor
            if (!ClassList.Any())
            {
                for (int i = 1; i <= 100; i++)
                {
                    ClassList.Add(new ClassInformationModel
                    {
                        Id = i,
                        ClassName = $"Class {i}",
                        StudentCount = 20 + (i % 10),
                        Description = $"Description {i}"
                    });
                }
            }

            var filteredClasses = string.IsNullOrWhiteSpace(FilterText)
                ? ClassList
                : ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

            TotalPages = (int)Math.Ceiling(filteredClasses.Count / (double)PageSize);
            DisplayedClasses = filteredClasses
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewClass.Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
            ClassList.Add(NewClass);

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassList.Remove(classToRemove);
            }

            return RedirectToPage();
        }

        public void OnGetEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = classToEdit;
            }
        }

        public IActionResult OnPostEdit()
        {
            var classToUpdate = ClassList.FirstOrDefault(c => c.Id == NewClass.Id);
            if (classToUpdate != null)
            {
                classToUpdate.ClassName = NewClass.ClassName;
                classToUpdate.StudentCount = NewClass.StudentCount;
                classToUpdate.Description = NewClass.Description;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostSmartExportJson()
        {
            var selectedColumnsList = string.IsNullOrEmpty(SelectedColumns) ? new List<string>() : SelectedColumns.Split(',').ToList();
            int pageNumber = string.IsNullOrEmpty(Request.Form["pageNumber"]) ? 1 : int.Parse(Request.Form["pageNumber"]);

            if (!string.IsNullOrWhiteSpace(FilterText) && selectedColumnsList.Any())
            {
                var filteredClasses = ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();
                var fpageClasses = filteredClasses.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();
                string json = Utils.Instance.ExportToJson(fpageClasses, selectedColumnsList);
                return File(Encoding.UTF8.GetBytes(json), "application/json", "filtered_columns.json");
            }

            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                var filteredClasses = ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();
                string json = JsonSerializer.Serialize(filteredClasses);
                return File(Encoding.UTF8.GetBytes(json), "application/json", "filtered_rows.json");
            }

            if (selectedColumnsList.Any())
            {
                var cpageClasses = ClassList.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();
                string json = Utils.Instance.ExportToJson(cpageClasses, selectedColumnsList);
                return File(Encoding.UTF8.GetBytes(json), "application/json", "columns_only.json");
            }

            var pageClasses = ClassList.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToList();
            string allJson = Utils.Instance.ExportToJson(pageClasses);
            return File(Encoding.UTF8.GetBytes(allJson), "application/json", "all_data.json");
        }
    }
}