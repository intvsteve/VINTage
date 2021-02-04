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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    /// <remarks>The GTK implementation uses the Data Contract mechanism for storing settings, except for
    /// those settings tagged as 'application' settings. Any setting that should be saved must have the
    /// <see cref="DataMemberAttribute"/> associated with it. Classes should also provide a separate
    /// Data Transfer Object type for deserialization, which is used to read settings from a file that are
    /// then assigned to the actual <see cref="SettingsBase"/> instance.</remarks>
    [DataContract(Namespace = "https://www.intvfunhouse.com")]
    public abstract partial class SettingsBase : INotifyPropertyChanged, IExtensibleDataObject
    {
        private static readonly Lazy<ApplicationSettings> SharedApplicationSettings = new Lazy<ApplicationSettings>();
        private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();
        private readonly HashSet<string> _applicationSettingsKeys = new HashSet<string>();
        private readonly Dictionary<Type, TypeConverter> _converters = new Dictionary<Type, TypeConverter>();

        public ExtensionDataObject ExtensionData
        {
            get { return _extensibleDataObject; }
            set { _extensibleDataObject = value; }
        }
        private ExtensionDataObject _extensibleDataObject;

        /// <summary>
        /// Gets the weight of the settings relative to other implementations.
        /// </summary>
        /// <remarks>This value is typically the same as the main <see cref="IComponent"/> provided by a plugin assembly
        /// It is mainly used to provide consistent ordering of the settings collected together in the shared
        /// application-level settings file.</remarks>
        public abstract double Weight { get; }

        /// <summary>
        /// Gets the name of the descriptions resource.
        /// </summary>
        /// <remarks>Most implementations can leave this as a value of <c>null</c> - provided they follow
        /// the convention that the default namespace used in the assembly matches the assembly name. For
        /// convenience, resources that provide descriptions of the application-level settings an assembly
        /// provides will be extracted from this resource. The grouping will use a resource key of
        /// 'SettingsDescription' while individual settings will use the name of the settings property
        /// as the key into the string resource. For an assembly named, for example, 'INTV.Shared' with the
        /// same default namespace, if this value is <c>null</c>, the resources will be expected to be found
        /// in a resource named `INTV.Shared.Resources.Strings`. If this value is not <c>null</c>, it must
        /// be the name of a strings resource that can be accessed via the ResourceManager API.</remarks>
        public virtual string DescriptionsResourceName { get; }

        /// <summary>
        /// Gets or sets the absolute path for the component's settings file.
        /// </summary>
        /// <value>The component settings file path.</value>
        protected string ComponentSettingsFilePath { get; set; }

        private static ApplicationSettings AppSettings
        {
            get { return SharedApplicationSettings.Value; }
        }

        #region Events

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // Events

        /// <summary>
        /// Loads the application-level settings from disk.
        /// </summary>
        /// <remarks>This method merely loads the raw string data from disk. Transferral of data values from
        /// this storage to the proper instances of <see cref="SettingsBase"/> happens during the initialization
        /// of the <see cref="SettingsBase"/> instance itself.</remarks>
        internal static void LoadApplicationSettings()
        {
            AppSettings.Load();
        }

        /// <summary>
        /// Saves the application-level settings to disk.
        /// </summary>
        internal static void SaveApplicationSettings()
        {
            AppSettings.Save();
        }

        /// <summary>
        /// Initializes the settings from the settings file on disk.
        /// </summary>
        /// <typeparam name="T">The data type of a simple Data Transfer Object to use during deserialization of the data contract.</typeparam>
        /// <remarks>The provided DTO type <typeparamref name="T"/> is inspected for correctness. The following are reported to the
        /// application log, and, if running a debug build, via a dialog:
        /// <list type="bullet">
        /// <item><description>This type and <typeparamref name="T"/> must have matching Data Contracts (name, namespace)</description></item>
        /// <item><description>All public properties that can be get and set, and that are not application properties, must also be marked as Data Members</description></item>
        /// <item><description>All properties in the data contract of this type must match those on <typeparamref name="T"/></description></item>
        /// <item><description>Data types of all properties in the data contracts must match</description></item>
        /// </list></remarks>
        protected void InitializeFromSettingsFile<T>() where T : new()
        {
            var dtoProperties = ValidateDataTransferObjectType<T>();
            if (File.Exists(ComponentSettingsFilePath))
            {
                try
                {
                    using (var xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(FileUtilities.OpenFileStream(ComponentSettingsFilePath), new XmlDictionaryReaderQuotas()))
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
                    var errorMessage = $"Error initializing from preferences file:\n  {ComponentSettingsFilePath}\n\n{e}";
                    ApplicationLogger.RecordDebugTraceMessage(errorMessage);
                }
            }
            InitializeFromApplicationSettings();
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
        protected virtual void AddCustomTypeConverters()
        {
            var stockConverterTypes = new[] { typeof(int), typeof(bool) };
            foreach (var type in stockConverterTypes)
            {
                var converter = TypeDescriptor.GetConverter(type);
                AddCustomTypeConverter(type, converter);
            }
        }

        /// <summary>
        /// Add a custom type converter.
        /// </summary>
        /// <param name="type">The data type of the setting.</param>
        /// <param name="converter">The <see cref="TypeConverter"/> to use to converter value for the type to and from a string.</param>
        /// <remarks>Only one converter for a given type may be registered.</remarks>
        protected void AddCustomTypeConverter(Type type, TypeConverter converter)
        {
            _converters.Add(type, converter);
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
                value = GetSetting(key);
                gotSetting = true;
            }
            catch (Exception)
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
            StoreSetting(key, value);
        }

        /// <summary>
        /// Get the value for a setting.
        /// </summary>
        /// <param name="key">The key for the setting.</param>
        /// <returns>The value of the setting.</returns>
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
        protected void StoreSetting(string key, object value)
        {
            var currentValue = GetSetting(key);
            if (_values.TryUpdate(key, value, currentValue))
            {
                this.RaisePropertyChanged(PropertyChanged, key);
            }
        }

        private static void ReportSettingsDataContractErrors(string settingsTypeName, IEnumerable<string> errors)
        {
            if (errors.Any())
            {
                var errorMessageBuilder = new System.Text.StringBuilder(string.Format(CultureInfo.CurrentCulture, "The following errors were reported when initializing settings for: {0}", settingsTypeName)).AppendLine();
                foreach (var error in errors)
                {
                    errorMessageBuilder.AppendLine($"  {error}");
                }
                var errorMessage = errorMessageBuilder.ToString();
                ApplicationLogger.RecordInternalWarning(errorMessage);
                ReportSettingsDataContractErrorsAfterLaunch(settingsTypeName, errors);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private static void ReportSettingsDataContractErrorsAfterLaunch(string settingsTypeName, IEnumerable<string> errors)
        {
            var reportSettingsDataContractErrors = new Action(() =>
            {
                var errorMessageHeader = string.Format(CultureInfo.CurrentCulture, "The following errors were reported when initializing settings for: {0}", settingsTypeName);
                var errorMessageBuilder = new System.Text.StringBuilder();
                foreach (var error in errors)
                {
                    errorMessageBuilder.AppendLine(error);
                }
                var dialog = View.ReportDialog.Create("Settings Data Contract Error", errorMessageHeader);
                dialog.ReportText = errorMessageBuilder.ToString();
                dialog.TextWrapping = View.OSTextWrapping.NoWrap;
                (dialog.DataContext as ViewModel.ReportDialogViewModel).CloseDialogButtonText = Resources.Strings.Close;
                dialog.ShowDialog();
            });
            SingleInstanceApplication.Current.AddStartupAction("zzzzReportSettingsDataContractErrors:" + settingsTypeName, reportSettingsDataContractErrors, StartupTaskPriority.LowestAsyncTaskPriority);
        }

        private static bool HasDataMemberProperty(PropertyInfo propertyInfo)
        {
            var dataMemberAttribute = propertyInfo.GetCustomAttribute(typeof(DataMemberAttribute));
            var hasDataMemberProperty = dataMemberAttribute != null;
            return hasDataMemberProperty;
        }

        private IEnumerable<PropertyInfo> ValidateDataTransferObjectType<T>()
        {
            var errorMessages = new List<string>();
            ValidateDataContract<T>(errorMessages);
            var validProperties = ValidateDataTransferObjectProperties<T>(errorMessages);
            ReportSettingsDataContractErrors(this.GetType().FullName, errorMessages);
            return validProperties;
        }

        private void ValidateDataContract<T>(IList<string> errorMessages)
        {
            var myContract = this.GetType().GetCustomAttribute(typeof(DataContractAttribute)) as DataContractAttribute;
            var myContractName = myContract?.Name ?? "<not defined>";
            var myContractNamespace = myContract?.Namespace ?? "<not defined>";

            var dtoContract = typeof(T).GetCustomAttribute(typeof(DataContractAttribute)) as DataContractAttribute;
            var dtoContractName = dtoContract?.Name ?? "<not defined>";
            var dtoContractNamespace = dtoContract?.Namespace ?? "<not defined>";

            var dataContractErrorMessage = string.Empty;
            if ((myContract != null) && (dtoContract == null))
            {
                dataContractErrorMessage = string.Format(CultureInfo.CurrentCulture, "Data contract missing: Expected: [{0}, {1}]", myContractName, myContractNamespace);
            }
            else if ((myContract == null) && (dtoContract != null))
            {
                dataContractErrorMessage = string.Format(CultureInfo.CurrentCulture, "Data contract not defined: [{0}, {1}]", dtoContractName, dtoContractNamespace);
            }
            else if ((myContractName != dtoContractName) || (myContractNamespace != dtoContractNamespace))
            {
                dataContractErrorMessage = string.Format(CultureInfo.CurrentCulture, "Data contract mismatch: Expected: [{0}, {1}] Actual: [{2}, {3}]", myContractName, myContractNamespace, dtoContractName, dtoContractNamespace);
            }

            if (!string.IsNullOrEmpty(dataContractErrorMessage))
            {
                errorMessages.Add(dataContractErrorMessage);
            }
        }

        private IEnumerable<PropertyInfo> ValidateDataTransferObjectProperties<T>(IList<string> errorMessages)
        {
            var dataContractProperties = this.GetType().GetPublicInstancePropertiesWithGetAndSet(HasDataMemberProperty);
            var dtoProperties = typeof(T).GetPublicInstancePropertiesWithGetAndSet(HasDataMemberProperty);

            var dataContractPropertyNames = dataContractProperties.Select(p => p.Name);
            foreach (var propertyName in _defaults.Keys.Except(_applicationSettingsKeys))
            {
                if (!dataContractPropertyNames.Contains(propertyName))
                {
                    var propertyErrorMessage = string.Format(CultureInfo.CurrentCulture, "Data contract is missing property: {0}", propertyName);
                    errorMessages.Add(propertyErrorMessage);
                }
            }

            var validProperties = new List<PropertyInfo>();
            foreach (var dataContractProperty in dataContractProperties)
            {
                var propertyErrorMessage = string.Empty;
                var dtoProperty = dtoProperties.FirstOrDefault(p => p.Name == dataContractProperty.Name);
                if (dtoProperty == null)
                {
                    propertyErrorMessage = string.Format(CultureInfo.CurrentCulture, "Data Transfer Object contract missing property: {0}", dataContractProperty.Name);
                }
                else if (dtoProperty.PropertyType != dataContractProperty.PropertyType)
                {
                    propertyErrorMessage = string.Format(CultureInfo.CurrentCulture, "Data contract property type mismatch for property: {0}: Expected: {1} Actual {2}", dataContractProperty.Name, dataContractProperty.PropertyType, dtoProperty.PropertyType);
                }
                else
                {
                    validProperties.Add(dtoProperty);
                }

                if (!string.IsNullOrEmpty(propertyErrorMessage))
                {
                    errorMessages.Add(propertyErrorMessage);
                }
            }

            return validProperties;
        }

        /// <summary>
        /// GTK-specific initialization.
        /// </summary>
        private void OSInitialize()
        {
            AddCustomTypeConverters();
            var componentName = this.GetType().Assembly.GetName().Name.ToLowerInvariant().Replace('.', '-');
            ComponentSettingsFilePath = Path.Combine(ApplicationSettings.Directory, componentName + ".config");
        }

        private void InitializeFromApplicationSettings()
        {
            try
            {
                AppSettings.InitializeSettings(this);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error initializing from application settings:\n\n{e}";
                ApplicationLogger.RecordDebugTraceMessage(errorMessage);
            }
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
                AppSettings.RegisterSetting(this, key, defaultValue.GetType());
            }
        }

        /// <summary>
        /// GTK-specific implementation to saves the settings to disk using <see cref="DataContractSerializer"/>.
        /// </summary>
        partial void OSSave()
        {
            // If no settings are specific to the type, and all are shunted to application settings,
            // don't bother creating the settings file!
            if (_values.Keys.Except(_applicationSettingsKeys).Any())
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
                    var errorMessage = $"Error saving preferences file:\n  {ComponentSettingsFilePath}\n\n{e}";
                    ApplicationLogger.RecordDebugTraceMessage(errorMessage);
                }
            }
        }
    }
}
