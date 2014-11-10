// Copyright © 2014 Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Debug;
using Antlr.Runtime.Tree;
using MySqlParser;

namespace GrammarDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			int port = 49101;
			string sql = "select * from t1 left join t2";
			MemoryStream ms = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(sql.ToUpper()));
			ANTLRInputStream input = new ANTLRInputStream(ms);
			MySQL51Lexer lexer = new MySQL51Lexer(input);
			// I need a Tree adaptor to build a DebugEventSocketProxy, but I need a 
			// DebugEventSocketProxy to build a Tree Adaptor.
			// Solution: Create the DebugEventSocketProxy 
			//ITreeAdaptor adaptor = new DebugTreeAdaptor(/*dbg*/ null, new CommonTreeAdaptor());
			// To create a DebugTokenStream I need a DebugEventSocketProxy and viceversa
			// Solution: Create DebugEventSocketProxy in the DebugTokenStream contructor
			// How do I get a ITokenStream implementation?
			// Another Caveat: The instance of DebugEventProxySocket must be the same for the lexer than for the parser.

			//DebugEventSocketProxy proxy = new DebugEventSocketProxy(this, port, adaptor);
			DebugTokenStream tokens = new DebugTokenStream( new BufferedTokenStream( lexer ), port, null);
			//CommonTokenStream tokens = new CommonTokenStream(lexer);
			MySQL51Parser parser = new MySQL51Parser(tokens, port, null);
			StringBuilder sb = new StringBuilder();
			TextWriter tw = new StringWriter(sb);
			try
			{
				parser.TraceDestination = tw;
				MySQL51Parser.statement_list_return r = parser.statement_list();
			}
			catch (RecognitionException re)
			{
				Console.WriteLine(re.StackTrace);
			}
		}
	}
}
