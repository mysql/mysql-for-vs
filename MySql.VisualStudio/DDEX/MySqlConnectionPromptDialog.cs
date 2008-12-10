using Microsoft.VisualStudio.Data;
using System.Drawing;

namespace MySql.Data.VisualStudio
{
    class MySqlConnectionPromptDialog : DataConnectionPromptDialog
    {
        public MySqlConnectionPromptDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MySqlConnectionPromptDialog
            // 
            this.ClientSize = new System.Drawing.Size(284, 264);
            //this.Location = new System.Drawing.Point(-10000, -10000);
            this.Visible = false;
            this.Name = "MySqlConnectionPromptDialog";
            this.Load += new System.EventHandler(this.MySqlConnectionPromptDialog_Load);
//            this.Shown += new System.EventHandler(this.MySqlConnectionPromptDialog_Shown);
            this.ResumeLayout(false);

        }

        private void MySqlConnectionPromptDialog_Load(object sender, System.EventArgs e)
        {
            Close();
        }

  //      private void MySqlConnectionPromptDialog_Shown(object sender, System.EventArgs e)
    //    {
      //      Close();
        //}


    }
}
