// <copyright file="ProgramCollection.cs" company="INTV Funhouse">
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Defines a collection of ProgramDescription objects.
    /// </summary>
    [System.Xml.Serialization.XmlRoot("ProgramDescriptions")]
    [System.ComponentModel.Composition.Export(typeof(ProgramCollection))]
    public class ProgramCollection : ObservableCollection<ProgramDescription>
    {
        /// <summary>
        /// Property name for use by property change handlers.
        /// </summary>
        public const string CanEditElementsPropertyName = "CanEditElements";

        /// <summary>
        /// Used for various silly things like making the XAML designer not crash.
        /// </summary>
        public static readonly ProgramCollection EmptyDummyList = new ProgramCollection();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ProgramCollection type.
        /// </summary>
        /// <remarks>Must be public for MONO version to deserialize.</remarks>
        public ProgramCollection()
        {
            _canEditElements = true;
            SelectionIndexes = new ObservableCollection<int>();
            InvokeProgramHandlers = new List<Tuple<EventHandler<InvokeProgramEventArgs>, double>>();
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the instance of the ROMs (from MEF). A bit hacky.
        /// </summary>
        public static ProgramCollection Roms
        {
            get
            {
                var theRoms = (SingleInstanceApplication.Instance != null) ? SingleInstanceApplication.Instance.Roms : EmptyDummyList;
                return theRoms;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the elements in this <see cref="INTV.Shared.ViewModel.RomListViewModel"/> are editable.
        /// </summary>
        public bool CanEditElements
        {
            get
            {
                return _canEditElements;
            }

            set
            {
                _canEditElements = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(CanEditElementsPropertyName));
            }
        }
        private bool _canEditElements;

        public ObservableCollection<int> SelectionIndexes { get; private set; }

        private List<Tuple<EventHandler<InvokeProgramEventArgs>, double>> InvokeProgramHandlers { get; set; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// Raised immediately prior to the addition of ROMs to the program collection.
        /// </summary>
        public event EventHandler<AddRomsFromFilesBeginEventArgs> AddRomsFromFilesBegin;

        /// <summary>
        /// Raised At the end of adding ROMs to the program collection.
        /// </summary>
        public event EventHandler<AddRomsFromFilesEndEventArgs> AddRomsFromFilesEnd;

        /// <summary>
        /// Raised to notify interested parties that program features have been modified.
        /// </summary>
        public event EventHandler<ProgramFeaturesChangedEventArgs> ProgramFeaturesChanged;

        /// <summary>
        /// Raised to notify interested parties that the status (availability) of ROMs has changed.
        /// </summary>
        public event EventHandler<ProgramFeaturesChangedEventArgs> ProgramStatusChanged;

        /// <summary>
        /// Raised to notify interested parties that a save operation failed. If unhandled, the error is thrown.
        /// </summary>
        public event EventHandler<ProgramCollectionSaveFailedEventArgs> SaveFailed;

        #endregion // Events

        /// <summary>
        /// Load a ProgramCollection from disk.
        /// </summary>
        /// <param name="filePath">Absolute path to a serialized ProgramCollection.</param>
        /// <returns>The deserialized collection of programs.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity", Justification = "We intern the string prior to locking, so it is not weak.")]
        public static ProgramCollection Load(string filePath)
        {
            var path = string.Intern(filePath);
            lock (path)
            {
                ProgramCollection programs = null;
                if (File.Exists(path))
                {
                    using (var fileStream = FileUtilities.OpenFileStream(path))
                    {
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Program.ProgramCollection));
                        programs = serializer.Deserialize(fileStream) as ProgramCollection;
                    }
                }
                return programs;
            }
        }

        // TODO This should be removed from this class. Loading should happen through RomListViewModel.
        // There is, however, a usage of this when restoring a MenuLayout as well. Ideally, this would be
        // done by

        /// <summary>
        /// Initializes the contents of the singleton from a file.
        /// </summary>
        /// <param name="romListFile">The absolute path to the file to read from.</param>
        public static void InitializeFromFile(string romListFile)
        {
            if (File.Exists(romListFile))
            {
                try
                {
                    Roms.Initialize(romListFile);
                }
                catch (System.InvalidOperationException e)
                {
                    HandleLoadError(romListFile, e);
                }
                catch (System.Xml.XmlException e)
                {
                    HandleLoadError(romListFile, e);
                }
            }
        }

        private static void HandleLoadError(string romListFile, Exception exception)
        {
            var backupCopyPath = romListFile.GetUniqueBackupFilePath();
            var errorDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.RomList_LoadFailed_Title, Resources.Strings.RomList_LoadFailed_Message);
            errorDialog.ReportText = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomList_LoadFailed_Error_Format, backupCopyPath, exception);
            errorDialog.BeginInvokeDialog(
                Resources.Strings.RomList_LoadFailed_BackupButton_Text,
                new System.Action<bool?>((result) =>
                {
                    if (result.HasValue && result.Value)
                    {
                        try
                        {
                            File.Copy(romListFile, backupCopyPath);
                        }
                        catch (System.Exception x)
                        {
                            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RomList_LoadFailed_BackupFailed_Message_Format, x.Message);
                            OSMessageBox.Show(message, Resources.Strings.RomList_LoadFailed_Title, x, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                        }
                    }
                }));
        }

        private static bool DoesNewRomInArchiveMatchExistingRomInArchive(IRom existingRom, IRom newRom)
        {
            // For a newly discovered ROM that does not use default storage (i.e. one in a compressed archive), it
            // can be the case that an existing copy is already in the list. However, at the time of this code being
            // added, those locations are treated as ephemeral, and during ROM list XML parsing, we skip creating
            // compressed archive instances for each entry in the ROM list. The primary driver for that is going to
            // be performance and memory related, so the degenerate case of all ROMs being in archives won't require
            // opening a large number of compressed archive accessors simultaneously.
            //
            // The reason no match is found is the byproduct of treating ROMs in archives as being at an ephemeral
            // location. So even though a saved CRC exists, it is being ignored because the ROM does not appear to
            // be valid -- due to the lack of a complete construction of the ROM's image and cfg storage locations.
            //
            // In light of this, we will look for a matching entry that has matching paths for the main ROM as well
            // as its alternate location. If the paths match, and the alternate paths match, and the CRCs of the alternates
            // match, one of which is this newly discovered ROM, then it _is_ a match, and is already in the list.
            //
            // Better alternative: Fix up "resolving" ROMs from the XML load. An idea on what to do is to implement
            // a lazier kind of storage access for compressed archives that can be used to confirm values but not
            // keep around the access to the underlying storage the way the existing implementations do.
            //
            // Mechanisms for this may go something like this:
            // a) Add a "resolve storage access" mechanism to INTV.Core -- possibly via a callback registration
            //    or by adding the method to the existing storage access interface
            // b) Modify existing storage access registration system in INTV.Core to provide a "Resolve" system
            //    that accepts a hint (path)
            // c) Implement another level of indirection (see above) for compressed archives
            // d) Other ideas?
            var matches = false;
            var isNewRomInArchive = !newRom.RomPath.UsesDefaultStorage;
            if (isNewRomInArchive)
            {
                var xRomPath = existingRom.RomPath.Path.NormalizePathSeparators();
                var xCfgPath = existingRom.ConfigPath.Path.NormalizePathSeparators();
                var yRomPath = newRom.RomPath.Path.NormalizePathSeparators();
                var yCfgPath = newRom.ConfigPath.Path.NormalizePathSeparators();

                var pathsMatch = PathComparer.Instance.Compare(xRomPath, yRomPath) == 0
                    && PathComparer.Instance.Compare(xCfgPath, yCfgPath) == 0;

                if (pathsMatch)
                {
                    var existingRomPath = xRomPath.CreateStorageLocationFromPath();
                    var existingCfgPath = xCfgPath.CreateStorageLocationFromPath();
                    var romCrc = Crc32.OfFile(existingRomPath);
                    var cfgCrc = Crc32.OfFile(existingCfgPath);
                    matches = romCrc == newRom.Crc && cfgCrc == newRom.CfgCrc;
                }
            }
            return matches;
        }

        /// <summary>
        /// Identifies ROMs in a list of files and returns them.
        /// </summary>
        /// <param name="romFiles">A list of ROM files.</param>
        /// <param name="existingRoms">Existing, identified ROMs.</param>
        /// <param name="duplicateRoms">Receives ROMs that were not added because duplicates are already in the list.</param>
        /// <param name="acceptCancellation">A function that allows the operation to be cancelled.</param>
        /// <param name="progressFunc">A function to call to report progress of the search.</param>
        /// <param name="updateNumDiscovered">A function to call to indicate the number of valid ROMs discovered.</param>
        /// <param name="filter">Optional custom filter function. If filter is non-<c>null</c>, it must return <c>true</c> for a ROM to be added.</param>
        /// <returns>A list of program descriptions for the ROMs.</returns>
        public static IList<ProgramDescription> GatherRomsFromFileList(IEnumerable<IRom> romFiles, IEnumerable<ProgramDescription> existingRoms, IList<string> duplicateRoms, Func<bool> acceptCancellation, Action<string> progressFunc, Action<int> updateNumDiscovered, Func<IProgramInformation, bool> filter)
        {
            var addedItems = new List<ProgramDescription>();
#if DEBUGGING
            using (RomComparer strict = RomComparer.GetComparer(RomComparison.Strict),
                               strictCrcOnly = RomComparer.GetComparer(RomComparison.StrictRomCrcOnly),
                               canonical = RomComparer.GetComparer(RomComparison.CanonicalStrict),
                               canonicalCrcOnly = RomComparer.GetComparer(RomComparison.CanonicalRomCrcOnly))
#else
            using (RomComparer comparer = RomComparer.GetComparer(RomComparer.DefaultCompareMode))
#endif // DEBUGGING
            {
                foreach (var romFile in romFiles)
                {
                    if (acceptCancellation())
                    {
                        break;
                    }
                    if (progressFunc != null)
                    {
                        progressFunc(romFile.RomPath.Path);
                    }
                    bool alreadyAdded = (romFile.Crc != 0) && (addedItems.FirstOrDefault(p => p.Rom.IsEquivalentTo(romFile, comparer)) != null);
                    if ((romFile.Crc != 0) && !alreadyAdded)
                    {
                        var programInfo = romFile.GetProgramInformation();
                        if ((filter == null) || filter(programInfo))
                        {
                            var haveIt = existingRoms.Any(d => d.Rom.IsEquivalentTo(romFile, programInfo, comparer) || DoesNewRomInArchiveMatchExistingRomInArchive(d.Rom, romFile));
                            if (!haveIt)
                            {
                                IRom localRomCopy = null;
                                if (romFile.RomPath.Path.IsPathOnRemovableDevice())
                                {
                                    localRomCopy = romFile.CopyToLocalRomsDirectory();
                                }
                                var programDescription = new ProgramDescription(romFile.Crc, romFile, programInfo);
                                if (localRomCopy != null)
                                {
                                    programDescription.Files.AddSupportFile(ProgramFileKind.Rom, localRomCopy.RomPath);
                                }
                                if ((romFile.Format == RomFormat.Bin) && (!romFile.ConfigPath.Exists() || (localRomCopy != null)))
                                {
                                    // Logic for .cfg file:
                                    // OnRemovableDevice: NO                          | YES
                                    // Has .cfg NO        Create (no local copy made) | Create (local copy made, original is null, copy is from stock - if original changes, update copy of both ROM and .cfg)
                                    //          YES       Use (no local copy made)    | Create (update if original changes)
                                    if (localRomCopy != null)
                                    {
                                        var cfgFilePath = localRomCopy.ConfigPath;
                                        if (!cfgFilePath.Exists())
                                        {
                                            cfgFilePath = localRomCopy.GenerateStockCfgFile(programInfo);
                                        }
                                        if (cfgFilePath.Exists())
                                        {
                                            programDescription.Files.AddSupportFile(ProgramFileKind.CfgFile, cfgFilePath);
                                        }
                                    }
                                    else
                                    {
                                        var cfgFilePath = romFile.GenerateStockCfgFile(programInfo);
                                        if (cfgFilePath.IsValid)
                                        {
                                            romFile.UpdateCfgFile(cfgFilePath);
                                        }
                                    }
                                }
                                addedItems.Add(programDescription);
                                if (updateNumDiscovered != null)
                                {
                                    updateNumDiscovered(addedItems.Count);
                                }
                            }
                            else if (duplicateRoms != null)
                            {
                                duplicateRoms.Add(romFile.RomPath.Path);
                            }
                        }
                    }
                    else if (alreadyAdded && (duplicateRoms != null))
                    {
                        duplicateRoms.Add(romFile.RomPath.Path);
                    }
                    else if ((romFile != null) && ((romFile.Crc == 0) || (romFile.Crc == INTV.Core.Utility.Crc32.InitialValue) || !romFile.IsValid))
                    {
                        // TODO: Report rejected ROMs?
                    }
                }
            }
            return addedItems;
        }

        /// <summary>
        /// Replaces the contents of this instance with contents loaded from a file.
        /// </summary>
        /// <param name="filePath">Absolute path to a file containing a ProgramCollection.</param>
        public void Initialize(string filePath)
        {
            // On Mac, it was found that in release builds, a condition exists in which the
            // jzIntv component's configuration has not been initialized prior to INTV.Core trying
            // to fetch a stock .cfg file. This access attempts to ensure proper order of operations.
            var jzIntvConfiguration = SingleInstanceApplication.Instance.GetConfiguration<INTV.JzIntv.Model.Configuration>();
            if (!Directory.Exists(jzIntvConfiguration.DefaultToolsDirectory))
            {
                throw new FileNotFoundException(Resources.Strings.ToolsDirectoryMissingMessage, jzIntvConfiguration.DefaultToolsDirectory);
            }
            var programs = ProgramCollection.Load(filePath);
            Clear();
            AddNewItemsFromList(programs);
        }

        /// <summary>
        /// Save this instance of ProgramCollection to a file.
        /// </summary>
        /// <param name="filePath">The absolute path to save the data to.</param>
        /// <param name="handleErrorIfPossible">If <c>true</c>, attempt to handle the error if possible, otherwise throw.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity", Justification = "We intern the string prior to locking, so it is not weak.")]
        public void Save(string filePath, bool handleErrorIfPossible)
        {
            var path = string.Intern(filePath);
            var backupPath = path.GetUniqueBackupFilePath();
            try 
            {
                lock (path)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Copy(path, backupPath); // back up the current file
                    }
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Program.ProgramCollection));
                        serializer.Serialize(fileStream, this);
                    }
                    if (!string.IsNullOrEmpty(backupPath) && System.IO.File.Exists(backupPath))
                    {
                        FileUtilities.DeleteFile(backupPath, false, 10);
                    }
                }
            }
            catch (Exception error)
            {
                var saveFailed = SaveFailed;
                if (handleErrorIfPossible && (saveFailed != null))
                {
                    var saveFailedArgs = new ProgramCollectionSaveFailedEventArgs(path, error, backupPath);
                    saveFailed(this, saveFailedArgs);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Adds ProgramDescription entries to the collection by inspecting a collection of ROM files.
        /// </summary>
        /// <param name="programDescriptions">The collection of ROM files to create ProgramDescription entries for.</param>
        /// <returns>A collection containing the ROMs that were actually added.</returns>
        public IList<ProgramDescription> AddNewItemsFromList(IEnumerable<ProgramDescription> programDescriptions)
        {
            return AddNewItemsFromList(programDescriptions, -1);
        }

        /// <summary>
        /// Adds ProgramDescription entries to the collection by inspecting a collection of ROM files.
        /// </summary>
        /// <param name="programDescriptions">The collection of ROM files to create ProgramDescription entries for.</param>
        /// <param name="insertLocation">Location to insert new ROMs. A value of <c>-1</c> indicates to append to the end.</param>
        /// <returns>A collection containing the ROMs that were actually added.</returns>
        public IList<ProgramDescription> AddNewItemsFromList(IEnumerable<ProgramDescription> programDescriptions, int insertLocation)
        {
            var addedItems = programDescriptions.Except(this).ToList();
            OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    foreach (var addedItem in addedItems)
                    {
                        if (insertLocation < 0)
                        {
                            Add(addedItem);
                        }
                        else
                        {
                            Insert(insertLocation++, addedItem);
                        }
                    }
                });
            return addedItems;
        }

        /// <summary>
        /// Call this method to prepare the collection and any interested parties for the addition of objects.
        /// This will raise the AddRomsFromFilesBegin event.
        /// </summary>
        /// <param name="addingStarterRoms">Indicates whether we are adding "starter ROMs".</param>
        /// <returns><c>true</c> if the operation should be canceled.</returns>
        internal bool BeginAddRomsFromFiles(bool addingStarterRoms)
        {
            var cancel = false;
            var addRomsFromFilesBegin = AddRomsFromFilesBegin;
            if (addRomsFromFilesBegin != null)
            {
                var addRomsArgs = new AddRomsFromFilesBeginEventArgs() { AddingStarterRoms = addingStarterRoms };
                addRomsFromFilesBegin(this, addRomsArgs);
                cancel = addRomsArgs.Cancel;
            }
            return cancel;
        }

        /// <summary>
        /// Call this method to inform the collection and any interested parties that object addition is finished.
        /// This will raise the AddRomsFromFilesEnd event.
        /// </summary>
        /// <param name="duplicateRomPaths">A collection of ROMs that were not added because duplicate entries were already in the main ROM list.</param>
        internal void EndAddRomsFromFiles(IEnumerable<string> duplicateRomPaths)
        {
            var addRomsFromFilesEnd = AddRomsFromFilesEnd;
            if (addRomsFromFilesEnd != null)
            {
                addRomsFromFilesEnd(this, new AddRomsFromFilesEndEventArgs(duplicateRomPaths));
            }
        }

        /// <summary>
        /// Registers an event handler to be called when the user expresses the desire to invoke a program ROM.
        /// </summary>
        /// <param name="invokeProgramHandler">The handler to register.</param>
        /// <param name="priority">The priority of the handler relative to other handlers. A lower value
        /// indicates a higher priority. For example, a handler with priorty 0.5 is called before one with a priority of 100.</param>
        public void AddInvokeProgramHandler(EventHandler<InvokeProgramEventArgs> invokeProgramHandler, double priority)
        {
            RemoveInvokeProgramHandler(invokeProgramHandler);
            InvokeProgramHandlers.Add(new Tuple<EventHandler<InvokeProgramEventArgs>, double>(invokeProgramHandler, priority));
        }

        /// <summary>
        /// Removes an event handler that had been registered for execution when a program in the list was to be invoked.
        /// </summary>
        /// <param name="invokeProgramHandler">The handler to remove.</param>
        public void RemoveInvokeProgramHandler(EventHandler<InvokeProgramEventArgs> invokeProgramHandler)
        {
            var toRemove = InvokeProgramHandlers.Where(h => h.Item1 == invokeProgramHandler).ToList();
            toRemove.ForEach(h => InvokeProgramHandlers.Remove(h));
        }

        /// <summary>
        /// Called to raise the InvokeProgram event.
        /// </summary>
        /// <param name="program">The program to invoke.</param>
        internal void InvokeProgramFromDescription(ProgramDescription program)
        {
            var invokeEventArgs = new InvokeProgramEventArgs(program);
            foreach (var invokeProgramHandler in InvokeProgramHandlers.OrderBy(h => h.Item2))
            {
                invokeProgramHandler.Item1(this, invokeEventArgs);
                if (invokeEventArgs.Handled)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Called to raise the ProgramFeaturesChanged event.
        /// </summary>
        /// <param name="updatedPrograms">The programs whose ROM features have changed.</param>
        /// <param name="resetToDefault">If <c>true</c>, features are being reset to default values.</param>
        internal void ReportProgramFeaturesChanged(IEnumerable<ProgramDescription> updatedPrograms, bool resetToDefault)
        {
            var modifiedRoms = updatedPrograms.Select(p => p.Rom);
            var programFeaturesChanged = ProgramFeaturesChanged;
            if (programFeaturesChanged != null)
            {
                programFeaturesChanged(this, new ProgramFeaturesChangedEventArgs(modifiedRoms, resetToDefault));
            }
        }

        /// <summary>
        /// Called to raise the ProgramStatusChanged event.
        /// </summary>
        /// <param name="updatedPrograms">The programs whose ROM availability has changed.</param>
        internal void ReportProgramStatusChanged(IEnumerable<ProgramDescription> updatedPrograms)
        {
            var updatedRoms = updatedPrograms.Select(p => p.Rom);
            var programStatusChanged = ProgramStatusChanged;
            if (programStatusChanged != null)
            {
                programStatusChanged(this, new ProgramFeaturesChangedEventArgs(updatedRoms, false));
            }
        }
    }
}
