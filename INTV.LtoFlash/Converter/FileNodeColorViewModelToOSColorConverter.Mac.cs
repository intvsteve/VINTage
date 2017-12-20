// <copyright file="FileNodeColorViewModelToOSColorConverter.Mac.cs" company="INTV Funhouse">
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

using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

#if __UNIFIED__
using nfloat = System.nfloat;
#else
using nfloat = System.Single;
#endif // __UNIFIED__

namespace INTV.LtoFlash.Converter
{
    /// <summary>
    /// Converts color to OS native format.
    /// </summary>
    [Register("FileNodeColorViewModelToOSColorConverter")]
    public class FileNodeColorViewModelToOSColorConverter : NSValueTransformer
    {
        private static readonly nfloat ColorEpsilon = (nfloat)0.0000001;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="INTV.LtoFlash.Converter.FileNodeColorViewModelToOSColorConverter"/> class.
        /// </summary>
        public FileNodeColorViewModelToOSColorConverter()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor to call on derived classes when the derived class has an [Export] constructor.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        public FileNodeColorViewModelToOSColorConverter(NSObjectFlag f)
            : base(f)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public FileNodeColorViewModelToOSColorConverter(System.IntPtr handle)
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
        public FileNodeColorViewModelToOSColorConverter(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }
#endif // !__UNIFIED__

        /// <inheritdoc />
        public override NSObject TransformedValue(NSObject value)
        {
            var newColor = INTV.Core.Model.Stic.Color.Black.ToColor();
            var viewModel = value as FileNodeColorViewModel;
            if (viewModel != null)
            {
                newColor = viewModel.Fill;
            }
            return newColor;
        }

        /// <inheritdoc />
        public override NSObject ReverseTransformedValue(NSObject value)
        {
            var osColor = value as NSColor;
            FileNodeColorViewModel colorViewModel = null;
            foreach (var color in INTV.LtoFlash.ViewModel.MenuLayoutViewModel.Colors)
            {
                var viewModelColor = FileNodeColorViewModel.GetColor(color);
                if (viewModelColor.Fill == osColor)
                {
                    colorViewModel = viewModelColor;
                    break;
                }
                else if (viewModelColor.Fill.ColorSpace == osColor.ColorSpace)
                {
                    nfloat r0, g0, b0, a0, r1, g1, b1, a1;
                    viewModelColor.Fill.GetRgba(out r0, out g0, out b0, out a0);
                    ////System.Diagnostics.Debug.WriteLine(r0 + "," + g0 + "," + b0 + ",e:" + ColorEpsilon);
                    osColor.GetRgba(out r1, out g1, out b1, out a1);
                    ////System.Diagnostics.Debug.WriteLine(r1 + "," + g1 + "," + b1);
                    if ((System.Math.Abs(r1 - r0) < ColorEpsilon * 2) && (System.Math.Abs(g1 - g0) < ColorEpsilon * 2) && (System.Math.Abs(b1 - b0) < ColorEpsilon * 2))
                    {
                        colorViewModel = viewModelColor;
                        break;
                    }
                }
            }
            return colorViewModel;
        }

        private void Initialize()
        {
        }
    }
}
