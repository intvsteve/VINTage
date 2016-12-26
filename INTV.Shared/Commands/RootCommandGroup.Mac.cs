// <copyright file="RootCommandGroup.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class RootCommandGroup
    {
        /// <summary>
        /// Command for the application menu.
        /// </summary>
        public static readonly VisualRelayCommand ApplicationMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".AppMenu",
            Weight = 0,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// Command for the File submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand FileMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FileMenu",
            Weight = 0.1,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// Command for the Edit submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand EditMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".EditMenu",
            Weight = 0.2,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// Command for the View submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand ViewMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ViewMenu",
            Weight = 0.3,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// Command for the Tools submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand ToolsMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ToolsMenu",
            Weight = 0.4,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// Command for the Window submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand WindowMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".WindowMenu",
            Weight = 0.5,
            MenuParent = RootMenuCommand
        };

        /// <summary>
        /// Command for the Help submenu in the menu bar.
        /// </summary>
        public static readonly VisualRelayCommand HelpMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".HelpMenu",
            Weight = 0.6,
            MenuParent = RootMenuCommand
        };

        /// <inheritdoc />
        public override object Context
        {
            get { return null; }
        }

        /// <inheritdoc />
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            var window = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
            var visualCommand = (VisualRelayCommand)command;
            var rootMenu = (NSMenu)RootMenuCommand.Visual;
            NSObject visual = null;
            NSMenuItem menuItem = null;
            switch (visualCommand.UniqueId)
            {
                case UniqueNameBase +  ".Root":
                    if (window != null)
                    {
                        visual = window.Toolbar;
                    }
                    break;
                case UniqueNameBase + ".RootMenu":
                    if (window != null)
                    {
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Current.Menu;
                    }
                    break;
                case UniqueNameBase + ".AppMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Application);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".FileMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.File);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".EditMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Edit);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".ViewMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.View);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".ToolsMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Tools);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".WindowMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Window);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".HelpMenu":
                    menuItem = rootMenu.ItemWithTag((int)StandardMenuId.Help);
                    visual = menuItem;
                    break;
                case UniqueNameBase + ".Separator":
                    visual = NSMenuItem.SeparatorItem;
                    break;
                default:
                    visual = rootMenu.ItemWithTag(command.GetHashCode());
                    if (visual == null)
                    {
                        visual = CreateMenuItemForCommand(command);
                    }
                    break;
            }
            if ((menuItem != null) && (menuItem.RepresentedObject == null))
            {
                menuItem.RepresentedObject = new NSObjectWrapper<ICommand>(command);
            }
            return visual;
        }

        #region ICommandGroup

        /// <inheritdoc />
        public override NSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            var visualCommand = (VisualRelayCommand)command;
            if (visualCommand.Visual == null)
            {
                visualCommand.Visual = base.CreateMenuItemForCommand(command);
            }
            return visualCommand.Visual as NSMenuItem;
        }

        #region CommandGroup

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

        #endregion // CommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            CommandList.Add(ApplicationMenuCommand);
            CommandList.Add(FileMenuCommand);
            CommandList.Add(EditMenuCommand);
#if DEBUG
            CommandList.Add(DebugCommandGroup.DebugMenuCommand);
#endif
            CommandList.Add(ViewMenuCommand);
            CommandList.Add(ToolsMenuCommand);
            CommandList.Add(WindowMenuCommand);
            CommandList.Add(HelpMenuCommand);
        }

        #endregion // ICommandGroup
    }
}
