// <copyright file="IDirectory.cs" company="INTV Funhouse">
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
    /// This interface describes a Locutus File System directory object.
    /// </summary>
    /// <remarks>The flat binary layout is as follows:
    /// Start   End     Description
    /// -----   -----   -------------------------------------------------------
    /// 0x000   0x001   Parent directory GFN
    /// 0x002   0x003   GFN of 1st file
    /// 0x004   0x005   GFN of 2nd file
    ///      ...
    /// 0x1FE   0x1FF   GFN of 255th file
    /// Each directory holds up to 255 files.  Expressed as C / C++ code, directories
    /// are laid out as follows:
    /// struct directory
    /// {
    ///     uint16_t    parent_directory;           // GFN of parent directory
    ///     uint16_t    presentation_order[255];    // GFNs of files in directory
    /// };
    /// </remarks>
    public interface IDirectory : IGlobalFileSystemEntry
    {
        #region Properties

        /// <summary>
        /// Gets the Global File Number of the parent directory.
        /// </summary>
        ushort ParentDirectoryGlobalFileNumber { get; }

        /// <summary>
        /// Gets a value that describes the order in which to display the elements in the Entries property.
        /// </summary>
        PresentationOrder PresentationOrder { get; }

        /// <summary>
        /// Gets or sets the global directory number.
        /// </summary>
        byte GlobalDirectoryNumber { get; set; }

        #endregion // Properties
    }
}
