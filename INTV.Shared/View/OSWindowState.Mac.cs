// <copyright file="OSWindowState.Mac.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    /// <summary>
    /// Wrapper enum for window state.
    /// </summary>
    public enum OSWindowState
    {
        /// <summary>
        /// Normal window state (not maximized, not minimized, not full-screen).
        /// </summary>
        Normal,

        /// <summary>
        /// Window is in the maximized state.
        /// </summary>
        Maximzed,

        /// <summary>
        /// Window is in the minimized state.
        /// </summary>
        Minimized
    }

    /// <summary>
    /// Helper methods for translation between the Cocoa-specific window state and the abstracted OSWindowState.
    /// </summary>
    public static class OSWindowStateHelpers
    {
        /// <summary>
        /// Given a window, retrieve its OSWindowState.
        /// </summary>
        /// <param name="window">The window whose state is desired.</param>
        /// <returns>The window state.</returns>
        public static OSWindowState WindowState(this MonoMac.AppKit.NSWindow window)
        {
            var state = OSWindowState.Normal;
            if (window.IsZoomed)
            {
                state = OSWindowState.Maximzed;
            }
            else if (window.IsMiniaturized)
            {
                state = OSWindowState.Minimized;
            }
            return state;
        }

        /// <summary>
        /// Given a view, retrieve its window state.
        /// </summary>
        /// <param name="view">The view whose owning window's state is desired.</param>
        /// <returns>The owning window's state.</returns>
        public static OSWindowState WindowState(this MonoMac.AppKit.NSView view)
        {
            return view.Window.WindowState();
        }
    }
}
