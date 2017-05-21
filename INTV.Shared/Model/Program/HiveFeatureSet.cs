// <copyright file="HiveFeatureSet.cs" company="INTV Funhouse">
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
    /// Describes the set of features for the Hive.
    /// </summary>
    public class HiveFeatureSet : ProgramFeatureSet<HiveFeatures>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly HiveFeatureSet Data = new HiveFeatureSet();

        private static readonly Dictionary<HiveFeatures, string> FeatureImages = new Dictionary<HiveFeatures, string>()
        {
            // { HiveFeatures.Incompatible, "ViewModel/Resources/Images/hive_incompatible_16xLG.png" },
            { HiveFeatures.Incompatible, null },
            { HiveFeatures.Tolerates, null },
            { HiveFeatures.Enhances, "ViewModel/Resources/Images/hive_enhanced_16xLG.png" },
            { HiveFeatures.Requires, "ViewModel/Resources/Images/hive_16xLG.png" },
            { HiveFeatures.SaveDataOptional, "ViewModel/Resources/Images/save_enhanced_16xLG.png" },
            { HiveFeatures.SaveDataRequired, "ViewModel/Resources/Images/save_16xLG.png" },
            { HiveFeatures.SixteenBitRAM, "ViewModel/Resources/Images/ram_16_16xLG.png" },
        };

        private static readonly Dictionary<HiveFeatures, string> FeatureDescriptions = new Dictionary<HiveFeatures, string>()
        {
            { HiveFeatures.Incompatible, Resources.Strings.HiveFeatures_Incompatible_Tip },
            { HiveFeatures.Tolerates, null },
            { HiveFeatures.Enhances, Resources.Strings.HiveFeatures_Enhanced_Tip },
            { HiveFeatures.Requires, Resources.Strings.HiveFeatures_Required_Tip },
            { HiveFeatures.SaveDataOptional, Resources.Strings.HiveFeatures_SaveDataOptional_Tip },
            { HiveFeatures.SaveDataRequired, Resources.Strings.HiveFeatures_SaveDataRequired_Tip },
            { HiveFeatures.SixteenBitRAM, Resources.Strings.HiveFeatures_Ram_Tip },
        };

        private static readonly Dictionary<HiveFeatures, OSImage> ImageCache = new Dictionary<HiveFeatures, OSImage>();

        private HiveFeatureSet()
            : base(FeatureCategory.Hive)
        {
        }

        #region IProgramFeatureSet Properties

        /// <inheritdoc />
        public override bool ExtendedFeaturesRequireEnhancedFlag
        {
            get { return false; }
        }

        #endregion // IProgramFeatureSet Properties

        #region ProgramFeatureSet Properties

        /// <inheritdoc />
        protected override Dictionary<HiveFeatures, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<HiveFeatures, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<HiveFeatures, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
