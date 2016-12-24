// <copyright file="EcsFeatureSet.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using OSImage = MonoMac.AppKit.NSImage;
#endif

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// Describes the set of features for the ECS.
    /// </summary>
    public class EcsFeatureSet : ProgramFeatureSet<EcsFeatures>
    {
        /// <summary>
        /// The instance of the feature set.
        /// </summary>
        public static readonly EcsFeatureSet Data = new EcsFeatureSet();

        private static readonly Dictionary<EcsFeatures, string> FeatureImages = new Dictionary<EcsFeatures, string>()
        {
            { EcsFeatures.Incompatible, "ViewModel/Resources/Images/ecs_incompatible_16xLG.png" },
            { EcsFeatures.Tolerates, null },
            { EcsFeatures.Enhances, "ViewModel/Resources/Images/ecs_enhanced_16xLG.png" },
            { EcsFeatures.Requires, "ViewModel/Resources/Images/ecs_16xLG.png" },
            { EcsFeatures.Synthesizer, "ViewModel/Resources/Images/synth_16xLG.png" },
            { EcsFeatures.Tape, "ViewModel/Resources/Images/tape_16xLG.png" },
            { EcsFeatures.Printer, "ViewModel/Resources/Images/printer_16xLG.png" },
            { EcsFeatures.SerialPortEnhanced, "ViewModel/Resources/Images/serialport_enhanced_16x16.png" },
            { EcsFeatures.SerialPortRequired, "ViewModel/Resources/Images/serialport_16x16.png" },
        };

        private static readonly Dictionary<EcsFeatures, string> FeatureDescriptions = new Dictionary<EcsFeatures, string>()
        {
            { EcsFeatures.Incompatible, Resources.Strings.EcsFeatures_Incompatible_Tip },
            { EcsFeatures.Tolerates, null },
            { EcsFeatures.Enhances, Resources.Strings.EcsFeatures_Enhanced_Tip },
            { EcsFeatures.Requires, Resources.Strings.EcsFeatures_Required_Tip },
            { EcsFeatures.Synthesizer, Resources.Strings.EcsFeatures_Synthesizer_Tip },
            { EcsFeatures.Tape, Resources.Strings.EcsFeatures_Tape_Tip },
            { EcsFeatures.Printer, Resources.Strings.EcsFeatures_Printer_Tip },
            { EcsFeatures.SerialPortEnhanced, Resources.Strings.EcsFeatures_SerialPortEnhanced_Tip },
            { EcsFeatures.SerialPortRequired, Resources.Strings.EcsFeatures_SerialPortRequired_Tip },
        };

        private static readonly Dictionary<EcsFeatures, OSImage> ImageCache = new Dictionary<EcsFeatures, OSImage>();

        private EcsFeatureSet()
            : base(FeatureCategory.Ecs)
        {
        }

        #region ProgramFeatureSet Properties

        /// <inheritdoc />
        protected override Dictionary<EcsFeatures, string> ImageResources
        {
            get { return FeatureImages; }
        }

        /// <inheritdoc />
        protected override Dictionary<EcsFeatures, string> Descriptions
        {
            get { return FeatureDescriptions; }
        }

        /// <inheritdoc />
        protected override Dictionary<EcsFeatures, OSImage> Images
        {
            get { return ImageCache; }
        }

        #endregion // ProgramFeatureSet Properties
    }
}
