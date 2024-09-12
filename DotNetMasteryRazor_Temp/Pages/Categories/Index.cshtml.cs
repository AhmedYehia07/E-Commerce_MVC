using DotNetMasteryRazor_Temp.Data;
using DotNetMasteryRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetMasteryRazor_Temp.Pages.Categories
{
    public class IndexModel(ApplicationDbContext dbContext) : PageModel
    {
        public List<Category> categorylist { get; set; }
        public void OnGet()
        {
            categorylist = dbContext.Categories.ToList();
        }
    }
}
