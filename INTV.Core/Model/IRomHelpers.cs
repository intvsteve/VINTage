// <copyright file="IRomHelpers.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Core.Utility;

namespace INTV.Core.Model
{
    /// <summary>
    /// Defines a delegate used to extract the title, copyright date, and other data from a ROM, as strings.
    /// </summary>
    /// <param name="rom">The ROM whose information is desired.</param>
    /// <returns>An enumerable of strings that may contain the title of the ROM, copyright, and other information.</returns>
    public delegate IEnumerable<string> GetRomInformationDelegate(IRom rom);

    /// <summary>
    /// Support methods for working with the IRom interface.
    /// </summary>
    public static class IRomHelpers
    {
        #region Configuration

        /// <summary>
        /// Key for the tools directory configuration value.
        /// </summary>
        public static readonly string ToolsDirectoryKey = "ToolsDirectoryKey";

        private static readonly Dictionary<string, object> Configuration = new Dictionary<string, object>();

        /// <summary>
        /// Gets the directory containing tools.
        /// </summary>
        public static string ToolsDirectory
        {
            get { return GetConfigurationEntry<string>(ToolsDirectoryKey); }
        }

        /// <summary>
        /// Adds a configuration-based data value to the general set of configurable values.
        /// </summary>
        /// <param name="entryName">The name of the configuration entry, used to retrieve the value.</param>
        /// <param name="value">The value of the configuration entry.</param>
        public static void AddConfigurationEntry(string entryName, object value)
        {
            Configuration[entryName] = value;
        }

        /// <summary>
        /// Gets the value of an entry.
        /// </summary>
        /// <typeparam name="T">The data type of the entry.</typeparam>
        /// <param name="entryName">The name of the entry whose value is desired.</param>
        /// <returns>The value of the entry.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if no entry exists for the given <paramref name="entryName"/>.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="entryName"/> is <c>null</c>.</exception>
        public static T GetConfigurationEntry<T>(string entryName)
        {
            return (T)Configuration[entryName];
        }

        /// <summary>
        /// Initialize delegates used in the implementation.
        /// </summary>
        /// <param name="getRomInformationDelegate">The delegate to call to get basic information about a ROM.</param>
        public static void InitializeCallbacks(GetRomInformationDelegate getRomInformationDelegate)
        {
            GetRomInfo = getRomInformationDelegate;
        }

        private static GetRomInformationDelegate GetRomInfo { get; set; }

        #endregion // Configuration

        #region ProgramFeatures-related Helpers

        /// <summary>
        /// Get additional ROM features directly from the ROM if possible.
        /// </summary>
        /// <param name="rom">The ROM whose features are desired.</param>
        /// <returns>The ROM features.</returns>
        /// <remarks>At this time, the implementation only extracts additional features from LUIGI-format ROMs. Future updates
        /// may support parsing .cfg files associated with BIN format ROMs.</remarks>
        public static ProgramFeatures GetProgramFeatures(this IRom rom)
        {
            var features = ProgramFeatures.GetUnrecognizedRomFeatures();
            if (rom.Format == RomFormat.Luigi)
            {
                var header = rom.GetLuigiHeader();
                features = ProgramFeatures.Combine(features, header.Features.ToProgramFeatures());
                features = ProgramFeatures.Combine(features, header.Features2.ToProgramFeatures());
            }
            return features;
        }

        #endregion // ProgramFeatures-related Helpers

        #region IProgramInformation-related Helpers

