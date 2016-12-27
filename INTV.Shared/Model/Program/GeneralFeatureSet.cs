// <copyright file="GeneralFeatureSet.cs" company="INTV Funhouse">
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
    /// Describes general features.
    /// </summary>
    public class GeneralFeatureSet : ProgramFeatureSet<GeneralFeatures>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly GeneralFeatureSet Data = new GeneralFeatureSet();

        private static readonly Dictionary<GeneralFeatures, string> FeatureImages = new Dictionary<GeneralFeatures, string>()
        {
            { GeneralFeatures.None, null },
            { GeneralFeatures.UnrecognizedRom, "ViewModel/Resources/Images/unrecognized_rom_16xLG.png" },
            { GeneralFeatures.PageFlipping, "ViewModel/Resources/Images/bankswitch_mattel_16XLG.png" },
            { GeneralFeatures.OnboardRam, "ViewModel/Resources/Images/ram_08_16xLG.png" },
            { GeneralFeatures.SystemRom, null }
        };

        private static readonly Dictionary<GeneralFeatures, string> FeatureDescriptions = new Dictionary<GeneralFeatures, string>()
        {
            { GeneralFeatures.None, null },
            { GeneralFeatures.UnrecognizedRom, Resources.Strings.GeneralFeatures_UnrecognizedRom_Tip },
            { GeneralFeatures.PageFlipping, Resources.Strings.GeneralFeatures_UsesPageFlipping_Tip },
            { GeneralFeatures.OnboardRam, Resources.Strings.GeneralFeatures_OnboardRam_Tip }
        };

        private static readonly Dictionary<GeneralFeatures, OSImage> ImageCache = new Dictionary<GeneralFeatures, OSImage>();

        private GeneralFeatureSet()
            : base(FeatureCategory.General)
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
        protected override Dictionary<GeneralFeatures, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<GeneralFeatures, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<GeneralFeatures, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
