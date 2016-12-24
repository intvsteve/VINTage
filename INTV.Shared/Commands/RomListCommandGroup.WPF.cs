// <copyright file="RomListCommandGroup.WPF.cs" company="INTV Funhouse">
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
        };

        #endregion // RomsRibbonGroupCommand

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

        /// <inheritdoc />
        partial void AddPlatformCommands()
        {
            RemoveRomsCommand.VisualParent = RootCommandGroup.RootCommand;
            RefreshRomsCommand.VisualParent = RootCommandGroup.RootCommand;

            CommandList.Add(RemoveRomsCommand);
            CommandList.Add(RefreshRomsCommand);
        }
    }
}
