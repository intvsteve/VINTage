﻿// <copyright file="ProgressIndicatorController.Mac.cs" company="INTV Funhouse">
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
using INTV.Core.ComponentModel;
using INTV.Shared.Commands;
using INTV.Shared.ViewModel;
#if __UNIFIED__
using AppKit;
using CoreGraphics;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.Foundation;
#endif // __UNIFIED__

#if __UNIFIED__
using CGRect = CoreGraphics.CGRect;
using nfloat = System.nfloat;
#else
using CGRect = System.Drawing.RectangleF;
using nfloat = System.Single;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Controller for <see cref="ProgressIndicator"/> visual.
    /// </summary>
    public partial class ProgressIndicatorController : NSViewController
    {
        private bool _loadedFromNib; // FIXME This is wrong

        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ProgressIndicatorController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public ProgressIndicatorController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public ProgressIndicatorController()
            : base("ProgressIndicator", NSBundle.MainBundle)
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
        public new ProgressIndicator View
        {
            get { return (ProgressIndicator)base.View; }
        }

        internal ProgressIndicatorViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets the title text for the progress indicator.
        /// </summary>
        [INTV.Shared.Utility.OSExport("ProgressTitle")]
        public string ProgressTitle
        {
            get
            {
                var title = string.Empty;

                // FIXME this is wrong
                if (_loadedFromNib)
                {
                    var viewModel = View.DataContext as ProgressIndicatorViewModel;
                    title = viewModel == null ? string.Empty : viewModel.Title;
                }
                return title.SafeString();
            }
        }

        /// <inheritdoc/>
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            View.Hidden = true;
            View.WantsLayer = true;
            var colorSpace = CGColorSpace.CreateDeviceRGB();
            var colorComponents = new nfloat[4];
            NSColor.LightGray.UsingColorSpace(NSColorSpace.DeviceRGB).GetComponents(out colorComponents);
            colorComponents[3] = 0.18f;
            View.Layer.BackgroundColor = new CGColor(colorSpace, colorComponents);
            NSColor.ControlBackground.UsingColorSpace(NSColorSpace.DeviceRGBColorSpace).GetComponents(out colorComponents);
            FeedbackArea.WantsLayer = true; // macOS 10.13 (High Sierra) changed behavior of how WantsLayer works to apparently no longer create layers for all children immediately.
            FeedbackArea.Layer.BackgroundColor = new CGColor(colorSpace, colorComponents);
            FeedbackArea.Layer.BorderWidth = 1;
            RomListCommandGroup.CancelRomsImportCommand.Visual = Cancel;
            Cancel.Activated += HandleActivated; // Doesn't go through command mechanism
            _loadedFromNib = true; // FIXME This is wrong
        }

        /// <summary>
        /// Initialize the data context for the visual.
        /// </summary>
        /// <param name="window">Window for the progress indicator.</param>
        /// <param name="frame">Size to use for the indicator.</param>
        public void InitializeDataContext(NSWindow window, CGRect frame)
        {
            ViewModel = new INTV.Shared.ViewModel.ProgressIndicatorViewModel(window);
            View.Frame = frame;
            var progressViewModel = ViewModel;

            // FIXME This is wrong! We should put the data context here, on the controller!
            View.DataContext = progressViewModel;
            RomListCommandGroup.CancelRomsImportCommand.CanExecute(progressViewModel); // EEEK TOTAL HACK
            progressViewModel.PropertyChanged += HandlePropertyChanged;
            ProgressBar.Indeterminate = progressViewModel.IsIndeterminate;
        }

        private void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandlePropertyChangedCore);
        }

        private void HandlePropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var viewModel = (ProgressIndicatorViewModel)View.DataContext;
            switch (e.PropertyName)
            {
                case ProgressIndicatorViewModel.AllowsCancelPropertyName:
                    Cancel.Enabled = viewModel.AllowsCancel;
                    break;
                case ProgressIndicatorViewModel.TitlePropertyName:
                    // this.Title.StringValue = viewModel.Title;
                    this.RaiseChangeValueForKey("ProgressTitle");
                    break;
                case ProgressIndicatorViewModel.UpdateTextPropertyName:
                    UpdateText.StringValue = viewModel.UpdateText;
                    break;
                case ProgressIndicatorViewModel.IsIndeterminatePropertyName:
                    ProgressBar.Indeterminate = viewModel.IsIndeterminate;
                    break;
                case ProgressIndicatorViewModel.PercentFinishedPropertyName:
                    if (!viewModel.IsIndeterminate)
                    {
                        ProgressBar.DoubleValue = viewModel.PercentFinished;
                    }
                    break;
                case ProgressIndicatorViewModel.IsVisiblePropertyName:
                    View.Hidden = !viewModel.IsVisible;
                    if (viewModel.IsIndeterminate)
                    {
                        if (viewModel.IsVisible)
                        {
                            ProgressBar.StartAnimation(this);
                        }
                        else
                        {
                            ProgressBar.StopAnimation(this);
                        }
                    }
                    break;
            }
        }

        private void HandleActivated(object sender, System.EventArgs e)
        {
            var viewModel = (ProgressIndicatorViewModel)View.DataContext;
            if (viewModel.CancelCommand.CanExecute(viewModel))
            {
                viewModel.CancelCommand.Execute(viewModel);
            }
        }
    }
}
