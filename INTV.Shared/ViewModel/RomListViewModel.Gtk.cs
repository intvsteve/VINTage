// <copyright file="RomListViewModel.Gtk.cs" company="INTV Funhouse">
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

using System.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(INTV.Shared.ComponentModel.IPrimaryComponent))]
    public partial class RomListViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="INTV.Shared.View.RomListView"/> Gtk.TreeView has focus.
        /// </summary>
        internal bool ListHasFocus
        {
            get
            {
                return _listHasFocus;
            }

            set
            {
                if (_listHasFocus != value)
                {
                    _listHasFocus = value;
                    INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        private bool _listHasFocus;

        private void FilesDropped(object dragEventArgs)
        {
            throw new System.NotImplementedException("RomListViewModel.FilesDropped");
        }

        private OSVisual OSInitializeVisual()
        {
            var visual = new RomListView(this);
            return visual;
        }

        /// <summary>
        /// Perform GTK-specific initialization.
        /// </summary>
        partial void OSInitialize()
        {
            ////_sortColumn = RomListColumn.None;
            ////_sortDirection = ListSortDirection.Ascending;
            var configuration = SingleInstanceApplication.Instance.GetConfiguration<RomListConfiguration>();
            var romListFilePath = configuration.RomFilesPath;
            _filePath = romListFilePath;
            Model = LoadRomList(romListFilePath);
            Model.SaveFailed += HandleSaveFailed;
            ((INotifyPropertyChanged)Model).PropertyChanged += HandleProgramCollectionPropertyChanged;
            _searchDirectories = Properties.Settings.Default.RomListSearchDirectories;
            if (_searchDirectories == null)
            {
                _searchDirectories = new SearchDirectories();
                Properties.Settings.Default.RomListSearchDirectories = _searchDirectories;
            }
            if (Properties.Settings.Default.RomListSearchForRomsAtStartup && CanRefreshRoms(null))
            {
                var options = RomDiscoveryOptions.AddNewRoms;
                if (Properties.Settings.Default.RomListValidateAtStartup)
                {
                    options |= RomDiscoveryOptions.DetectChanges | RomDiscoveryOptions.DetectMissingRoms | RomDiscoveryOptions.DetectNewRoms;
                }
                var taskData = new RomDiscoveryData(Properties.Settings.Default.RomListSearchDirectories, _programs.ModelCollection, Resources.Strings.RomListViewModel_ScanningForRoms_Title, options);
                SingleInstanceApplication.Instance.AddStartupAction("ScanForRoms", () => RefreshRoms(taskData), StartupTaskPriority.HighestAsyncTaskPriority);
            }

            // TODO: Initialize sorting!
            // TODO: Drag/Drop?
        }
    }
}
