// <copyright file="IFile.cs" company="INTV Funhouse">
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
    /// This interface describes a file for the purposes of manipulating a menu layout to be deployed to a Locutus device.
    /// </summary>
    public interface IFile
    {
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
        /// Gets or sets the container of the file.
        /// </summary>
        IFileContainer Parent { get; set; }

        /// <summary>
        /// Gets or sets the 32-bit CRC value to identify the file.
        /// </summary>
        uint Crc32 { get; set; }
    }
}
