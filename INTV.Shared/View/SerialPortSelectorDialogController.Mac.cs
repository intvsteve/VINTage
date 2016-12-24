// <copyright file="SerialPortSelectorDialogController.Mac.cs" company="INTV Funhouse">
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
using System.Collections.Specialized;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Controller for the SerialPortSelectorDialog.
    /// </summary>
    public partial class SerialPortSelectorDialogController : MonoMac.AppKit.NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SerialPortSelectorDialogController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SerialPortSelectorDialogController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public SerialPortSelectorDialogController()
            : base("SerialPortSelectorDialog")
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        /// <param name="dataContext">Data context for the dialog.</param>
        public SerialPortSelectorDialogController(SerialPortSelectorDialogViewModel dataContext)
            : base("SerialPortSelectorDialog")
        {
            DataContext = dataContext;
            DataContext.PortSelectorViewModel.PropertyChanged += HandlePortSelectorPropertyChanged;
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
        public new SerialPortSelectorDialog Window { get { return (SerialPortSelectorDialog)base.Window; } }

        private SerialPortSelectorDialogViewModel DataContext { get; set; }

        private SerialPortSelectorController PortSelectorController { get; set; }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            var closeButton = Window.StandardWindowButton(NSWindowButton.CloseButton);
            if (closeButton != null)
            {
                // OK to do this directly we don't present closing as an ICommand.
                closeButton.Activated += HandleCloseClicked;
            }
            Window.DataContext = DataContext;
            PortSelectorController = new SerialPortSelectorController(DataContext.PortSelectorViewModel);
            PortSelectorController.SelectionDoubleClicked += HandleDoubleClick;
            var view = PortSelectorController.View;
            HandlePortSelectorPropertyChanged(DataContext.PortSelectorViewModel, new System.ComponentModel.PropertyChangedEventArgs(SerialPortSelectorViewModel.SelectedSerialPortPropertyName));
            view.Frame = this.SerialPortSelectorView.Frame;
            Window.ContentView.ReplaceSubviewWith(SerialPortSelectorView, view);
            SerialPortSelectorView = view;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            // Unhook event handlers to ensure we don't leak.
            var closeButton = Window.StandardWindowButton(NSWindowButton.CloseButton);
            if (closeButton != null)
            {
                closeButton.Activated -= HandleCloseClicked;
            }
            DataContext.PortSelectorViewModel.PropertyChanged -= HandlePortSelectorPropertyChanged;
            PortSelectorController.SelectionDoubleClicked -= HandleDoubleClick;
            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }

        private void HandlePortSelectorPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case SerialPortSelectorViewModel.SelectedSerialPortPropertyName:
                    this.SelectPort.Enabled = (PortSelectorController.SelectedPortIndex != null) && (PortSelectorController.SelectedPortIndex.Count == 1);
                    break;
            }
        }

        private void HandleDoubleClick(object sender, EventArgs e)
        {
            OnSelect(this);
        }

        private void HandleCloseClicked(object sender, EventArgs e)
        {
            var closeButton = Window.StandardWindowButton(NSWindowButton.CloseButton);
            if (closeButton != null)
            {
                OnCancel(closeButton);
            }
        }

        partial void OnSelect(MonoMac.Foundation.NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Stopped);
        }

        partial void OnCancel(MonoMac.Foundation.NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Aborted);
        }
    }
}
