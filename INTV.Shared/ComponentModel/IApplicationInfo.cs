// <copyright file="IApplicationInfo.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

using INTV.Shared.Properties;
using INTV.Shared.Utility;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Provides application-level information to other components that may need it.
    /// </summary>
    public interface IApplicationInfo
    {
        /// <summary>
        /// Gets the invariant application name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the display name for the application, which may be localized.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the application version as a string.
        /// </summary>
        /// <value>The version.</value>
        string Version { get; }

        /// <summary>
        /// Gets the copyright string for the application.
        /// </summary>
        string Copyright { get; }

        /// <summary>
        /// Gets the author(s) of the application.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the application-specific name of the folder in which documents should be saved.
        /// </summary>
        string DocumentFolderName { get; }

        /// <summary>
        /// Gets the minimum supported operating system version required by the application.
        /// </summary>
        OSVersion MinimumOSVersion { get; }

        /// <summary>
        /// Gets the recommended operating system for the application.
        /// </summary>
        OSVersion RecommendedOSVersion { get; }

        /// <summary>
        /// Gets the application's URL, assumed to be a valid location to get information, download updates, et. al.
        /// </summary>
        string ProductUrl { get; }

        /// <summary>
        /// Gets the URL for the application's online help.
        /// </summary>
        string OnlineHelpUrl { get; }

        /// <summary>
        /// Gets the URL to use to see if there is a newer version of the application available for download.
        /// </summary>
        string VersionCheckUrl { get; }

        /// <summary>
        /// Gets the settings provided by the application itself.
        /// </summary>
        ISettings Settings { get; }
    }
}
