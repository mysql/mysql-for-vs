using Microsoft.VisualStudio.Package;

namespace MySql.Data.VisualStudio
{
    /// <summary>
    /// We don't actually need this class but we have to return a scanner object
    /// from our language service or the colorizing doesn't work.
    /// All the colorizing work happens in MySqlColorizer.
    /// </summary>
    class MySqlScanner : IScanner
    {
        #region IScanner Members

        bool IScanner.ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            return false;
        }

        void IScanner.SetSource(string source, int offset)
        {
        }

        #endregion
    }
}
