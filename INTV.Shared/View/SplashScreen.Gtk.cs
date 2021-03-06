﻿// <copyright file="SplashScreen.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class SplashScreen : Gtk.Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SplashScreen"/> class.
        /// </summary>
        public SplashScreen()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SplashScreen"/> class.
        /// </summary>
        /// <param name="splashScreenImage">Splash screen image.</param>
        public SplashScreen(Gdk.Pixbuf splashScreenImage)
            : this()
        {
            WidthRequest = splashScreenImage.Width;
            HeightRequest = splashScreenImage.Height;
            SetImage(splashScreenImage);
            SetPosition(Gtk.WindowPosition.CenterAlways);
            Show();
        }

        /// <summary>
        /// Sets the splash screen image.
        /// </summary>
        /// <param name="splashScreenImage">Splash screen image.</param>
        internal void SetImage(Gdk.Pixbuf splashScreenImage)
        {
            _splashImage.Pixbuf = splashScreenImage;
        }
    }
}
