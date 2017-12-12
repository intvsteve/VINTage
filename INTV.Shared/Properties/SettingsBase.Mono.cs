// <copyright file="SettingsBase.Mono.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.ComponentModel;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// A simple wrapper around the native settings system, depending on platform.
    /// </summary>
    /// <remarks>This type really has virtually nothing in common with the type of the same name
    /// in <see cref="System.Configuration.SettingsBase"/>. Rather than use the Mono
    /// implementation of ConfigurationManager, ApplicationSettingsBase, et. al.,
    /// we'll provide what we need for our little corner of the universe and thinly as possible
    /// wrap the native GConf or Mac settings system. There are just enough bugs and other complaints
    /// floating around out there that this skeptic is just going to do The Wrap and
    /// deal with the extra work.</remarks>
    public abstract partial class SettingsBase : PropertyChangedNotifier, ISettings
    {
        private Dictionary<string, object> _defaults = new Dictionary<string, object>();

        /// <summary>
        /// Initialize a new instance of the <see cref="INTV.Shared.Properties.SettingsBase"/> class.
        /// </summary>
        protected SettingsBase()
        {
            OSInitialize();
            InitializeDefaults();

            // TODO: Consider adding support for exposing things via SettingsProperty / SettingsCollection?
            // This could lead down a rabbit hole of re-implementing provider, et. al.
        }

        /// <summary>
        /// Gets or sets the value of the specified settings property using the specified key.
        /// </summary>
        /// <param name="key">The name of the setting.</param>
        /// <returns>The setting value corresponding to <paramref name="key"/>.</returns>
        /// <remarks>The base implementation presumes that accessing the setting as type System.Object works.
        /// Implementations that provide type-specific get / set operations must override this method to
        /// properly work with those types.</remarks>
        public virtual object this[string key]
        {
            get
            {
                var setting = GetSetting<object>(key);
                return setting;
            }

            set
            {
                SetSetting(key, value);
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            OSSave();
        }

        /// <summary>
        /// Initialize the default values for the preferences.
        /// </summary>
        /// <remarks>Derived classes must implement this so the core implementation can handle
        /// connecting up to the native implementation and provide values for un-set settings.</remarks>
        protected abstract void InitializeDefaults();

        /// <summary>
        /// Add a setting and its default value.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="defaultValue">The default value of the setting.</param>
        protected void AddSetting(string key, object defaultValue)
        {
            _defaults.Add(key, defaultValue);
            OSAddSetting(key, defaultValue);
        }

        /// <summary>
        /// Get a strongly typed value for the setting.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="key">The identifier of the setting.</param>
        /// <returns>The setting whose value is desired.</returns>
        /// <remarks>If the backing storage for the setting does not contain a value, then the
        /// default value registered in AddSetting will be returned.</remarks>
        protected T GetSetting<T>(string key)
        {
            object value;
            if (!TryGetSetting(key, typeof(T), out value))
            {
                value = _defaults[key];
            }
            return (T)value;
        }

        /// <summary>
        /// Apply a value for the setting.
        /// </summary>
        /// <typeparam name="T">The data type of the setting.</typeparam>
        /// <param name="key">The identifier of the setting.</param>
        /// <param name="value">The new value for the setting.</param>
        protected void SetSetting<T>(string key, T value)
        {
            if (typeof(T).IsClass)
            {
                System.Diagnostics.Debug.Assert((object)value != null, "A value of <null> is not permitted!");
            }
            OSSetSetting(key, value);
        }

        /// <summary>
        /// OS-specific implementation to add a setting.
        /// </summary>
        /// <param name="key">The identifier for the setting.</param>
        /// <param name="defaultValue">The default value for the setting.</param>
        partial void OSAddSetting(string key, object defaultValue);

        /// <summary>
        /// OS-specific save implementation.
        /// </summary>
        partial void OSSave();
    }
}
