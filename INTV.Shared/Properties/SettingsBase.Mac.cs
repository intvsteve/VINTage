// <copyright file="SettingsBase.Mac.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.Utility;

#if __UNIFIED__
using Converters = System.Tuple<System.Func<string, object>, System.Action<string, object>, System.Func<object, Foundation.NSObject>>;
#else
using Converters = System.Tuple<System.Func<string, object>, System.Action<string, object>, System.Func<object, MonoMac.Foundation.NSObject>>;
#endif // __UNIFIED__

namespace INTV.Shared.Properties
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public abstract partial class SettingsBase
    {
        private NSUserDefaults _userDefaults;
        private Dictionary<Type, Converters> _converters;

        /// <summary>
        /// Gets the native user defaults instance for the settings.
        /// </summary>
        /// <remarks>Subclasses with custom data types or special behaviors for default values will need access to this.</remarks>
        protected NSUserDefaults UserDefaults
        {
            get
            {
                if (_userDefaults == null)
                {
                    _userDefaults = NSUserDefaults.StandardUserDefaults;
                }
                return _userDefaults;
            }
        }

        /// <summary>
        /// Initialize the native user defaults.
        /// </summary>
        protected void InitializeUserDefaults()
        {
            var defaults = new NSMutableDictionary();
            foreach (var defaultSetting in _defaults)
            {
                Converters converterFuncs;
                if ((defaultSetting.Value != null) && _converters.TryGetValue(defaultSetting.Value.GetType(), out converterFuncs))
                {
                    var nativeDefaultCreator = converterFuncs.Item3;
                    if (nativeDefaultCreator != null)
                    {
                        var nativeDefaultValue = nativeDefaultCreator(defaultSetting.Value);
                        defaults[defaultSetting.Key] = nativeDefaultValue;
                    }
                }
            }
            UserDefaults.RegisterDefaults(defaults);
        }

        /// <summary>
        /// Derived classes may need to provide custom type converters.
        /// </summary>
        /// <remarks>If this method is overridden, it is likely to also necessary to
        /// override the Item indexer implementation.</remarks>
        protected virtual void AddCustomTypeConverters()
        {
            AddCustomTypeConverter(typeof(int), k => (int)UserDefaults.IntForKey(k), (k, v) => UserDefaults.SetInt((int)v, k), v => new NSNumber((int)v));
            AddCustomTypeConverter(typeof(bool), k => UserDefaults.BoolForKey(k), (k, v) => UserDefaults.SetBool((bool)v, k), v => new NSNumber((bool)v));
            AddCustomTypeConverter(typeof(string), k => UserDefaults.StringForKey(k), UpdateStringSetting, v => (NSString)v);
        }

        /// <summary>
        /// Add a custom type converter.
        /// </summary>
        /// <param name="type">The data type of the setting.</param>
        /// <param name="getter">The delegate to call to retrieve a setting value for the type.</param>
        /// <param name="setter">The delegate to call to store a setting value for the type.</param>
        /// <param name="nativeValueCreator">The delegate to call to create the native value for a key of the given type.</param>
        /// <remarks>Only one set of converter functions for a given type may be registered.</remarks>
        protected void AddCustomTypeConverter(Type type, Func<string, object> getter, Action<string, object> setter, Func<object, NSObject> nativeValueCreator)
        {
            _converters.Add(type, new Tuple<Func<string, object>, Action<string, object>, Func<object, NSObject>>(getter, setter, nativeValueCreator));
        }

        private void UpdateStringSetting(string settingName, object value)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                UserDefaults.RemoveObject(settingName);
            }
            else
            {
                UserDefaults.SetString((string)value, settingName);
            }
        }

        /// <summary>
        /// Attempt to get a setting.
        /// </summary>
        /// <param name="key">The identifier of the setting.</param>
        /// <param name="type">The data type of the setting.</param>
        /// <param name="value">Receives the value of the setting, or <c>null</c> if not found.</param>
        /// <returns><c>true</c>, if get setting was retrieved, <c>false</c> otherwise.</returns>
        /// <remarks>The implementation will store Enum-type values as strings. Therefore, derived
        /// types need to provide custom type converters for enum values.</remarks>
        protected bool TryGetSetting(string key, Type type, out object value)
        {
            bool gotSetting = false;
            try
            {
                Converters converters;
                if (_converters.TryGetValue(type, out converters))
                {
                    value = converters.Item1(key);
                }
                else
                {
                    throw new InvalidOperationException("No getter for setting: " + key + " of type: " + type);
                    /*
                    value = GetSetting(key);
                    if (type.IsEnum)
                    {
                        // Enums are always stored as strings.
                        try
                        {
                            value = Enum.Parse(type, value as string);
                        }
                        catch (Exception)
                        {
                            value = 0; // Parse failed, so return default for an enum.
                        }
                    } */
                }
                gotSetting = true;
            }
            catch (Exception)
            {
                value = null;
            }
            return gotSetting;
        }

        /// <summary>
        /// Mac-specific implementation to store a setting value.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="value">The new value for the setting.</param>
        protected void OSSetSetting(string key, object value)
        {
            if (value == null)
            {
                UserDefaults.RemoveObject(key);
            }
            else
            {
                Converters converters;
                if (_converters.TryGetValue(value.GetType(), out converters))
                {
                    converters.Item2(key, value);
                }
                else
                {
                    throw new InvalidOperationException("No setter for setting: " + key + " of type: " + value.GetType());
                }
            }
        }

        /// <summary>
        /// Mac-specific initialization.
        /// </summary>
        private void OSInitialize()
        {
            _converters = new Dictionary<Type, Converters>();
            AddCustomTypeConverters();
        }

        /// <summary>
        /// Mac-specific behavior for adding a setting.
        /// </summary>
        /// <param name="key">Key for the setting.</param>
        /// <param name="defaultValue">The default value for the setting.</param>
        partial void OSAddSetting(string key, object defaultValue)
        {
            //var type = defaultValue.GetType();
            NSUserDefaultsObserver.AddPreferenceChangedNotification(key, RaisePropertyChanged);
        }

        /// <summary>
        /// Saves the settings to disk.
        /// </summary>
        partial void OSSave()
        {
            UserDefaults.Synchronize();
        }
    }
}
