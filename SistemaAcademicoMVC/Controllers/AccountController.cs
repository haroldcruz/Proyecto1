using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

/// <summary>
/// Controlador de autenticación de docentes.
/// Gestiona login, registro y cierre de sesión.
/// </summary>
public class AccountController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    /// <summary>
    /// Muestra el formulario de login.
    /// </summary>
    [HttpGet]
    public ActionResult Login() => View();

    /// <summary>
    /// Procesa el login y muestra mensajes de éxito o error.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        // Verifica que el modelo sea válido antes de continuar
        if (!ModelState.IsValid)
        {
            // Permite mostrar errores de validación en la vista
            return View(model);
        }

        // Busca al docente por correo y contraseña (simple, ajusta a tus necesidades)
        var docente = db.Docentes.FirstOrDefault(d => d.Correo == model.Correo && d.Password == model.Password);
        if (docente != null)
        {
            // Guarda información del docente en la sesión para autenticación
            Session["DocenteId"] = docente.Id;
            Session["DocenteNombre"] = docente.Nombre;
            Session["DocenteCorreo"] = docente.Correo;
            // Prepara mensaje de éxito para mostrar en la siguiente vista
            TempData["SuccessMessage"] = "¡Bienvenido, " + docente.Nombre + "!";

            // Redirige a la página principal
            return RedirectToAction("Index", "Home");
        }

        // Si las credenciales no son válidas, muestra mensaje de error
        ModelState.AddModelError("", "Credenciales inválidas");
        return View(model);
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
        {
            // Permite mostrar errores de validación en la vista
            return View(model);
        }

        // Verifica si el correo ya está registrado
        if (db.Docentes.Any(d => d.Correo == model.Correo))
        {
            ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
            return View(model);
        }

        db.Docentes.Add(model);
        db.SaveChanges();

        // Mensaje de éxito para mostrar después del registro
        TempData["SuccessMessage"] = "¡Registro exitoso! Ahora puedes ingresar.";

        return RedirectToAction("Login");
    }

    /// <summary>
    /// Cierra la sesión y redirige a la página principal.
    /// </summary>
    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}