using System.ComponentModel.DataAnnotations;

public class ClassInformationModel
{
    public int Id { get; set; }

    // Required ve non-nullable olacak şekilde düzenle:
    public string ClassName { get; set; } = string.Empty;

    public int StudentCount { get; set; }

    // Required ve non-nullable olacak şekilde düzenle:
    public string Description { get; set; } = string.Empty;
}
