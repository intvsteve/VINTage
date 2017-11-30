// <copyright file="IProgramFeatureSet`T.cs" company="INTV Funhouse">
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

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#elif GTK
using INTV.Shared.Utility;
#endif // WIN

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Extends the IProgramFeatuerSet interface for generic (feature flags) types.
    /// </summary>
    /// <typeparam name="T">This should be an enumeration of feature flags.</typeparam>
    public interface IProgramFeatureSet<T> : IProgramFeatureSet where T : struct
    {
        /// <summary>
        /// Gets the user-friendly display name for a specific flag.
        /// </summary>
        /// <param name="featureBit">The specific feature whose name is desired.</param>
        /// <returns>The name of the feature bit.</returns>
        string GetFeatureName(T featureBit);

        /// <summary>
        /// Gets an image to display for a specific flag.
        /// </summary>
        /// <param name="featureBit">The specific feature whose icon is desired.</param>
        /// <returns>The icon for the feature bit.</returns>
        OSImage GetImageForFeature(T featureBit);

        /// <summary>
        /// Gets the image resource path for an icon describing a specific flag.
        /// </summary>
        /// <param name="featureBit">The specific feature whose image is desired.</param>
        /// <returns>The string resource path.</returns>
        string GetImageResourceForFeature(T featureBit);

        /// <summary>
        /// Gets image resource paths for all the bits set.
        /// </summary>
        /// <param name="featureBits">The features whose images are desired.</param>
        /// <returns>An enumerable containing the images that were found.</returns>
        IEnumerable<string> GetImageResourcesForFeatures(T featureBits);

        /// <summary>
        /// Gets the description of a feature bit.
        /// </summary>
        /// <param name="featureBit">The specific feature whose description is desired.</param>
        /// <returns>A descriptive string for given flag.</returns>
        string GetFeatureDescription(T featureBit);

        /// <summary>
        /// Gets descriptions for all the bits set.
        /// </summary>
        /// <param name="featureBits">The features whose images are desired.</param>
        /// <returns>An enumerable containing the descriptions for all flags set in the featureBits.</returns>
        IEnumerable<string> GetFeatureDescriptions(T featureBits);
    }
}
