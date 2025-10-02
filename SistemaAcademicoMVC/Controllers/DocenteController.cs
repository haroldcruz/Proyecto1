using System.Web.Mvc;
using SistemaAcademicoMVC.Models; // Ajusta este using según el namespace de tus modelos

public class DocentesController : Controller
{
    // Instancia del contexto de base de datos (Asegúrate de tenerlo y liberarlo correctamente)
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    /// <summary>
    /// Panel principal del docente (solo accesible si está autenticado).
    /// </summary>
    public ActionResult Panel()
    {
        // Redirige a Login si no hay sesión activa
        if (Session["DocenteId"] == null)
            return RedirectToAction("Login", "Account");

        // Aquí puedes cargar datos específicos del panel si lo necesitas
        return View();
    }

    /// <summary>
    /// Redirige el Index de docentes al perfil del usuario (no se muestra listado de docentes).
    /// </summary>
    public ActionResult Index()
    {
        // Redirige directamente a editar el perfil
        return RedirectToAction("EditarPerfil");
    }

    /// <summary>
    /// Muestra el formulario para editar el perfil del docente autenticado.
    /// </summary>
    public ActionResult EditarPerfil()
    {
        int? docenteId = Session["DocenteId"] as int?;
        if (docenteId == null)
            return RedirectToAction("Login", "Account");

        // Busca el docente por su Id de sesión
        var docente = db.Docentes.Find(docenteId.Value);
        if (docente == null)
            return HttpNotFound();

        // Devuelve la vista con los datos actuales del docente
        return View(docente);
    }

    /// <summary>
    /// Procesa el formulario de edición de perfil (solo para el docente autenticado).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditarPerfil(Docente model)
    {
        int? docenteId = Session["DocenteId"] as int?;
        if (docenteId == null)
            return RedirectToAction("Login", "Account");

        // Busca el docente en la base de datos
        var docente = db.Docentes.Find(docenteId.Value);
        if (docente == null)
            return HttpNotFound();

        // Actualiza solo los campos permitidos
        docente.Nombre = model.Nombre;
        docente.Correo = model.Correo;
        // Si permites cambio de contraseña, agrega aquí la lógica (opcional)
        // docente.Password = model.Password;

        db.SaveChanges();
        ViewBag.Message = "Perfil actualizado correctamente.";
        return View(docente);
    }

    /// <summary>
    /// Libera el contexto de base de datos.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}