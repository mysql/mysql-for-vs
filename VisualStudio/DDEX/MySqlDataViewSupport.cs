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
	internal class MySqlDataViewSupport : DataViewSupport
	{
		public MySqlDataViewSupport()
            : base("MySql.Data.VisualStudio.MySqlDataViewSupport", typeof(MySqlDataViewSupport).Assembly)
		{
            Logger.WriteLine("MySqlDataViewSupport::ctor");
        }
	}
}
