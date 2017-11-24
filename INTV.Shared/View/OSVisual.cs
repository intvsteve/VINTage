// <copyright file="OSVisual.cs" company="INTV Funhouse">
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

#if WIN
using NativeVisual = System.Windows.DependencyObject;
using NativeVisualBase = System.Windows.DependencyObject;
#elif MAC
#if __UNIFIED__
using NativeVisual = AppKit.NSView;
using NativeVisualBase = AppKit.NSResponder;
#else
using NativeVisual = MonoMac.AppKit.NSView;
using NativeVisualBase = MonoMac.AppKit.NSResponder;
#endif // __UNIFIED__
#elif GTK
using NativeVisual = Gtk.Widget;
using NativeVisualBase = Gtk.Widget;
#endif // WIN

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial struct OSVisual
    {
        /// <summary>
        /// The canonical empty visual.
        /// </summary>
        public static readonly OSVisual Empty = new OSVisual((NativeVisualBase)null);

        private NativeVisualBase _visual;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.OSVisual"/> struct.
        /// </summary>
        /// <param name="visual">A platform-specific visual.</param>
        public OSVisual(NativeVisual visual)
        {
            _visual = visual;
        }

        /// <summary>
        /// Gets the native visual.
        /// </summary>
        public NativeVisualBase NativeVisual
        {
            get { return _visual; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty (<c>null</c>); otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return _visual == null; }
        }

        /// <summary>
        /// Creates an OSVisual from a platform-specific object.
        /// </summary>
        /// <param name="visual">The OS-specific object to place into the abstraction..</param>
        /// <returns>The abstraction, which could be empty if the supplied object isn't a visual.</returns>
        public static OSVisual FromObject(object visual)
        {
            return new OSVisual(visual as NativeVisual);
        }

        /// <summary>>Wraps a platform-specific visual in an abstracted visual.</summary>
        /// <param name="visual">A platform-specific visual to wrap in the abstraction.</param>
        /// <returns>The wrapped visual.</returns>
        public static implicit operator OSVisual(NativeVisual visual)
        {
            return new OSVisual(visual);
        }

        /// <summary>>Unwraps the native visual from a platform-abstract visual.</summary>
        /// <param name="visual">The abstracted visual to convert to a platform-specific object.</param>
        /// <returns>The native visual.</returns>
        public static implicit operator NativeVisual(OSVisual visual)
        {
            return visual._visual as NativeVisual;
        }

        /// <summary>
        /// Returns the native object as a specific type.
        /// </summary>
        /// <returns>The wrapped value as the requested type.</returns>
        /// <typeparam name="T">The type to cast the wrapped object to.</typeparam>
        public T AsType<T>() where T : class
        {
            return _visual as T;
        }
    }
}
