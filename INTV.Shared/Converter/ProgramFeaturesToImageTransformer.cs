// <copyright file="ProgramFeaturesToImageTransformer.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

#if __UNIFIED__
using CGPoint = CoreGraphics.CGPoint;
using CGRect = CoreGraphics.CGRect;
using CGSize = CoreGraphics.CGSize;
#else
using CGPoint = System.Drawing.PointF;
using CGRect = System.Drawing.RectangleF;
using CGSize = System.Drawing.SizeF;
#endif // __UNIFIED__

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Transforms model ProgramFeatures data to NSImage.
    /// </summary>
    [Register("ProgramFeaturesToImageTransformer")]
    public class ProgramFeaturesToImageTransformer : NSValueTransformer
    {
        /// <summary>
        /// Padding (in pixels) to put around the image.
        /// </summary>
        public const float Padding = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Converter.ProgramFeaturesToImageTransformer"/> class.
        /// </summary>
        public ProgramFeaturesToImageTransformer()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor to call on derived classes when the derived class has an [Export] constructor.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        public ProgramFeaturesToImageTransformer(NSObjectFlag f)
            : base(f)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ProgramFeaturesToImageTransformer(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

#if !__UNIFIED__
        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        /// <remarks>Called when created directly from a XIB file.
        /// NOTE: Xamarin.Mac propery does not provide this constructor, as NSValueTransformer does not conform to NSCoding.</remarks>
        [Export("initWithCoder:")]
        public ProgramFeaturesToImageTransformer(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }
#endif // !__UNIFIED__

        /// <summary>
        /// Converts feature flags into an image containing glyphs.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>An image that represents feature flags.</returns>
        public static NSImage TransformToImage(ProgramDescriptionViewModel viewModel)
        {
            var features = viewModel.Features;
            var totWidth = System.Math.Max(0.0f, Padding * (features.Count - 1));
            var totHeight = 0.0f;
            foreach (var f in features)
            {
                totWidth += (float)f.Image.Size.Width;
                totHeight = System.Math.Max(totHeight, (float)f.Image.Size.Height);
            }
            var image = new NSImage(new CGSize(totWidth, totHeight));
            image.LockFocus();
            var x = 0.0f;
            foreach (var f in features)
            {
                f.Image.Draw(new CGPoint(x, 0), CGRect.Empty, NSCompositingOperation.Copy, 1);
                x += (float)Padding + (float)f.Image.Size.Width;
            }
            image.UnlockFocus();
            return image;
        }

        /// <inheritdoc />
        public override NSObject TransformedValue(NSObject value)
        {
            NSImage image = null;
            var viewModel = value as ProgramDescriptionViewModel;
            if (viewModel != null)
            {
                image = TransformToImage(viewModel);
            }
            else
            {
                image = new NSImage();
            }
            return image;
        }

        /// <inheritdoc />
        public override NSObject ReverseTransformedValue(NSObject value)
        {
            return null;
        }

        private void Initialize()
        {
        }
    }
}
