using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Resources;
using System.Reflection;

namespace MySql.VSTools
{
    internal partial class TableEditor : BaseEditor
    {
        private TableNode table;
        private ImageList listImages;
        private Font wingDingFont;

        public TableEditor(TableNode table)
        {
            InitializeComponent();

            if (DesignMode) return;
            base.Init();

            this.table = table;
            tableName.Text = table.Caption;
            tableSchema.Text = table.Schema;
            tableType.SelectedItem = table.TypeName;
            tableType.SelectedText = table.TypeName;
            dataDirectory.Text = table.DataDirectory;
            indexDirectory.Text = table.IndexDirectory;
            rowFormat.SelectedItem = table.RowFormat;
            useChecksum.Checked = table.UseChecksum;
            avgRowLen.Text = AsString(table.AverageRowLength);
            minRows.Text = AsString(table.MinimumRowCount);
            maxRows.Text = AsString(table.MaximumRowCount);

            // setup the image list that will handle the 'key' image
            listImages = new ImageList();
            ResourceManager rm = new ResourceManager("MySql.VSTools.VSPackage",
                Assembly.GetExecutingAssembly());
            listImages.ImageSize = new Size(9, 11);
            listImages.TransparentColor = Color.Transparent;
            listImages.Images.Add((Image)rm.GetObject("key"));
            columnList.SmallImageList = listImages;

            wingDingFont = new Font("Wingdings", 9F, FontStyle.Regular);
            PopulateGrid();
        }

        private string AsString(object val)
        {
            if (val.Equals(0))
                return String.Empty;
            return val.ToString();
        }

        private void PopulateGrid()
        {
            columnList.Items.Clear();

            ArrayList cols = table.GetColumns();
            foreach (ColumnNode node in cols)
            {
                ListViewItem item = columnList.Items.Add(node.Caption);
                if (node.IsPrimary)
                    item.ImageIndex = 0;
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(node.Typename);
                item.SubItems.Add(node.LengthAsString);
                ListViewItem.ListViewSubItem sub = item.SubItems.Add(
                    node.CanBeNull ? "u" : "");
                sub.ForeColor = Color.DarkBlue;
                sub.Font = wingDingFont;
                sub = item.SubItems.Add(node.IsBinary ? "u" : "");
                sub.ForeColor = Color.DarkBlue;
                sub.Font = wingDingFont;
                sub = item.SubItems.Add(node.ZeroFill ? "u" : "");
                sub.Font = wingDingFont;
                sub.ForeColor = Color.DarkBlue;
                sub = item.SubItems.Add(node.CharacterSet);
                sub = item.SubItems.Add(node.Collation);
            }
        }

        private void columnList_SizeChanged(object sender, EventArgs e)
        {

        }

        void columnList_DoubleClick(object sender, System.EventArgs e)
        {

            throw new System.Exception("The method or operation is not implemented.");
        }



    }
}
