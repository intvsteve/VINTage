// <copyright file="RomListViewModel.WPF.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using INTV.Core.Model.Program;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class RomListViewModel
    {
        private static readonly bool RegisteredTextBlockEditor = INTV.Shared.View.TextBlockEditorAdorner.RegisterInPlaceEditor();
        private static readonly bool RegisteredProgramFeaturesEditor = INTV.Shared.View.ProgramFeaturesEditorAdorner.RegisterInPlaceEditor();

        private static readonly Dictionary<RomListColumn, RomListColumn> SecondarySortData = new Dictionary<RomListColumn, RomListColumn>()
        {
            { RomListColumn.Title, RomListColumn.Vendor },
            { RomListColumn.Vendor, RomListColumn.Title },
            { RomListColumn.Year, RomListColumn.Title },
            { RomListColumn.RomFile, RomListColumn.Title },
            { RomListColumn.Features, RomListColumn.Title },
            { RomListColumn.ManualFile, RomListColumn.Title },
        };

        #region Commands

        /// <summary>
        /// Gets the command to use to add ROMs to the known list of programs available to the emulator.
        /// </summary>
        public RelayCommand AddRomsCommand
        {
            get { return new RelayCommand(AddRoms); }
        }

        /// <summary>
        /// Gets the command to execute to remove selected ROMs from the list, which also removes them from the menu layout.
        /// </summary>
        public RelayCommand RemoveRomsCommand
        {
            get { return new RelayCommand(RemoveRoms, CanRemoveRoms); }
        }

        #endregion Commands

        /// <summary>
        /// Gets or sets the sort direction on the current sort column. When set, the settings information will also be updated.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return _sortDirection; }
            set { AssignAndUpdateProperty(SortDirectionPropertyName, value, ref _sortDirection, (n, d) => Properties.Settings.Default.SortDirection = d); }
        }
        private ListSortDirection _sortDirection;

        /// <summary>
        /// Gets or sets the sort column. When set, the settings information will also be updated.
        /// </summary>
        public RomListColumn SortColumn
        {
            get { return _sortColumn; }
            set { AssignAndUpdateProperty(SortColumnPropertyName, value, ref _sortColumn, (n, c) => Properties.Settings.Default.SortColumn = c); }
        }
        private RomListColumn _sortColumn;

        /// <summary>
        /// Gets or sets a value indicating whether to edit the currently selected ROM.
        /// </summary>
        /// <remarks>This should only be set if one and only one ROM is selected.</remarks>
        public bool IsEditing
        {
            get { return _isEditing; }
            set { AssignAndUpdateProperty("IsEditing", value, ref _isEditing); }
        }
        private bool _isEditing;

        /// <summary>
        /// Gets or sets the width of the 'Name' column. When set, the settings information will also be updated.
        /// </summary>
        public double NameColumnWidth
        {
            get
            {
                return _nameColumnWidth;
            }

            set
            {
                if (value > 0)
                {
                    AssignAndUpdateProperty("NameColumnWidth", value, ref _nameColumnWidth, (n, w) => Properties.Settings.Default.RomListNameColWidth = w);
                }
            }
        }
        private double _nameColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'Vendor' column. When set, the settings information will also be updated.
        /// </summary>
        public double VendorColumnWidth
        {
            get
            {
                return _vendorColumnWidth;
            }

            set
            {
                if (value > 0)
                {
                    AssignAndUpdateProperty("VendorColumnWidth", value, ref _vendorColumnWidth, (n, w) => Properties.Settings.Default.RomListVendorColWidth = w);
                }
            }
        }
        private double _vendorColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'Year' column. When set, the settings information will also be updated.
        /// </summary>
        public double YearColumnWidth
        {
            get
            {
                return _yearColumnWidth;
            }

            set
            {
                if (value > 0)
                {
                    AssignAndUpdateProperty("YearColumnWidth", value, ref _yearColumnWidth, (n, w) => Properties.Settings.Default.RomListYearColWidth = w);
                }
            }
        }
        private double _yearColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'Features' column. When set, the settings information will also be updated.
        /// </summary>
        public double FeaturesColumnWidth
        {
            get
            {
                return _featuresColumnWidth;
            }

            set
            {
                if (value > 0)
                {
                    AssignAndUpdateProperty("FeaturesColumnWidth", value, ref _featuresColumnWidth, (n, w) => Properties.Settings.Default.RomListFeaturesColWidth = w);
                }
            }
        }
        private double _featuresColumnWidth;

        /// <summary>
        /// Gets or sets the width of the 'ROM Path' column. When set, the settings information will also be updated.
        /// </summary>
        public double RomPathColumnWidth
        {
            get
            {
                return _romPathColumnWidth;
            }

            set
            {
                if (value > 0)
                {
                    AssignAndUpdateProperty("RomPathColumnWidth", value, ref _romPathColumnWidth, (n, w) => Properties.Settings.Default.RomListPathColWidth = w);
                }
            }
        }
        private double _romPathColumnWidth;

        /// <summary>
        /// Gets the command to execute to start dragging items in the table.
        /// </summary>
        public INTV.Shared.Behavior.IDragStartCommand DragProgramsStartCommand
        {
            get { return _dragProgramsStartCommand; }
        }
        private StartDragProgramsCommand _dragProgramsStartCommand;

        /// <summary>
        /// Gets the command to execute when items are dragged over the table.
        /// </summary>
        public RelayCommand DragOverCommand
        {
            get { return new RelayCommand(DragOver); }
        }

        /// <summary>
        /// Gets the command to execute to sort the contents of the table.
        /// </summary>
        public RelayCommand SortByColumnCommand
        {
            get { return new RelayCommand(SortByColumn, CanSortByColumn); }
        }

        private void DragOver(object dragEventArgs)
        {
            var dragEnterArgs = dragEventArgs as DragEventArgs;
            if (!dragEnterArgs.Data.GetDataPresent(DataFormats.FileDrop) && !dragEnterArgs.Data.GetDataPresent(ProgramDescriptionViewModel.DragDataFormat))
            {
                dragEnterArgs.Effects = DragDropEffects.None;
            }
        }

        private void FilesDropped(object dragEventArgs)
        {
            var dragDropArgs = dragEventArgs as DragEventArgs;
            var data = dragDropArgs.Data as IDataObject;
            var dataFormats = data.GetFormats(true);
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = data.GetData(DataFormats.FileDrop) as IEnumerable<string>;
                var options = RomDiscoveryOptions.AddNewRoms | RomDiscoveryOptions.AccumulateRejectedRoms;
                var args = new RomDiscoveryData(files, Programs.ModelCollection, -1, Resources.Strings.RomListViewModel_Progress_Title, options);
                AddPrograms(args);
                bool updatedSearchDirectories = false;
                foreach (var file in files)
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        updatedSearchDirectories |= _searchDirectories.Add(file);
                    }
                }
                if (updatedSearchDirectories)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private bool CanSortByColumn(object columnData)
        {
            var sortColumnData = (INTV.Shared.Behavior.SortCommandData)columnData;
            var sortColumn = (RomListColumn)sortColumnData.SortData;
            return sortColumn != RomListColumn.None;
        }

        private void SortByColumn(object columnData)
        {
            var sortColumnData = (INTV.Shared.Behavior.SortCommandData)columnData;
            Sort(sortColumnData.SortDirection, (RomListColumn)sortColumnData.SortData);
        }

        public void Sort(ListSortDirection direction, RomListColumn sortColumn)
        {
            if (sortColumn != RomListColumn.None)
            {
                ICollectionView dataView = System.Windows.Data.CollectionViewSource.GetDefaultView(Programs);
                dataView.SortDescriptions.Clear();
                SortDescription sd = new SortDescription(RomListColumnToPropertyName(sortColumn), direction);
                dataView.SortDescriptions.Add(sd);
                RomListColumn secondarySortProperty = RomListColumn.None;
                if (SecondarySortData.TryGetValue(sortColumn, out secondarySortProperty))
                {
                    dataView.SortDescriptions.Add(new SortDescription(RomListColumnToPropertyName(secondarySortProperty), ListSortDirection.Ascending));
                }
                dataView.Refresh();
            }
            SortDirection = direction;
            SortColumn = sortColumn;
        }

        private void ExecuteStartSelectedProgramsDrag(object parameter)
        {
            var itemsToDrag = new List<ProgramDescription>();
            _dragProgramsStartCommand.Programs = itemsToDrag;
            itemsToDrag.AddRange(CurrentSelection.Select(p => p.Model));
        }

        private bool CanRemoveRoms(object parameter)
        {
            return CurrentSelection.Count > 0;
        }

        private OSVisual OSInitializeVisual()
        {
            var visual = new RomListView();
            visual.DataContext = this; // not sure if this needs to be done...
            return visual;
        }

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        partial void OSInitialize()
        {
            _sortColumn = RomListColumn.None;
            _sortDirection = ListSortDirection.Ascending;

            // This null check keeps the WPF XAML designer output clean.
            if (INTV.Shared.Utility.SingleInstanceApplication.Instance != null)
            {
                var configuration = SingleInstanceApplication.Instance.GetConfiguration<RomListConfiguration>();
                var romListFilePath = configuration.RomFilesPath;
                _filePath = romListFilePath;
                Model = LoadRomList(romListFilePath);
                Model.SaveFailed += HandleSaveFailed;
                ((INotifyPropertyChanged)Model).PropertyChanged += HandleProgramCollectionPropertyChanged;
                Sort(Properties.Settings.Default.SortDirection, Properties.Settings.Default.SortColumn);
                _nameColumnWidth = Properties.Settings.Default.RomListNameColWidth;
                _vendorColumnWidth = Properties.Settings.Default.RomListVendorColWidth;
                _yearColumnWidth = Properties.Settings.Default.RomListYearColWidth;
                _featuresColumnWidth = Properties.Settings.Default.RomListFeaturesColWidth;
                _romPathColumnWidth = Properties.Settings.Default.RomListPathColWidth;
                _searchDirectories = Properties.Settings.Default.RomListSearchDirectories;
                if (_searchDirectories == null)
                {
                    _searchDirectories = new SearchDirectories();
                    Properties.Settings.Default.RomListSearchDirectories = _searchDirectories;
                }
                _dragProgramsStartCommand = new StartDragProgramsCommand(ExecuteStartSelectedProgramsDrag);
                if (Properties.Settings.Default.RomListSearchForRomsAtStartup && CanRefreshRoms(null))
                {
                    var options = RomDiscoveryOptions.AddNewRoms;
                    if (Properties.Settings.Default.RomListValidateAtStartup)
                    {
                        options |= RomDiscoveryOptions.DetectChanges | RomDiscoveryOptions.DetectMissingRoms | RomDiscoveryOptions.DetectNewRoms;
                    }
                    var taskData = new RomDiscoveryData(Properties.Settings.Default.RomListSearchDirectories, _programs.ModelCollection, -1, Resources.Strings.RomListViewModel_ScanningForRoms_Title, options);
                    SingleInstanceApplication.Instance.AddStartupAction("ScanForRoms", () => RefreshRoms(taskData), StartupTaskPriority.HighestAsyncTaskPriority);
                }
            }
            else
            {
                Model = INTV.Shared.Model.Program.ProgramCollection.EmptyDummyList;
                _nameColumnWidth = Properties.Settings.Default.RomListNameColWidth;
                _vendorColumnWidth = Properties.Settings.Default.RomListVendorColWidth;
                _yearColumnWidth = Properties.Settings.Default.RomListYearColWidth;
                _featuresColumnWidth = Properties.Settings.Default.RomListFeaturesColWidth;
                _romPathColumnWidth = Properties.Settings.Default.RomListPathColWidth;
            }
        }
    }
}
