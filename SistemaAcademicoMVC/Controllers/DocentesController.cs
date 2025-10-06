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

        // Validación de correo único
        if (db.Docentes.Any(d => d.Correo == model.Correo && d.Id != docenteId))
        {
            ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
            return View(model);
        }

        // Edita campos permitidos
        docente.Nombre = model.Nombre;
        docente.Correo = model.Correo;
        docente.Password = model.Password;

        db.SaveChanges();

        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
        Session["DocenteNombre"] = docente.Nombre;

        return RedirectToAction("EditarPerfil");
    }

    /// <summary>
    /// Muestra el formulario de registro de docentes.
    /// </summary>
    [HttpGet]
    public ActionResult Register() => View();

    /// <summary>
    /// Procesa el registro de docentes y muestra mensaje de éxito o error.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(Docente model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Verifica si el correo ya está registrado
        if (db.Docentes.Any(d => d.Correo == model.Correo))
        {
            ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
            return View(model);
        }

        // Hashea la contraseña
        model.Password = PasswordHelper.HashPassword(model.Password);
        db.Docentes.Add(model);
        db.SaveChanges();

        TempData["SuccessMessage"] = "¡Registro exitoso! Ahora puedes ingresar.";

        return RedirectToAction("Login", "Account");
    }
}