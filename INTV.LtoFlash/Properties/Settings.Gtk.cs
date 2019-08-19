// <copyright file="Settings.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2018 All Rights Reserved
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

using System.Runtime.Serialization;

namespace INTV.LtoFlash.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [DataContract(Namespace = "https://www.intvfunhouse.com")]
    internal sealed partial class Settings
    {
        /// <summary>
        /// Gets or sets the width of the menu layout long name column.
        /// </summary>
        public int MenuLayoutLongNameColWidth
        {
            get { return GetSetting<int>(MenuLayoutLongNameColWidthSettingName); }
            set { SetSetting(MenuLayoutLongNameColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the menu layout short name column.
        /// </summary>
        public int MenuLayoutShortNameColWidth
        {
            get { return GetSetting<int>(MenuLayoutShortNameColWidthSettingName); }
            set { SetSetting(MenuLayoutShortNameColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the menu layout manual column.
        /// </summary>
        public int MenuLayoutManualColWidth
        {
            get { return GetSetting<int>(MenuLayoutManualColWidthSettingName); }
            set { SetSetting(MenuLayoutManualColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the width of the menu layout save data column.
        /// </summary>
        public int MenuLayoutSaveDataColWidth
        {
            get { return GetSetting<int>(MenuLayoutSaveDataColWidthSettingName); }
            set { SetSetting(MenuLayoutSaveDataColWidthSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the serial port read block size to use.
        /// </summary>
        [DataMember]
        public int LtoFlashSerialReadChunkSize
        {
            get { return GetSetting<int>(LtoFlashSerialReadChunkSizeSettingName); }
            set { SetSetting(LtoFlashSerialReadChunkSizeSettingName, value); }
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        partial void OSInitializeDefaults()
        {
            AddSetting(MenuLayoutLongNameColWidthSettingName, 256);
            AddSetting(MenuLayoutShortNameColWidthSettingName, 144);
            AddSetting(MenuLayoutManualColWidthSettingName, 168);
            AddSetting(MenuLayoutSaveDataColWidthSettingName, 128);
            AddSetting(LtoFlashSerialReadChunkSizeSettingName, 0);
        }
    }
}
