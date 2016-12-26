// <copyright file="DeviceInformation.Mac.cs" company="INTV Funhouse">
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
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Shared.Utility;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Mac-specific implementation of device information.
    /// </summary>
    public partial class DeviceInformation : NSWindow
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public DeviceInformation(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public DeviceInformation(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets or sets the controller for the dialog.
        /// </summary>
        internal DeviceInformationController Controller { get; set; }

        /// <summary>
        /// Creates a new instance of DeviceInformation.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static DeviceInformation Create(INTV.LtoFlash.ViewModel.LtoFlashViewModel viewModel)
        {
            var controller = new INTV.LtoFlash.View.DeviceInformationController(viewModel);
            controller.Window.Controller = controller;
            return controller.Window;
        }

        /// <summary>
        /// Show the dialog.
        /// </summary>
        /// <remarks>TODO: Delete this and use the extension methods!</remarks>
        public void ShowWindow()
        {
            this.ShowDialog(true);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (Controller != null))
            {
                Controller.Dispose();
                Controller = null;
            }
            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }
    }
}
