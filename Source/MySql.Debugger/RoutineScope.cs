using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  /// <summary>
  /// The scope of a routine, if recursivity was enabled, there could be many RoutineScopes for a single Routine.
  /// </summary>
  public class RoutineScope
  {
    /// <summary>
    /// Reference to Routine Metadata.
    /// </summary>
    public RoutineInfo OwningRoutine;

    /// <summary>
    /// A reference to a filename, in case the routine belongs to one.
    /// </summary>
    /// <remarks>This is of utility to debugger's clients, not for the core debugger itself.</remarks>
    public string FileName { get; set; }

    /// <summary>
    /// The dictionary of variables for the given scope (stack frame).
    /// </summary>
    public Dictionary<string, StoreType> Variables;
  }
}
