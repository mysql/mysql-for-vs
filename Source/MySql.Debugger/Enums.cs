using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  public enum SteppingTypeEnum : int
  {
    None = 0,
    StepInto = 1,
    StepOver = 2,
    StepOut = 3
  };

  public enum RoutineInfoType : int
  {
    Procedure = 1,
    Function = 2,
    Trigger = 3
  }

  public enum RoutineType : int
  {
    Procedure = 1,
    Function = 2
  }

  public enum TriggerActionTiming : int
  {
    Before = 1,
    After = 2
  }

  public enum TriggerEvent : int
  {
    Insert = 1,
    Update = 2,
    Delete = 3
  }

  public enum ArgTypeEnum : int
  {
    In = 0,
    Out = 1,
    InOut = 2
  }

  public enum VarKindEnum : int
  {
    Local = 0,
    Argument = 1,
    Session = 2,
    Global = 3,
    Internal = 4
  }
}
