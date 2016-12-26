// <copyright file="SettingsPageViewModel.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for the LTO Flash! settings page.
    /// </summary>
    public sealed partial class SettingsPageViewModel
    {
        private void Initialize()
        {
            var settings = NSUserDefaults.StandardUserDefaults;
            _searchForDevicesAtStartup = settings.BoolForKey("SearchForDevicesAtStartup");
            _validateMenuAtLaunch = settings.BoolForKey("ValidateMenuAtStartup");
            _reconcileDeviceMenuWithLocalMenu = settings.BoolForKey("ReconcileDeviceMenuWithLocalMenu");
        }
    }
}
