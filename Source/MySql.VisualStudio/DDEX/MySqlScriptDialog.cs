using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio
{
  public partial class MySqlScriptDialog : Form
  {
    public MySqlScriptDialog()
    {
      InitializeComponent();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    public string TextScript
    {
      get { return txtScript.Text; }
      set { txtScript.Text = value; }
    }
  }
}
