using System;
using System.IO;
using Newtonsoft.Json;

public class LoginConfig
{
    public int MaxFailedAttempts { get; set; }
    public int LockoutMinutes { get; set; }
    public int CaptchaAfterAttempts { get; set; }
    public bool EnableCaptcha { get; set; }

    public static LoginConfig Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<LoginConfig>(json);
    }
}