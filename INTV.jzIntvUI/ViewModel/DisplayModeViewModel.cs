// <copyright file="DisplayModeViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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

#if WIN
using BaseClass = System.Object;
#elif MAC
#if __UNIFIED__
using BaseClass = Foundation.NSObject;
#else
using BaseClass = MonoMac.Foundation.NSObject;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.JzIntvUI.ViewModel
{
    /// <summary>
    /// ViewModel for DisplayMode.
    /// </summary>
    public class DisplayModeViewModel : BaseClass
    {
        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="mode">The value to represent.</param>
        public DisplayModeViewModel(DisplayMode mode)
        {
            Mode = mode.ToString();
            DisplayMode = mode;
        }

        /// <summary>
        /// Gets the display name for the value.
        /// </summary>
        [INTV.Shared.Utility.OSExportAttribute("Mode")]
        public string Mode { get; private set; }

        /// <summary>
        /// Gets the represented value.
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }
    }
}
