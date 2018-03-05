// <copyright file="SettingsPageController.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
    /// Controller for the LTO Flash! settings page.
    /// </summary>
    public partial class SettingsPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SettingsPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SettingsPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public SettingsPageController()
            : base("SettingsPage", NSBundle.MainBundle)
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
        public new SettingsPage View
        {
            get { return (SettingsPage)base.View; }
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

        /// <summary>
        /// Gets or sets the serial port read chunk size.
        /// </summary>
        [INTV.Shared.Utility.OSExport("SelectedReadChunkSize")]
        public NSNumber SelectedReadChunkSize
        {
            get
            {
                if (!_initializedReadChunkSizes)
                {
                    var selectedChunkSizeViewModel = ViewModel.SelectedReadChunkSizeViewModel;
                    var selectedChunkSize = selectedChunkSizeViewModel.BlockSize;
                    var selectionIndex = ViewModel.AvailableSerialPortReadBlockSizes.IndexOf(selectedChunkSizeViewModel);
                    if (selectionIndex < SerialPortReadChunkSizesArrayController.ArrangedObjects().Length)
                    {
                        _selectedReadChunkSize = selectionIndex;
                    }
                }
                return _selectedReadChunkSize;
            }

            set
            {
                _selectedReadChunkSize = value;
                ViewModel.SelectedReadChunkSizeViewModel = ViewModel.AvailableSerialPortReadBlockSizes[_selectedReadChunkSize.Int32Value];
            }
        }
        private NSNumber _selectedReadChunkSize;
        private bool _initializedReadChunkSizes;

        private SettingsPageViewModel ViewModel
        {
            get { return DataContext as SettingsPageViewModel; }
        } 

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            View.Controller = this;
            SerialPortReadChunkSizesArrayController.SynchronizeCollection(ViewModel.AvailableSerialPortReadBlockSizes);
            _initializedReadChunkSizes = true;
        }
    }
}
