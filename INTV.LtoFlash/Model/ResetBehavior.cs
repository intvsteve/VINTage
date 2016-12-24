// <copyright file="ResetBehavior.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Behavior to execute when resetting the console during game play.
    /// </summary>
    public enum ResetBehavior : byte
    {
        /// <summary>
        /// Reset to the LTO Flash! menu.
        /// </summary>
        Menu,

        /// <summary>
        /// Reset to the menu only if a specific key is pressed. Otherwise, reset to the current game.
        /// </summary>
        MenuIfKeyPressed,

        /// <summary>
        /// Always reset to the game.
        /// </summary>
        Game,

        /// <summary>
        /// The default value for this setting.
        /// </summary>
        Default = MenuIfKeyPressed
    }
}
