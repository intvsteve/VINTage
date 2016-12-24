// <copyright file="RomFormat.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// This enumeration defines various ROM file formats supported by the library.
    /// </summary>
    public enum RomFormat
    {
        /// <summary>
        /// An unknown or undetermined ROM format.
        /// </summary>
        None,

        /// <summary>
        /// The original .bin format (e.g. from the earliest Intellivision emulators and ROM dumps). Typically accompanied by a companion .cfg file.
        /// </summary>
        Bin,

        /// <summary>
        /// The original .rom format conceived for the Intellicart.
        /// </summary>
        Intellicart,

        /// <summary>
        /// Alias for Intellicart ROM format.
        /// </summary>
        Rom = Intellicart,

        /// <summary>
        /// Modified .rom format for the CuttleCart 3.
        /// </summary>
        CuttleCart3,

        /// <summary>
        /// Advanced form of the CuttleCart3 format.
        /// </summary>
        CuttleCart3Advanced,

        /// <summary>
        /// Format used by the LTO Flash! device.
        /// </summary>
        Luigi,
    }

    /// <summary>
    /// Extension methods for the RomFormat enumeration.
    /// </summary>
    public static class RomFormatHelpers
    {
        /// <summary>
        /// Gets the file extension for the given ROM format.
        /// </summary>
        /// <returns>The file extension.</returns>
        /// <param name="romFormat">Rom format.</param>
        public static string FileExtension(this RomFormat romFormat)
        {
            var fileExtension = string.Empty;
            switch (romFormat)
            {
                case RomFormat.Bin:
                    fileExtension = ".bin";
                    break;
                case RomFormat.Intellicart:
                    fileExtension = ".rom";
                    break;
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    fileExtension = ".cc3";
                    break;
                case RomFormat.Luigi:
                    fileExtension = ".luigi";
                    break;
            }
            return fileExtension;
        }
    }
}
