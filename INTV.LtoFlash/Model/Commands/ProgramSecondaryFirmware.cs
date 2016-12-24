// <copyright file="ProgramSecondaryFirmware.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Implements the command to program a Locutus firmware image loaded into RAM to become the active secondary firmware.
    /// </summary>
    internal sealed class ProgramSecondaryFirmware : ProtocolCommand
    {
        /// <summary>
        /// Default timeout for reading response data in milliseconds.
        /// </summary>
        public const int DefaultResponseTimeout = 5 * 60 * 1000;

        private ProgramSecondaryFirmware()
            : base(ProtocolCommandId.FirmwareProgramSecondary, DefaultResponseTimeout)
        {
        }

        /// <summary>
        /// Gets the instance of the  command.
        /// </summary>
        public static readonly ProgramSecondaryFirmware Instance = new ProgramSecondaryFirmware();
    }
}