        /// <summary>
        /// Gets program information from a ROM.
        /// </summary>
        /// <param name="rom">The ROM from which to retrieve information.</param>
        /// <returns>The program information.</returns>
        /// <remarks>Program information will be honored according to the following order or precedence rules:
        /// 1. Program databases
        /// 2. ROM-specific metadata
        ///   a. LUIGI metadata check
        ///   b. ROM ID tag metadata check
        /// 3. intvname utility check (which has its own internal order-of-precedence, and should cover .bin+cfg)
        /// If multiple sources are available, an attempt is made to merge the results.</remarks>
        public static IProgramInformation GetProgramInformation(this IRom rom)
        {
            // NOTE: ROM database is still CRC-based, and does not use a RomComparer!
            var programInfo = ProgramInformationTable.Default.FindProgram(rom.Crc);
            rom.EnsureCfgFileProvided(programInfo);
            IProgramInformation metadataProgramInfo = rom.GetLuigiFileMetadata();
            if (metadataProgramInfo == null)
            {
                metadataProgramInfo = rom.GetRomFileMetadata();
            }
            IProgramInformation intvNameInfo = null;
            if (programInfo == null)
            {
                var programInfoData = rom.GetRomInformation();
                var programName = programInfoData.GetRomInfoString(RomInfoIndex.Name);
                var programYear = programInfoData.GetRomInfoString(RomInfoIndex.Copyright);
                intvNameInfo = new UserSpecifiedProgramInformation(rom.Crc, programName, programYear, rom.GetProgramFeatures());
                var programShortName = programInfoData.GetRomInfoString(RomInfoIndex.ShortName);
                if (!string.IsNullOrEmpty(programShortName))
                {
                    intvNameInfo.ShortName = programShortName;
                }
            }
            var primaryInfo = programInfo ?? metadataProgramInfo ?? intvNameInfo;
            var secondaryInfo = object.ReferenceEquals(primaryInfo, metadataProgramInfo) ? intvNameInfo : metadataProgramInfo ?? intvNameInfo;
            var tertiaryInfo = object.ReferenceEquals(secondaryInfo, intvNameInfo) ? null : intvNameInfo;
            if (secondaryInfo != null)
            {
                if (tertiaryInfo != null)
                {
                    programInfo = primaryInfo.Merge(
                        ProgramInformationMergeFieldsFlags.All,
                        new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(secondaryInfo, ProgramInformationMergeFieldsFlags.All),
                        new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(tertiaryInfo, ProgramInformationMergeFieldsFlags.All));
                }
                else
                {
                    programInfo = primaryInfo.Merge(
                        ProgramInformationMergeFieldsFlags.All,
                        new Tuple<IProgramInformation, ProgramInformationMergeFieldsFlags>(secondaryInfo, ProgramInformationMergeFieldsFlags.All));
                }
            }
            else
            {
                programInfo = primaryInfo;
            }
            return programInfo;
        }

        /// <summary>
        /// Gets a program's descriptive information by introspection assuming standard ROM layout.
        /// </summary>
        /// <param name="rom">The ROM to inspect.</param>
        /// <returns>Information about the ROM, ordered as described by RomInfoIndex.</returns>
        public static IEnumerable<string> GetRomInformation(this IRom rom)
        {
            return GetRomInfo(rom);
        }

        #endregion // IProgramInformation-related Helpers

        #region ROM-related Helpers

        /// <summary>
        /// Gets IProgramInformation from a .ROM-format file's metadata, if it is available.
        /// </summary>
        /// <param name="rom">The ROM from which metadata-based information is retrieved.</param>
        /// <returns>IProgramInformation retrieved from the .ROM-format ROM.</returns>
        public static RomFileMetadataProgramInformation GetRomFileMetadata(this IRom rom)
        {
            RomFileMetadataProgramInformation programInfo = null;
            var romRom = Rom.AsSpecificRomType<RomFormatRom>(rom);
            if (romRom != null)
            {
                programInfo = new RomFileMetadataProgramInformation(rom);
                if (!programInfo.Metadata.Any())
                {
                    programInfo = null;
                }
            }
            return programInfo;
        }

        #endregion // ROM-related Helpers

        #region LUIGI-related Helpers

        /// <summary>
        /// Safely retrieves the LUIGI header for a ROM.
        /// </summary>
        /// <param name="rom">The ROM whose LUIGI header is requested.</param>
        /// <returns>The <see cref="LuigiFileHeader"/> for the ROM, or <c>null</c> if the ROM is not in the LUIGI format.</returns>
        public static LuigiFileHeader GetLuigiHeader(this IRom rom)
        {
            LuigiFileHeader luigiHeader = null;
            if ((rom != null) && (rom.Format == RomFormat.Luigi) && !string.IsNullOrEmpty(rom.RomPath) && rom.RomPath.FileExists() && LuigiFileHeader.PotentialLuigiFile(rom.RomPath))
            {
                using (var file = rom.RomPath.OpenFileStream())
                {
                    luigiHeader = LuigiFileHeader.Inflate(file);
                }
            }
            return luigiHeader;
        }

