using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class Participacion
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Asistio { get; set; }
        public string Descripcion { get; set; }

        public virtual Matricula Matricula { get; set; }
    }
}