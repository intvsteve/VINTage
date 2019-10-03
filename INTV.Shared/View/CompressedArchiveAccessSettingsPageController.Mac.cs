// <copyright file="CompressedArchiveAccessSettingsPageController.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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

using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;
#else
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation for compressed archive access settings page.
    /// </summary>
    public partial class CompressedArchiveAccessSettingsPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public CompressedArchiveAccessSettingsPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public CompressedArchiveAccessSettingsPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public CompressedArchiveAccessSettingsPageController()
            : base("CompressedArchiveAccessSettingsPage", NSBundle.MainBundle)
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
        public new CompressedArchiveAccessSettingsPage View
        {
            get { return (CompressedArchiveAccessSettingsPage)base.View; }
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

        [OSExport("MaxCompressedArchiveSize")]
        private int MaxCompressedArchiveSize
        {
            get { return ViewModel.MaxCompressedArchiveSize; }
            set { ViewModel.MaxCompressedArchiveSize = value; }
        }

        [OSExport("EnableNestedArchives")]
        private bool EnableNestedArchives
        {
            get { return ViewModel.EnableNestedArchives; }
            set { ViewModel.EnableNestedArchives = value; }
        }

        [OSExport("CompoundFormats")]
        private string CompoundFormats
        {
            get { return ViewModel.CompoundFormats; }
        }

        [OSExport("EnableCustomModeSelection")]
        private bool EnableCustomModeSelection
        {
            get { return ViewModel.EnableCustomModeSelection; }
        }

        [OSExport("EnableOtherOptions")]
        private bool EnableOtherOptions
        {
            get { return ViewModel.EnableOtherOptions; }
        }

        private CompressedArchiveAccessSettingsViewModel ViewModel
        {
            get { return DataContext as CompressedArchiveAccessSettingsViewModel; }
        } 

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            View.Controller = this;
            InitializeCompressedArchiveSelectionRadioButtons();
            this.CompressedArchiveFormatsArrayController.SynchronizeCollection(ViewModel.CompressedArchiveFormats);
            MaxArchiveSizeLabel.AlphaValue = ViewModel.EnableOtherOptions ? 1.0f : 0.5f;
            ViewModel.PropertyChanged += HandleViewModelPropertyChanged;
        }

        private void InitializeCompressedArchiveSelectionRadioButtons()
        {
            var state = (int)ViewModel.Mode;
            foreach (var radioButton in View.Subviews.OfType<NSButton>().Where(b => b.Identifier.StartsWith("CompressedArchiveMode")))
            {
                radioButton.State = radioButton.Tag == state ? NSCellStateValue.On : NSCellStateValue.Off;
            }
        }

        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleViewModelPropertyChangedCore);
        }

        private void HandleViewModelPropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "EnableCustomModeSelection":
                case "EnableOtherOptions":
                case "CompoundFormats":
                    MaxArchiveSizeLabel.AlphaValue = ViewModel.EnableOtherOptions ? 1.0f : 0.5f;
                    this.RaiseChangeValueForKey(e.PropertyName);
                    break;
            }
        }

        /// <summary>
        /// Implementation of the handler for radio button value change.
        /// </summary>
        /// <param name="sender">The radio button that was clicked.</param>
        partial void OnCompressedArchiveModeSelected(NSObject sender)
        {
            var button = sender as NSButton;
            ViewModel.Mode = (CompressedArchiveSelection)(int)button.Tag;
        }
    }
}
