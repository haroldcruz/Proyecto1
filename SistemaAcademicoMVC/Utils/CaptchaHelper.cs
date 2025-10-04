using System;
using System.Web;

public static class CaptchaHelper
{
    // Genera un código captcha y lo guarda en la sesión
    public static string Generate()
    {
        var rnd = new Random();
        var code = rnd.Next(1000, 9999).ToString(); // 4 dígitos
        HttpContext.Current.Session["Captcha"] = code;
        return code;
    }

    // Valida el captcha ingresado por el usuario
    public static bool Validate(string input)
    {
        var sessionCode = HttpContext.Current.Session["Captcha"];
        return sessionCode != null && input == sessionCode.ToString();
    }
}