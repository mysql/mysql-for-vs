using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySql.Data.VisualStudio.Wizards.Web
{
  public partial class TablesSelection : WizardPage
  {

    private BindingList<DbTables> _tables = new BindingList<DbTables>();
    BindingSource _sourceTables = new BindingSource();

    public TablesSelection()
    {
      InitializeComponent();      
      listTables.CellContentClick +=new DataGridViewCellEventHandler(listTables_CellContentClick);
      chkSelectAllTables.CheckedChanged += chkSelectAllTables_CheckedChanged;
    }

    void chkSelectAllTables_CheckedChanged(object sender, EventArgs e)
    {
        _tables.Where(t => t.Selected == !chkSelectAllTables.Checked).ToList().ForEach(t => { t.CheckObject(chkSelectAllTables.Checked); });
      listTables.Refresh();
    }

    internal List<DbTables> selectedTables
    {
      get
      {
        return _tables.Where(t => t.Selected).ToList();
      }
    }


     private void FillTables(string connectionString)
    {
      var cnn = new MySqlConnection(connectionString);
      cnn.Open();

      var dtTables = cnn.GetSchema("Tables", new string[] { null, cnn.Database });
      _tables = new BindingList<DbTables>();

      for (int i = 0; i < dtTables.Rows.Count; i++)
      {
        _tables.Add(new DbTables(false, dtTables.Rows[i][2].ToString()));
      }

      _sourceTables.DataSource = _tables;
      listTables.DataSource = _sourceTables;
      FormatTablesList();
    }

    private void FormatTablesList()
    {
      if (listTables.Rows.Count <= 1 && listTables.Columns.Count == 1)
      {
        _sourceTables.DataSource = new BindingList<DbTables>();
        listTables.DataSource = _sourceTables;
        listTables.Update();
      }
      listTables.Columns[0].HeaderText = "Select";
      listTables.Columns[0].Width = 45;

      listTables.Columns[1].HeaderText = "Table";
      listTables.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      listTables.Columns[1].ReadOnly = true;

      listTables.Refresh();
    }

    private void listTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex == -1) return;
      if (String.IsNullOrEmpty(listTables.Rows[e.RowIndex].Cells[1].Value as string))
        return;

      if (e.ColumnIndex == 0)
      {
        var selected = _tables.Single(t => t.Name.Equals((string)listTables.Rows[e.RowIndex].Cells[1].Value,
          StringComparison.InvariantCultureIgnoreCase));
        selected.CheckObject(!selected.Selected);
        listTables.Refresh();
      }
    }


    internal override void OnStarting(BaseWizardForm wizard)
    {
      WebWizardForm wiz = (WebWizardForm)wizard;

      if (!string.IsNullOrEmpty(wiz.connectionStringForModel))
      {
        var cnn = new MySqlConnection(wiz.connectionStringForModel);
        try
        {
          cnn.Open();
        }
        catch (Exception)
        {
          DialogResult result = MessageBox.Show(Properties.Resources.ErrorOnConnection, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
          if (result == DialogResult.Cancel)
          {
            listTables.Enabled = false;
          }
        }
        FillTables(wiz.connectionStringForModel);
      }
    }

    internal override bool IsValid()
    {
      CancelEventArgs args = new CancelEventArgs();
      TableSelection_Validating(this, args);
      if (args.Cancel) return false;
      else return true;
    }


    void TableSelection_Validating(object sender, CancelEventArgs e)
    {   
        if (_tables == null && _tables.Count == 0)
        {
          e.Cancel = true;
          errorProvider1.SetError(listTables, "At least a table should be selected");
        }
        else
        {
          errorProvider1.SetError(listTables, "");
        }
    }
  }

  public class DbTables : INotifyPropertyChanged
  {

    private bool _selected { get; set; }
    private string _name { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public bool Selected
    {
      get
      {
        return _selected;
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
    }

    public DbTables(bool selected, string name)
    {
      _selected = selected;
      _name = name;
    }

    private void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public void CheckObject(bool selected)
    {
      _selected = selected;
      NotifyPropertyChanged("Selected");
    }
  }
}
