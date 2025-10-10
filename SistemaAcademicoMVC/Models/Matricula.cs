using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAcademicoMVC.Models
{
    public class Matricula
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EstudianteId { get; set; }
        [Required]
        public int CuatrimestreId { get; set; }
        [Required]
        public int CursoId { get; set; }

        [ForeignKey("EstudianteId")]
        public virtual Estudiante Estudiante { get; set; }
        [ForeignKey("CuatrimestreId")]
        public virtual Cuatrimestre Cuatrimestre { get; set; }
        [ForeignKey("CursoId")]
        public virtual Curso Curso { get; set; }

        public virtual ICollection<Evaluacion> Evaluaciones { get; set; }
        public virtual ICollection<Participacion> Participaciones { get; set; }
        public virtual ICollection<Tarea> Tareas { get; set; }
    }
}