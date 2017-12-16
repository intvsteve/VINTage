// <copyright file="EcsFeaturesConfigurationPageController.Mac.cs" company="INTV Funhouse">
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
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// NSViewController for <see cref="EcsFeaturesConfigurationPage"/>.
    /// </summary>
    public partial class EcsFeaturesConfigurationPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public EcsFeaturesConfigurationPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public EcsFeaturesConfigurationPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public EcsFeaturesConfigurationPageController()
            : base("EcsFeaturesConfigurationPage", NSBundle.MainBundle)
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
        public new EcsFeaturesConfigurationPage View
        {
            get { return (EcsFeaturesConfigurationPage)base.View; }
        }

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

        [OSExport(EcsFeaturesConfigurationPageViewModel.EcsPropertyName)]
        private NSNumber Ecs
        {
            get
            {
                return _ecs;
            }

            set
            {
                if (_ecs.Int32Value != value.Int32Value)
                {
                    ViewModel.Ecs = ViewModel.EcsOptions[value.Int32Value];
                }
                _ecs = value;
            }
        }
        private NSNumber _ecs;

        [OSExport(EcsFeaturesConfigurationPageViewModel.EnableFeatureEditingPropertyName)]
        private NSNumber EnableFeatureEditing
        {
            get { return new NSNumber(ViewModel.EnableFeatureEditing); }
        }

        [OSExport(EcsFeaturesConfigurationPageViewModel.UsesCassettePropertyName)]
        private NSNumber UsesCassette
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
        private NSNumber _usesCassette;

        [OSExport(EcsFeaturesConfigurationPageViewModel.UsesSynthesizerPropertyName)]
        private NSNumber UsesSynthesizer
        {
            get 
            {
                return _usesSynthesizer;
            }

            set
            {
                if (_usesSynthesizer.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesSynthesizer = value.BoolValue;
                }
                _usesSynthesizer = value;
            }
        }
        private NSNumber _usesSynthesizer;

        [OSExport(EcsFeaturesConfigurationPageViewModel.UsesPrinterPropertyName)]
        private NSNumber UsesPrinter
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
        private NSNumber _usesPrinter;

        [OSExport(EcsFeaturesConfigurationPageViewModel.UsesSerialPortPropertyName)]
        private NSNumber UsesSerialPort
        {
            get
            {
                return _usesSerialPort;
            }

            set
            {
                if (_usesSerialPort.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesSerialPort = value.BoolValue;
                }
                _usesSerialPort = value;
            }
        }
        private NSNumber _usesSerialPort;

        [OSExport(EcsFeaturesConfigurationPageViewModel.SerialPortPropertyName)]
        private NSNumber SerialPort
        {
            get
            {
                return _serialPort;
            }

            set
            {
                if (_serialPort.Int32Value != value.Int32Value)
                {
                    ViewModel.SerialPort = ViewModel.SerialPortOptions[value.Int32Value];
                }
                _serialPort = value;
            }
        }
        private NSNumber _serialPort;

        private EcsFeaturesConfigurationPageViewModel ViewModel
        {
            get { return DataContext as EcsFeaturesConfigurationPageViewModel; }
        }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            var initializationData = new[]
            {
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(EcsCompatibilityPopUpButton, ViewModel.EcsOptions, ViewModel.Ecs),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(SerialPortUsagePopUpButton, ViewModel.SerialPortOptions, ViewModel.SerialPort)
            };
            initializationData.InitializePopupButtons();
            ViewModel.RaisePropertyChangedForVisualInit();
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case EcsFeaturesConfigurationPageViewModel.EcsPropertyName:
                    _ecs = ViewModel.EcsOptions.SelectionToNSNumber(ViewModel.Ecs);
                    break;
                case EcsFeaturesConfigurationPageViewModel.EnableFeatureEditingPropertyName:
                    break;
                case EcsFeaturesConfigurationPageViewModel.UsesCassettePropertyName:
                    _usesCassette = new NSNumber(ViewModel.UsesCassette);
                    break;
                case EcsFeaturesConfigurationPageViewModel.UsesSynthesizerPropertyName:
                    _usesSynthesizer = new NSNumber(ViewModel.UsesSynthesizer);
                    break;
                case EcsFeaturesConfigurationPageViewModel.UsesPrinterPropertyName:
                    _usesPrinter = new NSNumber(ViewModel.UsesPrinter);
                    break;
                case EcsFeaturesConfigurationPageViewModel.UsesSerialPortPropertyName:
                    _usesSerialPort = new NSNumber(ViewModel.UsesSerialPort);
                    break;
                case EcsFeaturesConfigurationPageViewModel.SerialPortPropertyName:
                    _serialPort = ViewModel.SerialPortOptions.SelectionToNSNumber(ViewModel.SerialPort);
                    break;
            }
            this.RaiseChangeValueForKey(e.PropertyName);
        }
    }
}
