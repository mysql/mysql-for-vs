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
 * This file contains definition of command handler interface.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Data;

namespace MySql.Data.VisualStudio.Commands
{
    /// <summary>
    /// This interface describes functionality of command handler object. 
    /// It introduces methods to determine commend visibility and customized text, 
    /// method to execute command and properties to get command identifiers.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes command for the given set of hierarchy items
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="items">Array with item’s identifiers.</param>
        object[] Execute(ServerExplorerFacade hierarchy, int[] items);

        /// <summary>
        /// Used to check if command should be visible for given items
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Returns true if command should be shown and false otherwise</returns>
        bool GetIsVisible(ServerExplorerFacade hierarchy, int[] items);

        /// <summary>
        /// Used to get customized text for the command
        /// </summary>
        /// <param name="hierarchy">Server explorer facade object to be used for Server Explorer hierarchy interaction.</param>
        /// <param name="items">Array with item’s identifiers.</param>
        /// <returns>Returns customized text for the command</returns>
        string GetText(ServerExplorerFacade hierarchy, int[] items);
    }
}
