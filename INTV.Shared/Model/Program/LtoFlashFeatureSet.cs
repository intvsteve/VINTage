// <copyright file="LtoFlashFeatureSet.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Core.Model.Program;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif
#endif

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Describes the set of features for the LTO Flash!.
    /// </summary>
    public class LtoFlashFeatureSet : ProgramFeatureSet<LtoFlashFeatures>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly LtoFlashFeatureSet Data = new LtoFlashFeatureSet();

        private static readonly Dictionary<LtoFlashFeatures, string> FeatureImages = new Dictionary<LtoFlashFeatures, string>()
        {
            // { LtoFlashFeatures.Incompatible, "ViewModel/Resources/Images/lto_incompatible_9xSM.png" },
            { LtoFlashFeatures.Incompatible, null },
            { LtoFlashFeatures.Tolerates, null },
            { LtoFlashFeatures.Enhances, "ViewModel/Resources/Images/lto_enhanced_9xSM.png" },
            { LtoFlashFeatures.Requires, "ViewModel/Resources/Images/lto_9xSM.png" },
            { LtoFlashFeatures.SaveDataOptional, "ViewModel/Resources/Images/save_enhanced_16xLG.png" },
            { LtoFlashFeatures.SaveDataRequired, "ViewModel/Resources/Images/save_16xLG.png" },
            { LtoFlashFeatures.Bankswitching, "ViewModel/Resources/Images/bankswitch_16xMD.png" },
            { LtoFlashFeatures.SixteenBitRAM, "ViewModel/Resources/Images/ram_16_16xLG.png" },
            { LtoFlashFeatures.UsbPortEnhanced, "ViewModel/Resources/Images/usb_enhanced_16xLG.png" },
            { LtoFlashFeatures.UsbPortRequired, "ViewModel/Resources/Images/usb_required_16xLG.png" },
        };

        private static readonly Dictionary<LtoFlashFeatures, string> FeatureDescriptions = new Dictionary<LtoFlashFeatures, string>()
        {
            { LtoFlashFeatures.Incompatible, Resources.Strings.LtoFlashFeatures_Incompatible_Tip },
            { LtoFlashFeatures.Tolerates, null },
            { LtoFlashFeatures.Enhances, Resources.Strings.LtoFlashFeatures_Enhanced_Tip },
            { LtoFlashFeatures.Requires, Resources.Strings.LtoFlashFeatures_Required_Tip },
            { LtoFlashFeatures.SaveDataOptional, Resources.Strings.LtoFlashFeatures_SaveDataEnhanced_Tip },
            { LtoFlashFeatures.SaveDataRequired, Resources.Strings.LtoFlashFeatures_SaveDataRequired_Tip },
            { LtoFlashFeatures.Bankswitching, Resources.Strings.LtoFlashFeatures_Bankswitching_Tip },
            { LtoFlashFeatures.SixteenBitRAM, Resources.Strings.LtoFlashFeatures_Ram_Tip },
            { LtoFlashFeatures.UsbPortEnhanced, Resources.Strings.LtoFlashFeatures_UsbPortEnhanced_Tip },
            { LtoFlashFeatures.UsbPortRequired, Resources.Strings.LtoFlashFeatures_UsbPortRequired_Tip },
        };

        private static readonly Dictionary<LtoFlashFeatures, OSImage> ImageCache = new Dictionary<LtoFlashFeatures, OSImage>();

        private LtoFlashFeatureSet()
            : base(FeatureCategory.LtoFlash)
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
        protected override Dictionary<LtoFlashFeatures, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<LtoFlashFeatures, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<LtoFlashFeatures, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
