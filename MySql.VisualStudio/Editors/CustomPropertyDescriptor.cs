using System.ComponentModel;
using System;
using MySql.Data.VisualStudio.DbObjects;

namespace MySql.Data.VisualStudio.Editors
{
    class CustomPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor myProp;
        private bool readOnly;

        public CustomPropertyDescriptor(PropertyDescriptor pd) : 
            base(pd)
        {
            myProp = pd;
        }

        public override bool IsReadOnly
        {
            get { return readOnly; }
        }

        public void SetReadOnly(bool value)
        {
            readOnly = value;
        }

        public override bool CanResetValue(object component)
        {
            return true;
        }

        public override Type ComponentType
        {
            get { return typeof(Table); }
        }

        public override object GetValue(object component)
        {
            return myProp.GetValue(component);
        }

        public override System.Type PropertyType
        {
            get { return typeof(string); }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            myProp.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
