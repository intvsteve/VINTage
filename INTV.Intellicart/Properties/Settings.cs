// <copyright file="Settings.cs" company="INTV Funhouse">
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

namespace INTV.Intellicart.Properties
{
    /// <summary>
    /// Common Settings implementation for INTV.Intellicart.
    /// Defines names of the options. In Windows, these are not used -- these
    /// strings are from the .config file created by Visual Studio. Maybe some
    /// gracious soul with ample free time could write a tool to *generate* this
    /// file for the non-Windows platforms.
    /// </summary>
    internal sealed partial class Settings
    {
        public const string IntellicartSerialPortSettingName = "IntellicartSerialPort";
        public const string IntellicartBaudRateSettingName = "IntellicartBaudRate";
        public const string IntellicartWriteTimeoutSettingName = "IntellicartWriteTimeout";
    }
}
