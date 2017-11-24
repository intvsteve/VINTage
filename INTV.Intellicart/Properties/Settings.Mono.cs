// <copyright file=".cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

using INTV.Intellicart.Model;

namespace INTV.Intellicart.Properties
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    internal sealed partial class Settings : INTV.Shared.Properties.SettingsBase<Settings>
    {
        /// <summary>
        /// Gets or sets the setting that stores the name of the serial port used to communicate with an Intellicart.
        /// </summary>
        public string SerialPort
        {
            get { return GetSetting<string>(IntellicartSerialPortSettingName); }
            set { SetSetting(IntellicartSerialPortSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the serial port baud rate setting to communicate with an Intellicart.
        /// </summary>
        public int BaudRate
        {
            get { return GetSetting<int>(IntellicartBaudRateSettingName); }
            set { SetSetting(IntellicartBaudRateSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the serial port write timeout setting to communicate with an Intellicart.
        /// </summary>
        public int Timeout
        {
            get { return GetSetting<int>(IntellicartWriteTimeoutSettingName); }
            set { SetSetting(IntellicartWriteTimeoutSettingName, UpdateTimeout(value)); }
        }

        #region ISettings

        /// <inheritdoc/>
        protected override void InitializeDefaults()
        {
            AddSetting(IntellicartSerialPortSettingName, null);
            AddSetting(IntellicartBaudRateSettingName, IntellicartModel.DefaultBaudRate);
            AddSetting(IntellicartWriteTimeoutSettingName, IntellicartModel.DefaultWriteTimeout);
            OSInitializeDefaults();
        }

        #endregion // ISettings

        private int UpdateTimeout(int newValue)
        {
            var timeout = System.Math.Max(newValue, IntellicartModel.MinWriteTimeout);
            timeout = System.Math.Min(timeout, IntellicartModel.MaxWriteTimeout);
            return timeout;
        }

        private void TimeoutChanged(string propertyName)
        {
            var timeout = UpdateTimeout(Timeout);
            if (timeout != Timeout)
            {
                Timeout = timeout;
            }
            else
            {
                RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// OS-specific default setting initialization.
        /// </summary>
        partial void OSInitializeDefaults();
    }
}
