// <copyright file="FileSystemSyncErrors.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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
using System.Text;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This class is used to collect non-fatal errors that are encountered during file system synchronization operations.
    /// </summary>
    public class FileSystemSyncErrors
    {
        /// <summary>
        /// Initialize a new instance of type <see cref="FileSystemSyncErrors"/> type.
        /// </summary>
        /// <param name="initialDirtyFlags">The file system's 'dirty' flags at the beginning of the operation.</param>
        public FileSystemSyncErrors(LfsDirtyFlags initialDirtyFlags)
        {
            InitialDirtyFlags = initialDirtyFlags;
            FailedToCreateEntries = new HashSet<IGlobalFileSystemEntry>();
            OrphanedForks = new HashSet<Fork>();
            UnableToRetrieveForks = new HashSet<Fork>();
            UnsupportedForks = new HashSet<Tuple<ILfsFileInfo, Fork>>();
        }

        /// <summary>
        /// Gets the initial dirty flags state when the operation started.
        /// </summary>
        public LfsDirtyFlags InitialDirtyFlags { get; private set; }

        /// <summary>
        /// Gets the file system entries that could not be created during the operation.
        /// </summary>
        public ISet<IGlobalFileSystemEntry> FailedToCreateEntries { get; private set; }

        /// <summary>
        /// Gets a the orphaned forks encountered during the operation.
        /// </summary>
        public ISet<Fork> OrphanedForks { get; private set; }

        /// <summary>
        /// Gets the forks that could not be retrieved during the operation.
        /// </summary>
        public ISet<Fork> UnableToRetrieveForks { get; private set; }

        /// <summary>
        /// Gets the unsupported forks encountered during the operation.
        /// </summary>
        public ISet<Tuple<ILfsFileInfo, Fork>> UnsupportedForks { get; private set; }

        /// <summary>
        /// Gets a value indicating whether any errors were reported during the operation.
        /// </summary>
        public bool Any
        {
            get { return (InitialDirtyFlags != LfsDirtyFlags.None) || OrphanedForks.Any() || UnsupportedForks.Any(); }
        }

        /// <summary>
        /// Gets or sets data for use by clients who need to return any additional task-specific data as part of a file system synchronization operation.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Generates an error report file in the given directory.
        /// </summary>
        /// <param name="errorReportDirectory">The directory in which to generate the error report.</param>
        /// <param name="description">A short description of the type of operation during which the errors were accumulated.</param>
        /// <returns><c>true</c> if there were any errors, and thus a report generated; otherwise <c>false</c>, indicating that no error report was generated.</returns>
        public bool RecordErrors(string errorReportDirectory, string description)
        {
            return RecordErrors(errorReportDirectory, description, null);
        }

        /// <summary>
        /// Generates an error report file in the given directory.
        /// </summary>
        /// <param name="errorReportDirectory">The directory in which to generate the error report.</param>
        /// <param name="description">A short description of the type of operation during which the errors were accumulated.</param>
        /// <param name="errorLogFileName">Name of the error log. If <c>null</c> or empty, a default name of "ErrorLog.txt" will be used.</param>
        /// <returns><c>true</c> if there were any errors, and thus a report generated; otherwise <c>false</c>, indicating that no error report was generated.</returns>
        public bool RecordErrors(string errorReportDirectory, string description, string errorLogFileName)
        {
            if (Any)
            {
                var errorReportContents = new StringBuilder().AppendLine().AppendLine("FILE SYSTEM SYNC OPERATION ERROR REPORT").AppendLine("----------------------------------------");
                errorReportContents.AppendLine("OPERATION TYPE: " + description).AppendLine();
                errorReportContents.AppendLine("Initial File System Status: " + ((uint)InitialDirtyFlags).ToString("X8")).AppendLine();

                errorReportContents.AppendLine("Orphaned Forks: " + OrphanedForks.Count);
                foreach (var fork in OrphanedForks)
                {
                    errorReportContents.AppendLine("  " + fork.ToString());
                }

                errorReportContents.AppendLine().AppendLine("File System Entity Creation Failures: " + FailedToCreateEntries.Count);
                foreach (var fileSystemEntry in FailedToCreateEntries)
                {
                    errorReportContents.AppendLine("  " + fileSystemEntry.ToString());
                }

                errorReportContents.AppendLine().AppendLine("Unretrievable Forks: " + UnableToRetrieveForks.Count);
                foreach (var fork in UnableToRetrieveForks)
                {
                    errorReportContents.AppendLine("  " + fork.ToString());
                }

                errorReportContents.AppendLine().AppendLine("Unsupported Forks: " + UnsupportedForks.Count);
                foreach (var unsupportedForkData in UnsupportedForks)
                {
                    var file = unsupportedForkData.Item1;
                    var fork = unsupportedForkData.Item2;
                    var forkKind = ForkKind.None;
                    if (file != null)
                    {
                        var forkIndex = file.ForkNumbers.ToList().IndexOf(fork.GlobalForkNumber);
                        if ((forkIndex >= 0) && (forkIndex < (int)ForkKind.NumberOfForkKinds))
                        {
                            forkKind = (ForkKind)((ushort)forkIndex);
                        }
                    }
                    errorReportContents.AppendLine("  " + ((file == null) ? "<none>" : file.ToString()) + "; " + fork.ToString() + ": " + forkKind);
                }

                errorLogFileName = errorLogFileName ?? "ErrorLog.txt";
                var errorReportFile = System.IO.Path.Combine(errorReportDirectory, errorLogFileName).EnsureUniqueFileName();
                var errorReporter = new Logger(errorReportFile);
                errorReporter.Log(errorReportContents.ToString());
            }
            return Any;
        }
    }
}
