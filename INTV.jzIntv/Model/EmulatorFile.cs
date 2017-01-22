// <copyright file="EmulatorFile.cs" company="INTV Funhouse">
// Copyright (c) 2012-2016 All Rights Reserved
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

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// Specific files the emulator knows or cares about - either required, optional, or implicit.
    /// </summary>
    public enum EmulatorFile
    {
        /// <summary>Not a valid file.</summary>
        None = 0,

        /// <summary>Path to the actual jzIntv emulator executable.</summary>
        JzIntv,

        /// <summary>Path to the EXEC ROM file.</summary>
        Exec,

        /// <summary>Path to the GROM file.</summary>
        Grom,

        /// <summary>Path to the ECS ROM.</summary>
        Ecs,

        /// <summary>Path to a keyboard configuration (hack) file.</summary>
        KeyboardConfig,

        /// <summary>Path to Classic Game Controller 0 (Master Component) configuration (hack) file.</summary>
        Cgc0Config,

        /// <summary>Path to Classic Game Controller 1 (ECS) configuration (hack) file.</summary>
        Cgc1Config,
    }
}
