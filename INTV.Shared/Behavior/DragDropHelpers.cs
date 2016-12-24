// <copyright file="DragDropHelpers.cs" company="INTV Funhouse">
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

using System;
using System.Windows;

#if WIN
using OSVisualType = System.Windows.FrameworkElement;
#endif

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Data and methods to assist with Drag and Drop implementation.
    /// </summary>
    public static partial class DragDropHelpers
    {
        private static Point _lastMouseDown = new Point(double.NaN, double.NaN);
        private static bool _inDragOperation;

        /// <summary>
        /// Identifies the data format for enhancing drag/drop feedback.
        /// </summary>
        public static readonly string DragDropFeedbackDataFormat = "INTV.Shared.Behavior.IDragDropFeedback";

        /// <summary>
        /// Gets or sets the most recent visual in which a mouse down occurred that has enabled the 'DragStartBehavior'.
        /// </summary>
        public static OSVisualType LastMouseDownVisual { get; set; }

        /// <summary>
        /// Gets or sets the last mouse down position in screen (global) coordinates.
        /// </summary>
        public static Point LastMouseDown
        {
            get { return _lastMouseDown; }
            set { _lastMouseDown = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the application is executing a custom Drag and Drop operation.
        /// </summary>
        public static bool InDragOperation
        {
            get { return _inDragOperation; }
            set { _inDragOperation = value; }
        }

        /// <summary>
        /// Determines whether a drag operation should start based on the current mouse location in screen coordinates.
        /// </summary>
        /// <param name="currentMouseLocation">Current cursor position in screen coordinates.</param>
        /// <returns><c>true</c> if a Drag and Drop operation should start, <c>false</c> otherwise.</returns>
        /// <remarks>It is the caller's responsibility to check for whether the mouse is down, set the LastMouseDown value appropriately, and so forth.</remarks>
        public static bool DragStarted(Point currentMouseLocation)
        {
            bool dragStarted = (Math.Abs(currentMouseLocation.X - LastMouseDown.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                               (Math.Abs(currentMouseLocation.Y - LastMouseDown.Y) > SystemParameters.MinimumVerticalDragDistance);
            return dragStarted;
        }
    }
}
