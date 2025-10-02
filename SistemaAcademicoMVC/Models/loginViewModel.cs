using System.ComponentModel.DataAnnotations;

namespace SistemaAcademicoMVC.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}