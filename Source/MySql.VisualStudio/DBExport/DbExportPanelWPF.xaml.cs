// Copyright (c) 2021, Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL for Visual Studio, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace MySql.Data.VisualStudio.DBExport
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class DbExportPanelWPF : Window
  {
    /// <summary>
    /// The DB Export Panel control shown in this WPF window.
    /// </summary>
    private dbExportPanel _dbExportPanel;

    /// <summary>
    /// Timer used to refresh the scroll bars.
    /// </summary>
    private DispatcherTimer _dispatcherTimer;

    /// <summary>
    /// The object hosting the Winforms DB Export Panel control.
    /// </summary>
    private WindowsFormsHost _host;

    public DbExportPanelWPF()
    {
      InitializeComponent();
      _dbExportPanel = new dbExportPanel();
      _dbExportPanel.MouseEnter += _dbExportPanel_MouseEnter;
      _dispatcherTimer = new DispatcherTimer();
      _dispatcherTimer.Tick += new EventHandler(_dbExportPanel_MouseEnter);
      _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
      _dispatcherTimer.Start();
    }

    /// <summary>
    /// Gets the Winforms DB Export Panel control contained in this WPF window.
    /// </summary>
    public dbExportPanel DbExportPanel => _dbExportPanel;

    /// <summary>
    /// Updates the size of the scrollviewer control based on this window size. 
    /// </summary>
    private void ResizeScrollViewer()
    {
      ScrollContainer.Width = this.Width;
      ScrollContainer.Height = this.Height;
    }

    /// <summary>
    /// Event handler for mouse enter events of the Winforms DB Export Panel control.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void _dbExportPanel_MouseEnter(object sender, EventArgs e)
    {
      ResizeScrollViewer();
      if (_dispatcherTimer.IsEnabled)
      {
        _dispatcherTimer.Stop();
      }
    }

    /// <summary>
    /// Event handler triggered when the inner grid gets focus.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void InnerGrid_GotFocus(object sender, RoutedEventArgs e)
    {
      ResizeScrollViewer();
    }

    /// <summary>
    /// Event handler triggered when the main grid container is loaded.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    private void InnerGrid_Loaded(object sender, RoutedEventArgs e)
    {
      _host = new WindowsFormsHost();
      _host.Child = _dbExportPanel;
      InnerGrid.Children.Add(_host);
    }
  }
}
