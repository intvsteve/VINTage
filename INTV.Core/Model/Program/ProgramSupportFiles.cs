// <copyright file="ProgramSupportFiles.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
    public class ProgramSupportFiles : INTV.Core.ComponentModel.ModelBase
    {
        #region Property Names

        /// <summary>
        /// Name of the RomImagePath property.
        /// </summary>
        public const string RomImagePathPropertyName = "RomImagePath";

        /// <summary>
        /// Name of the RomConfigurationFilePath property.
        /// </summary>
        public const string RomConfigurationFilePathPropertyName = "RomConfigurationFilePath";

        /// <summary>
        /// Name of the DefaultBoxImagePath property.
        /// </summary>
        public const string DefaultBoxImagePathPropertyName = "DefaultBoxImagePath";

        /// <summary>
        /// Name of the DefaultOverlayImagePath property.
        /// </summary>
        public const string DefaultOverlayImagePathPropertyName = "DefaultOverlayImagePath";

        /// <summary>
        /// Name of the DefaultManualImagePath property.
        /// </summary>
        public const string DefaultManualImagePathPropertyName = "DefaultManualImagePath";

        /// <summary>
        /// Name of the DefaultLabelImagePath property.
        /// </summary>
        public const string DefaultLabelImagePathPropertyName = "DefaultLabelImagePath";

        /// <summary>
        /// Name of the DefaultManualTextPath property.
        /// </summary>
        public const string DefaultManualTextPathPropertyName = "DefaultManualTextPath";

        /// <summary>
        /// Name of the DefaultSaveDataPath property.
        /// </summary>
        public const string DefaultSaveDataPathPropertyName = "DefaultSaveDataPath";

        /// <summary>
        /// Name of the DefaultLtoFlashDataPath property.
        /// </summary>
        public const string DefaultLtoFlashDataPathPropertyName = "DefaultLtoFlashDataPath";

        /// <summary>
        /// Name of the DefaultVignettePath property.
        /// </summary>
        public const string DefaultVignettePathPropertyName = "DefaultVignettePath";

        /// <summary>
        /// Name of the DefaultReservedDataPath property.
        /// </summary>
        public const string DefaultReservedDataPathPropertyName = "DefaultReservedDataPath";

        #endregion // Property Names

        private Dictionary<ProgramFileKind, List<string>> _supportFiles;
        private Dictionary<ProgramFileKind, ProgramSupportFileState> _supportFileStates;
        private IRom _programRom;

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
            _supportFiles = new Dictionary<ProgramFileKind, List<string>>();
            _supportFileStates = new Dictionary<ProgramFileKind, ProgramSupportFileState>();
            _supportFiles[ProgramFileKind.Rom] = new List<string>();
            _supportFiles[ProgramFileKind.CfgFile] = new List<string>();
            _supportFiles[ProgramFileKind.Box] = new List<string>();
            _supportFiles[ProgramFileKind.Label] = new List<string>();
            _supportFiles[ProgramFileKind.Overlay] = new List<string>();
            _supportFiles[ProgramFileKind.ManualCover] = new List<string>();
            _supportFiles[ProgramFileKind.ManualText] = new List<string>();
            _supportFiles[ProgramFileKind.SaveData] = new List<string>();
            _supportFiles[ProgramFileKind.LuigiFile] = new List<string>();
            _supportFiles[ProgramFileKind.Vignette] = new List<string>();
            _supportFiles[ProgramFileKind.GenericSupportFile] = new List<string>();
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

        /// <summary>
        /// Gets or sets the path to the program ROM.
        /// </summary>
        /// <remarks>The setter is public to support XmlSerializer.</remarks>
        public string RomImagePath
        {
            get { return (_programRom == null) ? null : _programRom.RomPath; }
            set { UpdateProgramRom(ProgramFileKind.Rom, value); } // If this is only present for XML... why go through the complex code?
        }

        /// <summary>
        /// Gets or sets the path to the ROM's configuration path.
        /// </summary>
        /// <remarks>The setter is public to support XmlSerializer.</remarks>
        public string RomConfigurationFilePath
        {
            get { return (_programRom == null) ? null : _programRom.ConfigPath; }
            set { UpdateProgramRom(ProgramFileKind.CfgFile, value); } // If this is only present for XML... why go through the complex code?
        }

        /// <summary>
        /// Gets or sets the ROM file's alternate paths.
        /// </summary>
        /// <remarks>The setter is present and public to support XmlSerializer.</remarks>
        public List<string> AlternateRomImagePaths
        {
            get { return _supportFiles[ProgramFileKind.Rom]; }
            set { _supportFiles[ProgramFileKind.Rom].AddRange(value); }
        }

        /// <summary>
        /// Gets or sets the ROM's alternate configuration file paths.
        /// </summary>
        /// <remarks>The setter is present and public to support XmlSerializer.</remarks>
        public List<string> AlternateRomConfigurationFilePaths
        {
            get { return _supportFiles[ProgramFileKind.CfgFile]; }
            set { _supportFiles[ProgramFileKind.CfgFile].AddRange(value); }
        }

        /// <summary>
        /// Gets the box image paths.
        /// </summary>
        public IEnumerable<string> BoxImagePaths
        {
            get { return _supportFiles[ProgramFileKind.Box]; }
        }

        /// <summary>
        /// Gets the overlay image paths.
        /// </summary>
        public IEnumerable<string> OverlayImagePaths
        {
            get { return _supportFiles[ProgramFileKind.Overlay]; }
        }

        /// <summary>
        /// Gets the manual over image paths.
        /// </summary>
        public IEnumerable<string> ManualCoverImagePaths
        {
            get { return _supportFiles[ProgramFileKind.ManualCover]; }
        }

        /// <summary>
        /// Gets the label image paths.
        /// </summary>
        public IEnumerable<string> LabelImagePaths
        {
            get { return _supportFiles[ProgramFileKind.Label]; }
        }

        /// <summary>
        /// Gets the manual text file paths.
        /// </summary>
        public IEnumerable<string> ManualPaths
        {
            get { return _supportFiles[ProgramFileKind.ManualText]; }
        }

        /// <summary>
        /// Gets the save data paths.
        /// </summary>
        public IEnumerable<string> SaveDataPaths
        {
            get { return _supportFiles[ProgramFileKind.SaveData]; }
        }

        /// <summary>
        /// Gets or sets the path to the default box image.
        /// </summary>
        [System.Xml.Serialization.XmlElement("BoxImagePath")]
        public string DefaultBoxImagePath
        {
            get { return _supportFiles[ProgramFileKind.Box].FirstOrDefault(); }
            set { UpdateProperty(DefaultBoxImagePathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Box, value)); }
        }

        /// <summary>
        /// Gets or sets the path to the default overlay image.
        /// </summary>
        [System.Xml.Serialization.XmlElement("OverlayImagePath")]
        public string DefaultOverlayImagePath
        {
            get { return _supportFiles[ProgramFileKind.Overlay].FirstOrDefault(); }
            set { UpdateProperty(DefaultOverlayImagePathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Overlay, value)); }
        }

        /// <summary>
        /// Gets or sets the path to the default manual cover image.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ManualImagePath")]
        public string DefaultManualImagePath
        {
            get { return _supportFiles[ProgramFileKind.ManualCover].FirstOrDefault(); }
            set { UpdateProperty(DefaultManualImagePathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.ManualCover, value)); }
        }

        /// <summary>
        /// Gets or sets the path to the default label image.
        /// </summary>
        [System.Xml.Serialization.XmlElement("LabelImagePath")]
        public string DefaultLabelImagePath
        {
            get { return _supportFiles[ProgramFileKind.Label].FirstOrDefault(); }
            set { UpdateProperty(DefaultLabelImagePathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Label, value)); }
        }

        /// <summary>
        /// Gets or sets the path to the default program instructions.
        /// </summary>
        [System.Xml.Serialization.XmlElement("ManualPath")]
        public string DefaultManualTextPath
        {
            get { return _supportFiles[ProgramFileKind.ManualText].FirstOrDefault(); }
            set { UpdateProperty(DefaultManualTextPathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.ManualText, value)); }
        }

        /// <summary>
        /// Gets or sets the path to the default save data.
        /// </summary>
        [System.Xml.Serialization.XmlElement("SaveDataPath")]
        public string DefaultSaveDataPath
        {
            get { return _supportFiles[ProgramFileKind.SaveData].FirstOrDefault(); }
            set { UpdateProperty(DefaultSaveDataPathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.SaveData, value)); }
        }

        /// <summary>
        /// Gets or sets the path to the default LUIGI format path for the ROM.
        /// </summary>
        [System.Xml.Serialization.XmlElement("LTOFlashROMPath")]
        public string DefaultLtoFlashDataPath
        {
            get { return _supportFiles[ProgramFileKind.LuigiFile].FirstOrDefault(); }
            set { UpdateProperty(DefaultLtoFlashDataPathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.LuigiFile, value)); }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string DefaultVignettePath
        {
            get { return _supportFiles[ProgramFileKind.Vignette].FirstOrDefault(); }
            set { UpdateProperty(DefaultLtoFlashDataPathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.Vignette, value)); }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string DefaultReservedDataPath
        {
            get { return _supportFiles[ProgramFileKind.GenericSupportFile].FirstOrDefault(); }
            set { UpdateProperty(DefaultReservedDataPathPropertyName, value, SetDefaultSupportFilePath(ProgramFileKind.GenericSupportFile, value)); }
        }

        #endregion // Properties

        /// <summary>
        /// Unconditionally adds a support file to the support files associated with a program.
        /// </summary>
        /// <param name="whichFile">Which support file to add.</param>
        /// <param name="filePath">Absolute path of the support file being added.</param>
        public void AddSupportFile(ProgramFileKind whichFile, string filePath)
        {
            _supportFiles[whichFile].Add(filePath);
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
                    if (!string.IsNullOrEmpty(RomImagePath))
                    {
                        var previousValidationState = ProgramSupportFileState.None;
                        _supportFileStates.TryGetValue(whichFile, out previousValidationState);
                        if (!StreamUtilities.FileExists(RomImagePath))
                        {
                            validationState = ProgramSupportFileState.Missing;
                            if ((AlternateRomImagePaths != null) && AlternateRomImagePaths.Any(p => StreamUtilities.FileExists(p)))
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
                                isValid = (Rom.Crc == crc) && (crc == GetRefreshedCrcForRom(RomImagePath, RomConfigurationFilePath, out cfgCrc) && (Rom.CfgCrc == cfgCrc));
                            }
                            validationState = isValid ? ProgramSupportFileState.PresentAndUnchanged : ProgramSupportFileState.PresentButModified;
                        }
                        switch (validationState)
                        {
                            case ProgramSupportFileState.PresentAndUnchanged:
                            case ProgramSupportFileState.None:
                                // Treat an ROM file's missing or modified state as higher priority to report than peripheral-related information.
                                // This bit of code is entirely LTO Flash!-specific in its assumptions. If there should ever be other
                                // peripheral-specific needs to address here, a larger architectural change may be necessary. While the
                                // language of the states here is neutral, the basis of this check is not.
                                var requiresPeripheral = programDescription.Rom.IsLtoFlashOnlyRom();
                                if (requiresPeripheral)
                                {
                                    var isUniversallyCompatible = programDescription.Rom.GetTargetDeviceUniqueId() == LuigiScrambleKeyBlock.AnyLTOFlashId;
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
            var supportFiles = new Dictionary<ProgramFileKind, List<string>>();
            foreach (var entry in _supportFiles)
            {
                supportFiles[entry.Key] = new List<string>(entry.Value);
            }
            programSupportFiles._supportFiles = supportFiles;

            // This copy is OK -- dictionary only has POD in it.
            programSupportFiles._supportFileStates = new Dictionary<ProgramFileKind, ProgramSupportFileState>(_supportFileStates);
            return programSupportFiles;
        }

        private static uint GetRefreshedCrcForRom(string romPath, string cfgPath, out uint cfgCrc)
        {
            var refreshedCrc = 0u;
            cfgCrc = 0u;
            try
            {
                refreshedCrc = Core.Model.Rom.GetRefreshedCrcs(romPath, cfgPath, out cfgCrc);
            }
            catch (System.Exception)
            {
            }
            return refreshedCrc;
        }

        private string SetDefaultSupportFilePath(ProgramFileKind whichFile, string filePath)
        {
            string previousValue = null;
            var files = _supportFiles[whichFile];
            bool anyFiles = files.Any();
            if (anyFiles)
            {
                previousValue = files[0];
            }
            if (string.IsNullOrEmpty(filePath))
            {
                if (anyFiles)
                {
                    // When a support file path is removed, do so by setting the path to <null>.
                    files.RemoveAt(0);
                }
            }
            else
            {
                var existingMatch = files.FirstOrDefault(f => System.StringComparer.OrdinalIgnoreCase.Compare(f, filePath) == 0);
                if (existingMatch != null)
                {
                    files.Remove(existingMatch);
                    files.Insert(0, filePath);
                }
                else if (anyFiles)
                {
                    files[0] = filePath;
                }
                else
                {
                    files.Insert(0, filePath);
                }
            }
            return previousValue;
        }

        private void UpdateProgramRom(ProgramFileKind whichFile, object data)
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
                    xmlRom.UpdateRomPath(data as string); // ugh... why is this here?
                    ////if (!_supportFiles[ProgramFileKind.Rom].Contains(RomImagePath, System.StringComparer.OrdinalIgnoreCase))
                    ////{
                    ////    _supportFiles[ProgramFileKind.Rom].Add(RomImagePath);
                    ////}
                    break;
                case ProgramFileKind.CfgFile:
                    if (!string.IsNullOrEmpty(data as string))
                    {
                        xmlRom.UpdateConfigPath(data as string);
                        ////if (!_supportFiles[ProgramFileKind.CfgFile].Contains(RomConfigurationFilePath, System.StringComparer.OrdinalIgnoreCase))
                        ////{
                        ////    _supportFiles[ProgramFileKind.CfgFile].Add(RomConfigurationFilePath);
                        ////}
                    }
                    break;
            }
        }
    }
}
