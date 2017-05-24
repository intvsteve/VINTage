// <copyright file="MenuLayout.cs" company="INTV Funhouse">
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

////#define MEASURE_LOAD_PERFORMANCE
////#define MEASURE_SAVE_PERFORMANCE
////#define DEBUG_RESERVED_FORKS

using System.Collections.Generic;
using System.Xml.Serialization;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Specializes the root folder for a menu layout.
    /// </summary>
    [System.Xml.Serialization.XmlRoot]
    public class MenuLayout : Folder
    {
        /// <summary>
        /// Default name for the root of the menu.
        /// </summary>
        internal const string RootName = "LTO Flash!";
        private const string RootSaveDataForkXmlNodeOverrideName = "DownloadAndPlaySaveDataFork";

        private readonly System.Collections.Concurrent.ConcurrentDictionary<uint, SaveMenuLayoutTaskData> _saveTasks = new System.Collections.Concurrent.ConcurrentDictionary<uint, SaveMenuLayoutTaskData>();

        #region Constructors

        /// <summary>
        /// Initializes the special MenuLayout folder instance.
        /// </summary>
        public MenuLayout()
            : base(new FileSystem(FileSystemOrigin.HostComputer), GlobalDirectoryTable.RootDirectoryNumber, string.Empty)
        {
            base.LongName = RootName;
            if (FileSystem.Directories[GlobalDirectoryTable.RootDirectoryNumber] != this)
            {
                ErrorReporting.ReportError<System.InvalidOperationException>(ReportMechanism.Default, "MenuLayout", "MenuLayout");
            }
        }

        /// <summary>
        /// Initializes a new instance of a MenuLayout from an existing FileSystem.
        /// </summary>
        /// <param name="fileSystem">The file system describing the menu.</param>
        /// <param name="deviceId">If not <c>null</c> or empty, indicates the menu layout is for a specific device. Used in error reporting.</param>
        public MenuLayout(FileSystem fileSystem, string deviceId)
            : base(fileSystem, GlobalDirectoryTable.RootDirectoryNumber, deviceId)
        {
            if (fileSystem.Origin == FileSystemOrigin.LtoFlash)
            {
                base.ShortName = fileSystem.Files[0].ShortName;
                base.LongName = fileSystem.Files[0].LongName;
                var directoryFile = fileSystem.Files[GlobalFileTable.RootDirectoryFileNumber];
                var forkNumbers = directoryFile.ForkNumbers;
                SetForks(forkNumbers);
            }
            else if (fileSystem.Origin == FileSystemOrigin.None)
            {
                base.ShortName = Resources.Strings.FileSystem_Unknown;
                base.LongName = null;
            }
            else
            {
                base.ShortName = RootName;
                base.LongName = null;
            }
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the root folder of the menu layout.
        /// </summary>
        public Folder Root
        {
            get { return this; }
        }

        #region IFile Properties

        /// <inheritdoc />
        public override string ShortName
        {
            get { return base.ShortName; }
            set { base.ShortName = value; }
        }

        /// <inheritdoc />
        public override string LongName
        {
            get { return base.LongName; }
            set { base.LongName = value; }
        }

        /// <inheritdoc />
        [System.Xml.Serialization.XmlIgnore]
        public override IFileContainer Parent
        {
            get { return null; }
            set { }
        }

        #endregion // IFile Properties

        #region ILfsFileInfo Properties

        /// <inheritdoc />
        public override ushort GlobalFileNumber
        {
            get { return GlobalFileTable.RootDirectoryFileNumber; }
            set { }
        }

        #endregion // ILfsFileInfo Properties

        [System.Xml.Serialization.XmlIgnore]
        private uint SaveGeneration { get; set; }

#if MEASURE_LOAD_PERFORMANCE || MEASURE_SAVE_PERFORMANCE
        /// <summary>
        /// Gets the logger used by the port.
        /// </summary>
        private INTV.Shared.Utility.Logger Logger
        {
            get
            {
                if (_logger == null)
                {
                    var path = System.IO.Path.Combine(Configuration.Instance.ErrorLogDirectory, "MenuSaveLoadLog.txt");
                    _logger = new Logger(path);
                }
                return _logger;
            }
        }

        private INTV.Shared.Utility.Logger _logger;
#endif // MEASURE_LOAD_PERFORMANCE || MEASURE_SAVE_PERFORMANCE

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised upon completion of a save operation on the menu.
        /// </summary>
        public event System.EventHandler<MenuSaveCompleteEventArgs> MenuLayoutSaved;

        #endregion // Events

        /// <summary>
        /// Loads a MenuLayout from disk.
        /// </summary>
        /// <param name="filePath">Absolute path to a serialized MenuLayout.</param>
        /// <returns>The deserialized menu layout.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity", Justification = "We intern the string prior to locking, so it is not weak.")]
        public static MenuLayout Load(string filePath)
        {
            MenuLayout menuLayout = null;
#if MEASURE_LOAD_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // MEASURE_LOAD_PERFORMANCE
            var path = string.Intern(filePath);
            lock (path)
            {
                using (var fileStream = FileUtilities.OpenFileStream(filePath))
                {
                    var overrides = GetAttributeOverrides();
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MenuLayout), overrides, new System.Type[] { typeof(FileNode), typeof(MenuLayout), typeof(Folder), typeof(Program) }, null, null);
                    menuLayout = serializer.Deserialize(fileStream) as MenuLayout;
                    menuLayout.LoadComplete(false);
                    return menuLayout;
                }
            }
#if MEASURE_LOAD_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                DebugMessage("Load " + filePath + " took " + stopwatch.Elapsed.ToString());
                if (menuLayout != null)
                {
                    menuLayout.Logger.Log("Load: " + filePath + " took " + stopwatch.Elapsed.ToString());
                }
            }
