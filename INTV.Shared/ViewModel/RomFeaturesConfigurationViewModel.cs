// <copyright file="RomFeaturesConfigurationViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2015-2016 All Rights Reserved
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
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Shared.Behavior;
using INTV.Shared.ComponentModel;

#if WIN
using OSVisual = System.Windows.FrameworkElement;
#elif MAC
using OSVisual = MonoMac.AppKit.NSViewController;
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for ROM features configuration dialog.
    /// </summary>
    public partial class RomFeaturesConfigurationViewModel : ViewModelBase, System.ComponentModel.Composition.IPartImportsSatisfiedNotification
    {
        private readonly Dictionary<IRomFeaturesConfigurationPage, OSVisual> _pageVisuals;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RomFeaturesConfigurationViewModel type.
        /// </summary>
        public RomFeaturesConfigurationViewModel()
        {
            _pageVisuals = new Dictionary<IRomFeaturesConfigurationPage, OSVisual>();
#if DEBUG && WIN
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
#endif
            {
                this.DoImport();
            }
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets or sets (set via MEF) the known feature configuration pages.
        /// </summary>
        [System.ComponentModel.Composition.ImportMany]
        public IEnumerable<Lazy<IRomFeaturesConfigurationPage, ISettingsPageMetadata>> FeatureGroupImplementations { get; set; }

        /// <summary>
        /// Gets the feature groups that can be configured.
        /// </summary>
        public ObservableCollection<IRomFeaturesConfigurationPage> FeatureGroups { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected feature group configuration page in the list.
        /// </summary>
        public IRomFeaturesConfigurationPage CurrentSelection
        {
            get { return _currentSelection; }
            set { AssignAndUpdateProperty("CurrentSelection", value, ref _currentSelection, (s, v) => SelectedCategoryChanged(v)); }
        }
        private IRomFeaturesConfigurationPage _currentSelection;

        /// <summary>
        /// Gets the visual for the currently selected feature group.
        /// </summary>
        public OSVisual CurrentSelectionVisual
        {
            get { return _currentSelectionVisual; }
            private set { AssignAndUpdateProperty("CurrentSelectionVisual", value, ref _currentSelectionVisual); }
        }
        private OSVisual _currentSelectionVisual;

        /// <summary>
        /// Gets the in-place editor (Adorner) hosting this feature editor.
        /// </summary>
        public IInPlaceEditor InPlaceEditor { get; private set; }

        /// <summary>
        /// Gets the overall desired width for the editor visuals.
        /// </summary>
        public double DesiredWidth
        {
            get { return _desiredWidth; }
            private set { AssignAndUpdateProperty("DesiredWidth", value, ref _desiredWidth); }
        }
        private double _desiredWidth;

        /// <summary>
        /// Gets the overall desired height for the editor visuals.
        /// </summary>
        public double DesiredHeight
        {
            get { return _desiredHeight; }
            private set { AssignAndUpdateProperty("DesiredHeight", value, ref _desiredHeight); }
        }
        private double _desiredHeight;

        /// <summary>
        /// Gets a value indicating whether the 'revert to default' option should be shown.
        /// </summary>
        public bool ShowRevertToDefault
        {
            get { return _showRevertToDefault; }
            private set { AssignAndUpdateProperty("ShowRevertToDefault", value, ref _showRevertToDefault); }
        }
        private bool _showRevertToDefault;

        /// <summary>
        /// Gets the description being edited.
        /// </summary>
        public ProgramDescription Description
        {
            get { return _description; }
            private set { AssignAndUpdateProperty("Description", value, ref _description); }
        }
        private ProgramDescription _description;

        private ProgramFeatures OriginalFeatures { get; set; }
        private ProgramFeatures EditFeatures { get; set; }
        private ProgramFeatures EditsPriorToRevertToDatabase { get; set; }

        #endregion // Properties

        /// <summary>
        /// Initialize the view model and all of the known feature page editors.
        /// </summary>
        /// <param name="description">The description of the program whose features are being edited.</param>
        /// <param name="inplaceEditor">The in-place editor hosting this editor. Used for lifetime control.</param>
        public void Initialize(ProgramDescription description, IInPlaceEditor inplaceEditor)
        {
            InPlaceEditor = inplaceEditor;
            Description = description;
            ShowRevertToDefault = false;
            switch (description.ProgramInformation.DataOrigin)
            {
                case ProgramInformationOrigin.UserDefined:
                case ProgramInformationOrigin.None:
                    break;
                default:
                    ShowRevertToDefault = true;
                    break;
            }
            OriginalFeatures = description.Features.Clone();
            EditFeatures = OriginalFeatures.Clone();
            EditsPriorToRevertToDatabase = null;
            InitializeToEditFeatures();
        }

        /// <summary>
        /// Reverts any changes made to the features, resetting to values from the built-in program database.
        /// </summary>
        public void RevertToDatabase()
        {
            EditsPriorToRevertToDatabase = EditFeatures.Clone();
            EditFeatures = Description.ProgramInformation.Features.Clone();
            InitializeToEditFeatures();
        }

        /// <summary>
        /// Reverts any changes made to the features, resetting to values originally supplied to the editor.
        /// </summary>
        public void RevertChanges()
        {
            EditFeatures = OriginalFeatures.Clone();
            EditsPriorToRevertToDatabase = null;
            InitializeToEditFeatures();
        }

        /// <summary>
        /// Commits changes (if any) to the features of the ProgramDescription supplied to the editor.
        /// </summary>
        /// <returns><c>true</c> if any changes were made.</returns>
        public bool CommitChangesToProgramDescription()
        {
            var anyChanges = OriginalFeatures.CompareTo(EditFeatures) != 0;
            if (anyChanges)
            {
                var revertedToDatabase = false;
                if (EditsPriorToRevertToDatabase != null)
                {
                    revertedToDatabase = Description.ProgramInformation.Features.Clone().CompareTo(EditFeatures) == 0;
                }
                Description.Features = EditFeatures;
                INTV.Shared.Model.Program.ProgramCollection.Roms.ReportProgramFeaturesChanged(new[] { Description }, revertedToDatabase);
            }
            return anyChanges;
        }

        #region IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            var defaultFeatures = new ProgramFeatures();
            var featureGroups = new List<Tuple<string, double, IRomFeaturesConfigurationPage>>();
            foreach (var featureGroup in FeatureGroupImplementations)
            {
                var featurePage = featureGroup.Value;
                featurePage.Initialize(defaultFeatures);
                featureGroups.Add(new Tuple<string, double, IRomFeaturesConfigurationPage>(featureGroup.Metadata.Name, featureGroup.Metadata.Weight, featurePage));
                PreparePage(featurePage);
            }
            FeatureGroups = new ObservableCollection<IRomFeaturesConfigurationPage>(featureGroups.OrderBy(g => g.Item2).Select(g => g.Item3));
            CurrentSelection = FeatureGroups.First();
        }

        #endregion // IPartImportsSatisfiedNotification Members

        private void InitializeToEditFeatures()
        {
            foreach (var featureGroup in FeatureGroups)
            {
                featureGroup.Initialize(EditFeatures);
            }
        }

        private void SelectedCategoryChanged(IRomFeaturesConfigurationPage newPage)
        {
            if (newPage != null)
            {
                OSVisual pageVisual;
                if (!_pageVisuals.TryGetValue(newPage, out pageVisual))
                {
                    pageVisual = newPage.CreateVisual();
                    _pageVisuals[newPage] = pageVisual;
                }
                CurrentSelectionVisual = pageVisual;
                var values = _pageVisuals.Values;
            }
        }

        /// <summary>
        /// Prepare the page for use. This is the final stage of initialization.
        /// </summary>
        /// <param name="page">The page for the ROM features configuration dialog to prepare.</param>
        partial void PreparePage(IRomFeaturesConfigurationPage page);
    }
}
