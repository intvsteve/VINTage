// // <copyright file=".cs" company="INTV Funhouse">
// // Copyright (c) 2014-2016 All Rights Reserved
// // <author>Steven A. Orth</author>
// //
// // This program is free software: you can redistribute it and/or modify it
// // under the terms of the GNU General Public License as published by the
// // Free Software Foundation, either version 2 of the License, or (at your
// // option) any later version.
// //
// // This program is distributed in the hope that it will be useful, but
// // WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// // or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// // for more details.
// //
// // You should have received a copy of the GNU General Public License along
// // with this software. If not, see: http://www.gnu.org/licenses/.
// // or write to the Free Software Foundation, Inc.,
// // 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// // </copyright>
//
using System;

namespace INTV.JzIntvUI.Model
{
    /// <summary>
    /// Options for how to supply a command line to jzIntv.
    /// </summary>
    public enum CommandLineMode
    {
        /// <summary>Automatically determine command line options based on the ROM to be executed by the emulator.</summary>
        Automatic,

        /// <summary>Automatically determine command line options based on the ROM to be executed by the emulator, with additional user-specified arguments.</summary>
        AutomaticWithAdditionalArguments,

        /// <summary>User fully specifies the command line arguments, other than the ROM path.</summary>
        Custom
    }

    /// <summary>
    /// Helper methods for the CommandLineMode enumeration.
    /// </summary>
    public static class CommandLineModeHelpers
    {
        /// <summary>
        /// Given a string stored in a preferences file, convert it to a value from the CommandLineMode enumeration.
        /// </summary>
        /// <param name="setting">The string to convert.</param>
        /// <returns>The corresponding value in the CommandLineMode enumeration, or CommandLineMode.Automatic if no match is found.</returns>
        public static CommandLineMode FromSettingsString(string setting)
        {
            CommandLineMode mode;
            if (string.IsNullOrEmpty(setting) || !System.Enum.TryParse<CommandLineMode>(setting, out mode))
            {
                mode = CommandLineMode.Automatic;
            }
            return mode;
        }
    }
}
