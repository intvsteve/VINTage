// <copyright file="CompressedArchiveAccessSettings.Gtk.cs" company="INTV Funhouse">
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

using System;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Compressed archive access settings page for GTK.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class CompressedArchiveAccessSettings : Gtk.Bin, IFakeDependencyObject
    {
        private const string Mode = "Mode";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:INTV.Shared.View.CompressedArchiveAccessSettings"/> class.
        /// </summary>
        public CompressedArchiveAccessSettings()
        {
            this.Build();
            InitializeCompressedArchiveSelection();
            InitializeAvailableCompressedArchiveFormatsTable();
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object DataContext
        {
            get
            {
                return this.GetDataContext();
            }

            set
            {
                this.SetDataContext(value);
                FinishVisualStateInitialization(value as CompressedArchiveAccessSettingsViewModel);
            }
        }

        #endregion // IFakeDependencyObject

        private CompressedArchiveAccessSettingsViewModel ViewModel
        {
            get { return DataContext as CompressedArchiveAccessSettingsViewModel; }
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object GetValue(string propertyName)
        {
            return this.GetValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <inheritdoc />
        protected override void OnDestroyed()
        {
            ViewModel.PropertyChanged -= HandleViewModelPropertyChanged;
            base.OnDestroyed();
        }

        /// <summary>
        /// Called when the compressed archive selection mode changed.
        /// </summary>
        /// <param name="sender">The radio button whose value changed.</param>
        /// <param name="e">Unused event args.</param>
        protected void HandleCompressedArchiveSelectionChanged(object sender, EventArgs e)
        {
            var selection = sender as Gtk.RadioButton;
            if (selection != null)
            {
                if (selection.Active)
                {
                    if (selection.Data.Contains(Mode))
                    {
                        var newMode = (CompressedArchiveSelection)selection.Data[Mode];
                        ViewModel.Mode = newMode;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the enable nested archives toggled.
        /// </summary>
        /// <param name="sender">The enable nested archives toggle button.</param>
        /// <param name="e">Unused event args.</param>
        protected void HandleEnableNestedArchivesToggled(object sender, EventArgs e)
        {
            var enableNestedArchives = sender as Gtk.CheckButton;
            ViewModel.EnableNestedArchives = enableNestedArchives.Active;
        }

        /// <summary>
        /// Handles the max archive size value changed.
        /// </summary>
        /// <param name="sender">The max archive size spin button.</param>
        /// <param name="e">Unused event args.</param>
        protected void HandleMaxArchiveSizeChanged(object sender, EventArgs e)
        {
            var maxArchiveSize = sender as Gtk.SpinButton;
            ViewModel.MaxCompressedArchiveSize = maxArchiveSize.ValueAsInt;
        }

        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var viewModel = sender as CompressedArchiveAccessSettingsViewModel;
            switch (e.PropertyName)
            {
                case nameof(CompressedArchiveAccessSettingsViewModel.EnableOtherOptions):
                    _enableNestedArchives.Sensitive = viewModel.EnableOtherOptions;
                    _maxArchiveSize.Sensitive = viewModel.EnableOtherOptions;
                    _maxArchiveSizeLabel.Sensitive = viewModel.EnableOtherOptions;
                    break;
                case nameof(CompressedArchiveAccessSettingsViewModel.EnableCustomModeSelection):
                    _availableCompressedArchiveFormats.Sensitive = viewModel.EnableCustomModeSelection;
                    break;
                case nameof(CompressedArchiveAccessSettingsViewModel.CompoundFormats):
                    _compoundArchiveFormats.Text = viewModel.CompoundFormats;
                    break;
                default:
                    break;
            }
        }

        private void InitializeCompressedArchiveSelection()
        {
            _compressedArchivesDisabled.Data[Mode] = CompressedArchiveSelection.Disabled;
            _compressedArchivesAll.Data[Mode] = CompressedArchiveSelection.All;
            _compressedArchivesCustom.Data[Mode] = CompressedArchiveSelection.Custom;
        }

        private void InitializeAvailableCompressedArchiveFormatsTable()
        {
            var treeView = _availableCompressedArchiveFormats;

            var column = new Gtk.TreeViewColumn();
            var toggleRenderer = new Gtk.CellRendererToggle();
            toggleRenderer.Activatable = true;
            toggleRenderer.Toggled += CompressedArchiveFormatEnabledToggled;
            Gtk.CellRenderer cellRenderer = toggleRenderer;
            column.PackStart(cellRenderer, expand: true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellToggleColumnRenderer<CompressedArchiveFormatViewModel>(l, c, m, i, f => f.Enabled));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.FixedWidth = 22;
            treeView.AppendColumn(column);

            column = new Gtk.TreeViewColumn();
            cellRenderer = new Gtk.CellRendererText();
            column.PackStart(cellRenderer, expand: true);
            column.SetCellDataFunc(cellRenderer, (l, c, m, i) => VisualHelpers.CellTextColumnRenderer<CompressedArchiveFormatViewModel>(l, c, m, i, f => f.DisplayNameWithFileExtensions));
            column.Sizing = Gtk.TreeViewColumnSizing.Fixed;
            column.Resizable = false;
            treeView.AppendColumn(column);

            var formatsListStore = new Gtk.ListStore(typeof(CompressedArchiveFormatViewModel));
            treeView.Model = formatsListStore;
        }

        private void CompressedArchiveFormatEnabledToggled(object o, Gtk.ToggledArgs args)
        {
            var store = _availableCompressedArchiveFormats.Model;
            Gtk.TreeIter iter;
            if (store.GetIter(out iter, new Gtk.TreePath(args.Path)))
            {
                var format = store.GetValue(iter, 0) as CompressedArchiveFormatViewModel;
                var currentValue = format.Enabled;
                format.Enabled = !currentValue;
            }
        }

        private void FinishVisualStateInitialization(CompressedArchiveAccessSettingsViewModel viewModel)
        {
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            switch (viewModel.Mode)
            {
                case CompressedArchiveSelection.Disabled:
                    _compressedArchivesDisabled.Active = true;
                    break;
                case CompressedArchiveSelection.All:
                    _compressedArchivesAll.Active = true;
                    break;
                case CompressedArchiveSelection.Custom:
                    _compressedArchivesCustom.Active = true;
                    break;
            }

            _availableCompressedArchiveFormats.Model.SynchronizeCollection(viewModel.CompressedArchiveFormats);
            _compoundArchiveFormats.Text = viewModel.CompoundFormats;
            _enableNestedArchives.Active = viewModel.EnableNestedArchives;
            _maxArchiveSize.Value = viewModel.MaxCompressedArchiveSize;

            var fakePropertiesChanged = new[]
            {
                nameof(CompressedArchiveAccessSettingsViewModel.EnableOtherOptions),
                nameof(CompressedArchiveAccessSettingsViewModel.EnableCustomModeSelection),
            };
            foreach (var property in fakePropertiesChanged)
            {
                HandleViewModelPropertyChanged(viewModel, new System.ComponentModel.PropertyChangedEventArgs(property));
            }
        }
    }
}
