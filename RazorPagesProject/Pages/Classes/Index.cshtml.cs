public class IndexModel : PageModel
{
    private readonly SchoolDbContext _context;

    public IndexModel(SchoolDbContext context)
    {
        _context = context;
    }

    public IList<Class> ClassList { get; set; }

    public async Task OnGetAsync()
    {
        ClassList = await _context.Classes.ToListAsync();
    }
}