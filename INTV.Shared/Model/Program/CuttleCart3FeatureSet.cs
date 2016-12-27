// <copyright file="CuttleCart3FeatureSet.cs" company="INTV Funhouse">
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
    /// Describes the set of features for the CuttleCart 3.
    /// </summary>
    public class CuttleCart3FeatureSet : ProgramFeatureSet<CuttleCart3Features>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly CuttleCart3FeatureSet Data = new CuttleCart3FeatureSet();

        private static readonly Dictionary<CuttleCart3Features, string> FeatureImages = new Dictionary<CuttleCart3Features, string>()
        {
            { CuttleCart3Features.Incompatible, "ViewModel/Resources/Images/cuttlecart3_incompatible_16xLG.png" },
            { CuttleCart3Features.Tolerates, null },
            { CuttleCart3Features.Enhances, "ViewModel/Resources/Images/cuttlecart3_enhanced_16xLG.png" },
            { CuttleCart3Features.Requires, "ViewModel/Resources/Images/cuttlecart3_16xLG.png" },
            { CuttleCart3Features.Bankswitching, "ViewModel/Resources/Images/bankswitch_16xMD.png" },
            { CuttleCart3Features.MattelBankswitching, "ViewModel/Resources/Images/bankswitch_mattel_16XLG.png" },
            { CuttleCart3Features.SixteenBitRAM, "ViewModel/Resources/Images/ram_16_16xLG.png" },
            { CuttleCart3Features.EightBitRAM, "ViewModel/Resources/Images/ram_08_16xLG.png" },
            { CuttleCart3Features.SerialPortEnhanced, "ViewModel/Resources/Images/serialport_enhanced_16x16.png" },
            { CuttleCart3Features.SerialPortRequired, "ViewModel/Resources/Images/serialport_16x16.png" },
        };

        private static readonly Dictionary<CuttleCart3Features, string> FeatureDescriptions = new Dictionary<CuttleCart3Features, string>()
        {
            { CuttleCart3Features.Incompatible, Resources.Strings.CuttleCart3Features_Incompatible_Tip },
            { CuttleCart3Features.Tolerates, null },
            { CuttleCart3Features.Enhances, Resources.Strings.CuttleCart3Features_Enhanced_Tip },
            { CuttleCart3Features.Requires, Resources.Strings.CuttleCart3Features_Required_Tip },
            { CuttleCart3Features.Bankswitching, Resources.Strings.CuttleCart3Features_Bankswitching_Tip },
            { CuttleCart3Features.MattelBankswitching, Resources.Strings.CuttleCart3Features_MattelBankswitching_Tip },
            { CuttleCart3Features.SixteenBitRAM, Resources.Strings.CuttleCart3Features_Ram16_Tip },
            { CuttleCart3Features.EightBitRAM, Resources.Strings.CuttleCart3Features_Ram08_Tip },
            { CuttleCart3Features.SerialPortEnhanced, Resources.Strings.CuttleCart3Features_SerialPortEnhanced_Tip },
            { CuttleCart3Features.SerialPortRequired, Resources.Strings.CuttleCart3Features_SerialPortRequired_Tip },
        };

        private static readonly Dictionary<CuttleCart3Features, OSImage> ImageCache = new Dictionary<CuttleCart3Features, OSImage>();

        private CuttleCart3FeatureSet()
            : base(FeatureCategory.CuttleCart3)
        {
        }

        #region ProgramFeatureSet Properties

        /// <inheritdoc />
        protected override Dictionary<CuttleCart3Features, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<CuttleCart3Features, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<CuttleCart3Features, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
