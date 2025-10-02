using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

public class AccountController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    [HttpGet]
    public ActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var docente = db.Docentes.FirstOrDefault(d => d.Correo == model.Correo && d.Password == model.Password);
        if (docente != null)
        {
            Session["DocenteId"] = docente.Id;
            Session["DocenteNombre"] = docente.Nombre;
            return RedirectToAction("Panel", "Docentes");
        }
        ModelState.AddModelError("", "Credenciales inválidas");
        return View(model);
    }

    [HttpGet]
    public ActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Register(Docente model)
    {
        if (!ModelState.IsValid) return View(model);

        if (db.Docentes.Any(d => d.Correo == model.Correo))
        {
            ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
            return View(model);
        }

        db.Docentes.Add(model);
        db.SaveChanges();
        return RedirectToAction("Login");
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}