// <copyright file="Program.cs" company="INTV Funhouse">
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

////#define REPORT_PERFORMANCE
////#define PERSIST_RESERVED_FORKS

using System;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Implements the IFile interface for an Intellivision program stored on a host PC as a ROM file.
    /// </summary>
    public sealed class Program : FileNode, ILfsFileInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        /// <remarks>Exists for use by XmlSerializer.</remarks>
        private Program()
            : base(default(FileSystem))
        {
        }

        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        /// <param name="file">The directory entry describing the program.</param>
        /// <remarks>This constructor is typically used to create a model for a user interface to operate on for
        /// the purpose of viewing the contents of a Locutus device.</remarks>
        public Program(LfsFileInfo file)
            : base(file)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        /// <param name="description">The program which is represented as a file.</param>
        /// <param name="fileSystem">The file system to which the program entry belongs.</param>
        public Program(ProgramDescription description, FileSystem fileSystem)
            : base(fileSystem)
        {
            var newDescription = description.Copy();
            fileSystem.Files.Add(this);
            var updatedCrc = OnProgramDescriptionChanged(_description, newDescription, false);
            _description = newDescription;
            if (!updatedCrc)
            {
                UpdateCrc();
            }
            _description.Files.DefaultLtoFlashDataPath = _description.Rom.GetLtoFlashFilePath();

            // Force updates for support files.
            var forceSupportFilesUpdates = new[]
                {
                    ProgramSupportFiles.DefaultLtoFlashDataPathPropertyName,
                    ProgramSupportFiles.RomConfigurationFilePathPropertyName,
                    ProgramSupportFiles.DefaultManualTextPathPropertyName,
                    ProgramSupportFiles.DefaultSaveDataPathPropertyName
                };
            foreach (var supportFile in forceSupportFilesUpdates)
            {
                HandleProgramSupportFilesPropertyChanged(_description.Files, new System.ComponentModel.PropertyChangedEventArgs(supportFile));
            }
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the program this file represents.
        /// </summary>
        public ProgramDescription Description
        {
            get
            {
                return _description;
            }

            set
            {
                // only called during deserialization
                var previousDescription = _description;
                AssignAndUpdateProperty("Description", value, ref _description, (n, p) => OnProgramDescriptionChanged(previousDescription, p, Properties.Settings.Default.ValidateMenuAtStartup));
            }
        }
        private ProgramDescription _description;

        #region IFile Properties

        /// <inheritdoc />
        public override FileType FileType
        {
            get { return FileType.File; }
        }

        /// <inheritdoc />
        public override uint Crc32
        {
            get { return (Description == null) ? _crc32 : Description.Crc; }
            set { _crc32 = value; }
        }
        private uint _crc32;

        #endregion // IFileProperties

        #region ILfsFileInfo Properties

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public override byte GlobalDirectoryNumber
        {
            get { return GlobalDirectoryTable.InvalidDirectoryNumber; }
            set { }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public override byte Reserved
        {
            get { return 0xFF; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(RomForkXmlNodeName)]
        public override Fork Rom
        {
            get { return base.Rom; }
            set { base.Rom = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ManualForkXmlNodeName)]
        public override Fork Manual
        {
            get { return base.Manual; }
            set { base.Manual = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(SaveDataForkXmlNodeName)]
        public override Fork JlpFlash
        {
            get { return base.JlpFlash; }
            set { base.JlpFlash = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(VignetteForkXmlNodeName)]
        public override Fork Vignette
        {
            get { return base.Vignette; }
            set { base.Vignette = value; }
        }

#if PERSIST_RESERVED_FORKS

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ReservedFork4XmlNodeName)]
        public override Fork ReservedFork4
        {
            get { return base.ReservedFork4; }
            set { base.ReservedFork4 = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ReservedFork5XmlNodeName)]
        public override Fork ReservedFork5
        {
            get { return base.ReservedFork5; }
            set { base.ReservedFork5 = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlElement(ReservedFork6XmlNodeName)]
        public override Fork ReservedFork6
        {
            get { return base.ReservedFork6; }
            set { base.ReservedFork6 = value; }
        }

#endif // PERSIST_RESERVED_FORKS

        #region IGlobalFileSystemEntry Properties

        /// <inheritdoc />
        public override int EntryUpdateSize
        {
            get { return LfsFileInfo.FlatSizeInBytes; }
        }

        /// <inheritdoc />
        /// <remarks>Incorporates data from containing folder and forks.</remarks>
        public override uint Uid
        {
            get
            {
                var uid = Crc32;
                uid ^= (uint)LongName.GetHashCode();
                if (!string.IsNullOrEmpty(ShortName))
                {
                    uid ^= (uint)ShortName.GetHashCode();
                }
                uid ^= (uint)Color.GetHashCode();

                var positionInParent = Parent.IndexOfChild(this);
                uint uidFodder = GlobalFileNumber | (uint)(positionInParent << 24);
                uid ^= uidFodder;

                foreach (var fork in Forks)
                {
                    if ((fork != null) && (fork.Uid != Fork.InvalidUid))
                    {
                        uid ^= fork.Uid;
                    }
                }

                return uid;
            }
        }

        #endregion // IGlobalFileSystemEntry Properties

        #endregion // ILfsFileInfo Properties

        #endregion // Properties

        #region IGlobalFileSystemEntry Methods

        /// <inheritdoc />
        public override IGlobalFileSystemEntry Clone(FileSystem fileSystem)
        {
            var program = (Program)this.MemberwiseClone();
            program.RemoveAllEventHandlers();
            program.FileSystem = fileSystem;
            program.Forks = new Fork[(int)ForkKind.NumberOfForkKinds];
            program.SetForks(this.ForkNumbers);
            program.Parent = null;
            return program;
        }

        #endregion // IGlobalFileSystemEntry Methods

        /// <inheritdoc />
        public override void UpdateCrc()
        {
        }

        #region object Overrides

        /// <inheritdoc />
        public override string ToString()
        {
            return "Program {Long: " + LongName + ", Short: " + ShortName + "}";
        }

        #endregion // object Overrides

        /// <inheritdoc />
        internal override void LoadComplete(FileSystem fileSystem, bool updateRomList)
        {
            base.LoadComplete(fileSystem, updateRomList);
            if (Description == null)
            {
                if (Rom != null)
                {
                    if (Crc32 == 0)
                    {
                        var filePath = Rom.FilePath;
                        INTV.Core.Model.LuigiFileHeader luigiHeader = null;
                        using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                        {
                            try
                            {
                                luigiHeader = INTV.Core.Model.LuigiFileHeader.Inflate(fileStream);
                            }
                            catch (INTV.Core.UnexpectedFileTypeException)
                            {
                            }
                        }
                        if (luigiHeader != null)
                        {
                            string cfgFile = null;
                            string romFile = filePath;
                            uint crc = 0u;
                            if (luigiHeader.Version > 0)
                            {
                                crc = luigiHeader.OriginalRomCrc32;
                            }
                            else
                            {
                                var jzIntvConfiguration = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<INTV.JzIntv.Model.Configuration>();
                                var luigiToBinPath = jzIntvConfiguration.GetProgramPath(JzIntv.Model.ProgramFile.Luigi2Bin);
                                var luigiToBinResult = RunExternalProgram.Call(luigiToBinPath, "\"" + filePath + "\"", System.IO.Path.GetDirectoryName(filePath));
                                if (luigiToBinResult == 0)
                                {
                                    romFile = System.IO.Path.ChangeExtension(filePath, RomFormat.Bin.FileExtension());
                                    cfgFile = System.IO.Path.ChangeExtension(filePath, ProgramFileKind.CfgFile.FileExtension());
                                    if (!System.IO.File.Exists(cfgFile))
                                    {
                                        cfgFile = null;
                                    }
                                }
                            }

                            var rom = INTV.Core.Model.Rom.Create(romFile, cfgFile);
                            var programInfo = rom.GetProgramInformation();
                            var programDescription = new ProgramDescription(rom.Crc, rom, programInfo);
                            Description = programDescription;
                        }
                    }
                }
            }
        }

        private void UpdateFork(ForkKind kind, string filePath)
        {
            Fork fork = null;
            switch (kind)
            {
                case ForkKind.Program:
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        fork = FileSystem.Forks.AddFork(Description.GetRom());
                        if (!System.IO.File.Exists(fork.FilePath))
                        {
                            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomToLuigiFailed_OutputFileNotFound_Error_Format, Description.Rom.RomPath, System.IO.Path.GetFileNameWithoutExtension(filePath));
                            throw new LuigiFileGenerationException(message, Resources.Strings.RomToLuigiFailed_OutputFileNotFound_Error_Description_Format);
                        }
                    }
                    break;
                default:
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            fork = FileSystem.Forks.AddFork(filePath);
                        }
                    }
                    break;
            }
            SetFork(kind, fork);
        }

        private bool OnProgramDescriptionChanged(ProgramDescription previousDescription, ProgramDescription newDescription, bool validate)
        {
            if (previousDescription != null)
            {
                previousDescription.Files.PropertyChanged -= HandleProgramSupportFilesPropertyChanged;
            }

            // Do this work in LoadComplete.
            bool updatedCrc = false;

            // POSSIBLE BUG? We don't check for changes to .cfg file CRC here.
            if (Crc32 != 0)
            {
                if (Crc32 != newDescription.Crc)
                {
                    throw new InvalidOperationException("Menu item and its program description are out of sync!");
                }
            }
            bool updatedLongName = false;
            if (!string.IsNullOrEmpty(newDescription.Name))
            {
                bool replaceName = string.IsNullOrWhiteSpace(LongName);
                if (replaceName)
                {
                    if (newDescription.Name.Length > FileSystemConstants.MaxLongNameLength)
                    {
                        LongName = newDescription.Name.Substring(0, FileSystemConstants.MaxLongNameLength).Trim();
                    }
                    else
                    {
                        LongName = newDescription.Name.Trim();
                    }
                    updatedLongName = true;
                    updatedCrc = true;
                }
            }
            if (updatedLongName || !string.IsNullOrEmpty(newDescription.ShortName))
            {
                bool replaceName = string.IsNullOrWhiteSpace(ShortName);
                if (replaceName && !string.IsNullOrEmpty(newDescription.ShortName))
                {
                    ShortName = newDescription.ShortName.Trim();
                    if (ShortName.Length > FileSystemConstants.MaxShortNameLength)
                    {
                        ShortName = ShortName.Substring(0, FileSystemConstants.MaxShortNameLength).Trim();
                    }
                }
                else if (replaceName && (LongName.Length > FileSystemConstants.MaxShortNameLength))
                {
                    ShortName = LongName.Substring(0, FileSystemConstants.MaxShortNameLength).Trim();
                }
                updatedCrc = true;
            }
            if (updatedCrc)
            {
                UpdateCrc();
            }
            newDescription.Files.ValidateSupportFile(ProgramFileKind.Rom, Crc32, newDescription, null, Configuration.Instance.ConnectedPeripheralsHistory, validate);
            newDescription.Files.PropertyChanged += HandleProgramSupportFilesPropertyChanged;
            return updatedCrc;
        }

        private void HandleProgramSupportFilesPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var kind = ForkKind.None;
            string filePath = null;
            switch (e.PropertyName)
            {
                case ProgramSupportFiles.DefaultLtoFlashDataPathPropertyName:
                case ProgramSupportFiles.RomConfigurationFilePathPropertyName:
                    kind = ForkKind.Program;
                    filePath = Description.Files.DefaultLtoFlashDataPath;
                    break;
                case ProgramSupportFiles.DefaultManualTextPathPropertyName:
                    kind = ForkKind.Manual;
                    filePath = Description.Files.DefaultManualTextPath;
                    break;
                case ProgramSupportFiles.DefaultSaveDataPathPropertyName:
                    kind = ForkKind.JlpFlash;
                    filePath = Description.Files.DefaultSaveDataPath;
                    break;
            }
            UpdateFork(kind, filePath);
        }
    }
}
