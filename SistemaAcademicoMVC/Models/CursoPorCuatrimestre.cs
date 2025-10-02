using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaAcademicoMVC.Models
{
    public class CursosPorCuatrimestre
    {
        public int Id { get; set; }
        public int CuatrimestreId { get; set; }
        public int CursoId { get; set; }

        public virtual Cuatrimestre Cuatrimestre { get; set; }
        public virtual Curso Curso { get; set; }
    }
}