// <copyright file="KeyboardComponentFeaturesConfigurationPageController.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// NSViewController for <see cref="KeyboardComponentFeaturesConfigurationPage"/>.
    /// </summary>
    public partial class KeyboardComponentFeaturesConfigurationPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public KeyboardComponentFeaturesConfigurationPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public KeyboardComponentFeaturesConfigurationPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public KeyboardComponentFeaturesConfigurationPageController()
            : base("KeyboardComponentFeaturesConfigurationPage", NSBundle.MainBundle)
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
        public new KeyboardComponentFeaturesConfigurationPage View { get { return (KeyboardComponentFeaturesConfigurationPage)base.View; } }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContextWithDataContextPropertyChangedHandler(value, ViewModelPropertyChanged); }
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
        /// Gets or sets the Keyboard Component compatibility.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.KeyboardComponentPropertyName)]
        public NSNumber KeyboardComponent
        {
            get
            {
                return _keyboardComponent;
            }

            set
            {
                if (_keyboardComponent.Int32Value != value.Int32Value)
                {
                    ViewModel.KeyboardComponent = ViewModel.KeyboardComponentOptions[value.Int32Value];
                }
                _keyboardComponent = value;
            }
        }
        private NSNumber _keyboardComponent;

        /// <summary>
        /// Gets the whether KC feature edits are enabled.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.EnableFeatureEditingPropertyName)]
        public NSNumber EnableFeatureEditing
        {
            get { return new NSNumber(ViewModel.EnableFeatureEditing); }
        }

        /// <summary>
        /// Gets or sets whether the ROM uses the KC microphone.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.UsesMicrophonePropertyName)]
        public NSNumber UsesMicrophone
        {
            get
            {
                return _usesMicrophone;
            }

            set
            {
                if (_usesMicrophone.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesMicrophone = value.BoolValue;
                }
                _usesMicrophone = value;
            }
        }
        NSNumber _usesMicrophone;

        /// <summary>
        /// Gets or sets whether the ROM uses the KC printer.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.UsesPrinterPropertyName)]
        public NSNumber UsesPrinter
        {
            get
            {
                return _usesPrinter;
            }

            set
            {
                if (_usesPrinter.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesPrinter = value.BoolValue;
                }
                _usesPrinter = value;
            }
        }
        NSNumber _usesPrinter;

        /// <summary>
        /// Gets or sets whether the ROM uses KC cassette.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.UsesCassettePropertyName)]
        public NSNumber UsesCassette
        {
            get
            {
                return _usesCassette;
            }

            set
            {
                if (_usesCassette.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesCassette = value.BoolValue;
                }
                _usesCassette = value;
            }
        }
        NSNumber _usesCassette;

        /// <summary>
        /// Gets or sets the cassette requirements - optional or required.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.CassettePropertyName)]
        public NSNumber Cassette
        {
            get
            {
                return _cassette;
            }

            set
            {
                if (_cassette.Int32Value != value.Int32Value)
                {
                    ViewModel.Cassette = ViewModel.CassetteOptions[value.Int32Value];
                }
                _cassette = value;
            }
        }
        private NSNumber _cassette;

        /// <summary>
        /// Gets whether to enable the enable cassette compatibility options.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.EnableCassetteOptionSelectionPropertyName)]
        public NSNumber EnableCassetteOptionSelection
        {
            get { return new NSNumber(ViewModel.EnableCassetteOptionSelection); }
        }

        private KeyboardComponentFeaturesConfigurationPageViewModel ViewModel { get { return DataContext as KeyboardComponentFeaturesConfigurationPageViewModel; } }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            var initializationData = new[] {
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(KeyboardComponentCompatibilityPopUpButton, ViewModel.KeyboardComponentOptions, ViewModel.KeyboardComponent),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(CassetteRequirementPopUpButton, ViewModel.CassetteOptions, ViewModel.Cassette),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(MicrosoftBasicCartridgeRequirementPopUpButton, ViewModel.BasicOptions, ViewModel.Basic)
            };
            initializationData.InitializePopupButtons();
            ViewModel.RaisePropertyChangedForVisualInit();
        }

        /// <summary>
        /// Gets or sets the requirement for KC MS BASIC.
        /// </summary>
        [OSExport(KeyboardComponentFeaturesConfigurationPageViewModel.BasicPropertyName)]
        public NSNumber Basic
        {
            get
            {
                return _basic;
            }

            set
            {
                if (_basic.Int32Value != value.Int32Value)
                {
                    ViewModel.Basic = ViewModel.BasicOptions[value.Int32Value];
                }
                _basic = value;
            }
        }
        private NSNumber _basic;

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case KeyboardComponentFeaturesConfigurationPageViewModel.KeyboardComponentPropertyName:
                    _keyboardComponent = ViewModel.KeyboardComponentOptions.SelectionToNSNumber(ViewModel.KeyboardComponent);
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.EnableFeatureEditingPropertyName:
                    MicrosoftBasicUsageLabel.TextColor = ViewModel.EnableFeatureEditing ? NSColor.Text : NSColor.DisabledControlText;
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.UsesMicrophonePropertyName:
                    _usesMicrophone = new NSNumber(ViewModel.UsesMicrophone);
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.UsesPrinterPropertyName:
                    _usesPrinter = new NSNumber(ViewModel.UsesPrinter);
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.UsesCassettePropertyName:
                    _usesCassette = new NSNumber(ViewModel.UsesCassette);
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.CassettePropertyName:
                    _cassette = ViewModel.CassetteOptions.SelectionToNSNumber(ViewModel.Cassette);
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.EnableCassetteOptionSelectionPropertyName:
                    break;
                case KeyboardComponentFeaturesConfigurationPageViewModel.BasicPropertyName:
                    _basic = ViewModel.BasicOptions.SelectionToNSNumber(ViewModel.Basic);
                    break;
            }
            this.RaiseChangeValueForKey(e.PropertyName);
        }
    }
}
