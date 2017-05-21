// <copyright file="PromptToAddMenuItemsForNewRomsController.Mac.cs" company="INTV Funhouse">
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
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.Utility;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Controller for the PromptToAddMenuItemsForNewRoms dialog.
    /// </summary>
    public partial class PromptToAddMenuItemsForNewRomsController : NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public PromptToAddMenuItemsForNewRomsController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public PromptToAddMenuItemsForNewRomsController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public PromptToAddMenuItemsForNewRomsController()
            : base("PromptToAddMenuItemsForNewRoms")
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new PromptToAddMenuItemsForNewRoms Window { get { return (PromptToAddMenuItemsForNewRoms)base.Window; } }

        partial void OnOK(NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Stopped);
        }
    }
}
