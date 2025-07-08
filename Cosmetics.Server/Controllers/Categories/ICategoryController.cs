using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Categories.DTO;

namespace CMS.Server.Controllers.Categories
{
    public interface ICategoryController
    {
        Task<ActionResult<IEnumerable<CategoryGetDTO>>> GetAllCategories();
        Task<ActionResult<CategoryGetDTO>> GetCategoryById(int id);
        Task<ActionResult<CategoryGetDTO>> CreateCategory(CategoryCreateDTO categoryCreateDTO);
        Task<ActionResult<CategoryGetDTO>> UpdateCategory(CategoryUpdateDTO categoryUpdateDTO);
        Task<IActionResult> DeleteCategory(int id);
    }
}