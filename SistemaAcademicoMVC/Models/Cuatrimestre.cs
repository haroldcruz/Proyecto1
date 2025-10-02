using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class Cuatrimestre
    {
        public int Id { get; set; }
        public int Año { get; set; }
        public int Numero { get; set; } // 1, 2, 3, 4
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public virtual ICollection<CursosPorCuatrimestre> CursosPorCuatrimestre { get; set; }
    }
}