// <copyright file="CompressedArchiveFormatViewModel.cs" company="INTV Funhouse">
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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Shared.CompressedArchiveAccess;
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Implements the ViewModel for a compressed archive setting to be used for custom configuration.
    /// </summary>
    public sealed class CompressedArchiveFormatViewModel : OSViewModelBase, INotifyPropertyChanged
    {
        private static readonly IDictionary<EnabledCompressedArchiveFormats, string> DisplayNames = new Dictionary<EnabledCompressedArchiveFormats, string>()
        {
            { EnabledCompressedArchiveFormats.Zip, Resources.Strings.CompressedArchiveFormat_Zip_DisplayName },
            { EnabledCompressedArchiveFormats.GZip, Resources.Strings.CompressedArchiveFormat_GZip_DisplayName },
            { EnabledCompressedArchiveFormats.Tar, Resources.Strings.CompressedArchiveFormat_Tar_DisplayName },
            { EnabledCompressedArchiveFormats.BZip2, Resources.Strings.CompressedArchiveFormat_BZip2_DisplayName },
        };

        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="settings">The overall compressed archive settings.</param>
        /// <param name="format">A specific compressed archive format flag the instance represents.</param>
        /// <param name="enabled">The initial state of whether the given format should be available to the application.</param>
        public CompressedArchiveFormatViewModel(CompressedArchiveAccessSettingsViewModel settings, EnabledCompressedArchiveFormats format, bool enabled)
        {
            Settings = settings;
            Format = format;
            DisplayName = DisplayNames[format];
            _enabled = enabled;

            CompressedArchiveFormat = format.ToCompressedArchiveFormats().Single();
            FileExtensions = "(" + string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ", CompressedArchiveFormat.FileExtensions().Select(x => "*" + x)) + ")";
            DisplayNameWithFileExtensions = DisplayName + " " + FileExtensions;
        }

        /// <summary>
        /// Gets the format represented by the instance.
        /// </summary>
        public EnabledCompressedArchiveFormats Format { get; private set; }

        /// <summary>
        /// Gets the user-friendly display name of the compressed archive format.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the user-friendly display value for the file extensions that are commonly used for the compressed archive format.
        /// </summary>
        public string FileExtensions { get; private set; }

        /// <summary>
        /// Gets the user-friendly display name with file extensions.
        /// </summary>
        [OSExport("DisplayNameWithFileExtensions")]
        public string DisplayNameWithFileExtensions { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the compressed archive format should be available for use in the application.
        /// </summary>
        [OSExport("Enabled")]
        public bool Enabled
        {
            get { return _enabled; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Enabled", value, ref _enabled, (p, v) => Settings.UpdateEnabledFormats()); }
        }
        private bool _enabled;

        private CompressedArchiveFormat CompressedArchiveFormat { get; set; }

        private CompressedArchiveAccessSettingsViewModel Settings { get; set; }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
