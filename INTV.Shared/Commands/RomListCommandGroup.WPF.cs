// <copyright file="RomListCommandGroup.WPF.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class RomListCommandGroup
    {
        private static RomListView RomListView
        {
            get
            {
                if (_romListView == null)
                {
                    if ((SingleInstanceApplication.Instance != null) && (SingleInstanceApplication.Instance.MainWindow != null))
                    {
                        var contentView = SingleInstanceApplication.Instance.MainWindow;
                        _romListView = contentView.FindChild<RomListView>();
                    }
                }
                return _romListView;
            }
        }
        private static RomListView _romListView;

        #region RomsRibbonGroupCommand

        /// <summary>
        /// Pseudo-command for the ROMs ribbon group.
        /// </summary>
        public static readonly VisualRelayCommand RomsRibbonGroupCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RomsRibbonGroupCommand",
            Name = Resources.Strings.RomListCommandGroup_Name,
            LargeIcon = typeof(RomListCommandGroup).LoadImageResource("Resources/Images/rom_32xMD.png"),
            VisualParent = RootCommandGroup.HomeRibbonTabCommand,
            UseXamlResource = true
        };

        #endregion // RomsRibbonGroupCommand

        #region RunProgramRibbonCommand

        /// <summary>
        /// Container ribbon split button command that has actual commands added at run-time.
        /// </summary>
        public static readonly VisualRelayCommand RunProgramRibbonCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".RunProgramRibbonCommand",
            Name = Resources.Strings.RunProgramCommand_Name,
            LargeIcon = typeof(RomListCommandGroup).LoadImageResource("ViewModel/Resources/Images/Play_32xLG_color.png"),
            VisualParent = RomsRibbonGroupCommand,
            Weight = 0,
            UseXamlResource = true
        };

        #endregion // RunProgramRibbonCommand

        #region EditRomFeaturesCommand

        private static void OnEditRomFeatures(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            var featureEditor = ProgramFeaturesEditorDialog.Create(RomListView.SelectedItem as ProgramDescriptionViewModel);
            featureEditor.BeginEdit();
        }

        #endregion // EditRomFeaturesCommand

        private static void OnEditProgramName(object parameter)
        {
            var viewModel = parameter as RomListViewModel;
            RomListView.EditSelectedItemColumn(RomListColumn.Title);
            viewModel.IsEditing = true;
        }

        #region CommandGroup

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            RomListGroupCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            RomListGroupCommand.Weight = 0.101;

            AddRomFilesCommand.VisualParent = RomsRibbonGroupCommand;
            AddRomFilesCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            AddRomFilesCommand.UseXamlResource = true;

            AddRomFoldersCommand.VisualParent = RomsRibbonGroupCommand;
            AddRomFoldersCommand.MenuParent = RootCommandGroup.ApplicationMenuCommand;
            AddRomFoldersCommand.UseXamlResource = true;

            RemoveRomsCommand.VisualParent = RomsRibbonGroupCommand;
            RemoveRomsCommand.UseXamlResource = true;

            RefreshRomsCommand.MenuParent = RomListGroupCommand;

            ShowRomInfoCommand.VisualParent = RomsRibbonGroupCommand;
            ShowRomInfoCommand.UseXamlResource = true;
            ShowRomInfoCommand.Weight = 0.13;

            EditProgramNameCommand.VisualParent = RomsRibbonGroupCommand;
            EditProgramNameCommand.UseXamlResource = true;
            EditProgramNameCommand.Weight = 0.14;

            ValidateRomsCommand.MenuParent = RomListGroupCommand;

            BackupRomListCommand.Weight = 0.136;
            RestoreRomListCommand.Weight = 0.137;

            CommandList.Add(RomListGroupCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
            CommandList.Add(RomsRibbonGroupCommand);
            CommandList.Add(RunProgramRibbonCommand);
            CommandList.Add(RunProgramRibbonCommand.CreateRibbonSeparator(CommandLocation.After));
            CommandList.Add(RefreshRomsCommand.CreateRibbonMenuSeparator(CommandLocation.After, true));
        }

        #endregion // CommandGroup
    }
}
