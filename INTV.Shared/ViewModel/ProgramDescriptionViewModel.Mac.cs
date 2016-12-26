// <copyright file="ProgramDescriptionViewModel.Mac.cs" company="INTV Funhouse">
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

using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class ProgramDescriptionViewModel : NSObject
    {
        /// <summary>
        /// Gets the tool tip for the features image.
        /// </summary>
        public string FeaturesTip
        {
            get
            {
                var tips = Features.Select(f => f.ToolTip).ToArray();
                var tip = string.Join("\n", tips);
                return tip;
            }
        }

        /// <summary>
        /// Gets the composd image to represent a ROM's features.
        /// </summary>
        [OSExportAttribute("Features")]
        public NSImage FeaturesImage
        {
            get { return INTV.Shared.Converter.ProgramFeaturesToImageTransformer.TransformToImage(this); }
        }

        /// <summary>
        /// Gets the file system path of the ROM's file.
        /// </summary>
        [OSExportAttribute("RomFile")]
        public string RomFile
        {
            get { return Rom.RomPath.SafeString(); }
        }
    }
}
