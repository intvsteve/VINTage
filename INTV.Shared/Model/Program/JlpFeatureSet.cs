// <copyright file="JlpFeatureSet.cs" company="INTV Funhouse">
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
    /// Describes the set of features for the JLP.
    /// </summary>
    public class JlpFeatureSet : ProgramFeatureSet<JlpFeatures>
    {
        /// <summary>
        /// The default number of sectors for JLP Flash save data, if configured.
        /// </summary>
        public const ushort DefaultSaveDataSectorCount = 4;

        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly JlpFeatureSet Data = new JlpFeatureSet();

        private static readonly Dictionary<JlpFeatures, string> FeatureImages = new Dictionary<JlpFeatures, string>()
        {
            // { JlpFeatures.Incompatible, "ViewModel/Resources/Images/jlp_incompatible_9xSM.png" },
            { JlpFeatures.Incompatible, null },
            { JlpFeatures.Tolerates, "ViewModel/Resources/Images/jlp_9xSM.png" }, // Initially enable RAM and accelerators, NO flash
            { JlpFeatures.Enhances, "ViewModel/Resources/Images/jlp_enhanced_9xSM.png" }, // Initially disable RAM and accelerators, flash optional
            { JlpFeatures.Requires, "ViewModel/Resources/Images/jlp_required_9xSM.png" }, // Initially enable RAM and Accelerators, flash optional
            { JlpFeatures.SaveDataOptional, "ViewModel/Resources/Images/save_enhanced_16xLG.png" },
            { JlpFeatures.SaveDataRequired, "ViewModel/Resources/Images/save_16xLG.png" },
            { JlpFeatures.Bankswitching, "ViewModel/Resources/Images/bankswitch_16xMD.png" },
            { JlpFeatures.SixteenBitRAM, "ViewModel/Resources/Images/ram_16_16xLG.png" },
            { JlpFeatures.SerialPortEnhanced, "ViewModel/Resources/Images/serialport_enhanced_16x16.png" },
            { JlpFeatures.SerialPortRequired, "ViewModel/Resources/Images/serialport_16x16.png" },
            { JlpFeatures.UsesLEDs, "Resources/Images/led_16xLG.png" },
        };

        private static readonly Dictionary<JlpFeatures, string> FeatureDescriptions = new Dictionary<JlpFeatures, string>()
        {
            { JlpFeatures.Incompatible, Resources.Strings.JlpFeatures_Incompatibility_Tip },
            { JlpFeatures.Tolerates, Resources.Strings.JlpFeatures_Tolerates_Tip },
            { JlpFeatures.Enhances, Resources.Strings.JlpFeatures_Enhanced_Tip },
            { JlpFeatures.Requires, Resources.Strings.JlpFeatures_Required_Tip },
            { JlpFeatures.SaveDataOptional, Resources.Strings.JlpFeatures_SaveDataEnhanced_Tip },
            { JlpFeatures.SaveDataRequired, Resources.Strings.JlpFeatures_SaveDataRequired_Tip },
            { JlpFeatures.Bankswitching, Resources.Strings.JlpFeatures_Bankswitching_Tip },
            { JlpFeatures.SixteenBitRAM, Resources.Strings.JlpFeatures_Ram_Tip },
            { JlpFeatures.SerialPortEnhanced, Resources.Strings.JlpFeatures_SerialPortEnhanced_Tip },
            { JlpFeatures.SerialPortRequired, Resources.Strings.JlpFeatures_SerialPortRequired_Tip },
            { JlpFeatures.UsesLEDs, Resources.Strings.JLPFeatures_UsesLEDsTip },
        };

        private static readonly Dictionary<JlpFeatures, OSImage> ImageCache = new Dictionary<JlpFeatures, OSImage>();

        private JlpFeatureSet()
            : base(FeatureCategory.Jlp)
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
        protected override Dictionary<JlpFeatures, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<JlpFeatures, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<JlpFeatures, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
