// <copyright file="PromptToInstallFtdiDriverDialogController.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Controller for the dialog to prompt for install the FTDI driver.
    /// </summary>
    public partial class PromptToInstallFtdiDriverDialogController : NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public PromptToInstallFtdiDriverDialogController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public PromptToInstallFtdiDriverDialogController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public PromptToInstallFtdiDriverDialogController()
            : base("PromptToInstallFtdiDriverDialog")
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            DialogDataContext = new PromptToInstallFtdiDriverDialogViewModel();
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new PromptToInstallFtdiDriverDialog Window
        {
            get { return (PromptToInstallFtdiDriverDialog)base.Window; }
        }

        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        [OSExport("Title")]
        public string Title
        {
            get { return DialogDataContext.Title; }
        }

        /// <summary>
        /// Gets the message to display in the dialog.
        /// </summary>
        [OSExport("Message")]
        public string Message
        {
            get { return DialogDataContext.Message; }
        }

        private PromptToInstallFtdiDriverDialogViewModel DialogDataContext { get; set; }

        /// <summary>
        /// Handle do not ask again checkbox.
        /// </summary>
        /// <param name="sender">The checkbox that was clicked.</param>
        partial void DoNotAskAgain(NSObject sender)
        {
            Properties.Settings.Default.PromptToInstallFTDIDriver = false;
            Properties.Settings.Default.Save();
            Window.EndDialog(NSRunResponse.Aborted);
        }

        /// <summary>
        /// Handle do not install button click.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        partial void DoNotInstall(NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Aborted);
        }

        /// <summary>
        /// Handle launch installer button click.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        partial void LaunchInstaller(NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Stopped);
        }
    }
}