        /// <summary>
        /// Determines if the ROM will only run on LTO Flash! hardware.
        /// </summary>
        /// <param name="rom">The ROM to check.</param>
        /// <returns><c>true</c> if the ROM only runs on LTO Flash! hardware; otherwise, <c>false</c>.</returns>
        /// <remarks>Only LUIGI format ROMs are currently known to support this capability. Any LUIGI-format
        /// ROM that contains a 'scramble key' block is considered LTO Flash!-only. Three categories of
        /// behavior are currently defined:
        /// 1) ROM runs anywhere; if ROM is in LUIGI format, target must understand it; .bin and .rom formats are considered universally supported
        /// 2) ROM runs on any LTO Flash! device;
        /// 3) ROM runs only on a specific LTO Flash! device.</remarks>
        public static bool IsLtoFlashOnlyRom(this IRom rom)
        {
            var luigiRom = Rom.AsSpecificRomType<LuigiFormatRom>(rom);
            var isLtoFlashOnly = (luigiRom != null) && !string.IsNullOrEmpty(luigiRom.TargetDeviceUniqueId);
            return isLtoFlashOnly;
        }

        /// <summary>
        /// Gets the unique ID of the device for which a ROM may be targeted, if applicable.
        /// </summary>
        /// <param name="rom">The ROM whose target's unique ID is desired.</param>
        /// <returns>The ROM target device's unique identifier. If the ROM is not targeted to a
        /// general category of peripheral (e.g. LTO Flash!), or a specific device, the return
        /// value is <c>null</c>.</returns>
        public static string GetTargetDeviceUniqueId(this IRom rom)
        {
            var luigiRom = Rom.AsSpecificRomType<LuigiFormatRom>(rom);
            var uniqueId = (luigiRom != null) ? luigiRom.TargetDeviceUniqueId : null;
            return uniqueId;
        }

        /// <summary>
        /// Determines if the given ROM can execute on a specific device that has the given <paramref name="deviceUniqueId"/>.
        /// </summary>
        /// <param name="rom">The ROM to check.</param>
        /// <param name="deviceUniqueId">The unique device identifier to use to determine if the ROM can execute upon it.</param>
        /// <returns><c>true</c> if the ROM can execute on device identified by <paramref name="deviceUniqueId"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>By default, all non-LUIGI-format ROMs are assumed to be executable on any device. If <paramref name="rom"/>
        /// is a LUIGI-format ROM, and only runs on LTO Flash!, then the following rules apply:
        /// 1) If no scramble key block is present, the ROM will execute, regardless of the value if <paramref name="deviceUniqueId"/>
        /// 2) If a scramble key block is present, but the TargetDeviceUniqueId contained within it is <see cref="LuigiScrambleKeyBlock.AnyLTOFlashId"/>, the ROM will execute
        /// 3) If a scramble key block is present, the ROM is executable only if the ROM's scramble key block contains a TargetDeviceUniqueId that matches <paramref name="deviceUniqueId"/>.</remarks>
        public static bool CanExecuteOnDevice(this IRom rom, string deviceUniqueId)
        {
            var canExecuteOnDevice = true;
            if ((rom != null) && !string.IsNullOrEmpty(deviceUniqueId))
            {
                var luigiRom = Rom.AsSpecificRomType<LuigiFormatRom>(rom);
                if (luigiRom != null)
                {
                    var targetDeviceId = luigiRom.TargetDeviceUniqueId;
                    canExecuteOnDevice = string.IsNullOrEmpty(targetDeviceId) || targetDeviceId.Equals(LuigiScrambleKeyBlock.AnyLTOFlashId) || targetDeviceId.Equals(deviceUniqueId);
                }
            }
            return canExecuteOnDevice;
        }

