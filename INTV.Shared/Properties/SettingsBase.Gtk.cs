// <copyright file="SettingsBase.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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
using System.Linq;

using Converters = System.Tuple<System.Func<string, object>, System.Action<string, object>>;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    /// <remarks>GTK# currently supports GTK2, and does not have bindings for the newer DConf
    /// or GSettings APIs. We're using GConf.</remarks>
    /// <remarks>NOTE: The underlying GConf only supports a few basic types:</remarks>
    public abstract partial class SettingsBase
    {
        private GConf.Client _client;
        private HashSet<string> _applicationSettingsKeys = new HashSet<string>();
        private Dictionary<Type, Converters> _converters;

        /// <summary>
        /// Gets the GConf client for accessing settings.
        /// </summary>
        protected GConf.Client Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new GConf.Client();
                }
                return _client;
            }
        }

        private string ConfigAppPath { get; set; }

        private string ComponentName { get; set; }

        /// <summary>
        /// Derived classes may need to provide custom type converters.
        /// </summary>
        /// <remarks>If this method is overridden, it is likely to also necessary to
        /// override the Item indexer implementation.</remarks>
        protected virtual void AddCustomTypeConverters()
        {
        }

        /// <summary>
        /// Add a custom type converter.
        /// </summary>
        /// <param name="type">The data type of the setting.</param>
        /// <param name="getter">The delegate to call to retrieve a setting value for the type.</param>
        /// <param name="setter">The delegate to call to store a setting value for the type.</param>
        /// <remarks>Only one set of converter functions for a given type may be registered.</remarks>
        protected void AddCustomTypeConverter(Type type, Func<string, object> getter, Action<string, object> setter)
        {
            _converters.Add(type, new Tuple<Func<string, object>, Action<string, object>>(getter, setter));
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
                    }
                }
                gotSetting = true;
            }
            catch (GConf.NoSuchKeyException)
            {
                value = null;
            }
            return gotSetting;
        }

        /// <summary>
        /// GTK-specific implementation to store a setting value.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="value">The new value for the setting.</param>
        protected void OSSetSetting(string key, object value)
        {
            Converters converters;
            if (_converters.TryGetValue(value.GetType(), out converters))
            {
                converters.Item2(key, value);
            }
            else
            {
                StoreSetting(key, value);
            }
        }

        /// <summary>
        /// Get the value for a setting.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="subkeys">Additional sub-key data.</param>
        /// <returns>The value of the setting.</returns>
        /// <remarks>Since we're not using schemas, provide a simple brain-dead mechanism for
        /// derived classes to store more complex data. A typical example is how a Point is stored.
        /// The base value for <paramref name="key"/> will store two sub-values -- X and Y.
        /// This method will append the subkeys to the base key to form the absolute key to the setting.</remarks>
        protected object GetSetting(string key, params string[] subkeys)
        {
            var gconfKey = GetAbsoluteKey(key, subkeys);
            var value = Client.Get(gconfKey);
            return value;
        }

        /// <summary>
        /// Store the value for a setting
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="value">The value to store for the setting.</param>
        /// <param name="subkeys">Additional sub-key data.</param>
        /// <remarks>Since we're not using schemas, provide a simple brain-dead mechanism for
        /// derived classes to store more complex data. A typical example is how a Point is stored.
        /// The base value for <paramref name="key"/> will store two sub-values -- X and Y.
        /// This method will append the subkeys to the base key to form the absolute key to the setting.</remarks>
        protected void StoreSetting(string key, object value, params string[] subkeys)
        {
            var gconfKey = GetAbsoluteKey(key, subkeys);
            if (value.GetType().IsEnum)
            {
                // Always store Enums as string.
                value = value.ToString();
            }
            Client.Set(gconfKey, value);
        }

        /// <summary>
        /// Get the full key to use in GConf.
        /// </summary>
        /// <param name="key">The base key for a setting.</param>
        /// <param name="subkeys">Additional sub-key data.</param>
        /// <returns>The absolute key for the setting in GConf.</returns>
        protected string GetAbsoluteKey(string key, params string[] subkeys)
        {
            var absoluteKey = string.Join("/", ConfigAppPath, ComponentName, key);
            if (subkeys.Any())
            {
                var additional = string.Join("/", subkeys);
                absoluteKey = string.Join("/", absoluteKey, additional);
            }
            return absoluteKey;
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        private void OSInitialize()
        {
            _converters = new Dictionary<Type, Tuple<Func<string, object>, Action<string, object>>>();
            AddCustomTypeConverters();
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var appName = System.IO.Path.GetFileNameWithoutExtension(entryAssembly.GetName().Name);
            ConfigAppPath = "/apps/" + appName;
            ComponentName = this.GetType().FullName;
            Client.AddNotify(string.Join("/", ConfigAppPath, ComponentName), HandleSettingChanged);
        }

        /// <summary>
        /// GTK-specific behavior for adding a setting.
        /// </summary>
        /// <param name="key">Key for the setting.</param>
        /// <param name="defaultValue">The default value for the setting.</param>
        /// <param name="isApplicationSetting">If <c>true</c>, indicates the setting is for the application and not
        /// the specific instance of the Settings class.</param>
        partial void OSAddSetting(string key, object defaultValue, bool isApplicationSetting)
        {
            if (isApplicationSetting)
            {
                _applicationSettingsKeys.Add(key);
            }
        }

        private void HandleSettingChanged(object sender, GConf.NotifyEventArgs args)
        {
            // We just need the last part of the key.
            var key = args.Key.Split(new[] { '/' }).Last();
            this.RaisePropertyChanged(key);
        }
    }
}
