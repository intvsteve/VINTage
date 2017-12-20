// <copyright file="IntellicartSettingsPageController.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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

using INTV.Intellicart.Commands;
using INTV.Intellicart.ViewModel;
using INTV.Shared.View;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Intellicart.View
{
    /// <summary>
    /// Controller implementation for the Intellicart settings page.
    /// </summary>
    public partial class IntellicartSettingsPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public IntellicartSettingsPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public IntellicartSettingsPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public IntellicartSettingsPageController()
            : base("IntellicartSettingsPage", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new IntellicartSettingsPage View
        {
            get { return (IntellicartSettingsPage)base.View; }
        }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            View.Controller = this;
        }

        private SettingsPageViewModel ViewModel
        {
            get { return DataContext as SettingsPageViewModel; }
        } 

        /// <summary>
        /// Called when the button is clicked to reset settings to default.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        partial void OnResetToDefault(NSObject sender)
        {
            if (SettingsPageViewModel.ResetToDefaultWriteTimeoutCommand.CanExecute(ViewModel))
            {
                SettingsPageViewModel.ResetToDefaultWriteTimeoutCommand.Execute(ViewModel);
            }
        }

        /// <summary>
        /// Called when a serial port is chosen.
        /// </summary>
        /// <param name="sender">The button that was clicked to select the port..</param>
        partial void OnSelectSerialPort(NSObject sender)
        {
            if (DeviceCommandGroup.SelectPortCommand.CanExecute(ViewModel.Intellicart))
            {
                DeviceCommandGroup.Group.SelectPortFromPreferences = true;
                DeviceCommandGroup.SelectPortCommand.Execute(ViewModel.Intellicart);
                DeviceCommandGroup.Group.SelectPortFromPreferences = false;
            }
        }
    }
}
