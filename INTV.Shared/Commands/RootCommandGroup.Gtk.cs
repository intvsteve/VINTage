// <copyright file="RootCommandGroup.Gtk.cs" company="INTV Funhouse">
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
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class RootCommandGroup
    {
        #region ApplicationMenuCommand

        /// <summary>
        /// The application menu.
        /// </summary>
        public static readonly VisualRelayCommand ApplicationMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ApplicationMenuCommand",
            Weight = 0
        };

        #endregion // ApplicationMenuCommand

        #region ToolbarSeparatorCommand

        /// <summary>
        /// The toolbar separator "command".
        /// </summary>
        public static readonly VisualRelayCommand ToolbarSeparatorCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ToolbarSeparatorCommand",
        };

        #endregion // ToolbarSeparatorCommand

        #region CommandGroup

        /// <summary>
        /// Gets the general data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return null; }
        }

        #endregion // CommandGroup

        #region ICommandGroup

        public override OSVisual CreateVisualForCommand(ICommand command)
        {
            OSVisual visual = OSVisual.Empty;
            var visualCommand = command as VisualRelayCommand;
            var window = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow;
            if (visualCommand != null)
            {
                switch (visualCommand.UniqueId)
                {
                    case UniqueNameBase + ".Root":
                        visual = window.FindChild<Gtk.Toolbar>(c => c.Name == "_toolbar");
                        break;
                    case UniqueNameBase + ".RootMenu":
                    case UniqueNameBase + ".ApplicationMenuCommand":
                        visual = window.FindChild<Gtk.MenuBar>(c => c.Name == "_menubar");
                        break;
                    default:
                        visual = base.CreateVisualForCommand(command);
                        break;
                }
            }
            return visual;
        }

        #endregion // ICommandGroup

        #region CommandGroup

        #if false
        /// <inheritdoc />
        protected override void AttachActivateHandler(RelayCommand command, NSObject visual)
        {
            // we do not need to attach handlers to these commands.
        }

        /// <inheritdoc />
        internal override void AttachCanExecuteChangeHandler(RelayCommand command)
        {
            // we do not need to attach can execute handlers to these
        }
        #endif

        /// <summary>
        /// Add GTK-specific commands.
        /// </summary>
        partial void AddPlatformCommands()
        {
            CommandList.Add(FileMenuCommand);
            CommandList.Add(EditMenuCommand);
            CommandList.Add(ViewMenuCommand);
            CommandList.Add(ToolsMenuCommand);
            CommandList.Add(WindowMenuCommand);
            CommandList.Add(HelpMenuCommand);
        }

        #endregion // CommandGroup
    }
}
