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
    // Filtreleme ve sayfalama için eklenen özellikler

    public List<ClassInformationTable> DisplayedClasses { get; set; } = new List<ClassInformationTable>();
    public int TotalPages { get; set; }
    public const int PageSize = 10;

    // Kullanıcıdan alınacak filtreleme kriteri
    [BindProperty(SupportsGet = true)]
    public string? FilterText { get; set; }

    // Sayfa numarası
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    // Seçilen kolonları almak için property
    [BindProperty]
    public string SelectedColumns { get; set; }

    // Geçici veri listesi (burada sahte verilerle test edilecek)
    public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

    public void OnGet()
    {
        // Sahte veri ekleniyor
        if (!ClassList.Any()) // Eğer sınıf listesi boşsa, sahte veriler eklenir
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

        // Filtreleme işlemi
        var filteredClasses = string.IsNullOrWhiteSpace(FilterText)
            ? ClassList
            : ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

        // Sayfalama işlemi
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

     // Sınıf ekleme işlemi
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ID otomatik artırılıyor
            NewClass.Id = ClassList.Count > 0 ? ClassList.Max(c => c.Id) + 1 : 1;
            ClassList.Add(NewClass);

            return RedirectToPage();
        }

        // Sınıf silme işlemi
        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                ClassList.Remove(classToRemove);
            }

            return RedirectToPage(); 
        }

        // Düzenleme için formu doldurma
        public void OnGetEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = classToEdit;
            }
        }

        // Güncellenmiş veriyi kaydetme
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

    // Export all data
    public IActionResult OnPostExportJsonAll()
    {
        // Tüm veriyi JSON olarak dışa aktarır
        var json = Utils.Instance.ExportToJson(ClassList);
        return File(Encoding.UTF8.GetBytes(json), "application/json", "all_data.json");
    }

    public IActionResult OnPostExportJsonFiltered()
    {
        // Filtrelenmiş tüm satırları getir (sayfalama olmadan)
        var filteredRows = string.IsNullOrWhiteSpace(FilterText)
            ? ClassList
            : ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

        // JSON'a dönüştür
        string json = JsonSerializer.Serialize(filteredRows);

        // JSON olarak döndür
        return File(Encoding.UTF8.GetBytes(json), "application/json", "filtered_rows.json");
    }


    // Export selected columns
    public IActionResult OnPostExportJsonColumns()
    {
        // Sayfa numarasını formdan alıyoruz
        int pageNumber = string.IsNullOrEmpty(Request.Form["pageNumber"]) ? 1 : int.Parse(Request.Form["pageNumber"]);

        // Formdan gelen seçili kolonları alıyoruz
        var selectedColumnsList = string.IsNullOrEmpty(SelectedColumns) ? new List<string>() : SelectedColumns.Split(',').ToList();

        var filteredClasses = string.IsNullOrWhiteSpace(FilterText)
            ? ClassList
            : ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).ToList();

        var pageClasses = filteredClasses
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        // JSON'a dönüştürme işlemi
        string json = Utils.Instance.ExportToJson(pageClasses, selectedColumnsList);

        // JSON olarak döndürme
        return File(Encoding.UTF8.GetBytes(json), "application/json", "filtered_data.json");
    }
}
}
