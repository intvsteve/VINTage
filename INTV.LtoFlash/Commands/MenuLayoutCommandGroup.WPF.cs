// <copyright file="MenuLayoutCommandGroup.WPF.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Windows.Controls;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Behavior;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class MenuLayoutCommandGroup
    {
        private static readonly string UIReadmeFilename = "Readme.Windows.txt";

        #region EditNameCommand

        /// <summary>
        /// Command to edit the long name of a menu item. This is used in the Ribbon.
        /// </summary>
        /// <remarks>The execute method is platform-specific.</remarks>
        public static readonly VisualRelayCommand EditNameCommand = new VisualRelayCommand(OnEditName, CanEditLongName)
        {
            UniqueId = UniqueNameBase + ".EditNameCommand",
            Name = Resources.Strings.EditNameCommand_Name,
            ToolTipTitle = Resources.Strings.EditNameCommand_ToolTipTitle,
            ToolTipDescription = Resources.Strings.EditNameCommand_ToolTipDescription,
            SmallIcon = typeof(MenuLayoutCommandGroup).LoadImageResource("Resources/Images/rename_16xLG.png"),
            Weight = 0.24,
            PreferredParameterType = typeof(MenuLayoutViewModel)
        };

        private static void OnEditName(object parameter)
        {
            var menuLayoutViewModel = parameter as MenuLayoutViewModel;
            menuLayoutViewModel.IsEditing = true;
        }

        #endregion // EditNameCommand

        #region EditLongNameCommand

        private static void OnEditLongName(object parameter)
        {
            if (SingleInstanceApplication.Instance != null)
            {
                var mainWindow = SingleInstanceApplication.Instance.MainWindow;
                ItemsControl itemsContainer = mainWindow.FindChild<INTV.Shared.View.TreeListView>(t => t.Name == "_menuLayout");
                var viewModel = parameter as MenuLayoutViewModel;
                var currentSelection = viewModel.CurrentSelection;
                if ((currentSelection != null) && (itemsContainer != null))
                {
                    var containerOfCurrentSelection = FindVisualContainerForViewModel(itemsContainer, viewModel, currentSelection);
                    InPlaceEditBehavior.BeginInPlaceEdit(itemsContainer, containerOfCurrentSelection, currentSelection, (v) => v.FindChild<TextBlock>(t => t.Name == "LongName"));
                }
            }
        }

        #endregion // EditLongNameCommand

        #region EditShortNameCommand

        private static void OnEditShortName(object parameter)
        {
            if (SingleInstanceApplication.Instance != null)
            {
                var mainWindow = SingleInstanceApplication.Instance.MainWindow;
                ItemsControl itemsContainer = mainWindow.FindChild<INTV.Shared.View.TreeListView>(t => t.Name == "_menuLayout");
                var viewModel = parameter as MenuLayoutViewModel;
                var currentSelection = viewModel.CurrentSelection;
                if ((currentSelection != null) && (itemsContainer != null))
                {
                    var containerOfCurrentSelection = FindVisualContainerForViewModel(itemsContainer, viewModel, currentSelection);
                    InPlaceEditBehavior.BeginInPlaceEdit(itemsContainer, containerOfCurrentSelection, currentSelection, (v) => v.FindChild<TextBlock>(t => t.Name == "ShortName"));
                }
            }
        }

        #endregion // EditShortNameCommand

        private static void AddSetColorSubmenuItem(Control parentMenuItem, VisualRelayCommand submenuItemCommand, System.Tuple<MenuLayoutViewModel, FileNodeViewModel, INTV.Core.Model.Stic.Color> context)
        {
            var parent = (MenuItem)parentMenuItem;
            var submenuItem = submenuItemCommand.MenuItem as MenuItem;
            submenuItem.CommandParameter = context;
            parent.Items.Add(submenuItemCommand.MenuItem);
        }

        private static ItemsControl FindVisualContainerForViewModel(ItemsControl menuLayoutTree, FolderViewModel root, FileNodeViewModel item)
        {
            var container = menuLayoutTree;
            var path = new List<IFileContainer>();
            var parent = item.Parent;
            while (parent != root.Model)
            {
                path.Add(parent);
                parent = parent.Parent;
            }
            path.Reverse();
            FolderViewModel containerViewModel = root;
            foreach (var element in path)
            {
                containerViewModel = containerViewModel.FindViewModelForModel(element);
                container = container.ItemContainerGenerator.ContainerFromItem(containerViewModel) as ItemsControl;
            }
            return container;
        }
    }
}
