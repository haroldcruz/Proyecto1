using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    /// <summary>
    /// Representa la entidad Curso en el sistema académico.
    /// Incluye la asociación con el docente mediante su correo.
    /// </summary>
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int Creditos { get; set; }

        /// <summary>
        /// Correo del docente que imparte el curso.
        /// Se asigna automáticamente desde la sesión al crear el curso.
        /// </summary>
        public string CorreoDocente { get; set; }

        public virtual ICollection<CursosPorCuatrimestre> CursosPorCuatrimestre { get; set; }
    }
}