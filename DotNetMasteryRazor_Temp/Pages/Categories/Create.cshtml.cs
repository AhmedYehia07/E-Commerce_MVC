using DotNetMasteryRazor_Temp.Data;
using DotNetMasteryRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetMasteryRazor_Temp.Pages.Categories
{
    public class CreateModel(ApplicationDbContext dbContext) : PageModel
    {
        [BindProperty]
        public Category Category { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if(Category.Name == Category.DisplayOrder.ToString())
            {
				ModelState.AddModelError("Name", "Display Order can't match the Name");
			}
            if(ModelState.IsValid)
            {
				dbContext.Categories.Add(Category);
                dbContext.SaveChanges();
                TempData["success"] = "Category Created Successfully";
                return RedirectToPage("Index");
			}
            return Page();
        }
    }
}
