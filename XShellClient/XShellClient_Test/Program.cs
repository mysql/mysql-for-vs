using System;
using MySqlX.Shell;
using MySqlX;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ConsoleTables.Core;

namespace XShellClient_Test
{
  class Program
  {
    private const string MySqlXResultType = "mysqlx.result";
    private const string MySqlXDocResultType = "mysqlx.docresult";
    private const string MySqlXRowResultType = "mysqlx.rowresult";
    private const string MySqlXSqlResultType = "mysqlx.sqlresult";
    private const string SystemStringType = "system.string";

    static void Main(string[] args)
    {
      string connString = "root:@localhost:33570";
      object result = string.Empty;
      string query = "dir(session);";
      Mode mode = Mode.JScript;
      ShellClient xShellClient = new ShellClient();

      try
      {
        xShellClient.MakeConnection(connString);
        xShellClient.SwitchMode(mode);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return;
      }

      Console.WriteLine(Resources.InstructionsMessages);
      Console.WriteLine("");

      try
      {
        while (query != "quit")
        {
          if (mode == Mode.JScript)
          {
            Console.Write("mysql-js> ");
          }
          else
          {
            Console.Write("mysql-py> ");
          }
          query = Console.ReadLine();
          if (!string.IsNullOrEmpty(query) && query.ToLower() != "quit")
          {
            if (query == "\\py")
            {
              mode = Mode.Python;
              xShellClient.SwitchMode(mode);
            }
            else if (query == "\\js")
            {
              mode = Mode.JScript;
              xShellClient.SwitchMode(mode);
            }
            else
            {
              result = xShellClient.Execute(query);
              PrintResult(result);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private static void PrintResult(Object result)
    {
      string type = result.GetType().ToString().ToLowerInvariant();
      switch (type)
      {
        case MySqlXResultType:
          PrintResult((Result)result);
          break;
        case MySqlXDocResultType:
          PrintDocResult((DocResult)result);
          break;
        case MySqlXRowResultType:
          PrintRowResult((RowResult)result);
          break;
        case MySqlXSqlResultType:
          PrintSqlResult((SqlResult)result);
          break;
        case SystemStringType:
          if (string.IsNullOrEmpty(result.ToString()))
          {
            return;
          }

          Console.WriteLine(result.ToString());
          break;
      }
    }

    private static void PrintResult(Result result)
    {
      StringBuilder resultMessage = new StringBuilder();
      resultMessage.Append("Query OK");
      long affectedItems = result.GetAffectedItemCount();
      if (affectedItems >= 0)
      {
        resultMessage.AppendFormat(", {0} items affected", affectedItems);
      }

      Console.WriteLine("{0} ({1})", resultMessage.ToString(), result.GetExecutionTime());
      PrintWarnings(result);
    }

    private static void PrintWarnings(BaseResult result)
    {
      if (result.GetWarningCount() > 0)
      {
        StringBuilder warningsMessages = new StringBuilder();
        warningsMessages.AppendFormat(" Warning Count: {0}\n", result.GetWarningCount());
        List<Dictionary<String, Object>> warnings = result.GetWarnings();
        foreach (Dictionary<String, Object> warning in warnings)
        {
          warningsMessages.AppendFormat("{0} ({1}): {2}\n", warning["Level"], warning["Code"], warning["Message"]);
        }

        Console.WriteLine("{0} ({1})", warningsMessages.ToString(), result.GetExecutionTime());
      }
    }

    private static void PrintRowResult(RowResult res, long affectedItems = 0)
    {
      // Get columns names
      string[] columns = new string[res.GetColumns().Count];
      int i = 0;
      foreach (Column col in res.GetColumns())
      {
        columns[i++] += col.GetColumnName();
      }

      // Create console table object for output format
      var table = new ConsoleTable(columns);
      object[] record = res.FetchOne();
      while (record != null)
      {
        object[] columnValue = new object[res.GetColumns().Count];
        i = 0;
        foreach (object o in record)
        {
          if (o == null)
          {
            columnValue[i++] = "null";
          }
          else
          {
            columnValue[i++] = o.ToString();
          }
        }

        table.AddRow(columnValue);
        record = res.FetchOne();
      }

      if (table.Rows.Count > 0)
      {
        Console.WriteLine(table.ToStringAlternative());
      }

      StringBuilder resultMessage = new StringBuilder();
      // If no items are returned, it is a DDL statement (Drop, Create, etc.)
      int totalItems = res.FetchAll().Count;
      if (totalItems == 0)
      {
        resultMessage.Append("Query OK");
        if (affectedItems >= 0)
        {
          resultMessage.AppendFormat(", {0} rows affected", affectedItems);
        }

        resultMessage.AppendFormat(", {0} warning(s)", res.GetWarningCount());
      }
      else
      {
        resultMessage.AppendFormat("{0} rows in set.", totalItems);
      }

      Console.WriteLine("{0} ({1})", resultMessage.ToString(), res.GetExecutionTime());
      PrintWarnings(res);
    }

    private static void PrintDocResult(DocResult doc)
    {
      List<Dictionary<string, object>> data = doc.FetchAll();
      foreach (Dictionary<string, object> row in data)
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        int i = 0;
        foreach (KeyValuePair<string, object> kvp in row)
        {
          sb.AppendFormat("\t\"{0}\" : \"{1}\"{2}\n", kvp.Key, kvp.Value, (i == row.Count - 1) ? "" : ",");
          i++;
        }

        sb.AppendLine("},");
        Console.WriteLine(sb.ToString());

        Console.WriteLine("{0} documents in set. ({1})", doc.FetchAll().Count, doc.GetExecutionTime());
        PrintWarnings(doc);
      }
    }

    private static void PrintSqlResult(SqlResult res)
    {
      if ((bool)res.HasData())
      {
        PrintRowResult(res, res.GetAffectedRowCount());
      }
      else
      {
        PrintWarnings(res);
      }
    }
  }
}