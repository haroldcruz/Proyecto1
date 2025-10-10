using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;
using System.Data.Entity;
using PagedList; // Usar PagedList para paginación

namespace SistemaAcademicoMVC.Controllers
{
    public class ParticipacionController : Controller
    {
        private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

        // Listado con filtro y paginación
        public ActionResult Index(int? page, int? cursoId, string search)
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            var correoDocente = Session["DocenteCorreo"]?.ToString();

            // Cursos del docente para el filtro
            var cursos = db.Cursos
                .Where(c => c.CorreoDocente == correoDocente)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToList();
            ViewBag.Cursos = cursos;

            // Filtrar participaciones según búsqueda y curso
            var participaciones = db.Participaciones
                .Include(p => p.Matricula.Estudiante)
                .Include(p => p.Matricula.Curso)
                .AsQueryable();

            if (cursoId.HasValue)
                participaciones = participaciones.Where(p => p.Matricula.Curso.Id == cursoId.Value);

            if (!string.IsNullOrEmpty(search))
                participaciones = participaciones.Where(p =>
                    p.Matricula.Estudiante.Nombre.Contains(search) ||
                    p.Matricula.Estudiante.Apellidos.Contains(search) ||
                    p.Descripcion.Contains(search));

            participaciones = participaciones.OrderByDescending(p => p.Fecha);

            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(participaciones.ToPagedList(pageNumber, pageSize));
        }

        // Vista de registro (sin cambios)
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["DocenteId"] == null)
                return RedirectToAction("Login", "Account");

            var correoDocente = Session["DocenteCorreo"]?.ToString();
            ViewBag.Cursos = db.Cursos
                .Where(c => c.CorreoDocente == correoDocente)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                }).ToList();

            return View();
        }

        // Resto de acciones sin cambios...
    }
}