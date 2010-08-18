// Copyright (c) 2004-2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Diagnostics;

namespace MySql.Data.MySqlClient
{
	/// <summary>
	/// Provides methods to output messages to our log
	/// </summary>
	internal class Logger
	{
		// private ctor
		private Logger() 
		{
		}

		static public void LogCommand( DBCmd cmd, string text)
		{
			string msg = String.Format("Executing command {0} with text ='{1}'", cmd, text);
			//TODO: check this
				//Enum.GetName( typeof(DBCmd), cmd ), text );
			WriteLine( msg );
		}

        static public void LogInformation(string msg)
        {
#if !CF
            Trace.WriteLine(msg);
#endif
        }

		static public void LogException(Exception ex)
		{
			string msg = String.Format("EXCEPTION: " + ex.Message);
			WriteLine(msg);
		}

		static public void LogWarning(string s)
		{
			WriteLine("WARNING:" + s);
		}

		static public void Write(string s) 
		{
#if !CF
			Trace.Write(s);
#endif
		}

		static public void WriteLine(string s) 
		{
#if !CF
            Trace.WriteLine(String.Format("[{0}] - {1}",
                DateTime.Now, s));
#endif
		}
	}
}
