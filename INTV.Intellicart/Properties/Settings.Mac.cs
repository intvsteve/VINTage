// <copyright file="Settings.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Core.ComponentModel;
using INTV.Intellicart.Model;
using INTV.Intellicart.ViewModel;
using INTV.Shared.Utility;

namespace INTV.Intellicart.Properties
{
    /// <summary>
    /// Settings for the Intellicart.
    /// </summary>
    public class Settings : PropertyChangedNotifier
    {
        private Settings()
        {
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.IntellicartSerialPortPropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.IntellicartBaudRatePropertyName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(SettingsPageViewModel.IntellicartWriteTimeoutPropertyName, TimeoutChanged);
        }

        /// <summary>
        /// Gets the default settings object.
        /// </summary>
        public static Settings Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }
        private static Settings _instance;

        private static NSUserDefaults UserDefaults
        {
            get
            {
                if (_userDefaults == null)
                {
                    _userDefaults = NSUserDefaults.StandardUserDefaults;
                    var defaults = new NSMutableDictionary();
                    defaults[SettingsPageViewModel.IntellicartSerialPortPropertyName] = new NSString(string.Empty);
                    defaults[SettingsPageViewModel.IntellicartBaudRatePropertyName] = new NSNumber(IntellicartModel.DefaultBaudRate);
                    defaults[SettingsPageViewModel.IntellicartWriteTimeoutPropertyName] = new NSNumber(IntellicartModel.DefaultWriteTimeout);
                    _userDefaults.RegisterDefaults(defaults);
                }
                return _userDefaults;
            }
        }
        private static NSUserDefaults _userDefaults;

        /// <summary>
        /// Gets or sets the setting that stores the name of the serial port used to communicate with an Intellicart.
        /// </summary>
        public string SerialPort
        {
            get { return UserDefaults.StringForKey(SettingsPageViewModel.IntellicartSerialPortPropertyName); }
            set { UserDefaults.SetString(value, SettingsPageViewModel.IntellicartSerialPortPropertyName); }
        }

        /// <summary>
        /// Gets or sets the serial port baud rate setting to communicate with an Intellicart.
        /// </summary>
        public int BaudRate
        {
            get { return (int)UserDefaults.IntForKey(SettingsPageViewModel.IntellicartBaudRatePropertyName); }
            set { UserDefaults.SetInt(value, SettingsPageViewModel.IntellicartBaudRatePropertyName); }
        }

        /// <summary>
        /// Gets or sets the serial port write timeout setting to communicate with an Intellicart.
        /// </summary>
        public int Timeout
        {
            get { return (int)UserDefaults.IntForKey(SettingsPageViewModel.IntellicartWriteTimeoutPropertyName); }
            set { UserDefaults.SetInt(UpdateTimeout(value), SettingsPageViewModel.IntellicartWriteTimeoutPropertyName); }
        }

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
    }
}
