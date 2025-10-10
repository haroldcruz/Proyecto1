using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

namespace SistemaAcademicoMVC.Controllers
{
    public class EstadisticasController : Controller
    {
        private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

        // Muestra la vista principal con filtros
        public ActionResult Index()
        {
            // Solo docentes autenticados
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            string correoDocente = Session["DocenteCorreo"]?.ToString();

            // Listado de cuatrimestres y cursos del docente para los filtros
            ViewBag.Cuatrimestres = db.Cuatrimestres.ToList();
            ViewBag.Cursos = db.Cursos.Where(c => c.CorreoDocente == correoDocente).ToList();

            return View();
        }

        // Acción AJAX que devuelve estadísticas filtradas
        [HttpPost]
        public JsonResult ObtenerEstadisticas(int cuatrimestreId, int cursoId)
        {
            // Consulta matrículas filtradas
            var matriculas = db.Matriculas
                .Where(m => m.CuatrimestreId == cuatrimestreId && m.CursoId == cursoId)
                .ToList();

            int total = matriculas.Count;
            int participantes = matriculas.Count(m => m.Evaluaciones.Any()); // Ejemplo: participación si tiene al menos una evaluación
            int aprobados = matriculas.Count(m => m.Evaluaciones.Any(e => e.NotaNumerica >= 70)); // Cambia el 70 según criterio
            int reprobados = total - aprobados;

            // Calcula porcentajes
            double porcentajeParticipacion = total > 0 ? (100.0 * participantes / total) : 0;
            double porcentajeAprobados = total > 0 ? (100.0 * aprobados / total) : 0;
            double porcentajeReprobados = total > 0 ? (100.0 * reprobados / total) : 0;

            return Json(new
            {
                total,
                porcentajeParticipacion,
                porcentajeAprobados,
                porcentajeReprobados
            });
        }
    }
}