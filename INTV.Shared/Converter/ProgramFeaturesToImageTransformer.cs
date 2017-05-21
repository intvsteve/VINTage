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
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

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

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public ProgramFeaturesToImageTransformer(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Converts feature flags into an image containing glyphs.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static NSImage TransformToImage(ProgramDescriptionViewModel viewModel)
        {
            var features = viewModel.Features;
            var totWidth = System.Math.Max(0.0f, Padding * (features.Count - 1));
            var totHeight = 0.0f;
            foreach (var f in features)
            {
                totWidth += f.Image.Size.Width;
                totHeight = System.Math.Max(totHeight, f.Image.Size.Height);
            }
            var image = new NSImage(new System.Drawing.SizeF(totWidth, totHeight));
            image.LockFocus();
            var x = 0.0f;
            foreach (var f in features)
            {
                f.Image.Draw(new System.Drawing.PointF(x,0), System.Drawing.RectangleF.Empty, NSCompositingOperation.Copy, 1);
                x += Padding + f.Image.Size.Width;
            }
            image.UnlockFocus();
            return image;
        }

        /// <inheritdoc />
        public override NSObject TransformedValue (NSObject value)
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
