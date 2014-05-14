using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySQL.Utility.Classes;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio
{
  public class MySqlForVisualStudioSettings : CustomSettingsProvider
  {
    public override string ApplicationName
    {
      get { return Resources.ApplicationName; }
      set { }
    }

    public override string SettingsPath
    {
      get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Oracle\MySQL For Visual Studio\settings.config"; }
    }
  }
}
