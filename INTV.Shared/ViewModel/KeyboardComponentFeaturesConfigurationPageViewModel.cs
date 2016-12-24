// <copyright file="KeyboardComponentFeaturesConfigurationPageViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2015 All Rights Reserved
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
using KeyboardComponentFeaturesVisualType = INTV.Shared.View.KeyboardComponentFeaturesConfigurationPage;
#elif MAC
using KeyboardComponentFeaturesVisualType = INTV.Shared.View.KeyboardComponentFeaturesConfigurationPageController;
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the Keyboard Component settings configuration page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IRomFeaturesConfigurationPage))]
    [LocalizedName(typeof(Resources.Strings), "KeyboardComponentFeatures_Name")]
    [Weight(0.6)]
    [Icon(null)]
    public class KeyboardComponentFeaturesConfigurationPageViewModel : SettingsPageViewModel<KeyboardComponentFeaturesVisualType>, IRomFeaturesConfigurationPage
    {
        #region Property Names

        internal const string KeyboardComponentPropertyName = "KeyboardComponent";
        internal const string EnableFeatureEditingPropertyName = "EnableFeatureEditing";
        internal const string UsesCassettePropertyName = "UsesCassette";
        internal const string CassettePropertyName = "Cassette";
        internal const string EnableCassetteOptionSelectionPropertyName = "EnableCassetteOptionSelection";
        internal const string UsesMicrophonePropertyName = "UsesMicrophone";
        internal const string UsesPrinterPropertyName = "UsesPrinter";
        internal const string BasicPropertyName = "Basic";

        #endregion // Property Names

        #region UI Strings

        public static readonly string KeyboardComponentCompatibility = Resources.Strings.KeyboardComponentFeatures_Name;
        public static readonly string GroupName = Resources.Strings.KeyboardComponentFeatures_GroupName;
        public static readonly string Compatibility = Resources.Strings.Compatibility;
        public static readonly string UsesMicrophoneText = Resources.Strings.KeyboardComponentFeatures_Microphone;
        public static readonly string UsesPrinterText = Resources.Strings.EcsFeatures_Printer;
        public static readonly string UsesCassetteText = Resources.Strings.KeyboardComponentFeatures_Cassette;
        public static readonly string BasicText = Resources.Strings.KeyboardComponentFeatures_Basic;

        #endregion // UI Strings

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the KeyboardComponentFeaturesConfigurationPageViewModel type.
        /// </summary>
        public KeyboardComponentFeaturesConfigurationPageViewModel()
        {
            var selectableOptions = new KeyboardComponentFeatures[] { KeyboardComponentFeatures.Incompatible, KeyboardComponentFeatures.Tolerates, KeyboardComponentFeatures.Enhances, KeyboardComponentFeatures.Requires };
            KeyboardComponentOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(KeyboardComponentFeatureSet.Data));
            selectableOptions = new KeyboardComponentFeatures[] { KeyboardComponentFeatures.TapeOptional, KeyboardComponentFeatures.TapeRequired };
            CassetteOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(KeyboardComponentFeatureSet.Data));
            selectableOptions = new KeyboardComponentFeatures[] { KeyboardComponentFeatures.BasicIncompatible, KeyboardComponentFeatures.BasicTolerated, KeyboardComponentFeatures.BasicRequired };
            BasicOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(KeyboardComponentFeatureSet.Data));
        }

        #endregion // Constructor

        #region Properties

        #region Compatibility

        /// <summary>
        /// Gets the available options for Keyboard Component compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> KeyboardComponentOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected Keyboard Component compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel KeyboardComponent
        {
            get { return _keyboardComponent; }
            set { UpdateFeatureProperty(KeyboardComponentPropertyName, value, ref _keyboardComponent, (s, v) => UpdateCompatibility((KeyboardComponentFeatures)v.Flags)); }
        }
        private ProgramFeatureImageViewModel _keyboardComponent;

        /// <summary>
        /// Gets a value indicating whether Keyboard Component feature configuration is available.
        /// </summary>
        public bool EnableFeatureEditing
        {
            get { return _enableFeatureEditing; }
            private set { AssignAndUpdateProperty(EnableFeatureEditingPropertyName, value, ref _enableFeatureEditing); }
        }
        private bool _enableFeatureEditing;

        #endregion // Compatibility

        #region Cassette Usage

        /// <summary>
        /// Gets or sets a value indicating whether the Keyboard Component cassette is used.
        /// </summary>
        public bool UsesCassette
        {
            get { return _usesCassette; }
            set { AssignAndUpdateProperty(UsesCassettePropertyName, value, ref _usesCassette, (s, v) => UpdateCassetteOptions(v)); }
        }
        private bool _usesCassette;

        /// <summary>
        /// Gets a value indicating whether Keyboard Component cassette options can be configured.
        /// </summary>
        public bool EnableCassetteOptionSelection
        {
            get { return _enableCassetteOptionSelection; }
            private set { AssignAndUpdateProperty(EnableCassetteOptionSelectionPropertyName, value, ref _enableCassetteOptionSelection); }
        }
        private bool _enableCassetteOptionSelection;

        /// <summary>
        /// Gets the available options for Keyboard Component cassette compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> CassetteOptions { get; private set; }

        /// <summary>
        /// Gets or sets the Keyboard Component cassette usage option.
        /// </summary>
        public ProgramFeatureImageViewModel Cassette
        {
            get { return _cassette; }
            set { UpdateFeatureProperty(CassettePropertyName, value, ref _cassette, (s, v) => MarkRequiredIfNeeded()); }
        }
        private ProgramFeatureImageViewModel _cassette;

        #endregion // Cassette Usage

        #region Microphone

        /// <summary>
        /// Gets or sets a value indicating whether the Keyboard Component microphone is used.
        /// </summary>
        public bool UsesMicrophone
        {
            get { return _usesMicrophone; }
            set { AssignAndUpdateProperty(UsesMicrophonePropertyName, value, ref _usesMicrophone, (s, v) => Features.UpdateFeatureBits(FeatureCategory.KeyboardComponent, (uint)KeyboardComponentFeatures.Microphone, v)); }
        }
        private bool _usesMicrophone;

        #endregion // Microphone

        #region // Printer

        /// <summary>
        /// Gets or sets a value indicating whether the Keyboard Component printer is used.
        /// </summary>
        public bool UsesPrinter
        {
            get { return _usesPrinter; }
            set { AssignAndUpdateProperty(UsesPrinterPropertyName, value, ref _usesPrinter, (s, v) => Features.UpdateFeatureBits(FeatureCategory.KeyboardComponent, (uint)KeyboardComponentFeatures.Printer, v)); }
        }
        private bool _usesPrinter;

        #endregion // Printer

        #region Microsoft BASIC

        /// <summary>
        /// Gets the available options for Keyboard Component compatibility with Microsoft BASIC cartridge.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> BasicOptions { get; private set; }

        /// <summary>
        /// Gets or sets the Microsoft BASIC cartridge compatibility.
        /// </summary>
        public ProgramFeatureImageViewModel Basic
        {
            get { return _basic; }
            set { UpdateFeatureProperty(BasicPropertyName, value, ref _basic, (s, v) => MarkRequiredIfNeeded()); }
        }
        private ProgramFeatureImageViewModel _basic;

        #endregion // Microsoft BASIC

        #endregion // Properties

        #region IRomFeaturesConfigurationPage

        /// <inheritdoc />
        [OSExport("Name")]
        public string Name
        {
            get { return Resources.Strings.KeyboardComponentFeatures_Name; }
        }

        /// <inheritdoc />
        public override void Initialize(ProgramFeatures features)
        {
            base.Initialize(features);
            _keyboardComponent = KeyboardComponentOptions.FirstOrDefault(e => (e != null) && (e.Flags == ((uint)features.KeyboardComponent & FeatureCompatibilityHelpers.CompatibilityMask)));
            _usesMicrophone = features.KeyboardComponent.HasFlag(KeyboardComponentFeatures.Microphone);
            _usesPrinter = features.KeyboardComponent.HasFlag(KeyboardComponentFeatures.Printer);
            _usesCassette = features.KeyboardComponent.HasFlag(KeyboardComponentFeatures.TapeOptional) || features.KeyboardComponent.HasFlag(KeyboardComponentFeatures.TapeRequired);
            _cassette = CassetteOptions.FirstOrDefault(s => s.Flags == (uint)(features.KeyboardComponent & KeyboardComponentFeaturesHelpers.TapeMask));
            _basic = BasicOptions.FirstOrDefault(s => s.Flags == (uint)(features.KeyboardComponent & KeyboardComponentFeaturesHelpers.BasicMask));
            RaiseAllPropertiesChanged();
            UpdateCompatibility((KeyboardComponentFeatures)_keyboardComponent.Flags);
        }

        #endregion // IRomFeaturesConfigurationPage

        #region SettingsPageViewModel<T>

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
             var propertyNames = new string[]
                {
                    KeyboardComponentPropertyName,
                    EnableFeatureEditingPropertyName,
                    UsesCassettePropertyName,
                    CassettePropertyName,
                    EnableCassetteOptionSelectionPropertyName,
                    UsesMicrophonePropertyName,
                    UsesPrinterPropertyName,
                    BasicPropertyName
                };
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion // SettingsPageViewModel<T>

        private void UpdateCompatibility(KeyboardComponentFeatures compatibility)
        {
            EnableFeatureEditing = compatibility.HasFlag(KeyboardComponentFeatures.Enhances);
            EnableCassetteOptionSelection = EnableFeatureEditing && UsesCassette;
            if (!EnableFeatureEditing)
            {
                UsesCassette = false;
                UsesPrinter = false;
                UsesMicrophone = false;
                Basic = null;
            }
            else
            {
                if (Basic == null)
                {
                    Basic = BasicOptions.First(b => b.Flags == (uint)KeyboardComponentFeatures.BasicTolerated);
                }
                if (compatibility == KeyboardComponentFeatures.Enhances)
                {
                    if ((Basic != null) && (Basic.Flags == (uint)KeyboardComponentFeatures.BasicRequired))
                    {
                        Basic = BasicOptions.First(b => b.Flags == (uint)KeyboardComponentFeatures.BasicTolerated);
                    }
                    if ((Cassette != null) && (Cassette.Flags == (uint)KeyboardComponentFeatures.TapeRequired))
                    {
                        Cassette = CassetteOptions.First(c => c.Flags == (uint)KeyboardComponentFeatures.TapeOptional);
                    }
                }
            }
        }

        private void UpdateCassetteOptions(bool usesCassette)
        {
            if (usesCassette && (Cassette == null))
            {
                Cassette = CassetteOptions.First(c => c.Flags == (uint)KeyboardComponentFeatures.TapeOptional);
            }
            else if (!usesCassette)
            {
                Cassette = null;
            }
            EnableCassetteOptionSelection = EnableFeatureEditing && UsesCassette;
        }

        private void UpdateBasicOptions(bool usesBasic)
        {
            if (usesBasic && (Basic == null))
            {
                Basic = BasicOptions.First(c => c.Flags == (uint)KeyboardComponentFeatures.BasicTolerated);
            }
        }

        private void MarkRequiredIfNeeded()
        {
            var markRequired = (Cassette != null) && ((KeyboardComponentFeatures)Cassette.Flags).HasFlag(KeyboardComponentFeatures.TapeRequired);
            markRequired |= (Basic != null) && ((KeyboardComponentFeatures)Basic.Flags).HasFlag(KeyboardComponentFeatures.BasicRequired);
            if (markRequired)
            {
                KeyboardComponent = KeyboardComponentOptions.First(k => k.Flags == (uint)KeyboardComponentFeatures.Requires);
            }
        }
    }
}
