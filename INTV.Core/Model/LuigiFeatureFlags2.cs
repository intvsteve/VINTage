// <copyright file="LuigiFeatureFlags2.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Program;

namespace INTV.Core.Model
{
    /// <summary>
    /// These flags describe the features of a program ROM placed into a LUIGI file.
    /// </summary>
    /// <remarks>These features actually comprise a bit array based upon multiple applications of the more
    /// general FeatureCompatibility flags from INTV.Core.</remarks>
    [System.Flags]
    public enum LuigiFeatureFlags2 : ulong
    {
        /// <summary>
        /// No features reported.
        /// </summary>
        None = 0,

        /// <summary>
        /// Mask for unused feature bits.
        /// </summary>
        UnusedMask = 0xFFFFFFFFFFFFFFFF
    }

    /// <summary>
    /// Extension methods for LuigiFeatureFlags2.
    /// </summary>
    public static class LuigiFeatureFlags2Helpers
    {
        /// <summary>
        /// Convert LuigiFeatureFlags2 into a ProgramFeatures object.
        /// </summary>
        /// <param name="featureFlags2">The flags to convert,</param>
        /// <returns>ProgramFeatures representing the compatibility modes described by the feature flags.</returns>
        public static ProgramFeatures ToProgramFeatures(this LuigiFeatureFlags2 featureFlags2)
        {
            // No relevant features are currently defined in LuigiFeatureFlags2.
            var programFeatures = ProgramFeatures.EmptyFeatures.Clone();
            return programFeatures;
        }
    }
}
