using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  public class MetaRoutine
  {
    public string Schema { get; set; }
    public string Name { get; set; }
    public RoutineType Type { get; set; }
    public string DataType { get; set; }
    public string RoutineDefinition { get; set; }

    public MetaRoutine()
    {
      Schema = null;
      DataType = null;
    }
  }
}
