// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

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
