// <copyright file="SystemCompatibilityConfigurationPageViewModel.cs" company="INTV Funhouse">
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
using SystemCompatibilityConfigurationVisualType = INTV.Shared.View.SystemCompatibilityConfigurationPage;
#elif MAC
using SystemCompatibilityConfigurationVisualType = INTV.Shared.View.SystemCompatibilityConfigurationPageController;
#endif // WIN

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the system compatibility settings configuration page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IRomFeaturesConfigurationPage))]
    [LocalizedName(typeof(Resources.Strings), "SystemCompatibility_Name")]
    [Weight(0.1)]
    [Icon(null)]
    public class SystemCompatibilityConfigurationPageViewModel : SettingsPageViewModel<SystemCompatibilityConfigurationVisualType>, IRomFeaturesConfigurationPage
    {
        #region Property Names

        internal const string IntellivisionIIPropertyName = "IntellivisionII";
        internal const string SuperVideoArcadePropertyName = "SuperVideoArcade";
        internal const string TutorvisionPropertyName = "Tutorvision";

        #endregion // Property Names

        #region UI Strings

        public static readonly string SystemCompatibility = Resources.Strings.SystemCompatibility_GroupName;

        #endregion // UI Strings

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SystemCompatibilityConfigurationPageViewModel type.
        /// </summary>
        public SystemCompatibilityConfigurationPageViewModel()
        {
            var selectableOptions = new FeatureCompatibility[] { FeatureCompatibility.Incompatible, FeatureCompatibility.Tolerates, FeatureCompatibility.Enhances, FeatureCompatibility.Requires };
            IntellivisionIIOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(IntellivisionIIFeatureSet.Data));
            SuperVideoArcadeOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(SuperVideoArcadeFeatureSet.Data));
            TutorvisionOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(TutorvisionFeatureSet.Data));
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the available options for Intellivision II compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> IntellivisionIIOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected Intellivision II compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel IntellivisionII
        {
            get { return _intellivisionII; }
            set { UpdateFeatureProperty(IntellivisionIIPropertyName, value, ref _intellivisionII); }
        }
        private ProgramFeatureImageViewModel _intellivisionII;

        /// <summary>
        /// Gets the available options for Sears Super Video Arcade compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> SuperVideoArcadeOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected Sears Super Video Arcade compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel SuperVideoArcade
        {
            get { return _superVideoArcade; }
            set { UpdateFeatureProperty(SuperVideoArcadePropertyName, value, ref _superVideoArcade); }
        }
        private ProgramFeatureImageViewModel _superVideoArcade;

        /// <summary>
        /// Gets the available options for Tutorvision compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> TutorvisionOptions { get; private set; }

        /// <summary>
        /// Gets or sets the selected Tutorvision compatibility option.
        /// </summary>
        public ProgramFeatureImageViewModel Tutorvision
        {
            get { return _tutorvision; }
            set { UpdateFeatureProperty(TutorvisionPropertyName, value, ref _tutorvision); }
        }
        private ProgramFeatureImageViewModel _tutorvision;

        #endregion // Properties

        #region IRomFeaturesConfigurationPage

        /// <inheritdoc />
        [OSExport("Name")]
        public string Name
        {
            get { return Resources.Strings.SystemCompatibility_Name; }
        }

        /// <inheritdoc />
        public override void Initialize(ProgramFeatures features)
        {
            base.Initialize(features);
            _intellivisionII = IntellivisionIIOptions.FirstOrDefault(n => n.Flags == (uint)features.IntellivisionII);
            _superVideoArcade = SuperVideoArcadeOptions.FirstOrDefault(p => p.Flags == (uint)features.SuperVideoArcade);
            _tutorvision = TutorvisionOptions.FirstOrDefault(p => (p != null) && (p.Flags == (uint)features.Tutorvision));
            RaiseAllPropertiesChanged();
        }

        #endregion // IRomFeaturesConfigurationPage

        #region SettingsPageViewModel<T>

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
            var propertyNames = new string[]
                {
                    IntellivisionIIPropertyName,
                    SuperVideoArcadePropertyName,
                    TutorvisionPropertyName
                };
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion // SettingsPageViewModel<T>
    }
}
