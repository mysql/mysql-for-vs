// Copyright (C) 2006-2007 MySQL AB
//
// This file is part of MySQL Tools for Visual Studio.
// MySQL Tools for Visual Studio is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public 
// License version 2.1 as published by the Free Software Foundation
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA using System;

/*
 * This file contains implementation of UI helper utility.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using MySql.Data.VisualStudio.Properties;

namespace MySql.Data.VisualStudio.Utils
{
    /// <summary>
    /// This class is used to display user interaction.
    /// </summary>
    static class UIHelper
    {
        #region Public methods
        /// <summary>
        /// Shows simple string message with only OK button.
        /// </summary>
        /// <param name="text">Message to show.</param>
        public static void ShowMessage(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            uiService.ShowMessage(text);
        }

        /// <summary>
        /// Show message with custom buttons
        /// </summary>
        /// <param name="text">Message to show.</param>
        /// <param name="buttons">Buttons to show.</param>
        /// <returns>Selected button.</returns>
        public static DialogResult ShowMessage(string text, MessageBoxButtons buttons)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            return uiService.ShowMessage(text, null, buttons);
        }

        /// <summary>
        /// Shows warning string message.
        /// </summary>
        /// <param name="text">Message to show.</param>
        public static void ShowWarning(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            ShowWarning(text, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Shows warning string message with customizable buttons.
        /// </summary>
        /// <param name="text">Message to show.</param>
        /// <param name="buttons">Buttons to show.</param>
        /// <returns>Selected button.</returns>
        public static DialogResult ShowWarning(string text, MessageBoxButtons buttons)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            return ShowMessage(text, buttons);
        }

        /// <summary>
        /// Shows error message.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        public static void ShowError(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");
            uiService.ShowError(ex);
        }

        /// <summary>
        /// Shows error message.
        /// </summary>
        /// <param name="message">String error message.</param>
        public static void ShowError(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            uiService.ShowError(message);
        }

        /// <summary>
        /// Shows error message.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        /// <param name="message">String error message.</param>
        public static void ShowError(Exception ex, string message)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");
            if (message == null)
                throw new ArgumentNullException("message");
            uiService.ShowError(ex, message);
        }

        /// <summary>
        /// Shows custom dialog.
        /// </summary>
        /// <param name="dialog">Form instance with dialog to show.</param>
        /// <returns>Returns dialog result from dialog execution.</returns>
        public static DialogResult ShowDialog(Form dialog)
        {
            if (dialog == null)
                throw new ArgumentNullException("dialog");

            return uiService.ShowDialog(dialog);
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Refernce to IUIService implementation
        /// </summary>
        private static readonly IUIService uiService;


        /// <summary>
        /// Initializes uiService reference for future use
        /// </summary>
        static UIHelper()
        {
            // Get UI service object
            uiService = Package.GetGlobalService(typeof(IUIService)) as IUIService;
            Debug.Assert(uiService != null, "Unable to get UI service!");
            if (uiService == null)
                throw new Exception(Resources.Error_UnableToGetUIService);
        }
        #endregion
    }
}
