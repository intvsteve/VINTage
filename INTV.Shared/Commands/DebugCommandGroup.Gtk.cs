// <copyright file="DebugCommandGroup.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
using INTV.Shared.View;

namespace INTV.Shared.Commands
{
    public partial class DebugCommandGroup
    {
        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return null; }
        }

        #endregion // CommandGroup

        #if false
        /// <inheritdoc />
        public override OSVisual CreateVisualForCommand(ICommand command)
        {
            var window = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
            var visualCommand = (VisualRelayCommand)command;
            var rootMenu = RootCommandGroup.RootMenuCommand.Visual.AsType<Gtk.MenuBar>();
            Gtk.Widget visual = null;
            switch (visualCommand.UniqueId)
            {
                case UniqueNameBase + ".DebugMenuCommand":
                    //visual = rootMenu.ItemWithTag((int)DebugMenuCommand.GetHashCode());
                    if (visual == null)
                    {
                        visual = CreateMenuItemForCommand(command);
                    }
                    break;
                default:
                    visual = base.CreateVisualForCommand(command);
                    break;
            }
            return visual;
        }
        #endif
    }
}
