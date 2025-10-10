using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademicoMVC.Models
{
    public class Participacion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int MatriculaId { get; set; }

        [ForeignKey("MatriculaId")]
        public virtual Matricula Matricula { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public bool Asistio { get; set; }
        public string Descripcion { get; set; }
    }
}