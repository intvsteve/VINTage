// <copyright file="VisualRelayCommand.Mono.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

using System;
using MonoMac.AppKit;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Non-WPF portion of VisualRelayCommand.
    /// </summary>
    public partial class VisualRelayCommand
    {
        partial void Initialize()
        {
            CommandManager.RequerySuggested += HandleRequerySuggested;
        }

        private string GetMenuItemName()
        {
            string menuItemName = null;
            if (MenuItem != null)
            {
                menuItemName = MenuItem.Title;
            }
            return menuItemName;
        }

        private void SetMenuItemName(string menuItemName)
        {
            if (MenuItem != null)
            {
                MenuItem.Title = menuItemName;
            }
        }

        private void HandleRequerySuggested(object sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
#if false
            bool canExecute = CanExecute(null);
            if (MenuItem != null)
            {
                MenuItem.Enabled = canExecute;
            }
            var toolbarItem = Visual as NSToolbarItem;
            if (toolbarItem != null)
            {
                toolbarItem.Enabled = canExecute;
            }
            var control = Visual as NSControl;
            if (control != null)
            {
                if (control is NSSegmentedControl)
                {
                    // locate segment via hash of command
                    var segmentedControl = (NSSegmentedControl)control;
                    var tag = GetHashCode();
                    for (int i = 0; i < segmentedControl.SegmentCount; ++i)
                    {
                        if (segmentedControl.Cell.GetTag(i) == tag)
                        {
                            segmentedControl.Cell.SetEnabled(canExecute, i);
                            break;
                        }
                    }
                }
                else
                {
                    control.Enabled = canExecute;
                }
            }
#endif
        }
    }
}
