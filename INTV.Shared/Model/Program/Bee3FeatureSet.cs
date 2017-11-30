// <copyright file="Bee3FeatureSet.cs" company="INTV Funhouse">
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
#elif GTK
using INTV.Shared.Utility;
#endif // WIN

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Describes the set of features for Bee3.
    /// </summary>
    public class Bee3FeatureSet : ProgramFeatureSet<Bee3Features>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly Bee3FeatureSet Data = new Bee3FeatureSet();

        private static readonly Dictionary<Bee3Features, string> FeatureImages = new Dictionary<Bee3Features, string>()
        {
            // { Bee3Features.Incompatible, "ViewModel/Resources/Images/bee3_incompatible_16xLG.png" },
            { Bee3Features.Incompatible, null },
            { Bee3Features.Tolerates, null },
            { Bee3Features.Enhances, "ViewModel/Resources/Images/bee3_enhanced_16xLG.png" },
            { Bee3Features.Requires, "ViewModel/Resources/Images/bee3_16xLG.png" },
            { Bee3Features.SaveDataOptional, "ViewModel/Resources/Images/save_enhanced_16xLG.png" },
            { Bee3Features.SaveDataRequired, "ViewModel/Resources/Images/save_16xLG.png" },
            { Bee3Features.SixteenBitRAM, "ViewModel/Resources/Images/ram_16_16xLG.png" },
        };

        private static readonly Dictionary<Bee3Features, string> FeatureDescriptions = new Dictionary<Bee3Features, string>()
        {
            { Bee3Features.Incompatible, Resources.Strings.Bee3Features_Incompatible_Tip },
            { Bee3Features.Tolerates, null },
            { Bee3Features.Enhances, Resources.Strings.Bee3Features_Enhanced_Tip },
            { Bee3Features.Requires, Resources.Strings.Bee3Features_Required_Tip },
            { Bee3Features.SaveDataOptional, Resources.Strings.Bee3Features_SaveDataOptional_Tip },
            { Bee3Features.SaveDataRequired, Resources.Strings.Bee3Features_SaveDataRequired_Tip },
            { Bee3Features.SixteenBitRAM, Resources.Strings.Bee3Features_Ram_Tip },
        };

        private static readonly Dictionary<Bee3Features, OSImage> ImageCache = new Dictionary<Bee3Features, OSImage>();

        private Bee3FeatureSet()
            : base(FeatureCategory.Bee3)
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
        protected override Dictionary<Bee3Features, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<Bee3Features, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<Bee3Features, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
