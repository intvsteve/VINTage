// <copyright file="DeviceInformationPage.Gtk.cs" company="INTV Funhouse">
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

using System;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Device information page visual for GTK.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceInformationPage : Gtk.Bin, IFakeDependencyObject
    {
        private TextCellInPlaceEditor _nameEditor;
        private TextCellInPlaceEditor _ownerEditor;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.DeviceInformationPage"/> class.
        /// </summary>
        public DeviceInformationPage()
        {
            this.Build();
            _nameEditor = new TextCellInPlaceEditor(_deviceName, FileSystemConstants.MaxShortNameLength);
            _nameEditor.EditorClosed += DeviceNameEditorClosed;
            _ownerEditor = new TextCellInPlaceEditor(_deviceOwner, FileSystemConstants.MaxLongNameLength);
            _ownerEditor.EditorClosed += DeviceOwnerEditorClosed;
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
        /// Sets the device name Gtk.Entry text.
        /// </summary>
        /// <param name="name">Name of the device.</param>
        public void SetDeviceName(string name)
        {
            _deviceName.Text = name;
        }

        /// <summary>
        /// Sets the device owner Gtk.Entry text.
        /// </summary>
        /// <param name="owner">Owner of the device.</param>
        public void SetDeviceOwner(string owner)
        {
            _deviceOwner.Text = owner;
        }

        /// <summary>
        /// Sets the device unique identifier text.
        /// </summary>
        /// <param name="uniqueId">Unique identifier of the device.</param>
        public void SetUniqueId(string uniqueId)
        {
            _deviceUniqueId.Text = uniqueId;
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            CommandManager.RequerySuggested -= HandleRequerySuggested;
            _nameEditor.EditorClosed -= DeviceNameEditorClosed;
            _nameEditor.Dispose();
            _nameEditor = null;
            _ownerEditor.EditorClosed -= DeviceOwnerEditorClosed;
            _ownerEditor.Dispose();
            _ownerEditor = null;
            base.OnDestroyed();
        }

        private void DeviceNameEditorClosed(object sender, INTV.Shared.Behavior.InPlaceEditorClosedEventArgs e)
        {
            if (e.CommitedChanges)
            {
                ViewModel.ActiveLtoFlashDevice.Name = _deviceName.Text;
                DeviceCommandGroup.SetDeviceNameCommand.Execute(ViewModel);
            }
            else
            {
                _deviceName.Text = ViewModel.ActiveLtoFlashDevice.Name;
            }
        }

        private void DeviceOwnerEditorClosed(object sender, INTV.Shared.Behavior.InPlaceEditorClosedEventArgs e)
        {
            if (e.CommitedChanges)
            {
                ViewModel.ActiveLtoFlashDevice.Owner = _deviceOwner.Text;
                DeviceCommandGroup.SetDeviceOwnerCommand.Execute(ViewModel);
            }
            else
            {
                _deviceOwner.Text = ViewModel.ActiveLtoFlashDevice.Owner;
            }
        }

        private void HandleRequerySuggested(object sender, System.EventArgs args)
        {
            var canEdit = DeviceCommandGroup.SetDeviceNameCommand.CanExecute(ViewModel);
            if (_deviceName.IsEditable != canEdit)
            {
                _deviceName.IsEditable = canEdit;
            }

            canEdit = DeviceCommandGroup.SetDeviceOwnerCommand.CanExecute(ViewModel);
            if (_deviceOwner.IsEditable != canEdit)
            {
                _deviceOwner.IsEditable = canEdit;
            }
        }
    }
}
