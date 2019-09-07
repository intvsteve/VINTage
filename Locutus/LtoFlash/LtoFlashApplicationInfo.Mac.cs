// <copyright file="LtoFlashApplicationInfo.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

namespace Locutus
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    internal sealed partial class LtoFlashApplicationInfo
    {
        private const string OSString = "mac";
        private static readonly OSVersion OSVersionMinimum = new OSVersion(10, 7, 0); // Lion
        private static readonly OSVersion OSVersionRecommended = new OSVersion(10, 9, 0); // Mavericks

        /// <summary>
        /// Gets or sets the dummy settings.
        /// </summary>
        internal System.Configuration.ApplicationSettingsBase DummySettings { get; set; }
    }
}
