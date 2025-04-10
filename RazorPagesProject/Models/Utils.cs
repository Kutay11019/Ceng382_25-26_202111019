using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class Utils
{
    private static readonly Lazy<Utils> _instance = new Lazy<Utils>(() => new Utils());
    public static Utils Instance => _instance.Value;

    private Utils() { }

    // Tüm property'leri içeren dışa aktarma
    public string ExportToJson<T>(List<T> data)
    {
        return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
    }

    // Seçilen kolonlara göre dışa aktarma
    public string ExportToJson<T>(List<T> data, List<string> selectedProperties)
    {
        if (selectedProperties == null || selectedProperties.Count == 0)
        {
            // Hiçbir kolon seçilmediyse tüm veriyi dışa aktar
            return ExportToJson(data);
        }

        // Dinamik olarak sadece istenen property'leri seç
        var filteredData = data.Select(item =>
        {
            var dict = new Dictionary<string, object?>();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (selectedProperties.Contains(prop.Name))
                {
                    dict[prop.Name] = prop.GetValue(item);
                }
            }

            return dict;
        }).ToList();

        return JsonSerializer.Serialize(filteredData, new JsonSerializerOptions { WriteIndented = true });
    }
}
