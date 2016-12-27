// <copyright file="BackupInfoViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Shared.Utility;

#if WIN
using BaseClass = System.Object;
#elif MAC
#if __UNIFIED__
using BaseClass = Foundation.NSObject;
#else
using BaseClass = MonoMac.Foundation.NSObject;
#endif
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for selecting backup data for a restore operation.
    /// </summary>
    public class BackupInfoViewModel : BaseClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.ViewModel.BackupInfoViewModel"/> class.
        /// </summary>
        /// <param name="path">The absolute path to the directory containing desired device backups.</param>
        /// <param name="fileExtensions">Supported backup file extensions used to determine how many files will be restored.</param>
        public BackupInfoViewModel(string path, IEnumerable<string> fileExtensions)
        {
            Path = path;
            var dateTime = PathUtils.GetDateTimeFromString(path);
            if ((dateTime == System.DateTime.MinValue) || (dateTime == System.DateTime.MaxValue))
            {
                // Ignore things with bad dates.
                throw new System.ArgumentOutOfRangeException();
            }
            DateTime = dateTime.ToLongDateString();
            int fileCount = 0;
            if (fileExtensions != null)
            {
                foreach (var fileExtension in fileExtensions)
                {
                    var files = System.IO.Directory.EnumerateFiles(path, "*" + fileExtension);
                    fileCount += files.Count();
                }
            }
            FileCount = fileCount.ToString();
        }

        /// <summary>
        /// Gets the date and time of a device backup.
        /// </summary>
        [OSExport("DateTime")]
        public string DateTime { get; private set; }

        /// <summary>
        /// Gets the absolute path to the backup location.
        /// </summary>
        [OSExport("Path")]
        public string Path { get; private set; }

        /// <summary>
        /// Gets the number of ROMs in the backup.
        /// </summary>
        [OSExport("FileCount")]
        public string FileCount { get; private set; }
    }
}
