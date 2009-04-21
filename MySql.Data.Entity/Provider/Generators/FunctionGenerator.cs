// Copyright © 2008 MySQL AB, 2008-2009 Sun Microsystems, Inc.
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
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Data;

namespace MySql.Data.Entity
{
    class FunctionGenerator : SqlGenerator 
    {
        public CommandType CommandType { get; private set; }

        public override string GenerateSQL(DbCommandTree commandTree)
        {
            DbFunctionCommandTree tree = (commandTree as DbFunctionCommandTree);
            EdmFunction function = tree.EdmFunction;
            CommandType = CommandType.StoredProcedure;

            string cmdText = (string)function.MetadataProperties["CommandTextAttribute"].Value;
            if (String.IsNullOrEmpty(cmdText))
            {
                string schema = (string)function.MetadataProperties["Schema"].Value;
                if (String.IsNullOrEmpty(schema))
                    schema = function.NamespaceName;

                string functionName = (string)function.MetadataProperties["StoreFunctionNameAttribute"].Value;
                if (String.IsNullOrEmpty(functionName))
                    functionName = function.Name;

                return String.Format("`{0}`", functionName);
            }
            else
            {
                CommandType = CommandType.Text;
                return cmdText;
            }
        }
    }
}
