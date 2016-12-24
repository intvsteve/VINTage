// <copyright file="EcsFeaturesConfigurationPageViewModel.cs" company="INTV Funhouse">
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

#if WIN
using EcsFeaturesVisualType = INTV.Shared.View.EcsFeaturesConfigurationPage;
#elif MAC
using EcsFeaturesVisualType = INTV.Shared.View.EcsFeaturesConfigurationPageController;
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the ECS settings configuration page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IRomFeaturesConfigurationPage))]
    [LocalizedName(typeof(Resources.Strings), "EcsFeatures_Name")]
    [Weight(0.2)]
    [Icon(null)]
    public class EcsFeaturesConfigurationPageViewModel : SettingsPageViewModel<EcsFeaturesVisualType>, IRomFeaturesConfigurationPage
    {
        #region Property Names

        internal const string EcsPropertyName = "Ecs";
        internal const string EnableFeatureEditingPropertyName = "EnableFeatureEditing";
        internal const string UsesCassettePropertyName = "UsesCassette";
        internal const string UsesSynthesizerPropertyName = "UsesSynthesizer";
        internal const string UsesPrinterPropertyName = "UsesPrinter";
        internal const string UsesSerialPortPropertyName = "UsesSerialPort";
        internal const string SerialPortPropertyName = "SerialPort";

        #endregion // Property Names

        #region UI Strings

        public static readonly string EcsCompatibility = Resources.Strings.EcsFeatures_GroupName;
        public static readonly string Compatibility = Resources.Strings.Compatibility;
        public static readonly string UsesCassetteText = Resources.Strings.EcsFeatures_Tape;
        public static readonly string UsesSynthesizerText = Resources.Strings.EcsFeatures_Synthesizer;
        public static readonly string UsesPrinterText = Resources.Strings.EcsFeatures_Printer;
        public static readonly string UsesSerialPortText = Resources.Strings.EcsFeatures_UsesSerialPort;

        #endregion // UI Strings

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EcsFeaturesConfigurationPageViewModel type.
        /// </summary>
        public EcsFeaturesConfigurationPageViewModel()
        {
            var selectableOptions = new EcsFeatures[] { EcsFeatures.Incompatible, EcsFeatures.Tolerates, EcsFeatures.Enhances, EcsFeatures.Requires };
            EcsOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(EcsFeatureSet.Data));
            selectableOptions = new EcsFeatures[] { EcsFeatures.SerialPortEnhanced, EcsFeatures.SerialPortRequired };
            SerialPortOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(EcsFeatureSet.Data));
        }

        #endregion // Constructor

        #region Properties

        #region Compatibility

        /// <summary>
        /// Gets the available options for ECS compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> EcsOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected ECS compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel Ecs
        {
            get { return _ecs; }
            set { UpdateFeatureProperty(EcsPropertyName, value, ref _ecs, (s, v) => UpdateCompatibility((EcsFeatures)v.Flags)); }
        }
        private ProgramFeatureImageViewModel _ecs;

        /// <summary>
        /// Gets a value indicating whether specific ECS feature configuration is available.
        /// </summary>
        public bool EnableFeatureEditing
        {
            get { return _enableFeatureEditing; }
            private set { AssignAndUpdateProperty(EnableFeatureEditingPropertyName, value, ref _enableFeatureEditing); }
        }
        private bool _enableFeatureEditing;

        #endregion // Compatibility

        #region Cassette

        /// <summary>
        /// Gets or sets a value indicating whether ECS cassette features are used.
        /// </summary>
        public bool UsesCassette
        {
            get { return _usesCassette; }
            set { AssignAndUpdateProperty(UsesCassettePropertyName, value, ref _usesCassette, (s, v) => Features.UpdateFeatureBits(FeatureCategory.Ecs, (uint)EcsFeatures.Tape, v)); }
        }
        private bool _usesCassette;

        #endregion // Cassette

        #region Synthesizer

        /// <summary>
        /// Gets or sets a value indicating whether the ECS Music Synthesizer add-on is used.
        /// </summary>
        public bool UsesSynthesizer
        {
            get { return _usesSynthesizer; }
            set { AssignAndUpdateProperty(UsesSynthesizerPropertyName, value, ref _usesSynthesizer, (s, v) => Features.UpdateFeatureBits(FeatureCategory.Ecs, (uint)EcsFeatures.Synthesizer, v)); }
        }
        private bool _usesSynthesizer;

        #endregion // Synthesizer

        #region Printer

        /// <summary>
        /// Gets or sets a value indicating whether the ECS printer feature is used.
        /// </summary>
        public bool UsesPrinter
        {
            get { return _usesPrinter; }
            set { AssignAndUpdateProperty(UsesPrinterPropertyName, value, ref _usesPrinter, (s, v) => Features.UpdateFeatureBits(FeatureCategory.Ecs, (uint)EcsFeatures.Printer, v)); }
        }
        private bool _usesPrinter;

        #endregion // Printer

        #region Serial Port Usage

        /// <summary>
        /// Gets or sets a value indicating whether the ECS's serial port capabilities are used.
        /// </summary>
        public bool UsesSerialPort
        {
            get { return _usesSerialPort; }
            set { AssignAndUpdateProperty(UsesSerialPortPropertyName, value, ref _usesSerialPort, (s, v) => UpdateSerialPortOptions(v)); }
        }
        private bool _usesSerialPort;

        /// <summary>
        /// Gets a value indicating whether ECS serial port configuration options are available.
        /// </summary>
        public bool EnableSerialPortOptionSelection
        {
            get { return _enableSerialPortOptionSelection; }
            private set { AssignAndUpdateProperty("EnableSerialPortOptionSelection", value, ref _enableSerialPortOptionSelection); }
        }
        private bool _enableSerialPortOptionSelection;

        /// <summary>
        /// Gets the available options for ECS serial port compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> SerialPortOptions { get; private set; }

        /// <summary>
        /// Gets or sets the ECS serial port usage.
        /// </summary>
        public ProgramFeatureImageViewModel SerialPort
        {
            get { return _serialPort; }
            set { UpdateFeatureProperty(SerialPortPropertyName, value, ref _serialPort, (s, v) => MarkRequiredIfNeeded()); }
        }
        private ProgramFeatureImageViewModel _serialPort;

        #endregion // Serial Port Usage

        #endregion // Properties

        #region IRomFeaturesConfigurationPage

        /// <inheritdoc />
        [OSExport("Name")]
        public string Name
        {
            get { return Resources.Strings.EcsFeatures_Name; }
        }

        /// <inheritdoc />
        public override void Initialize(ProgramFeatures features)
        {
            base.Initialize(features);
            _ecs = EcsOptions.FirstOrDefault(e => (e != null) && (e.Flags == ((uint)features.Ecs & FeatureCompatibilityHelpers.CompatibilityMask)));
            _usesCassette = features.Ecs.HasFlag(EcsFeatures.Tape);
            _usesSynthesizer = features.Ecs.HasFlag(EcsFeatures.Synthesizer);
            _usesPrinter = features.Ecs.HasFlag(EcsFeatures.Printer);
            _usesSerialPort = features.Ecs.HasFlag(EcsFeatures.SerialPortEnhanced) || features.Ecs.HasFlag(EcsFeatures.SerialPortRequired);
            _serialPort = SerialPortOptions.FirstOrDefault(s => UsesSerialPort && (s != null) && features.Ecs.HasFlag((EcsFeatures)s.Flags));
            RaiseAllPropertiesChanged();
            UpdateCompatibility((EcsFeatures)_ecs.Flags);
            UpdateSerialPortOptions(_usesSerialPort);
        }

        #endregion // IRomFeaturesConfigurationPage

        #region SettingsPageViewModel<T>

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
            var propertyNames = new string[]
                {
                    EcsPropertyName,
                    EnableFeatureEditingPropertyName,
                    UsesCassettePropertyName,
                    UsesSynthesizerPropertyName,
                    UsesPrinterPropertyName,
                    UsesSerialPortPropertyName,
                    SerialPortPropertyName
                };
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion // SettingsPageViewModel<T>

        private void UpdateCompatibility(EcsFeatures compatibility)
        {
            EnableFeatureEditing = compatibility.HasFlag(EcsFeatures.Enhances);
            EnableSerialPortOptionSelection = EnableFeatureEditing && UsesSerialPort;
            if (!EnableFeatureEditing)
            {
                UsesCassette = false;
                UsesSynthesizer = false;
                UsesPrinter = false;
                UsesSerialPort = false;
                SerialPort = null;
            }
            else
            {
                if (compatibility == EcsFeatures.Enhances)
                {
                    if ((SerialPort != null) && (SerialPort.Flags == (uint)EcsFeatures.SerialPortRequired))
                    {
                        SerialPort = SerialPortOptions.First(p => p.Flags == (uint)EcsFeatures.SerialPortEnhanced);
                    }
                }
            }
        }

        private void UpdateSerialPortOptions(bool usesSerialPort)
        {
            if (usesSerialPort && (SerialPort == null))
            {
                SerialPort = SerialPortOptions.First(p => p.Flags == (uint)EcsFeatures.SerialPortEnhanced);
            }
            else if (!usesSerialPort)
            {
                SerialPort = null;
            }
            EnableSerialPortOptionSelection = EnableFeatureEditing && UsesSerialPort;
        }

        private void MarkRequiredIfNeeded()
        {
            var markRequired = (SerialPort != null) && ((EcsFeatures)SerialPort.Flags).HasFlag(EcsFeatures.SerialPortRequired);
            if (markRequired)
            {
                Ecs = EcsOptions.First(e => e.Flags == (uint)EcsFeatures.Requires);
            }
        }
    }
}
