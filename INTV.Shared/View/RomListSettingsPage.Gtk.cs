// <copyright file="RomListSettingsPage.Gtk.cs" company="INTV Funhouse">
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
    public partial class RomListSettingsPage : Gtk.Bin, IFakeDependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.RomListSettingsPage"/> class.
        /// </summary>
        public RomListSettingsPage()
        {
            this.Build();
            var directoriesStore = new Gtk.ListStore(typeof(string));
            foreach (var directory in Properties.Settings.Default.RomListSearchDirectories)
            {
                directoriesStore.AppendValues(directory);
            }

            var settingsForInit = new[]
                {
                    Properties.Settings.ValidateAtLaunchSettingName,
                    Properties.Settings.SearchForRomsAtLaunchSettingName,
                    Properties.Settings.RomListSearchDirectoriesSettingName,
                    Properties.Settings.DisplayRomFileNameForTitleSettingName,
                    Properties.Settings.ShowRomDetailsSettingName
                };
            foreach (var setting in settingsForInit)
            {
                HandleSettingChanged(null, new System.ComponentModel.PropertyChangedEventArgs(setting));
            }
            Properties.Settings.Default.PropertyChanged += HandleSettingChanged;
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

        /// <inheritdoc />
        protected override void OnDestroyed()
        {
            Properties.Settings.Default.PropertyChanged -= HandleSettingChanged;
            base.OnDestroyed();
        }

        protected void HandleValidateAtLaunchToggled(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.RomListValidateAtStartup != _validateAtLaunch.Active)
            {
                Properties.Settings.Default.RomListValidateAtStartup = _validateAtLaunch.Active;
            }
        }

        protected void HandleSearchForRomsAtLaunchToggled(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.RomListSearchForRomsAtStartup != _searchForRomsAtLaunch.Active)
            {
                Properties.Settings.Default.RomListSearchForRomsAtStartup = _searchForRomsAtLaunch.Active;
            }
        }

        protected void HandleDisplayRomFileNameForTitleToggled(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.DisplayRomFileNameForTitle != _displayRomFileNameForTitle.Active)
            {
                Properties.Settings.Default.DisplayRomFileNameForTitle = _displayRomFileNameForTitle.Active;
            }
        }

        protected void HandleShowRomDetailsToggled(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.ShowRomDetails != _showRomDetails.Active)
            {
                Properties.Settings.Default.ShowRomDetails = _showRomDetails.Active;
            }
        }

        private void HandleSettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleSettingChangedCore);
        }

        private void HandleSettingChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Properties.Settings.ValidateAtLaunchSettingName:
                    _validateAtLaunch.Active = Properties.Settings.Default.RomListValidateAtStartup;
                    break;
                case Properties.Settings.SearchForRomsAtLaunchSettingName:
                    _searchForRomsAtLaunch.Active = Properties.Settings.Default.RomListSearchForRomsAtStartup;
                    break;
                case Properties.Settings.RomListSearchDirectoriesSettingName:
                    // TODO: not implemented
                    break;
                case Properties.Settings.DisplayRomFileNameForTitleSettingName:
                    _displayRomFileNameForTitle.Active = Properties.Settings.Default.DisplayRomFileNameForTitle;
                    break;
                case Properties.Settings.ShowRomDetailsSettingName:
                    _showRomDetails.Active = Properties.Settings.Default.ShowRomDetails;
                    break;
                default:
                    break;
            }
        }
    }
}
