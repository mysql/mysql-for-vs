namespace MySql.Data.VisualStudio.DBExport
{
    partial class dbExportPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.components = new System.ComponentModel.Container();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.single_transaction = new System.Windows.Forms.CheckBox();
      this.mySqlDbExportOptionsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.routines = new System.Windows.Forms.CheckBox();
      this.no_data = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.btnSaveFile = new System.Windows.Forms.Button();
      this.txtFileName = new System.Windows.Forms.TextBox();
      this.btnAdvanced = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.btnUnSelect = new System.Windows.Forms.Button();
      this.btnSelectAll = new System.Windows.Forms.Button();
      this.dbObjectsList = new System.Windows.Forms.DataGridView();
      this.txtFilter = new System.Windows.Forms.TextBox();
      this.btnFilter = new System.Windows.Forms.Button();
      this.dbSchemasList = new System.Windows.Forms.DataGridView();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btnExport = new System.Windows.Forms.Button();
      this.cmbConnections = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.pnlGeneral = new System.Windows.Forms.Panel();
      this.pnlAdvanced = new System.Windows.Forms.Panel();
      this.groupBox9 = new System.Windows.Forms.GroupBox();
      this.no_create_info = new System.Windows.Forms.CheckBox();
      this.add_drop_table = new System.Windows.Forms.CheckBox();
      this.create_options = new System.Windows.Forms.CheckBox();
      this.add_locks = new System.Windows.Forms.CheckBox();
      this.groupBox8 = new System.Windows.Forms.GroupBox();
      this.replace = new System.Windows.Forms.CheckBox();
      this.insert_ignore = new System.Windows.Forms.CheckBox();
      this.disable_keys = new System.Windows.Forms.CheckBox();
      this.extended_insert = new System.Windows.Forms.CheckBox();
      this.delayed_insert = new System.Windows.Forms.CheckBox();
      this.complete_insert = new System.Windows.Forms.CheckBox();
      this.btnReturn = new System.Windows.Forms.Button();
      this.groupBox6 = new System.Windows.Forms.GroupBox();
      this.max_allowed_packet = new System.Windows.Forms.TextBox();
      this.quote_names = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.flush_logs = new System.Windows.Forms.CheckBox();
      this.compact = new System.Windows.Forms.CheckBox();
      this.comments = new System.Windows.Forms.CheckBox();
      this.default_character_set = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox5 = new System.Windows.Forms.GroupBox();
      this.events = new System.Windows.Forms.CheckBox();
      this.lock_tables = new System.Windows.Forms.CheckBox();
      this.allow_keywords = new System.Windows.Forms.CheckBox();
      this.add_drop_database = new System.Windows.Forms.CheckBox();
      this.schemaColumn1 = new MySql.Data.VisualStudio.DBExport.SchemaColumn();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.mySqlDbExportOptionsBindingSource)).BeginInit();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dbObjectsList)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbSchemasList)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.pnlGeneral.SuspendLayout();
      this.pnlAdvanced.SuspendLayout();
      this.groupBox9.SuspendLayout();
      this.groupBox8.SuspendLayout();
      this.groupBox6.SuspendLayout();
      this.groupBox5.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.single_transaction);
      this.groupBox1.Controls.Add(this.routines);
      this.groupBox1.Controls.Add(this.no_data);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.btnSaveFile);
      this.groupBox1.Controls.Add(this.txtFileName);
      this.groupBox1.Controls.Add(this.btnAdvanced);
      this.groupBox1.Location = new System.Drawing.Point(8, 355);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(793, 145);
      this.groupBox1.TabIndex = 2;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Options";
      // 
      // single_transaction
      // 
      this.single_transaction.AutoSize = true;
      this.single_transaction.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "single_transaction", true));
      this.single_transaction.Location = new System.Drawing.Point(211, 80);
      this.single_transaction.Name = "single_transaction";
      this.single_transaction.Size = new System.Drawing.Size(139, 17);
      this.single_transaction.TabIndex = 1;
      this.single_transaction.Text = "Use a single transaction";
      this.single_transaction.UseVisualStyleBackColor = true;
      // 
      // mySqlDbExportOptionsBindingSource
      // 
      this.mySqlDbExportOptionsBindingSource.DataSource = typeof(MySql.Data.VisualStudio.DBExport.MySqlDbExportOptions);
      // 
      // routines
      // 
      this.routines.AutoSize = true;
      this.routines.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "routines", true));
      this.routines.Location = new System.Drawing.Point(17, 105);
      this.routines.Name = "routines";
      this.routines.Size = new System.Drawing.Size(133, 17);
      this.routines.TabIndex = 3;
      this.routines.Text = "Include stored routines";
      this.routines.UseVisualStyleBackColor = true;
      // 
      // no_data
      // 
      this.no_data.AutoSize = true;
      this.no_data.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "no_data", true));
      this.no_data.Location = new System.Drawing.Point(17, 80);
      this.no_data.Name = "no_data";
      this.no_data.Size = new System.Drawing.Size(152, 17);
      this.no_data.TabIndex = 4;
      this.no_data.Text = "Skip Insert data sentences";
      this.no_data.UseVisualStyleBackColor = true;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(13, 34);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(192, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Select path to save export file (*.mysql):";
      // 
      // btnSaveFile
      // 
      this.btnSaveFile.Location = new System.Drawing.Point(610, 31);
      this.btnSaveFile.Name = "btnSaveFile";
      this.btnSaveFile.Size = new System.Drawing.Size(30, 20);
      this.btnSaveFile.TabIndex = 3;
      this.btnSaveFile.Text = "...";
      this.btnSaveFile.UseVisualStyleBackColor = true;
      this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
      // 
      // txtFileName
      // 
      this.txtFileName.Location = new System.Drawing.Point(211, 32);
      this.txtFileName.Name = "txtFileName";
      this.txtFileName.Size = new System.Drawing.Size(393, 20);
      this.txtFileName.TabIndex = 2;
      // 
      // btnAdvanced
      // 
      this.btnAdvanced.Location = new System.Drawing.Point(652, 30);
      this.btnAdvanced.Name = "btnAdvanced";
      this.btnAdvanced.Size = new System.Drawing.Size(127, 25);
      this.btnAdvanced.TabIndex = 0;
      this.btnAdvanced.Text = "Advanced";
      this.btnAdvanced.UseVisualStyleBackColor = true;
      this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btnRefresh);
      this.groupBox3.Controls.Add(this.btnUnSelect);
      this.groupBox3.Controls.Add(this.btnSelectAll);
      this.groupBox3.Controls.Add(this.dbObjectsList);
      this.groupBox3.Controls.Add(this.txtFilter);
      this.groupBox3.Controls.Add(this.btnFilter);
      this.groupBox3.Controls.Add(this.dbSchemasList);
      this.groupBox3.Location = new System.Drawing.Point(8, 58);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(787, 278);
      this.groupBox3.TabIndex = 11;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Select objects to export";
      // 
      // btnRefresh
      // 
      this.btnRefresh.Location = new System.Drawing.Point(13, 246);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(99, 21);
      this.btnRefresh.TabIndex = 15;
      this.btnRefresh.Text = "Refresh";
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
      // 
      // btnUnSelect
      // 
      this.btnUnSelect.Location = new System.Drawing.Point(665, 246);
      this.btnUnSelect.Name = "btnUnSelect";
      this.btnUnSelect.Size = new System.Drawing.Size(103, 21);
      this.btnUnSelect.TabIndex = 14;
      this.btnUnSelect.Text = "Unselect All";
      this.btnUnSelect.UseVisualStyleBackColor = true;
      this.btnUnSelect.Click += new System.EventHandler(this.btnUnSelect_Click);
      // 
      // btnSelectAll
      // 
      this.btnSelectAll.Location = new System.Drawing.Point(556, 246);
      this.btnSelectAll.Name = "btnSelectAll";
      this.btnSelectAll.Size = new System.Drawing.Size(103, 21);
      this.btnSelectAll.TabIndex = 13;
      this.btnSelectAll.Text = "Select All";
      this.btnSelectAll.UseVisualStyleBackColor = true;
      this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
      // 
      // dbObjectsList
      // 
      this.dbObjectsList.BackgroundColor = System.Drawing.Color.White;
      this.dbObjectsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dbObjectsList.Location = new System.Drawing.Point(358, 31);
      this.dbObjectsList.Name = "dbObjectsList";
      this.dbObjectsList.RowHeadersVisible = false;
      this.dbObjectsList.Size = new System.Drawing.Size(417, 206);
      this.dbObjectsList.TabIndex = 3;
      // 
      // txtFilter
      // 
      this.txtFilter.Location = new System.Drawing.Point(13, 32);
      this.txtFilter.Name = "txtFilter";
      this.txtFilter.Size = new System.Drawing.Size(222, 20);
      this.txtFilter.TabIndex = 12;
      // 
      // btnFilter
      // 
      this.btnFilter.Location = new System.Drawing.Point(241, 31);
      this.btnFilter.Name = "btnFilter";
      this.btnFilter.Size = new System.Drawing.Size(90, 21);
      this.btnFilter.TabIndex = 11;
      this.btnFilter.Text = "Filter schemas";
      this.btnFilter.UseVisualStyleBackColor = true;
      this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
      // 
      // dbSchemasList
      // 
      this.dbSchemasList.AllowUserToAddRows = false;
      this.dbSchemasList.BackgroundColor = System.Drawing.Color.White;
      this.dbSchemasList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dbSchemasList.Location = new System.Drawing.Point(13, 68);
      this.dbSchemasList.Name = "dbSchemasList";
      this.dbSchemasList.RowHeadersVisible = false;
      this.dbSchemasList.Size = new System.Drawing.Size(318, 169);
      this.dbSchemasList.TabIndex = 10;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btnExport);
      this.groupBox2.Controls.Add(this.cmbConnections);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Location = new System.Drawing.Point(8, 6);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(787, 44);
      this.groupBox2.TabIndex = 4;
      this.groupBox2.TabStop = false;
      // 
      // btnExport
      // 
      this.btnExport.Location = new System.Drawing.Point(506, 13);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(99, 21);
      this.btnExport.TabIndex = 5;
      this.btnExport.Text = "Export";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // cmbConnections
      // 
      this.cmbConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbConnections.FormattingEnabled = true;
      this.cmbConnections.Location = new System.Drawing.Point(157, 13);
      this.cmbConnections.Name = "cmbConnections";
      this.cmbConnections.Size = new System.Drawing.Size(333, 21);
      this.cmbConnections.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 17);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(127, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "MySql Server Connection";
      // 
      // pnlGeneral
      // 
      this.pnlGeneral.Controls.Add(this.groupBox2);
      this.pnlGeneral.Controls.Add(this.groupBox3);
      this.pnlGeneral.Location = new System.Drawing.Point(6, 5);
      this.pnlGeneral.Name = "pnlGeneral";
      this.pnlGeneral.Size = new System.Drawing.Size(801, 344);
      this.pnlGeneral.TabIndex = 13;
      // 
      // pnlAdvanced
      // 
      this.pnlAdvanced.Controls.Add(this.groupBox9);
      this.pnlAdvanced.Controls.Add(this.groupBox8);
      this.pnlAdvanced.Controls.Add(this.btnReturn);
      this.pnlAdvanced.Controls.Add(this.groupBox6);
      this.pnlAdvanced.Controls.Add(this.groupBox5);
      this.pnlAdvanced.Location = new System.Drawing.Point(-2, 3);
      this.pnlAdvanced.Name = "pnlAdvanced";
      this.pnlAdvanced.Size = new System.Drawing.Size(809, 510);
      this.pnlAdvanced.TabIndex = 15;
      this.pnlAdvanced.Visible = false;
      // 
      // groupBox9
      // 
      this.groupBox9.Controls.Add(this.no_create_info);
      this.groupBox9.Controls.Add(this.add_drop_table);
      this.groupBox9.Controls.Add(this.create_options);
      this.groupBox9.Controls.Add(this.add_locks);
      this.groupBox9.Location = new System.Drawing.Point(13, 227);
      this.groupBox9.Name = "groupBox9";
      this.groupBox9.Size = new System.Drawing.Size(768, 92);
      this.groupBox9.TabIndex = 7;
      this.groupBox9.TabStop = false;
      this.groupBox9.Text = "Table Options";
      // 
      // no_create_info
      // 
      this.no_create_info.AutoSize = true;
      this.no_create_info.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "no_create_info", true));
      this.no_create_info.Location = new System.Drawing.Point(417, 19);
      this.no_create_info.Name = "no_create_info";
      this.no_create_info.Size = new System.Drawing.Size(329, 17);
      this.no_create_info.TabIndex = 11;
      this.no_create_info.Text = "Do not write CREATE TABLE that re-create each dumped table.";
      this.no_create_info.UseVisualStyleBackColor = true;
      // 
      // add_drop_table
      // 
      this.add_drop_table.AutoSize = true;
      this.add_drop_table.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.mySqlDbExportOptionsBindingSource, "add_drop_table", true));
      this.add_drop_table.Location = new System.Drawing.Point(14, 19);
      this.add_drop_table.Name = "add_drop_table";
      this.add_drop_table.Size = new System.Drawing.Size(259, 17);
      this.add_drop_table.TabIndex = 3;
      this.add_drop_table.Text = "Add DROP TABLE before each CREATE TABLE";
      this.add_drop_table.UseVisualStyleBackColor = true;
      // 
      // create_options
      // 
      this.create_options.AutoSize = true;
      this.create_options.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "create_options", true));
      this.create_options.Location = new System.Drawing.Point(14, 60);
      this.create_options.Name = "create_options";
      this.create_options.Size = new System.Drawing.Size(365, 17);
      this.create_options.TabIndex = 10;
      this.create_options.Text = "Include all MySQL-specific table options in CREATE TABLE statements.";
      this.create_options.UseVisualStyleBackColor = true;
      // 
      // add_locks
      // 
      this.add_locks.AutoSize = true;
      this.add_locks.DataBindings.Add(new System.Windows.Forms.Binding("CheckState", this.mySqlDbExportOptionsBindingSource, "add_locks", true));
      this.add_locks.Location = new System.Drawing.Point(14, 38);
      this.add_locks.Name = "add_locks";
      this.add_locks.Size = new System.Drawing.Size(232, 17);
      this.add_locks.TabIndex = 5;
      this.add_locks.Text = "Add LOCK TABLES and UNLOCK TABLES";
      this.add_locks.UseVisualStyleBackColor = true;
      // 
      // groupBox8
      // 
      this.groupBox8.Controls.Add(this.replace);
      this.groupBox8.Controls.Add(this.insert_ignore);
      this.groupBox8.Controls.Add(this.disable_keys);
      this.groupBox8.Controls.Add(this.extended_insert);
      this.groupBox8.Controls.Add(this.delayed_insert);
      this.groupBox8.Controls.Add(this.complete_insert);
      this.groupBox8.Location = new System.Drawing.Point(13, 325);
      this.groupBox8.Name = "groupBox8";
      this.groupBox8.Size = new System.Drawing.Size(768, 134);
      this.groupBox8.TabIndex = 6;
      this.groupBox8.TabStop = false;
      this.groupBox8.Text = "Insert Options";
      // 
      // replace
      // 
      this.replace.AutoSize = true;
      this.replace.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "replace", true));
      this.replace.Location = new System.Drawing.Point(417, 19);
      this.replace.Name = "replace";
      this.replace.Size = new System.Drawing.Size(200, 17);
      this.replace.TabIndex = 13;
      this.replace.Text = "Write REPLACE rather than INSERT";
      this.replace.UseVisualStyleBackColor = true;
      // 
      // insert_ignore
      // 
      this.insert_ignore.AutoSize = true;
      this.insert_ignore.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "insert_ignore", true));
      this.insert_ignore.Location = new System.Drawing.Point(14, 85);
      this.insert_ignore.Name = "insert_ignore";
      this.insert_ignore.Size = new System.Drawing.Size(233, 17);
      this.insert_ignore.TabIndex = 12;
      this.insert_ignore.Text = "Use INSERT IGNORE rather than INSERT.";
      this.insert_ignore.UseVisualStyleBackColor = true;
      // 
      // disable_keys
      // 
      this.disable_keys.AutoSize = true;
      this.disable_keys.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "disable_keys", true));
      this.disable_keys.Location = new System.Drawing.Point(14, 107);
      this.disable_keys.Name = "disable_keys";
      this.disable_keys.Size = new System.Drawing.Size(342, 17);
      this.disable_keys.TabIndex = 11;
      this.disable_keys.Text = "INSERT with statements to disable and enable keys for each table.";
      this.disable_keys.UseVisualStyleBackColor = true;
      // 
      // extended_insert
      // 
      this.extended_insert.AutoSize = true;
      this.extended_insert.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "extended_insert", true));
      this.extended_insert.Location = new System.Drawing.Point(14, 63);
      this.extended_insert.Name = "extended_insert";
      this.extended_insert.Size = new System.Drawing.Size(342, 17);
      this.extended_insert.TabIndex = 11;
      this.extended_insert.Text = "Use multiple-row INSERT syntax that include several VALUES lists.";
      this.extended_insert.UseVisualStyleBackColor = true;
      // 
      // delayed_insert
      // 
      this.delayed_insert.AutoSize = true;
      this.delayed_insert.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "delayed_insert", true));
      this.delayed_insert.Location = new System.Drawing.Point(14, 41);
      this.delayed_insert.Name = "delayed_insert";
      this.delayed_insert.Size = new System.Drawing.Size(241, 17);
      this.delayed_insert.TabIndex = 10;
      this.delayed_insert.Text = "Use INSERT DELAYED rather than INSERT.";
      this.delayed_insert.UseVisualStyleBackColor = true;
      // 
      // complete_insert
      // 
      this.complete_insert.AutoSize = true;
      this.complete_insert.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "complete_insert", true));
      this.complete_insert.Location = new System.Drawing.Point(14, 19);
      this.complete_insert.Name = "complete_insert";
      this.complete_insert.Size = new System.Drawing.Size(320, 17);
      this.complete_insert.TabIndex = 9;
      this.complete_insert.Text = "Use complete INSERT statements that include column names.";
      this.complete_insert.UseVisualStyleBackColor = true;
      // 
      // btnReturn
      // 
      this.btnReturn.Location = new System.Drawing.Point(642, 473);
      this.btnReturn.Name = "btnReturn";
      this.btnReturn.Size = new System.Drawing.Size(138, 24);
      this.btnReturn.TabIndex = 4;
      this.btnReturn.Text = "Return";
      this.btnReturn.UseVisualStyleBackColor = true;
      this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
      // 
      // groupBox6
      // 
      this.groupBox6.Controls.Add(this.max_allowed_packet);
      this.groupBox6.Controls.Add(this.quote_names);
      this.groupBox6.Controls.Add(this.label4);
      this.groupBox6.Controls.Add(this.flush_logs);
      this.groupBox6.Controls.Add(this.compact);
      this.groupBox6.Controls.Add(this.comments);
      this.groupBox6.Controls.Add(this.default_character_set);
      this.groupBox6.Controls.Add(this.label3);
      this.groupBox6.Location = new System.Drawing.Point(13, 14);
      this.groupBox6.Name = "groupBox6";
      this.groupBox6.Size = new System.Drawing.Size(768, 108);
      this.groupBox6.TabIndex = 3;
      this.groupBox6.TabStop = false;
      this.groupBox6.Text = "General Options";
      // 
      // max_allowed_packet
      // 
      this.max_allowed_packet.Location = new System.Drawing.Point(603, 18);
      this.max_allowed_packet.Name = "max_allowed_packet";
      this.max_allowed_packet.Size = new System.Drawing.Size(49, 20);
      this.max_allowed_packet.TabIndex = 9;
      // 
      // quote_names
      // 
      this.quote_names.AutoSize = true;
      this.quote_names.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "quote_names", true));
      this.quote_names.Location = new System.Drawing.Point(14, 57);
      this.quote_names.Name = "quote_names";
      this.quote_names.Size = new System.Drawing.Size(232, 17);
      this.quote_names.TabIndex = 9;
      this.quote_names.Text = "Quote identifiers within backtick characters.";
      this.quote_names.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(277, 20);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(320, 13);
      this.label4.TabIndex = 8;
      this.label4.Text = "Maximum packet length to send to or receive from the server (MB):";
      // 
      // flush_logs
      // 
      this.flush_logs.AutoSize = true;
      this.flush_logs.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "flush_logs", true));
      this.flush_logs.Location = new System.Drawing.Point(14, 78);
      this.flush_logs.Name = "flush_logs";
      this.flush_logs.Size = new System.Drawing.Size(297, 17);
      this.flush_logs.TabIndex = 4;
      this.flush_logs.Text = "Flush the MySQL server log files before starting the dump.";
      this.flush_logs.UseVisualStyleBackColor = true;
      // 
      // compact
      // 
      this.compact.AutoSize = true;
      this.compact.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.compact.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "compact", true));
      this.compact.Location = new System.Drawing.Point(14, 19);
      this.compact.Name = "compact";
      this.compact.Size = new System.Drawing.Size(172, 17);
      this.compact.TabIndex = 7;
      this.compact.Text = "Produce more compact output.";
      this.compact.UseVisualStyleBackColor = true;
      // 
      // comments
      // 
      this.comments.AutoSize = true;
      this.comments.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "comments", true));
      this.comments.Location = new System.Drawing.Point(14, 38);
      this.comments.Name = "comments";
      this.comments.Size = new System.Drawing.Size(174, 17);
      this.comments.TabIndex = 8;
      this.comments.Text = "Add comments to the dump file.";
      this.comments.UseVisualStyleBackColor = true;
      // 
      // default_character_set
      // 
      this.default_character_set.Location = new System.Drawing.Point(603, 55);
      this.default_character_set.Name = "default_character_set";
      this.default_character_set.Size = new System.Drawing.Size(94, 20);
      this.default_character_set.TabIndex = 1;
      this.default_character_set.Text = "utf8";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(390, 58);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(207, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Charset name as the default character set:";
      // 
      // groupBox5
      // 
      this.groupBox5.Controls.Add(this.events);
      this.groupBox5.Controls.Add(this.lock_tables);
      this.groupBox5.Controls.Add(this.allow_keywords);
      this.groupBox5.Controls.Add(this.add_drop_database);
      this.groupBox5.Location = new System.Drawing.Point(13, 129);
      this.groupBox5.Name = "groupBox5";
      this.groupBox5.Size = new System.Drawing.Size(768, 92);
      this.groupBox5.TabIndex = 2;
      this.groupBox5.TabStop = false;
      this.groupBox5.Text = "Database Options";
      // 
      // events
      // 
      this.events.AutoSize = true;
      this.events.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "events", true));
      this.events.Location = new System.Drawing.Point(14, 61);
      this.events.Name = "events";
      this.events.Size = new System.Drawing.Size(226, 17);
      this.events.TabIndex = 13;
      this.events.Text = "Dump events from the dumped databases.";
      this.events.UseVisualStyleBackColor = true;
      // 
      // lock_tables
      // 
      this.lock_tables.AutoSize = true;
      this.lock_tables.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "lock_tables", true));
      this.lock_tables.Location = new System.Drawing.Point(417, 17);
      this.lock_tables.Name = "lock_tables";
      this.lock_tables.Size = new System.Drawing.Size(199, 17);
      this.lock_tables.TabIndex = 12;
      this.lock_tables.Text = "Lock all tables before dumping them.";
      this.lock_tables.UseVisualStyleBackColor = true;
      // 
      // allow_keywords
      // 
      this.allow_keywords.AutoSize = true;
      this.allow_keywords.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "allow_keywords", true));
      this.allow_keywords.Location = new System.Drawing.Point(14, 40);
      this.allow_keywords.Name = "allow_keywords";
      this.allow_keywords.Size = new System.Drawing.Size(265, 17);
      this.allow_keywords.TabIndex = 6;
      this.allow_keywords.Text = "Allow creation of column names that are keywords.";
      this.allow_keywords.UseVisualStyleBackColor = true;
      // 
      // add_drop_database
      // 
      this.add_drop_database.AutoSize = true;
      this.add_drop_database.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mySqlDbExportOptionsBindingSource, "add_drop_database", true));
      this.add_drop_database.Location = new System.Drawing.Point(14, 19);
      this.add_drop_database.Name = "add_drop_database";
      this.add_drop_database.Size = new System.Drawing.Size(308, 17);
      this.add_drop_database.TabIndex = 2;
      this.add_drop_database.Text = "Add DROP DATABASE before each CREATE DATABASE ";
      this.add_drop_database.UseVisualStyleBackColor = true;
      // 
      // schemaColumn1
      // 
      this.schemaColumn1.HeaderText = "";
      this.schemaColumn1.Name = "schemaColumn1";
      this.schemaColumn1.ReadOnly = true;
      this.schemaColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.schemaColumn1.SchemaName = null;
      this.schemaColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
      this.schemaColumn1.Width = 125;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.HeaderText = "";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.dataGridViewTextBoxColumn1.Width = 125;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.HeaderText = "Schema Objects";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridViewTextBoxColumn2.Width = 200;
      // 
      // dbExportPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.pnlAdvanced);
      this.Controls.Add(this.pnlGeneral);
      this.Controls.Add(this.groupBox1);
      this.Name = "dbExportPanel";
      this.Size = new System.Drawing.Size(810, 516);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.mySqlDbExportOptionsBindingSource)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dbObjectsList)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dbSchemasList)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.pnlGeneral.ResumeLayout(false);
      this.pnlAdvanced.ResumeLayout(false);
      this.groupBox9.ResumeLayout(false);
      this.groupBox9.PerformLayout();
      this.groupBox8.ResumeLayout(false);
      this.groupBox8.PerformLayout();
      this.groupBox6.ResumeLayout(false);
      this.groupBox6.PerformLayout();
      this.groupBox5.ResumeLayout(false);
      this.groupBox5.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private SchemaColumn schemaColumn1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnUnSelect;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.DataGridView dbObjectsList;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.DataGridView dbSchemasList;
        private System.Windows.Forms.Panel pnlGeneral;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbConnections;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox routines;
        private System.Windows.Forms.CheckBox single_transaction;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel pnlAdvanced;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox add_drop_database;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox allow_keywords;
        private System.Windows.Forms.CheckBox add_locks;
        private System.Windows.Forms.CheckBox flush_logs;
        private System.Windows.Forms.CheckBox add_drop_table;
        private System.Windows.Forms.CheckBox compact;
        private System.Windows.Forms.CheckBox create_options;
        private System.Windows.Forms.CheckBox complete_insert;
        private System.Windows.Forms.CheckBox comments;
        private System.Windows.Forms.Button btnReturn;
        private System.Windows.Forms.TextBox default_character_set;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox delayed_insert;
        private System.Windows.Forms.CheckBox extended_insert;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.CheckBox disable_keys;
        private System.Windows.Forms.CheckBox insert_ignore;
        private System.Windows.Forms.CheckBox lock_tables;
        private System.Windows.Forms.TextBox max_allowed_packet;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox events;
        private System.Windows.Forms.CheckBox quote_names;
        private System.Windows.Forms.CheckBox replace;
        private System.Windows.Forms.CheckBox no_create_info;
        private System.Windows.Forms.BindingSource mySqlDbExportOptionsBindingSource;
        private System.Windows.Forms.CheckBox no_data;
        private System.Windows.Forms.Label label2;
    }
}
