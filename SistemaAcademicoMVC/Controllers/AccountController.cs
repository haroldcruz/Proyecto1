using System.Linq;
using System.Web.Mvc;
using SistemaAcademicoMVC.Models;
using System;

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
    public ActionResult Login()
    {
        ViewBag.ShowCaptcha = false;
        return View();
    }

    /// <summary>
    /// Procesa el login y muestra mensajes de éxito o error.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel model)
    {
        // Carga la configuración desde JSON
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

        // Verifica si el usuario está bloqueado
        if (docente.LockoutEnd.HasValue && docente.LockoutEnd.Value > DateTime.Now)
        {
            var minutosRestantes = (docente.LockoutEnd.Value - DateTime.Now).TotalMinutes;
            ModelState.AddModelError("", $"Cuenta bloqueada. Intente nuevamente en {Math.Ceiling(minutosRestantes)} minutos.");
            ViewBag.ShowCaptcha = false;
            return View(model);
        }

        // ¿Mostrar captcha?
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
            // Login exitoso: resetea los intentos fallidos y desbloquea
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
            // Solo sumar intentos si no está bloqueado
            if (docente.LockoutEnd == null || docente.LockoutEnd.Value <= DateTime.Now)
            {
                docente.FailedLoginAttempts += 1;

                // Si excede el máximo, bloquear
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
                // Ya está bloqueado, no sumar intentos
                var minutosRestantes = (docente.LockoutEnd.Value - DateTime.Now).TotalMinutes;
                ModelState.AddModelError("", $"Cuenta bloqueada. Intente nuevamente en {Math.Ceiling(minutosRestantes)} minutos.");
            }

            ViewBag.ShowCaptcha = showCaptcha;
            return View(model);
        }
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
            return View(model);
        }

        // Verifica si el correo ya está registrado
        if (db.Docentes.Any(d => d.Correo == model.Correo))
        {
            ModelState.AddModelError("Correo", "Ya existe un docente con ese correo.");
            return View(model);
        }

        model.Password = PasswordHelper.HashPassword(model.Password);
        db.Docentes.Add(model);
        db.SaveChanges();

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

    /// <summary>
    /// Genera la imagen del captcha.
    /// </summary>
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