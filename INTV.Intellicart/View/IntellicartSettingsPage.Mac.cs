// <copyright file="IntellicartSettingsPage.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using MonoMac.Foundation;
using INTV.Shared.View;

namespace INTV.Intellicart.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class IntellicartSettingsPage : MonoMac.AppKit.NSView, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public IntellicartSettingsPage(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public IntellicartSettingsPage(NSCoder coder)
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
        /// Controller for the view.
        /// </summary>
        internal IntellicartSettingsPageController Controller { get; set; }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return Controller.DataContext; }
            set { Controller.DataContext = value; }
        }

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return Controller.GetValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            Controller.SetValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject
    }
}
