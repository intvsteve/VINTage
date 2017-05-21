// <copyright file="GeneralFeaturesConfigurationPageViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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
using GeneralFeaturesVisualType = INTV.Shared.View.GeneralFeaturesConfigurationPage;
#elif MAC
using GeneralFeaturesVisualType = INTV.Shared.View.GeneralFeaturesConfigurationPageController;
#endif // WIN

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the general settings configuration page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IRomFeaturesConfigurationPage))]
    [LocalizedName(typeof(Resources.Strings), "GeneralFeatures_Name")]
    [Weight(0.0)]
    [Icon(null)]
    public class GeneralFeaturesConfigurationPageViewModel : SettingsPageViewModel<GeneralFeaturesVisualType>, IRomFeaturesConfigurationPage
    {
        #region Property Names

        internal const string NtscPropertyName = "Ntsc";
        internal const string PalPropertyName = "Pal";
        internal const string IntellivoicePropertyName = "Intellivoice";

        #endregion // Property Names

        #region UI Strings

        public static readonly string GroupName = Resources.Strings.GeneralFeaturesConfiguration_GroupName;
        public static readonly string VideoStandards = Resources.Strings.GeneralFeaturesConfiguration_VideoStandardsGroupName;

        #endregion // UI Strings

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the GeneralFeaturesConfigurationPageViewModel type.
        /// </summary>
        public GeneralFeaturesConfigurationPageViewModel()
        {
            var selectableOptions = new FeatureCompatibility[] { FeatureCompatibility.Incompatible, FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances };
            NtscOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(NtscFeatureSet.Data));
            PalOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(PalFeatureSet.Data));
            selectableOptions = new FeatureCompatibility[] { FeatureCompatibility.Incompatible, FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances, FeatureCompatibility.Requires };
            IntellivoiceOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(IntellivoiceFeatureSet.Data));
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the available options for NTSC compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> NtscOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected NTSC compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel Ntsc
        {
            get { return _ntsc; }
            set { UpdateFeatureProperty(NtscPropertyName, value, ref _ntsc); }
        }
        private ProgramFeatureImageViewModel _ntsc;

        /// <summary>
        /// Gets the available options for PAL compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> PalOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected PAL compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel Pal
        {
            get { return _pal; }
            set { UpdateFeatureProperty(PalPropertyName, value, ref _pal); }
        }
        private ProgramFeatureImageViewModel _pal;

        /// <summary>
        /// Gets the available options for Intellivoice compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> IntellivoiceOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected Intellivoice compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel Intellivoice
        {
            get { return _intellivoice; }
            set { UpdateFeatureProperty(IntellivoicePropertyName, value, ref _intellivoice); }
        }
        private ProgramFeatureImageViewModel _intellivoice;

        #endregion // Properties

        #region IRomFeaturesConfigurationPage

        [OSExport("Name")]
        public string Name
        {
            get { return Resources.Strings.GeneralFeatures_Name; }
        }

        /// <inheritdoc />
        public override void Initialize(ProgramFeatures features)
        {
            base.Initialize(features);
            _ntsc = NtscOptions.FirstOrDefault(n => n.Flags == (uint)features.Ntsc);
            _pal = PalOptions.FirstOrDefault(p => p.Flags == (uint)features.Pal);
            _intellivoice = IntellivoiceOptions.FirstOrDefault(p => (p != null) && (p.Flags == (uint)features.Intellivoice));
            RaiseAllPropertiesChanged();
        }

        #endregion // IRomFeaturesConfigurationPage

        #region SettingsPageViewModel<T>

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
            var propertyNames = new string[]
                {
                    NtscPropertyName,
                    PalPropertyName,
                    IntellivoicePropertyName
                };
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion // SettingsPageViewModel<T>
    }
}
