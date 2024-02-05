using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
	[ApiController]
	public class PostController : ControllerBase
	{
		[HttpGet("v1/posts")]
		public async Task<IActionResult> GetAsync(
			[FromServices]BlogDataContext context,
			[FromQuery]int page = 0,
			[FromQuery]int pageSize = 25)
		{
			try
			{
				var count = await context.Posts.AsNoTracking().CountAsync();

				var posts = await context.Posts
					.AsNoTracking()
					.Include(x => x.Category)
					.Include(x => x.Author)
					.Select(x => new ListPostsViewModel()
					{
						Id = x.Id,
						Title = x.Title,
						Slug = x.Slug,
						LastUpdateTime = x.LastUpdateDate,
						Category = x.Category.Name,
						Author = $"{x.Author.Name} ({x.Author.Email})"
					})
					.Skip(page * pageSize)
					.Take(pageSize)
					.OrderByDescending(x => x.LastUpdateTime)
					.ToListAsync();

				return Ok(new ResultViewModel<dynamic>(new
				{
					total = count,
					page,
					pageSize,
					posts
				}));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<List<Post>>("03X000E11 - Falha interna no servidor"));
			}
		}


		[HttpGet("v1/posts/id={id:int}")]
		public async Task<IActionResult> GetByIdAsync(
			[FromRoute]int id,
			[FromServices]BlogDataContext context,
			[FromQuery] int page = 0,
			[FromQuery] int pageSize = 25)
				{
			try
			{
				var post = await context.Posts
					.AsNoTracking()
					.Include(x => x.Author)
					.ThenInclude(x => x.Roles)
					.Include(x => x.Category)
					.FirstOrDefaultAsync(x => x.Id == id);

				if (post == null)
					return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));


				return Ok(new ResultViewModel<Post>(post));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<Post>("03X000E12 - Falha interna no servidor"));
			}
		}


		[HttpGet("v1/posts/category/{category}")]
		public async Task<IActionResult> GetAsync(
			[FromRoute]string category,
			[FromServices]BlogDataContext context,
			[FromQuery] int page = 0,
			[FromQuery] int pageSize = 25)
		{
			try
			{
				var count = await context.Posts.AsNoTracking().CountAsync();

				var posts = await context.Posts
					.AsNoTracking()
					.Include(x => x.Author)
					.Include(x => x.Category)
					.Where(x => x.Category.Slug == category)
					.Select(x => new ListPostsViewModel()
					{
						Id = x.Id,
						Title = x.Title,
						Slug = x.Slug,
						LastUpdateTime = x.LastUpdateDate,
						Category = x.Category.Name,
						Author = $"{x.Author.Name} ({x.Author.Email})"
					})
					.Skip(page * pageSize)
					.Take(pageSize)
					.OrderByDescending(x => x.LastUpdateTime)
					.ToListAsync();

				return Ok(new ResultViewModel<dynamic>(new
				{
					total = count,
					page,
					pageSize,
					posts
				}));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<List<Post>>("03X000E13 - Falha interna no servidor"));
			}
		}
	}
}
