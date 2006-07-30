using System;
using System.IO;
using System.Globalization;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents an implementation of data object support that returns
	/// the stream of XML containing the data object support elements.
	/// </summary>
	internal class MySqlDataObjectSupport : DataObjectSupport
	{
		public MySqlDataObjectSupport()
            : base("MySql.Data.VisualStudio.MySqlDataObjectSupport", typeof(MySqlDataObjectSupport).Assembly)
		{
            Logger.WriteLine("MySqlDataObjectSupport::ctor");
        }
	}
}
