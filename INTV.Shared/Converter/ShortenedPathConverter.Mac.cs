// <copyright file="ShortenedPathConverter.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    [Register("ShortenedPathConverter")]
    public partial class ShortenedPathConverter : NSValueTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ShortenedPathConverter"/> class.
        /// </summary>
        public ShortenedPathConverter()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ShortenedPathConverter"/> class.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        /// <remarks>Constructor to call on derived classes when the derived class has an [Export] constructor.</remarks>
        public ShortenedPathConverter(NSObjectFlag f)
            : base(f)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ShortenedPathConverter"/> class.
        /// </summary>
        /// <param name="handle">Native object pointer.</param>
        /// <remarks>Called when created from unmanaged code.</remarks>
        public ShortenedPathConverter (System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ShortenedPathConverter"/> class.
        /// </summary>
        /// <param name="coder">The unarchiver.</param>
        /// <remarks>Called when created directly from a XIB file.</remarks>
        [Export ("initWithCoder:")]
        public ShortenedPathConverter(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <inheritdoc />
        public override NSObject TransformedValue (NSObject value)
        {
            var path = value as NSString;
            return new NSString(GetShortenedPath(path));
        }

        /// <inheritdoc />
        public override NSObject ReverseTransformedValue (NSObject value)
        {
            return null;
        }

        private void Initialize()
        {
        }
    }
}
