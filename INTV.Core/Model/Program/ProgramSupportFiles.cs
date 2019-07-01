// <copyright file="ProgramSupportFiles.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Core.Model.Device;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// The support files associated with a program.
    /// </summary>
    /// <remarks>It's better to split out the XML serialized version to a separate class, but oh well. This crept in when StorageLocation was introduced.</remarks>
    public class ProgramSupportFiles : INTV.Core.ComponentModel.ModelBase
    {
        #region Property Names

        public const string RomImageLocationPropertyName = "RomImageLocation";
        public const string RomConfigurationFilePathPropertyName = "RomConfigurationFilePath";
        public const string RomConfigurationLocationPropertyName = "RomConfigurationLocation";
        public const string DefaultBoxImageLocationPropertyName = "DefaultBoxImageLocation";
        public const string DefaultOverlayImageLocationPropertyName = "DefaultOverlayImageLocation";
        public const string DefaultManualImageLocationPropertyName = "DefaultManualImageLocation";
        public const string DefaultLabelImageLocationPropertyName = "DefaultLabelImageLocation";
        public const string DefaultManualTextPathPropertyName = "DefaultManualTextPath";
        public const string DefaultManualTextLocationPropertyName = "DefaultManualTextLocation";
        public const string DefaultSaveDataPathPropertyName = "DefaultSaveDataPath";
        public const string DefaultSaveDataLocationPropertyName = "DefaultSaveDataLocation";
        public const string DefaultLtoFlashDataPathPropertyName = "DefaultLtoFlashDataPath";
        public const string DefaultLtoFlashDataLocationPropertyName = "DefaultLtoFlashDataLocation";
        public const string DefaultVignetteLocationPropertyName = "DefaultVignetteLocation";
        public const string DefaultReservedDataLocationPropertyName = "DefaultReservedDataLocation";

        #endregion // Property Names

        private Dictionary<ProgramFileKind, List<StorageLocation>> _supportFiles;
        private Dictionary<ProgramFileKind, ProgramSupportFileState> _supportFileStates;
        private IRom _programRom = null;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ProgramSupportFiles.
        /// </summary>
        /// <param name="programRom">The ROM for which to track support files.</param>
        public ProgramSupportFiles(IRom programRom)
            : this(programRom, null, null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ProgramSupportFiles.
        /// </summary>
        /// <param name="programRom">The ROM for which to track support files.</param>
        /// <param name="boxPath">The path to the default box art to use.</param>
        /// <param name="manualPath">The path to the default manual cover art to use.</param>
        /// <param name="manualTextPath">The path to the default manual text to use.</param>
        /// <param name="overlayPath">The path to the default overlay art to use.</param>
        /// <param name="labelPath">The path to the default label art to use.</param>
        public ProgramSupportFiles(IRom programRom, string boxPath, string manualPath, string manualTextPath, string overlayPath, string labelPath)
        {
            _programRom = programRom;
            _supportFiles = new Dictionary<ProgramFileKind, List<StorageLocation>>();
            _supportFileStates = new Dictionary<ProgramFileKind, ProgramSupportFileState>();
            _supportFiles[ProgramFileKind.Rom] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.CfgFile] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.Box] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.Label] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.Overlay] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.ManualCover] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.ManualText] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.SaveData] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.LuigiFile] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.Vignette] = new List<StorageLocation>();
            _supportFiles[ProgramFileKind.GenericSupportFile] = new List<StorageLocation>();
        }

        /// <summary>
        /// Initializes a new instance of ProgramSupportFiles.
        /// </summary>
        /// <remarks>This constructor exists so XmlSerializer is easy to use with it.</remarks>
        private ProgramSupportFiles()
            : this(null)
        {
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the program ROM.
        /// </summary>
        public IRom Rom
        {
            get { return _programRom; }
        }

        #region XML Properties

        /// <summary>
        /// Gets or sets the path to the program ROM for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use RomImageLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("RomImagePath")]
        public string XmlRomImagePath
        {
            get { return RomImageLocation.Path; }
            set { UpdateProgramRom(ProgramFileKind.Rom, value); }
        }

        /// <summary>
        /// Gets or sets the path to the ROM's configuration path for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use RomConfigurationLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("RomConfigurationFilePath")]
        public string XmlRomConfigurationFilePath
        {
            get { return RomConfigurationLocation.Path; }
            set { UpdateProgramRom(ProgramFileKind.CfgFile, value); }
        }

        /// <summary>
        /// Gets or sets the ROM file's alternate paths for XML Serialization.
        /// </summary>
        /// <remarks>Uses <see cref="List"/> to support XmlSerializer.</remarks>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use AlternateRomImageLocations instead.")]</remarks>
        [System.Xml.Serialization.XmlArray("AlternateRomImagePaths")]
        public List<string> XmlAlternateRomImagePaths
        {
            get
            {
                return _alternateRomImagePaths;
            }

            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException("XmlAlternateRomImagePaths");
                }
                _alternateRomImagePaths = value;
            }
        }
        private List<string> _alternateRomImagePaths;

        /// <summary>
        /// Gets or sets the ROM's alternate configuration file paths for XML Serialization.
        /// </summary>
        /// <remarks>Uses <see cref="List"/> to support XmlSerializer.</remarks>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use AlternateRomConfigurationLocations instead.")]</remarks>
        [System.Xml.Serialization.XmlArray("AlternateRomConfigurationFilePaths")]
        public List<string> XmlAlternateRomConfigurationFilePaths
        {
            get
            {
                return _alternateConfigurationFilePaths;
            }

            set
            {
                if (value == null)
                {
                    throw new System.ArgumentNullException("XmlAlternateRomConfigurationFilePaths");
                }
                _alternateConfigurationFilePaths = value;
            }
        }
        private List<string> _alternateConfigurationFilePaths;

        /// <summary>
        /// Gets or sets the location of the default box image for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultBoxImageLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("BoxImagePath")]
        public string XmlDefaultBoxImagePath
        {
            get { return DefaultBoxImageLocation.Path; }
            set { DefaultBoxImageLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to the default overlay image for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultOverlayImageLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("OverlayImagePath")]
        public string XmlDefaultOverlayImagePath
        {
            get { return DefaultOverlayImageLocation.Path; }
            set { DefaultOverlayImageLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to the default manual cover image for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultManualImageLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("ManualImagePath")]
        public string XmlDefaultManualImagePath
        {
            get { return DefaultManualCoverImageLocation.Path; }
            set { DefaultManualCoverImageLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to the default label image for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultLabelImageLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("LabelImagePath")]
        public string XmlDefaultLabelImagePath
        {
            get { return DefaultLabelImageLocation.Path; }
            set { DefaultLabelImageLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to the default program instructions for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultManualTextLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("ManualPath")]
        public string XmlDefaultManualTextPath
        {
            get { return DefaultManualTextLocation.Path; }
            set { DefaultManualTextLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to the default save data for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultSaveDataLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("SaveDataPath")]
        public string XmlDefaultSaveDataPath
        {
            get { return DefaultSaveDataLocation.Path; }
            set { DefaultSaveDataLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to the default LUIGI format version of the ROM for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultLtoFlashDataLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlElement("LTOFlashROMPath")]
        public string XmlDefaultLtoFlashDataPath
        {
            get { return DefaultLtoFlashDataLocation.Path; }
            set { DefaultLtoFlashDataLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the location of the default vignette for the ROM for XML Serialization.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultVignetteLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlIgnore]
        [System.Xml.Serialization.XmlElement("LTOFlashVignettePath")]
        public string XmlDefaultVignettePath
        {
            get { return DefaultVignetteLocation.Path; }
            set { DefaultVignetteLocation = new StorageLocation(value); }
        }

        /// <summary>
        /// Gets or sets the path to a default, generic data file associated with the program for XML Serialization. Reserved for future use.
        /// </summary>
        /// <remarks>[System.Obsolete("This is only for use by XmlSerializer. Use DefaultReservedDataLocation instead.")]</remarks>
        [System.Xml.Serialization.XmlIgnore]
        [System.Xml.Serialization.XmlElement("LTOFlashReservedDataPath")]
        public string XmlDefaultReservedDataPath
        {
            get { return DefaultReservedDataLocation.Path; }
            set { DefaultReservedDataLocation = new StorageLocation(value); }
        }

        #endregion // XML Properties

        /// <summary>
        /// Gets the location of the program ROM.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation RomImageLocation
        {
            get { return ProgramRom.RomPath; }
        }

        /// <summary>
        /// Gets the location of the program ROM's configuration data.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation RomConfigurationLocation
        {
            get { return ProgramRom.ConfigPath; }
        }

        /// <summary>
        /// Gets the ROM image's alternate locations.
        /// </summary>
        public IEnumerable<StorageLocation> AlternateRomImageLocations
        {
            get { return GetSupportFiles(ProgramFileKind.Rom); }
        }

        /// <summary>
        /// Gets the ROM image's alternate locations.
        /// </summary>
        public IEnumerable<StorageLocation> AlternateRomConfigurationLocations
        {
            get { return GetSupportFiles(ProgramFileKind.CfgFile); }
        }

        /// <summary>
        /// Gets the box image locations.
        /// </summary>
        public IEnumerable<StorageLocation> BoxImageLocations
        {
            get { return GetSupportFiles(ProgramFileKind.Box); }
        }

        /// <summary>
        /// Gets the overlay image locations.
        /// </summary>
        public IEnumerable<StorageLocation> OverlayImageLocations
        {
            get { return GetSupportFiles(ProgramFileKind.Overlay); }
        }

        /// <summary>
        /// Gets the manual cover image locations.
        /// </summary>
        public IEnumerable<StorageLocation> ManualCoverImageLocations
        {
            get { return GetSupportFiles(ProgramFileKind.ManualCover); }
        }

        /// <summary>
        /// Gets the label image locations.
        /// </summary>
        public IEnumerable<StorageLocation> LabelImageLocations
        {
            get { return GetSupportFiles(ProgramFileKind.Label); }
        }

        /// <summary>
        /// Gets the manual text file locations.
        /// </summary>
        public IEnumerable<StorageLocation> ManualLocations
        {
            get { return GetSupportFiles(ProgramFileKind.ManualText); }
        }

        /// <summary>
        /// Gets the save data locations.
        /// </summary>
        public IEnumerable<StorageLocation> SaveDataLocations
        {
            get { return GetSupportFiles(ProgramFileKind.SaveData); }
        }

        /// <summary>
        /// Gets the LUIGI ROM image locations.
        /// </summary>
        public IEnumerable<StorageLocation> LtoFlashDataLocations
        {
            get { return GetSupportFiles(ProgramFileKind.LuigiFile); }
        }

        /// <summary>
        /// Gets or sets the location of the default box image.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultBoxImageLocation
        {
            get { return BoxImageLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultBoxImageLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Box, value)); }
        }

        /// <summary>
        /// Gets or sets the location of the default overlay image.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultOverlayImageLocation
        {
            get { return OverlayImageLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultOverlayImageLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Overlay, value)); }
        }

        /// <summary>
        /// Gets or sets the location of the default manual cover image.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultManualCoverImageLocation
        {
            get { return ManualCoverImageLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultManualImageLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.ManualCover, value)); }
        }

        /// <summary>
        /// Gets or sets the location of the default label image.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultLabelImageLocation
        {
            get { return LabelImageLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultLabelImageLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Label, value)); }
        }

        /// <summary>
        /// Gets or sets the location of the default program instructions.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultManualTextLocation
        {
            get { return ManualLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultManualTextLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.ManualText, value), (p, v) => RaisePropertyChanged(DefaultManualTextPathPropertyName)); }
        }

        /// <summary>
        /// Gets or sets the location of the default save data.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultSaveDataLocation
        {
            get { return SaveDataLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultSaveDataLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.SaveData, value), (p, v) => RaisePropertyChanged(DefaultSaveDataPathPropertyName)); }
        }

        /// <summary>
        /// Gets or sets the location of the default LUIGI format version of the ROM.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultLtoFlashDataLocation
        {
            get { return LtoFlashDataLocations.DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultLtoFlashDataLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.LuigiFile, value), (p, v) => RaisePropertyChanged(DefaultLtoFlashDataPathPropertyName)); }
        }

        /// <summary>
        /// Gets or sets the location of the default vignette for the ROM.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultVignetteLocation
        {
            get { return _supportFiles[ProgramFileKind.Vignette].DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultLtoFlashDataLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Vignette, value), (p, v) => RaisePropertyChanged(DefaultLtoFlashDataPathPropertyName)); }
        }

        /// <summary>
        /// Gets or sets the location of a generic data file associated with the program. Reserved for future use.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public StorageLocation DefaultReservedDataLocation
        {
            get { return _supportFiles[ProgramFileKind.GenericSupportFile].DefaultIfEmpty(StorageLocation.InvalidLocation).First(); }
            set { UpdateProperty(DefaultReservedDataLocationPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.GenericSupportFile, value)); }
        }

        private IRom ProgramRom
        {
            get { return _programRom == null ? Model.Rom.InvalidRom : _programRom; }
        }

        #endregion // Properties

        /// <summary>
        /// Unconditionally adds a support file to the support files associated with a program.
        /// </summary>
        /// <param name="whichFile">Which support file to add.</param>
        /// <param name="location">The location of the support file being added.</param>
        public void AddSupportFile(ProgramFileKind whichFile, StorageLocation location)
        {
            var files = _supportFiles[whichFile];
            files.Add(location);
            UpdateXmlSupportFilesList(whichFile, files);
        }

        /// <summary>
        /// Retrieves the current validation state of the given support file.
        /// </summary>
        /// <param name="whichFile">Which support file to get a validation state for.</param>
        /// <returns>The validation state of the file.</returns>
        public ProgramSupportFileState GetSupportFileState(ProgramFileKind whichFile)
        {
            var state = ProgramSupportFileState.None;
            if (!_supportFileStates.TryGetValue(whichFile, out state))
            {
                state = ProgramSupportFileState.None;
            }
            return state;
        }

        /// <summary>
        /// Updates the validation state of the given support file.
        /// </summary>
        /// <param name="whichFile">Which file to validate.</param>
        /// <param name="crc">If non-zero, the CRC of the program ROM to compare against if validating the ROM file.</param>
        /// <param name="programDescription">The program description being validated (if applicable).</param>
        /// <param name="peripherals">The peripherals attached to the system, used for compatibility checks.</param>
        /// <param name="connectedPeripheralsHistory">The peripherals that have been attached to the system over time.</param>
        /// <param name="reportIfModified">If <c>true</c>, check if the file has been modified, not just whether it exists.</param>
        /// <returns>The updated state of the file.</returns>
        public ProgramSupportFileState ValidateSupportFile(ProgramFileKind whichFile, uint crc, IProgramDescription programDescription, IEnumerable<IPeripheral> peripherals, IEnumerable<IPeripheral> connectedPeripheralsHistory, bool reportIfModified)
        {
            var validationState = ProgramSupportFileState.None;
            switch (whichFile)
            {
                case ProgramFileKind.Rom:
                    var isValid = Rom.Validate();
                    if (RomImageLocation.IsValid)
                    {
                        var previousValidationState = ProgramSupportFileState.None;
                        _supportFileStates.TryGetValue(whichFile, out previousValidationState);
                        if (!RomImageLocation.Exists())
                        {
                            validationState = ProgramSupportFileState.Missing;
                            if (AlternateRomImageLocations.Any(p => p.Exists()))
                            {
                                validationState = ProgramSupportFileState.MissingWithAlternateFound;
                            }
                        }
                        else if (reportIfModified)
                        {
                            isValid = _programRom.IsValid;
                            if (crc != 0)
                            {
                                // In some cases, the CRC provided is actually Rom.Crc, so if they match, recompute the CRC.
                                var cfgCrc = 0u;
                                isValid = (Rom.Crc == crc) && (crc == GetRefreshedCrcForRom(RomImageLocation, RomConfigurationLocation, out cfgCrc) && (Rom.CfgCrc == cfgCrc));
                            }
                            validationState = isValid ? ProgramSupportFileState.PresentAndUnchanged : ProgramSupportFileState.PresentButModified;
                        }
                        switch (validationState)
                        {
                            case ProgramSupportFileState.PresentAndUnchanged:
                            case ProgramSupportFileState.None:
                                // Treat a ROM file's missing or modified state as higher priority to report than peripheral-related information.
                                // This bit of code is entirely LTO Flash!-specific in its assumptions. If there should ever be other
                                // peripheral-specific needs to address here, a larger architectural change may be necessary. While the
                                // language of the states here is neutral, the basis of this check is not.
                                var rom = programDescription == null ? Rom : programDescription.Rom;
                                var requiresPeripheral = rom.IsLtoFlashOnlyRom();
                                if (requiresPeripheral)
                                {
                                    var isUniversallyCompatible = rom.GetTargetDeviceUniqueId() == LuigiScrambleKeyBlock.AnyLTOFlashId;
                                    var matchesPeripheralInDeviceHistory = isUniversallyCompatible || ((connectedPeripheralsHistory != null) && (connectedPeripheralsHistory.FirstOrDefault(p => p.IsRomCompatible(programDescription)) != null));
                                    var canRunOnConnected = isUniversallyCompatible || ((peripherals != null) && (peripherals.FirstOrDefault(p => p.IsRomCompatible(programDescription)) != null));

                                    if (peripherals == null)
                                    {
                                        // If previous validation state was due to peripheral, retain it, since we don't
                                        // have any peripherals to check against.
                                        if (validationState != previousValidationState)
                                        {
                                            switch (previousValidationState)
                                            {
                                                case ProgramSupportFileState.RequiredPeripheralAvailable:
                                                case ProgramSupportFileState.RequiredPeripheralIncompatible:
                                                case ProgramSupportFileState.RequiredPeripheralNotAttached:
                                                case ProgramSupportFileState.RequiredPeripheralUnknown:
                                                    validationState = previousValidationState;
                                                    break;
                                                case ProgramSupportFileState.None:
                                                    validationState = matchesPeripheralInDeviceHistory ? ProgramSupportFileState.RequiredPeripheralNotAttached : ProgramSupportFileState.RequiredPeripheralUnknown;
                                                    break;
                                                default:
                                                    // TODO: Decide if the following is a bug:
                                                    // 0: Presume a scrambled (unique) LUIGI, no device or device history provided (null)
                                                    // 1. Initially ROM's file is missing, but its alternate is found - this caches the 'MissingButAlternateFound' state
                                                    // 2. Update ROM to use alternate path as primary path
                                                    // 3. Re-validate
                                                    // At this point, the "Present and unmodified" state is used -- despite the ROM requiring
                                                    // a specific device.
                                                    // Why is this considered correct at this time?
                                                    // a) When no devices or device history are give (nulls), it's impossible to know. So just use the simple state of he file.
                                                    // b) It MAY be a bug that, if we pass in EMPTY peripheral / history lists that we should consider something different... but
                                                    //    then again, should we report 'unknown peripheral' at that time? Or would reporting 'not attached' be better?
                                                    //    What about 'universally' scrambled ROMs? Using 'not attached' may be more accurate then as well...
                                                    // The case of scrambled ROMs likely needs more careful consideration generally...
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (peripherals.Any())
                                        {
                                            if (canRunOnConnected)
                                            {
                                                validationState = ProgramSupportFileState.RequiredPeripheralAvailable;
                                            }
                                            else
                                            {
                                                validationState = ProgramSupportFileState.RequiredPeripheralIncompatible;
                                            }
                                        }
                                        else
                                        {
                                            validationState = matchesPeripheralInDeviceHistory ? ProgramSupportFileState.RequiredPeripheralNotAttached : ProgramSupportFileState.RequiredPeripheralUnknown;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    // TODO: Implement remaining validation code.
                    break;
            }
            _supportFileStates[whichFile] = validationState;
            return validationState;
        }

        /// <summary>
        /// Makes a deep-where-it-makes-sense copy of this instance.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public ProgramSupportFiles Copy()
        {
            var programSupportFiles = (ProgramSupportFiles)this.MemberwiseClone(); // shallow copy

            // Need to deep copy this dictionary.
            var supportFiles = new Dictionary<ProgramFileKind, List<StorageLocation>>();
            foreach (var entry in _supportFiles)
            {
                supportFiles[entry.Key] = new List<StorageLocation>(entry.Value);
            }
            programSupportFiles._supportFiles = supportFiles;
            foreach (var entry in supportFiles)
            {
                var xmlSupportFiles = GetXmlSupportFilesList(entry.Key);
                if (xmlSupportFiles != null)
                {
                    programSupportFiles.UpdateXmlSupportFilesList(entry.Key, entry.Value);
                }
            }

            // This copy is OK -- dictionary only has POD in it.
            programSupportFiles._supportFileStates = new Dictionary<ProgramFileKind, ProgramSupportFileState>(_supportFileStates);
            return programSupportFiles;
        }

        private static uint GetRefreshedCrcForRom(StorageLocation romLocation, StorageLocation cfgLocation, out uint cfgCrc)
        {
            var refreshedCrc = 0u;
            cfgCrc = 0u;
            try
            {
                refreshedCrc = Core.Model.Rom.GetRefreshedCrcs(romLocation, cfgLocation, out cfgCrc);
            }
            catch (System.Exception)
            {
            }
            return refreshedCrc;
        }

        private StorageLocation SetDefaultSupportFilePath(ProgramFileKind whichFile, StorageLocation location)
        {
            var previousValue = StorageLocation.InvalidLocation;
            var files = _supportFiles[whichFile];
            bool anyFiles = files.Any();
            if (anyFiles)
            {
                previousValue = files[0];
            }
            if (!location.IsValid)
            {
                if (anyFiles)
                {
                    // When the default support file path is set to null, it is removed.
                    // TODO: Should we be setting the path to <null>? The way this works, if
                    // we have more entries, then the second in the list will become the new
                    // default, which is not intuitive.
                    files.RemoveAt(0);
                }
            }
            else
            {
                var existingIndex = files.IndexOf(location);
                if (existingIndex >= 0)
                {
                    files.RemoveAt(existingIndex);
                    files.Insert(0, location);
                }
                else if (anyFiles)
                {
                    files[0] = location;
                }
                else
                {
                    files.Insert(0, location);
                }
            }
            UpdateXmlSupportFilesList(whichFile, files);
            return previousValue;
        }

        private void UpdateProgramRom(ProgramFileKind whichFile, string data)
        {
            if (_programRom == null)
            {
                _programRom = new XmlRom();
            }
            var xmlRom = _programRom as XmlRom;
            switch (whichFile)
            {
                case ProgramFileKind.Rom:
                    System.Diagnostics.Debug.Assert(!xmlRom.IsValid, "When is this called on a valid ROM?");
                    xmlRom.UpdateRomPath(new StorageLocation(data)); // ugh... why is this here?
                    ////if (!_supportFiles[ProgramFileKind.Rom].Contains(RomImagePath, System.StringComparer.OrdinalIgnoreCase))
                    ////{
                    ////    _supportFiles[ProgramFileKind.Rom].Add(RomImagePath);
                    ////}
                    break;
                case ProgramFileKind.CfgFile:
                    if (!string.IsNullOrEmpty(data))
                    {
                        xmlRom.UpdateConfigPath(new StorageLocation(data));
                        ////if (!_supportFiles[ProgramFileKind.CfgFile].Contains(RomConfigurationFilePath, System.StringComparer.OrdinalIgnoreCase))
                        ////{
                        ////    _supportFiles[ProgramFileKind.CfgFile].Add(RomConfigurationFilePath);
                        ////}
                    }
                    break;
            }
        }

        private List<string> GetXmlSupportFilesList(ProgramFileKind whichCollection)
        {
            List<string> supportFilesFromXml = null;
            switch (whichCollection)
            {
                case ProgramFileKind.Rom:
                    supportFilesFromXml = _alternateRomImagePaths;
                    break;
                case ProgramFileKind.CfgFile:
                    supportFilesFromXml = _alternateConfigurationFilePaths;
                    break;
                default:
                    break;
            }
            return supportFilesFromXml;
        }

        private void UpdateXmlSupportFilesList(ProgramFileKind whichCollection, IEnumerable<StorageLocation> supportFiles)
        {
            switch (whichCollection)
            {
                case ProgramFileKind.Rom:
                    _alternateRomImagePaths = supportFiles.Select(l => l.Path).ToList();
                    break;
                case ProgramFileKind.CfgFile:
                    _alternateConfigurationFilePaths = supportFiles.Select(l => l.Path).ToList();
                    break;
                default:
                    break;
            }
        }

        private IEnumerable<StorageLocation> GetSupportFiles(ProgramFileKind whichCollection)
        {
            var supportFiles = _supportFiles[whichCollection];
            var supportFilesFromXml = GetXmlSupportFilesList(whichCollection);
            if (supportFilesFromXml != null)
            {
                var supportLocationsFromXml = supportFilesFromXml.Where(f => !string.IsNullOrEmpty(f)).Select(f => new StorageLocation(f));
                var supportLocationsToAdd = supportLocationsFromXml.Except(supportFiles).ToList();
                supportFiles.AddRange(supportLocationsToAdd);
                UpdateXmlSupportFilesList(whichCollection, supportFiles);
            }
            return supportFiles;
        }
    }
}
