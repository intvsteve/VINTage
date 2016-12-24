// <copyright file="ProgramInformationMergeFieldsFlags.cs" company="INTV Funhouse">
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
    
namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Flags used to control how IProgramInformation entries are merged.
    /// </summary>
    [System.Flags]
    public enum ProgramInformationMergeFieldsFlags : uint
    {
        /// <summary>
        /// No fields are specifically included or excluded.
        /// </summary>
        None = 0,

        /// <summary>
        /// The title field should be considered.
        /// </summary>
        Title = 1 << 0,

        /// <summary>
        /// The vendor field should be considered.
        /// </summary>
        Vendor = 1 << 1,

        /// <summary>
        /// The year field should be considered.
        /// </summary>
        Year = 1 << 2,

        /// <summary>
        /// The features field should be considered.
        /// </summary>
        Features = 1 << 3,

        /// <summary>
        /// The short name field should be considered.
        /// </summary>
        ShortName = 1 << 4,

        /// <summary>
        /// The CRCs field should be considered.
        /// </summary>
        Crcs = 1 << 5,

        /// <summary>
        /// All fields should be considered.
        /// </summary>
        All = 0xFFFFFFFF
    }
}
