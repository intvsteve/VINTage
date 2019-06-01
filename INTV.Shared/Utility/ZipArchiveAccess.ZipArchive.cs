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
    public sealed partial class ZipArchiveAccess
    {
        internal static void Testing()
        {
            var path = @"/Users/steveno/Downloads/LTO_Flash_4764.zip";
            var clonePath = @"/Users/steveno/Downloads/LTO_Flash_4764-clone2.zip";
            ////          path = clonePath;
            ////path = @"D:\Users\Steve\Projects\appletSource1.zip";
            ////            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            ////            using (var zip = new ZipArchiveAccess(stream))
            using (var zip = new ZipArchiveAccess(stream, ZipArchiveAccessMode.Update))
            {
                ////zip.Delete("release_notes.txt");
                ////zip.Delete("goover");
                using (var clone = new FileStream(clonePath, FileMode.CreateNew, FileAccess.Write))
                {
                    using (var zipClone = new ZipArchiveAccess(clone, ZipArchiveAccessMode.Create))
                    {
                        foreach (var file in zip.FileNames)
                        {
                            System.Diagnostics.Debug.WriteLine(file);
                            using (var copy = zipClone.Add(file, ZipArchiveCompressionMethod.MaximumCompression))
                            {
                                using (var s = zip.OpenFileEntry(file))
                                {
                                    s.CopyTo(copy);
                                }
                            }
                        }
                        var exists = zip.FileExists("release_notes.txt");
                        // NOTE The OPenFileEntry below crashes on old MonoMac builds stating that the file is already
                        // open for writing. This seems like a bug, since this .zip is openf for *udpate* which should
                        // allow for read AND write.  Perhaps it's fixed in later versions of Mono than what the
                        // MonoMac build is using.
                        using (var s = zip.OpenFileEntry("release_notes.txt"))
                        {
                            var reader = new StreamReader(s);
                            var text = reader.ReadToEnd();
                        }
                        exists = zip.FileExists("goober");
                    }
                }
            }
        }

        private static IDisposable Open(Stream stream, ZipArchiveAccessMode mode)
        {
            var zipArchive = new ZipArchive(stream, (ZipArchiveMode)mode);
            return zipArchive;
        }

        private IEnumerable<string> GetFileEntryNames()
        {
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
