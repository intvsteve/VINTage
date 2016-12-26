// <copyright file="FileNodeSupportFilePathOrFolderInfoTransformer.cs" company="INTV Funhouse">
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
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.LtoFlash.ViewModel;

namespace INTV.LtoFlash
{
    [Register("FileNodeSupportFilePathOrFolderInfoTransformer")]
    public class FileNodeSupportFilePathOrFolderInfoTransformer : NSValueTransformer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.FileNodeSupportFilePathOrFolderInfoTransformer"/> class.
        /// </summary>
        public FileNodeSupportFilePathOrFolderInfoTransformer()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor to call on derived classes when the derived class has an [Export] constructor.
        /// </summary>
        /// <param name="f">Flags used by MonoMac.</param>
        public FileNodeSupportFilePathOrFolderInfoTransformer(NSObjectFlag f)
            : base(f)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public FileNodeSupportFilePathOrFolderInfoTransformer(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export ("initWithCoder:")]
        public FileNodeSupportFilePathOrFolderInfoTransformer(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <inheritdoc />
        public override NSObject TransformedValue (NSObject value)
        {
            // var viewModel = value as FileNodeViewModel;

            return null;
        }

        /// <inheritdoc />
        public override NSObject ReverseTransformedValue (NSObject value)
        {
            var osColor = value as NSColor;
            FileNodeColorViewModel colorViewModel = null;
            // var intvColor = INTV.Core.Model.Stic.Color.NotAColor;
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
                    float r0, g0, b0, a0, r1, g1, b1, a1;
                    viewModelColor.Fill.GetRgba(out r0, out g0, out b0, out a0);
                    osColor.GetRgba(out r1, out g1, out b1, out a1);
                    if ((System.Math.Abs(r1 - r0) < float.Epsilon * 2) && (System.Math.Abs(g1 - g0) < float.Epsilon * 2) && (System.Math.Abs(b1 - b0) < float.Epsilon * 2))
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
