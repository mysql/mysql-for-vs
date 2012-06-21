using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MySql.Debugger.VisualStudio
{
  public partial class StoredRoutineArgumentsDlg : Form
  {
    public StoredRoutineArgumentsDlg()
    {
      InitializeComponent();
      Init();
    }

    private void Init()
    {
      Arguments = new DataTable();
      Arguments.Columns.Add("Name", typeof(string));
      Arguments.Columns.Add("Value", typeof(string));
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void StoredRoutineArguments_Load(object sender, EventArgs e)
    {
      
    }

    internal DataTable Arguments { get; set; }

    internal void AddNameValue( string Name, string Value )
    {
      Arguments.Rows.Add(Name, Value);
    }

    internal void DataBind()
    {
      gridArguments.DataSource = Arguments;
      gridArguments.Columns["Name"].ReadOnly = true;
    }

    internal IEnumerable<NameValue> GetNameValues()
    {
      foreach (DataRow dr in Arguments.Rows)
      {
        yield return new NameValue() { Name = ( string )dr[ 0 ], Value = ( string )dr[ 1 ] }; 
      }
    }
  }

  internal class NameValue
  {
    internal string Name { get; set; }
    internal string Value { get; set; }
  }
}
