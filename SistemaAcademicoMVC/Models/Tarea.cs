using System;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    // Modelo de Tarea con campo Nota
    public class Tarea
    {
        public int Id { get; set; }
        public int MatriculaId { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public decimal PorcentajePonderacion { get; set; } // Ponderación de la tarea
        public bool Entregada { get; set; } // Indica si fue entregada
        public decimal? Nota { get; set; } // Nota obtenida en la tarea

        public virtual Matricula Matricula { get; set; }
    }
}