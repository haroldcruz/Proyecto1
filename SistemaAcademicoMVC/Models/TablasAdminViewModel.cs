// ViewModel para mostrar varias tablas en la vista de administración
using System.Collections.Generic;

namespace SistemaAcademicoMVC.Models
{
    public class TablasAdminViewModel
    {
        public List<Docente> Docentes { get; set; }
        public List<string> DocentesContrasenas { get; set; } // Contraseñas de docentes
        public List<Estudiante> Estudiantes { get; set; }
        public List<string> EstudiantesContrasenas { get; set; } // Contraseñas de estudiantes
        public List<Curso> Cursos { get; set; }
        public List<Matricula> Matriculas { get; set; }
        public List<Cuatrimestre> Cuatrimestres { get; set; }
        public List<Evaluacion> Evaluaciones { get; set; }
        public List<Participacion> Participaciones { get; set; }
        public List<Tarea> Tareas { get; set; }
    }
}