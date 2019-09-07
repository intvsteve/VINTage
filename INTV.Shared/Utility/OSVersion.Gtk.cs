// <copyright file="OSVersion.Gtk.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class OSVersion
    {
        /// <summary>
        /// Gets the 'friendly' generic OS name.
        /// </summary>
        public static string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_osName))
                {
                    // Since it seems the availability and ubiquity of lsb_release, et. al. is ... unreliable
                    _osName = GetOSName("/etc/os-release");
                    if (string.IsNullOrEmpty(_osName))
                    {
                        _osName = GetOSName("/usr/lib/os-release");
                    }
                    if (string.IsNullOrEmpty(_osName))
                    {
                        _osName = "Linux";
                    }
                }
                return _osName;
            }
        }
        private static string _osName = null;

        private static OSVersion Initialize()
        {
            var version = new OSVersion(0, 0, 0);
            version._version = System.Environment.OSVersion.Version;
            return version;
        }

        private static string GetOSName(string fileName)
        {
            string osName = null;
            if (File.Exists(fileName))
            {
                var entries = new Dictionary<string, string>();
                foreach (var line in File.ReadAllLines(fileName))
                {
                    var parts = line.Split(new[] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        entries[parts[0].Trim()] = parts[1].Trim().Trim('"');
                    }
                }
                if (!entries.TryGetValue("PRETTY_NAME", out osName))
                {
                    if (entries.TryGetValue("NAME", out osName))
                    {
                        string version = null;
                        if (entries.TryGetValue("VERSION", out version))
                        {
                            osName = string.Format(CultureInfo.CurrentCulture, "{0} {1}", osName, version);
                        }
                    }
                }
            }

            return osName;
        }
    }
}
