// <copyright file="SettingsBase.Gtk.ApplicationSettings.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using INTV.Core.Utility;
using INTV.Shared.Utility;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public abstract partial class SettingsBase
    {
        /// <summary>
        /// GTK-specific private class for aggregating application settings and saving
        /// to a separate file. The file is INI-file-like, in that it supports sections
        /// and a simple 'name = value' layout.
        /// </summary>
        /// <remarks>When transferring a value from the text settings file to the concrete
        /// <see cref="SettingsBase"/> instance that has a strongly typed value for the setting,
        /// the implementation uses a <see cref="TypeConverter"/> to perform culture-invariant
        /// string-to-value conversion. Similarly, when saving the settings to a text file, a
        /// culture-invariant string is requested from a <see cref="TypeConverter"/>.</remarks>
        private class ApplicationSettings
        {
            private static readonly Lazy<string> AppSettingsDir = new Lazy<string>(InitializeAppSettingsDir);
            private static readonly Lazy<string> AppSettingsPath = new Lazy<string>(InitializeAppSettingsPath);
            private readonly List<Setting> _applicationSettings = new List<Setting>();
            private readonly Dictionary<string, List<SettingDto>> _applicationSettingsDataTransferObjects = new Dictionary<string, List<SettingDto>>();

            /// <summary>
            /// Gets the directory to use to store application settings.
            /// </summary>
            public static string Directory
            {
                get { return AppSettingsDir.Value; }
            }

            private static string FilePath
            {
                get { return AppSettingsPath.Value; }
            }

            /// <summary>
            /// Registers a setting as an application setting.
            /// </summary>
            /// <param name="owningSettings">The concrete settings implementation that provides the actual settings property.</param>
            /// <param name="name">The name of the setting.</param>
            /// <param name="type">The data type of the setting.</param>
            public void RegisterSetting(SettingsBase owningSettings, string name, Type type)
            {
                _applicationSettings.Add(new Setting(owningSettings, name, type));
            }

            /// <summary>
            /// Loads the settings from the application settings file.
            /// </summary>
            public void Load()
            {
                var settingsDtos = new Dictionary<string, List<SettingDto>>();
                if (File.Exists(FilePath))
                {
                    try
                    {
                        var currentSection = string.Empty;
                        var settingsLines = File.ReadAllLines(FilePath);
                        foreach (var settingsLine in settingsLines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                        {
                            if (!char.IsLetter(settingsLine[0]) && (settingsLine[0] != '['))
                            {
                                continue;
                            }
                            var sectionMatch = Regex.Match(settingsLine, @"\[(.*?)\]");
                            if (sectionMatch.Success)
                            {
                                currentSection = sectionMatch.Value;
                                if (!settingsDtos.ContainsKey(currentSection))
                                {
                                    settingsDtos[currentSection] = new List<SettingDto>();
                                }
                            }
                            else
                            {
                                var settingData = settingsLine.Split(new[] { '=' });
                                if (settingData.Length == 2)
                                {
                                    var name = settingData[0].Trim();
                                    var value = settingData[1].Trim();
                                    var quotes = value.GetEnclosingQuoteCharacterIndexes();
                                    if (quotes.IsValid && (quotes.Minimum >= 0) && (quotes.Maximum > quotes.Minimum))
                                    {
                                        var unquotedStringLength = quotes.Maximum - quotes.Minimum - 2;
                                        if (unquotedStringLength > 0)
                                        {
                                            value = value.Substring(quotes.Minimum + 1, unquotedStringLength);
                                        }
                                        else
                                        {
                                            value = string.Empty;
                                        }
                                    }
                                    if (settingsDtos.ContainsKey(currentSection))
                                    {
                                        var setting = new SettingDto() { Name = name, Value = value };
                                        settingsDtos[currentSection].Add(setting);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        var errorMessage = $"Error parsing application preferences file:\n  {FilePath}\n\n{e}";
                        ApplicationLogger.RecordDebugTraceMessage(errorMessage);
                    }
                }

                foreach (var settingsDto in settingsDtos)
                {
                    _applicationSettingsDataTransferObjects.Add(settingsDto.Key, settingsDto.Value);
                }
            }

            /// <summary>
            /// Initialize the values of the specific provided settings that exist as application settings.
            /// </summary>
            /// <param name="settings">Settings that may have entries stored in the shared application settings file.</param>
            /// <remarks>When an implementation of <see cref="SettingsBase"/> registers its different settings values, it may
            /// indicate that some of its values are application-level settings. Such values are not persisted as part of the
            /// typical DataContract, but rather separately, via this smaller, simpler application-level settings file. As such,
            /// after data in an implementation of <see cref="SettingsBase"/> has been hydrated from its DataContract, it must
            /// subsequently request values from this separate settings file via this method.</remarks>
            public void InitializeSettings(SettingsBase settings)
            {
                var sectionName = $"[{GetSectionName(settings.GetType())}]";
                List<SettingDto> settingDtos;
                if (_applicationSettingsDataTransferObjects.TryGetValue(sectionName, out settingDtos))
                {
                    foreach (var settingDto in settingDtos)
                    {
                        var setting = _applicationSettings.FirstOrDefault(s => (settings.GetType() == s.OwningSettings.GetType()) && (s.Name == settingDto.Name));
                        if (setting != null)
                        {
                            object value = settingDto.Value;
                            TypeConverter converter;
                            if (settings._converters.TryGetValue(setting.Type, out converter))
                            {
                                value = converter.ConvertFromInvariantString(settingDto.Value);
                            }
                            setting.Value = value;
                        }
                    }
                }
            }

            /// <summary>
            /// Saves the application settings to a text file.
            /// </summary>
            public void Save()
            {
                try
                {
                    using (var settingsFileWriter = new StreamWriter(FilePath, append: false, encoding: System.Text.Encoding.UTF8))
                    {
                        settingsFileWriter.WriteLine(Resources.Strings.SettingsDescriptionHeader);
                        foreach (var applicationSettingsSection in _applicationSettings.OrderBy(s => s.OwningSettings.Weight).GroupBy(s => s.OwningSettings))
                        {
                            var settingsType = applicationSettingsSection.Key.GetType();
                            var section = GetSectionName(settingsType);
                            var resourceName = applicationSettingsSection.Key.DescriptionsResourceName;
                            var description = GetDescription(settingsType, resourceName, "SettingsDescription");
                            if (!string.IsNullOrEmpty(description))
                            {
                                settingsFileWriter.WriteLine("# {0}", description);
                            }
                            settingsFileWriter.WriteLine("[{0}]", section);
                            foreach (var setting in applicationSettingsSection.OrderBy(s => s.Name))
                            {
#if false // just too noisy
                                description = GetDescription(settingsType, resourceName, setting.Name);
                                if (!string.IsNullOrEmpty(description))
                                {
                                    settingsFileWriter.WriteLine("# {0}: {1}", setting.Name, description);
                                }
#endif
                                var settingValue = setting.Value;
                                var settingIsString = settingValue is string;
                                var shouldWrite = settingValue != null;
                                if (settingIsString)
                                {
                                    shouldWrite = !string.IsNullOrWhiteSpace((string)settingValue);
                                }
                                else
                                {
                                    TypeConverter converter;
                                    if (setting.OwningSettings._converters.TryGetValue(settingValue.GetType(), out converter))
                                    {
                                        settingValue = converter.ConvertToInvariantString(setting.Value);
                                    }
                                }
                                if (shouldWrite)
                                {
                                    var format = settingIsString ? "{0} = \"{1}\"" : "{0} = {1}";
                                    settingsFileWriter.WriteLine(format, setting.Name, settingValue);
                                }
                            }
                            settingsFileWriter.WriteLine();
                        }
                        settingsFileWriter.Flush();
                    }
                }
                catch (Exception e)
                {
                    var errorMessage = $"Error saving application preferences file:\n  {FilePath}\n\n{e}";
                    ApplicationLogger.RecordDebugTraceMessage(errorMessage);
                }
            }

            private static string GetDescription(Type typeForResources, string resourceName, string resourceKey)
            {
                var description = string.Empty;
                if (string.IsNullOrEmpty(resourceName))
                {
                    description = typeForResources.GetResourceString(resourceKey);
                }
                else
                {
                    description = typeForResources.GetStringFromResource(resourceName, resourceKey);
                }
                if (description?.StartsWith("!!", StringComparison.InvariantCulture) == true)
                {
                    description = null;
                }
                return description;
            }

            private static string GetSectionName(Type settingsType)
            {
                var sectionName = settingsType.FullName.Replace(".", string.Empty);
                return sectionName;
            }

            private static string InitializeAppSettingsDir()
            {
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                var appName = Path.GetFileNameWithoutExtension(entryAssembly.GetName().Name);
                var applicationSettingsDirectory = Path.Combine(XdgEnvironmentVariable.Config.GetEnvironmentVariableValue(), appName);
                if (!System.IO.Directory.Exists(applicationSettingsDirectory))
                {
                    System.IO.Directory.CreateDirectory(applicationSettingsDirectory);
                }
                return applicationSettingsDirectory;
            }

            private static string InitializeAppSettingsPath()
            {
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
                var appName = Path.GetFileNameWithoutExtension(entryAssembly.GetName().Name);
                var applicationSettingsFilePath = Path.Combine(Directory, appName.ToLowerInvariant() + "rc");
                return applicationSettingsFilePath;
            }

            /// <summary>
            /// Stashes a setting to store in the application settings file.
            /// </summary>
            [System.Diagnostics.DebuggerDisplay("{OwningSettings}: {Name,nq} = {Value}")]
            private class Setting
            {
                /// <summary>
                /// Initializes a new instance of the
                /// <see cref="T:INTV.Shared.Properties.SettingsBase.ApplicationSettings.Setting"/> class.
                /// </summary>
                /// <param name="settings">The <see cref="SettingsBase"/> instance that provides the actual setting value.</param>
                /// <param name="name">The name of the setting.</param>
                /// <param name="type">The data type of the setting value.</param>
                public Setting(SettingsBase settings, string name, Type type)
                {
                    OwningSettings = settings;
                    Name = name;
                    Type = type;
                }

                /// <summary>
                /// Gets the owning settings instance.
                /// </summary>
                public SettingsBase OwningSettings { get; }

                /// <summary>
                /// Gets the name of the setting.
                /// </summary>
                public string Name { get; }

                /// <summary>
                /// Gets the data type of the setting value.
                /// </summary>
                public Type Type { get; }

                public object Value
                {
                    get { return OwningSettings[Name]; }
                    set { OwningSettings[Name] = value; }
                }
            }

            /// <summary>
            /// Data Transfer Object to use for a parsed setting from the application settings file.
            /// </summary>
            [System.Diagnostics.DebuggerDisplay("{Name,nq} = {Value,nq}")]
            private class SettingDto
            {
                /// <summary>
                /// Gets or sets the name of the setting.
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the value of the setting, as a string.
                /// </summary>
                public string Value { get; set; }
            }
        }
    }
}
