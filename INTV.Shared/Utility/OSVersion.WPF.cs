// <copyright file="OSVersion.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2021 All Rights Reserved
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
using System.Management;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class OSVersion
    {
        private static readonly Lazy<string> WmiOSName = new Lazy<string>(GetOSName);
        private static readonly Lazy<Version> WmiOSVersion = new Lazy<Version>(GetOSVersion);

        /// <summary>
        /// Gets the 'friendly' generic OS name.
        /// </summary>
        public static string Name
        {
            get
            {
                return WmiOSName.Value;
            }
        }

        /// <summary>
        /// Gets a WMI <see cref="ManagementObjectSearcher"/> for the given object.
        /// </summary>
        /// <param name="queryObjectName">The type of object to search for; the default is Win32_OperatingSystem.</param>
        /// <returns>An instance of <see cref="ManagementObjectSearcher"/>, or <c>null</c>.</returns>
        /// <remarks>The returned object is an <see cref="IDisposable"/> so use appropriately.</remarks>
        internal static ManagementObjectSearcher GetWmiObjectSearcher(string queryObjectName = "Win32_OperatingSystem")
        {
            var operatingSystemSearcher = new ManagementObjectSearcher("SELECT * FROM " + queryObjectName);
            return operatingSystemSearcher;
        }

        /// <summary>
        /// Gets the value of a property with the given name, using a default value if not available.
        /// </summary>
        /// <typeparam name="T">The data type of the property being read.</typeparam>
        /// <param name="searcher">A <see cref="ManagementObjectSearcher"/> that should provide an object with the given property.</param>
        /// <param name="propertyName">The name of the property whose value is desired.</param>
        /// <param name="defaultValue">A default value to use for the property if none is available or the call fails.</param>
        /// <returns>The value of the property, or <paramref name="defaultValue"/>.</returns>
        /// <remarks>If multiple objects in <paramref name="searcher"/> exist, only the value from the first will be returned.</remarks>
        internal static T GetObjectProperty<T>(ManagementObjectSearcher searcher, string propertyName, T defaultValue = default(T))
        {
            var propertyValue = defaultValue;
            if (searcher != null)
            {
                try
                {
                    var information = searcher.Get();
                    if (information != null)
                    {
                        var o = information.OfType<ManagementObject>().FirstOrDefault();
                        if (o != null)
                        {
                            var propertyObjectValue = o[propertyName];
                            if (propertyObjectValue != null)
                            {
                                propertyValue = (T)propertyObjectValue;
                            }
                        }
                    }
                }
                catch (ManagementException)
                {
                    // ignore errors
                }
            }
            return propertyValue;
        }

        /// <summary>
        /// Gets the value of a property with the given name for all objects in <paramref name="searcher"/>.
        /// </summary>
        /// <typeparam name="T">The data type of the property being read.</typeparam>
        /// <param name="searcher">A <see cref="ManagementObjectSearcher"/> that should provide at least one object with the given property.</param>
        /// <param name="propertyName">The name of the property whose value is desired.</param>
        /// <returns>An enumerable containing any non-<c>null</c> values for <paramref name="propertyName"/>.</returns>
        internal static IEnumerable<T> GetObjectPropertyValues<T>(ManagementObjectSearcher searcher, string propertyName)
        {
            var propertyValues = new List<T>();
            if (searcher != null)
            {
                try
                {
                    var information = searcher.Get();
                    if (information != null)
                    {
                        foreach (var o in information.OfType<ManagementObject>())
                        {
                            var propertyObjectValue = o[propertyName];
                            if (propertyObjectValue != null)
                            {
                                var propertyValue = (T)propertyObjectValue;
                                propertyValues.Add(propertyValue);
                            }
                        }
                    }
                }
                catch (ManagementException)
                {
                    // ignore errors
                }
            }
            return propertyValues;
        }

        private static OSVersion Initialize()
        {
            var version = new OSVersion(0, 0, 0);
            version._version = WmiOSVersion.Value;
            return version;
        }

        private static Version GetOSVersion()
        {
            using (var searcher = GetWmiObjectSearcher())
            {
                var version = Environment.OSVersion.Version;
                var versionString = GetObjectProperty(searcher, "Version", string.Empty);
                Version wmiVersion;
                if (Version.TryParse(versionString, out wmiVersion))
                {
                    version = wmiVersion;
                }
                return version;
            }
        }

        private static string GetOSName()
        {
            using (var searcher = GetWmiObjectSearcher())
            {
                var osName = GetObjectProperty(searcher, "Caption", "Windows");
                var servicePack = GetObjectProperty(searcher, "CSDVersion", string.Empty);
                if (!string.IsNullOrEmpty(servicePack))
                {
                    osName += " " + servicePack;
                }
                return osName;
            }
        }
    }
}
