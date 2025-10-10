using System;
using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;
using System.Collections.Generic;
using System.Data.Entity;

public class EvaluacionController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    // GET: Calificar, muestra el formulario principal
    [HttpGet]
    public ActionResult Calificar(int cursoId, int estudianteId)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var matricula = db.Matriculas
            .Include(m => m.Estudiante)
            .Include(m => m.Curso)
            .Include(m => m.Tareas)
            .Include(m => m.Participaciones)
            .FirstOrDefault(m => m.CursoId == cursoId && m.EstudianteId == estudianteId);

        if (matricula == null)
            return HttpNotFound("Matrícula no encontrada.");

        var model = new CalificarViewModel
        {
            Matricula = matricula,
            Tareas = matricula.Tareas.ToList(),
            Participaciones = matricula.Participaciones.ToList(),
            Evaluacion = db.Evaluaciones.FirstOrDefault(e => e.MatriculaId == matricula.Id)
        };

        return View(model);
    }

    // GET: Editar tarea
    [HttpGet]
    public ActionResult EditarTarea(int id)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var tarea = db.Tareas.Include(t => t.Matricula).FirstOrDefault(t => t.Id == id);
        if (tarea == null)
            return HttpNotFound();

        return View(tarea);
    }

    // POST: Editar tarea (nota y entregada)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarTarea(Tarea tarea)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var tareaOriginal = db.Tareas.Find(tarea.Id);
        if (tareaOriginal == null)
            return HttpNotFound();

        tareaOriginal.Nota = tarea.Nota;
        tareaOriginal.Entregada = tarea.Entregada;
        db.SaveChanges();

        return RedirectToAction("Calificar", new { cursoId = tareaOriginal.Matricula.CursoId, estudianteId = tareaOriginal.Matricula.EstudianteId });
    }

    // POST: Generar nota final
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GenerarNotaFinal(int matriculaId)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var matricula = db.Matriculas
            .Include(m => m.Tareas)
            .Include(m => m.Participaciones)
            .FirstOrDefault(m => m.Id == matriculaId);

        if (matricula == null)
            return HttpNotFound("Matrícula no encontrada.");

        // Lógica de cálculo ponderado de la nota final
        decimal notaFinal = 0;
        decimal sumaPonderacion = 0;
        foreach (var tarea in matricula.Tareas)
        {
            if (tarea.Nota.HasValue)
            {
                notaFinal += tarea.Nota.Value * (tarea.PorcentajePonderacion / 100);
                sumaPonderacion += tarea.PorcentajePonderacion;
            }
        }
        // Participaciones: el resto del porcentaje se asigna a participación
        decimal porcentajeParticipacion = 100 - sumaPonderacion;
        decimal participacionValor = 0;
        if (matricula.Participaciones.Any() && porcentajeParticipacion > 0)
        {
            decimal puntosPorParticipacion = porcentajeParticipacion / matricula.Participaciones.Count;
            foreach (var part in matricula.Participaciones)
            {
                participacionValor += (part.Asistio ? puntosPorParticipacion : 0);
            }
        }
        notaFinal += participacionValor;

        // Guarda o actualiza evaluación
        var evaluacion = db.Evaluaciones.FirstOrDefault(e => e.MatriculaId == matricula.Id);
        if (evaluacion == null)
        {
            evaluacion = new Evaluacion
            {
                MatriculaId = matricula.Id,
                Fecha = DateTime.Now,
                NotaNumerica = notaFinal,
                Estado = notaFinal >= 70 ? "Aprobado" : "Reprobado"
            };
            db.Evaluaciones.Add(evaluacion);
        }
        else
        {
            evaluacion.NotaNumerica = notaFinal;
            evaluacion.Fecha = DateTime.Now;
            evaluacion.Estado = notaFinal >= 70 ? "Aprobado" : "Reprobado";
            db.Entry(evaluacion).State = EntityState.Modified;
        }
        db.SaveChanges();

        TempData["Success"] = "Nota final generada.";
        return RedirectToAction("Calificar", new { cursoId = matricula.CursoId, estudianteId = matricula.EstudianteId });
    }
    // GET: Editar participación
    [HttpGet]
    public ActionResult EditarParticipacion(int id)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var participacion = db.Participaciones.Include(p => p.Matricula).FirstOrDefault(p => p.Id == id);
        if (participacion == null)
            return HttpNotFound();

        return View(participacion);
    }

    // POST: Editar participación
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarParticipacion(Participacion participacion)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var original = db.Participaciones.Find(participacion.Id);
        if (original == null)
            return HttpNotFound();

        original.Asistio = participacion.Asistio;
        db.SaveChanges();

        // Redirige a la calificación del estudiante
        return RedirectToAction("Calificar", new { cursoId = original.Matricula.CursoId, estudianteId = original.Matricula.EstudianteId });
    }
    // Acción GET para mostrar el formulario de participación
    [HttpGet]
    public ActionResult CrearParticipacion(int matriculaId)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");
        return View(matriculaId);
    }

    // Acción POST para guardar la participación
    // Acción POST para guardar la participación con fecha
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CrearParticipacion(int matriculaId, bool Asistio = false, DateTime? Fecha = null)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        var matricula = db.Matriculas.Find(matriculaId);
        if (matricula == null)
            return HttpNotFound();

        var participacion = new Participacion
        {
            MatriculaId = matriculaId,
            Asistio = Asistio,
            Fecha = Fecha ?? DateTime.Now // Usa la fecha enviada o la actual
        };
        db.Participaciones.Add(participacion);
        db.SaveChanges();

        return RedirectToAction("Calificar", new { cursoId = matricula.CursoId, estudianteId = matricula.EstudianteId });
    }
}

// ViewModel descriptivo para la vista de calificar
public class CalificarViewModel
{
    public Matricula Matricula { get; set; }
    public List<Tarea> Tareas { get; set; }
    public List<Participacion> Participaciones { get; set; }
    public Evaluacion Evaluacion { get; set; }
}