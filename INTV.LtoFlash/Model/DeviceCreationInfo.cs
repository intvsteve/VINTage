// <copyright file="DeviceCreationInfo.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Information used to set behaviors of a Locutus device model when created.
    /// </summary>
    public class DeviceCreationInfo : System.Tuple<bool, bool, ActivationMode>
    {
        /// <summary>
        /// Name for the configuration data.
        /// </summary>
        public const string ConfigName = "DeviceCreationInfo";

        /// <summary>
        /// Initializes a new instance of the DeviceCreationInfo class.
        /// </summary>
        /// <param name="performFullValidation">If <c>true</c>, wait for beacon and perform full validation on device creation.</param>
        /// <param name="reportValidationError">If <c>true</c>, report any errors validating the device to the user.</param>
        /// <param name="activationMode">Take no action, make active device if no other active, or force device to become active.</param>
        public DeviceCreationInfo(bool performFullValidation, bool reportValidationError, ActivationMode activationMode)
            : base(performFullValidation, reportValidationError, activationMode)
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not full device validation should be done (i.e. WaitForBeacon).
        /// </summary>
        public bool PerformFullValidation
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets a value indicating whether or not device validation errors should be reported.
        /// </summary>
        public bool ReportValidationError
        {
            get { return Item2; }
        }

        /// <summary>
        /// Gets the activation mode of the device.
        /// </summary>
        public ActivationMode ActivationMode
        {
            get { return Item3; }
        }
    }
}
