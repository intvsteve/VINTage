// <copyright file="EnableFeature.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
namespace INTV.JzIntvUI.Model
{
    /// <summary>
    /// Options for how to determine options for jzIntv to enable or disable an emulator feature.
    /// </summary>
    public enum EnableFeature
    {
        /// <summary>The default setting.</summary>
        Default = UseRomSetting,

        /// <summary>Use information about the ROM to determine whether to enable or disable the feature.</summary>
        UseRomSetting = 0,

        /// <summary>Always enable the feature.</summary>
        Always = 1,

        /// <summary>Never enable the feature.</summary>
        Never = 2
    }

    /// <summary>
    /// Helper methods for the EnableFeature enumeration.
    /// </summary>
    public static class EnableFeatureHelpers
    {
        /// <summary>
        /// Given a string stored in a preferences file, convert it to a value from the EnableFeature enumeration.
        /// </summary>
        /// <param name="setting">The string to convert.</param>
        /// <returns>The corresponding value in the EnableFeature enumeration, or EnableFeature.Default if no match is found.</returns>
        public static EnableFeature FromSettingString(string setting)
        {
            EnableFeature enable;
            if (string.IsNullOrEmpty(setting) || !System.Enum.TryParse<EnableFeature>(setting, out enable))
            {
                enable = EnableFeature.Default;
            }
            return enable;
        }
    }
}
