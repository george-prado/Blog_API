using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
	public class EditorCategoryViewModel
	{
        [Required(ErrorMessage = "O campo 'nome' é obrigatório.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Por favor entre um nome válido.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo 'slug' é obrigatório.")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Por favor entre um slug válido.")]
		public string Slug { get; set; }    
    }
}
