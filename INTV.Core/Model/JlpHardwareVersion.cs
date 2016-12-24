// <copyright file="JlpHardwareVersion.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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
using System.Linq;

namespace INTV.Core.Model
{
    /// <summary>
    /// Defines the available (or soon-to-be so) versions of JLP hardware.
    /// </summary>
    public enum JlpHardwareVersion
    {
        /// <summary>
        /// Used for describing programs that may be incompatible with JLP hardware. Are there any?
        /// </summary>
        Incompatible = -1,

        /// <summary>
        /// JLP hardware not used or required.
        /// </summary>
        None = 0,

        /// <summary>
        /// JLP03 - the first publicly defined version?
        /// </summary>
        Jlp03 = 3,

        /// <summary>
        /// JLP04 - the most widely used, though two form factors exist.
        /// </summary>
        Jlp04 = 4,

        /// <summary>
        /// JLP05 - unreleased? Has LEDs!
        /// </summary>
        Jlp05 = 5
    }

    /// <summary>
    /// Extension methods for JlpHardwareVersion.
    /// </summary>
    public static class JlpHardwareVersionHelpers
    {
        private static readonly Dictionary<JlpHardwareVersion, string> VersionStrings = new Dictionary<JlpHardwareVersion, string>()
        {
            { JlpHardwareVersion.Incompatible, Resources.Strings.JlpHardwareVersion_Incompatible },
            { JlpHardwareVersion.None, Resources.Strings.JlpHardwareVersion_None },
            { JlpHardwareVersion.Jlp03, Resources.Strings.JlpHardwareVersion_03 },
            { JlpHardwareVersion.Jlp04, Resources.Strings.JlpHardwareVersion_04 },
            { JlpHardwareVersion.Jlp05, Resources.Strings.JlpHardwareVersion_05 },
        };

        /// <summary>
        /// Converts a JLP version to a display string.
        /// </summary>
        /// <param name="jlpVersion">The version to convert to string.</param>
        /// <returns>The display string for the version.</returns>
        public static string ToDisplayString(this JlpHardwareVersion jlpVersion)
        {
            return VersionStrings[jlpVersion];
        }

        /// <summary>
        /// Converts an expected JLP version string back to the version enumeration.
        /// </summary>
        /// <param name="jlpHardwareVersion">The display string to convert back to an enum value.</param>
        /// <returns>The version as an enumeration.</returns>
        public static JlpHardwareVersion FromDisplayString(string jlpHardwareVersion)
        {
            return VersionStrings.FirstOrDefault(k => k.Value == jlpHardwareVersion).Key;
        }
    }
}
