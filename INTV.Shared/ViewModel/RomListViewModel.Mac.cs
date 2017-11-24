// <copyright file="RomListViewModel.Mac.cs" company="INTV Funhouse">
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
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class RomListViewModel
    {
        /// <summary>
        /// Data format string used to support file system drag-drop operations.
        /// </summary>
        internal static readonly string DragDropFilesDataFormat = "public.file-url";

        /// <summary>
        /// Perform Mac-specific initialization.
        /// </summary>
        partial void OSInitialize()
        {
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
        }

        /// <summary>
        /// Gets the drag enter effects.
        /// </summary>
        /// <param name="dragArgs">Drag information used to determine drag-drop effects.</param>
        /// <returns>The drag-drop effects to apply when a drag operation enters the ROM list.</returns>
        internal NSDragOperation GetDragEnterEffects(NSDraggingInfo dragArgs)
        {
            var effects = NSDragOperation.None;
            if (dragArgs.DraggingPasteboard.CanReadItemWithDataConformingToTypes(new string[] { DragDropFilesDataFormat }) && !dragArgs.DraggingPasteboard.CanReadItemWithDataConformingToTypes(new string[] { ProgramDescriptionViewModel.DragDataFormat }))
            {
                effects = NSDragOperation.Link;
            }
            return effects;
        }

        partial void GetFilesDropped(object osDropArgs, List<string> droppedFiles)
        {
            var dropArgs = osDropArgs as NSDraggingInfo;
            if (dropArgs.DraggingPasteboard.CanReadItemWithDataConformingToTypes(new string[] { DragDropFilesDataFormat }))
            {
                foreach (var filePasteboardItem in dropArgs.DraggingPasteboard.PasteboardItems)
                {
                    var fileUrl = NSUrl.FromString(filePasteboardItem.GetStringForType(DragDropFilesDataFormat));
                    var file = fileUrl.Path;
                    droppedFiles.Add(file);
                }
            }
        }

        private void FilesDropped(object dropEventArgs)
        {
            List<string> files = new List<string>();
            GetFilesDropped(dropEventArgs, files);
            if (files.Any())
            {
                var options = RomDiscoveryOptions.AddNewRoms | RomDiscoveryOptions.AccumulateRejectedRoms;
                var args = new RomDiscoveryData(files, Programs.ModelCollection, Resources.Strings.RomListViewModel_Progress_Title, options);
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

        private INTV.Shared.View.OSVisual OSInitializeVisual()
        {
            var controller = new INTV.Shared.View.RomListViewController();
            return controller.View;
        }
    }
}
