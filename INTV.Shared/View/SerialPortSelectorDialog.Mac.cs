// <copyright file="SerialPortSelectorDialog.Mac.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class SerialPortSelectorDialog : MonoMac.AppKit.NSPanel, System.ComponentModel.INotifyPropertyChanged, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SerialPortSelectorDialog(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SerialPortSelectorDialog(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            Level = NSWindowLevel.ModalPanel;
        }

        #endregion // Constructors

        #region INotifyPropertyChanged Implementation

        /// <inheritdoc/>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged Implementation

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
        }

        #endregion // IFakeDependencyObject Properties

        /// <summary>
        /// Gets the view model for the dialog.
        /// </summary>
        internal SerialPortSelectorDialogViewModel ViewModel
        {
            get
            {
                var viewModel = DataContext as SerialPortSelectorDialogViewModel;
                if (viewModel == null)
                {
                    var propertyInfo = DataContext.GetType().GetProperties().First(pi => pi.PropertyType == typeof(SerialPortSelectorDialogViewModel));
                    viewModel = (SerialPortSelectorDialogViewModel)propertyInfo.GetValue(DataContext, null);
                }
                return viewModel;
            }
        }

        private NSWindowController Controller { get; set; }

        /// <summary>
        /// Runs the dialog.
        /// </summary>
        /// <returns>Result of the dialog. If <c>true</c>, the selections should be used. If <c>false</c> or <c>null</c>, the data
        /// should be ignored, e.g. the dialog was cancelled.</returns>
        public bool? ShowDialog()
        {
            return this.ShowDialog(true);
        }

        #region IFakeDependencyObject Methods

        /// <inheritdoc/>
        public object GetValue (string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue (string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject Methods

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

        private static SerialPortSelectorDialog Create(SerialPortSelectorDialogViewModel viewModel)
        {
            var dialogController = new SerialPortSelectorDialogController(viewModel);
            var dialog = dialogController.Window;
            dialog.Controller = dialogController;
            return dialog;
        }
    }
}
