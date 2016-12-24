// <copyright file="ProgramSupportFileState.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// These values describe the state of a program's support file.
    /// </summary>
    public enum ProgramSupportFileState
    {
        /// <summary>
        /// File state is unknown.
        /// </summary>
        None = 0,

        /// <summary>
        /// File is present and unchanged compared to reference file.
        /// </summary>
        PresentAndUnchanged,

        /// <summary>
        /// File is present, but differs from the reference file.
        /// </summary>
        PresentButModified,

        /// <summary>
        /// File cannot be located.
        /// </summary>
        Missing,

        /// <summary>
        /// File is new.
        /// </summary>
        New,

        /// <summary>
        /// File is to be deleted.
        /// </summary>
        Deleted,

        /// <summary>
        /// The file may only be used when a certain peripheral is attached, but
        /// the required peripheral is not accessible at the moment.
        /// </summary>
        RequiredPeripheralNotAttached,

        /// <summary>
        /// The file may only be used when a certain peripheral is attached, and
        /// that required peripheral is currently accessible.
        /// </summary>
        RequiredPeripheralAvailable,

        /// <summary>
        /// The file may only be used when a certain peripheral is attached.
        /// Although a peripheral is attached, is is not compatible with the file.
        /// </summary>
        RequiredPeripheralIncompatible,

        /// <summary>
        /// The file may only be used when a certain peripheral is attached.
        /// A specific peripheral is required, but has not been attached to
        /// the system. One specific scenario in which this is used is the case
        /// in which a LUIGI file is marked to execute on a specific LTO Flash!
        /// device, but that device has never been attached to the host computer.
        /// </summary>
        RequiredPeripheralUnknown,
    }
}
