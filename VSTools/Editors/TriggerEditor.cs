using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MySql.VSTools
{
    internal partial class TriggerEditor : BaseEditor
    {
        public TriggerEditor(TriggerNode node) : base(node)
        {
            InitializeComponent();
            if (DesignMode) return;
            base.Init();

            if (node == null) return;
            sql.Text = node.Body;
            triggername.Text = node.Name;
            schema.Text = node.Schema;
            when.SelectedItem = node.ActionTime;
            action.SelectedItem = node.Action;
            IsDirty = false;
        }

        public string TriggerName
        {
            get { return schema.Text + "." + triggername.Text; }
        }

        public string ActionTiming
        {
            get { return when.SelectedItem.ToString(); }
        }

        public string Action
        {
            get { return action.SelectedItem.ToString(); }
        }

        public string Body
        {
            get { return sql.Text; }
        }

        private void triggername_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void when_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void action_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void sql_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
        }
    }
}