#endif // MEASURE_LOAD_PERFORMANCE
        }

        /// <summary>
        /// Finishes initializing the MenuLayout after it is loaded into memory.
        /// </summary>
        /// <param name="updateRomList">If <c>true</c>, also update the ROM list.</param>
        public void LoadComplete(bool updateRomList)
        {
            LoadComplete(FileSystem, updateRomList);
#if DEBUG_RESERVED_FORKS
            foreach (var file in FileSystem.Files)
            {
                if (file != null)
                {
                    var reservedFork = new Fork(CacheIndex.Path);
                    FileSystem.Forks.Add(reservedFork);
                    file.ReservedFork4 = reservedFork;
                }
            }
#endif // DEBUG_RESERVED_FORKS
        }

        /// <summary>
        /// Creates a new Folder, placing into the same FileSystem as the MenuLayout.
        /// </summary>
        /// <param name="folderName">The name of the Folder.</param>
        /// <returns>The new Folder.</returns>
        public Folder CreateFolder(string folderName)
        {
            var folder = new Folder(FileSystem, GlobalDirectoryTable.InvalidDirectoryNumber, string.Empty) { LongName = folderName };
            return folder;
        }

        /// <summary>
        /// Creates a new Program, placing into the same FileSystem as the MenuLayout.
        /// </summary>
        /// <param name="description">The ProgramDescription the new Program object represents.</param>
        /// <returns>The new Program.</returns>
        public Program CreateProgram(ProgramDescription description)
        {
            var program = new Program(description, FileSystem);
            return program;
        }

        /// <summary>
        /// Save the menu layout to a specific path.
        /// </summary>
        /// <param name="path">The absolute path of the file to save.</param>
        public void Save(string path)
        {
            Save(path, false);
        }

        /// <summary>
        /// Save the menu layout to a specific path.
        /// </summary>
        /// <param name="path">The absolute path of the file to save.</param>
        /// <param name="nonDirtying">If <c>true</c>, indicates save was not due to user edits, but some other operation.</param>
        internal void Save(string path, bool nonDirtying)
        {
            SaveMenuLayoutTaskData existingSave;
            if (!_saveTasks.TryGetValue(SaveGeneration, out existingSave))
            {
                var saveMenuTask = new AsyncTaskWithProgress("SaveMenuLayout", true); // does not show progress
                var saveTaskData = new SaveMenuLayoutTaskData(saveMenuTask, this, path, nonDirtying);
                DebugMessage("Save BEGIN " + path + " GENERATION " + SaveGeneration);
                _saveTasks[SaveGeneration] = saveTaskData;
                saveMenuTask.RunTask(saveTaskData, Save, SaveComplete);
            }
            else
            {
                DebugMessage("Save ALREADY CREATED for " + path + " GENERATION " + SaveGeneration);
            }
        }

        /// <inheritdoc />
        protected override void OnItemsChanged()
        {
            RaiseContentsChanged(this, System.EventArgs.Empty);
        }

        /// <summary>
        /// Get the XML Element name overrides for any properties that we may want to have different names for at different points in the
        /// persistence hierarchy. For example, 'ManualFork' doesn't make sense for directories (Folder, MenuLayout) - but LTO Flash! can
        /// put other useful data in such forks. (E.g. menu position information.)
        /// </summary>
        /// <returns>The override data to apply during serialize / deserialize operations.</returns>
        private static XmlAttributeOverrides GetAttributeOverrides()
        {
            var overrides = new XmlAttributeOverrides();

            var overrideData = new Dictionary<System.Type, IEnumerable<System.Tuple<string, string>>>()
            {
                { typeof(Folder), new[] { new System.Tuple<string, string>("JlpFlash", RootSaveDataForkXmlNodeOverrideName) } },
                ////{ typeof(TYPE ON WHICH PROPERTY EXISTS AS XML ELEMENT), new[] { new System.Tuple<string, string>("PROPERTY NAME IN C#", "NEW XML NODE NAME") } }, // If we rename other nodes, put new names here
            };

            foreach (var overrideInfo in overrideData)
            {
                foreach (var attributeNameOverride in overrideInfo.Value)
                {
                    var elementAttribute = new XmlElementAttribute(attributeNameOverride.Item2);
                    var attributes = new XmlAttributes();
                    attributes.XmlIgnore = false;
                    attributes.XmlElements.Add(elementAttribute);
                    overrides.Add(overrideInfo.Key, attributeNameOverride.Item1, attributes);
                }
            }

            return overrides;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity", Justification = "We intern the string prior to locking, so it is not weak.")]
        private static void Save(AsyncTaskData taskData)
        {
            var saveMenuTaskData = (SaveMenuLayoutTaskData)taskData;
            var menuLayout = saveMenuTaskData.GetMenuLayoutToSave();
            var path = saveMenuTaskData.Path;
            if (taskData.AcceptCancelIfRequested())
            {
                DebugMessage("Save CANCELLED for " + path + " GENERATION " + menuLayout.SaveGeneration);
                return;
            }
#if MEASURE_SAVE_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
#endif // MEASURE_SAVE_PERFORMANCE
                if (taskData.AcceptCancelIfRequested())
                {
                    DebugMessage("Save CANCELLED for " + path + " GENERATION " + menuLayout.SaveGeneration);
                    return;
                }
                DebugMessage("Save EXECUTE for " + path + " GENERATION " + menuLayout.SaveGeneration);
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                }
                if (taskData.AcceptCancelIfRequested())
                {
                    DebugMessage("Save CANCELLED for " + path + " GENERATION " + menuLayout.SaveGeneration);
                    return;
                }
                var filePath = string.Intern(path);
                lock (filePath)
                {
                    using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
                    {
                        var overrides = GetAttributeOverrides();
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MenuLayout), overrides, new System.Type[] { typeof(FileNode), typeof(MenuLayout), typeof(Folder), typeof(Program) }, null, null);
                        serializer.Serialize(fileStream, menuLayout);
                    }
                }
