// <copyright file="LtoFlashViewModel.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_COLORS_PATCH

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__
using INTV.LtoFlash.Model;
using INTV.Shared.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.ViewModel
{
    public partial class LtoFlashViewModel
    {
        private bool _showFTDIWarning;

#if ENABLE_COLORS_PATCH
        private FixColorsList _fixColors;
#endif // ENABLE_ROMS_PATCH

        partial void OSInitialize()
        {
            _showFTDIWarning = OSVersion.Current < Configuration.Instance.RecommendedOSVersion;
#if ENABLE_COLORS_PATCH
            _fixColors = new FixColorsList();
            _fixColors.Register();
#endif // ENABLE_COLORS_PATCH
        }

        partial void OSOnActiveDeviceChanged()
        {
            if (_showFTDIWarning)
            {
                _showFTDIWarning = false;
                ApplicationCommandGroup.ShowFTDIWarningCommand.Execute(null);
            }
        }

        partial void OSDeviceArrivalDepartureActiveChanged()
        {
            var menuItem = INTV.LtoFlash.Commands.DeviceCommandGroup.ConnectToDeviceSubmenuCommand.MenuItem;
            var subMenu = !menuItem.IsEmpty ? menuItem.NativeMenuItem.Submenu : null;
            var menuDelegate = subMenu != null ? subMenu.Delegate as NSMenuDelegate : null;
            if (menuDelegate != null)
            {
                menuDelegate.MenuWillOpen(subMenu); // force re-initialization of the submenu
            }
        }

#if ENABLE_COLORS_PATCH
        /// <summary>
        /// Earlier beta releases (b10 and older) did not support Black in the color list.
        /// Because the color list is stored in a file, this fix will delete the color list
        /// so it can be recreated.
        /// NOTE: This is a two-stage patch. Because the color list file is processed before
        /// the patch runs, the first launch will delete the colors list AFTER the application
        /// processes it. On the SECOND launch after this patch applies, the color list updates.
        /// </summary>
        private class FixColorsList : OneShotLaunchTask
        {
            public FixColorsList()
                : base("FixColorsList")
            {
            }

            /// <inheritdoc />
            protected override void Run()
            {
                var cfg = SingleInstanceApplication.Instance.GetConfiguration<INTV.LtoFlash.Model.Configuration>();
                var colorsPath = System.IO.Path.Combine(cfg.ApplicationConfigurationPath, INTV.LtoFlash.View.MenuLayoutView.MenuColors);
                if (System.IO.File.Exists(colorsPath))
                {
                    var attributes = System.IO.File.GetAttributes(colorsPath);
                    attributes &= ~System.IO.FileAttributes.ReadOnly;
                    System.IO.File.SetAttributes(colorsPath, attributes);
                    System.IO.File.Delete(colorsPath);
                }
            }
        }
#endif // ENABLE_COLORS_PATCH
    }
}

