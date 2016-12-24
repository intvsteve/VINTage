// <copyright file="ActivationMode.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Describes how to handle a newly created / discovered device.
    /// </summary>
    public enum ActivationMode
    {
        /// <summary>
        /// Let the user decide, based on preferences and circumstances.
        /// </summary>
        UserSettings,

        /// <summary>
        /// Do not make the new device the active one in the system.
        /// </summary>
        DoNotActivate,

        /// <summary>
        /// Activate the device if it's the first valid device that's been created.
        /// </summary>
        ActivateIfFirst,

        /// <summary>
        /// Force the device to become the active device.
        /// </summary>
        ForceActivate
    }
}
