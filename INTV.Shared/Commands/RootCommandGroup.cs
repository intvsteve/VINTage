// <copyright file="RootCommandGroup.cs" company="INTV Funhouse">
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
    /// These form the 'root' commands, which are typically used to display other commands.
    /// </summary>
    public partial class RootCommandGroup : CommandGroup
    {
        /// <summary>
        /// Use this value as an 'epsilon' to have menu separators "hug" just above or below a menu item.
        /// </summary>
        public const double MenuSeparatorDelta = 0.0001;

        /// <summary>
        /// The root command group.
        /// </summary>
        public static readonly RootCommandGroup Group = new RootCommandGroup();

        private const string UniqueNameBase = "INTV.Shared.Commands.RootCommandGroup";

        private RootCommandGroup()
            : base(UniqueNameBase, string.Empty, 0)
        {
        }

        /// <summary>
        /// The root of the "command tree".
        /// </summary>
        public static readonly VisualRelayCommand RootCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".Root",
            Weight = 0
        };

        /// <summary>
        /// The root menu command.
        /// </summary>
        public static readonly VisualRelayCommand RootMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RootMenu",
            Weight = 0
        };

        /// <summary>
        /// The menu separator "command".
        /// </summary>
        public static readonly VisualRelayCommand MenuSeparatorCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".Separator",
        };

        #region CommandGroup

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(RootCommand);
            CommandList.Add(RootMenuCommand);
            AddPlatformCommands();
        }

        /// <summary>
        /// Implement to provide platform-specific commands and behaviors.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}
