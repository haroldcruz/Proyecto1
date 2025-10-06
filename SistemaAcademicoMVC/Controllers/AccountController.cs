using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;
using System;

/// <summary>
/// Controlador de autenticación de docentes.
/// Gestiona login y cierre de sesión.
/// </summary>
public class AccountController : Controller
{
    private SistemaAcademicoMVCContext db = new SistemaAcademicoMVCContext();

    [HttpGet]
    public ActionResult Login()
    {
        ViewBag.ShowCaptcha = false;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        var config = LoginConfig.Load(Server.MapPath("~/App_Data/loginConfig.json"));

        if (!ModelState.IsValid)
        {
            ViewBag.ShowCaptcha = false;
            return View(model);
        }

        var docente = db.Docentes.FirstOrDefault(d => d.Correo == model.Correo);
        bool showCaptcha = false;

        if (docente == null)
        {
            ModelState.AddModelError("", "Credenciales inválidas");
            ViewBag.ShowCaptcha = false;
            return View(model);
        }

        if (docente.LockoutEnd.HasValue && docente.LockoutEnd.Value > DateTime.Now)
        {
            var minutosRestantes = (docente.LockoutEnd.Value - DateTime.Now).TotalMinutes;
            ModelState.AddModelError("", $"Cuenta bloqueada. Intente nuevamente en {Math.Ceiling(minutosRestantes)} minutos.");
            ViewBag.ShowCaptcha = false;
            return View(model);
        }

        if (config.EnableCaptcha && docente.FailedLoginAttempts >= config.CaptchaAfterAttempts)
        {
            showCaptcha = true;
            ViewBag.ShowCaptcha = true;

            var captchaInput = Request["Captcha"];
            if (string.IsNullOrEmpty(captchaInput) || !CaptchaHelper.Validate(captchaInput))
            {
                ModelState.AddModelError("Captcha", "El código ingresado es incorrecto.");
                return View(model);
            }
        }
        else
        {
            ViewBag.ShowCaptcha = false;
        }

        string hash = PasswordHelper.HashPassword(model.Password);

        if (docente.Password == hash)
        {
            docente.FailedLoginAttempts = 0;
            docente.LockoutEnd = null;
            db.SaveChanges();

            Session["DocenteId"] = docente.Id;
            Session["DocenteNombre"] = docente.Nombre;
            Session["DocenteCorreo"] = docente.Correo;
            TempData["SuccessMessage"] = "¡Bienvenido, " + docente.Nombre + "!";
            return RedirectToAction("Index", "Home");
        }
        else
        {
            if (docente.LockoutEnd == null || docente.LockoutEnd.Value <= DateTime.Now)
            {
                docente.FailedLoginAttempts += 1;

                if (docente.FailedLoginAttempts >= config.MaxFailedAttempts)
                {
                    docente.LockoutEnd = DateTime.Now.AddMinutes(config.LockoutMinutes);
                    ModelState.AddModelError("", $"Cuenta bloqueada por {config.LockoutMinutes} minutos.");
                }
                else
                {
                    ModelState.AddModelError("", "Credenciales inválidas");
                }
                db.SaveChanges();
            }
            else
            {
                var minutosRestantes = (docente.LockoutEnd.Value - DateTime.Now).TotalMinutes;
                ModelState.AddModelError("", $"Cuenta bloqueada. Intente nuevamente en {Math.Ceiling(minutosRestantes)} minutos.");
            }

            ViewBag.ShowCaptcha = showCaptcha;
            return View(model);
        }
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    public FileResult GetCaptchaImage()
    {
        string code = CaptchaHelper.Generate();

        using (var bmp = new System.Drawing.Bitmap(100, 40))
        using (var gfx = System.Drawing.Graphics.FromImage(bmp))
        using (var font = new System.Drawing.Font("Arial", 20, System.Drawing.FontStyle.Bold))
        using (var ms = new System.IO.MemoryStream())
        {
            gfx.Clear(System.Drawing.Color.LightGray);
            gfx.DrawString(code, font, System.Drawing.Brushes.Black, 10, 5);

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return File(ms.ToArray(), "image/png");
        }
    }
}