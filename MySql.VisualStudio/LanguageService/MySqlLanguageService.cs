using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// This language service will provide language
    /// </summary>
    [ComVisible(true)]
    [Guid("FA498A2D-116A-4f25-9B55-7938E8E6DDA7")]
    class MySqlLanguageService : LanguageService
    {
        private LanguagePreferences preferences;

        public const string LanguageName = "MySQL";

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }

        public override string Name
        {
            get { return LanguageName; }
        }

        public override string GetFormatFilterList()
        {
            return String.Empty;
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return new MySqlScanner();
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (preferences == null)
            {
                preferences = new LanguagePreferences(this.Site,
                    typeof(MySqlLanguageService).GUID, LanguageName);
                preferences.Init();
            }
            return preferences;
        }
    }
}
