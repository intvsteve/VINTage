// <copyright file="Configuration.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using INTV.Core.Model.Device;
using INTV.Shared.ComponentModel;

namespace INTV.Intellicart.Model
{
    /// <summary>
    /// Configuration interface for the Intellicart.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IConfiguration))]
    [System.ComponentModel.Composition.ExportMetadata("FeatureName", "Intellicart")]
    public class Configuration : IConfiguration, System.ComponentModel.Composition.IPartImportsSatisfiedNotification
    {
        private const string FeatureName = "Intellicart";

        private const string RomsStagingArea = "ROMsCache";

        private Configuration()
        {
            _instance = this;
        }

        #region Properties

        /// <summary>
        /// Gets the instance of the configuration data.
        /// </summary>
        internal static Configuration Instance
        {
            get { return _instance; }
        }
        private static Configuration _instance;

        /// <summary>
        /// Gets the absolute application configuration path.
        /// </summary>
        public string ApplicationConfigurationPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which to place ROMs prepared for deployment to Locutus.
        /// </summary>
        public string RomsStagingAreaPath { get; private set; }

        #region IConfiguration

        /// <inheritdoc />
        public object Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <inheritdoc />
        public IEnumerable<IPeripheral> ConnectedPeripheralsHistory
        {
            get { return Enumerable.Empty<IPeripheral>(); } // we do not record these
        }

        #endregion // IConfiguration

        #region Imports

        [System.ComponentModel.Composition.Import]
        private IApplicationInfo AppInfo { get; set; }

        #endregion // Imports

        #endregion // Properties;

        #region IPartImportsSatisfiedNotification

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            var appInfo = CompositionHelpers.Container.GetExport<IApplicationInfo>();
            ApplicationConfigurationPath = System.IO.Path.Combine(INTV.Shared.Utility.PathUtils.GetDocumentsDirectory(), appInfo.Value.DocumentFolderName); // applicationConfiguration.DocumentsPath;
            RomsStagingAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, RomsStagingArea);
        }

        #endregion // IPartImportsSatisfiedNotification
    }
}