#if MEASURE_SAVE_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                DebugMessage("Save " + path + " GENERATION " + menuLayout.SaveGeneration + " took " + stopwatch.Elapsed.ToString());
                if (menuLayout != null)
                {
                    menuLayout.Logger.Log("Save: " + path + " GENERATION " + menuLayout.SaveGeneration + " took " + stopwatch.Elapsed.ToString());
                }
            }
#endif // MEASURE_SAVE_PERFORMANCE
        }

        private static void SaveComplete(AsyncTaskData taskData)
        {
            var saveMenuTaskData = (SaveMenuLayoutTaskData)taskData;
            var savedMenu = saveMenuTaskData.OriginalMenuLayout;
            SaveMenuLayoutTaskData validationData;
            if (!savedMenu._saveTasks.TryRemove(saveMenuTaskData.SaveGeneration, out validationData) || !object.ReferenceEquals(validationData, taskData))
            {
                DebugMessage("Save BUG for " + saveMenuTaskData.Path + " GENERATION " + saveMenuTaskData.SaveGeneration);
            }
            if (taskData.Cancelled || (savedMenu.Crc32 != saveMenuTaskData.GetMenuLayoutToSave().Crc32))
            {
                // re-queue the save.
                DebugMessage("Re-queue save for: " + saveMenuTaskData.Path + " GENERATION " + savedMenu.SaveGeneration);
                savedMenu.Save(saveMenuTaskData.Path, saveMenuTaskData.NonDirtying);
            }
            else
            {
                DebugMessage("Save COMPLETE for: " + saveMenuTaskData.Path + " GENERATION " + savedMenu.SaveGeneration);
                ++saveMenuTaskData.OriginalMenuLayout.SaveGeneration;
                var menuLayoutSaved = saveMenuTaskData.OriginalMenuLayout.MenuLayoutSaved;
                if (menuLayoutSaved != null)
                {
                    var saveFinishedArgs = new MenuSaveCompleteEventArgs(saveMenuTaskData.Path, taskData.Error, saveMenuTaskData.BackupPath, saveMenuTaskData.NonDirtying);
                    menuLayoutSaved(savedMenu, saveFinishedArgs);
                }
            }
        }

        [System.Diagnostics.Conditional("MEASURE_SAVE_PERFORMANCE")]
        [System.Diagnostics.Conditional("MEASURE_LOAD_PERFORMANCE")]
        private static void DebugMessage(object message)
        {
            System.Diagnostics.Debug.WriteLine("## ThId" + System.Threading.Thread.CurrentThread.ManagedThreadId + ": " + message);
        }

        /// <summary>
        /// Async task data for saving a MenuLayout.
        /// </summary>
        private class SaveMenuLayoutTaskData : AsyncTaskData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="INTV.LtoFlash.Model.MenuLayout+SaveMenuLayoutTaskData"/> class.
            /// </summary>
            /// <param name="task">The task that's going to use this instance.</param>
            /// <param name="menuLayout">The MenuLayout being saved.</param>
            /// <param name="path">The absolute path to save the MenuLayout to.</param>
            /// <param name="nonDirtying">If <c>true</c>, indicates save was not due to user edits, but some other operation.</param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity", Justification = "We intern the string prior to locking, so it is not weak.")]
            public SaveMenuLayoutTaskData(AsyncTaskWithProgress task, MenuLayout menuLayout, string path, bool nonDirtying)
                : base(task)
            {
                lock (menuLayout.FileSystem)
                {
                    if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                    {
                        var menuPath = string.Intern(path);
                        lock (menuPath)
                        {
                            BackupPath = path.GetUniqueBackupFilePath();
                            System.IO.File.Copy(path, BackupPath);
                        }
                    }
                }

                OriginalMenuLayout = menuLayout;
                SaveGeneration = menuLayout.SaveGeneration;
                if (menuLayout.FileSystem.Origin == FileSystemOrigin.HostComputer)
                {
                    MenuLayoutToSave = menuLayout.FileSystem.Clone().Directories[GlobalDirectoryTable.RootDirectoryNumber] as MenuLayout;
                    MenuLayoutToSave.FileSystem.Frozen = true;
#if MEASURE_SAVE_PERFORMANCE
                    MenuLayoutToSave.SaveGeneration = menuLayout.SaveGeneration;
#endif // MEASURE_SAVE_PERFORMANCE
                }
                else
                {
                    MenuLayoutToSave = menuLayout;
                }
                Path = path;
                NonDirtying = nonDirtying;
            }

            /// <summary>
            /// Gets the original menu layout being saved.
            /// </summary>
            public MenuLayout OriginalMenuLayout { get; private set; }

            /// <summary>
            /// Gets the absolute path to save the menu to.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Gets the backup path of the previous menu file.
            /// </summary>
            public string BackupPath { get; private set; }

            /// <summary>
            /// Gets the internal 'save generation' number.
            /// </summary>
            internal uint SaveGeneration { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the save operation was incidental, and not due to user edits.
            /// </summary>
            internal bool NonDirtying { get; private set; }

            private MenuLayout MenuLayoutToSave { get; set; }

            /// <summary>
            /// Gets the menu layout to save.
            /// </summary>
            /// <returns>The menu layout to save.</returns>
            public MenuLayout GetMenuLayoutToSave()
            {
                if (MenuLayoutToSave.Crc32 != OriginalMenuLayout.Crc32)
                {
                    MenuLayoutToSave = OriginalMenuLayout.FileSystem.Clone().Directories[GlobalDirectoryTable.RootDirectoryNumber] as MenuLayout;
                    MenuLayoutToSave.SaveGeneration = OriginalMenuLayout.SaveGeneration;
                }
                return MenuLayoutToSave;
            }
        }
    }
}
