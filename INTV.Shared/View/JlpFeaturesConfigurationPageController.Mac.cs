// <copyright file="JlpFeaturesConfigurationPageController.Mac.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// NSViewController for <see cref="JlpFeaturesConfigurationPage"/>.
    /// </summary>
    public partial class JlpFeaturesConfigurationPageController : MonoMac.AppKit.NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public JlpFeaturesConfigurationPageController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public JlpFeaturesConfigurationPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public JlpFeaturesConfigurationPageController()
            : base("JlpFeaturesConfigurationPage", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        [OSExport(JlpFeaturesConfigurationPageViewModel.JlpVersionPropertyName)]
        private NSNumber JlpVersion
        {
            get
            {
                return _jlpVersion;
            }

            set
            {
                if (_jlpVersion.Int32Value != value.Int32Value)
                {
                    ViewModel.JlpVersion = ViewModel.JlpVersions[value.Int32Value];
                }
                _jlpVersion = value;
            }
        }
        private NSNumber _jlpVersion;

        [OSExport(JlpFeaturesConfigurationPageViewModel.JlpFeaturesConfigurablePropertyName)]
        private NSNumber JlpFeaturesConfigurable
        {
            get { return new NSNumber(ViewModel.JlpFeaturesConfigurable); }
        }

        [OSExport(JlpFeaturesConfigurationPageViewModel.EnableAcceleratorsAtStartupPropertyName)]
        private NSNumber EnableAcceleratorsAtStartup
        {
            get
            {
                return _enableAcceleratorsAtStartup;
            }

            set
            {
                if (_enableAcceleratorsAtStartup.BoolValue != value.BoolValue)
                {
                    ViewModel.EnableAcceleratorsAtStartup = value.BoolValue;
                }
                _enableAcceleratorsAtStartup = value;
            }
        }
        NSNumber _enableAcceleratorsAtStartup;

        [OSExport(JlpFeaturesConfigurationPageViewModel.UsesFlashStoragePropertyName)]
        private NSNumber UsesFlashStorage
        {
            get
            {
                return _usesFlashStorage;
            }

            set
            {
                if (_usesFlashStorage.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesFlashStorage = value.BoolValue;
                }
                _usesFlashStorage = value;
            }
        }
        NSNumber _usesFlashStorage;

        [OSExport(JlpFeaturesConfigurationPageViewModel.MinimumFlashSectorsPropertyName)]
        private NSNumber MinimumFlashSectors
        {
            get
            {
                return _minimumFlashSectors;
            }

            set
            {
                if (_minimumFlashSectors.UInt16Value != value.UInt16Value)
                {
                    ViewModel.MinimumFlashSectors = value.UInt16Value;
                }
                _minimumFlashSectors = value;
            }
        }
        private NSNumber _minimumFlashSectors;

        [OSExport("FlashSectorsTextColor")]
        private NSColor FlashSectorsTextColor { get; set; }

        [OSExport(JlpFeaturesConfigurationPageViewModel.FlashSizeInBytesTipPropertyName)]
        private string FlashSizeInBytesTip { get { return ViewModel.FlashSizeInBytesTip.SafeString(); } }

        [OSExport(JlpFeaturesConfigurationPageViewModel.CanConfigureFlashStoragePropertyName)]
        private NSNumber CanConfigureFlashStorage
        {
            get { return new NSNumber(ViewModel.CanConfigureFlashStorage); }
        }

        [OSExport(JlpFeaturesConfigurationPageViewModel.UsesSerialPortPropertyName)]
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
        NSNumber _usesSerialPort;

        [OSExport(JlpFeaturesConfigurationPageViewModel.SerialPortPropertyName)]
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

        [OSExport(JlpFeaturesConfigurationPageViewModel.CanConfigureSerialPortFeaturePropertyName)]
        private NSNumber CanConfigureSerialPortFeature
        {
            get { return new NSNumber(ViewModel.CanConfigureSerialPortFeature); }
        }

        [OSExport(JlpFeaturesConfigurationPageViewModel.CanSelectSerialPortOptionPropertyName)]
        private NSNumber CanSelectSerialPortOption
        {
            get { return new NSNumber(ViewModel.CanSelectSerialPortOption); }
        }

        [OSExport(JlpFeaturesConfigurationPageViewModel.UsesLEDsPropertyName)]
        private NSNumber UsesLEDs
        {
            get
            {
                return _usesLEDs;
            }

            set
            {
                if (_usesLEDs.BoolValue != value.BoolValue)
                {
                    ViewModel.UsesLEDs = value.BoolValue;
                }
                _usesLEDs = value;
            }
        }
        NSNumber _usesLEDs;

        [OSExport(JlpFeaturesConfigurationPageViewModel.CanConfigureLEDFeaturePropertyName)]
        private NSNumber CanConfigureLEDFeature
        {
            get { return new NSNumber(ViewModel.CanConfigureLEDFeature); }
        }

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new JlpFeaturesConfigurationPage View { get { return (JlpFeaturesConfigurationPage)base.View; } }

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

        private JlpFeaturesConfigurationPageViewModel ViewModel { get { return DataContext as JlpFeaturesConfigurationPageViewModel; } }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            FlashSectorsFormatter.Minimum = 0;
            FlashSectorsFormatter.Maximum = INTV.Core.Model.Program.LtoFlashFeaturesHelpers.MaxFlashSaveDataSectorsCount;
            var initializationData = new[] {
                new Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(JlpVersionPopUpButton, ViewModel.JlpVersions, ViewModel.JlpVersion),
                new Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(SerialPortPopUpButton, ViewModel.SerialPortOptions, ViewModel.SerialPort)
            };
            initializationData.InitializePopupButtons();
            ViewModel.RaisePropertyChangedForVisualInit();
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case JlpFeaturesConfigurationPageViewModel.JlpVersionPropertyName:
                    _jlpVersion = ViewModel.JlpVersions.SelectionToNSNumber(ViewModel.JlpVersion);
                    break;
                case JlpFeaturesConfigurationPageViewModel.JlpFeaturesConfigurablePropertyName:
                    break;
                case JlpFeaturesConfigurationPageViewModel.EnableAcceleratorsAtStartupPropertyName:
                    _enableAcceleratorsAtStartup = new NSNumber(ViewModel.EnableAcceleratorsAtStartup);
                    break;
                case JlpFeaturesConfigurationPageViewModel.UsesFlashStoragePropertyName:
                    _usesFlashStorage = new NSNumber(ViewModel.UsesFlashStorage);
                    break;
                case JlpFeaturesConfigurationPageViewModel.MinimumFlashSectorsPropertyName:
                    _minimumFlashSectors = new NSNumber(ViewModel.MinimumFlashSectors);
                    break;
                case JlpFeaturesConfigurationPageViewModel.FlashUsageLevelPropertyName:
                case JlpFeaturesConfigurationPageViewModel.CanConfigureFlashStoragePropertyName:
                    UpdateJlpFlashConfigurationElements(ViewModel.FlashUsageLevel);
                    break;
                case JlpFeaturesConfigurationPageViewModel.UsesSerialPortPropertyName:
                    _usesSerialPort = new NSNumber(ViewModel.UsesSerialPort);
                    break;
                case JlpFeaturesConfigurationPageViewModel.SerialPortPropertyName:
                    _serialPort = ViewModel.SerialPortOptions.SelectionToNSNumber(ViewModel.SerialPort);
                    break;
                case JlpFeaturesConfigurationPageViewModel.CanConfigureSerialPortFeaturePropertyName:
                    break;
                case JlpFeaturesConfigurationPageViewModel.CanSelectSerialPortOptionPropertyName:
                    break;
                case JlpFeaturesConfigurationPageViewModel.UsesLEDsPropertyName:
                    _usesLEDs = new NSNumber(ViewModel.UsesLEDs);
                    break;
                case JlpFeaturesConfigurationPageViewModel.CanConfigureLEDFeaturePropertyName:
                    break;
            }
            this.RaiseChangeValueForKey(e.PropertyName);
        }

        private void UpdateJlpFlashConfigurationElements(JlpFlashStorageUsageLevel flashUsageLevel)
        {
            var color = ViewModel.CanConfigureFlashStorage ? NSColor.Text : NSColor.DisabledControlText;
            if (ViewModel.CanConfigureFlashStorage)
            {
                switch (flashUsageLevel)
                {
                    case JlpFlashStorageUsageLevel.None:
                    case JlpFlashStorageUsageLevel.Normal:
                        break;
                    case JlpFlashStorageUsageLevel.High:
                        color = INTV.Core.Model.Stic.Color.Orange.ToColor();
                        break;
                    case JlpFlashStorageUsageLevel.LtoFlashOnly:
                        color = INTV.Core.Model.Stic.Color.Red.ToColor();
                        break;
                }
            }
            FlashSectorsTextColor = color;
            this.RaiseChangeValueForKey("FlashSectorsTextColor");
            FlashSectorsFormatter.Minimum = ViewModel.CanConfigureFlashStorage ? INTV.Core.Model.Program.JlpFeaturesHelpers.MinJlpFlashSectorUsage : 0;
        }
    }
}
