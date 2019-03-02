// <copyright file="DeviceActivity.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This enumerable lists the kinds of device 'activities' that may execute a specific points in time, such as
    /// at device connect or application startup.
    /// </summary>
    internal enum DeviceActivity
    {
        /// <summary>
        /// Nothing to do - sentinel value.
        /// </summary>
        None = 0,

        /// <summary>
        /// When device connects, reconcile the menu layout in the editor with the menu layout on the device.
        /// </summary>
        ReconcileMenuOnConnect,

        /// <summary>
        /// When device connects, check whether a firmware update is available and if so, prompt to apply it.
        /// </summary>
        PromptForFirmwareUpdateOnConnect,
    }
}
