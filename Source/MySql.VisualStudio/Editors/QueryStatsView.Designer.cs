// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
//
// MySQL for Visual Studio is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most
// MySQL Connectors. There are special exceptions to the terms and
// conditions of the GPLv2 as it is applied to this software, see the
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

namespace MySql.Data.VisualStudio.Editors
{
  partial class QueryStatsView
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
      System.Windows.Forms.Label lblSortRange;
      this.lblThreadIdVal = new System.Windows.Forms.Label();
      this.lblEventIdVal = new System.Windows.Forms.Label();
      this.lblIndexUsedVal = new System.Windows.Forms.Label();
      this.lblSortScanVal = new System.Windows.Forms.Label();
      this.lblSortRangeVal = new System.Windows.Forms.Label();
      this.lblSortMergePassesVal = new System.Windows.Forms.Label();
      this.lblSortRowsVal = new System.Windows.Forms.Label();
      this.lblSelectRangeVal = new System.Windows.Forms.Label();
      this.lblSelectRangeCheckVal = new System.Windows.Forms.Label();
      this.lblSelectFullRangeJoinVal = new System.Windows.Forms.Label();
      this.lblSelectFullJoinVal = new System.Windows.Forms.Label();
      this.lblSelectScanVal = new System.Windows.Forms.Label();
      this.lblTempTablesVal = new System.Windows.Forms.Label();
      this.lblTempDiskTablesVal = new System.Windows.Forms.Label();
      this.lblRowsExaminedVal = new System.Windows.Forms.Label();
      this.lblRowsSentVal = new System.Windows.Forms.Label();
      this.lblRowsAffectedVal = new System.Windows.Forms.Label();
      this.lblWarningsVal = new System.Windows.Forms.Label();
      this.lblErrorsVal = new System.Windows.Forms.Label();
      this.lblLockTimeVal = new System.Windows.Forms.Label();
      this.lblServerExecutionTimeVal = new System.Windows.Forms.Label();
      this.lblThread = new System.Windows.Forms.Label();
      this.lblEvent = new System.Windows.Forms.Label();
      this.lblOtherInfo = new System.Windows.Forms.Label();
      this.lblIdxUsed = new System.Windows.Forms.Label();
      this.lblIdx = new System.Windows.Forms.Label();
      this.lblSortTable = new System.Windows.Forms.Label();
      this.lblSortMerge = new System.Windows.Forms.Label();
      this.lblSortedRows = new System.Windows.Forms.Label();
      this.lblSorting = new System.Windows.Forms.Label();
      this.lblJoinRange = new System.Windows.Forms.Label();
      this.lblJoinRangeCheck = new System.Windows.Forms.Label();
      this.lblJoinRangeSearch = new System.Windows.Forms.Label();
      this.lblJoinTableScan = new System.Windows.Forms.Label();
      this.lblTableScan = new System.Windows.Forms.Label();
      this.lblJoinTypes = new System.Windows.Forms.Label();
      this.lblTempTables = new System.Windows.Forms.Label();
      this.lblTempDiskTables = new System.Windows.Forms.Label();
      this.lblTmpTables = new System.Windows.Forms.Label();
      this.lblRowsExamined = new System.Windows.Forms.Label();
      this.lblRowsSent = new System.Windows.Forms.Label();
      this.lblRowAffected = new System.Windows.Forms.Label();
      this.lblRowProc = new System.Windows.Forms.Label();
      this.lblWarningTitle = new System.Windows.Forms.Label();
      this.lblHadErrors = new System.Windows.Forms.Label();
      this.lblErrorTitle = new System.Windows.Forms.Label();
      this.lblTableLock = new System.Windows.Forms.Label();
      this.lblExecTime = new System.Windows.Forms.Label();
      this.lblServer = new System.Windows.Forms.Label();
      this.bsQueryStatsData = new System.Windows.Forms.BindingSource(this.components);
      lblSortRange = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.bsQueryStatsData)).BeginInit();
      this.SuspendLayout();
      // 
      // lblSortRange
      // 
      lblSortRange.AutoSize = true;
      lblSortRange.Location = new System.Drawing.Point(344, 216);
      lblSortRange.Name = "lblSortRange";
      lblSortRange.Size = new System.Drawing.Size(91, 13);
      lblSortRange.TabIndex = 72;
      lblSortRange.Text = "Sorts with ranges:";
      // 
      // lblThreadIdVal
      // 
      this.lblThreadIdVal.AutoSize = true;
      this.lblThreadIdVal.Location = new System.Drawing.Point(409, 351);
      this.lblThreadIdVal.Name = "lblThreadIdVal";
      this.lblThreadIdVal.Size = new System.Drawing.Size(0, 13);
      this.lblThreadIdVal.TabIndex = 99;
      // 
      // lblEventIdVal
      // 
      this.lblEventIdVal.AutoSize = true;
      this.lblEventIdVal.Location = new System.Drawing.Point(403, 333);
      this.lblEventIdVal.Name = "lblEventIdVal";
      this.lblEventIdVal.Size = new System.Drawing.Size(0, 13);
      this.lblEventIdVal.TabIndex = 98;
      // 
      // lblIndexUsedVal
      // 
      this.lblIndexUsedVal.AutoSize = true;
      this.lblIndexUsedVal.Location = new System.Drawing.Point(420, 277);
      this.lblIndexUsedVal.Name = "lblIndexUsedVal";
      this.lblIndexUsedVal.Size = new System.Drawing.Size(0, 13);
      this.lblIndexUsedVal.TabIndex = 97;
      // 
      // lblSortScanVal
      // 
      this.lblSortScanVal.AutoSize = true;
      this.lblSortScanVal.Location = new System.Drawing.Point(463, 234);
      this.lblSortScanVal.Name = "lblSortScanVal";
      this.lblSortScanVal.Size = new System.Drawing.Size(0, 13);
      this.lblSortScanVal.TabIndex = 96;
      // 
      // lblSortRangeVal
      // 
      this.lblSortRangeVal.AutoSize = true;
      this.lblSortRangeVal.Location = new System.Drawing.Point(441, 216);
      this.lblSortRangeVal.Name = "lblSortRangeVal";
      this.lblSortRangeVal.Size = new System.Drawing.Size(0, 13);
      this.lblSortRangeVal.TabIndex = 95;
      // 
      // lblSortMergePassesVal
      // 
      this.lblSortMergePassesVal.AutoSize = true;
      this.lblSortMergePassesVal.Location = new System.Drawing.Point(448, 198);
      this.lblSortMergePassesVal.Name = "lblSortMergePassesVal";
      this.lblSortMergePassesVal.Size = new System.Drawing.Size(0, 13);
      this.lblSortMergePassesVal.TabIndex = 94;
      // 
      // lblSortRowsVal
      // 
      this.lblSortRowsVal.AutoSize = true;
      this.lblSortRowsVal.Location = new System.Drawing.Point(421, 180);
      this.lblSortRowsVal.Name = "lblSortRowsVal";
      this.lblSortRowsVal.Size = new System.Drawing.Size(0, 13);
      this.lblSortRowsVal.TabIndex = 93;
      // 
      // lblSelectRangeVal
      // 
      this.lblSelectRangeVal.AutoSize = true;
      this.lblSelectRangeVal.Location = new System.Drawing.Point(431, 105);
      this.lblSelectRangeVal.Name = "lblSelectRangeVal";
      this.lblSelectRangeVal.Size = new System.Drawing.Size(0, 13);
      this.lblSelectRangeVal.TabIndex = 92;
      // 
      // lblSelectRangeCheckVal
      // 
      this.lblSelectRangeCheckVal.AutoSize = true;
      this.lblSelectRangeCheckVal.Location = new System.Drawing.Point(463, 85);
      this.lblSelectRangeCheckVal.Name = "lblSelectRangeCheckVal";
      this.lblSelectRangeCheckVal.Size = new System.Drawing.Size(0, 13);
      this.lblSelectRangeCheckVal.TabIndex = 91;
      // 
      // lblSelectFullRangeJoinVal
      // 
      this.lblSelectFullRangeJoinVal.AutoSize = true;
      this.lblSelectFullRangeJoinVal.Location = new System.Drawing.Point(466, 68);
      this.lblSelectFullRangeJoinVal.Name = "lblSelectFullRangeJoinVal";
      this.lblSelectFullRangeJoinVal.Size = new System.Drawing.Size(0, 13);
      this.lblSelectFullRangeJoinVal.TabIndex = 90;
      // 
      // lblSelectFullJoinVal
      // 
      this.lblSelectFullJoinVal.AutoSize = true;
      this.lblSelectFullJoinVal.Location = new System.Drawing.Point(458, 50);
      this.lblSelectFullJoinVal.Name = "lblSelectFullJoinVal";
      this.lblSelectFullJoinVal.Size = new System.Drawing.Size(0, 13);
      this.lblSelectFullJoinVal.TabIndex = 89;
      // 
      // lblSelectScanVal
      // 
      this.lblSelectScanVal.AutoSize = true;
      this.lblSelectScanVal.Location = new System.Drawing.Point(422, 32);
      this.lblSelectScanVal.Name = "lblSelectScanVal";
      this.lblSelectScanVal.Size = new System.Drawing.Size(0, 13);
      this.lblSelectScanVal.TabIndex = 88;
      // 
      // lblTempTablesVal
      // 
      this.lblTempTablesVal.AutoSize = true;
      this.lblTempTablesVal.Location = new System.Drawing.Point(163, 295);
      this.lblTempTablesVal.Name = "lblTempTablesVal";
      this.lblTempTablesVal.Size = new System.Drawing.Size(0, 13);
      this.lblTempTablesVal.TabIndex = 87;
      // 
      // lblTempDiskTablesVal
      // 
      this.lblTempDiskTablesVal.AutoSize = true;
      this.lblTempDiskTablesVal.Location = new System.Drawing.Point(185, 277);
      this.lblTempDiskTablesVal.Name = "lblTempDiskTablesVal";
      this.lblTempDiskTablesVal.Size = new System.Drawing.Size(0, 13);
      this.lblTempDiskTablesVal.TabIndex = 86;
      // 
      // lblRowsExaminedVal
      // 
      this.lblRowsExaminedVal.AutoSize = true;
      this.lblRowsExaminedVal.Location = new System.Drawing.Point(119, 216);
      this.lblRowsExaminedVal.Name = "lblRowsExaminedVal";
      this.lblRowsExaminedVal.Size = new System.Drawing.Size(0, 13);
      this.lblRowsExaminedVal.TabIndex = 85;
      // 
      // lblRowsSentVal
      // 
      this.lblRowsSentVal.AutoSize = true;
      this.lblRowsSentVal.Location = new System.Drawing.Point(151, 198);
      this.lblRowsSentVal.Name = "lblRowsSentVal";
      this.lblRowsSentVal.Size = new System.Drawing.Size(0, 13);
      this.lblRowsSentVal.TabIndex = 84;
      // 
      // lblRowsAffectedVal
      // 
      this.lblRowsAffectedVal.AutoSize = true;
      this.lblRowsAffectedVal.Location = new System.Drawing.Point(113, 180);
      this.lblRowsAffectedVal.Name = "lblRowsAffectedVal";
      this.lblRowsAffectedVal.Size = new System.Drawing.Size(0, 13);
      this.lblRowsAffectedVal.TabIndex = 83;
      // 
      // lblWarningsVal
      // 
      this.lblWarningsVal.AutoSize = true;
      this.lblWarningsVal.Location = new System.Drawing.Point(88, 123);
      this.lblWarningsVal.Name = "lblWarningsVal";
      this.lblWarningsVal.Size = new System.Drawing.Size(0, 13);
      this.lblWarningsVal.TabIndex = 82;
      // 
      // lblErrorsVal
      // 
      this.lblErrorsVal.AutoSize = true;
      this.lblErrorsVal.Location = new System.Drawing.Point(93, 105);
      this.lblErrorsVal.Name = "lblErrorsVal";
      this.lblErrorsVal.Size = new System.Drawing.Size(0, 13);
      this.lblErrorsVal.TabIndex = 81;
      // 
      // lblLockTimeVal
      // 
      this.lblLockTimeVal.AutoSize = true;
      this.lblLockTimeVal.Location = new System.Drawing.Point(137, 50);
      this.lblLockTimeVal.Name = "lblLockTimeVal";
      this.lblLockTimeVal.Size = new System.Drawing.Size(0, 13);
      this.lblLockTimeVal.TabIndex = 80;
      // 
      // lblServerExecutionTimeVal
      // 
      this.lblServerExecutionTimeVal.AutoSize = true;
      this.lblServerExecutionTimeVal.Location = new System.Drawing.Point(112, 32);
      this.lblServerExecutionTimeVal.Name = "lblServerExecutionTimeVal";
      this.lblServerExecutionTimeVal.Size = new System.Drawing.Size(0, 13);
      this.lblServerExecutionTimeVal.TabIndex = 79;
      // 
      // lblThread
      // 
      this.lblThread.AutoSize = true;
      this.lblThread.Location = new System.Drawing.Point(347, 351);
      this.lblThread.Name = "lblThread";
      this.lblThread.Size = new System.Drawing.Size(56, 13);
      this.lblThread.TabIndex = 78;
      this.lblThread.Text = "Thread Id:";
      // 
      // lblEvent
      // 
      this.lblEvent.AutoSize = true;
      this.lblEvent.Location = new System.Drawing.Point(347, 333);
      this.lblEvent.Name = "lblEvent";
      this.lblEvent.Size = new System.Drawing.Size(50, 13);
      this.lblEvent.TabIndex = 77;
      this.lblEvent.Text = "Event Id:";
      // 
      // lblOtherInfo
      // 
      this.lblOtherInfo.AutoSize = true;
      this.lblOtherInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblOtherInfo.Location = new System.Drawing.Point(333, 313);
      this.lblOtherInfo.Name = "lblOtherInfo";
      this.lblOtherInfo.Size = new System.Drawing.Size(68, 13);
      this.lblOtherInfo.TabIndex = 76;
      this.lblOtherInfo.Text = "Other Info:";
      // 
      // lblIdxUsed
      // 
      this.lblIdxUsed.AutoSize = true;
      this.lblIdxUsed.Location = new System.Drawing.Point(344, 277);
      this.lblIdxUsed.Name = "lblIdxUsed";
      this.lblIdxUsed.Size = new System.Drawing.Size(70, 13);
      this.lblIdxUsed.TabIndex = 75;
      this.lblIdxUsed.Text = "Index Used?:";
      // 
      // lblIdx
      // 
      this.lblIdx.AutoSize = true;
      this.lblIdx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblIdx.Location = new System.Drawing.Point(330, 257);
      this.lblIdx.Name = "lblIdx";
      this.lblIdx.Size = new System.Drawing.Size(82, 13);
      this.lblIdx.TabIndex = 74;
      this.lblIdx.Text = "Index Usage:";
      // 
      // lblSortTable
      // 
      this.lblSortTable.AutoSize = true;
      this.lblSortTable.Location = new System.Drawing.Point(344, 234);
      this.lblSortTable.Name = "lblSortTable";
      this.lblSortTable.Size = new System.Drawing.Size(113, 13);
      this.lblSortTable.TabIndex = 73;
      this.lblSortTable.Text = "Sorts with table scans:";
      // 
      // lblSortMerge
      // 
      this.lblSortMerge.AutoSize = true;
      this.lblSortMerge.Location = new System.Drawing.Point(344, 198);
      this.lblSortMerge.Name = "lblSortMerge";
      this.lblSortMerge.Size = new System.Drawing.Size(98, 13);
      this.lblSortMerge.TabIndex = 71;
      this.lblSortMerge.Text = "Sort merge passed:";
      // 
      // lblSortedRows
      // 
      this.lblSortedRows.AutoSize = true;
      this.lblSortedRows.Location = new System.Drawing.Point(344, 180);
      this.lblSortedRows.Name = "lblSortedRows";
      this.lblSortedRows.Size = new System.Drawing.Size(71, 13);
      this.lblSortedRows.TabIndex = 70;
      this.lblSortedRows.Text = "Sorted Rows:";
      // 
      // lblSorting
      // 
      this.lblSorting.AutoSize = true;
      this.lblSorting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSorting.Location = new System.Drawing.Point(330, 160);
      this.lblSorting.Name = "lblSorting";
      this.lblSorting.Size = new System.Drawing.Size(51, 13);
      this.lblSorting.TabIndex = 69;
      this.lblSorting.Text = "Sorting:";
      // 
      // lblJoinRange
      // 
      this.lblJoinRange.AutoSize = true;
      this.lblJoinRange.Location = new System.Drawing.Point(333, 104);
      this.lblJoinRange.Name = "lblJoinRange";
      this.lblJoinRange.Size = new System.Drawing.Size(92, 13);
      this.lblJoinRange.TabIndex = 68;
      this.lblJoinRange.Text = "Joins using range:";
      // 
      // lblJoinRangeCheck
      // 
      this.lblJoinRangeCheck.AutoSize = true;
      this.lblJoinRangeCheck.Location = new System.Drawing.Point(333, 86);
      this.lblJoinRangeCheck.Name = "lblJoinRangeCheck";
      this.lblJoinRangeCheck.Size = new System.Drawing.Size(124, 13);
      this.lblJoinRangeCheck.TabIndex = 67;
      this.lblJoinRangeCheck.Text = "Joins with range checks:";
      // 
      // lblJoinRangeSearch
      // 
      this.lblJoinRangeSearch.AutoSize = true;
      this.lblJoinRangeSearch.Location = new System.Drawing.Point(333, 68);
      this.lblJoinRangeSearch.Name = "lblJoinRangeSearch";
      this.lblJoinRangeSearch.Size = new System.Drawing.Size(127, 13);
      this.lblJoinRangeSearch.TabIndex = 66;
      this.lblJoinRangeSearch.Text = "Joins using range search:";
      // 
      // lblJoinTableScan
      // 
      this.lblJoinTableScan.AutoSize = true;
      this.lblJoinTableScan.Location = new System.Drawing.Point(333, 50);
      this.lblJoinTableScan.Name = "lblJoinTableScan";
      this.lblJoinTableScan.Size = new System.Drawing.Size(119, 13);
      this.lblJoinTableScan.TabIndex = 65;
      this.lblJoinTableScan.Text = "Joins using table scans:";
      // 
      // lblTableScan
      // 
      this.lblTableScan.AutoSize = true;
      this.lblTableScan.Location = new System.Drawing.Point(333, 32);
      this.lblTableScan.Name = "lblTableScan";
      this.lblTableScan.Size = new System.Drawing.Size(83, 13);
      this.lblTableScan.TabIndex = 64;
      this.lblTableScan.Text = "Full table scans:";
      // 
      // lblJoinTypes
      // 
      this.lblJoinTypes.AutoSize = true;
      this.lblJoinTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblJoinTypes.Location = new System.Drawing.Point(319, 12);
      this.lblJoinTypes.Name = "lblJoinTypes";
      this.lblJoinTypes.Size = new System.Drawing.Size(95, 13);
      this.lblJoinTypes.TabIndex = 63;
      this.lblJoinTypes.Text = "Joins Per Type:";
      // 
      // lblTempTables
      // 
      this.lblTempTables.AutoSize = true;
      this.lblTempTables.Location = new System.Drawing.Point(27, 295);
      this.lblTempTables.Name = "lblTempTables";
      this.lblTempTables.Size = new System.Drawing.Size(130, 13);
      this.lblTempTables.TabIndex = 62;
      this.lblTempTables.Text = "Temporary tables created:";
      // 
      // lblTempDiskTables
      // 
      this.lblTempDiskTables.AutoSize = true;
      this.lblTempDiskTables.Location = new System.Drawing.Point(27, 277);
      this.lblTempDiskTables.Name = "lblTempDiskTables";
      this.lblTempDiskTables.Size = new System.Drawing.Size(152, 13);
      this.lblTempDiskTables.TabIndex = 61;
      this.lblTempDiskTables.Text = "Temporary disk tables created:";
      // 
      // lblTmpTables
      // 
      this.lblTmpTables.AutoSize = true;
      this.lblTmpTables.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblTmpTables.Location = new System.Drawing.Point(13, 257);
      this.lblTmpTables.Name = "lblTmpTables";
      this.lblTmpTables.Size = new System.Drawing.Size(112, 13);
      this.lblTmpTables.TabIndex = 60;
      this.lblTmpTables.Text = "Temporary Tables:";
      // 
      // lblRowsExamined
      // 
      this.lblRowsExamined.AutoSize = true;
      this.lblRowsExamined.Location = new System.Drawing.Point(27, 216);
      this.lblRowsExamined.Name = "lblRowsExamined";
      this.lblRowsExamined.Size = new System.Drawing.Size(86, 13);
      this.lblRowsExamined.TabIndex = 59;
      this.lblRowsExamined.Text = "Rows Examined:";
      // 
      // lblRowsSent
      // 
      this.lblRowsSent.AutoSize = true;
      this.lblRowsSent.Location = new System.Drawing.Point(27, 198);
      this.lblRowsSent.Name = "lblRowsSent";
      this.lblRowsSent.Size = new System.Drawing.Size(118, 13);
      this.lblRowsSent.TabIndex = 58;
      this.lblRowsSent.Text = "Rows sent to the client:";
      // 
      // lblRowAffected
      // 
      this.lblRowAffected.AutoSize = true;
      this.lblRowAffected.Location = new System.Drawing.Point(27, 180);
      this.lblRowAffected.Name = "lblRowAffected";
      this.lblRowAffected.Size = new System.Drawing.Size(80, 13);
      this.lblRowAffected.TabIndex = 57;
      this.lblRowAffected.Text = "Rows Affected:";
      // 
      // lblRowProc
      // 
      this.lblRowProc.AutoSize = true;
      this.lblRowProc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblRowProc.Location = new System.Drawing.Point(13, 160);
      this.lblRowProc.Name = "lblRowProc";
      this.lblRowProc.Size = new System.Drawing.Size(105, 13);
      this.lblRowProc.TabIndex = 56;
      this.lblRowProc.Text = "Rows Processed:";
      // 
      // lblWarningTitle
      // 
      this.lblWarningTitle.AutoSize = true;
      this.lblWarningTitle.Location = new System.Drawing.Point(27, 123);
      this.lblWarningTitle.Name = "lblWarningTitle";
      this.lblWarningTitle.Size = new System.Drawing.Size(55, 13);
      this.lblWarningTitle.TabIndex = 55;
      this.lblWarningTitle.Text = "Warnings:";
      // 
      // lblHadErrors
      // 
      this.lblHadErrors.AutoSize = true;
      this.lblHadErrors.Location = new System.Drawing.Point(27, 105);
      this.lblHadErrors.Name = "lblHadErrors";
      this.lblHadErrors.Size = new System.Drawing.Size(60, 13);
      this.lblHadErrors.TabIndex = 54;
      this.lblHadErrors.Text = "Had Errors:";
      // 
      // lblErrorTitle
      // 
      this.lblErrorTitle.AutoSize = true;
      this.lblErrorTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblErrorTitle.Location = new System.Drawing.Point(13, 85);
      this.lblErrorTitle.Name = "lblErrorTitle";
      this.lblErrorTitle.Size = new System.Drawing.Size(44, 13);
      this.lblErrorTitle.TabIndex = 53;
      this.lblErrorTitle.Text = "Errors:";
      // 
      // lblTableLock
      // 
      this.lblTableLock.AutoSize = true;
      this.lblTableLock.Location = new System.Drawing.Point(27, 50);
      this.lblTableLock.Name = "lblTableLock";
      this.lblTableLock.Size = new System.Drawing.Size(104, 13);
      this.lblTableLock.TabIndex = 52;
      this.lblTableLock.Text = "Table lock wait time:";
      // 
      // lblExecTime
      // 
      this.lblExecTime.AutoSize = true;
      this.lblExecTime.Location = new System.Drawing.Point(27, 32);
      this.lblExecTime.Name = "lblExecTime";
      this.lblExecTime.Size = new System.Drawing.Size(79, 13);
      this.lblExecTime.TabIndex = 51;
      this.lblExecTime.Text = "Execution time:";
      // 
      // lblServer
      // 
      this.lblServer.AutoSize = true;
      this.lblServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblServer.Location = new System.Drawing.Point(13, 12);
      this.lblServer.Name = "lblServer";
      this.lblServer.Size = new System.Drawing.Size(209, 13);
      this.lblServer.TabIndex = 50;
      this.lblServer.Text = "Timing (as measured by the server):";
      // 
      // QueryStatsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.lblThreadIdVal);
      this.Controls.Add(this.lblEventIdVal);
      this.Controls.Add(this.lblIndexUsedVal);
      this.Controls.Add(this.lblSortScanVal);
      this.Controls.Add(this.lblSortRangeVal);
      this.Controls.Add(this.lblSortMergePassesVal);
      this.Controls.Add(this.lblSortRowsVal);
      this.Controls.Add(this.lblSelectRangeVal);
      this.Controls.Add(this.lblSelectRangeCheckVal);
      this.Controls.Add(this.lblSelectFullRangeJoinVal);
      this.Controls.Add(this.lblSelectFullJoinVal);
      this.Controls.Add(this.lblSelectScanVal);
      this.Controls.Add(this.lblTempTablesVal);
      this.Controls.Add(this.lblTempDiskTablesVal);
      this.Controls.Add(this.lblRowsExaminedVal);
      this.Controls.Add(this.lblRowsSentVal);
      this.Controls.Add(this.lblRowsAffectedVal);
      this.Controls.Add(this.lblWarningsVal);
      this.Controls.Add(this.lblErrorsVal);
      this.Controls.Add(this.lblLockTimeVal);
      this.Controls.Add(this.lblServerExecutionTimeVal);
      this.Controls.Add(this.lblThread);
      this.Controls.Add(this.lblEvent);
      this.Controls.Add(this.lblOtherInfo);
      this.Controls.Add(this.lblIdxUsed);
      this.Controls.Add(this.lblIdx);
      this.Controls.Add(this.lblSortTable);
      this.Controls.Add(lblSortRange);
      this.Controls.Add(this.lblSortMerge);
      this.Controls.Add(this.lblSortedRows);
      this.Controls.Add(this.lblSorting);
      this.Controls.Add(this.lblJoinRange);
      this.Controls.Add(this.lblJoinRangeCheck);
      this.Controls.Add(this.lblJoinRangeSearch);
      this.Controls.Add(this.lblJoinTableScan);
      this.Controls.Add(this.lblTableScan);
      this.Controls.Add(this.lblJoinTypes);
      this.Controls.Add(this.lblTempTables);
      this.Controls.Add(this.lblTempDiskTables);
      this.Controls.Add(this.lblTmpTables);
      this.Controls.Add(this.lblRowsExamined);
      this.Controls.Add(this.lblRowsSent);
      this.Controls.Add(this.lblRowAffected);
      this.Controls.Add(this.lblRowProc);
      this.Controls.Add(this.lblWarningTitle);
      this.Controls.Add(this.lblHadErrors);
      this.Controls.Add(this.lblErrorTitle);
      this.Controls.Add(this.lblTableLock);
      this.Controls.Add(this.lblExecTime);
      this.Controls.Add(this.lblServer);
      this.Name = "QueryStatsView";
      this.Size = new System.Drawing.Size(600, 500);
      ((System.ComponentModel.ISupportInitialize)(this.bsQueryStatsData)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblThreadIdVal;
    private System.Windows.Forms.Label lblEventIdVal;
    private System.Windows.Forms.Label lblIndexUsedVal;
    private System.Windows.Forms.Label lblSortScanVal;
    private System.Windows.Forms.Label lblSortRangeVal;
    private System.Windows.Forms.Label lblSortMergePassesVal;
    private System.Windows.Forms.Label lblSortRowsVal;
    private System.Windows.Forms.Label lblSelectRangeVal;
    private System.Windows.Forms.Label lblSelectRangeCheckVal;
    private System.Windows.Forms.Label lblSelectFullRangeJoinVal;
    private System.Windows.Forms.Label lblSelectFullJoinVal;
    private System.Windows.Forms.Label lblSelectScanVal;
    private System.Windows.Forms.Label lblTempTablesVal;
    private System.Windows.Forms.Label lblTempDiskTablesVal;
    private System.Windows.Forms.Label lblRowsExaminedVal;
    private System.Windows.Forms.Label lblRowsSentVal;
    private System.Windows.Forms.Label lblRowsAffectedVal;
    private System.Windows.Forms.Label lblWarningsVal;
    private System.Windows.Forms.Label lblErrorsVal;
    private System.Windows.Forms.Label lblLockTimeVal;
    private System.Windows.Forms.Label lblServerExecutionTimeVal;
    private System.Windows.Forms.Label lblThread;
    private System.Windows.Forms.Label lblEvent;
    private System.Windows.Forms.Label lblOtherInfo;
    private System.Windows.Forms.Label lblIdxUsed;
    private System.Windows.Forms.Label lblIdx;
    private System.Windows.Forms.Label lblSortTable;
    private System.Windows.Forms.Label lblSortMerge;
    private System.Windows.Forms.Label lblSortedRows;
    private System.Windows.Forms.Label lblSorting;
    private System.Windows.Forms.Label lblJoinRange;
    private System.Windows.Forms.Label lblJoinRangeCheck;
    private System.Windows.Forms.Label lblJoinRangeSearch;
    private System.Windows.Forms.Label lblJoinTableScan;
    private System.Windows.Forms.Label lblTableScan;
    private System.Windows.Forms.Label lblJoinTypes;
    private System.Windows.Forms.Label lblTempTables;
    private System.Windows.Forms.Label lblTempDiskTables;
    private System.Windows.Forms.Label lblTmpTables;
    private System.Windows.Forms.Label lblRowsExamined;
    private System.Windows.Forms.Label lblRowsSent;
    private System.Windows.Forms.Label lblRowAffected;
    private System.Windows.Forms.Label lblRowProc;
    private System.Windows.Forms.Label lblWarningTitle;
    private System.Windows.Forms.Label lblHadErrors;
    private System.Windows.Forms.Label lblErrorTitle;
    private System.Windows.Forms.Label lblTableLock;
    private System.Windows.Forms.Label lblExecTime;
    private System.Windows.Forms.Label lblServer;
    private System.Windows.Forms.BindingSource bsQueryStatsData;

  }
}
