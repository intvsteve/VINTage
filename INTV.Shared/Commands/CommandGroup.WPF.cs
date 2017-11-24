// <copyright file="CommandGroup.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

using INTV.Shared.ComponentModel;

using OSCommandVisual = System.Windows.UIElement;
using OSMenuItem = System.Windows.Controls.Control;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Provides WPF-specific implementation of the ICommandGroup interface.
    /// </summary>
    public abstract partial class CommandGroup
    {
        #region ICommandGroup

        /// <inheritdoc />
        public virtual OSCommandVisual CreateVisualForCommand(ICommand command)
        {
            OSCommandVisual visual = null;
            var visualCommand = command as VisualRelayCommand;
            if ((visualCommand != null) && visualCommand.Visual.IsEmpty)
            {
                visual = visualCommand.CreateVisualForCommand(visualCommand.VisualParent != null);
            }
            return visual;
        }

        /// <inheritdoc />
        public virtual OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            OSMenuItem menuItem = null;
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand != null)
            {
                menuItem = visualCommand.CreateMenuItemForCommand(visualCommand.MenuParent != null);
            }
            return menuItem;
        }

        #endregion // ICommandGroup

        private object OSGetContextForCommand(object target, ICommand command, object context)
        {
            return context;
        }

        private void InitializeMenuItem(VisualRelayCommand command, object target, object context)
        {
            var menuItem = command.MenuItem.NativeMenuItem;
            if (menuItem != null)
            {
                menuItem.CommandParameter = GetContextForCommand(target, command, context);
            }
        }
    }
}
