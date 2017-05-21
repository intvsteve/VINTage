// <copyright file="SuperVideoArcadeFeatureSet.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Program;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Describes the set of features for the Sears Super Video Arcade.
    /// </summary>
    public class SuperVideoArcadeFeatureSet : ProgramFeatureSet<FeatureCompatibility>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly SuperVideoArcadeFeatureSet Data = new SuperVideoArcadeFeatureSet();

        private static readonly Dictionary<FeatureCompatibility, string> FeatureImages = new Dictionary<FeatureCompatibility, string>()
        {
            { FeatureCompatibility.Incompatible, "ViewModel/Resources/Images/sears_incompatible_16x16.png" },
            { FeatureCompatibility.Tolerates, null },
            { FeatureCompatibility.Enhances, "ViewModel/Resources/Images/sears_warning_16x16.png" },
            { FeatureCompatibility.Requires, "ViewModel/Resources/Images/sears_16x16.png" },
        };

        private static readonly Dictionary<FeatureCompatibility, string> FeatureDescriptions = new Dictionary<FeatureCompatibility, string>()
        {
            { FeatureCompatibility.Incompatible, Resources.Strings.SuperVideoArcadeCompatibility_Incompatible_Tip },
            { FeatureCompatibility.Tolerates, null },
            { FeatureCompatibility.Enhances, Resources.Strings.SuperVideoArcadeCompatibility_Enhanced_Tip },
            { FeatureCompatibility.Requires, Resources.Strings.SuperVideoArcadeCompatibility_Required_Tip },
        };

        private static readonly Dictionary<FeatureCompatibility, OSImage> ImageCache = new Dictionary<FeatureCompatibility, OSImage>();

        private SuperVideoArcadeFeatureSet()
            : base(FeatureCategory.SuperVideoArcade)
        {
        }

        #region ProgramFeatureSet Properties

        /// <inheritdoc />
        protected override Dictionary<FeatureCompatibility, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<FeatureCompatibility, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<FeatureCompatibility, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
