// <copyright file="RootCommandGroup.WPF.cs" company="INTV Funhouse">
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

using System.Windows;
using System.Windows.Controls;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class RootCommandGroup : CommandGroup
    {
        #region ApplicationMenuCommand

        /// <summary>
        /// The ribbon application menu.
        /// </summary>
        public static readonly VisualRelayCommand ApplicationMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ApplicationMenuCommand",
            Weight = 0
        };

        #endregion // ApplicationMenuCommand

        #region HomeRibbonTabCommand

        /// <summary>
        /// The Home tab pseudo-command.
        /// </summary>
        public static readonly VisualRelayCommand HomeRibbonTabCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".HomeRibbonTabCommand",
            Name = Resources.Strings.HomeRibbonTabCommand_Name,
            VisualParent = RootCommand,
            Weight = 0,
            UseXamlResource = true
        };

        #endregion // HomeRibbonTabCommand

        #region RibbonSeparatorCommand

        /// <summary>
        /// The menu separator "command".
        /// </summary>
        public static readonly VisualRelayCommand RibbonSeparatorCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RibbonSeparatorCommand",
            UseXamlResource = true
        };

        /// <summary>
        /// The menu separator "command".
        /// </summary>
        public static readonly VisualRelayCommand RibbonMenuSeparatorCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RibbonMenuSeparatorCommand",
            UseXamlResource = true
        };

        #endregion // RibbonSeparatorCommand

        #region Generally Useful Commands

        #region CloseWindowCommand

        /// <summary>
        /// A command to use to close a WPF window.
        /// </summary>
        public static readonly ICommand CloseWindowCommand = new RelayCommand(OnCloseWindow)
        {
            UniqueId = UniqueNameBase + ".CloseWindowCommand"
        };

        private static void OnCloseWindow(object parameter)
        {
            var window = parameter as Window;
            if ((window != null) && CloseWindowCommand.CanExecute(parameter))
            {
                window.Close();
            }
        }

        #endregion // CloseWindowCommand

        #endregion // Generally Useful Commands

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
                    case UniqueNameBase + ".Root":
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow.FindName("_ribbon") as UIElement;
                        break;
                    case UniqueNameBase + ".RootMenu":
                    case UniqueNameBase + ".ApplicationMenuCommand":
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow.FindName("_menu") as UIElement;
                        break;
                    case UniqueNameBase + ".HomeRibbonTabCommand":
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow.FindName("_home") as UIElement;
                        break;
                    default:
                        visual = base.CreateVisualForCommand(command);
                        break;
                }
            }
            return visual;
        }

        /// <inheritdoc />
        public override Control CreateMenuItemForCommand(ICommand command)
        {
            Control menuItem = null;
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand != null)
            {
                switch (visualCommand.UniqueId)
                {
                    case UniqueNameBase + ".RootMenu":
                    case UniqueNameBase + ".ApplicationMenuCommand":
                        menuItem = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow.FindName("_menu") as Control;
                        break;
                    default:
                        menuItem = base.CreateMenuItemForCommand(command);
                        break;
                }
            }
            return menuItem;
        }

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            CommandList.Add(HomeRibbonTabCommand);
            CommandList.Add(RibbonSeparatorCommand);
        }

        #endregion // CommandGroup
    }
}
