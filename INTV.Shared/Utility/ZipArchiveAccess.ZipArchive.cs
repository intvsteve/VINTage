// <copyright file="ZipArchiveAccess.ZipArchive.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Wraps access to the native ZipArchive implementation from System.IO.Compression in .NET 4.5 and later.
    /// </summary>
    public sealed partial class ZipArchiveAccess
    {
        private static IDisposable Open(Stream stream, ZipArchiveAccessMode mode)
        {
            var zipArchive = new ZipArchive(stream, (ZipArchiveMode)mode);
            return zipArchive;
        }

        private IEnumerable<string> GetFileEntryNames()
        {
            // or use full name here?
            var zipArchive = (ZipArchive)_zipArchiveObject;
            return zipArchive.Entries.Select(e => e.Name);
        }

        private bool FileEntryExists(string fileName)
        {
            var zipArchive = (ZipArchive)_zipArchiveObject;
            var entry = zipArchive.GetEntry(fileName);
            return entry != null;
        }

        private Stream OpenFileEntry(string fileName)
        {
            var zipArchive = (ZipArchive)_zipArchiveObject;
            var entry = zipArchive.GetEntry(fileName);
            var stream = entry.Open();
            return stream;
        }

        private Stream CreateAndOpenFileEntry(string fileName, ZipArchiveCompressionMethod compressionMethod)
        {
            var zipArchive = (ZipArchive)_zipArchiveObject;
            var entry = zipArchive.CreateEntry(fileName, (CompressionLevel)compressionMethod);
            var stream = entry.Open();
            return stream;
        }

        private void DeleteFileEntry(string fileName)
        {
            var zipArchive = (ZipArchive)_zipArchiveObject;
            var entry = zipArchive.GetEntry(fileName);
            if (entry != null)
            {
                entry.Delete();
            }
        }
    }
}
