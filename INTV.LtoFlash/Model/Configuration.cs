// <copyright file="Configuration.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class specifies various configuration data - temporary as well as permanent - for working with Locutus devices.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IApplicationInfo))]
    [System.ComponentModel.Composition.Export(typeof(IConfiguration))]
    [System.ComponentModel.Composition.ExportMetadata("FeatureName", FeatureName)]
    public partial class Configuration : IApplicationInfo, IConfiguration
    {
        /// <summary>File extension to use for a generic LFS fork when stored as a file on disk.</summary>
        public const string ForkExtension = ".fork";

        private const string FeatureName = "LtoFlash";
        private const string RomsStagingArea = "ROMsCache";
        private const string RecoveredDataArea = "RecoveredData";
        private const string ReservedDataArea = "ReservedData";
        private const string VignetteDataArea = "Vignettes";
        private const string BackupDataArea = "BackupData";
        private const string MenuLayoutFileBaseName = "MenuLayout";
        private const string DeviceSubdirectoryBaseName = "LTOFlash";
        private const string PendingChangesFileBaseName = "PendingChanges";
        private const string XmlExtension = ".xml";
        private const string ForkBaseName = "Data";
        private const string IndexBaseName = "index";
        private const string ErrorLogDir = "ErrorLog";
        private const string RomsDir = "ROMs";
        private const string StarterRomsDir = "StarterROMs";
        private const string FirmwareUpdatesDir = "FirmwareUpdates";

        private static Configuration _instance;
        private string _helpPostData;
        private string _versionCheckPostData;

        #region Constructors

        private Configuration()
        {
            // TODO Make partial class instead of preprocessor check.
            // This null check keeps the WPF XAML designer output clean.
            if (INTV.Shared.Utility.SingleInstanceApplication.Instance != null)
            {
                ApplicationConfigurationPath = System.IO.Path.Combine(PathUtils.GetDocumentsDirectory(), DocumentFolderName); // applicationConfiguration.DocumentsPath;
                MenuLayoutPath = System.IO.Path.Combine(ApplicationConfigurationPath, MenuLayoutFileBaseName + XmlExtension);
                ErrorLogDirectory = System.IO.Path.Combine(ApplicationConfigurationPath, ErrorLogDir);
                RomsStagingAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, RomsStagingArea);
                RomsStagingAreaIndexPath = System.IO.Path.Combine(RomsStagingAreaPath, IndexBaseName + XmlExtension);
                RecoveredDataAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, RecoveredDataArea);
                ReservedDataAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, ReservedDataArea);
                VignetteDataAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, VignetteDataArea);
                HostBackupDataAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, BackupDataArea);
                MenuLayoutBackupDataAreaPath = System.IO.Path.Combine(HostBackupDataAreaPath, MenuLayoutFileBaseName);
                StarterRomsDirectory = System.IO.Path.Combine(ApplicationConfigurationPath, RomsDir, StarterRomsDir);
                FirmwareUpdatesDirectory = System.IO.Path.Combine(ApplicationConfigurationPath, FirmwareUpdatesDir);
                RedistributablesPath = System.IO.Path.Combine(SingleInstanceApplication.Instance.ProgramDirectory, RedistributablesDirectoryName);

                _helpPostData = "os=";
                _versionCheckPostData = "os=";
#if MAC
                _helpPostData += "mac";
                _versionCheckPostData += "mac";
                MinimumOSVersion = new OSVersion(10,7,0);
                RecommendedOSVersion = new OSVersion(10, 9, 0);
#elif WIN
                _helpPostData += "win";
                _versionCheckPostData += "win";
                MinimumOSVersion = new OSVersion(5, 1, 0); // Windows xp
                RecommendedOSVersion = new OSVersion(6, 1, 0); // Windows 7 or later
