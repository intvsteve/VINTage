// <copyright file="ValidEmailAddressFormatter.Mac.cs" company="INTV Funhouse">
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
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Formatter used for checking email address format.
    /// </summary>
    [Register("ValidEmailAddressFormatter")]
    public class ValidEmailAddressFormatter : NSFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ValidEmailAddressFormatter"/> class.
        /// </summary>
        public ValidEmailAddressFormatter()
            : base()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ValidEmailAddressFormatter"/> class.
        /// </summary>
        /// <param name="coder">The unarchiver.</param>
        /// <remarks>Called when created directly from a XIB file.</remarks>
        [Export("initWithCoder:")]
        public ValidEmailAddressFormatter(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ValidEmailAddressFormatter"/> class.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        /// <remarks>Constructor to call on derived classes when the derived class has an [Export] constructor.</remarks>
        public ValidEmailAddressFormatter(NSObjectFlag t)
            : base(t)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ValidEmailAddressFormatter"/> class.
        /// </summary>
        /// <param name="handle">Native object pointer.</param>
        /// <remarks>Called when created from unmanaged code.</remarks>
        public ValidEmailAddressFormatter(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Callback used to validate string to have a valid email address format.
        /// </summary>
        public System.Action<string, bool> TextUpdateCallback { get; set; }

        private NSMutableAttributedString AttributedString { get; set; }

        private NSDictionary DefaultAttributesDictionary { get; set; }

        /// <inheritdoc/>
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

        /// <summary>
        /// The default implementation returns nil to indicate that the formatter object does not provide an attributed string.
        /// </summary>
        /// <param name="anObject">The object for which a textual representation is returned.</param>
        /// <param name="attributes">The default attributes to use for the returned attributed string.</param>
        /// <returns>An attributed string that represents anObject.</returns>
        [Export("attributedStringForObjectValue:withDefaultAttributes:")]
        public NSAttributedString AttributedStringForObjectValue(NSObject anObject, NSDictionary attributes)
        {
            DefaultAttributesDictionary = attributes;
            var theString = StringFor(anObject);
            var newAttributes = new NSMutableDictionary(attributes);
#if __UNIFIED__
            newAttributes[NSStringAttributeKey.ForegroundColor] = NSColor.Red;
#else
            newAttributes[NSAttributedString.ForegroundColorAttributeName] = NSColor.Red;
#endif // __UNIFIED__
            AttributedString = new NSMutableAttributedString(theString, newAttributes);
            return AttributedString;
        }

        /// <inheritdoc/>
        public override string EditingStringFor(NSObject value)
        {
            return base.EditingStringFor(value);
        }

        /// <summary>
        /// Determines whether a partial string valid.
        /// </summary>
        /// <param name="partialString">Partial string.</param>
        /// <param name="newString">New string.</param>
        /// <param name="errorDescription">Error description.</param>
        /// <returns><c>true</c> if the partial string valid.</returns>
        [Export("isPartialStringValid:newEditingString:errorDescription:")]
        public bool IsPartialStringValid(NSString partialString, ref NSString newString, ref NSError errorDescription)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this instance is partial string valid the specified partialString proposedSelectedRange
        /// originalString originalSelectedRange errorDescription.
        /// </summary>
        /// <returns><c>true</c> if this instance is partial string valid the specified partialString proposedSelectedRange
        /// originalString originalSelectedRange errorDescription; otherwise, <c>false</c>.</returns>
        /// <param name="partialString">Partial string.</param>
        /// <param name="proposedSelectedRange">Proposed selected range.</param>
        /// <param name="originalString">Original string.</param>
        /// <param name="originalSelectedRange">Original selected range.</param>
        /// <param name="errorDescription">Error description.</param>
        [Export("isPartialStringValid:proposedSelectedRange:originalString:originalSelectedRange:errorDescription:")]
        public bool IsPartialStringValid(ref NSString partialString, ref NSRange proposedSelectedRange, NSString originalString, NSRange originalSelectedRange, ref NSString errorDescription)
        {
            AttributedString.SetAttributes(DefaultAttributesDictionary, new NSRange(0, AttributedString.Length));
            var newAttributes = new NSMutableDictionary(DefaultAttributesDictionary);
#if __UNIFIED__
            newAttributes[NSStringAttributeKey.ForegroundColor] = NSColor.Red;
#else
            newAttributes[NSAttributedString.ForegroundColorAttributeName] = NSColor.Red;
#endif // __UNIFIED__
            var x = new NSMutableAttributedString(partialString, newAttributes);
            x.SetAttributes(newAttributes, new NSRange(0, partialString.Length));
            AttributedString.SetString(x);
            // AttributedString.AddAttribute((NSString)"NSForegroundColorAttributeName", NSColor.Red, new NSRange(0, theString.Length));
            if (TextUpdateCallback != null)
            {
                TextUpdateCallback(partialString, INTV.Shared.Behavior.ValidEmailAddressBehavior.IsValidEmailAddress(partialString));
            }
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
                System.Runtime.InteropServices.Marshal.WriteIntPtr(errorDescription, errorDescriptionObj.Handle);
            }
            return ret;
        }

        private bool ObjectFor(ref NSObject objectFor, string forString, bool needsErrorDescription, ref string errorDescription)
        {
            objectFor = AttributedString;
            return true;
        }

        private void Initialize()
        {
        }
    }
}
