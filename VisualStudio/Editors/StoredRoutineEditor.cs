using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Windows.Forms;

namespace MySql.Data.VisualStudio
{
    public class StoredRoutineEditor : TextBasedEditor
    {
        private MyEditor textBox;

        public StoredRoutineEditor()
        {
        }

        public StoredRoutineEditor(MyPackage package)
            : base(package)
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.textBox = new MySql.Data.VisualStudio.MyEditor();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.Location = new System.Drawing.Point(8, 8);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(403, 358);
            this.textBox.TabIndex = 0;
            this.textBox.Text = "";
            // 
            // StoredRoutineEditor
            // 
            this.Controls.Add(this.textBox);
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "StoredRoutineEditor";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(419, 374);
            this.ResumeLayout(false);

        }

        public override string EditorType
        {
            get { return "PROCEDURE"; }
        }

        protected override void LoadData(string fileName)
        {
            string name = TrimFileName(fileName);
            base.LoadData(name);

            MySqlDataReader reader = null;
            try
            {
                MySqlConnection conn = Connection.GetLockedProviderObject() as MySqlConnection;
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SHOW CREATE PROCEDURE " + name;
                reader = cmd.ExecuteReader();
                reader.Read();
                textBox.Text = reader.GetString(2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Connection.UnlockProviderObject();
            }
        }

        protected override void SaveData(string fileName)
        {
            int i = 0;
        }
    }
}
