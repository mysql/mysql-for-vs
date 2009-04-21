// Copyright (c) 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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

/*
 * This file contains data view support entity implementation.
 */

using Microsoft.VisualStudio.Data;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.Shell;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents an implementation of data view support that returns
	/// the stream of XML containing the data view support elements.
	/// </summary>
	internal class MySqlDataViewSupport : DataViewSupport
	{
        /// <summary>
        /// Constructor just passes reference to XML to base constructor.
        /// </summary>
        public MySqlDataViewSupport() : base()
		{
		}

        public override Stream GetDataViews()
        {
            string xmlName = "MySql.Data.VisualStudio.DDEX.MySqlDataViewSupport.xml";
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream stream = executingAssembly.GetManifestResourceStream(xmlName);
            StreamReader reader = new StreamReader(stream);
            string xml = reader.ReadToEnd();
            reader.Close();

            // if we are running under VS 2008 then we need to switch out a couple
            // of command handlers
/*            DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte.Version.StartsWith("9"))
                xml = xml.Replace("Microsoft.VisualStudio.DataTools.DBPackage.VDT_OLEDB_CommandHandler_TableTools",
                    "884DD964-5327-461f-9F06-6484DD540F8F");
            */
            MemoryStream ms = new MemoryStream(xml.Length);
            StreamWriter writer = new StreamWriter(ms);
            writer.Write(xml);
            writer.Flush();
            ms.Position = 0;
            return ms;
        }
	}
}