        /// <summary>
        /// Gets IProgramInformation from a LUIGI file's metadata, if it is available.
        /// </summary>
        /// <param name="rom">The ROM from which metadata-based information is retrieved.</param>
        /// <returns>IProgramInformation retrieved from the LUIGI format ROM.</returns>
        public static LuigiFileMetadataProgramInformation GetLuigiFileMetadata(this IRom rom)
        {
            LuigiFileMetadataProgramInformation programInfo = null;
            var luigiRom = Rom.AsSpecificRomType<LuigiFormatRom>(rom);
            if (luigiRom != null)
            {
                try
                {
                    var metadata = luigiRom.LocateDataBlock<LuigiMetadataBlock>();
                    if (metadata != null)
                    {
                        programInfo = new LuigiFileMetadataProgramInformation(luigiRom.Header, metadata);
                    }
                }
                catch (Exception e)
                {
                    // We don't really want to raise a lot of trouble if this is somehow wrong... Perhaps we should report this more aggressively
                    // if it happens in the field, but for now, quietly fail.
                    System.Diagnostics.Debug.WriteLine("Failed to get LUIGI metadata. Error: " + e.Message);
#if DEBUG
                    throw;
#endif
                }
            }
            return programInfo;
        }

        #endregion // LUIGI-related Helpers

        #region Stock Configuration File Helpers

        /// <summary>
        /// Update the config file path of a ROM.
        /// </summary>
        /// <param name="rom">The ROM whose config file path is being updated.</param>
        /// <param name="cfgFile">The new config file path.</param>
        public static void UpdateCfgFile(this IRom rom, string cfgFile)
        {
            Rom.ReplaceCfgPath(rom, cfgFile);
        }

        /// <summary>
        /// Retrieves a 'stock' .cfg file if the given ROM is of .bin format, and does not already have a .cfg associated with it.
        /// </summary>
        /// <param name="rom">The ROM for which a .cfg is needed.</param>
        /// <param name="programInfo">Program information for the ROM.</param>
        /// <returns>The path to a 'stock' .cfg file. These 'canonical' configuration files have been known to work with ROMs distributed
        /// with various products from Intellivision Productions' emulators, jzIntv, and other emulators. If the ROM cannot be identified
        /// via its CRC, then a default configuration for the ROM is assumed.</returns>
        public static string GetStockCfgFile(this IRom rom, IProgramInformation programInfo)
        {
            string stockCfgFilePath = null;
            if (rom.Format == RomFormat.Bin)
            {
                // The following is disabled because it turns out it may cause an infinite recursion. I.e. if programInfo is null,
                // the act of retrieving program information on a .bin-format ROM w/o a .cfg file may result in... get this... another
                // call to this method with a null programInfo!
                ////programInfo = programInfo ?? rom.GetProgramInformation();
                stockCfgFilePath = GetStockCfgFile(rom.Crc, rom.RomPath, programInfo);
            }
            return stockCfgFilePath;
        }

        /// <summary>
        /// Retrieves a 'stock' .cfg file if the given CRC matches a known ROM in .bin format.
        /// </summary>
        /// <param name="crc">THe CRC of the ROM whose corresponding .cfg file is desired.</param>
        /// <param name="romPath">Absolute path to the ROM on disk.</param>
        /// <param name="programInfo">Program information for the ROM.</param>
        /// <returns>The path to a 'stock' .cfg file. These 'canonical' configuration files have been known to work with ROMs distributed
        /// with various products from Intellivision Productions' emulators, jzIntv, and other emulators. If the ROM cannot be identified
        /// via its CRC, then a default configuration for the ROM is assumed.</returns>
        public static string GetStockCfgFile(uint crc, string romPath, IProgramInformation programInfo)
        {
            string stockCfgFilePath = null;
            var stockConfigFileNumber = 0; // the default
            if (programInfo != null)
            {
                // This direct ROM CRC compare is necessary because we don't *have* a .cfg file to check against.
                var crcData = programInfo.Crcs.FirstOrDefault(c => c.Crc == crc);
                if (crcData != null)
                {
                    stockConfigFileNumber = crcData.BinConfigTemplate;
                }
            }
            var stockCfgFileName = stockConfigFileNumber.ToString() + ProgramFileKind.CfgFile.FileExtension();
            var stockCfgUri = new Uri(ToolsDirectory + stockCfgFileName);
            stockCfgFilePath = Uri.UnescapeDataString(stockCfgUri.AbsolutePath); // Need to unescape spaces.
#if WIN
            stockCfgFilePath = stockCfgFilePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
#elif PCL
            // NOTE: This will, of course, cause trouble if we build PCL for non-Windows platforms, in which case the proper
            // solution is to register the 'FixUpUri' method and use it instead... In fact, it would be better to just have a
            // file system interface to use for stuff like this... Maybe imported via MEF or some such...
            stockCfgFilePath = stockCfgFilePath.Replace('/', '\\');
#endif
            if (!stockCfgFilePath.FileExists())
            {
                stockCfgFilePath = null;
            }
            return stockCfgFilePath;
        }

