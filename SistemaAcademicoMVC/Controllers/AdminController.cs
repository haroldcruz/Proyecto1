using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;

public class AdminController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    // Acción para mostrar la vista de administración de docentes
    public ActionResult Administrador()
    {
        var docentes = db.Docentes.ToList();
        return View(docentes);
    }
}