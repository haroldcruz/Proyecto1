using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

namespace SistemaAcademicoMVC.Controllers
{
    /// <summary>
    /// Controlador para la gestión de estudiantes.
    /// Permite listar, crear, editar, ver detalles y eliminar estudiantes.
    /// </summary>
    public class EstudiantesController : Controller
    {
        // Instancia del contexto de base de datos para interactuar con los modelos.
        private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

        /// <summary>
        /// Acción que muestra la lista de todos los estudiantes registrados.
        /// </summary>
        public ActionResult Index()
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");
            // Código para mostrar estudiantes
            return View();
        }

        /// <summary>
        /// Acción que muestra los detalles de un estudiante específico.
        /// </summary>
        /// <param name="id">Identificador del estudiante</param>
        public ActionResult Details(int? id)
        {
            // Validación básica del parámetro
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Busca el estudiante por ID
            Estudiante estudiante = db.Estudiantes.Find(id);

            if (estudiante == null)
                return HttpNotFound();

            return View(estudiante);
        }

        /// <summary>
        /// Acción GET para mostrar el formulario de registro de estudiante.
        /// </summary>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Acción POST para registrar un nuevo estudiante en la base de datos.
        /// </summary>
        /// <param name="estudiante">Objeto estudiante recibido del formulario</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Apellidos,Identificacion,FechaNacimiento,Provincia,Canton,Distrito,Correo")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                db.Estudiantes.Add(estudiante);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Si hay errores de validación, vuelve al formulario
            return View(estudiante);
        }

        /// <summary>
        /// Acción GET para mostrar el formulario de edición de un estudiante existente.
        /// </summary>
        /// <param name="id">Identificador del estudiante</param>
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
                return HttpNotFound();

            return View(estudiante);
        }

        /// <summary>
        /// Acción POST para guardar los cambios de edición de un estudiante.
        /// </summary>
        /// <param name="estudiante">Objeto estudiante modificado</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Apellidos,Identificacion,FechaNacimiento,Provincia,Canton,Distrito,Correo")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estudiante).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estudiante);
        }

        /// <summary>
        /// Acción GET para mostrar la confirmación de eliminación de un estudiante.
        /// </summary>
        /// <param name="id">Identificador del estudiante</param>
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
                return HttpNotFound();

            return View(estudiante);
        }

        /// <summary>
        /// Acción POST para eliminar el estudiante de la base de datos.
        /// </summary>
        /// <param name="id">Identificador del estudiante</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estudiante estudiante = db.Estudiantes.Find(id);
            db.Estudiantes.Remove(estudiante);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Libera los recursos del contexto de base de datos cuando el controlador se destruye.
        /// </summary>
        /// <param name="disposing">Indica si se están liberando recursos administrados</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}