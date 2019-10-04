// <copyright file="CompressedArchiveAccessConfiguration.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.Model
{
    [System.ComponentModel.Composition.Export(typeof(IConfiguration))]
    [System.ComponentModel.Composition.ExportMetadata("FeatureName", FeatureName)]
    [System.ComponentModel.Composition.ExportMetadata("Weight", Weight)]
    internal sealed class CompressedArchiveAccessConfiguration : IConfiguration
    {
        private const string FeatureName = "CompressedArchiveAccessConfiguration";
        private const double Weight = 0.85;

        private CompressedArchiveAccessConfiguration()
        {
            Instance = this;
        }

        /// <summary>
        /// Gets the single instance of the configuration object.
        /// </summary>
        public static CompressedArchiveAccessConfiguration Instance { get; private set; }

        /// <summary>
        /// Gets the enabled compressed archive formats.
        /// </summary>
        public EnabledCompressedArchiveFormats EnabledFormats
        {
            get { return Properties.Settings.Default.EnabledArchiveFormats; }
        }

        /// <summary>
        /// Gets a value indicating whether nested archive support is enabled.
        /// </summary>
        public bool EnableNestedArchives
        {
            get { return Properties.Settings.Default.SearchNestedArchives; }
        }

        /// <summary>
        /// Gets the largest allowed compressed archive that is supported. This value can be used to
        /// help with performance and memory usage.
        /// </summary>
        public int MaxArchiveSizeInMegabytes
        {
            get { return Properties.Settings.Default.MaxArchiveSizeMB; }
        }

        /// <summary>
        /// Gets the maximum number of concatenated GZIP entries to attempt to enumerate.
        /// </summary>
        /// <remarks>The implementation does not have a rigorous implementation of the GZIP format, so its attempts
        /// to locate valid GZIP entries that have been concatenated into a single file can produce false entries.
        /// Although keeping this value small _may_ reduce the likelihood of errors, it is not proper to assume that
        /// it will. Concatenating multiple GZIP files together is uncommon, but allowed by the specification.</remarks>
        public int MaxGZipEntriesToIdentify
        {
            get { return Properties.Settings.Default.MaxGZipEntriesSearch; }
        }

        /// <inheritdoc />
        public object Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <inheritdoc />
        public IEnumerable<Core.Model.Device.IPeripheral> ConnectedPeripheralsHistory
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public void Initialize()
        {
            if (EnabledFormats.ToCompressedArchiveFormats().Any())
            {
                ICompressedArchiveAccessExtensions.EnableCompressedArchiveAccess();
            }
            else
            {
                ICompressedArchiveAccessExtensions.DisableCompressedArchiveAccess();
            }
            EnabledFormats.UpdateAvailableCompressedArchiveFormats();

            if (EnableNestedArchives)
            {
                ICompressedArchiveAccessExtensions.EnableNestedArchiveAccess();
            }
            else
            {
                ICompressedArchiveAccessExtensions.DisableNestedArchiveAccess();
            }

            ICompressedArchiveAccessExtensions.SetMaxAllowedArchiveSizeInMegabytes(MaxArchiveSizeInMegabytes);
        }
    }
}
