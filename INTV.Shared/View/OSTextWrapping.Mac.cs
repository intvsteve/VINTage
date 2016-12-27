// <copyright file="OSTextWrapping.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
#endif

namespace INTV.Shared.View
{
    /// <summary>
    /// Wraps the Cocoa enum for text wrapping.
    /// </summary>
    public enum OSTextWrapping
    {
        /// <summary>
        /// No line-wrapping is performed.
        /// </summary>
        NoWrap = NSLineBreakMode.Clipping,

        /// <summary>
        /// Wrap at words, but break words across lines if needed.
        /// </summary>
        Wrap = NSLineBreakMode.CharWrapping,

        /// <summary>
        /// Wrap at words, but if a natural break doesn't allow a line to fit, the line may overflow.
        /// </summary>
        WrapWithOverflow = NSLineBreakMode.ByWordWrapping
    }
}
