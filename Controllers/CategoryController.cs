using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
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

				return Ok(categories);
			}
			catch (Exception ex)
			{

				return StatusCode(500, "05X000E11 - Falha interna no servidor");
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
					return NotFound();

				return Ok(category);
			}

			catch (Exception ex)
			{
				return StatusCode(500, "05X000E12 - Falha interna no servidor");
			}
		}

		//POST by body
		[HttpPost("v1/categories")]
		public async Task<IActionResult> PostAsync([FromBody] CreateCategoryViewModel model, [FromServices] BlogDataContext context)
		{
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

				return Created($"v1/categories/{category.Id}", category);
			}
			
			catch (DbUpdateException ex)
			{
				return StatusCode(500, "05X00DB10 - Não foi possível incluir no banco de dados");
			}
			
			catch (Exception ex)
			{
				return StatusCode(500, "05X000E13 - Falha interna no servidor");
			}
		}

		//PUT by id
		[HttpPut("v1/categories/id={id:int}")]
		public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] Category model, [FromServices] BlogDataContext context)
		{
			try
			{
				var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

				if (category == null)
					return NotFound();

				category.Name = model.Name;
				category.Slug = model.Slug;

				context.Categories.Update(category);
				await context.SaveChangesAsync();

				return Ok(category);
			}
			
			catch (DbUpdateException ex)
			{
				return StatusCode(500, "05X00DB11 - Não foi possível alterar no banco de dados");
			}
			
			catch (Exception ex)
			{
				return StatusCode(500, "05X000E14 - Falha interna no servidor");
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
					return NotFound();

				context.Categories.Remove(category);
				await context.SaveChangesAsync();

				return Ok(category);
			}
			
			catch (DbUpdateException ex)
			{
				return StatusCode(500, "05X00DB12 - Não foi possível excluir no banco de dados");
			}
			
			catch (Exception ex)
			{
				return StatusCode(500, "05X000E15 - Falha interna no servidor");
			}
		}
	}
}
