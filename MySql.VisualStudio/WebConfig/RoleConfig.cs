// Copyright (c) 2009 Sun Microsystems, Inc.
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Configuration;
using System.Web.Configuration;

namespace MySql.Data.VisualStudio.WebConfig
{
    internal class RoleConfig : GenericConfig
    {
        public RoleConfig()
            : base()
        {
            typeName = "MySQLRoleProvider";
            sectionName = "roleManager";
        }

        protected override ProviderSettings GetMachineSettings()
        {
            Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
            RoleManagerSection section = (RoleManagerSection)machineConfig.SectionGroups["system.web"].Sections[sectionName];
            foreach (ProviderSettings p in section.Providers)
                if (p.Type.Contains(typeName)) return p;
            return null;
        }
    }
}
