using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public int Creditos { get; set; }

        public virtual ICollection<CursosPorCuatrimestre> CursosPorCuatrimestre { get; set; }
    }
}