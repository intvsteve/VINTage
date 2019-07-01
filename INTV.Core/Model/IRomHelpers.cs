// <copyright file="IRomHelpers.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static readonly string DefaultToolsDirectoryKey = "DefaultToolsDirectoryKey";

        private static readonly Dictionary<string, object> Configuration = new Dictionary<string, object>();

        /// <summary>
        /// Gets the directory containing tools.
        /// </summary>
        public static string DefaultToolsDirectory
        {
            get { return GetConfigurationEntry<string>(DefaultToolsDirectoryKey); }
        }

        /// <summary>
        /// Sets or adds a configuration-based data value to the general set of configurable values.
        /// </summary>
        /// <param name="entryName">The name of the configuration entry, used to retrieve the value.</param>
        /// <param name="value">The value of the configuration entry.</param>
        public static void SetConfigurationEntry(string entryName, object value)
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
        public static ProgramFeatures GetProgramFeatures(this IRom rom)
        {
            var features = ProgramFeatures.GetUnrecognizedRomFeatures();

            // TODO: Add support for CFGVAR and ROM metadata features!
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
        ///   c. CFGVAR metadata check
        /// 3. intvname utility check (which has its own internal order-of-precedence, likely overlapping with this)
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
            if (metadataProgramInfo == null)
            {
                metadataProgramInfo = rom.GetBinFileMetadata();
            }

            var programInfoData = rom.GetRomInformation();
            var programName = programInfoData.GetRomInfoString(RomInfoIndex.Name);
            var programYear = programInfoData.GetRomInfoString(RomInfoIndex.Copyright);
            var programShortName = programInfoData.GetRomInfoString(RomInfoIndex.ShortName);
            var intvNameInfo = new UserSpecifiedProgramInformation(rom.Crc, programName, programYear, rom.GetProgramFeatures());
            if (programInfoData.Any(s => !string.IsNullOrEmpty(s)))
            {
                if (!string.IsNullOrEmpty(programShortName))
                {
                    intvNameInfo.ShortName = programShortName;
                }

                if (programInfo != null)
                {
                    // If we have a database entry, which is the ultimate arbiter of "known" -- either a-priory, or as specified
                    // explicitly by the user as a new entry for the database, then strip the "unrecognized-ness" from the intvname
                    // data.  Presumably, the database entry specified these values. We do not want to re-mark the features as unknown
                    // when merging intvname information into a database entry's information. The ROM metadata that can be directly
                    // harvested from a program, however, does not track these aspects of a program, hence we will retain the values
                    // in the situation where we may have nearly complete feature data about a program aside from what the database tracks.
                    intvNameInfo.Features.ClearUnrecongizedRomFeatures();
                }
            }
            else if ((programInfo != null) || (metadataProgramInfo != null))
            {
                intvNameInfo = null;
            }

            if ((programInfo == null) && (metadataProgramInfo != null))
            {
                // Mark features with unrecognized settings, since we don't have a database entry.
                metadataProgramInfo.Features.SetUnrecongizedRomFeatures();
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

        #region BIN-related Helpers

        /// <summary>
        /// Gets IProgramInformation from a .BIN-format ROM's metadata, if it is available.
        /// </summary>
        /// <param name="rom">The ROM from which metadata-based information is retrieved.</param>
        /// <returns>IProgramInformation retrieved from the .BIN-format ROM's .cfg data.</returns>
        public static CfgFileMetadataProgramInformation GetBinFileMetadata(this IRom rom)
        {
            CfgFileMetadataProgramInformation programInfo = null;
            var binRom = Rom.AsSpecificRomType<BinFormatRom>(rom);
            if (binRom != null)
            {
                programInfo = new CfgFileMetadataProgramInformation(rom);
                if (!programInfo.Metadata.Any())
                {
                    programInfo = null;
                }
            }
            return programInfo;
        }

        #endregion // BIN-related Helpers

        #region ROM-related Helpers

        /// <summary>
        /// Gets IProgramInformation from a .ROM-format image's metadata, if it is available.
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
            if ((rom != null) && (rom.Format == RomFormat.Luigi) && rom.RomPath.IsValid && rom.RomPath.Exists() && LuigiFileHeader.PotentialLuigiFile(rom.RomPath))
            {
                luigiHeader = LuigiFileHeader.GetHeader(rom.RomPath);
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
        /// Gets IProgramInformation from a LUIGI image's metadata, if it is available.
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
#endif // DEBUG
                }
            }
            return programInfo;
        }

        #endregion // LUIGI-related Helpers

        #region Stock Configuration File Helpers

        /// <summary>
        /// Update the config location of a ROM.
        /// </summary>
        /// <param name="rom">The ROM whose configuration location is being updated.</param>
        /// <param name="cfgLocation">The new configuration location.</param>
        public static void UpdateCfgFile(this IRom rom, StorageLocation cfgLocation)
        {
            Rom.ReplaceCfgPath(rom, cfgLocation);
        }

        /// <summary>
        /// Retrieves a 'stock' .cfg data if the given ROM is of .bin format, and does not already have a .cfg associated with it.
        /// </summary>
        /// <param name="rom">The ROM for which a .cfg is needed.</param>
        /// <param name="programInfo">Program information for the ROM.</param>
        /// <returns>The location of a 'stock' .cfg data. These 'canonical' configuration data have been known to work with ROMs distributed
        /// with various products from Intellivision Productions' emulators, jzIntv, and other emulators. If the ROM cannot be identified
        /// via its CRC, then a default configuration for the ROM is assumed.</returns>
        public static StorageLocation GetStockCfgFile(this IRom rom, IProgramInformation programInfo)
        {
            var stockCfgFilePath = StorageLocation.InvalidLocation;
            if (rom.Format == RomFormat.Bin)
            {
                // The following is disabled because it turns out it may cause an infinite recursion. I.e. if programInfo is null,
                // the act of retrieving program information on a .bin-format ROM w/o a .cfg data may result in... get this... another
                // call to this method with a null programInfo!
                ////programInfo = programInfo ?? rom.GetProgramInformation();
                stockCfgFilePath = GetStockCfgFile(rom.Crc, rom.RomPath, programInfo);
            }
            return stockCfgFilePath;
        }

        /// <summary>
        /// Retrieves a 'stock' .cfg data if the given CRC matches a known ROM in .bin format.
        /// </summary>
        /// <param name="crc">THe CRC of the ROM whose corresponding .cfg data is desired.</param>
        /// <param name="romLocation">Location of the ROM.</param>
        /// <param name="programInfo">Program information for the ROM.</param>
        /// <returns>The path to a 'stock' .cfg data. These 'canonical' configuration data have been known to work with ROMs distributed
        /// with various products from Intellivision Productions' emulators, jzIntv, and other emulators. If the ROM cannot be identified
        /// via its CRC, then a default configuration for the ROM is assumed.</returns>
        public static StorageLocation GetStockCfgFile(uint crc, StorageLocation romLocation, IProgramInformation programInfo)
        {
            var stockConfigFileNumber = 0; // the default
            if (programInfo != null)
            {
                // This direct ROM CRC compare is necessary because we don't *have* a .cfg data to check against.
                var crcData = programInfo.Crcs.FirstOrDefault(c => c.Crc == crc);
                if (crcData != null)
                {
                    stockConfigFileNumber = crcData.BinConfigTemplate;
                }
            }
            var stockCfgFilePath = GetStockCfgFilePath(stockConfigFileNumber);
            return stockCfgFilePath;
        }

        /// <summary>
        /// Gets a path to an existing stock configuration location given its canonical CFG identifier.
        /// </summary>
        /// <param name="stockConfigFileNumber">A positive integer value indicating which canonical configuration data whose disk location is desired.</param>
        /// <returns>The location of the data, or <see cref="StorageLocation.InvalidLocation"/> if data for the given canonical configuration data does not exist.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="stockConfigFileNumber"/> is less than zero.</exception>
        public static StorageLocation GetStockCfgFilePath(int stockConfigFileNumber)
        {
            var stockCfgFileName = stockConfigFileNumber.ToString(CultureInfo.InvariantCulture) + ProgramFileKind.CfgFile.FileExtension();
            var stockCfgUri = new Uri(DefaultToolsDirectory + stockCfgFileName);
            var stockCfgFilePath = new StorageLocation(Uri.UnescapeDataString(stockCfgUri.AbsolutePath)); // Need to unescape spaces.
#if WIN
            stockCfgFilePath = stockCfgFilePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
#elif PCL
            // NOTE: This will, of course, cause trouble if we build PCL for non-Windows platforms, in which case the proper
            // solution is to register the 'FixUpUri' method and use it instead... Consider having this on the IStorageAccess
            // interface or the StorageLocation type itself?
            stockCfgFilePath = new StorageLocation(stockCfgFilePath.Path.Replace('/', '\\'));
#endif // WIN
            if (!stockCfgFilePath.Exists())
            {
                stockCfgFilePath = StorageLocation.InvalidLocation;
            }
            return stockCfgFilePath;
        }

        /// <summary>
        /// Ensures that a configuration data can be located for a given ROM.
        /// </summary>
        /// <param name="rom">A ROM that may need configuration (.cfg) data.</param>
        /// <param name="programInfo">Detailed <see cref="IProgramInformation"/> providing detailed data about the ROM.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> uses an existing stock CFG data, <c>false</c> otherwise.</returns>
        /// <remarks>If a ROM requires configuration data, but one cannot be found, the best possible matching data will be searched for. This is only applicable
        /// to .bin format ROMs (or their compatriots, .itv and .int files, typically). If <paramref name="rom"/> either does not provide a .cfg, or
        /// the data it provides cannot be found, the best possible match based on <paramref name="programInfo"/> will be used. If <paramref name="programInfo"/>
        /// is also null, the CRC of the ROM will be checked against the active ROM database in memory for a possible stock .cfg data match. If the matching
        /// stock data is found, then the function returns <c>true</c>.</remarks>
        public static bool EnsureCfgFileProvided(this IRom rom, IProgramInformation programInfo)
        {
            var usesStockCfgFile = false;
            if ((rom.Format == RomFormat.Bin) && (!rom.ConfigPath.IsValid || !rom.ConfigPath.Exists()))
            {
                var cfgFileLocation = GetStockCfgFile(rom.Crc, rom.RomPath, programInfo);
                usesStockCfgFile = cfgFileLocation.IsValid;
                if (usesStockCfgFile)
                {
                    rom.UpdateCfgFile(cfgFileLocation);
                }
            }
            return usesStockCfgFile;
        }

        #endregion // Stock Configuration File Helpers

        /// <summary>
        /// Checks the given ROM to determine if it is operating as an alternative version (referring to a backup location).
        /// </summary>
        /// <param name="rom">The <see cref="IRom"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="rom"/> refers to an alternative location of the ROM and its configuration data.</returns>
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

        /// <summary>
        /// Checks if the given ROM's format is compatible with the given format.
        /// </summary>
        /// <param name="rom">The ROM whose format's compatibility is to be checked.</param>
        /// <param name="romFormat">The other ROM format.</param>
        /// <param name="considerOriginalFormat">If <c>true</c> and <paramref name="rom"/> is of a format that may have a different 'format of origin' such as LUIGI, the also consider that format.</param>
        /// <returns><c>true</c> if <paramref name="rom"/>'s format is compatible with <paramref name="romFormat"/>, or, if <paramref name="considerOriginalFormat"/> is <c>true</c>,
        /// the original ROM format is compatible with <paramref name="romFormat"/>.</returns>
        public static bool MatchingRomFormat(this IRom rom, RomFormat romFormat, bool considerOriginalFormat)
        {
            var match = false;
            if (rom != null)
            {
                match = rom.Format.IsCompatibleWithRomFormat(romFormat);
                if (!match && (rom.Format == RomFormat.Luigi) && considerOriginalFormat)
                {
                    var luigiHeader = rom.GetLuigiHeader();
                    match = (luigiHeader != null) && romFormat.IsCompatibleWithRomFormat(luigiHeader.OriginalRomFormat);
                }
            }
            return match;
        }

        /// <summary>
        /// Checks if the given ROM's CRC data matches the given program identifier with selectable strictness. See remarks.
        /// </summary>
        /// <param name="rom">The ROM to check for a match.</param>
        /// <param name="programIdentifier">The program identifier to match.</param>
        /// <param name="cfgCrcMustMatch">If <c>true</c> and a .BIN format ROM is being checked, determines whether the CRC of the config data must match as well.</param>
        /// <returns><c>true</c> if <paramref name="rom"/>'s CRC data matches the given <paramref name="programIdentifier"/>, <c>false</c> otherwise.</returns>
        /// <remarks><para>For .BIN format ROMs, there is the possibility, depending on the value if <paramref name="cfgCrcMustMatch"/>, that a match is reported, even though
        /// the .CFG data could have a profound impact on the actual ROM being used. The .CFG data can actually cause modifications to the ROM itself, or describe different
        /// features in the ROM (e.g. different compatibility or metadata). While metadata and compatibility differences could be considered cosmetic only, they can affect
        /// how the ROM is handled by hardware in the case of LTO Flash! for example. Most important of course is the matter of the memory map, RAM mappings, or actual
        /// code modification (e.g. the patch for Dreadnaught Factor for PAL systems). That said, until .CFG CRC values are computed on a 'canonicalized' form of the .CFG
        /// data, it is important to note that mere whitespace changes will cause .CFG mismatches, making it fragile to do full compares when working with database.</para>
        /// <para>Similarly, when working with the .ROM format, it is important to note that at this time, the CRC is computed for the entire data, including metadata that
        /// may be provided. This means that mismatches can occur because a documentation credit was modified in the metadata of one version of the data, even though
        /// the executable ROM itself was not changed.</para>
        /// <para>Finally, for the LUIGI format, there is an additional challenge. To this point, this ROM format is only used as the destination format for ROMs copied to
        /// the LTO Flash! hardware, though the jzIntv emulator supports it as well. As such, it is more than a container format for .BIN or .ROM formats, as it is different
        /// in several important ways. However, it has not yet (and possibly will never become) a target ROM format. It addresses some deficiencies in the original .ROM format
        /// that parallel the limitations of the original Intellicart and CuttleCart 3 hardware. That said, in theory the LUIGI header's UID entry poses a challenge. If the
        /// LUIGI image was created from a .ROM-format ROM, it is unambiguous. However, thereafter it is not clear if the image was created directly from the assembler, in which
        /// case the UID contains an as-yet-to-be-defined strong hash of something, or wither it contains a pair of 32-bit CRC values, one from the .BIN image, the other from
        /// the corresponding .CFG data. Given the nature of more modern Intellivision programs to commonly stray from the standard set of common .CFG data (and memory maps),
        /// it will be an interesting challenge to distinguish a LUIGI created via bin2luigi from one created directly by the assembler. In such a case, most likely we'd want
        /// to bump the version number of the LUIGI header and at least set a flag somewhere in the sea of bits that we swim through to figure out all these intricacies.</para></remarks>
        public static bool MatchesProgramIdentifier(this IRom rom, ProgramIdentifier programIdentifier, bool cfgCrcMustMatch)
        {
            var match = false;
            if (rom != null)
            {
                switch (rom.Format)
                {
                    case RomFormat.Bin:
                        match = (programIdentifier.DataCrc == rom.Crc) && (!cfgCrcMustMatch || (programIdentifier.OtherData == rom.CfgCrc));
                        break;
                    case RomFormat.Rom:
                    case RomFormat.CuttleCart3:
                    case RomFormat.CuttleCart3Advanced:
                        match = programIdentifier.DataCrc == rom.Crc;
                        break;
                    case RomFormat.Luigi:
                        var luigiHeader = rom.GetLuigiHeader();
                        if (luigiHeader != null)
                        {
                            match = programIdentifier.Id == luigiHeader.Uid; // probably will never use this result, but...
                            if (luigiHeader.OriginalRomFormat == RomFormat.Bin)
                            {
                                match = (programIdentifier.DataCrc == luigiHeader.OriginalRomCrc32) && (!cfgCrcMustMatch || (programIdentifier.OtherData == luigiHeader.OriginalCfgCrc32));
                            }
                            else if (luigiHeader.OriginalRomFormat == RomFormat.Rom)
                            {
                                match = programIdentifier.DataCrc == luigiHeader.OriginalRomCrc32;
                            }

                            // else .. Should we throw here? This situation (as of 2. Sept. 2018) cannot exist.
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
            return match;
        }

        /// <summary>
        /// Gets program metadata given a ROM.
        /// </summary>
        /// <param name="rom">The ROM whose metadata is desired.</param>
        /// <returns>The <see cref="IProgramMetadata"/> for the ROM, or <c>null</c> if it cannot be determined.</returns>
        public static IProgramMetadata GetProgramMetadata(this IRom rom)
        {
            IProgramMetadata programMetadata = null;
            if (rom != null)
            {
                switch (rom.Format)
                {
                    case RomFormat.Bin:
                        programMetadata = rom.GetBinFileMetadata();
                        break;
                    case RomFormat.Intellicart:
                    case RomFormat.CuttleCart3:
                    case RomFormat.CuttleCart3Advanced:
                        programMetadata = rom.GetRomFileMetadata();
                        break;
                    case RomFormat.Luigi:
                        programMetadata = rom.GetLuigiFileMetadata();
                        break;
                }
            }
            return programMetadata;
        }
    }
}
