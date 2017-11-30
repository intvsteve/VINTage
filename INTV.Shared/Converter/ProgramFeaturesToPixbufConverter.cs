// <copyright file="ProgramFeaturesToPixbufConverter.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using INTV.Shared.ViewModel;

namespace INTV.Shared.Converter
{
    /// <summary>
    /// Provides a converter for program features to a Gdk.Pixbuf object.
    /// </summary>
    public class ProgramFeaturesToPixbufConverter
    {
        /// <summary>
        /// Padding (in pixels) to put around the image.
        /// </summary>
        public const int Padding = 2;

        /// <summary>
        /// Given a ProgramDescriptionViewModel, produce a single Gdk.Pixbuf that includes all of the program's feature icons.
        /// </summary>
        /// <param name="viewModel">View model of the program whose feature icons are to be rendered as a pixbuf.</param>
        /// <returns>A pixbuf containing the features.</returns>
        public static Gdk.Pixbuf ConvertToPixbuf(ProgramDescriptionViewModel viewModel)
        {
            // TODO: cache these so they don't need to be recreated all the time.
            Gdk.Pixbuf pixbuf = null;
            var features = viewModel.Features;
            var totWidth = System.Math.Max(0, Padding * (features.Count - 1));
            var totHeight = 0;
            foreach (var f in features)
            {
                totWidth += f.Image.Size.Width;
                totHeight = System.Math.Max(totHeight, f.Image.Size.Height);
            }
            if (totWidth > 0)
            {
                Gdk.Pixbuf image = features[0].Image;
                pixbuf = new Gdk.Pixbuf(image.Colorspace, image.HasAlpha, image.BitsPerSample, totWidth, totHeight);
                pixbuf.Fill(0xFFFFFF00); // fill with transparent white (??)
                var x = 0;
                foreach (var f in features)
                {
                    var imageSize = f.Image.Size;
                    ((Gdk.Pixbuf)f.Image).CopyArea(0, 0, f.Image.Size.Width, f.Image.Size.Height, pixbuf, x, 0);
                    x += Padding + f.Image.Size.Width;
                }
            }
            return pixbuf;
        }

        /// <summary>
        /// Gets the feature tooltip.
        /// </summary>
        /// <param name="viewModel">View model whose feature tooltip is desired.</param>
        /// <param name="x">The x coordinate within the cell.</param>
        /// <param name="y">The y coordinate within the cell.</param>
        /// <returns>The feature tooltip corresponding to the icon at the given coordinate, or <c>string.Empty</c> if none.</returns>
        public static string GetFeatureTooltip(ProgramDescriptionViewModel viewModel, int x, int y)
        {
            var tooltip = string.Empty;
            var features = viewModel.Features;
            var featureX = 0;
            var featureIndex = -1;
            var i = 0;
            foreach (var f in features)
            {
                if ((x >= featureX) && (x <= (featureX + f.Image.Size.Width + (Padding / 2))))
                {
                    featureIndex = i;
                    break;
                }
                else
                {
                    featureX += f.Image.Size.Width + Padding;
                    ++i;
                }
            }
            if (featureIndex >= 0)
            {
                tooltip = features[featureIndex].ToolTip;
            }
            return tooltip;
        }
    }
}
