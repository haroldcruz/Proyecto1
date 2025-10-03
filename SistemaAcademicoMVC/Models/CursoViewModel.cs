using System.ComponentModel.DataAnnotations;

namespace SistemaAcademicoMVC.Models
{
    /// <summary>
    /// ViewModel para el registro de cursos.
    /// El correo del docente se asigna automáticamente, no se ingresa aquí.
    /// </summary>
    public class CursoViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Los créditos deben estar entre 1 y 20.")]
        public int Creditos { get; set; }
    }
}