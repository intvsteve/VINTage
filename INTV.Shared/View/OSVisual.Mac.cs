// <copyright file="OSVisual.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial struct OSVisual
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.OSVisual"/> struct.
        /// </summary>
        /// <param name="visual">A window.</param>
        public OSVisual(NSWindow window)
        {
            _visual = window;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.OSVisual"/> struct.
        /// </summary>
        /// <param name="visual">A responder.</param>
        public OSVisual(NSResponder responder)
        {
            _visual = responder;
        }

        /// <summary>
        /// Creates an OSVisual from a platform-specific object.
        /// </summary>
        /// <param name="window">The OS-specific object to place into the abstraction..</param>
        /// <returns>The abstraction.</returns>
        public static OSVisual FromObject(NSWindow window)
        {
            return new OSVisual(window);
        }

        /// <summary>
        /// Creates an OSVisual from a platform-specific object.
        /// </summary>
        /// <param name="responder">The OS-specific object to place into the abstraction..</param>
        /// <returns>The abstraction.</returns>
        public static OSVisual FromObject(NSResponder responder)
        {
            return new OSVisual(responder);
        }

        /// <summary>>Wraps a NSWindow in an abstracted visual.</summary>
        /// <param name="color">A NSWindow to wrap in the abstraction.</param>
        /// <returns>The wrapped NSWindow.</returns>
        public static implicit operator OSVisual(NSWindow window)
        {
            return new OSVisual(window);
        }

        /// <summary>>Wraps a NSResponder in an abstracted visual.</summary>
        /// <param name="color">A NSResponder to wrap in the abstraction.</param>
        /// <returns>The wrapped NSResponder.</returns>
        public static implicit operator OSVisual(NSResponder responder)
        {
            return new OSVisual(responder);
        }

        /// <summary>>Unwraps the NSWindow from a platform-abstract visual.</summary>
        /// <param name="color">The abstracted NSWindow to convert to a platform-specific object.</param>
        /// <returns>The native NSWindow.</returns>
        public static implicit operator NSWindow(OSVisual visual)
        {
            return visual._visual as NSWindow;
        }

        /// <summary>>Unwraps the NSResponder from a platform-abstract visual.</summary>
        /// <param name="color">The abstracted NSResponder to convert to a platform-specific object.</param>
        /// <returns>The native NSResponder.</returns>
        public static implicit operator NSResponder(OSVisual visual)
        {
            return visual._visual as NSResponder;
        }
    }
}
