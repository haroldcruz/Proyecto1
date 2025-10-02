using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class Tarea
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public bool Entregada { get; set; }

        public virtual Matricula Matricula { get; set; }
    }
}