using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace MySql.Parser
{
  public static class ParserUtils
  {
    public static void GetTables(ITree ct, List<TableWithAlias> tables)
    {      
      for (int i = 0; i < ct.ChildCount; i++)
      {
        ITree child = ct.GetChild(i);
        if (child.Text.Equals( "table_ref", StringComparison.OrdinalIgnoreCase ))
        {
          string alias = "";
          string table = "";
          string db = "";

          switch (child.ChildCount)
          {
            case 1:
              table = child.GetChild(0).Text;
              break;
            case 2:
              table = child.GetChild(0).Text;
              alias = child.GetChild(1).GetChild(0).Text;
              break;
            case 3:
              db = child.GetChild(0).Text;
              table = child.GetChild(2).Text;
              break;
            case 4:
              db = child.GetChild(0).Text;              
              table = child.GetChild(2).Text;
              alias = child.GetChild(3).GetChild(0).Text;
              break;
          }
          tables.Add(new TableWithAlias(db, table, alias));
          //ITree objName = child.GetChild( 0 );
          //table = objName.Text;
          //if (objName.ChildCount != 0)
          //{
          //  int j = 0;
          //  while (j < objName.ChildCount)
          //  {
          //    if (objName.GetChild(j).Text == ".")
          //    {
          //      db = table;
          //      table = objName.GetChild(j + 1).Text;
          //      j += 2;
          //    }
          //    else if (objName.GetChild(j).Text.Equals( 
          //      "alias", StringComparison.OrdinalIgnoreCase ))
          //    {
          //      alias = objName.GetChild(j).GetChild(0).Text;
          //      break;
          //    }
          //    else
          //    {
          //      j++;
          //    }
          //  }
          //}
          //tables.Add(new TableWithAlias(db, table, alias));
          //switch (child.GetChild( 0 ).ChildCount)
          //{
          //  case 1: // only table    
          //    if (child.GetChild(0).GetChild(0).Text.Equals(
          //      "alias", StringComparison.CurrentCultureIgnoreCase))
          //          alias = child.GetChild(0).GetChild(0).GetChild(0).Text;
          //    table = child.GetChild(0).Text;
          //    tables.Add(new TableWithAlias(table, alias));
          //    break;
          //  case 3:
          //    if (child.GetChild(0).GetChild(2).Text.Equals(
          //      "alias", StringComparison.CurrentCultureIgnoreCase))
          //          alias = child.GetChild(0).GetChild(2).GetChild(0).Text;
          //    db = child.GetChild(0).Text;
          //    table = child.GetChild(0).GetChild(1).Text;
          //    tables.Add(new TableWithAlias(db, table, alias));
          //    break;
          //  //case 2:
          //    //if (child.GetChild(1).Text.Equals(
          //    //  "alias", StringComparison.CurrentCultureIgnoreCase))
          //    //{ // table & alias
          //    //  tables.Add(new TableWithAlias(
          //    //    child.GetChild(0).Text, child.GetChild(1).GetChild(0).Text));
          //    //}
          //    //else
          //    //{
          //      // table & database
          //      //tables.Add(new TableWithAlias(
          //      //  child.GetChild(0).Text, child.GetChild(1).Text, alias));
          //    //}
          //    //break;
          //  //case 3: // database, table & alias
          //  //  tables.Add(new TableWithAlias(
          //  //    child.GetChild(0).Text, child.GetChild(1).Text, child.GetChild(2).GetChild(0).Text));
          //  //  break;
          //}
        }
        else GetTables(child, tables);
      }
    }
  }

  public class TableWithAlias : IEquatable<TableWithAlias>
  {
    public TableWithAlias(string TableName)
      : this("", TableName, "")
    {
    }

    public TableWithAlias(string TableName, string Alias)
      : this("", TableName, Alias)
    {
    }

    public TableWithAlias(string Database, string TableName, string Alias)
    {
      this.Database = Database.Replace("`", "").ToLower();
      this.TableName = TableName.Replace("`", "").ToLower();
      this.Alias = Alias.Replace("`", "");
    }

    public string TableName { get; private set; }
    public string Alias { get; private set; }
    public string Database { get; private set; }

    public bool Equals(TableWithAlias other)
    {
      if (other == null) return false;
      return
        (other.TableName.Equals(this.TableName, StringComparison.CurrentCultureIgnoreCase)) &&
        (other.Alias.Equals(this.Alias, StringComparison.CurrentCultureIgnoreCase)) &&
        (other.Database.Equals(this.Database, StringComparison.CurrentCultureIgnoreCase));
    }
  }
}
