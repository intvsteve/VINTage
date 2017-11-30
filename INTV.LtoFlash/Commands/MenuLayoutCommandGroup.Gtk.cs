// <copyright file="MenuLayoutCommandGroup.Gtk.cs" company="INTV Funhouse">
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

using System.Linq;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

namespace INTV.LtoFlash.Commands
{
    public partial class MenuLayoutCommandGroup
    {
        private static readonly string UIReadmeFilename = "README.Linux.txt";

        /// <inheritdoc/>
        public override object Context
        {
            get { return MenuLayoutViewModel; }
        }

        private MenuLayoutViewModel MenuLayoutViewModel
        {
            get
            {
                var ltoFlashViewModel = CompositionHelpers.Container.GetExportedValue<LtoFlashViewModel>();
                var menuLayoutViewModel = ltoFlashViewModel.HostPCMenuLayout;
                return menuLayoutViewModel;
            }
        }

        #region EditLongNameCommand

        private static void OnEditLongName(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            var menuLayout = menuLayoutViewModel.LtoFlashViewModel.GetVisuals().First().Item2.NativeVisual as INTV.LtoFlash.View.MenuLayoutView;
            menuLayout.EditLongName();
        }

        #endregion // EditLongNameCommand

        #region EditShortNameCommand

        private static void OnEditShortName(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            var menuLayout = menuLayoutViewModel.LtoFlashViewModel.GetVisuals().First().Item2.NativeVisual as INTV.LtoFlash.View.MenuLayoutView;
            menuLayout.EditShortName();
        }

        #endregion // EditShortNameCommand

        private static void AddSetColorSubmenuItem(OSMenuItem parentMenuItem, VisualRelayCommand submenuItemCommand, System.Tuple<MenuLayoutViewModel, FileNodeViewModel, INTV.Core.Model.Stic.Color> context)
        {
            // TODO: Implement MenuLayoutCommandGroup.AddSetColorSubmenuItem
            throw new System.NotImplementedException("AddSetColorSubmenuItem.");
        }

        #region CommandGroup

        partial void AddPlatformCommands()
        {
            var addToMenuClone = AddRomsToMenuCommand.Clone(); // for toolbar
            AddRomsToMenuCommand.Weight = NewDirectoryCommand.Weight + 0.001;
            AddRomsToMenuCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            // Don't use arrow keys - they are snatched up by the control
            ////AddRomsToMenuCommand.KeyboardShortcutKey = new string((char)Gdk.Key.rightarrow, 1); // right arrow
            AddRomsToMenuCommand.KeyboardShortcutKey = "M";
            AddRomsToMenuCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;

            addToMenuClone.MenuParent = null;
            addToMenuClone.Weight = 0.11;
            addToMenuClone.VisualParent = RootCommandGroup.RootCommand;
            CommandList.Add(addToMenuClone);

            NewDirectoryCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            NewDirectoryCommand.VisualParent = RootCommandGroup.RootCommand;
            CommandList.Add(NewDirectoryCommand.CreateSeparator(CommandLocation.Before));
            CommandList.Add(NewDirectoryCommand.CreateToolbarSeparator(CommandLocation.Before));

            DeleteItemsCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            DeleteItemsCommand.VisualParent = RootCommandGroup.RootCommand;
            //MenuLayoutGroupCommand.Weight = DeleteItemsCommand.Weight + RootCommandGroup.MenuSeparatorDelta;

            MenuLayoutGroupCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            MenuLayoutGroupCommand.Weight = DeleteItemsCommand.Weight + RootCommandGroup.MenuSeparatorDelta;
            //CommandList.Add(MenuLayoutGroupCommand.CreateSeparator(CommandLocation.After));

            EditLongNameCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            CommandList.Add(EditLongNameCommand);
            CommandList.Add(EditLongNameCommand.CreateSeparator(CommandLocation.Before));

            EditShortNameCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            CommandList.Add(EditShortNameCommand);

            SetManualCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            RemoveManualCommand.MenuParent = RootCommandGroup.EditMenuCommand;

            #if false

            // Need to write a custom dialog.
            SetColorCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            #endif

            ReadmeCommand.MenuParent = RootCommandGroup.HelpMenuCommand;
        }

        #endregion // CommandGroup
    }
}
