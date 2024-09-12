using DotNetMasteryRazor_Temp.Data;
using DotNetMasteryRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DotNetMasteryRazor_Temp.Pages.Categories
{
    public class EditModel(ApplicationDbContext dbContext) : PageModel
    {
        [BindProperty]
        public Category Category { get; set; }
        public void OnGet(int id)
        {
            Category = dbContext.Categories.FirstOrDefault(x => x.Id == id);
        }
        public IActionResult OnPost()
        {
            if (Category.Id == null || Category.Id ==0)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                dbContext.Categories.Update(Category);
                dbContext.SaveChanges();
				TempData["success"] = "Category Updated Successfully";
				return RedirectToPage("Index");
            }
            return RedirectToPage();
        }
    }
}
