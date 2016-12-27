// <copyright file="KeyboardComponentFeatureSet.cs" company="INTV Funhouse">
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
    /// Describes the set of features for the original Model 1149 Keyboard Component.
    /// </summary>
    public class KeyboardComponentFeatureSet : ProgramFeatureSet<KeyboardComponentFeatures>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly KeyboardComponentFeatureSet Data = new KeyboardComponentFeatureSet();

        private static readonly Dictionary<KeyboardComponentFeatures, string> FeatureImages = new Dictionary<KeyboardComponentFeatures, string>()
        {
            { KeyboardComponentFeatures.Incompatible, "ViewModel/Resources/Images/kc_incompatible_16xLG.png" },
            { KeyboardComponentFeatures.Tolerates, null },
            { KeyboardComponentFeatures.Enhances, "ViewModel/Resources/Images/kc_enhanced_16xLG.png" },
            { KeyboardComponentFeatures.Requires, "ViewModel/Resources/Images/kc_16xLG.png" },
            { KeyboardComponentFeatures.TapeOptional, "ViewModel/Resources/Images/tape_enhanced_16xLG.png" },
            { KeyboardComponentFeatures.TapeRequired, "ViewModel/Resources/Images/tape_16xLG.png" },
            { KeyboardComponentFeatures.Microphone, "ViewModel/Resources/Images/microphone_16xLG.png" },
            { KeyboardComponentFeatures.BasicIncompatible, "ViewModel/Resources/Images/kc_basic_incompatible_16xLG.png" },
            { KeyboardComponentFeatures.BasicTolerated, null },
            { KeyboardComponentFeatures.BasicRequired, "ViewModel/Resources/Images/kc_basic_16xLG.png" },
            { KeyboardComponentFeatures.Printer, "ViewModel/Resources/Images/printer_16xLG.png" },
        };

        private static readonly Dictionary<KeyboardComponentFeatures, string> FeatureDescriptions = new Dictionary<KeyboardComponentFeatures, string>()
        {
            { KeyboardComponentFeatures.Incompatible, Resources.Strings.KeyboardComponentFeatures_Incompatible_Tip },
            { KeyboardComponentFeatures.Tolerates, null },
            { KeyboardComponentFeatures.Enhances, Resources.Strings.KeyboardComponentFeatures_Enhanced_Tip },
            { KeyboardComponentFeatures.Requires, Resources.Strings.KeyboardComponentFeatures_Required_Tip },
            { KeyboardComponentFeatures.TapeOptional, Resources.Strings.KeyboardComponentFeatures_TapeOptional_Tip },
            { KeyboardComponentFeatures.TapeRequired, Resources.Strings.KeyboardComponentFeatures_TapeRequired_Tip },
            { KeyboardComponentFeatures.Microphone, Resources.Strings.KeyboardComponentFeatures_Microphone_Tip },
            { KeyboardComponentFeatures.BasicIncompatible, Resources.Strings.KeyboardComponentFeatures_BasicIncompatible_Tip },
            { KeyboardComponentFeatures.BasicTolerated, null },
            { KeyboardComponentFeatures.BasicRequired, Resources.Strings.KeyboardComponentFeatures_BasicRequired_Tip },
            { KeyboardComponentFeatures.Printer, Resources.Strings.KeyboardComponentFeatures_Printer_Tip },
        };

        private static readonly Dictionary<KeyboardComponentFeatures, OSImage> ImageCache = new Dictionary<KeyboardComponentFeatures, OSImage>();

        private KeyboardComponentFeatureSet()
            : base(FeatureCategory.KeyboardComponent)
        {
        }

        #region ProgramFeatureSet Properties

        /// <inheritdoc />
        protected override Dictionary<KeyboardComponentFeatures, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<KeyboardComponentFeatures, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<KeyboardComponentFeatures, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
