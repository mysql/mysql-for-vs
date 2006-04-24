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
        private TriggerNode triggerNode;

        public TriggerEditor(TriggerNode node)
        {
            InitializeComponent();
            if (DesignMode) return;
            base.Init();

            triggerNode = node;
            if (triggerNode == null) return;
            sql.Text = triggerNode.Body;
            triggername.Text = triggerNode.Caption;
            schema.Text = triggerNode.Schema;
            when.SelectedItem = triggerNode.ActionTime;
            action.SelectedItem = triggerNode.Action;
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
