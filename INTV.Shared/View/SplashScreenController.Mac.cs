// <copyright file="SplashScreenController.Mac.cs" company="INTV Funhouse">
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

using INTV.Shared.Utility;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Splash screen controller.
    /// </summary>
    public partial class SplashScreenController : NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SplashScreenController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SplashScreenController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public SplashScreenController()
            : base("SplashScreen")
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SplashScreenController"/> class.
        /// </summary>
        /// <param name="splashImage">Absolute path to the splash screen image to display.</param>
        public SplashScreenController(string splashImage)
            : this()
        {
            SplashScreenImage = splashImage.LoadImageFromDisk();
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new SplashScreen Window
        {
            get { return (SplashScreen)base.Window; }
        }

        private NSImage SplashScreenImage { get; set; }

        #endregion // Properties

        /// <inheritdoc/>
        public override void AwakeFromNib()
        {
            Window.Level = NSWindowLevel.ModalPanel;
            SplashScreenImageView.Image = SplashScreenImage;
            var size = SplashScreenImageView.FittingSize;
            Window.SetContentSize(size);
        }

        /// <summary>
        /// Update the image displayed in the splash screen.
        /// </summary>
        /// <param name="image">The new image to display.</param>
        internal void UpdateImage(NSImage image)
        {
            SplashScreenImage = image;
            SplashScreenImageView.Image = image;
            SplashScreenImageView.NeedsDisplay = true;
            SplashScreenImageView.Display();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }
    }
}
