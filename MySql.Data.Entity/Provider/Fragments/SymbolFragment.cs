// Copyright (C) 2008-2009 Sun Microsystems, Inc.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 2 as published by
// the Free Software Foundation
//
// There are special exceptions to the terms and conditions of the GPL 
// as it is applied to this software. View the full text of the 
// exception in file EXCEPTIONS in the directory of this software 
// distribution.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Metadata.Edm;

namespace MySql.Data.Entity
{
    class SymbolFragment : SqlFragment
    {
        public SymbolFragment() : base()
        {
        }

        public string Property { get; set; }
        public SqlFragment Fragment { get; set; }

        //protected override string  InnerText
        //{
        //    get 
        //    {
        //        return String.Format("{0}.{1}", QuoteIdentifier(Fragment.Name), 
        //            QuoteIdentifier(Property));
        //    }
        //}
    }
}
