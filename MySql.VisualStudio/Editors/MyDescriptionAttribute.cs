using System.ComponentModel;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Editors
{
    class MyDescriptionAttribute : DescriptionAttribute
    {
        private string rezName;

        public MyDescriptionAttribute(string resourceName)
        {
            rezName = resourceName;
        }

        public override string Description
        {
            get
            {
                return Resources.ResourceManager.GetString(rezName);
            }
        }
    }
}
