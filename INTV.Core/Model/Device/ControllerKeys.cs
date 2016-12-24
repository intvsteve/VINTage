// <copyright file="ControllerKeys.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Controller virtual keys.
    /// </summary>
    public enum ControllerKeys : byte
    {
        /// <summary>
        /// No active inputs.
        /// </summary>
        None = 0,

        /// <summary>
        /// Consider keypad keys active.
        /// </summary>
        KeypadActive = 1 << 4,

        /// <summary>
        /// Keypad button '0'.
        /// </summary>
        Keypad0 = 0 | KeypadActive,

        /// <summary>
        /// Keypad button '1'.
        /// </summary>
        Keypad1 = 1 | KeypadActive,

        /// <summary>
        /// Keypad button '2'.
        /// </summary>
        Keypad2 = 2 | KeypadActive,

        /// <summary>
        /// Keypad button '3'.
        /// </summary>
        Keypad3 = 3 | KeypadActive,

        /// <summary>
        /// Keypad button '4'.
        /// </summary>
        Keypad4 = 4 | KeypadActive,

        /// <summary>
        /// Keypad button '5'.
        /// </summary>
        Keypad5 = 5 | KeypadActive,

        /// <summary>
        /// Keypad button '6'.
        /// </summary>
        Keypad6 = 6 | KeypadActive,

        /// <summary>
        /// Keypad button '7'.
        /// </summary>
        Keypad7 = 7 | KeypadActive,

        /// <summary>
        /// Keypad button '8'.
        /// </summary>
        Keypad8 = 8 | KeypadActive,

        /// <summary>
        /// Keypad button '9'.
        /// </summary>
        Keypad9 = 9 | KeypadActive,

        /// <summary>
        /// Keypad button 'clear'.
        /// </summary>
        KeypadClear = 10 | KeypadActive,

        /// <summary>
        /// Keypad button 'enter'.
        /// </summary>
        KeypadEnter = 11 | KeypadActive,

        /// <summary>
        /// Consider side action buttons active.
        /// </summary>
        ActionKeyActive = 1 << 5,

        /// <summary>
        /// Top action key.
        /// </summary>
        ActionKeyTop = 1 | ActionKeyActive,

        /// <summary>
        /// Lower left action key.
        /// </summary>
        ActionKeyBottomLeft = 2 | ActionKeyActive,

        /// <summary>
        /// Lower right action key.
        /// </summary>
        ActionKeyBottomRight = 3 | ActionKeyActive,

        /// <summary>
        /// Consider direction disc input active.
        /// </summary>
        DiscActive = 1 << 6,

        /// <summary>
        /// Disc direction 'east'.
        /// </summary>
        DiscE = 0 | DiscActive,

        /// <summary>
        /// Disc direction 'east-northeast'.
        /// </summary>
        DiscENE = 1 | DiscActive,

        /// <summary>
        /// Disc direction 'northeast'.
        /// </summary>
        DiscNE = 2 | DiscActive,

        /// <summary>
        /// Disc direction 'north-northeast'.
        /// </summary>
        DiscNNE = 3 | DiscActive,

        /// <summary>
        /// Disc direction 'north'.
        /// </summary>
        DiscN = 4 | DiscActive,

        /// <summary>
        /// Disc direction 'north-northwest'.
        /// </summary>
        DiscNNW = 5 | DiscActive,

        /// <summary>
        /// Disc direction 'northwest'.
        /// </summary>
        DiscNW = 6 | DiscActive,

        /// <summary>
        /// Disc direction 'west-northwest'.
        /// </summary>
        DiscWNW = 7 | DiscActive,

        /// <summary>
        /// Disc direction 'west'.
        /// </summary>
        DiscW = 8 | DiscActive,

        /// <summary>
        /// Disc direction 'west-southwest'.
        /// </summary>
        DiscWSW = 9 | DiscActive,

        /// <summary>
        /// Disc direction 'southwest'.
        /// </summary>
        DiscSW = 10 | DiscActive,

        /// <summary>
        /// Disc direction 'south-southwest'.
        /// </summary>
        DiscSSW = 11 | DiscActive,

        /// <summary>
        /// Disc direction 'south'.
        /// </summary>
        DiscS = 12 | DiscActive,

        /// <summary>
        /// Disc direction 'south-southeast'.
        /// </summary>
        DiscSSE = 13 | DiscActive,

        /// <summary>
        /// Disc direction 'southeast'.
        /// </summary>
        DiscSE = 14 | DiscActive,

        /// <summary>
        /// Disc direction 'east-southeast'.
        /// </summary>
        DiscESE = 15 | DiscActive,

        /// <summary>
        /// Consider no controller inputs active.
        /// </summary>
        NoneActive = 1 << 7
    }
}
