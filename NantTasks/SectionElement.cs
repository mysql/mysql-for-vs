using System;
using System.Text;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace MyTasks
{
	/// <summary>
	/// Summary description for Section.
	/// </summary>
	public class SectionElement : Element
	{
		protected string	to;
		protected string	from;
		protected string	fromFile;
		protected string	toFile;

		[TaskAttribute("from")]
		public string From 
		{
			get { return from; }
			set { from = value; }
		}

		[TaskAttribute("to")]
		public string To 
		{
			get { return to; }
			set { to = value; }
		}

		[TaskAttribute("fromfile")]
		public string FromFile 
		{
			get { return fromFile; }
			set { fromFile = value; }
		}

		[TaskAttribute("tofile")]
		public string ToFile
		{
			get { return toFile; }
			set { toFile = value; }
		}

		internal void Load() 
		{
			if (From != null && From.Length > 0 && To != null && To.Length > 0) return;
			
			if (From == null || From.Length == 0)
				From = LoadFile( FromFile );

			if (To == null || To.Length == 0)
				To = LoadFile( ToFile );

			if (From == null || From.Length == 0)
				throw new Exception("Either FROM or FROMFILE must be valid");
			if (To == null || To.Length == 0)
				throw new Exception("Either TO or TOFILE must be valid");
		}

		private string LoadFile(string file) 
		{
			StreamReader sr = new StreamReader(file);
			StringBuilder sb = new StringBuilder();
			string line = sr.ReadLine();
			while (line != null) 
			{
				sb.Append(line);
				sb.Append(Environment.NewLine);
				line = sr.ReadLine();
			}
			string contents = sb.ToString();
			sr.Close();
			return contents;
		}
	}
}
