// <copyright file="LtoFlashFeaturesConfigurationPageViewModel.cs" company="INTV Funhouse">
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

using System.Collections.ObjectModel;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;

namespace INTV.Shared.ViewModel
{
    // [System.ComponentModel.Composition.Export(typeof(IRomFeaturesConfigurationPage))]
    [LocalizedName(typeof(Resources.Strings), "LtoFlash")]
    [Weight(0.3)]
    public class LtoFlashFeaturesConfigurationPageViewModel : SettingsPageViewModel<INTV.Shared.View.LtoFlashFeaturesConfigurationPage>, IRomFeaturesConfigurationPage
    {
        #region Public static strings used by View

        public static readonly string GroupName = Resources.Strings.LtoFlashFeatures_Name;
        public static readonly string AccelerationText = Resources.Strings.HardwareAccelerationText;
        public static readonly string UsesFlashStorageText = Resources.Strings.LtoFlashFeatures_UsesFlashStorage;
        public static readonly string UsesSerialPortText = Resources.Strings.LtoFlashFeatures_UsesUsbPort;

        #endregion // Public static strings used by View

        #region Property Names

        private static readonly string AccelerationPropertyName = "Acceleration";
        private static readonly string UsesFlashStoragePropertyName = "UsesFlashStorage";
        private static readonly string FlashStoragePropertyName = "FlashStorage";
        private static readonly string UsesSerialPortPropertyName = "UsesSerialPort";
        private static readonly string SerialPortPropertyName = "SerialPort";

        #endregion // Property Names

        #region Constructor

        public LtoFlashFeaturesConfigurationPageViewModel()
        {
            var selectableOptions = new LtoFlashFeatures[] { LtoFlashFeatures.Incompatible, LtoFlashFeatures.Tolerates, LtoFlashFeatures.Enhances, LtoFlashFeatures.Requires };
            AccelerationOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(LtoFlashFeatureSet.Data));
            selectableOptions = new LtoFlashFeatures[] { LtoFlashFeatures.SaveDataOptional, LtoFlashFeatures.SaveDataRequired };
            FlashStorageOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(LtoFlashFeatureSet.Data));
            selectableOptions = new LtoFlashFeatures[] { LtoFlashFeatures.UsbPortEnhanced, LtoFlashFeatures.UsbPortRequired };
            SerialPortOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(LtoFlashFeatureSet.Data));
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the available options for LTO Flash! / JLP hardware accelerated instructions compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> AccelerationOptions { get; private set; }

        /// <summary>
        /// Gets or sets the LTO Flash! / JLP accelerated instructions compatibility.
        /// </summary>
        public ProgramFeatureImageViewModel Acceleration
        {
            get { return _acceleration; }
            set { UpdateFeatureProperty(AccelerationPropertyName, value, ref _acceleration); }
        }
        private ProgramFeatureImageViewModel _acceleration;

        /// <summary>
        /// Gets or sets a value indicating whether LTO Flash! / JLP onboard flash storage is used.
        /// </summary>
        public bool UsesFlashStorage
        {
            get { return _usesFlashStorage; }
            set { AssignAndUpdateProperty(UsesFlashStoragePropertyName, value, ref _usesFlashStorage, (s, v) => UpdateEnableStates()); }
        }
        private bool _usesFlashStorage;

        /// <summary>
        /// Gets the available options for onboard JLP flash storage compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> FlashStorageOptions { get; private set; }

        /// <summary>
        /// Gets or sets the JLP onboard flash storage compatibility.
        /// </summary>
        public ProgramFeatureImageViewModel FlashStorage
        {
            get { return _flashStorage; }
            set { UpdateFeatureProperty(FlashStoragePropertyName, value, ref _flashStorage); }
        }
        private ProgramFeatureImageViewModel _flashStorage;

        /// <summary>
        /// Gets or sets a value indicating whether onboard LTO Flash! serial port is used.
        /// </summary>
        public bool UsesSerialPort
        {
            get { return _usesSerialPort; }
            set { AssignAndUpdateProperty(UsesSerialPortPropertyName, value, ref _usesSerialPort, (s, v) => UpdateEnableStates()); }
        }
        private bool _usesSerialPort;

        /// <summary>
        /// Gets the available options for LTO Flash! serial port compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> SerialPortOptions { get; private set; }

        /// <summary>
        /// Gets or sets LTO Flash! serial port compatibility.
        /// </summary>
        public ProgramFeatureImageViewModel SerialPort
        {
            get { return _serialPort; }
            set { UpdateFeatureProperty(SerialPortPropertyName, value, ref _serialPort); }
        }
        private ProgramFeatureImageViewModel _serialPort;

        #endregion // Properties

        #region IRomFeaturesConfigurationPage

        public string Name
        {
            get { return Resources.Strings.LtoFlash; }
        }

        public override void Initialize(ProgramFeatures features)
        {
            base.Initialize(features);
            _acceleration = AccelerationOptions.FirstOrDefault(e => (e != null) && (e.Flags == ((uint)features.LtoFlash & FeatureCompatibilityHelpers.CompatibilityMask)));
            _usesFlashStorage = features.LtoFlash.HasFlag(LtoFlashFeatures.SaveDataOptional) || features.LtoFlash.HasFlag(LtoFlashFeatures.SaveDataRequired);
            _flashStorage = FlashStorageOptions.FirstOrDefault(e => UsesFlashStorage && (e != null) && features.LtoFlash.HasFlag((LtoFlashFeatures)e.Flags));
            _usesSerialPort = features.LtoFlash.HasFlag(LtoFlashFeatures.UsbPortEnhanced) || features.LtoFlash.HasFlag(LtoFlashFeatures.UsbPortRequired);
            _serialPort = SerialPortOptions.FirstOrDefault(s => UsesSerialPort && (s != null) && features.LtoFlash.HasFlag((LtoFlashFeatures)s.Flags));
            RaiseAllPropertiesChanged();
            UpdateEnableStates();
        }

        #endregion // IRomFeaturesConfigurationPage

        #region SettingsPageViewModel<T>

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
            var propertyNames = new string[]
                {
                    AccelerationPropertyName,
                    UsesFlashStoragePropertyName,
                    FlashStoragePropertyName,
                    UsesSerialPortPropertyName,
                    SerialPortPropertyName
                };
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion // SettingsPageViewModel<T>

        private void UpdateEnableStates()
        {
            if (!UsesFlashStorage)
            {
                Features.UpdateFeatureBits(FeatureCategory.LtoFlash, (uint)LtoFlashFeaturesHelpers.SaveDataMask, false);
            }
            if (!UsesSerialPort)
            {
                Features.UpdateFeatureBits(FeatureCategory.LtoFlash, (uint)LtoFlashFeaturesHelpers.UsbPortMask, false);
            }
        }
    }
}
