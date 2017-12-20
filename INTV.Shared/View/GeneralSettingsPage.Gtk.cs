// <copyright file="GeneralSettingsPage.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class GeneralSettingsPage : Gtk.Bin, IFakeDependencyObject
    {
        private static readonly string Entry = "Entry";

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.GeneralSettingsPage"/> class.
        /// </summary>
        public GeneralSettingsPage()
        {
            this.Build();
            _checkForUpdates.Active = Properties.Settings.Default.CheckForAppUpdatesAtLaunch;
            _showDetailedErrors.Active = Properties.Settings.Default.ShowDetailedErrors;
        }

        #region IFakeDependencyObject

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

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

        protected override void OnRealized()
        {
            base.OnRealized();

            var viewModel = DataContext as INTV.Shared.ViewModel.GeneralSettingsPageViewModel;

            _romsPath.Text = viewModel.RomsDir;
            _showRomsDir.SetValue(Entry, _romsPath);

            _manualsPath.Text = viewModel.ManualsDir;
            _showManualsDir.SetValue(Entry, _manualsPath);

            _overlaysPath.Text = viewModel.OverlaysDir;
            _showOverlaysDir.SetValue(Entry, _overlaysPath);

            _boxesPath.Text = viewModel.BoxesDir;
            _showBoxesDir.SetValue(Entry, _boxesPath);

            _labelsPath.Text = viewModel.LabelsDir;
            _showLabelsDir.SetValue(Entry, _labelsPath);

            _backupsPath.Text = viewModel.BackupDir;
            _showBackupsDir.SetValue(Entry, _backupsPath);

            _errorLogsPath.Text = viewModel.ErrorLogDir;
            _showErrorLogsDir.SetValue(Entry, _errorLogsPath);
        }

        private void HandleShowButton(object sender, System.EventArgs e)
        {
            var button = sender as Gtk.Button;
            var entry = button.GetValue(Entry) as Gtk.Entry;
            if (entry != null)
            {
                entry.Text.RevealInFileSystem();
            }
        }

        protected void HandleCheckForUpdatesToggle(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.CheckForAppUpdatesAtLaunch != _checkForUpdates.Active)
            {
                Properties.Settings.Default.CheckForAppUpdatesAtLaunch = _checkForUpdates.Active;
            }
        }

        protected void HandleShowDetailedErrorsToggle(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.ShowDetailedErrors != _showDetailedErrors.Active)
            {
                Properties.Settings.Default.ShowDetailedErrors = _showDetailedErrors.Active;
            }
        }
    }
}
