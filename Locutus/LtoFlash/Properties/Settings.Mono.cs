// <copyright file="Settings.Mono.cs" company="INTV Funhouse">
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

using INTV.Shared.Properties;

#if MAC
#if __UNIFIED__
using SplitterPosition = System.nfloat;
#else
using SplitterPosition = System.Single;
#endif // __UNIFIED__
#elif GTK
using SplitterPosition = System.Int32;
#endif

namespace LtoFlash.Properties
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    internal sealed partial class Settings : SettingsBase<Settings>
    {
        private static readonly SplitterPosition DefaultSplitterPosition = 444;

        #region Properties

        /// <summary>
        /// Gets or sets the setting indicating whether to validate the ROMs at startup.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if you attempt to assign the property.</exception>
        public SplitterPosition PrimarySplitterPosition
        {
            get { return GetSetting<SplitterPosition>(PrimarySplitterPositionSettingName); }
            set { SetSetting(PrimarySplitterPositionSettingName, value); }
        }

        #endregion // Properties

        #region ISettings

        /// <inheritdoc/>
        protected override void InitializeDefaults()
        {
            AddSetting(PrimarySplitterPositionSettingName, DefaultSplitterPosition);
            OSInitializeDefaults();
        }

        #endregion // ISettings

        /// <summary>
        /// Operating system-specific default setting initialization goes here.
        /// </summary>
        partial void OSInitializeDefaults();
    }
}
