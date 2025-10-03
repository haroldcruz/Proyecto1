using System.Linq;
using System.Net;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

namespace SistemaAcademicoMVC.Controllers
{
    /// <summary>
    /// Controlador para la gestión de cursos.
    /// Permite listar, registrar, editar y eliminar cursos, asociando el curso al docente mediante el correo en sesión.
    /// </summary>
    public class CursosController : Controller
    {
        private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

        /// <summary>
        /// Muestra la lista de cursos asociados al docente autenticado.
        /// </summary>
        public ActionResult Index()
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            string correoDocente = Session["DocenteCorreo"].ToString();

            // Sólo mostrar cursos del docente autenticado
            var cursos = db.Cursos.Where(c => c.CorreoDocente == correoDocente).ToList();
            return View(cursos);
        }

        /// <summary>
        /// Muestra el formulario para registrar un nuevo curso.
        /// </summary>
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            return View();
 
        }

        /// <summary>
        /// Procesa el registro del nuevo curso y lo asocia al docente autenticado.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CursoViewModel model)
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            string correoDocente = Session["DocenteCorreo"].ToString();
            // Validar que no exista un curso con el mismo nombre para el docente actual
            if (db.Cursos.Any(c => c.Nombre == model.Nombre && c.CorreoDocente == correoDocente))
            {
                ModelState.AddModelError("Nombre", "Ya existe un curso con ese nombre.");
                return View(model);
            }

            // Si quieres validar por código también:
            if (db.Cursos.Any(c => c.Codigo == model.Codigo && c.CorreoDocente == correoDocente))
            {
                ModelState.AddModelError("Codigo", "Ya existe un curso con ese código.");
                return View(model);
            }
            // Crear la entidad Curso y asignar el correo del docente
            var curso = new Curso
            {
                Nombre = model.Nombre,
                Codigo = model.Codigo,
                Creditos = model.Creditos,
                CorreoDocente = correoDocente
            };

            db.Cursos.Add(curso);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Curso registrado correctamente.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Muestra el formulario para editar un curso existente.
        /// Solo permite editar cursos del docente autenticado.
        /// </summary>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string correoDocente = Session["DocenteCorreo"].ToString();
            var curso = db.Cursos.FirstOrDefault(c => c.Id == id && c.CorreoDocente == correoDocente);

            if (curso == null)
                return HttpNotFound();

            // Prepara el modelo para la edición
            var model = new CursoViewModel
            {
                Nombre = curso.Nombre,
                Codigo = curso.Codigo,
                Creditos = curso.Creditos
            };

            ViewBag.CursoId = curso.Id; // Para usarlo en el formulario
            return View(model);
        }

        /// <summary>
        /// Procesa la edición de un curso.
        /// Solo permite editar cursos del docente autenticado.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CursoViewModel model)
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.CursoId = id;
                return View(model);
            }

            string correoDocente = Session["DocenteCorreo"].ToString();
            var curso = db.Cursos.FirstOrDefault(c => c.Id == id && c.CorreoDocente == correoDocente);

            if (curso == null)
                return HttpNotFound();

            // Actualiza los datos del curso
            curso.Nombre = model.Nombre;
            curso.Codigo = model.Codigo;
            curso.Creditos = model.Creditos;

            db.SaveChanges();

            TempData["SuccessMessage"] = "Curso modificado correctamente.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Muestra la confirmación para eliminar un curso.
        /// Solo permite eliminar cursos del docente autenticado.
        /// </summary>
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string correoDocente = Session["DocenteCorreo"].ToString();
            var curso = db.Cursos.FirstOrDefault(c => c.Id == id && c.CorreoDocente == correoDocente);

            if (curso == null)
                return HttpNotFound();

            return View(curso);
        }

        /// <summary>
        /// Procesa la eliminación de un curso.
        /// Solo permite eliminar cursos del docente autenticado.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["DocenteCorreo"] == null)
                return RedirectToAction("Login", "Account");

            string correoDocente = Session["DocenteCorreo"].ToString();
            var curso = db.Cursos.FirstOrDefault(c => c.Id == id && c.CorreoDocente == correoDocente);

            if (curso == null)
                return HttpNotFound();

            db.Cursos.Remove(curso);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Curso eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}