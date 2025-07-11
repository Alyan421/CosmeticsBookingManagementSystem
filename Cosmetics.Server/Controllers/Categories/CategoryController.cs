using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cosmetics.Server.Controllers.Categories.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Cosmetics.Server.Managers.Categories;

namespace Cosmetics.Server.Controllers.Categories
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase, ICategoryController
    {
        private readonly ICategoryManager _categoryManager;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryManager categoryManager, IMapper mapper)
        {
            _categoryManager = categoryManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryGetDTO>>> GetAllCategories()
        {
            var categories = await _categoryManager.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryGetDTO>>(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryGetDTO>> GetCategoryById(int id)
        {
            var category = await _categoryManager.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found");
            }

            return _mapper.Map<CategoryGetDTO>(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryGetDTO>> CreateCategory(CategoryCreateDTO categoryCreateDTO)
        {
            var createdCategory = await _categoryManager.CreateCategoryAsync(categoryCreateDTO);
            return _mapper.Map<CategoryGetDTO>(createdCategory);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryGetDTO>> UpdateCategory(CategoryUpdateDTO categoryUpdateDTO)
        {
            var updatedCategory = await _categoryManager.UpdateCategoryAsync(categoryUpdateDTO);

            if (updatedCategory == null)
            {
                return NotFound($"Category with ID {categoryUpdateDTO.Id} not found");
            }

            return _mapper.Map<CategoryGetDTO>(updatedCategory);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryManager.DeleteCategoryAsync(id);

                if (!result)
                {
                    return NotFound($"Category with ID {id} not found");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}