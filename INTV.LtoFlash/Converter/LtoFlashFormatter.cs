// <copyright file="LtoFlashFormatter.cs" company="INTV Funhouse">
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

namespace INTV.LtoFlash.Converter
{
    /// <summary>
    /// Formatter to use when entering text intended to display on an Intellivision console,
    /// typically in the LTO Flash! menu system.
    /// </summary>
    /// <remarks>NOTE: This class may not be used! It was intended to hook up via Interface Builder as a formatter to use when
    /// entering device name / owner name, and other text entry. Haven't figured out how to get the desired behavior using
    /// NSFormatter, so perhaps this whole thing should be deleted...</remarks>
    [Register("LtoFlashFormatter")]
    public class LtoFlashFormatter : NSFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Converter.LtoFlashFormatter"/> class.
        /// </summary>
        public LtoFlashFormatter()
            : base()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Converter.LtoFlashFormatter"/> class.
        /// </summary>
        /// <param name="coder">The unarchiver.</param>
        /// <remarks>Called when created directly from a XIB file.</remarks>
        [Export("initWithCoder:")]
        public LtoFlashFormatter(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Converter.LtoFlashFormatter"/> class.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        /// <remarks>Constructor to call on derived classes when the derived class has an [Export] constructor.</remarks>
        public LtoFlashFormatter(NSObjectFlag t)
            : base(t)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.Converter.LtoFlashFormatter"/> class.
        /// </summary>
        /// <param name="handle">Native object pointer.</param>
        /// <remarks>Called when created from unmanaged code.</remarks>
        public LtoFlashFormatter(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to restrict string to GROM characters.
        /// </summary>
        public bool RestrictToGromCharacters { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the string.
        /// </summary>
        public int MaxLength { get; set; }

        /// <inheritdoc />
        public override string StringFor(NSObject value)
        {
            var asString = string.Empty;
            var nsString = value as NSString;
            if (nsString != null)
            {
                asString = nsString;
            }
            return asString;
        }

        /// <inheritdoc />
        [Export("isPartialStringValid:newEditingString:errorDescription:")]
        public bool IsPartialStringValid(NSString partialString, ref NSString newString, ref NSError errorDescription)
        {
            return true;
        }

        /// <inheritdoc />
        [Export("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
        public bool IsPartialStringValid(ref NSString partialStringPtr, ref NSRange proposedSelectedRange, NSString originalString, NSRange originalSelectedRange, ref NSString errorDescription)
        {
            return true;
        }

        /// <summary>
        /// Converts a string to an object.
        /// </summary>
        /// <returns><c>true</c>, if the conversion was successful, <c>false</c> otherwise.</returns>
        /// <param name="objectFor">The object described by the string.</param>
        /// <param name="forString">String to parse to convert to an object.</param>
        /// <param name="errorDescription">Receives a descriptive error string.</param>
        /// <remarks>From a proposed (rejected) pull: https://github.com/mono/maccore/pull/25/files</remarks>
        [Export ("getObjectValue:forString:errorDescription:")]
        private bool ObjectFor(System.IntPtr objectFor, string forString, System.IntPtr errorDescription)
        {
            string errorDescriptionString = null;
            NSObject outObjectFor = null;

            bool ret = ObjectFor(ref outObjectFor, forString, errorDescription != System.IntPtr.Zero, ref errorDescriptionString);

            if (outObjectFor != null)
            {
                System.Runtime.InteropServices.Marshal.WriteIntPtr(objectFor, outObjectFor.Handle);
            }
            if (errorDescription != System.IntPtr.Zero && string.IsNullOrEmpty (errorDescriptionString) == false)
            {
                NSString errorDescriptionObj = new NSString (errorDescriptionString);
                System.Runtime.InteropServices.Marshal.WriteIntPtr (errorDescription, errorDescriptionObj.Handle);
            }
            return ret;
        }

        /// <inheritdoc />
        public bool ObjectFor(ref NSObject objectFor, string forString, bool needsErrorDescription, ref string errorDescription)
        {
            return true;
        }

        private void Initialize()
        {
        }
    }
}
