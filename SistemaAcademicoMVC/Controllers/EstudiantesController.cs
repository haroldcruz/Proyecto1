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
        /// SOLO muestra los estudiantes matriculados en cursos del docente logueado.
        /// </summary>
        public ActionResult Index()
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            string correoDocente = Session["DocenteCorreo"]?.ToString();

            // IDs de cursos impartidos por el docente actual
            var cursosDocenteIds = db.Cursos
                .Where(c => c.CorreoDocente == correoDocente)
                .Select(c => c.Id)
                .ToList();

            // IDs de estudiantes matriculados en esos cursos
            var estudiantesIds = db.Matriculas
                .Where(m => cursosDocenteIds.Contains(m.CursoId))
                .Select(m => m.EstudianteId)
                .Distinct()
                .ToList();

            // Solo los estudiantes que están matriculados en los cursos del docente, incluye matrículas
            var lista = db.Estudiantes
                .Include(e => e.Matriculas) // Incluye las matrículas de cada estudiante
                .Where(e => estudiantesIds.Contains(e.Id))
                .ToList();

            return View(lista);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Estudiante estudiante = db.Estudiantes.Find(id);

            if (estudiante == null)
                return HttpNotFound();

            // Carga los cursos matriculados por el estudiante para mostrar en la vista
            var matriculas = db.Matriculas
                .Include(m => m.Curso)
                .Include(m => m.Cuatrimestre)
                .Where(m => m.EstudianteId == id)
                .ToList();
            ViewBag.CursosMatriculados = matriculas;

            return View(estudiante);
        }

        /// <summary>
        /// Acción GET para mostrar el formulario de registro de estudiante.
        /// Muestra solo los cursos que pertenecen al docente autenticado.
        /// </summary>
        public ActionResult Create()
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            // Obtener el correo del docente autenticado
            string correoDocente = Session["DocenteCorreo"]?.ToString();

            // Cuatrimestres disponibles
            ViewBag.Cuatrimestres = db.Cuatrimestres
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Año + " - " + c.Numero
                }).ToList();

            // Solo los cursos del docente autenticado
            ViewBag.CursosSeleccionados = db.Cursos
                .Where(c => c.CorreoDocente == correoDocente)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToList();

            return View();
        }

        /// <summary>
        /// Acción POST para registrar un nuevo estudiante.
        /// La validación se realiza solo en este método y no se modifica ninguna otra lógica.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estudiante estudiante, int? CuatrimestreId, int[] CursosSeleccionados)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            // Validación para campos obligatorios
            if (string.IsNullOrWhiteSpace(estudiante.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
            if (CursosSeleccionados == null || CursosSeleccionados.Length == 0)
                ModelState.AddModelError("CursosSeleccionados", "Debe seleccionar al menos un curso.");
            if (string.IsNullOrWhiteSpace(estudiante.Apellidos))
                ModelState.AddModelError("Apellidos", "Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(estudiante.Identificacion))
                ModelState.AddModelError("Identificacion", "La identificación es obligatoria.");
            else if (db.Estudiantes.Any(e => e.Identificacion == estudiante.Identificacion))
                ModelState.AddModelError("Identificacion", "Ya existe otro estudiante con esa identificación.");

            if (estudiante.FechaNacimiento == null)
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento es obligatoria.");
            else if (estudiante.FechaNacimiento > DateTime.Today)
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser mayor a hoy.");

            if (string.IsNullOrWhiteSpace(estudiante.Provincia))
                ModelState.AddModelError("Provincia", "La provincia es obligatoria.");

            if (string.IsNullOrWhiteSpace(estudiante.Canton))
                ModelState.AddModelError("Canton", "El cantón es obligatorio.");

            if (string.IsNullOrWhiteSpace(estudiante.Distrito))
                ModelState.AddModelError("Distrito", "El distrito es obligatorio.");

            if (string.IsNullOrWhiteSpace(estudiante.Correo))
                ModelState.AddModelError("Correo", "El correo es obligatorio.");
            else if (!estudiante.Correo.Contains("@") || !estudiante.Correo.Contains("."))
                ModelState.AddModelError("Correo", "El correo no tiene un formato válido.");
            else if (db.Estudiantes.Any(e => e.Correo == estudiante.Correo))
                ModelState.AddModelError("Correo", "Ya existe otro estudiante con ese correo.");

            // Validar selección de cuatrimestre
            if (CuatrimestreId == null)
                ModelState.AddModelError("CuatrimestreId", "Debe seleccionar un cuatrimestre.");

            if (ModelState.IsValid)
            {
                db.Estudiantes.Add(estudiante);
                db.SaveChanges();

                // Matricular en los cursos seleccionados
                if (CursosSeleccionados != null)
                {
                    foreach (var cursoId in CursosSeleccionados)
                    {
                        var matricula = new Matricula
                        {
                            EstudianteId = estudiante.Id,
                            CursoId = cursoId,
                            CuatrimestreId = CuatrimestreId.Value // Ya garantizado no nulo
                        };
                        db.Matriculas.Add(matricula);
                    }
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Recarga combos en caso de error
            string correoDocente = Session["DocenteCorreo"]?.ToString();

            ViewBag.Cuatrimestres = db.Cuatrimestres
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Año + " - " + c.Numero
                }).ToList();

            ViewBag.CursosSeleccionados = db.Cursos
                .Where(c => c.CorreoDocente == correoDocente)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToList();

            return View(estudiante);
        }

        public ActionResult Edit(int? id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
                return HttpNotFound();

            return View(estudiante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Apellidos,Identificacion,FechaNacimiento,Provincia,Canton,Distrito,Correo")] Estudiante estudiante)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            // Validaciones personalizadas
            if (string.IsNullOrWhiteSpace(estudiante.Nombre))
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(estudiante.Apellidos))
                ModelState.AddModelError("Apellidos", "Los apellidos son obligatorios.");

            if (string.IsNullOrWhiteSpace(estudiante.Identificacion))
                ModelState.AddModelError("Identificacion", "La identificación es obligatoria.");
            // Solo error si la identificación ya existe en otro estudiante
            else if (db.Estudiantes.Any(e => e.Identificacion == estudiante.Identificacion && e.Id != estudiante.Id))
                ModelState.AddModelError("Identificacion", "Ya existe otro estudiante con esa identificación.");

            if (string.IsNullOrWhiteSpace(estudiante.Correo))
                ModelState.AddModelError("Correo", "El correo es obligatorio.");
            else if (!estudiante.Correo.Contains("@") || !estudiante.Correo.Contains("."))
                ModelState.AddModelError("Correo", "El correo no tiene un formato válido.");
            // Solo error si el correo ya existe en otro estudiante
            else if (db.Estudiantes.Any(e => e.Correo == estudiante.Correo && e.Id != estudiante.Id))
                ModelState.AddModelError("Correo", "Ya existe otro estudiante con ese correo.");

            if (estudiante.FechaNacimiento == null)
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento es obligatoria.");
            else if (estudiante.FechaNacimiento > DateTime.Today)
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser mayor a hoy.");

            if (string.IsNullOrWhiteSpace(estudiante.Provincia))
                ModelState.AddModelError("Provincia", "La provincia es obligatoria.");

            if (string.IsNullOrWhiteSpace(estudiante.Canton))
                ModelState.AddModelError("Canton", "El cantón es obligatorio.");

            if (string.IsNullOrWhiteSpace(estudiante.Distrito))
                ModelState.AddModelError("Distrito", "El distrito es obligatorio.");

            if (ModelState.IsValid)
            {
                db.Entry(estudiante).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estudiante);
        }

        public ActionResult Delete(int? id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Estudiante estudiante = db.Estudiantes.Find(id);
            if (estudiante == null)
                return HttpNotFound();

            return View(estudiante);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            Estudiante estudiante = db.Estudiantes.Find(id);
            db.Estudiantes.Remove(estudiante);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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