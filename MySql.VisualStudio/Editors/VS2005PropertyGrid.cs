using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace MySql.Data.VisualStudio.Editors
{
    public class VS2005PropertyGrid : PropertyGrid
    {
        public VS2005PropertyGrid()
        {
            
            this.ToolStripRenderer = new ToolStripProfessionalRenderer(new VS2005ColorTable());
        }
       
        
    }
}
