// <copyright file="IntellicartFeatureSet.cs" company="INTV Funhouse">
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
    /// Describes the set of features for the Intellicart.
    /// </summary>
    public class IntellicartFeatureSet : ProgramFeatureSet<IntellicartCC3Features>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly IntellicartFeatureSet Data = new IntellicartFeatureSet();

        private static readonly Dictionary<IntellicartCC3Features, string> FeatureImages = new Dictionary<IntellicartCC3Features, string>()
        {
            { IntellicartCC3Features.Incompatible, "ViewModel/Resources/Images/intellicart_incompatible_16xLG.png" },
            { IntellicartCC3Features.Tolerates, null },
            { IntellicartCC3Features.Enhances, "ViewModel/Resources/Images/intellicart_enhanced_16xLG.png" },
            { IntellicartCC3Features.Requires, "ViewModel/Resources/Images/intellicart_16xLG.png" },
            { IntellicartCC3Features.Bankswitching, "ViewModel/Resources/Images/bankswitch_16xMD.png" },
            { IntellicartCC3Features.SixteenBitRAM, "ViewModel/Resources/Images/ram_16_16xLG.png" },
            { IntellicartCC3Features.SerialPortEnhanced, "ViewModel/Resources/Images/serialport_enhanced_16x16.png" },
            { IntellicartCC3Features.SerialPortRequired, "ViewModel/Resources/Images/serialport_16x16.png" },
        };

        private static readonly Dictionary<IntellicartCC3Features, string> FeatureDescriptions = new Dictionary<IntellicartCC3Features, string>()
        {
            { IntellicartCC3Features.Incompatible, Resources.Strings.IntellicartFeatures_Incompatible_Tip },
            { IntellicartCC3Features.Tolerates, null },
            { IntellicartCC3Features.Enhances, Resources.Strings.IntellicartFeatures_Enhanced_Tip },
            { IntellicartCC3Features.Requires, Resources.Strings.IntellicartFeatures_Required_Tip },
            { IntellicartCC3Features.Bankswitching, Resources.Strings.IntellicartFeatures_Bankswitching_Tip },
            { IntellicartCC3Features.SixteenBitRAM, Resources.Strings.IntellicartFeatures_Ram_Tip },
            { IntellicartCC3Features.SerialPortEnhanced, Resources.Strings.IntellicartFeatures_SerialPortEnhanced_Tip },
            { IntellicartCC3Features.SerialPortRequired, Resources.Strings.IntellicartFeatures_SerialPortRequired_Tip },
        };

        private static readonly Dictionary<IntellicartCC3Features, OSImage> ImageCache = new Dictionary<IntellicartCC3Features, OSImage>();

        private IntellicartFeatureSet()
            : base(FeatureCategory.Intellicart)
        {
        }

        #region ProgramFeatureSet Properties

        /// <inheritdoc />
        protected override Dictionary<IntellicartCC3Features, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<IntellicartCC3Features, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<IntellicartCC3Features, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
