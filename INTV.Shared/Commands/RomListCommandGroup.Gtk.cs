// <copyright file="RomListCommandGroup.Gtk.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.Commands
{
    public partial class RomListCommandGroup
    {
        #region RunProgramToolbarCommand

        /// <summary>
        /// Container Gtk.MenuToolButton command that has actual commands added at run-time.
        /// </summary>
        public static readonly VisualRelayCommand RunProgramToolbarCommand = new VisualRelayCommand(RelayCommand.NoOp)
            {
                UniqueId = UniqueNameBase + ".RunProgramToolbarCommand",
                Name = Resources.Strings.RunProgramCommand_Name,
                LargeIcon = typeof(RomListCommandGroup).LoadImageResource("ViewModel/Resources/Images/Play_32xLG_color.png"),
                VisualParent = RootCommandGroup.RootCommand,
                Weight = 0,
            };

        #endregion // RunProgramToolbarCommand

        #region EditRomFeaturesCommand

        private static void OnEditRomFeatures(object parameter)
        {
            throw new System.NotImplementedException("RomListCommandGroup.OnEditRomFeatures");
//            var viewModel = parameter as RomListViewModel;
//            var featureEditor = ProgramFeaturesEditorDialog.Create(RomListView.SelectedItem as ProgramDescriptionViewModel);
//            featureEditor.BeginEdit();
        }

        #endregion // EditRomFeaturesCommand

        #region EditProgramNameCommand

        private static void OnEditProgramName(object parameter)
        {
            throw new System.NotImplementedException("RomListCommandGroup.OnEditProgramName");
//            var viewModel = parameter as RomListViewModel;
//            RomListView.EditSelectedItemColumn(RomListColumn.Title);
//            viewModel.IsEditing = true;
        }

        #endregion // EditProgramNameCommand

        #region ShowRomInfoCommand

        private static void OnToggleShowRomInfo(object sender, System.EventArgs args)
        {
            var menuItem = sender as Gtk.CheckMenuItem;
            Properties.Settings.Default.ShowRomDetails = menuItem.Active;
        }

        #endregion // ShowRomInfoCommand

        #region CommandGroup

        /// <inheritdoc />
        public override object Context
        {
            get { return RomListViewModel; }
        }

        #endregion // CommandGroup

        #region ICommandGroup

        /// <inheritdoc />
        public override OSVisual CreateToolbarItemForCommand(ICommand command)
        {
            OSVisual visual;
            var visualRelayCommand = command as VisualRelayCommand;
            if (visualRelayCommand.UniqueId == RunProgramToolbarCommand.UniqueId)
            {
                // TODO: Figure out how to hook up the "default" "Play" command to the button.
                // There's a mechanism used by double-click in ROM list, so use that....
                visual = visualRelayCommand.CreateMenuToolButtonForCommand(false, null);
            }
            else
            {
                visual = base.CreateToolbarItemForCommand(command);
            }
            return visual;
        }

        public override OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            Gtk.MenuItem menuItem = null;
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand.UniqueId == RomListCommandGroup.ShowRomInfoCommand.UniqueId)
            {
                var showDetails = Properties.Settings.Default.ShowRomDetails;
                menuItem = visualCommand.CreateMenuItemForCommand(Gtk.StockItem.Zero, true, typeof(Gtk.CheckMenuItem), showDetails);
                ((Gtk.CheckMenuItem)menuItem).Toggled += OnToggleShowRomInfo;
            }
            else
            {
                menuItem = base.CreateMenuItemForCommand(command);
            }
            return menuItem;
        }

        #region CommandGroup

        /// <inheritdoc />
        protected override bool HandleCanExecuteChangedForCommand(VisualRelayCommand command, OSVisual visual)
        {
            var canExecute = true;
            if ((command.UniqueId == RomListCommandGroup.RemoveRomsCommand.UniqueId) && (visual.NativeVisual is Gtk.MenuItem))
            {
                // for this one, disable the menu item unless the RomListView's tree has focus.
                canExecute = RomListViewModel.ListHasFocus;
            }
            return canExecute && base.HandleCanExecuteChangedForCommand(command, visual);
        }

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            // TODO: Fix up command weights, add dividers!
            CommandList.Add(RunProgramToolbarCommand);

            RomListGroupCommand.MenuParent = RootCommandGroup.EditMenuCommand;

            AddRomFilesCommand.VisualParent = RootCommandGroup.RootCommand; // add to toolbar
            //AddRomFilesCommand.SmallIcon = ResourceHelpers.LoadImageResource(typeof(RomListCommandGroup), "ViewModel/Resources/Images/add_file_16xLG.png");
            AddRomFilesCommand.MenuParent = RootCommandGroup.FileMenuCommand;

            AddRomFoldersCommand.VisualParent =  RootCommandGroup.RootCommand;
            AddRomFoldersCommand.MenuParent = RootCommandGroup.FileMenuCommand;

            //RemoveRomsCommand.VisualParent = RomsRibbonGroupCommand;
            RemoveRomsCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            RemoveRomsCommand.KeyboardShortcutKey = CommandProviderHelpers.GtkDeleteCharacterString;

            RefreshRomsCommand.MenuParent = RootCommandGroup.ViewMenuCommand;
            RefreshRomsCommand.KeyboardShortcutKey = new string((char)Gdk.Key.F5, 1);

            ValidateRomsCommand.KeyboardShortcutKey = "r";
            ValidateRomsCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;
            ValidateRomsCommand.MenuParent = RootCommandGroup.ViewMenuCommand;

            ShowRomInfoCommand.MenuParent = RootCommandGroup.ViewMenuCommand;

            // TODO: More in toolbar?
            // TODO: RemoveRomsCommand in toolbar?
            // TODO: EditProgramNameCommand
            // TODO: RestoreRomListCommand
            // TODO: RunProgramRibbonCommand -- Use Gtk.MenuToolButton!
        }

        #endregion // CommandGroup

        #endregion // ICommandGroup
    }
}
