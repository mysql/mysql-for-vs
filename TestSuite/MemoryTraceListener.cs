using System;
using System.Collections;
using System.Diagnostics;

namespace MySql.Data.MySqlClient.Tests
{
	/// <summary>
	/// Summary description for MemoryTraceListener.
	/// </summary>
	public class MemoryTraceListener : TraceListener
	{
		private ArrayList	lines;
		private bool		partial;

		public MemoryTraceListener()
		{
			lines = new ArrayList();
		}

		public override void Write(string message)
		{
			if (lines.Count == 0)
				lines.Add( message );
			else
				lines[lines.Count-1] += message;
			partial = true;
		}

		public override void WriteLine(string message)
		{
			if (partial)
				lines[lines.Count-1] += message;
			else
				lines.Add( message );
			partial = false;
		}

		public string ReadLine() 
		{
			if (lines.Count == 0) return null;

			string s = (string)lines[0];
			lines.RemoveAt(0);
			return s;
		}

		public void Clear() 
		{
			lines.Clear();
		}
	}
}
