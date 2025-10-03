using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

public class DocentesController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    // ... otras acciones ...

    /// <summary>
    /// Muestra el formulario para editar el perfil del docente autenticado.
    /// </summary>
    [HttpGet]
    public ActionResult EditarPerfil()
    {
        // Verifica autenticación
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        int docenteId = (int)Session["DocenteId"];
        var docente = db.Docentes.Find(docenteId);
        if (docente == null)
            return HttpNotFound();

        return View(docente);
    }

    /// <summary>
    /// Procesa la edición del perfil del docente autenticado.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarPerfil(Docente model)
    {
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        int docenteId = (int)Session["DocenteId"];
        var docente = db.Docentes.Find(docenteId);

        if (docente == null)
            return HttpNotFound();

        if (!ModelState.IsValid)
            return View(model);

        // Validación de unicidad de correo (excepto para el propio usuario)
        if (db.Docentes.Any(d => d.Correo == model.Correo && d.Id != docenteId))
        {
            ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
            return View(model);
        }

        // Solo permite modificar Nombre, Correo y Password
        docente.Nombre = model.Nombre;
        docente.Correo = model.Correo;
        docente.Password = model.Password;

        db.SaveChanges();

        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
        // Actualiza el nombre en sesión por si cambió
        Session["DocenteNombre"] = docente.Nombre;

        return RedirectToAction("EditarPerfil");
    }
}