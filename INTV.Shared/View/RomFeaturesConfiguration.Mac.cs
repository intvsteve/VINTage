// <copyright file="RomFeaturesConfiguration.Mac.cs" company="INTV Funhouse">
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
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class RomFeaturesConfiguration : NSPanel
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public RomFeaturesConfiguration(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public RomFeaturesConfiguration(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        private NSWindowController Controller { get; set; }

        /// <summary>
        /// Create the dialog from configuring ROM features.
        /// </summary>
        /// <param name="program">The program whose features are being edited.</param>
        /// <returns>THe dialog for editing ROM features.</returns>
        public static RomFeaturesConfiguration Create(ProgramDescriptionViewModel program)
        {
            var controller = new RomFeaturesConfigurationController();
            var viewModel = controller.DataContext as RomFeaturesConfigurationViewModel;
            viewModel.Initialize(program.Model, controller);
            var editor = controller.Window;
            editor.Controller = controller;
            editor.Title = string.Format(Resources.Strings.RomFeaturesEditor_TitleFormat, program.Name);
            return editor;
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
