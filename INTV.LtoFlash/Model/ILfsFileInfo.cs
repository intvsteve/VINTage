// <copyright file="ILfsFileInfo.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This interface describes a Locutus File System (LFS) file object. It consists of various
    /// descriptors as declared in the specification.
    /// </summary>
    /// <remarks>The flat binary layout is as follows:
    /// Start   End     Description
    /// -----   -----   -------------------------------------------------------
    /// 0x000   0x000   File type
    /// 0x001   0x001   File color
    /// 0x002   0x013   Short name, NUL padded to 18 bytes if needed
    /// 0x014   0x04F   Long name, NUL padded to 60 bytes if needed
    /// 0x050   0x050   Directory fork Global Directory Number (GDN)
    /// 0x051   0x051   Reserved (Set to 0xFF)
    /// 0x052   0x053   Global forK Number (GKN) of data fork 0
    /// 0x054   0x055   Global forK Number (GKN) of data fork 1
    /// 0x056   0x057   Global forK Number (GKN) of data fork 2
    /// 0x058   0x059   Global forK Number (GKN) of data fork 3
    /// 0x05A   0x05B   Global forK Number (GKN) of data fork 4
    /// 0x05C   0x05D   Global forK Number (GKN) of data fork 5
    /// 0x05E   0x05F   Global forK Number (GKN) of data fork 6
    /// Expressed as a C / C++ data structure, each GFT entry is laid out as follows:
    /// struct file_info
    /// {
    ///     uint8_t     type;
    ///     uint8_t     color;          // 0 .. 7
    ///     uint8_t     short_name[18]; // pad with NUL to 18 chars
    ///     uint8_t     long_name [60]; // pad with NUL to 60 chars
    ///     uint8_t     dir_fork_gdn;   // GDN of directory fork; FF if none
    ///     uint8_t     rsvd_0;         // align to 16-bit boundary
    ///     uint8_t     fork[7][2];     // fork number in GKT 
    /// };
    /// </remarks>
    public interface ILfsFileInfo : IGlobalFileSystemEntry
    {
        #region Properties

        /// <summary>
        /// Gets the kind of file represented (file or directory).
        /// </summary>
        FileType FileType { get; }

        /// <summary>
        /// Gets or sets the color used to display the file icon or other visual representation on an Intellivision system.
        /// </summary>
        INTV.Core.Model.Stic.Color Color { get; set; }

        /// <summary>
        /// Gets or sets the the short name to display for a file.
        /// </summary>
        string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the long name to display for a file.
        /// </summary>
        string LongName { get; set; }

        /// <summary>
        /// Gets or sets the global ID for the file, GlobalFileTable.InvalidFileNumber if invalid or unassigned.
        /// </summary>
        ushort GlobalFileNumber { get; set; }

        /// <summary>
        /// Gets or sets the global ID for the directory fork, GlobalDirectoryTable.InvalidDirectoryNumber if none.
        /// </summary>
        byte GlobalDirectoryNumber { get; set; }

        /// <summary>
        /// Gets a value whose meaning is reserved for future use.
        /// </summary>
        byte Reserved { get; }

        /// <summary>
        /// Gets the global ID numbers of the forks associated with the file. Unused forks have the value GlobalForkTable.InvalidForkNumber.
        /// </summary>
        ushort[] ForkNumbers { get; }

        /// <summary>
        /// Gets or sets the data fork for the entry's program ROM.
        /// </summary>
        Fork Rom { get; set; }

        /// <summary>
        /// Gets or sets the data fork for the entry's instruction manual.
        /// </summary>
        Fork Manual { get; set; }

        /// <summary>
        /// Gets or sets the data fork for the entry's JLP Flash storage.
        /// </summary>
        Fork JlpFlash { get; set; }

        /// <summary>
        /// Gets or sets the data fork for the entry's vignette entry.
        /// </summary>
        Fork Vignette { get; set; }

        /// <summary>
        /// Gets or sets a fork reserved for future expansion.
        /// </summary>
        Fork ReservedFork4 { get; set; }

        /// <summary>
        /// Gets or sets a fork reserved for future expansion.
        /// </summary>
        Fork ReservedFork5 { get; set; }

        /// <summary>
        /// Gets or sets a fork reserved for future expansion.
        /// </summary>
        Fork ReservedFork6 { get; set; }

        #endregion // Properties
    }
}
