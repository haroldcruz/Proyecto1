using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

public class AdminController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    public ActionResult Administrador()
    {
        // En el método Administrador del AdminController
        var vm = new TablasAdminViewModel
        {
            Docentes = db.Docentes.ToList(),
            DocentesContrasenas = db.Docentes.Select(d => d.Password).ToList(),
            Estudiantes = db.Estudiantes.ToList(),

            Cursos = db.Cursos.ToList(),
            Matriculas = db.Matriculas.ToList(),
            Cuatrimestres = db.Cuatrimestres.ToList(),
            Evaluaciones = db.Evaluaciones.ToList(),
            Participaciones = db.Participaciones.ToList(),
            Tareas = db.Tareas.ToList()
        };
        return View(vm);
    }

    [HttpPost]
    public ActionResult Limpiar()
    {
        // Borra datos de todas las tablas principales y relacionadas
        db.Evaluaciones.RemoveRange(db.Evaluaciones);
        db.Participaciones.RemoveRange(db.Participaciones);
        db.Tareas.RemoveRange(db.Tareas);
        db.Docentes.RemoveRange(db.Docentes);
        db.Estudiantes.RemoveRange(db.Estudiantes);
        db.Cursos.RemoveRange(db.Cursos);
        db.Matriculas.RemoveRange(db.Matriculas);
        db.SaveChanges();
        return RedirectToAction("Administrador");
    }
}