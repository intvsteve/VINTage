// <copyright file="ProgramFileKind.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// The kinds of files used to describe a program.
    /// </summary>
    public enum ProgramFileKind
    {
        /// <summary>
        /// Not a valid program file kind.
        /// </summary>
        None,

        /// <summary>
        /// ROM file for the program.
        /// </summary>
        Rom,

        /// <summary>
        /// Sentinel value for the first support file value.
        /// </summary>
        SupportFile,

        /// <summary>
        /// Sentinel value for the first image support file value.
        /// </summary>
        FirstImageFile = SupportFile,

        /// <summary>
        /// Box image file.
        /// </summary>
        Box = FirstImageFile,

        /// <summary>
        /// Label image file.
        /// </summary>
        Label,

        /// <summary>
        /// Overlay image file.
        /// </summary>
        Overlay,

        /// <summary>
        /// Manual image file.
        /// </summary>
        ManualCover,

        /// <summary>
        /// Sentinel value for last image support file value.
        /// </summary>
        LastImageFile = ManualCover,

        /// <summary>
        /// Program instructions file.
        /// </summary>
        ManualText,

        /// <summary>
        /// JLP save data file.
        /// </summary>
        SaveData,

        /// <summary>
        /// Configuration file for .bin-format ROMs.
        /// </summary>
        CfgFile,

        /// <summary>
        /// Universal ROM format for LTO Flash.
        /// </summary>
        LuigiFile,

        /// <summary>
        /// LTO Flash! vignette support file.
        /// </summary>
        Vignette,

        /// <summary>
        /// Generic support file.
        /// </summary>
        GenericSupportFile,

        /// <summary>
        /// Sentinel value for number of program files.
        /// </summary>
        NumFileKinds
    }
}
