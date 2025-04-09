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
    [BindProperty(SupportsGet = true)]
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

    // Export all data
    public IActionResult OnPostExportJsonAll()
    {
        // Tüm veriyi JSON olarak dışa aktarır
        var json = Utils.Instance.ExportToJson(ClassList);
        return File(Encoding.UTF8.GetBytes(json), "application/json", "all_data.json");
    }

    // Export filtered data based on selected columns
    public IActionResult OnPostExportJsonFiltered()
    {
        // Formdan gelen seçili kolonları alıyoruz
        var selectedColumnsList = string.IsNullOrEmpty(SelectedColumns) ? new List<string>() : SelectedColumns.Split(',').ToList();

        // Filtrelenmiş listeyi oluşturuyoruz
        var filteredList = string.IsNullOrWhiteSpace(FilterText) 
            ? ClassList.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList() 
            : ClassList.Where(c => c.ClassName.Contains(FilterText, StringComparison.OrdinalIgnoreCase)).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

        // JSON'a dönüştürme işlemi
        string json = Utils.Instance.ExportToJson(filteredList, selectedColumnsList);

        // JSON olarak döndürme
        return File(Encoding.UTF8.GetBytes(json), "application/json", "filtered_data.json");
    }
}
}
