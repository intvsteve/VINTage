// <copyright file="OSImage.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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

using NativeSize = Gdk.Size;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial struct OSImage
    {
        /// <summary>
        /// Gets the size of the image.
        /// </summary>
        public NativeSize Size
        {
            get { return new Gdk.Size(_image.Width, _image.Height); }
        }

        /// <summary>
        /// Creates a copy of the image with opacity applied to it.
        /// </summary>
        /// <param name="opacity">The opacity, in the range [0.0,1.0], where 0.0 is completely transparent, 1.0 is completely opaque.</param>
        /// <returns>A copy of the image with opacity applied to it.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="opacity"/> value was outside the range [0.0, 1.0].</exception>
        public OSImage CreateNewWithOpacity(double opacity)
        {
            if ((opacity < 0) || (opacity > 1))
            {
                throw new System.ArgumentOutOfRangeException("opacity", opacity, "value must be in the range 0.0 <= opacity <= 1.0");
            }
            var imageWithOpacity = Empty;
            if (!IsEmpty)
            {
                imageWithOpacity = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, _image.BitsPerSample, _image.Width, _image.Height);
                imageWithOpacity._image.Fill(0xFFFFFF00);
                _image.Composite(imageWithOpacity, 0, 0, _image.Width, _image.Height, 0, 0, 1, 1, Gdk.InterpType.Hyper, System.Convert.ToInt32(opacity * 255));
            }
            return imageWithOpacity;
        }
    }
}
