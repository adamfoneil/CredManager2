using JsonSettings;
using System.Collections.Generic;
using WinForms.Library.Models;

namespace CredManager2
{
    public class Settings : JsonSettingsBase
    {
        public override Scope Scope => Scope.User;
        public override string CompanyName => "Adam O'Neil";
        public override string ProductName => "CredManager2";
        public override string Filename => "settings.json";

        public FormPosition FormPosition { get; set; }
        public string DatabaseFile { get; set; }
        public string PasswordHint { get; set; }
        public HashSet<string> Recent { get; set; }
    }
}
