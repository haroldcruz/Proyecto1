using System;
using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

public class EvaluacionController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    // GET: muestra el formulario para calificar
    [HttpGet]
    public ActionResult Calificar(int cursoId, int estudianteId)
    {
        var matricula = db.Matriculas.FirstOrDefault(m => m.CursoId == cursoId && m.EstudianteId == estudianteId);
        if (matricula == null)
            return HttpNotFound("Matrícula no encontrada.");

        ViewBag.MatriculaId = matricula.Id;
        ViewBag.Estudiante = matricula.Estudiante;
        ViewBag.Curso = matricula.Curso;

        var evaluacion = new Evaluacion
        {
            MatriculaId = matricula.Id
        };

        return View(evaluacion);
    }

    // POST: recibe datos del formulario y guarda la evaluación (compatible AJAX)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Calificar(Evaluacion evaluacion)
    {
        // Recupera datos de la matrícula (para usar estudiante y curso en ViewBag si ocupas en la vista tradicional)
        var matricula = db.Matriculas.FirstOrDefault(m => m.Id == evaluacion.MatriculaId);
        if (matricula != null)
        {
            ViewBag.Estudiante = matricula.Estudiante;
            ViewBag.Curso = matricula.Curso;
        }

        if (!ModelState.IsValid)
        {
            var errores = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Campo = x.Key,
                    Mensaje = x.Value.Errors.First().ErrorMessage
                }).ToList();

            if (Request.IsAjaxRequest())
                return Json(new { success = false, errores });

            return View(evaluacion);
        }

        bool existe = db.Evaluaciones.Any(e => e.MatriculaId == evaluacion.MatriculaId);
        if (existe)
        {
            if (Request.IsAjaxRequest())
                return Json(new { success = false, errores = new[] { new { Campo = "General", Mensaje = "Ya existe una evaluación para esta matrícula." } } });

            ModelState.AddModelError("", "Ya existe una evaluación para esta matrícula.");
            return View(evaluacion);
        }

        evaluacion.Fecha = DateTime.Now;
        db.Evaluaciones.Add(evaluacion);
        db.SaveChanges();

        if (Request.IsAjaxRequest())
            return Json(new { success = true, mensaje = "Calificación registrada exitosamente." });

        return RedirectToAction("Index", "Estudiantes"); // Corrige el nombre del controlador
    }
}
