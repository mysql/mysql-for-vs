using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Debugger
{
  public class MetaTrigger : IEquatable<MetaTrigger>
  {
    public string TriggerSchema { get; set; }
    public string Name { get; set; }
    public TriggerEvent Event { get; set; }
    public string ObjectSchema { get; set; }
    public string Table { get; set; }
    public string Source { get; set; }
    public TriggerActionTiming ActionTiming { get; set; }

    public bool Equals(MetaTrigger other)
    {
      if (other == null) return false;
      /* There cannot be two triggers for the same action & timing over a given table. */
      return
        (other.ObjectSchema.Equals(this.ObjectSchema, StringComparison.CurrentCultureIgnoreCase)) &&
        (other.Table.Equals(this.Table, StringComparison.InvariantCulture)) &&
        (other.ActionTiming == this.ActionTiming) &&
        (other.Event == this.Event);
    }

    public MetaTrigger()
    {
    }
  }
}
