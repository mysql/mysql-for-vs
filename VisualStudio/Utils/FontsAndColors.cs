using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Drawing;

namespace MySql.Data.VisualStudio.Utils
{
    internal class FontsAndColors
    {
        public static Font GetFont(string name)
        {
            MySqlDataProviderPackage package = MySqlDataProviderPackage.Instance as MySqlDataProviderPackage;
            IVsFontAndColorStorage pStorage;
            pStorage = (IVsFontAndColorStorage)package.GetVsService(
                typeof(IVsFontAndColorStorage));

            Guid textCategory = new Guid("{A27B4E24-A735-4d1d-B8E7-9716E1E3D8E0}");
            int result = pStorage.OpenCategory(ref textCategory, (int)( __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS));
            if (result == VSConstants.S_OK)
            {
                FontInfo[] fi = new FontInfo[1];
                fi[0] = new FontInfo();
                result = pStorage.GetFont(null, fi);
                pStorage.CloseCategory();
                if (result == VSConstants.S_OK)
                {
                    return new Font(fi[0].bstrFaceName, (float)fi[0].wPointSize);
                }
            }
            return null;
        }
    }
}
