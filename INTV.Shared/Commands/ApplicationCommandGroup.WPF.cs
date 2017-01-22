// <copyright file="ApplicationCommandGroup.WPF.cs" company="INTV Funhouse">
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

using System.Windows;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class ApplicationCommandGroup
    {
        #region RootMenuFooterCommand

        /// <summary>
        /// The container for items in the root ribbon menu footer.
        /// </summary>
        public static readonly VisualRelayCommand ApplicationMenuFooterCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ApplicationMenuFooter",
        };

        #endregion // RootMenuFooterCommand

        #region CommandGroup

        /// <inheritdoc />
        public override UIElement CreateVisualForCommand(ICommand command)
        {
            UIElement visual = null;
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand != null)
            {
                switch (visualCommand.UniqueId)
                {
                    case UniqueNameBase + ".ApplicationMenuFooter":
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow.FindName("_rootMenuFooter") as UIElement;
                        break;
                    default:
                        visual = base.CreateVisualForCommand(command);
                        break;
                }
            }
            return visual;
        }

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            SettingsDialogCommand.VisualParent = ApplicationMenuFooterCommand;
            SettingsDialogCommand.UseXamlResource = true;
            SettingsDialogCommand.Weight = 0;
            CheckForUpdatesCommand.MenuParent = null;
            CheckForUpdatesCommand.VisualParent = ApplicationMenuFooterCommand;
            CheckForUpdatesCommand.UseXamlResource = true;
            ShowOnlineHelpCommand.VisualParent = ApplicationMenuFooterCommand;
            ShowOnlineHelpCommand.UseXamlResource = true;
            ShowOnlineHelpCommand.Weight = 0.7;
            ExitCommand.VisualParent = ApplicationMenuFooterCommand;
            ExitCommand.UseXamlResource = true;

            CommandList.Add(ApplicationMenuFooterCommand);
            CommandList.Add(ShowOnlineHelpCommand);
            CommandList.Add(ExitCommand);
        }

        #endregion // CommandGroup
    }
}
