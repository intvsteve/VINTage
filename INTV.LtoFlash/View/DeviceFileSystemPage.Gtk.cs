// <copyright file="DeviceFileSystemPage.Gtk.cs" company="INTV Funhouse">
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
//

using INTV.LtoFlash.ViewModel;
using INTV.Shared.View;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Device file system page visual for GTK.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceFileSystemPage : Gtk.Bin, IFakeDependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.DeviceFileSystemPage"/> class.
        /// </summary>
        public DeviceFileSystemPage()
        {
            this.Build();
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public LtoFlashViewModel ViewModel
        {
            get { return DataContext as LtoFlashViewModel; }
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <summary>
        /// Update this instance to display data from the DataContext (current device).
        /// </summary>
        /// <remarks>We track that we're updating due to changes from the ViewModel to avoid multiple changes
        /// that can happen when user changes a setting on hardware.</remarks>
        internal void Update()
        {
            _physicalBlocksInUse.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PhysicalBlocksInUse);
            _physicalBlocksClean.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PhysicalBlocksClean);
            _physicalBlocksTotal.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PhysicalBlocksTotal);
            _virtualBlocksInUse.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.VirtualBlocksInUse);
            _virtualBlocksAvailable.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.VirtualBlocksAvailable);
            _virtualBlocksTotal.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.VirtualBlocksTotal);
            _physicalSectorErasures.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PhysicalSectorErasures);
            _metadataSectorErasures.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.MetadataSectorErasures);
            _fileSystemJournalWrapCount.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.VirtualToPhysicalMapVersion);
            _flashUsedByPhysicalBlockErasures.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PercentFlashLifetimeUsedByPhysicalBlockErasures);
            _flashUsedByFileSystemJournal.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PercentageFlashLifetimeUsedByVirtualToPhysicalMap);
            _flashLifetimeRemaining.Markup = FormatStringForDisplay(ViewModel.FileSystemStatistics.PercentageLifetimeRemaining);
        }

        private static string FormatStringForDisplay(string stringToFormat)
        {
            var formattedString = "<small>" + GLib.Markup.EscapeText(stringToFormat) + "</small>";
            return formattedString;
        }
    }
}
