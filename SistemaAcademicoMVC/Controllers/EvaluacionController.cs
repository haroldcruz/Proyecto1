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
        // Protege la ruta
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

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

    // POST: guarda la evaluación (solo creación)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Calificar(Evaluacion evaluacion)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        // Valida modelo
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

        // Verifica duplicado solo en creación
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

        return RedirectToAction("Index", "Estudiantes");
    }

    // GET: editar calificación
    [HttpGet]
    public ActionResult Edit(int id)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var evaluacion = db.Evaluaciones
            .Include("Matricula.Estudiante")
            .Include("Matricula.Curso")
            .FirstOrDefault(e => e.Id == id);

        if (evaluacion == null)
            return HttpNotFound();

        ViewBag.MatriculaId = evaluacion.MatriculaId;
        ViewBag.Estudiante = evaluacion.Matricula.Estudiante;
        ViewBag.Curso = evaluacion.Matricula.Curso;

        return View("Calificar", evaluacion); // reutiliza vista
    }

    // POST: actualiza calificación
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Evaluacion evaluacion)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View("Calificar", evaluacion);

        evaluacion.Fecha = DateTime.Now;
        db.Entry(evaluacion).State = System.Data.Entity.EntityState.Modified;
        db.SaveChanges();

        // Devuelve JSON si es AJAX, si no redirige
        if (Request.IsAjaxRequest())
            return Json(new { success = true, mensaje = "Calificación modificada exitosamente." });

        return RedirectToAction("Index", "Estudiantes");
    }
}