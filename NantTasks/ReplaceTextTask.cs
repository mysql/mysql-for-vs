using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace MyTasks
{
	/// <summary>
	/// Summary description for ReplaceTextTask.
	/// </summary>
	[TaskName("replacetext")]
	public class ReplaceTextTask : Task
	{
		protected	FileSet				files;
		protected	SectionElement[]	sections;

		#region Properties

		[BuildElement("files")]
		public FileSet	fileSet 
		{
			get { return files; }
			set { files = value; }
		}

		[BuildElementArray("section", ElementType=typeof(SectionElement))]
		public SectionElement[] Sections
		{
			get { return sections; }
			set { sections = value; }
		}

		#endregion

		protected override void ExecuteTask()
		{
			foreach (SectionElement se in sections) 
				se.Load();

			foreach (string name in files.FileNames)
			{
				StreamReader sr = new StreamReader( name );
				string contents = sr.ReadToEnd();
				sr.Close();

				foreach (SectionElement se in sections) 
					contents = contents.Replace( se.From, se.To );

				File.Delete( name );
				StreamWriter sw = new StreamWriter( name );
				sw.Write( contents );
				sw.Flush();
				sw.Close();
			}
		}

	}
}
