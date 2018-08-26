// <copyright file="CompositionHelpers.mac.cs" company="INTV Funhouse">
// Copyright (c) 2016-2018 All Rights Reserved
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

using System;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

#if __UNIFIED__
using Foundation;
using ObjCRuntime;
#else
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class CompositionHelpers
    {
        /// <summary>
        /// Mac-specific implementation.
        /// </summary>
        /// <param name="assembly">The assembly whose composition parts are to be initialized.</param>
        /// <param name="parts">The parts to initialize.</param>
        /// <remarks>When a composable part is found on Mac, and it contains visual implementations (xib/nib), at least in MonoMac it was not possible to actually
        /// create instances any of those types unless they were in the main application bundle. We dig out the resources and drop copies into the main bundle.
        /// Hack city! This technique is what would be necessary for true plugins to work, where the main application does not have direct dependencies on
        /// all of the other projects that implement various features (e.g. LTO Flash!, Intellicart, and jzIntv UI plugins).</remarks>
        static partial void OSInitializeParts(Assembly assembly, ComposablePartDefinition[] parts)
        {
            var mainBundle = NSBundle.MainBundle;
            if ((mainBundle != null) && !string.IsNullOrEmpty(mainBundle.ResourcePath))
            {
                // TODO: See how this need so be changed for Xamarin.Mac.
                Runtime.RegisterAssembly(assembly);
                var visualResourcesPrefix = "__monomac_content_";
                var allResourceNames = assembly.GetManifestResourceNames();
                var visualResources = allResourceNames.Where(r => r.StartsWith(visualResourcesPrefix, StringComparison.OrdinalIgnoreCase) && r.EndsWith(".nib", ignoreCase: true, culture: System.Globalization.CultureInfo.InvariantCulture)).ToList();
                foreach (var visualResource in visualResources)
                {
                    // NIB/XIB must just be name of the form name.xib -- no prefix from the resource. Just keep the last two parts.
                    var xibOrNibFileName = visualResource.Substring(visualResourcesPrefix.Length);
                    var xibOrNibTargetResourceFilePath = System.IO.Path.Combine(mainBundle.ResourcePath, xibOrNibFileName);
                    using (var resourceStream = assembly.GetManifestResourceStream(visualResource))
                    using (var fileStream = System.IO.File.Create(xibOrNibTargetResourceFilePath))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
        }
    }
}
