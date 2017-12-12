// <copyright file="DeviceFirmwarePage.Gtk.cs" company="INTV Funhouse">
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
//

using System.Collections.Generic;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Device firmware page for GTK.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceFirmwarePage : Gtk.Bin, IFakeDependencyObject
    {
        private Dictionary<RelayCommand, bool> _blockWhenBusy = new Dictionary<RelayCommand, bool>();
        private bool _updating;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.DeviceFirmwarePage"/> class.
        /// </summary>
        public DeviceFirmwarePage()
        {
            _blockWhenBusy[FirmwareCommandGroup.UpdateFirmwareCommand] = FirmwareCommandGroup.UpdateFirmwareCommand.BlockWhenAppIsBusy;
            foreach (var blockWhenBusy in _blockWhenBusy)
            {
                blockWhenBusy.Key.BlockWhenAppIsBusy = false;
            }
            this.Build();
            CommandManager.RequerySuggested += HandleRequerySuggested;
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        public LtoFlashViewModel ViewModel
        {
            get { return DataContext as LtoFlashViewModel; }
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
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <summary>
        /// Update this instance to display data from the DataContext (current device).
        /// </summary>
        /// <remarks>We track that we're updating due to changes from the ViewModel to avoid multiple changes
        /// that can happen when user changes a setting on hardware.</remarks>
        internal void Update()
        {
            try
            {
                _updating = true;
                _factoryVersion.Text = ViewModel.FirmwareRevisions.Primary.SafeString();
                _updateVersion.Text = ViewModel.FirmwareRevisions.Secondary.SafeString();
                _currentVersion.Text = ViewModel.FirmwareRevisions.Current.SafeString();
            }
            finally
            {
                _updating = false;
            }
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            foreach (var blockWhenBusy in _blockWhenBusy)
            {
                blockWhenBusy.Key.BlockWhenAppIsBusy = blockWhenBusy.Value;
            }
            CommandManager.RequerySuggested -= HandleRequerySuggested;
            base.OnDestroyed();
        }

        protected void HandleUpdateFirmwareClicked(object sender, System.EventArgs e)
        {
            if ((ViewModel != null) && !_updating && FirmwareCommandGroup.UpdateFirmwareCommand.CanExecute(ViewModel))
            {
                FirmwareCommandGroup.UpdateFirmwareCommand.Execute(ViewModel);
            }
        }

        private void HandleRequerySuggested(object sender, System.EventArgs args)
        {
            var canEdit = FirmwareCommandGroup.UpdateFirmwareCommand.CanExecute(ViewModel);
            if (_updateFirmware.Sensitive != canEdit)
            {
                _updateFirmware.Sensitive = canEdit;
            }
        }
    }
}
