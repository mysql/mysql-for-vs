using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  /// <summary>
  /// A debugger watch expression 
  /// </summary>
  public class Watch
  {
    public Debugger Debugger { get; set; }
    public string Expression { get; set; }

    public delegate void ValueChangedHandler(Watch w);

    public event ValueChangedHandler OnValueChanged;

    internal void RaiseValueChanged(Watch w)
    {
      if (OnValueChanged != null)
        OnValueChanged(w);
    }

    private object _value = null;
    public bool ValueChanged { get; set; }

    public object Eval()
    {
      // TODO: Watches are parsed each time they are evaluated
      // can this be optimized (ie. parse only in new Watch()? ).
      object newValue = Debugger.Eval(Expression);
      if (_value != newValue)
      {
        ValueChanged = true;
        _value = newValue;
        RaiseValueChanged(this);
      }
      return newValue;
    }
  }
}
