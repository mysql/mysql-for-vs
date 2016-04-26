using System;
using MySqlX.Shell;
using MySqlX;
using System.Collections.Generic;
using System.Text;

namespace XShellClient_Test
{
  class Program
  {
    static void Main(string[] args)
    {
      string connString = "root:@localhost:33570";
      object result = string.Empty;
      string query = "dir(session);";

      ShellClient xShellClient = new ShellClient();

      try
      {
        xShellClient.MakeConnection(connString);
        xShellClient.SwitchMode(Mode.JScript);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return;
      }

      Console.WriteLine("Enter javascript commands(s), or enter \"quit\" to exit.");
      Console.WriteLine("");

      try
      {
        while (query != "quit")
        {
          Console.Write("mysql-js> ");
          query = Console.ReadLine();
          if (!string.IsNullOrEmpty(query) && query.ToLower() != "quit")
          {
            result = xShellClient.Execute(query);
            PrintResult(result);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return;
      }
    }

    private static void PrintResult(Object result)
    {
      Result res = result as Result;
      DocResult doc = result as DocResult;
      RowResult row = result as RowResult;
      SqlResult sql = result as SqlResult;
      if (res != null)
        PrintResult(res);
      else if (doc != null)
        PrintDocResult(doc);
      else if (row != null)
        PrintRowResult(row);
      else if (sql != null)
        PrintSqlResult(sql);
      else if (result == null)
        Console.WriteLine("null");
      else
        Console.WriteLine(result.ToString());
    }

    private static void PrintResult(Result res)
    {
      Console.WriteLine("Affected Items: {0}\n", res.GetAffectedItemCount());
      Console.WriteLine("Last Insert Id: {0}\n", res.GetLastInsertId());
      Console.WriteLine("Last Document Id: {0}\n", res.GetLastDocumentId());

      PrintBaseResult(res);
    }

    private static void PrintBaseResult(BaseResult res)
    {
      Console.WriteLine("Execution Time: {0}\n", res.GetExecutionTime());
      Console.WriteLine("Warning Count: {0}\n", res.GetWarningCount());

      if (Convert.ToUInt64(res.GetWarningCount()) > 0)
      {
        List<Dictionary<String, Object>> warnings = res.GetWarnings();

        foreach (Dictionary<String, Object> warning in warnings)
          Console.WriteLine("{0} ({1}): (2)\n", warning["Level"], warning["Code"], warning["Message"]);
      }
    }

    private static void PrintRowResult(RowResult res)
    {
      foreach (Column col in res.GetColumns())
      {
        Console.WriteLine("{0}\t", col.GetColumnName());
      }
      Console.WriteLine();

      object[] record = res.FetchOne();

      while (record != null)
      {
        foreach (object o in record)
        {
          string s = "";
          if (o == null)
            s = "null";
          else
            s = o.ToString();

          Console.WriteLine("{0}\t", s);
        }
        Console.WriteLine();

        record = res.FetchOne();
      }
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
        Console.WriteLine(sb);
      }
    }

    private static void PrintSqlResult(SqlResult res)
    {
      if ((bool)res.HasData())
        PrintRowResult(res);
      else
        PrintBaseResult(res);

      Console.WriteLine("Affected Rows: {0}\n", res.GetAffectedRowCount());
      Console.WriteLine("Last Insert Id: {0}\n", res.GetLastInsertId());
    }
  }
}
