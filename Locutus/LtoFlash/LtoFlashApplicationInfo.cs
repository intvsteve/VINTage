// <copyright file="LtoFlashApplicationInfo.cs" company="INTV Funhouse">
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
using INTV.Shared.ComponentModel;
using INTV.Shared.Properties;
using INTV.Shared.Utility;

namespace Locutus
{
    /// <summary>
    /// Implements <see cref="IApplicationInfo"/> for the program.
    /// </summary>
    internal sealed partial class LtoFlashApplicationInfo : ApplicationInfo
    {
        private const string BaseProductUrl = "https://www.intvfunhouse.com/intvfunhouse/ltoflash/";
        private readonly Lazy<string> _helpUrl = new Lazy<string>(InitializeHelpUrl);

        /// <inheritdoc />
        public override string Name
        {
            get { return "LTO Flash"; }
        }

        /// <inheritdoc />
        public override string DisplayName
        {
            get { return Resources.Strings.ApplicationDisplayName; }
        }

        /// <inheritdoc />
        public override string DocumentFolderName
        {
            get { return "LTO Flash"; }
        }

        /// <inheritdoc />
        public override OSVersion MinimumOSVersion
        {
            get { return OSVersionMinimum; }
        }

        /// <inheritdoc />
        public override OSVersion RecommendedOSVersion
        {
            get { return OSVersionRecommended; }
        }

        /// <inheritdoc />
        public override string ProductUrl
        {
            get { return BaseProductUrl; }
        }

        /// <inheritdoc />
        public override string OnlineHelpUrl
        {
            get { return _helpUrl.Value; }
        }

        /// <inheritdoc />
        public override string VersionCheckUrl
        {
            get { return ProductUrl + "current_version.php?os=" + OSString; }
        }

        /// <inheritdoc />
        public override ISettings Settings
        {
            get { return Locutus.Properties.Settings.Default; }
        }

        private static string InitializeHelpUrl()
        {
            var versionParts = StandardVersion.Split('.');
            var helpPostData = "os=" + OSString + "&ver=" + versionParts[0] + versionParts[1];
            return BaseProductUrl + "help/?" + helpPostData;
        }
    }
}
