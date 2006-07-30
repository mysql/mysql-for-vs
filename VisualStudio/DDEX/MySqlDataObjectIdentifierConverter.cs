using System;
using System.Diagnostics;
using System.Collections;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace MySql.Data.VisualStudio
{
	/// <summary>
	/// Represents a custom data object identifier converter that correctly
	/// parses and formats SQL Server object identifiers.
	/// </summary>
	internal class MySqlDataObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
	{
		public MySqlDataObjectIdentifierConverter(DataConnection connection) : base(connection)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierConverter::ctor");
            Debug.Assert(connection != null);
			this._connection = connection;
		}

		/// <summary>
		/// This implements correct parsing of a string identifier into parts.
		/// </summary>
		protected override string[] SplitIntoParts(string typeName, string identifier)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierConverter::SplitIntoParts");
            if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			
			// Split the string around '.', except when appearing inside an identifier part
			string[] arrIdentifier = null;
			int length = MySqlDataObjectIdentifierResolver.GetIdentifierLength(typeName);
			if (length == -1)
			{
				throw new NotSupportedException();
			}
			arrIdentifier = new string[length];
			if (identifier != null)
			{
				int arrIndex = 0;
				int startIndex = 0;
				int endIndex = 0;
				char quote = '\0';
				while (endIndex < identifier.Length)
				{
					if (identifier[endIndex] == '[' && quote == '\0')
					{
						// We entered a quoted identifier part using '['
						quote = ']';
					}
					else if (identifier[endIndex] == ']' && quote == ']')
					{
						if (endIndex < identifier.Length - 1 && identifier[endIndex + 1] == ']')
						{
							// We encountered an embedded quote in a quoted identifier part; skip it
							endIndex++;
						}
						else
						{
							// We left a quoted identifier part using ']'
							quote = '\0';
						}
					}
					else if (identifier[endIndex] == '"' && quote == '\0')
					{
						// We entered a quoted identifier part using '"'
						quote = '"';
					}
					else if (identifier[endIndex] == '"' && quote == '"')
					{
						if (endIndex < identifier.Length - 1 && identifier[endIndex + 1] == '"')
						{
							// We encountered an embedded quote in a quoted identifier part; skip it
							endIndex++;
						}
						else
						{
							// We left a quoted identifier part using '"'
							quote = '\0';
						}
					}
					else if (identifier[endIndex] == '.' && quote == '\0')
					{
						// We encountered a separator outside of a quoted identifier part
						if (arrIndex == arrIdentifier.Length)
						{
							throw new FormatException();
						}
						arrIdentifier[arrIndex] = identifier.Substring(startIndex, endIndex - startIndex);
						arrIndex++;
						startIndex = endIndex + 1;
					}
					endIndex++;
				}
				if (identifier.Length > 0)
				{
					if (arrIndex == arrIdentifier.Length)
					{
						throw new FormatException();
					}
					arrIdentifier[arrIndex] = identifier.Substring(startIndex);
				}
			}

			// Shift the elements in the array so they are right aligned
			int shiftCount = 0;
			for (int i = arrIdentifier.Length - 1; i >= 0; i--)
			{
				if (arrIdentifier[i] != null)
				{
					break;
				}
				shiftCount++;
			}
			string[] tempArray = arrIdentifier;
			arrIdentifier = new string[tempArray.Length];
			Array.Copy(tempArray, 0, arrIdentifier, shiftCount, arrIdentifier.Length - shiftCount);

			return arrIdentifier;
		}

		/// <summary>
		/// This method removes quotes from an identifier part, and unescapes
		/// the string.
		/// </summary>
		protected override object UnformatPart(string typeName, string identifierPart)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierConverter::UnformatPart");
            if (identifierPart == null)
			{
				return null;
			}

			string part = identifierPart.Trim();
			if (part.StartsWith("["))
			{
				if (!part.EndsWith("]"))
				{
					throw new FormatException();
				}
				return part.Substring(1, part.Length - 2).Replace("]]", "]");
			}
			else if (part.StartsWith("\""))
			{
				if (!part.EndsWith("\""))
				{
					throw new FormatException();
				}
				return part.Substring(1, part.Length - 2).Replace("\"\"", "\"");
			}
			else
			{
				return part;
			}
		}

		/// <summary>
		/// This method adds quotes to an identifier part, if necessary, and
		/// escapes the quote character in the string.
		/// </summary>
		protected override string FormatPart(string typeName, object identifierPart, bool withQuotes)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierConverter::FormatPart");
            if (identifierPart == null)
			{
				return String.Empty;
			}
			else if (withQuotes && MustQuote(identifierPart.ToString()))
			{
				return ("[" + identifierPart.ToString().Replace("]", "]]") + "]");
			}
			else
			{
				return identifierPart.ToString();
			}
		}

		/// <summary>
		/// SQL Server has strict rules defined for when an identifier must
		/// be quoted.  This method simulates the server behavior to ensure
		/// all identifiers are quoted correctly on the client side.
		/// </summary>
		private bool MustQuote(string s)
		{
            Logger.WriteLine("MySqlDataObjectIdentifierConverter::MustQuote");
            // If string does not follow rules for regular (unquoted) identifier,
			// then it must be quoted.

			// 0) If empty string, does not need to be quoted
			if (s.Length == 0)
			{
				return false;
			}

			// 1) First character must be Unicode 2.0 letter, '_', '@' or '#'
			//    If one of the latter three characters, quote since they have special usage
			if (!Char.IsLetter(s[0]) || s[0] == '_' || s[0] == '@' || s[0] == '#')
			{
				return true;
			}

			// 2) Subsequent characters can be Unicode 2.0 letters, decimal numbers, '@', '$', '#' or '_'
			for (int i = 1; i < s.Length; i++)
			{
				if (!(Char.IsLetterOrDigit(s[i]) || s[i] == '@' || s[i] == '$' || s[i] == '#' || s[i] == '_'))
				{
					return true;
				}
			}

			// 3) Cannot be a Transact-SQL reserved word (either upper or lower case)
			if (_reservedWords == null)
			{
				_reservedWords = _connection.SourceInformation[DataSourceInformation.ReservedWords].ToString().Split(',');
			}
			foreach (string reservedWord in _reservedWords)
			{
				if (s.Equals(reservedWord, StringComparison.InvariantCultureIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

		private string[] _reservedWords = null;
		private DataConnection _connection;
	}
}