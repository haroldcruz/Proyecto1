using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

namespace SistemaAcademicoMVC.Controllers
{
    public class AsignacionesController : Controller
    {
        private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

        // Listar tareas
        public ActionResult Index(int? cursoId)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            var tareas = db.Tareas.Include(t => t.Matricula.Curso);
            if (cursoId.HasValue)
                tareas = tareas.Where(t => t.Matricula.CursoId == cursoId);
            return View(tareas.ToList());
        }

        // Detalle
        public ActionResult Details(int id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            var tarea = db.Tareas.Include(t => t.Matricula.Curso).FirstOrDefault(t => t.Id == id);
            if (tarea == null) return HttpNotFound();
            return View(tarea);
        }

        // Crear GET
        public ActionResult Create()
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            ViewBag.MatriculaId = new SelectList(db.Matriculas.Include(m => m.Curso), "Id", "Curso.Nombre");
            return View();
        }

        // Crear POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tarea tarea)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                db.Tareas.Add(tarea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MatriculaId = new SelectList(db.Matriculas.Include(m => m.Curso), "Id", "Curso.Nombre", tarea.MatriculaId);
            return View(tarea);
        }

        // Editar GET
        public ActionResult Edit(int id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            var tarea = db.Tareas.Find(id);
            if (tarea == null) return HttpNotFound();
            ViewBag.MatriculaId = new SelectList(db.Matriculas.Include(m => m.Curso), "Id", "Curso.Nombre", tarea.MatriculaId);
            return View(tarea);
        }

        // Editar POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tarea tarea)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MatriculaId = new SelectList(db.Matriculas.Include(m => m.Curso), "Id", "Curso.Nombre", tarea.MatriculaId);
            return View(tarea);
        }

        // Eliminar GET
        public ActionResult Delete(int id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            var tarea = db.Tareas.Include(t => t.Matricula.Curso).FirstOrDefault(t => t.Id == id);
            if (tarea == null) return HttpNotFound();
            return View(tarea);
        }

        // Eliminar POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            var tarea = db.Tareas.Find(id);
            db.Tareas.Remove(tarea);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}