#elif GTK
#endif // MAC
                var versionParts = SingleInstanceApplication.Version.Split('.');
                _helpPostData += "&ver=" + versionParts[0] + versionParts[1];
                _instance = this;
            }
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the instance of the configuration data.
        /// </summary>
        internal static Configuration Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the name of the redistributables directory.
        /// </summary>
        public static string RedistributablesDirectoryName
        {
            get { return "redist"; }
        }

        /// <summary>
        /// Gets the default name for a menu layout.
        /// </summary>
        public string DefaultMenuLayoutFileName
        {
            get { return MenuLayoutFileBaseName + XmlExtension; }
        }

        /// <summary>
        /// Gets the absolute path to the file storing the menu layout.
        /// </summary>
        public string MenuLayoutPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the error log directory.
        /// </summary>
        public string ErrorLogDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which to place ROMs prepared for deployment to Locutus.
        /// </summary>
        public string RomsStagingAreaPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the ROMs cache index path.
        /// </summary>
        public string RomsStagingAreaIndexPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which recovered data is stored.
        /// </summary>
        public string RecoveredDataAreaPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which recovered reserved forks are stored.
        /// </summary>
        public string ReservedDataAreaPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which recovered vignettes are stored.
        /// </summary>
        public string VignetteDataAreaPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which backup copies of menu and ROM lists are stored
        /// when doing a SyncFromDevice command.
        /// </summary>
        public string HostBackupDataAreaPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory in which backup copies of menu and ROM list are stored.
        /// </summary>
        public string MenuLayoutBackupDataAreaPath { get; private set; }

        /// <summary>
        /// Gets the absolute application configuration path.
        /// </summary>
        public string ApplicationConfigurationPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the directory containing redistributable files.
        /// </summary>
        public string RedistributablesPath { get; private set; }

        /// <summary>
        /// Gets the directory used to contain ROMs that were shipped with the product.
        /// </summary>
        public string StarterRomsDirectory { get; private set; }

        /// <summary>
        /// Gets the directory used to contain firmware updates. LUI may be built to include a firmware update out-of-the-box if desired.
        /// </summary>
        public string FirmwareUpdatesDirectory { get; private set; }

        /// <summary>
        /// Gets a useful filename fragment for tagging files created during backup.
        /// </summary>
        public string SyncFromDeviceBackupFilenameFragment
        {
            get { return "SyncFromDevice"; }
        }

        /// <summary>
        /// Gets the unique IDs of devices known to have been connected to the system.
        /// </summary>
        public IEnumerable<string> KnownDeviceIds
        {
            get
            {
                if (System.IO.Directory.Exists(Configuration.Instance.ApplicationConfigurationPath))
                {
                    var genericDeviceDirectoryName = System.IO.Path.GetFileName(GetDeviceDataAreaPath(INTV.Core.Model.LuigiScrambleKeyBlock.AnyLTOFlashId));
                    var validLtoFlashDeviceDirectoryNameLength = genericDeviceDirectoryName.Length;
                    var deviceIdLength = INTV.Core.Model.LuigiScrambleKeyBlock.AnyLTOFlashId.Length;
                    var deviceDirectoryWildcard = genericDeviceDirectoryName.Remove(validLtoFlashDeviceDirectoryNameLength - deviceIdLength) + "*";

                    // NOTE: Mono(Mac) assume case-sensitive file system here! So the case of the wildcard must exactly match!
                    foreach (var ltoFlashDeviceDirectory in System.IO.Directory.EnumerateDirectories(ApplicationConfigurationPath, deviceDirectoryWildcard).Where(d => System.IO.Path.GetFileName(d).Length == validLtoFlashDeviceDirectoryNameLength))
                    {
                        // Check that the string looks like a potentially valid DRUID. It should parse as a pair of 64-bit integers in hex.
                        var deviceId = ltoFlashDeviceDirectory.Substring(ltoFlashDeviceDirectory.Length - deviceIdLength);
                        var druidHi = 0ul;
                        var druidLo = 0ul;
                        if (ulong.TryParse(deviceId.Substring(0, 16), System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo, out druidHi) &&
                            ulong.TryParse(deviceId.Substring(16, 16), System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo, out druidLo))
                        {
                            yield return deviceId;
                        }
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        #region IConfiguration

        /// <inheritdoc />
        public object Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <inheritdoc />
        public IEnumerable<IPeripheral> ConnectedPeripheralsHistory
        {
            get
            {
                return KnownDeviceIds.Select(id => new Device(id));
            }
        }

        #endregion // IConfiguration

        #region IApplicationInfo

        /// <inheritdoc />
        public string DocumentFolderName
        {
            get { return "LTO Flash"; }
        }

        /// <inheritdoc />
        public OSVersion MinimumOSVersion { get; private set; }

        /// <inheritdoc />
        public OSVersion RecommendedOSVersion { get; private set; }

        /// <inheritdoc />
        public string ProductUrl
        {
            get { return "http://www.intvfunhouse.com/intvfunhouse/ltoflash/"; }
        }

        /// <inheritdoc />
        public string OnlineHelpUrl
        {
            get { return ProductUrl + "help/?" + _helpPostData; }
        }

        /// <inheritdoc />
        public string VersionCheckUrl
        {
            get { return ProductUrl + "current_version.php?" + _versionCheckPostData; }
        }

        #endregion // IApplicationInfo

        #endregion // Properties;

        /// <summary>
        /// Gets a Locutus device-specific file path for storing a menu layout.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the device.</param>
        /// <returns>An absolute path for storing a specific device's menu layout.</returns>
        public string GetMenuLayoutPath(string uniqueId)
        {
            var menuLayoutPath = System.IO.Path.Combine(GetDeviceDataAreaPath(uniqueId), MenuLayoutFileBaseName + XmlExtension);
            return menuLayoutPath;
        }

        /// <summary>
        /// Gets the menu layout backup directory.
        /// </summary>
        /// <returns>The menu layout backup directory.</returns>
        public string GetMenuLayoutBackupDirectory()
        {
            var menuLayoutBackupDirectory = System.IO.Path.Combine(MenuLayoutBackupDataAreaPath, INTV.Shared.Utility.PathUtils.GetTimeString());
            return menuLayoutBackupDirectory;
        }

        /// <summary>
        /// Gets a Locutus device-specific path for storing data.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the device.</param>
        /// <returns>An absolute path for storing a specific device-specific content.</returns>
        public string GetDeviceDataAreaPath(string uniqueId)
        {
            var recoveredDataAreaPath = ApplicationConfigurationPath;
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                recoveredDataAreaPath = System.IO.Path.Combine(ApplicationConfigurationPath, DeviceSubdirectoryBaseName + "_" + uniqueId);
            }
            return recoveredDataAreaPath;
        }

        /// <summary>
        /// Gets a Locutus device-specific path for storing backup data.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the device.</param>
        /// <returns>An absolute path for storing a specific device's file system backup.</returns>
        public string GetDeviceBackupDataAreaPath(string uniqueId)
        {
            var backupDataAreaPath = System.IO.Path.Combine(GetDeviceDataAreaPath(uniqueId), BackupDataArea);
            return backupDataAreaPath;
        }

        /// <summary>
        /// Gets a Locutus device-specific path for storing a device backup.
        /// </summary>
        /// <param name="uniqueId">The unique identifier of the device.</param>
        /// <returns>An absolute path for storing a specific device's file system backup.</returns>
        public string GetUniqueBackupDataDataFilePath(string uniqueId)
        {
            var backupDirectory = System.IO.Path.Combine(GetDeviceBackupDataAreaPath(uniqueId), INTV.Shared.Utility.PathUtils.GetTimeString());
            return backupDirectory;
        }

        /// <summary>
        /// Gets a path for a fork data file.
        /// </summary>
        /// <param name="forkId">The fork ID of the fork needing a path.</param>
        /// <returns>The path for the fork data file.</returns>
        public string GetForkDataFileName(ushort forkId)
        {
            var recoveredFileName = ForkBaseName + "_" + forkId.ToString("x4") + ForkExtension;
            return recoveredFileName;
        }

        /// <summary>
        /// Gets a path to use for port logging.
        /// </summary>
        /// <param name="portName">The name of the port for which the logging path is created.</param>
        /// <returns>The logging path.</returns>
        public string GetPortLogPath(string portName)
        {
            var portLogFileName = portName.Replace("/dev/", string.Empty).Replace('/', '.') + "_log.txt";
            var serialPortLogPath = System.IO.Path.Combine(ApplicationConfigurationPath, portLogFileName);
            return serialPortLogPath;
        }
    }
}
