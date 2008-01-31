// Copyright (C) 2007 MySQL AB
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

//  This code was contributed by Sean Wright (srwright@alcor.concordia.ca) on 2007-01-12
//  The copyright was assigned and transferred under the terms of
//  the MySQL Contributor License Agreement (CLA)

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Profile;
using System.Web.Security;

namespace MySql.Web.Tests
{
    public class TestProfile : ProfileBase
    {
        public static TestProfile GetUserProfile(string username, bool auth)
        {
            return Create(username, auth) as TestProfile;
        }

        public static TestProfile GetUserProfile(bool auth) 
        {
            return Create(Membership.GetUser().UserName, auth) as TestProfile; 
        }

        [SettingsAllowAnonymous(false)]
        public string Description 
        { 
            get { return base["Description"] as string; } 
            set { base["Description"] = value; } 
        }

        [SettingsAllowAnonymous(false)]
        public string Location 
        { 
            get { return base["Location"] as string; } 
            set { base["Location"] = value; } 
        }

        [SettingsAllowAnonymous(false)]
        public string FavoriteMovie 
        { 
            get { return base["FavoriteMovie"] as string; } 
            set { base["FavoriteMovie"] = value; } 
        }
    } 
}
