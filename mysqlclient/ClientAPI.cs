// Copyright (C) 2004 MySQL AB
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Runtime.InteropServices;

namespace MySql.Data.MySqlClient
{
	internal enum ClientAPIOption
	{
		MYSQL_OPT_CONNECT_TIMEOUT, 
		MYSQL_OPT_COMPRESS, 
		MYSQL_OPT_NAMED_PIPE,
		MYSQL_INIT_COMMAND, 
		MYSQL_READ_DEFAULT_FILE, 
		MYSQL_READ_DEFAULT_GROUP,
		MYSQL_SET_CHARSET_DIR, 
		MYSQL_SET_CHARSET_NAME, 
		MYSQL_OPT_LOCAL_INFILE,
		MYSQL_OPT_PROTOCOL, 
		MYSQL_SHARED_MEMORY_BASE_NAME, 
		MYSQL_OPT_READ_TIMEOUT,
		MYSQL_OPT_WRITE_TIMEOUT, 
		MYSQL_OPT_USE_RESULT,
		MYSQL_OPT_USE_REMOTE_CONNECTION, 
		MYSQL_OPT_USE_EMBEDDED_CONNECTION,
		MYSQL_OPT_GUESS_CONNECTION, 
		MYSQL_SET_CLIENT_IP, 
		MYSQL_SECURE_AUTH
	};

	[StructLayout(LayoutKind.Sequential)]
	internal class ClientField
	{
		public string		name;				// Name of column 
		public string		org_name;			// Original column name, if an alias 
		public string		table;				// Table of column if column was a field 
		public string		org_table;			// Org table name, if table was an alias 
		public string		db;					// Database for table 
		public string		catalog;			// Catalog for table 
		public string		def;				// Default value (set by mysql_list_fields) 
		public uint			length;				// Width of column (create length) 
		public uint			max_length;			// Max width for selected set
		public uint			name_length;
		public uint			org_name_length;
		public uint			table_length;
		public uint			org_table_length;
		public uint			db_length;
		public uint			catalog_length;
		public uint			def_length;
		public uint			flags;				// Div flags 
		public uint			decimals;			// Number of decimals in field
		public uint			charset;			// Character set 
		public MySqlDbType	type;				// Type of field. See mysql_com.h for types
	}


	/// <summary>
	/// Summary description for ClientAPI.
	/// </summary>
	internal class ClientAPI
	{
		[DllImport("libmysql.dll", EntryPoint="mysql_init")]
		public static extern IntPtr Init(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_real_connect")]
		public static extern IntPtr Connect( IntPtr mysql,
			string host, string user, string password, string db, uint port,
			string unix_socket, uint flag );

		[DllImport("libmysql.dll", EntryPoint="mysql_close")]
		public static extern void Close(IntPtr mysql);
		
		[DllImport("libmysql.dll", EntryPoint="mysql_ping")]
		public static extern int Ping(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_select_db")]
		public static extern int SelectDatabase(IntPtr mysql, string dbName);

		[DllImport("libmysql.dll", EntryPoint="mysql_real_query")]
		public static extern int Query(IntPtr mysql, byte[] query, uint len);

		[DllImport("libmysql.dll", EntryPoint="mysql_error")]
		public static extern string ErrorMsg(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_errno")]
		public static extern int ErrorNumber(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_options")]
		public static extern int SetOptions(IntPtr mysql, ClientAPIOption option, ref object optionValue);

		[DllImport("libmysql.dll", EntryPoint="mysql_use_result")]
		public static extern IntPtr UseResult(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_more_results")]
		public static extern bool MoreResults(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_next_result")]
		public static extern int NextResult(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_free_result")]
		public static extern void FreeResult(IntPtr result);

		[DllImport("libmysql.dll", EntryPoint="mysql_field_count")]
		public static extern int FieldCount(IntPtr resultSet);

		[DllImport("libmysql.dll", EntryPoint="mysql_affected_rows")]
		public static extern ulong AffectedRows(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_insert_id")]
		public static extern ulong LastInsertId(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_fetch_field")]
		public static extern ClientField FetchField(IntPtr resultSet);

		[DllImport("libmysql.dll", EntryPoint="mysql_fetch_row")]
		public static extern IntPtr FetchRow(IntPtr resultSet);
		
		[DllImport("libmysql.dll", EntryPoint="mysql_fetch_lengths")]
		public static extern IntPtr FetchLengths(IntPtr resultSet);

		[DllImport("libmysql.dll", EntryPoint="mysql_get_server_info")]
		public static extern string VersionString(IntPtr mysql);

		[DllImport("libmysql.dll", EntryPoint="mysql_character_set_name")]
		public static extern string CharacterSetName(IntPtr mysql);
	}
}
