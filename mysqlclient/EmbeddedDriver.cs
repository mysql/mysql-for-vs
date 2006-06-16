using System;

namespace MySql.Data.MySqlClient
{
#if !CF

	/// <summary>
	/// Summary description for EmbeddedDriver.
	/// </summary>
	internal class EmbeddedDriver : ClientDriver
	{
		string[] options;

		public EmbeddedDriver(MySqlConnectionStringBuilder settings) : base(settings)
		{
			options = new string[2];
		}

		public override void Open()
		{
			options[0] = "dummy";
			options[1] = "--defaults-file=" + connectionString.OptionFile;

			int result = EmbeddedAPI.ServerInit(2, options, null);
			if (result == 1) 
			{
				string msg = EmbeddedAPI.ErrorMsg(IntPtr.Zero);
				throw new MySqlException("The embedded library failed to initialize");
			}

			base.Open();
		}

		public override void Close()
		{
			base.Close();
			EmbeddedAPI.ServerEnd();
		}


	#region Interface methods

		protected override IntPtr Init(IntPtr mysql) 
		{
			return EmbeddedAPI.Init(mysql);
		}

		protected override IntPtr Connect(IntPtr mysql, string host, string user,
		string password, string db, uint port, string unix_socket, uint flag)
		{
			return EmbeddedAPI.Connect(mysql, host, user, password, db, port, unix_socket, flag);
		}

		protected override int SetOptions(IntPtr mysql, ClientAPIOption option, ref object optionValue)
		{
			return EmbeddedAPI.SetOptions(mysql, option, ref optionValue);
		}

		protected override void Close(IntPtr mysql) 
		{
			EmbeddedAPI.Close(mysql);
		}

		protected override int SelectDatabase(IntPtr mysql, string dbName) 
		{
			return EmbeddedAPI.SelectDatabase(mysql, dbName);
		}

		protected override int Query(IntPtr mysql, byte[] query, uint len) 
		{
			return EmbeddedAPI.Query(mysql, query, len);
		}

		protected override string ErrorMsg(IntPtr mysql)
		{
			return EmbeddedAPI.ErrorMsg(mysql);
		}

		protected override int ErrorNumber(IntPtr mysql) 
		{
			return EmbeddedAPI.ErrorNumber(mysql);
		}

		protected override IntPtr UseResult(IntPtr mysql)
		{
			return EmbeddedAPI.UseResult(mysql);
		}

		protected override bool MoreResults(IntPtr mysql)
		{
			return EmbeddedAPI.MoreResults(mysql);
		}

		protected override int NextResult(IntPtr mysql)
		{
			return EmbeddedAPI.NextResult(mysql);
		}

		protected override void FreeResult(IntPtr resultSet) 
		{
			EmbeddedAPI.FreeResult(resultSet);
		}

		protected override int GetFieldCount(IntPtr resultSet) 
		{
			return EmbeddedAPI.FieldCount(resultSet);
		}

		protected override ulong AffectedRows(IntPtr mysql)
		{
			return EmbeddedAPI.AffectedRows(mysql);
		}

		protected override ulong LastInsertId(IntPtr mysql)
		{
			return EmbeddedAPI.LastInsertId(mysql);
		}

		protected override ClientField FetchField(IntPtr resultSet)
		{
			return EmbeddedAPI.FetchField(resultSet);
		}

		protected override IntPtr FetchRow(IntPtr resultSet)
		{
			return EmbeddedAPI.FetchRow(resultSet);
		}
			
		protected override IntPtr FetchLengths(IntPtr resultSet)
		{
			return EmbeddedAPI.FetchLengths(resultSet);
		}

		protected override int Ping(IntPtr mysql)
		{
			return EmbeddedAPI.Ping(mysql);
		}

		protected override string VersionString(IntPtr mysql)
		{
			return EmbeddedAPI.VersionString(mysql);
		}

		protected override string CharacterSetName(IntPtr mysql)
		{
			return EmbeddedAPI.CharacterSetName(mysql);
		}

	}

#endregion

#endif
}