        /// <summary>
        /// Ensures that a configuration file can be located for a given ROM. If the ROM does not already specify one, the best possible matching file is returned.
        /// </summary>
        /// <param name="rom">A ROM that may need a configuration (.cfg) file.</param>
        /// <param name="programInfo">Detailed <see cref="IProgramInformation"/> providing detailed data about the ROM.</param>
        /// <returns>An absolute path to the best known configuration file for <paramref name="rom"/>, using a stock file if necessary.</returns>
        /// <remarks>If a ROM requires a configuration file, but one cannot be found, the best possible matching file will be supplied. This is only applicable
        /// to .bin format ROMs (or their compatriots, .itv and .int files, typically). If <paramref name="rom"/> either does not provide a .cfg file, or
        /// the file it provides cannot be found, the best possible match based on <paramref name="programInfo"/> will be used. If <paramref name="programInfo"/>
        /// is also null, the CRC of the ROM will be checked against the active ROM database in memory for a possible stock .cfg file match, and a path to a
        /// stock file will be returned.</remarks>
        public static bool EnsureCfgFileProvided(this IRom rom, IProgramInformation programInfo)
        {
            var usesStockCfgFile = false;
            if ((rom.Format == RomFormat.Bin) && (string.IsNullOrEmpty(rom.ConfigPath) || !rom.ConfigPath.FileExists()))
            {
                var cfgFilePath = GetStockCfgFile(rom.Crc, rom.RomPath, programInfo);
                usesStockCfgFile = !string.IsNullOrEmpty(cfgFilePath) && cfgFilePath.FileExists();
                if (usesStockCfgFile)
                {
                    rom.UpdateCfgFile(cfgFilePath);
                }
            }
            return usesStockCfgFile;
        }

        #endregion // Stock Configuration File Helpers

        /// <summary>
        /// Checks the given ROM to determine if it is operating as an alternative version (referring to a backup location).
        /// </summary>
        /// <param name="rom">The <see cref="IRom"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> refers to an alternative location of the ROM and its configuration file.</returns>
        public static bool IsAlternateRom(this IRom rom)
        {
            return rom is AlternateRom;
        }

        /// <summary>
        /// Gets the original <see cref="IRom"/> referred to by <paramref name="rom"/>.
        /// </summary>
        /// <param name="rom">The ROM whose original is desired.</param>
        /// <returns>The original ROM. Unless <paramref name="rom"/> refers to a ROM located on external devices, and that device is not accessible, this is usually just <paramref name="rom"/>.</returns>
        public static IRom OriginalRom(this IRom rom)
        {
            var originalRom = rom;
            var alternateRom = Rom.AsSpecificRomType<AlternateRom>(rom);
            if (alternateRom != null)
            {
                originalRom = alternateRom.Original;
            }
            else
            {
                var xmlRom = Rom.AsSpecificRomType<XmlRom>(rom);
                if (xmlRom != null)
                {
                    originalRom = xmlRom.ResolvedRom;
                }
            }
            return originalRom;
        }
    }
}
