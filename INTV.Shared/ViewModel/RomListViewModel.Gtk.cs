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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        internal static readonly Gtk.TargetEntry[] DragDropTargetEntries = new[]
        {
                new Gtk.TargetEntry("UTF8_STRING", Gtk.TargetFlags.OtherApp, (uint)DragDropDataType.Utf8String),
        };

        private enum DragDropDataType : uint
        {
            /// <summary>
            /// The custom value we use to register UTF8 string drag/drop data type Gtk.TargetEntry.
            /// </summary>
            Utf8String,
        }

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

        /// <summary>
        /// Gets the files dropped from OS-specific drag/drop event data.
        /// </summary>
        /// <param name="osDropArgs">OS-specific drag drop arguments.</param>
        /// <param name="droppedFiles">Recieves the dropped files retrieved from <paramref name="osDropArgs"/>.</param>
        /// <returns>The insert location of the new items.</returns>
        private int GetFilesDropped(object osDropArgs, List<string> droppedFiles)
        {
            var insertLocation = -1;
            var dropData = osDropArgs as System.Tuple<Gtk.DragDataReceivedArgs, Gtk.TreePath, Gtk.TreeViewDropPosition>;
            var dropArgs = dropData.Item1;
            if ((DragDropDataType)dropArgs.Info == DragDropDataType.Utf8String)
            {
                var droppedFileUris = dropArgs.SelectionData.Text.Split('\r', '\n').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p));
                foreach (var droppedFileUri in droppedFileUris)
                {
                    Uri fileUri;
                    if (Uri.TryCreate(droppedFileUri, UriKind.Absolute, out fileUri))
                    {
                        var file = fileUri.AbsolutePath;
                        droppedFiles.Add(file);
                    }
                }

                // A value of null for the Gtk.TreePath indicates after the end of the last item - so append.
                if (dropData.Item2 != null)
                {
                    insertLocation = dropData.Item2.Indices[0];
                    if ((dropData.Item3 & Gtk.TreeViewDropPosition.After) == Gtk.TreeViewDropPosition.After)
                    {
                        ++insertLocation;
                        if (insertLocation >= Programs.ModelCollection.Count)
                        {
                            insertLocation = -1;
                        }
                    }
                }
            }
            return insertLocation;
        }

        private void FilesDropped(object dropEventArgs)
        {
            List<string> files = new List<string>();
            var insertLocation = GetFilesDropped(dropEventArgs, files);
            if (files.Any())
            {
                var options = RomDiscoveryOptions.AddNewRoms | RomDiscoveryOptions.AccumulateRejectedRoms;
                var args = new RomDiscoveryData(files, Programs.ModelCollection, insertLocation, Resources.Strings.RomListViewModel_Progress_Title, options);
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
                    INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
                }
            }
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
                var taskData = new RomDiscoveryData(Properties.Settings.Default.RomListSearchDirectories, _programs.ModelCollection, -1, Resources.Strings.RomListViewModel_ScanningForRoms_Title, options);
                SingleInstanceApplication.Instance.AddStartupAction("ScanForRoms", () => RefreshRoms(taskData), StartupTaskPriority.HighestAsyncTaskPriority);
            }

            // TODO: Initialize sorting!
            // TODO: Drag/Drop?
        }
    }
}
