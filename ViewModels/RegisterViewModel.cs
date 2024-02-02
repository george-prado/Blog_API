using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
	public class RegisterViewModel
	{
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(40, MinimumLength = 6, ErrorMessage = "O nome deve conter entre 6 e 40 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O E-mail é inválido.")]
        public string Email { get; set; }
    }
}
