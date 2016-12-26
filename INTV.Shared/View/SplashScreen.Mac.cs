// <copyright file="SplashScreen.Mac.cs" company="INTV Funhouse">
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
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace INTV.Shared.View
{
    /// <summary>
    /// Splash screen class. The consumer of this class is responsible for showing and hiding it.
    /// </summary>
    public partial class SplashScreen : NSPanel
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SplashScreen(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SplashScreen(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        void Initialize()
        {
            Title = "SplashScreen";
        }

        #endregion // Constructors

        #region Properties

        private SplashScreenController Controller { get; set; }

        #endregion // Properties

        /// <summary>
        /// Creates a splash screen that displays the given image.
        /// </summary>
        /// <param name="imageResource">An image resource from the application.</param>
        /// <returns>The SplashScreen, or <c>null</c> if the image resource cannot be located.</returns>
        /// <remarks>If the image resource cannot be located in the main bundle, this function returns <c>null</c>.</remarks>
        public static SplashScreen Show(string imageResource)
        {
            SplashScreen splashScreen = null;
            var splashResourcePath = NSBundle.MainBundle.PathForImageResource(System.IO.Path.GetFileName(imageResource));
            if (System.IO.File.Exists(splashResourcePath))
            {
                var controller = new SplashScreenController(splashResourcePath);
                splashScreen = controller.Window;
                splashScreen.Controller = controller;
                splashScreen.Center();
                splashScreen.IsVisible = true;
            }
            return splashScreen;
        }

        /// <summary>
        /// Closes the splash screen. After this point, it is no longer useful.
        /// </summary>
        public void Hide()
        {
            Close();
            Controller.Dispose();
            Dispose();
        }

        /// <summary>
        /// Update the image displayed in the splash screen.
        /// </summary>
        /// <param name="image">The udpated image to display in the splash screen.</param>
        public void UpdateImage(NSImage image)
        {
            Controller.UpdateImage(image);
        }
    }
}
