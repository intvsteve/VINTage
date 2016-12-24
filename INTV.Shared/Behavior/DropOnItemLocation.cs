// <copyright file="DropOnItemLocation.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// These values describe the sub-area within a drop target that the operation is currently over. This can
    /// be used to define specific behaviors for drop actions, such as inserting above or below.
    /// </summary>
    public enum DropOnItemLocation
    {
        /// <summary>
        /// Drag operation is not over an allowed target, or the sub-region within the target is indeterminate.
        /// </summary>
        None,

        /// <summary>
        /// Drag operation is over the top quarter of the target's visual, as determined by the drop target's ActualHeight.
        /// </summary>
        TopQuarter,

        /// <summary>
        /// Drag operation is over the lower quarter of the top half of target's visual, as determined by the drop target's ActualHeight.
        /// </summary>
        TopMiddleQuarter,

        /// <summary>
        /// Drag operation is over the top quarter lower half of the target's visual, as determined by the drop target's ActualHeight.
        /// </summary>
        BottomMiddleQuarter,

        /// <summary>
        /// Drag operation is over the bottom quarter of the target's visual, as determined by the drop target's ActualHeight.
        /// </summary>
        BottomQuarter
    }
}
