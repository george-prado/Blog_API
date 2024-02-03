using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
	[ApiController]
	public class AccountController : ControllerBase
	{

		[HttpPost("v1/accounts/")]
		public async Task<IActionResult> Register(
			[FromBody]RegisterViewModel model,
			[FromServices]EmailService emailService,
			[FromServices]BlogDataContext context)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

			var user = new User()
			{
				Name = model.Name,
				Email = model.Email,
				Slug = model.Email.Replace('@', '-').Replace('.','-')
			};

			var password = PasswordGenerator.Generate(length: 25);
			user.PasswordHash = PasswordHasher.Hash(password);

			try
			{
				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				emailService.Send(toName: user.Name, 
					toEmail: user.Email, 
					subject:"Bem-vindo ao API Blog", 
					body: $"Sua senha é <strong>{password}</strong>");

				return Ok(new ResultViewModel<dynamic>(new
				{
					user = user.Email,
					password //just for study purpose, remove if implementing
				}));
			}
			catch(DbUpdateException)
			{
				return StatusCode(400, new ResultViewModel<Category>("02X000E01 - E-mail já cadastrado."));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("02X000E11 - Falha interna do servidor."));
			}
		}

		[HttpPost("v1/accounts/login")]
		public async Task<IActionResult> Login(
			[FromBody]LoginViewModel model, 
			[FromServices]BlogDataContext context, 
			[FromServices]TokenService tokenService)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

			var user = await context
				.Users
				.AsNoTracking()
				.Include(x => x.Roles)
				.FirstOrDefaultAsync(x => x.Email == model.Email);

			if (user == null)
				return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));


			if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
				return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

			try
			{
				var token = tokenService.GenerateToken(user);
				return Ok(new ResultViewModel<string>(token, null));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<string>("\"02X000E12 - Falha interna do servidor."));
			}
		}

		[Authorize]
		[HttpPost("v1/accounts/upload-image")]
		public async Task<IActionResult> UploadImage(
			[FromBody] UploadImageViewModel model, 
			[FromServices] BlogDataContext context)
		{
			var fileName = $"{Guid.NewGuid().ToString()}.jpg";
			var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");

			var bytes = Convert.FromBase64String(data);

			try
			{
				await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<string>("02X000E31 - Falha interna do servidor."));
			}

			var user = await context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

			if (user == null)
				return BadRequest(new ResultViewModel<User>("Usuário não encontrado"));

			user.Image = $"https://localhost:0000/images/{fileName}";
			try
			{
				context.Users.Update(user);
				await context.SaveChangesAsync();
			}
			catch (Exception)
			{
				return StatusCode(500, new ResultViewModel<string>("02X000E32 - Falha interna do servidor."));
			}

			return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!", null));
		}
	}
}
