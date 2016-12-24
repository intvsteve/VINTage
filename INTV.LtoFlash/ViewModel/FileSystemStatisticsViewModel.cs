// <copyright file="FileSystemStatisticsViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

using INTV.LtoFlash.Model;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for presenting file system statistics.
    /// </summary>
    public class FileSystemStatisticsViewModel : INTV.Shared.ViewModel.ViewModelBase
    {
        public static readonly string PhysicalFileSystemHeader = Resources.Strings.FileSystemStatisticsView_PhysicalFileSystem;
        public static readonly string VirtualFileSystemHeader = Resources.Strings.FileSystemStatisticsView_VirtualFileSystem;
        public static readonly string BlocksAvailable = Resources.Strings.FileSystemStatisticsView_BlocksAvailable;
        public static readonly string BlocksInUse = Resources.Strings.FileSystemStatisticsView_BlocksInUse;
        public static readonly string PhysicalBlocksInUseLabel = Resources.Strings.FileSystemStatisticsView_PhysicalBlocksInUse;
        public static readonly string BlocksClean = Resources.Strings.FileSystemStatisticsView_BlocksClean;
        public static readonly string BlocksTotal = Resources.Strings.FileSystemStatisticsView_BlocksTotal;
        public static readonly string PhysicalErasures = Resources.Strings.FileSystemStatisticsView_PhysicalSectorErasures;
        public static readonly string MetadataErasures = Resources.Strings.FileSystemStatisticsView_MetadataSectorErasures;
        public static readonly string VirtualToPhysicalVersion = Resources.Strings.FileSystemStatisticsView_VirtualToPhysicalMapVersion;
        public static readonly string FlashLifetimeHeader = Resources.Strings.FileSystemStatisticsView_FlashLifetimeHeader;
        public static readonly string PercentUsedByErasures = Resources.Strings.FileSystemStatisticsView_PercentUsedByErasures;
        public static readonly string PercentageUsedByVtoPMap = Resources.Strings.FileSystemStatisticsView_PercentUsedByJournal;
        public static readonly string FlashLifetimeRemaining = Resources.Strings.FileSystemStatisticsView_PercentRemaining;
        private static readonly string UnknownPercentage = Resources.Strings.FileSystemStatisticsView_UnknownPercentage;

        /// <summary>
        /// Gets or sets the model data.
        /// </summary>
        public FileSystemStatistics FileSystemStatistics
        {
            get { return _fileSystemStatistics; }
            set { AssignAndUpdateProperty(Device.FileSystemStatisticsPropertyName, value, ref _fileSystemStatistics, (p, v) => UpdateFileSystemStatistics(v)); }
        }
        private FileSystemStatistics _fileSystemStatistics;

        /// <summary>
        /// Gets a value reporting the number of virtual blocks available.
        /// </summary>
        public string VirtualBlocksAvailable
        {
            get { return _virtualBlocksAvailable; }
            private set { AssignAndUpdateProperty("VirtualBlocksAvailable", value, ref _virtualBlocksAvailable); }
        }
        private string _virtualBlocksAvailable = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        /// Gets a value reporting the number of virtual blocks in use.
        /// </summary>
        public string VirtualBlocksInUse
        {
            get { return _virtualBlocksInUse; }
            private set { AssignAndUpdateProperty("VirtualBlocksInUse", value, ref _virtualBlocksInUse); }
        }
        private string _virtualBlocksInUse = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        ///  Gets a value reporting the total number of virtual blocks.
        /// </summary>
        public string VirtualBlocksTotal
        {
            get { return _virtualBlocksTotal; }
            private set { AssignAndUpdateProperty("VirtualBlocksTotal", value, ref _virtualBlocksTotal); }
        }
        private string _virtualBlocksTotal = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        ///  Gets a value reporting the number of physical blocks available.
        /// </summary>
        public string PhysicalBlocksAvailable
        {
            get { return _physicalBlocksAvailable; }
            private set { AssignAndUpdateProperty("PhysicalBlocksAvailable", value, ref _physicalBlocksAvailable); }
        }
        private string _physicalBlocksAvailable = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        ///  Gets a value reporting the number of free physical blocks that are clean.
        /// </summary>
        public string PhysicalBlocksClean
        {
            get { return _physicalBlocksClean; }
            private set { AssignAndUpdateProperty("PhysicalBlocksClean", value, ref _physicalBlocksClean); }
        }
        private string _physicalBlocksClean = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        ///  Gets a value reporting the total number of physical blocks.
        /// </summary>
        public string PhysicalBlocksTotal
        {
            get { return _physicalBlocksTotal; }
            private set { AssignAndUpdateProperty("PhysicalBlocksTotal", value, ref _physicalBlocksTotal); }
        }
        private string _physicalBlocksTotal = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        ///  Gets a value reporting the number of physical blocks in use.
        /// </summary>
        public string PhysicalBlocksInUse
        {
            get { return _physicalBlocksInUse; }
            private set { AssignAndUpdateProperty("PhysicalBlocksInUse", value, ref _physicalBlocksInUse); }
        }
        private string _physicalBlocksInUse = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        ///  Gets a value reporting the lifetime erasure count as a string.
        /// </summary>
        public string PhysicalSectorErasures
        {
            get { return _physicalSectorErasures; }
            private set { AssignAndUpdateProperty("PhysicalSectorErasures", value, ref _physicalSectorErasures); }
        }
        private string _physicalSectorErasures = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        /// Gets a value reporting the metadata sector erasures as a string.
        /// </summary>
        public string MetadataSectorErasures
        {
            get { return _metadataSectorErasures; }
            private set { AssignAndUpdateProperty("MetadataSectorErasures", value, ref _metadataSectorErasures); }
        }
        private string _metadataSectorErasures = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        /// Gets a value reporting the V2PM map version as a string.
        /// </summary>
        public string VirtualToPhysicalMapVersion
        {
            get { return _virtualToPhysicalVersion; }
            private set { AssignAndUpdateProperty("VirtualToPhysicalMapVersion", value, ref _virtualToPhysicalVersion); }
        }
        private string _virtualToPhysicalVersion = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        /// Gets a value reporting the percentage of flash lifespan used by physical block erasures.
        /// </summary>
        public string PercentFlashLifetimeUsedByPhysicalBlockErasures
        {
            get { return _percentFlashLifetimeUsedByPhysicalBlockErasures; }
            private set { AssignAndUpdateProperty("PercentFlashLifetimeUsedByPhysicalBlockErasures", value, ref _percentFlashLifetimeUsedByPhysicalBlockErasures); }
        }
        private string _percentFlashLifetimeUsedByPhysicalBlockErasures = UnknownPercentage;

        /// <summary>
        /// Gets a value reporting percentage of flash lifespan used by the VtoP map.
        /// </summary>
        public string PercentageFlashLifetimeUsedByVirtualToPhysicalMap
        {
            get { return _percentFlashLifetimeUsedByVirtualToPhysicalMap; }
            private set { AssignAndUpdateProperty("PercentageFlashLifetimeUsedByVirtualToPhysicalMap", value, ref _percentFlashLifetimeUsedByVirtualToPhysicalMap); }
        }
        private string _percentFlashLifetimeUsedByVirtualToPhysicalMap = UnknownPercentage;

        /// <summary>
        /// Gets a value indicating percentage of lifespan of the flash remaining.
        /// </summary>
        public string PercentageLifetimeRemaining
        {
            get { return _percentageLifetimeRemaining; }
            private set { AssignAndUpdateProperty("PercentageLifetimeRemaining", value, ref _percentageLifetimeRemaining); }
        }
        private string _percentageLifetimeRemaining = UnknownPercentage;

        private void UpdateFileSystemStatistics(FileSystemStatistics fileSystemStatistics)
        {
            if (fileSystemStatistics == null)
            {
                VirtualBlocksAvailable = Resources.Strings.FileSystemStatisticsView_Unavailable;
                VirtualBlocksInUse = Resources.Strings.FileSystemStatisticsView_Unavailable;
                VirtualBlocksTotal = Resources.Strings.FileSystemStatisticsView_Unavailable;
                PhysicalBlocksAvailable = Resources.Strings.FileSystemStatisticsView_Unavailable;
                PhysicalBlocksClean = Resources.Strings.FileSystemStatisticsView_Unavailable;
                PhysicalBlocksTotal = Resources.Strings.FileSystemStatisticsView_Unavailable;
                PhysicalBlocksInUse = Resources.Strings.FileSystemStatisticsView_Unavailable;
                PhysicalSectorErasures = Resources.Strings.FileSystemStatisticsView_Unavailable;
                MetadataSectorErasures = Resources.Strings.FileSystemStatisticsView_Unavailable;
                VirtualToPhysicalMapVersion = Resources.Strings.FileSystemStatisticsView_Unavailable;
                PercentFlashLifetimeUsedByPhysicalBlockErasures = UnknownPercentage;
                PercentageFlashLifetimeUsedByVirtualToPhysicalMap = UnknownPercentage;
                PercentageLifetimeRemaining = UnknownPercentage;
            }
            else
            {
                VirtualBlocksAvailable = fileSystemStatistics.VirtualBlocksAvailable.ToString();
                VirtualBlocksInUse = fileSystemStatistics.VirtualBlocksInUse.ToString();
                VirtualBlocksTotal = fileSystemStatistics.VirtualBlocksTotal.ToString();
                PhysicalBlocksAvailable = fileSystemStatistics.PhysicalBlocksTotal.ToString();
                PhysicalBlocksInUse = fileSystemStatistics.PhysicalBlocksInUse.ToString();
                PhysicalBlocksClean = fileSystemStatistics.PhysicalBlocksClean.ToString();
                PhysicalBlocksTotal = fileSystemStatistics.PhysicalBlocksTotal.ToString();
                PhysicalSectorErasures = fileSystemStatistics.PhysicalSectorErasures.ToString();
                MetadataSectorErasures = fileSystemStatistics.MetadataSectorErasures.ToString();
                VirtualToPhysicalMapVersion = fileSystemStatistics.VirtualToPhysicalMapVersion.ToString();
                PercentFlashLifetimeUsedByPhysicalBlockErasures = (fileSystemStatistics.LifetimeUsedDueByPhysicalSectorErasure / 100).ToString("0.##%");
                PercentageFlashLifetimeUsedByVirtualToPhysicalMap = (fileSystemStatistics.LifetimeUsedByVirtualToPhysicalMapLog / 100).ToString("0.##%");
                PercentageLifetimeRemaining = (fileSystemStatistics.RemainingFlashLifetime / 100).ToString("0.##%");
            }
        }
    }
}
