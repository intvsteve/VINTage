// <copyright file="MenuLayoutCommandGroup.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.LtoFlash.View;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class MenuLayoutCommandGroup
    {
        private static readonly string UIReadmeFilename = "Readme.Mac.txt";

        /// <summary>
        /// Is there a better way to do this?
        /// </summary>
        private MenuLayoutViewModel MenuLayoutViewModel
        {
            get
            {
                if (_menuLayoutViewModel == null)
                {
                    if (MenuLayoutView != null)
                    {
                        _menuLayoutViewModel = MenuLayoutView.ViewModel;
                    }
                }
                return _menuLayoutViewModel;
            }
        }
        private MenuLayoutViewModel _menuLayoutViewModel;

        private MenuLayoutView MenuLayoutView
        {
            get
            {
                if (_menuLayoutView == null)
                {
                    if ((NSApplication.SharedApplication != null) && (NSApplication.SharedApplication.MainWindow != null))
                    {
                        var contentView = NSApplication.SharedApplication.MainWindow.ContentView;
                        _menuLayoutView = contentView.FindChild<INTV.LtoFlash.View.MenuLayoutView>();
                    }
                }
                return _menuLayoutView;
            }
        }
        private MenuLayoutView _menuLayoutView;

        #region EditLongNameCommand

        private static void OnEditLongName(object parameter)
        {
            // NOTE: This operates on the current selection in the menu layout.
            var controller = Group.MenuLayoutView.Controller;
            controller.EditMenuItemName(EditableOutlineViewColumn.LongName);
        }

        #endregion // EditLongNameCommand

        #region EditShortNameCommand

        private static void OnEditShortName(object parameter)
        {
            // NOTE: This operates on the current selection in the menu layout.
            var controller = Group.MenuLayoutView.Controller;
            controller.EditMenuItemName(EditableOutlineViewColumn.ShortName);
        }

        #endregion // EditShortNameCommand

        #region SetColorCommand

        static partial void OSSetColor(object parameter)
        {
            // NOTE: This operates on the current selection in the menu layout.
            var colorWell = SetColorCommand.Visual as NSColorWell;
            colorWell.Activate(true);
        }

        #endregion // SetColorCommand

        #region RestoreMenuLayoutCommand

        static partial void RestoreMenuLayoutComplete(LtoFlashViewModel viewModel)
        {
            RomListCommandGroup.SortRoms();
        }

        #endregion // RestoreMenuLayoutCommand

        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return MenuLayoutViewModel; }
        }

        /// <inheritdoc />
        protected override bool HandleCanExecuteChangedForCommand(VisualRelayCommand command)
        {
            var canExecute = base.HandleCanExecuteChangedForCommand(command);
            if (canExecute && (command.UniqueId == DeleteItemsCommand.UniqueId))
            {
                NSResponder firstResponder = null;
                if ((NSApplication.SharedApplication != null) && (NSApplication.SharedApplication.MainWindow != null))
                {
                    firstResponder = NSApplication.SharedApplication.MainWindow.FirstResponder;
                    canExecute = firstResponder is NSOutlineView;
                    if (canExecute)
                    {
                        canExecute = (MenuLayoutView != null) && (MenuLayoutView.FindChild<NSOutlineView>() == firstResponder);
                    }
                    command.MenuItem.KeyEquivalent = canExecute ? DeleteItemsCommand.KeyboardShortcutKey : string.Empty;
                    command.MenuItem.KeyEquivalentModifierMask = (NSEventModifierMask)(canExecute ? DeleteItemsCommand.KeyboardShortcutModifiers : OSModifierKeys.None);
                }
            }
            return canExecute;
        }

        /// <inheritdoc/>
        protected override void InitializeMenuItem(VisualRelayCommand command, object target, object context)
        {
            base.InitializeMenuItem(command, null, context); // we don't use target on Mac -- just context
        }

        #endregion // CommandGroup

        #region // ICommandGroup

        /// <inheritdoc />
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            var visual = base.CreateVisualForCommand(command);
            if (((RelayCommand)command).UniqueId == DeleteItemsCommand.UniqueId)
            {
                var toolbarItem = visual as NSToolbarItem;
                toolbarItem.Label = DeleteItemsCommand.MenuItemName;
            }
            return visual;
        }

        partial void AddPlatformCommands()
        {
            EditLongNameCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            CommandList.Add(EditLongNameCommand);
            CommandList.Add(EditShortNameCommand);

            SetColorCommand.MenuParent = RootCommandGroup.EditMenuCommand;

            MenuLayoutGroupCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            CommandList.Add(MenuLayoutGroupCommand.CreateSeparator(CommandLocation.Before));

            CommandList.Add(MenuLayoutGroupCommand);
            CommandList.Add(BackupMenuLayoutCommand);
            CommandList.Add(RestoreMenuLayoutCommand);
            CommandList.Add(EmptyMenuLayoutCommand);

            NewDirectoryCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            NewDirectoryCommand.VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand;

            AddRomsToMenuCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            AddRomsToMenuCommand.KeyboardShortcutKey = new string((char)0xF703, 1); // right arrow (NSRightArrowFunctionKey)
            AddRomsToMenuCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;
            AddRomsToMenuCommand.VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand;

            DeleteItemsCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            DeleteItemsCommand.KeyboardShortcutKey = CommandProviderHelpers.NSBackspaceCharacterString; // delete
            DeleteItemsCommand.VisualParent = INTV.Shared.Commands.RootCommandGroup.RootCommand;
            MenuLayoutGroupCommand.Weight = DeleteItemsCommand.Weight + RootCommandGroup.MenuSeparatorDelta;
            CommandList.Add(MenuLayoutGroupCommand.CreateSeparator(CommandLocation.After));

            SetManualCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            RemoveManualCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            CommandList.Add(RemoveManualCommand.CreateSeparator(CommandLocation.After));

            ReadmeCommand.MenuParent = RootCommandGroup.HelpMenuCommand;
        }

        #endregion // ICommandGroup

        private static void AddSetColorSubmenuItem(NSMenuItem parentMenuItem, VisualRelayCommand submenuItemCommand, System.Tuple<MenuLayoutViewModel, FileNodeViewModel, INTV.Core.Model.Stic.Color> context)
        {
            var submenuItem = submenuItemCommand.MenuItem;
            var parentMenu = parentMenuItem.Submenu;
            if (parentMenu == null)
            {
                parentMenu = new NSMenu("Colors");
                parentMenuItem.Submenu = parentMenu;
            }
            submenuItemCommand.SetValue("DataContext", context);
            submenuItemCommand.MenuItem.Image = submenuItemCommand.SmallIcon;
            parentMenu.AddItem(submenuItemCommand.MenuItem);
        }
    }
}
