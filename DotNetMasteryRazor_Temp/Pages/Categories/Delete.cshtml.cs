using DotNetMasteryRazor_Temp.Data;
using DotNetMasteryRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetMasteryRazor_Temp.Pages.Categories
{
    public class DeleteModel(ApplicationDbContext dbContext) : PageModel
    {
        [BindProperty]
        public Category Category {  get; set; }
        public void OnGet(int? id)
        {
            Category = dbContext.Categories.Find(id);
        }
        public IActionResult OnPost()
        {
            dbContext.Categories.Remove(Category);
            dbContext.SaveChanges();
			TempData["success"] = "Category Deleted Successfully";
			return RedirectToPage("Index");
        }
    }
}
