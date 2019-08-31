// <copyright file="XdgEnvironmentVariable.cs" company="INTV Funhouse">
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

using System.Collections.Generic;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Enumeration to simplify using XDG environment variables.
    /// </summary>
    /// <remarks>Some of these values are are adapted from <see href="https://wiki.archlinux.org/index.php/XDG_user_directories"/>.</remarks>
    public enum XdgEnvironmentVariable
    {
        /// <summary>
        /// Not a valid XDG environment variable.
        /// </summary>
        None,

        /// <summary>
        /// The base data directory.
        /// </summary>
        Data,

        /// <summary>
        /// The base directy to use for configuration files.
        /// </summary>
        Config,

        /// <summary>
        /// A list of directories to be searched for data files in addition to <see cref="Data"/>.
        /// </summary>
        /// <remarks>Directories are separated by the colon character.</remarks>
        DataDirectories,

        /// <summary>
        /// A preference-ordered set of directories to search for configuration files in addition to <see cref="Config"/>.
        /// </summary>
        /// <remarks>Directories are separated by the colon character.</remarks>
        ConfigDirectories,

        /// <summary>
        /// The directory to use for cached content.
        /// </summary>
        Cache,

        /// <summary>
        /// The directory to use for user-specific, non-essential runtime files, e.g. named pipes.
        /// </summary>
        /// <remarks>Several rules apply. See https://specifications.freedesktop.org/basedir-spec/latest/ar01s03.html </remarks>
        Runtime,

        /// <summary>
        /// The (user's) desktop directory.
        /// </summary>
        Desktop,

        /// <summary>
        /// The user's documents directory.
        /// </summary>
        Documents,

        /// <summary>
        /// The user's downloads directory.
        /// </summary>
        Downloads,

        /// <summary>
        /// The user's directory for storing music files.
        /// </summary>
        Music,

        /// <summary>
        /// The user's directory for storing pictures.
        /// </summary>
        Pictures,

        /// <summary>
        /// A public directory, accessible to other users.
        /// </summary>
        Public,

        /// <summary>
        /// The templates directory.
        /// </summary>
        Templates,

        /// <summary>
        /// The directory used to store videos.
        /// </summary>
        Videos,
    }

    /// <summary>
    /// XDG environment variable helpers.
    /// </summary>
    public static class XdgEnvironmentVariableHelpers
    {
        private static readonly IReadOnlyDictionary<XdgEnvironmentVariable, string> XdgEnvironmentVariableNames = new Dictionary<XdgEnvironmentVariable, string>()
        {
            { XdgEnvironmentVariable.Data, "XDG_DATA_HOME" },
            { XdgEnvironmentVariable.Config, "XDG_CONFIG_HOME" },
            { XdgEnvironmentVariable.DataDirectories, "XDG_DATA_DIRS" },
            { XdgEnvironmentVariable.ConfigDirectories, "XDG_CONFIG_DIRS" },
            { XdgEnvironmentVariable.Cache, "XDG_CACHE_HOME" },
            { XdgEnvironmentVariable.Runtime, "XDG_RUNTIME_DIR" },
            { XdgEnvironmentVariable.Desktop, "XDG_DESKTOP_DIR" },
            { XdgEnvironmentVariable.Documents, "XDG_DOCUMENTS_DIR" },
            { XdgEnvironmentVariable.Downloads, "XDG_DOWNLOAD_DIR" },
            { XdgEnvironmentVariable.Music, "XDG_MUSIC_DIR" },
            { XdgEnvironmentVariable.Pictures, "XDG_PICTURES_DIR" },
            { XdgEnvironmentVariable.Public, "XDG_PUBLICSHARE_DIR" },
            { XdgEnvironmentVariable.Templates, "XDG_TEMPLATES_DIR" },
            { XdgEnvironmentVariable.Videos, "XDG_VIDEOS_DIR" },
        };

        /// <summary>
        /// Gets the environment variable value for the given XDG environment variable.
        /// </summary>
        /// <param name="xdgEnvironmentVariable">The XDG environment variable whose value is desired.</param>
        /// <returns>The environment variable value.</returns>
        public static string GetEnvironmentVariableValue(this XdgEnvironmentVariable xdgEnvironmentVariable)
        {
            var xdgEnvironmentVariableValue = string.Empty;
            string xdgEnvironmentVariableName;
            if (XdgEnvironmentVariableNames.TryGetValue(xdgEnvironmentVariable, out xdgEnvironmentVariableName))
            {
                xdgEnvironmentVariableValue = System.Environment.GetEnvironmentVariable(xdgEnvironmentVariableName);
                if (string.IsNullOrEmpty(xdgEnvironmentVariableValue))
                {
                    xdgEnvironmentVariableValue = xdgEnvironmentVariable.GetDefaultXdgEnvironmentVariableValue();
                }
            }
            return xdgEnvironmentVariableValue;
        }

        private static string GetDefaultXdgEnvironmentVariableValue(this XdgEnvironmentVariable xdgEnvironmentVariable)
        {
            var value = string.Empty;
            var homeDir = PathUtils.UserHomeDirectory;
            switch (xdgEnvironmentVariable)
            {
                case XdgEnvironmentVariable.Data:
                    value = System.IO.Path.Combine(homeDir, ".local", "share");
                    break;
                case XdgEnvironmentVariable.Config:
                    value = System.IO.Path.Combine(homeDir, ".config");
                    break;
                case XdgEnvironmentVariable.DataDirectories:
                    value = "/usr/local/share/:/usr/share/";
                    break;
                case XdgEnvironmentVariable.ConfigDirectories:
                    value = "/etc/xdg";
                    break;
                case XdgEnvironmentVariable.Cache:
                    value = System.IO.Path.Combine(homeDir, ".cache");
                    break;
                case XdgEnvironmentVariable.Runtime:
                    ////value = System.IO.Path.Combine (homeDir, );
                    break;
                case XdgEnvironmentVariable.Desktop:
                    value = System.IO.Path.Combine(homeDir, "Desktop");
                    break;
                case XdgEnvironmentVariable.Documents:
                    value = System.IO.Path.Combine(homeDir, "Documents");
                    break;
                case XdgEnvironmentVariable.Downloads:
                    value = System.IO.Path.Combine(homeDir, "Downloads");
                    break;
                case XdgEnvironmentVariable.Music:
                    value = System.IO.Path.Combine(homeDir, "Music");
                    break;
                case XdgEnvironmentVariable.Pictures:
                    value = System.IO.Path.Combine(homeDir, "Pictures");
                    break;
                case XdgEnvironmentVariable.Public:
                    value = System.IO.Path.Combine(homeDir, "Public");
                    break;
                case XdgEnvironmentVariable.Templates:
                    value = System.IO.Path.Combine(homeDir, "Templates");
                    break;
                case XdgEnvironmentVariable.Videos:
                    value = System.IO.Path.Combine(homeDir, "Videos");
                    break;
                default:
                    break;
            }
            return value;
        }
    }
}
