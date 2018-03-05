// <copyright file="DisplayResolutionViewModel.cs" company="INTV Funhouse">
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
using INTV.JzIntv.Model;
using INTV.Shared.ViewModel;

namespace INTV.JzIntvUI.ViewModel
{
    /// <summary>
    /// ViewModel for DisplayResolution.
    /// </summary>
	public class DisplayResolutionViewModel : OSViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="resolution">The value to represent.</param>
        public DisplayResolutionViewModel(DisplayResolution resolution)
        {
            Resolution = resolution;
            DisplayResolution = resolution.ToDisplayString();
        }

        /// <summary>
        /// Gets the represented value.
        /// </summary>
        public DisplayResolution Resolution { get; private set; }

        /// <summary>
        /// Gets the display name for the value.
        /// </summary>
        [INTV.Shared.Utility.OSExportAttribute("DisplayResolution")]
        public string DisplayResolution { get; private set; }
    }
}
