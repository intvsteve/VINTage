// <copyright file="NSUrlStringConverter.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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
using INTV.Shared.Utility;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Converter for displaying paths in a path control when the backing value is a string.
    /// Accounts for the string being either in URI or plain old path format.
    /// </summary>
    [Register("NSUrlStringConverter")]
    public class NSUrlStringConverter : NSValueTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.NSUrlStringConverter"/> class.
        /// </summary>
        public NSUrlStringConverter()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.NSUrlStringConverter"/> class.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        /// <remarks>Constructor to call on derived classes when the derived class has an [Export] constructor.</remarks>
        public NSUrlStringConverter(NSObjectFlag f)
            : base(f)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.NSUrlStringConverter"/> class.
        /// </summary>
        /// <param name="handle">Native object pointer.</param>
        /// <remarks>Called when created from unmanaged code.</remarks>
        public NSUrlStringConverter (System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

#if !__UNIFIED__
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.NSUrlStringConverter"/> class.
        /// </summary>
        /// <param name="coder">The unarchiver.</param>
        /// <remarks>Called when created directly from a XIB file.
        /// NOTE: Xamarin.Mac propery does not provide this constructor, as NSValueTransformer does not conform to NSCoding.</remarks>
        [Export ("initWithCoder:")]
        public NSUrlStringConverter(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }
#endif // !__UNIFIED__

        /// <inheritdoc />
        public override NSObject TransformedValue(NSObject value)
        {
            NSUrl pathUrl = null;
            var path = value as NSString;
            if (path != null)
            {
                if (((string)path).IsFileUrlAsString())
                {
                    pathUrl = NSUrl.FromString(path);
                }
                else
                {
                    pathUrl = NSUrl.FromFilename(path);
                }
            }
            return pathUrl;
        }

        /// <inheritdoc />
        public override NSObject ReverseTransformedValue(NSObject value)
        {
            NSString path = null;
            var pathUrl = value as NSUrl;
            if (pathUrl != null)
            {
                path = new NSString(pathUrl.AbsoluteString);
            }
            return path;
        }

        private void Initialize()
        {
        }
    }
}
