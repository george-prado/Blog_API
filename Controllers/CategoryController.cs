using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
	public class CategoryController : ControllerBase
	{
		//GET all
		[HttpGet("v1/categories")]
		public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
		{
			try
			{
				var categories = await context.Categories.ToListAsync();

				return Ok(new ResultViewModel<List<Category>>(categories));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("05X000E11 - Falha interna no servidor"));
			}
		}

		//GET by id
		[HttpGet("v1/categories/id={id:int}")]
		public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
		{
			try
			{
				var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

				if (category == null)
					return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

				return Ok(new ResultViewModel<Category>(category));
			}

			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("05X000E12 - Falha interna no servidor"));
			}
		}

		//POST by body
		[HttpPost("v1/categories")]
		public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

			try
			{
				var category = new Category()
				{
					Id = 0,
					Name = model.Name,
					Slug = model.Slug.ToLower()
				};

				await context.Categories.AddAsync(category);
				await context.SaveChangesAsync();

				return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
			}
			
			catch (DbUpdateException ex)
			{
				return StatusCode(500, new ResultViewModel<Category>("05X00DB10 - Não foi possível incluir no banco de dados"));
			}
			
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("05X000E13 - Falha interna no servidor"));
			}
		}

		//PUT by id
		[HttpPut("v1/categories/id={id:int}")]
		public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

			try
			{

				var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

				if (category == null)
					return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

				category.Name = model.Name;
				category.Slug = model.Slug;

				context.Categories.Update(category);
				await context.SaveChangesAsync();

				return Ok(new ResultViewModel<Category>(category));
			}
			
			catch (DbUpdateException ex)
			{
				return StatusCode(500, new ResultViewModel<Category>("05X00DB11 - Não foi possível alterar no banco de dados"));
			}
			
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("05X000E14 - Falha interna no servidor"));
			}

		}

		//DELETE by id
		[HttpDelete("v1/categories/id={id:int}")]
		public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
		{
			try
			{
				var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

				if (category == null)
					return NotFound(new ResultViewModel<Category>("Conteúdo não encontrado"));

				context.Categories.Remove(category);
				await context.SaveChangesAsync();

				return Ok(new ResultViewModel<Category>(category));
			}
			
			catch (DbUpdateException ex)
			{
				return StatusCode(500, new ResultViewModel<Category>("05X00DB12 - Não foi possível excluir no banco de dados"));
			}
			
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("05X000E15 - Falha interna no servidor"));
			}
		}
	}
}
