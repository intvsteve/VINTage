// <copyright file="RomListConfiguration.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.Model
{
    /// <summary>
    /// Model used for coordinating the application behavior and program list.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IConfiguration))]
    [System.ComponentModel.Composition.ExportMetadata("FeatureName", "ROMs")]
    [System.ComponentModel.Composition.ExportMetadata("Weight", 0.1)]
    public class RomListConfiguration : INTV.Core.ComponentModel.ModelBase, IConfiguration, System.ComponentModel.Composition.IPartImportsSatisfiedNotification
    {
        private const string BackupDataArea = "BackupData";
        private const string RomListBackupDir = "ROMList";
        private const string RomsDir = "ROMs";
        private const string ManualsDir = "Manuals";
        private const string BoxesDir = "Boxes";
        private const string OverlaysDir = "Overlays";
        private const string LabelsDir = "Labels";
        private const string RomsFile = "ROMs.xml";
        private const string LocalRomDefintions = "UserDefinedROMs.xml";
        private const string ErrorLogDir = "ErrorLog";

        private static RomListConfiguration _instance;

        private string _applicationDocumentsPath;

        #region Constructors

        private RomListConfiguration()
        {
            _instance = this;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the single instance of the configuration object.
        /// </summary>
        internal static RomListConfiguration Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Gets the default name for the ROM list.
        /// </summary>
        public string DefaultRomsFileName
        {
            get { return RomsFile; }
        }

        /// <summary>
        /// Gets a value for the absolute path to the file storing the list of known ROMs supplied by the user.
        /// </summary>
        public string RomFilesPath { get; private set; }

        /// <summary>
        /// Gets the absolute path to the backup data directory.
        /// </summary>
        public string BackupDataDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the error log directory.
        /// </summary>
        public string ErrorLogDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the ROMs directory used to store copies of ROMs.
        /// </summary>
        public string RomsDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the manuals directory.
        /// </summary>
        public string ManualsDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the boxes directory.
        /// </summary>
        public string BoxesDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the overlays directory.
        /// </summary>
        public string OverlaysDirectory { get; private set; }

        /// <summary>
        /// Gets the absolute path to the labels directory.
        /// </summary>
        public string LabelsDirectory { get; private set; }

        #region IConfiguration

        /// <inheritdoc />
        public object Settings
        {
            get { return Properties.Settings.Default; }
        }

        /// <inheritdoc />
        public IEnumerable<IPeripheral> ConnectedPeripheralsHistory
        {
            get { return Enumerable.Empty<IPeripheral>(); } // this does not make sense for ROM list
        }

        #endregion // IConfiguration

        private IProgramInformationTable ProgramInfoTable { get; set; }

        #endregion // Properties

        #region IPartImportsSatisfiedNotification

        /// <inheritdoc />
        public void Initialize()
        {
        }

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            var initializedCoreStreamUtils = INTV.Core.Utility.IStorageAccessHelpers.Initialize(new StorageAccess());
            if (initializedCoreStreamUtils)
            {
                INTV.Core.Utility.StringUtilities.RegisterHtmlDecoder(Shared.Utility.StringUtilities.HtmlDecode);
                INTV.Core.Utility.StringUtilities.RegisterHtmlEncoder(Shared.Utility.StringUtilities.HtmlEncode);
            }
            System.Diagnostics.Debug.Assert(initializedCoreStreamUtils, "Failed to initialize stream utilities!");
            Core.Model.IRomHelpers.InitializeCallbacks(GetIntvNameData);
            var documentFolderName = SingleInstanceApplication.AppInfo.DocumentFolderName;
            _applicationDocumentsPath = Path.Combine(PathUtils.GetDocumentsDirectory(), documentFolderName);
            if (!Directory.Exists(_applicationDocumentsPath))
            {
                Directory.CreateDirectory(_applicationDocumentsPath);
            }
            BackupDataDirectory = System.IO.Path.Combine(_applicationDocumentsPath, BackupDataArea);
            ErrorLogDirectory = Path.Combine(_applicationDocumentsPath, ErrorLogDir);
            RomsDirectory = Path.Combine(_applicationDocumentsPath, RomsDir);
            ManualsDirectory = Path.Combine(_applicationDocumentsPath, ManualsDir);
            BoxesDirectory = Path.Combine(_applicationDocumentsPath, BoxesDir);
            OverlaysDirectory = Path.Combine(_applicationDocumentsPath, OverlaysDir);
            LabelsDirectory = Path.Combine(_applicationDocumentsPath, LabelsDir);

            var directories = new[] { _applicationDocumentsPath, RomsDirectory, ManualsDirectory, BoxesDirectory, OverlaysDirectory, LabelsDirectory, BackupDataDirectory, ErrorLogDirectory };
            FileUtilities.EnsureDirectoriesExist(directories);

            RomFilesPath = Path.Combine(_applicationDocumentsPath, RomsFile);
            var localRomDefinitionsPath = new StorageLocation(Path.Combine(_applicationDocumentsPath, LocalRomDefintions));
            var localInfoTables = new ProgramInformationTableDescriptor[]
            {
                new ProgramInformationTableDescriptor(localRomDefinitionsPath, UserSpecifiedProgramInformationTable.Initialize)
            };
            ProgramInfoTable = ProgramInformationTable.Initialize(localInfoTables);
        }

        #endregion // IPartImportsSatisfiedNotification

        /// <summary>
        /// Gets the ROM list backup directory.
        /// </summary>
        /// <returns>The ROM list backup directory.</returns>
        public string GetROMListBackupDirectory()
        {
            var romListBackupDirectory = System.IO.Path.Combine(BackupDataDirectory, INTV.Shared.Utility.PathUtils.GetTimeString());
            return romListBackupDirectory;
        }

        /// <summary>
        /// Gets a program's name, copyright and other data by introspection assuming standard ROM layout.
        /// </summary>
        /// <param name="rom">The ROM to inspect.</param>
        /// <returns>An enumeration of strings that contains information about the ROM in a well-ordered format, using the intvname utility.</returns>
        private static string[] GetIntvNameData(INTV.Core.Model.IRom rom)
        {
            var intvNameData = new string[(int)RomInfoIndex.NumEntries];
            var jzIntv = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<JzIntv.Model.Configuration>();
            string intvname = jzIntv.GetProgramPath(JzIntv.Model.ProgramFile.IntvName);
            if ((intvname != null) && System.IO.File.Exists(intvname))
            {
                var localRomCopy = rom;
                if (rom.RomPath.Path.IsPathOnRemovableDevice())
                {
                    localRomCopy = rom.CopyToLocalRomsDirectory();
                }

                if (localRomCopy.IsUsingStockCfgFilePath())
                {
                    // intvname tool requires .cfg next to .bin, so make a copy of the stock file next to the ROM.
                    var stockCfgFilePath = localRomCopy.ConfigPath;
                    var cfgFilePath = localRomCopy.RomPath.ChangeExtension(ProgramFileKind.CfgFile.FileExtension());
                    File.Copy(stockCfgFilePath.Path, cfgFilePath.Path, true);
                    localRomCopy.UpdateCfgFile(cfgFilePath);
                }

                var results = INTV.Shared.Utility.RunExternalProgram.CallAndReturnStdOut(intvname, "\"" + localRomCopy.RomPath + "\"", string.Empty);
                if (!string.IsNullOrWhiteSpace(results))
                {
                    intvNameData = System.Text.RegularExpressions.Regex.Split(results, "\r\n|\r|\n");
                    for (int i = 0; i < intvNameData.Length; ++i)
                    {
                        var rawEntry = intvNameData[i];
                        var unescapedEntry = INTV.Core.Utility.StringUtilities.UnescapeString(rawEntry, null);
                        intvNameData[i] = unescapedEntry;
                    }
                    if (intvNameData.Length > 0)
                    {
                        intvNameData[0] = System.Text.RegularExpressions.Regex.Replace(intvNameData[0], @"\s+", " ").Trim();
                    }
                }
                if (string.IsNullOrEmpty(intvNameData[0]))
                {
                    intvNameData[0] = Path.GetFileNameWithoutExtension(rom.RomPath.Path);
                }
                if (string.Equals(intvNameData[0], "IntyBASIC program", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Just use the file name.
                    intvNameData[0] = Path.GetFileNameWithoutExtension(localRomCopy.RomPath.Path);
                }
            }
            return intvNameData;
        }
    }
}
