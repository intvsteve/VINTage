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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;

using Converters = System.Tuple<System.Func<string, object>, System.Action<string, object>>;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    /// <remarks>GTK# currently supports GTK2, and does not have bindings for the newer DConf
    /// or GSettings APIs. We're using GConf.</remarks>
    /// <remarks>NOTE: The underlying GConf only supports a few basic types:</remarks>
    [DataContract(Namespace = "https://www.intvfunhouse.com")]
    public abstract partial class SettingsBase : INotifyPropertyChanged, IExtensibleDataObject
    {
        private ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();
        private HashSet<string> _applicationSettingsKeys = new HashSet<string>();
        private Dictionary<Type, Converters> _converters = new Dictionary<Type, Converters>();

        public ExtensionDataObject ExtensionData
        {
            get { return _extensibleDataObject; }
            set { _extensibleDataObject = value; }
        }
        private ExtensionDataObject _extensibleDataObject;

        protected string ComponentSettingsFilePath { get; set; }

        private string ApplicationSettingsDirectory { get; set; }

        private string ApplicationSettingsFilePath { get; set; }

        #region Events

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // Events

        protected void InitializeFromSettingsFile<T>() where T : new()
        {
            var dtoProperties = ValidateDataTransferObjectType<T>();
            var preferencesFilePath = ComponentSettingsFilePath;
            if (File.Exists(preferencesFilePath))
            {
                try
                {
                    using (var xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(FileUtilities.OpenFileStream(preferencesFilePath), new XmlDictionaryReaderQuotas()))
                    {
                        var dataContractSerializer = new DataContractSerializer(typeof(T));
                        var dtoSettingsData = dataContractSerializer.ReadObject(xmlDictionaryReader);
                        foreach (var dtoProperty in dtoProperties)
                        {
                            var settingValue = dtoProperty.GetValue(dtoSettingsData);
                            this[dtoProperty.Name] = settingValue;
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        /// <summary>
        /// Updates the setting and raises a property changed event as appropriate.
        /// </summary>
        /// <typeparam name="T">The data type of the setting.</typeparam>
        /// <param name="settingName">Setting name.</param>
        /// <param name="newValue">New value.</param>
        /// <param name="currentValue">Current value.</param>
        protected void UpdateSetting<T>(string settingName, T newValue, T currentValue)
        {
            this.UpdateProperty(PropertyChanged, settingName, newValue, currentValue, (s, v) => SetSetting(s, v));
        }

        /// <summary>
        /// Assigns and updates the setting value and raises a property changed event as appropriate.
        /// </summary>
        /// <typeparam name="T">The data type of the setting.</typeparam>
        /// <param name="settingName">Setting name.</param>
        /// <param name="newValue">New value.</param>
        /// <param name="currentValue">Current value.</param>
        protected void AssignAndUpdateSetting<T>(string settingName, T newValue, ref T currentValue)
        {
            this.AssignAndUpdateProperty(PropertyChanged, settingName, newValue, ref currentValue);
        }

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
                    //if (type.IsEnum)
                    //{
                    //    // Enums are always stored as strings.
                    //    try
                    //    {
                    //        value = Enum.Parse(type, value as string);
                    //    }
                    //    catch (Exception)
                    //    {
                    //        value = 0; // Parse failed, so return default for an enum.
                    //    }
                    //}
                }
                gotSetting = true;
            }
            catch(Exception)
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
        /// <returns>The value of the setting.</returns>
        /// <remarks>Since we're not using schemas, provide a simple brain-dead mechanism for
        /// derived classes to store more complex data. A typical example is how a Point is stored.
        /// The base value for <paramref name="key"/> will store two sub-values -- X and Y.
        /// This method will append the subkeys to the base key to form the absolute key to the setting.</remarks>
        protected object GetSetting(string key)
        {
            object value;
            if (!_values.TryGetValue(key, out value))
            {
                value = _defaults[key];
                _values.TryAdd(key, value);
            }
            return value;
        }

        /// <summary>
        /// Store the value for a setting
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <param name="value">The value to store for the setting.</param>
        /// <remarks>Since we're not using schemas, provide a simple brain-dead mechanism for
        /// derived classes to store more complex data. A typical example is how a Point is stored.
        /// The base value for <paramref name="key"/> will store two sub-values -- X and Y.
        /// This method will append the subkeys to the base key to form the absolute key to the setting.</remarks>
        protected void StoreSetting(string key, object value)
        {
            if (value.GetType().IsEnum)
            {
                // Always store Enums as string.
                //value = value.ToString();
            }
            var currentValue = GetSetting(key);
            if (_values.TryUpdate(key, value, currentValue))
            {
                this.RaisePropertyChanged(PropertyChanged, key);
            }
        }

        private IEnumerable<PropertyInfo> ValidateDataTransferObjectType<T>()
        {
            var myContract = this.GetType ().GetCustomAttribute(typeof(DataContractAttribute));
            var dtoContract = typeof(T).GetCustomAttribute(typeof (DataContractAttribute));

            var myProperties = this.GetType().GetPublicInstancePropertiesWithGetAndSet(HasDataMemberProperty);
            var dtoProperties = typeof(T).GetPublicInstancePropertiesWithGetAndSet(HasDataMemberProperty);

            var propertyErrors = new List<string>();
            var validProperties = new List<PropertyInfo>();
            foreach (var myProperty in myProperties)
            {
                var dtoProperty = dtoProperties.FirstOrDefault (p => p.Name == myProperty.Name);
                if (dtoProperty == null)
                {
                    propertyErrors.Add ($"Missing: {myProperty.Name}");
                }
                else if (dtoProperty.PropertyType != myProperty.PropertyType)
                {
                    propertyErrors.Add ($"Invalid data type: {myProperty.Name} - expected: {myProperty.PropertyType} actual: {dtoProperty.PropertyType}");
                }
                else
                {
                    validProperties.Add(dtoProperty);
                }
            }

            return validProperties;
        }

        private static bool HasDataMemberProperty(PropertyInfo propertyInfo)
        {
            var dataMemberAttribute = propertyInfo.GetCustomAttribute(typeof(DataMemberAttribute));
            var hasDataMemberProperty = dataMemberAttribute != null;
            return hasDataMemberProperty;
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        private void OSInitialize()
        {
            AddCustomTypeConverters();
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var appName = Path.GetFileNameWithoutExtension(entryAssembly.GetName().Name);

            ApplicationSettingsDirectory = Path.Combine(XdgEnvironmentVariable.Config.GetEnvironmentVariableValue(), appName);
            if (!Directory.Exists(ApplicationSettingsDirectory))
            {
                Directory.CreateDirectory(ApplicationSettingsDirectory);
            }

            ApplicationSettingsFilePath = Path.Combine(ApplicationSettingsDirectory, appName.ToLowerInvariant() + "rc");

            var componentName = this.GetType().Assembly.GetName().Name.ToLowerInvariant().Replace('.', '-');
            ComponentSettingsFilePath = Path.Combine(ApplicationSettingsDirectory, componentName + ".config");
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

        /// <summary>
        /// Saves the settings to disk.
        /// </summary>
        partial void OSSave()
        {
            try
            {
                var xmlWriter = XmlWriter.Create(ComponentSettingsFilePath, new XmlWriterSettings() { Indent = true });
                using (var xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter))
                {
                    var dataContractSerializer = new DataContractSerializer(this.GetType());
                    dataContractSerializer.WriteObject(xmlDictionaryWriter, this);
                    xmlDictionaryWriter.Flush();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
