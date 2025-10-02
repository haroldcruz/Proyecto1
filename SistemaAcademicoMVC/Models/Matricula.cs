using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public int EstudianteId { get; set; }
        public int CuatrimestreId { get; set; }
        public int CursoId { get; set; }

        public virtual Estudiante Estudiante { get; set; }
        public virtual Cuatrimestre Cuatrimestre { get; set; }
        public virtual Curso Curso { get; set; }
        public virtual ICollection<Evaluacion> Evaluaciones { get; set; }
        public virtual ICollection<Participacion> Participaciones { get; set; }
        public virtual ICollection<Tarea> Tareas { get; set; }
    }
}