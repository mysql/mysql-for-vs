// Copyright (c) 2009, 2019, Oracle and/or its affiliates. All rights reserved.
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
using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;
using System.Diagnostics;
using System.Xml;

namespace MySql.Data.VisualStudio.WebConfig
{
  internal struct MembershipOptions
  {
    public bool EnablePasswordRetrieval;
    public bool EnablePasswordReset;
    public bool RequiresQA;
    public bool RequiresUniqueEmail;
    public MembershipPasswordFormat PasswordFormat;
    public int MaxInvalidPasswordAttempts;
    public int MinRequiredPasswordLength;
    public int MinRequiredNonAlphaNumericCharacters;
    public int PasswordAttemptWindow;
    public string PasswordStrengthRegEx;
  }

  internal class MembershipConfig : GenericConfig
  {
    private new MembershipOptions defaults = new MembershipOptions();
    private new MembershipOptions values;

    public MembershipConfig()
      : base()
    {
      typeName = "MySQLMembershipProvider";
      sectionName = "membership";
    }

    public MembershipOptions MemberOptions
    {
      get { return values; }
      set { values = value; }
    }

    protected override ProviderSettings GetMachineSettings()
    {
      Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
      MembershipSection section = (MembershipSection)machineConfig.SectionGroups["system.web"].Sections[sectionName];
      foreach (ProviderSettings p in section.Providers)
        if (p.Type.Contains(typeName)) return p;
      return null;
    }

    public override void Initialize(AppConfig wc)
    {
      base.Initialize(wc);

      values = defaults;

      XmlElement e = wc.GetProviderElement(sectionName);
      if (e == null) return;

      //GetOptionalParameter(e, "description");
      if (e.HasAttribute("enablePasswordRetrieval"))
        values.EnablePasswordRetrieval = Convert.ToBoolean(e.GetAttribute("enablePasswordRetrieval"));
      if (e.HasAttribute("enablePasswordReset"))
        values.EnablePasswordReset = Convert.ToBoolean(e.GetAttribute("enablePasswordReset"));
      if (e.HasAttribute("requiresQuestionAndAnswer"))
        values.RequiresQA = Convert.ToBoolean(e.GetAttribute("requiresQuestionAndAnswer"));
      if (e.HasAttribute("requiresUniqueEmail"))
        values.RequiresUniqueEmail = Convert.ToBoolean(e.GetAttribute("requiresUniqueEmail"));
      if (e.HasAttribute("passwordFormat"))
        values.PasswordFormat = (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), e.GetAttribute("passwordFormat"));
      if (e.HasAttribute("passwordStrengthRegularExpression"))
        values.PasswordStrengthRegEx = e.GetAttribute("passwordStrengthRegularExpression");
      if (e.HasAttribute("maxInvalidPasswordAttempts"))
        values.MaxInvalidPasswordAttempts = Convert.ToInt32(e.GetAttribute("maxInvalidPasswordAttempts"));
      if (e.HasAttribute("minRequiredPasswordLength"))
        values.MinRequiredPasswordLength = Convert.ToInt32(e.GetAttribute("minRequiredPasswordLength"));
      if (e.HasAttribute("minRequiredNonalphanumericCharacters"))
        values.MinRequiredNonAlphaNumericCharacters = Convert.ToInt32(e.GetAttribute("minRequiredNonalphanumericCharacters"));
      if (e.HasAttribute("passwordAttemptWindow"))
        values.PasswordAttemptWindow = Convert.ToInt32(e.GetAttribute("passwordAttemptWindow"));
    }

    override public void GetDefaults()
    {
      base.GetDefaults();

      foreach (MembershipProvider p in Membership.Providers)
      {
        if (!p.GetType().ToString().Contains("MySQL")) continue;

        base.defaults.AppDescription = p.Description;
        defaults.EnablePasswordReset = p.EnablePasswordReset;
        defaults.EnablePasswordRetrieval = p.EnablePasswordRetrieval;
        defaults.RequiresQA = p.RequiresQuestionAndAnswer;
        defaults.RequiresUniqueEmail = p.RequiresUniqueEmail;
        defaults.PasswordFormat = p.PasswordFormat;
        defaults.MaxInvalidPasswordAttempts = p.MaxInvalidPasswordAttempts;
        defaults.MinRequiredPasswordLength = p.MinRequiredPasswordLength;
        defaults.MinRequiredNonAlphaNumericCharacters = p.MinRequiredNonAlphanumericCharacters;
        defaults.PasswordAttemptWindow = p.PasswordAttemptWindow;
        defaults.PasswordStrengthRegEx = p.PasswordStrengthRegularExpression;
        break;
      }
    }

    protected override void SaveProvider(XmlElement provider)
    {
      base.SaveProvider(provider);

      provider.SetAttribute("enablePasswordRetrieval", values.EnablePasswordRetrieval.ToString());
      provider.SetAttribute("enablePasswordReset", values.EnablePasswordReset.ToString());
      provider.SetAttribute("requiresQuestionAndAnswer", values.RequiresQA.ToString());
      provider.SetAttribute("requiresUniqueEmail", values.RequiresUniqueEmail.ToString());
      provider.SetAttribute("passwordFormat", values.PasswordFormat.ToString());
      provider.SetAttribute("maxInvalidPasswordAttempts", values.MaxInvalidPasswordAttempts.ToString());
      provider.SetAttribute("minRequiredPasswordLength", values.MinRequiredPasswordLength.ToString());
      provider.SetAttribute("minRequiredNonalphanumericCharacters", values.MinRequiredNonAlphaNumericCharacters.ToString());
      provider.SetAttribute("passwordAttemptWindow", values.PasswordAttemptWindow.ToString());
      provider.SetAttribute("passwordStrengthRegularExpression", values.PasswordStrengthRegEx);
    }
  }
}
