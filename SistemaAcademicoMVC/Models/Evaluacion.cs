using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class Evaluacion
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public decimal NotaNumerica { get; set; }
        public string Observaciones { get; set; }
        public string TipoParticipacion { get; set; }
        public string Estado { get; set; } // Aprobado, Reprobado
        public DateTime Fecha { get; set; }

        public virtual Matricula Matricula { get; set; }
    }
}