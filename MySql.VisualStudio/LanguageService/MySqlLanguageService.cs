using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

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
        private MySqlScanner scanner;

        public const string LanguageName = "MySQL";

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            throw new System.NotImplementedException();
        }

        public override string Name
        {
            get { return LanguageName; }
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (scanner == null)
                scanner = new MySqlScanner();
            return scanner;
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

        /// <summary>
        /// We override this method because we are using an Antlr grammer and 
        /// need to lex the entire buffer before returning our color info
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public override Colorizer GetColorizer(IVsTextLines buffer)
        {
            return new MySqlColorizer(this, buffer, new MySqlScanner());
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            MySqlSource src = new MySqlSource(this, buffer, GetColorizer(buffer));
            return src;
        }
    }
}
