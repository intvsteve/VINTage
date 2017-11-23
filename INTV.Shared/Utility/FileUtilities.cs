// <copyright file="FileUtilities.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace INTV.Shared.Utility
{
    public static class FileUtilities
    {
        public static Stream OpenFileStream(string filePath)
        {
            Stream fileStream = null;
            if (File.Exists(filePath))
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            return fileStream;
        }

        public static void EnsureDirectoriesExist(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        #region File and Directory Deletion Helpers

        public static void DeleteFile(string filePath)
        {
            DeleteFile(filePath, true, 0);
        }

        public static void DeleteFile(string filePath, bool reportErrors, int retryCount)
        {
            DeleteFromFileSystem(filePath, reportErrors, retryCount, false);
        }

        public static void DeleteDirectory(string directoryPath)
        {
            DeleteDirectory(directoryPath, true, 0);
        }

        public static void DeleteDirectory(string directoryPath, bool reportErrors, int retryCount)
        {
            DeleteFromFileSystem(directoryPath, reportErrors, retryCount, true);
        }

        private static void DeleteFromFileSystem(string path, bool reportErrors, int retryCount, bool isDirectory)
        {
            if (retryCount <= 0)
            {
                DeleteFileSystemEntry(path, reportErrors, retryCount, true);
            }
            else
            {
                // Do the delete on a background thread. If an error occurs, retry, with a delay between attempts.
                var backgroundDelete = new BackgroundWorker();
                backgroundDelete.DoWork += BackgroundDelete;
                backgroundDelete.RunWorkerCompleted += BackgroundDeleteComplete;
                var data = new Tuple<string, bool, int, bool>(path, reportErrors, retryCount, isDirectory);
                backgroundDelete.RunWorkerAsync(data);
            }
        }

        private static Exception DeleteFileSystemEntry(string path, bool reportErrors, int retryCount, bool isDirectory)
        {
            Exception error = null;
            retryCount = Math.Max(1, retryCount);
            for (int retryNumber = 0; retryNumber < retryCount; ++retryNumber)
            {
                try
                {
                    if (isDirectory)
                    {
                        Directory.Delete(path, true);
                    }
                    else
                    {
                        File.Delete(path);
                    }
                    error = null;
                    break;
                }
                catch (Exception e)
                {
                    error = e;
                    if (reportErrors && (retryNumber == (retryCount - 1)))
                    {
                        throw;
                    }
                    System.Diagnostics.Debug.WriteLine(string.Format("Error deleting: {0}, retry attempt: {1}\nError: {2}", path, retryNumber, e));
                    System.Threading.Thread.Sleep(1000);
                }
            }
            return error;
        }

        private static void BackgroundDelete(object sender, DoWorkEventArgs e)
        {
            var data = e.Argument as Tuple<string, bool, int, bool>;
            var path = data.Item1;
            var reportErrors = data.Item2;
            var retryCount = data.Item3;
            var isDirectory = data.Item4;
            e.Result = DeleteFileSystemEntry(path, reportErrors, retryCount, isDirectory);
        }

        private static bool _showingErrorDialog = false;

        private static void BackgroundDeleteComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Error != null) || (e.Result != null))
            {
                if (!_showingErrorDialog && Properties.Settings.Default.ShowDetailedErrors)
                {
                    // Why are background complete callbacks not happening on UI thread on Mac???
                    // Mono BUG: https://bugzilla.xamarin.com/show_bug.cgi?id=57544
                    SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new Action(() =>
                        {
                            var error = e.Error == null ? e.Result as Exception : e.Error;
                            System.Diagnostics.Debug.WriteLine("Error deleting file:\n" + error);
                            var reportDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.DeleteFileError_Title, Resources.Strings.DeleteFileError_Message);
                            reportDialog.ReportText = Resources.Strings.DeleteFileError_ReportText;
                            reportDialog.Exception = error;
                            reportDialog.ShowSendEmailButton = false;
                            try
                            {
                                _showingErrorDialog = true;
                                reportDialog.ShowDialog(Resources.Strings.OKButton_Text);
                            }
                            finally
                            {
                                _showingErrorDialog = false;
                            }
                        }));
                }
            }
        }

        #endregion //  File and Directory Deletion Helpers
    }
}
