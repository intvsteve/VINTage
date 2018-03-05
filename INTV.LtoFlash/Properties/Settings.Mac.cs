// <copyright file="Settings.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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

using INTV.Shared.Utility;

namespace INTV.LtoFlash.Properties
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    internal sealed partial class Settings
    {
        /// <summary>
        /// The name of the Locutus serial port write chunk size setting.
        /// </summary>
        public const string LtoFlashSerialWriteChunkSizeSettingName = "LtoFlashSerialWriteChunkSize";

        /// <summary>
        /// The minimum OS version requiring restricted read block sizes.
        /// </summary>
        public static readonly OSVersion OSVersionRequiringRestrictedReadBlockSize = new OSVersion(10, 13, 0);

        /// <summary>
        /// Gets the default size of the serial port read chunk in bytes.
        /// </summary>
        internal static int DefaultReadChunkSize
        {
            get
            {
                var defaultChunkSize = 0;
                if (OSVersion.Current >= OSVersionRequiringRestrictedReadBlockSize)
                {
                    // It has been found that trying to read 'large' blocks from Locutus encounters timeout
                    // errors. We suspect it's an inter-byte timeout problem -- possibly due to the out-of-band
                    // bytes the FTDI serial interface uses. This has not been confirmed.
                    // Further, the issue seems specific to macOS High Sierra (10.13) -- at least that was the
                    // first macOS version against which the problem was reported and debugged.
                    defaultChunkSize = 512;
                }
                return defaultChunkSize;
            }
        }

        /// <summary>
        /// Gets the default size of the serial port write chunk in bytes.
        /// </summary>
        internal static int DefaultWriteChunkSize
        {
            get
            {
                // Provisionally preparing for this...
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the serial port read block size to use.
        /// </summary>
        public int LtoFlashSerialReadChunkSize
        {
            get { return GetSetting<int>(LtoFlashSerialReadChunkSizeSettingName); }
            set { SetSetting(LtoFlashSerialReadChunkSizeSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the serial port write block size to use.
        /// </summary>
        public int LtoFlashSerialWriteChunkSize
        {
            get { return GetSetting<int>(LtoFlashSerialWriteChunkSizeSettingName); }
            set { SetSetting(LtoFlashSerialWriteChunkSizeSettingName, value); }
        }

        /// <summary>
        /// Mac-specific initialization.
        /// </summary>
        partial void OSInitializeDefaults()
        {
            AddSetting(LtoFlashSerialReadChunkSizeSettingName, DefaultReadChunkSize);
            AddSetting(LtoFlashSerialWriteChunkSizeSettingName, DefaultWriteChunkSize);
            InitializeUserDefaults();
        }
    }
}
