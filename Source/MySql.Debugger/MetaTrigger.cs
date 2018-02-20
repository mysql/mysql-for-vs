// Copyright (c) 2004, 2014, Oracle and/or its affiliates. All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

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
        (other.Event == this.Event) &&
        (other.TriggerSchema == this.TriggerSchema) &&
        (other.Name == this.Name);
    }

    public MetaTrigger()
    {
    }
  }
}
