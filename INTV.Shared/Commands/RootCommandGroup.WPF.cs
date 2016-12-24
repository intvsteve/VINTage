// <copyright file="RootCommandGroup.WPF.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class RootCommandGroup
    {
        #region HomeRibbonGroupCommand

        /// <summary>
        /// The Home tab pseudo-command.
        /// </summary>
        public static readonly VisualRelayCommand HomeRibbonTabCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".HomeRibbonTabCommand",
            Name = Resources.Strings.HomeRibbonTabCommand_Name,
            Weight = 0
        };

        /// <summary>
        /// Dummy command for now. This will eventually become the ribbon menu.
        /// </summary>
        public static readonly VisualRelayCommand ApplicationMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ApplicationMenuCommand",
            Weight = 0
        };

        #endregion // HomeRibbonTabCommand

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            // CommandList.Add(HomeRibbonGroupCommand);
        }
    }
}
