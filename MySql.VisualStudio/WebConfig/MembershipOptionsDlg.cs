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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Web.Security;

namespace MySql.Data.VisualStudio.WebConfig
{
    public partial class MembershipOptionsDlg : Form
    {
        public MembershipOptionsDlg()
        {
            InitializeComponent();
            passwordFormat.DataSource = Enum.GetValues(typeof(MembershipPasswordFormat));
        }

        internal MembershipOptions Options
        {
            get 
            {
                MembershipOptions options = new MembershipOptions();
                options.EnablePasswordReset = enablePasswordReset.Checked;
                options.EnablePasswordRetrieval = enablePasswordRetrieval.Checked;
                options.MaxInvalidPasswordAttempts = (int)maxInvalidPassAttempts.Value;
                options.MinRequiredNonAlphaNumericCharacters = (int)minRequiredNonAlpha.Value;
                options.MinRequiredPasswordLength = (int)minPassLength.Value;
                options.PasswordAttemptWindow = (int)passwordAttemptWindow.Value;
                options.PasswordFormat = (MembershipPasswordFormat)passwordFormat.SelectedItem;
                options.PasswordStrengthRegEx = passwordRegex.Text.Trim();
                options.RequiresQA = requireQA.Checked;
                options.RequiresUniqueEmail = requireUniqueEmail.Checked;
                return options;
            }
            set 
            {
                enablePasswordReset.Checked = value.EnablePasswordReset;
                enablePasswordRetrieval.Checked = value.EnablePasswordRetrieval;
                maxInvalidPassAttempts.Value = value.MaxInvalidPasswordAttempts;
                minRequiredNonAlpha.Value = value.MinRequiredNonAlphaNumericCharacters;
                minPassLength.Value = value.MinRequiredPasswordLength;
                passwordAttemptWindow.Value = value.PasswordAttemptWindow;
                passwordFormat.SelectedItem = value.PasswordFormat;
                passwordRegex.Text = value.PasswordStrengthRegEx;
                requireQA.Checked = value.RequiresQA;
                requireUniqueEmail.Checked = value.RequiresUniqueEmail;
            }
        }
    }
}
