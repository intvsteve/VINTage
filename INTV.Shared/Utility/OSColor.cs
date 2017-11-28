// <copyright file="OSColor.cs" company="INTV Funhouse">
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
using NativeColor = System.Windows.Media.Color;
#elif MAC
#if __UNIFIED__
using NativeColor = AppKit.NSColor;
#else
using NativeColor = MonoMac.AppKit.NSColor;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Simple wrapper for native Color type.
    /// </summary>
    public partial struct OSColor
    {
        private NativeColor _color;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.OSColor"/> struct.
        /// </summary>
        /// <param name="color">A platform-specific color object.</param>
        public OSColor(NativeColor color)
        {
            _color = color;
        }

        /// <summary>Tests two colors for equality.</summary>
        /// <param name="lhs">A color to compare.</param>
        /// <param name="rhs">Another color to compare.</param>
        /// <returns><c>true</c> if the colors are equal.</returns>
        public static bool operator ==(OSColor lhs, OSColor rhs)
        {
            return object.Equals(lhs._color, rhs._color);
        }

        /// <summary>Tests two colors for inequality.</summary>
        /// <param name="lhs">A color to compare.</param>
        /// <param name="rhs">Another color to compare.</param>
        /// <returns><c>true</c> if the colors are not equal.</returns>
        public static bool operator !=(OSColor lhs, OSColor rhs)
        {
            return !object.Equals(lhs._color, rhs._color);
        }

        /// <summary>>Wraps a platform-specific color in an abstracted color.</summary>
        /// <param name="color">A platform-specific color to wrap in the abstraction.</param>
        /// <returns>The wrapped color.</returns>
        public static implicit operator OSColor(NativeColor color)
        {
            return new OSColor(color);
        }

        /// <summary>>Unwraps the native color from a platform-abstract color.</summary>
        /// <param name="color">The abstracted color to convert to a platform-specific object.</param>
        /// <returns>The native color.</returns>
        public static implicit operator NativeColor(OSColor color)
        {
            return color._color;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _color.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is OSColor)
            {
                return object.Equals(_color, ((OSColor)obj)._color);
            }
            else if (obj is NativeColor)
            {
                return object.Equals(_color, (NativeColor)obj);
            }
            return false;
        }
    }
}
