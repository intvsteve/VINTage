// <copyright file="FirmwareRevisionsViewModel.cs" company="INTV Funhouse">
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
    /// ViewModel for presenting firmware revision information.
    /// </summary>
    public class FirmwareRevisionsViewModel : INTV.Shared.ViewModel.ViewModelBase
    {
        public static readonly string FirmwareRevisionPrimary = Resources.Strings.FileSystemStatisticsView_FirmwareVersionPrimary;
        public static readonly string FirmwareRevisionSecondary = Resources.Strings.FileSystemStatisticsView_FirmwareVersionSecondary;
        public static readonly string FirmwareRevisionCurrent = Resources.Strings.FileSystemStatisticsView_FirmwareVersionCurrent;

        private FirmwareRevisions _firmwareRevisions;
        private string _firmwareVersionPrimary = Resources.Strings.FileSystemStatisticsView_Unavailable;
        private string _firmwareVersionSecondary = Resources.Strings.FileSystemStatisticsView_Unavailable;
        private string _firmwareVersionCurrent = Resources.Strings.FileSystemStatisticsView_Unavailable;
        private string _firmwareVersionPrimaryInternal = Resources.Strings.FileSystemStatisticsView_Unavailable;
        private string _firmwareVersionSecondaryInternal = Resources.Strings.FileSystemStatisticsView_Unavailable;
        private string _firmwareVersionCurrentInternal = Resources.Strings.FileSystemStatisticsView_Unavailable;

        /// <summary>
        /// Gets or sets the raw firmware revision data.
        /// </summary>
        public FirmwareRevisions FirmwareRevisions
        {
            get { return _firmwareRevisions; }
            set { AssignAndUpdateProperty(Device.FirmwareRevisionsPropertyName, value, ref _firmwareRevisions, (p, v) => UpdateFirmwareRevisions(v)); }
        }

        /// <summary>
        /// Gets the primary firmware version as a string.
        /// </summary>
        public string Primary
        {
            get { return _firmwareVersionPrimary; }
            internal set { AssignAndUpdateProperty("Primary", value, ref _firmwareVersionPrimary); }
        }

        /// <summary>
        /// Gets the secondary firmware version as a string.
        /// </summary>
        public string Secondary
        {
            get { return _firmwareVersionSecondary; }
            internal set { AssignAndUpdateProperty("Secondary", value, ref _firmwareVersionSecondary); }
        }

        /// <summary>
        /// Gets the current firmware version as a string.
        /// </summary>
        public string Current
        {
            get { return _firmwareVersionCurrent; }
            internal set { AssignAndUpdateProperty("Current", value, ref _firmwareVersionCurrent); }
        }

        /// <summary>
        /// Gets the primary firmware SVN version as a string.
        /// </summary>
        public string PrimaryInternal
        {
            get { return _firmwareVersionPrimaryInternal; }
            internal set { AssignAndUpdateProperty("PrimaryInternal", value, ref _firmwareVersionPrimaryInternal); }
        }

        /// <summary>
        /// Gets the secondary firmware SVN version as a string.
        /// </summary>
        public string SecondaryInternal
        {
            get { return _firmwareVersionSecondaryInternal; }
            internal set { AssignAndUpdateProperty("SecondaryInternal", value, ref _firmwareVersionSecondaryInternal); }
        }

        /// <summary>
        /// Gets the current firmware SVN version as a string.
        /// </summary>
        public string CurrentInternal
        {
            get { return _firmwareVersionCurrentInternal; }
            internal set { AssignAndUpdateProperty("CurrentInternal", value, ref _firmwareVersionCurrentInternal); }
        }

        private void UpdateFirmwareRevisions(FirmwareRevisions firmwareRevisions)
        {
            // Running vs. locutus_sim will pass null here.
            if (firmwareRevisions != null)
            {
                Primary = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Primary, false);
                Secondary = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Secondary, false);
                Current = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Current, false);
                PrimaryInternal = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Primary, true);
                SecondaryInternal = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Secondary, true);
                CurrentInternal = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Current, true);
            }
            else
            {
                // TODO Set to unavailable?
            }
        }
    }
}
