// <copyright file="RootCommandGroup.Mono.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    public partial class RootCommandGroup
    {
        #region FileMenuCommand

        /// <summary>
        /// Command for the File submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand FileMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            // TODO: Put strings into resources.
            UniqueId = UniqueNameBase + ".FileMenu",
            Name = "File",
            Weight = 0.1,
            MenuParent = RootMenuCommand
        };

        #endregion // FileMenuCommand

        #region EditMenuCommand

        /// <summary>
        /// Command for the Edit submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand EditMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            // TODO: Put strings into resources.
            UniqueId = UniqueNameBase + ".EditMenu",
            Name = "Edit",
            Weight = 0.2,
            MenuParent = RootMenuCommand
        };

        #endregion // EditMenuCommand

        #region ViewMenuCommand

        /// <summary>
        /// Command for the View submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand ViewMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            // TODO: Put strings into resources.
            UniqueId = UniqueNameBase + ".ViewMenu",
            Name = "View",
            Weight = 0.3,
            MenuParent = RootMenuCommand
        };

        #endregion // ViewMenuCommand

        #region ToolsMenuCommand

        /// <summary>
        /// Command for the Tools submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand ToolsMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            // TODO: Put strings into resources.
            UniqueId = UniqueNameBase + ".ToolsMenu",
            Name = "Tools",
            Weight = 0.4,
            MenuParent = RootMenuCommand
        };

        #endregion // ToolsMenuCommand

        #region WindowMenuCommand

        /// <summary>
        /// Command for the Window submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand WindowMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            // TODO: Put strings into resources.
            UniqueId = UniqueNameBase + ".WindowMenu",
            Name = "Window",
            Weight = 0.5,
            MenuParent = RootMenuCommand
        };

        #endregion // WindowMenuCommand

        #region HelpMenuCommand

        /// <summary>
        /// Command for the Help submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand HelpMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            // TODO: Put strings into resources.
            UniqueId = UniqueNameBase + ".HelpMenu",
            Name = "Help",
            Weight = 0.6,
            MenuParent = RootMenuCommand
        };

        #endregion // HelpMenuCommand
    }
}
