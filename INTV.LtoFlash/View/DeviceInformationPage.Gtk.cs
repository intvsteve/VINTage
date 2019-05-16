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
using System.Collections.Generic;
using System.Linq;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Device information page visual for GTK.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceInformationPage : Gtk.Bin, IFakeDependencyObject
    {
        private Dictionary<RelayCommand, bool> _blockWhenBusy = new Dictionary<RelayCommand, bool>();
        private bool _updating;
        private bool _committing;
        private TextCellInPlaceEditor _nameEditor;
        private TextCellInPlaceEditor _ownerEditor;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.DeviceInformationPage"/> class.
        /// </summary>
        public DeviceInformationPage()
        {
            // Don't want to block this due to the dialog being up, which is the default behavior
            // for dialogs. Save the value and then restore when destroyed.
            _blockWhenBusy[DeviceCommandGroup.SetDeviceNameCommand] = DeviceCommandGroup.SetDeviceNameCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetDeviceOwnerCommand] = DeviceCommandGroup.SetDeviceOwnerCommand.BlockWhenAppIsBusy;
            foreach (var blockWhenBusy in _blockWhenBusy)
            {
                blockWhenBusy.Key.BlockWhenAppIsBusy = false;
            }
            this.Build();
            _deviceName.TooltipText = DeviceCommandGroup.SetDeviceNameCommand.ToolTipDescription;
            _nameEditor = new TextCellInPlaceEditor(_deviceName, FileSystemConstants.MaxShortNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };
            _nameEditor.EditorClosed += DeviceNameEditorClosed;
            _deviceOwner.TooltipText = DeviceCommandGroup.SetDeviceOwnerCommand.ToolTipDescription;
            _ownerEditor = new TextCellInPlaceEditor(_deviceOwner, FileSystemConstants.MaxLongNameLength) { IsValidCharacter = INTV.Core.Model.Grom.Characters.Contains };
            _ownerEditor.EditorClosed += DeviceOwnerEditorClosed;
            _deviceUniqueId.TooltipText = DeviceCommandGroup.DeviceUniqueIdCommand.ToolTipDescription;
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
        /// Update the data on the page.
        /// </summary>
        internal void Update()
        {
            if (!_committing)
            {
                try
                {
                    _updating = true;
                    _deviceName.Text = ViewModel.ActiveLtoFlashDevice.Name.SafeString();
                    _deviceOwner.Text = ViewModel.ActiveLtoFlashDevice.Owner.SafeString();
                    _deviceUniqueId.Text = ViewModel.ActiveLtoFlashDevice.UniqueId.SafeString();
                }
                finally
                {
                    _updating = false;
                }
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
            if (!_updating)
            {
                if (e.CommitedChanges)
                {
                    try
                    {
                        _committing = true;
                        ViewModel.ActiveLtoFlashDevice.Name = _deviceName.Text;
                        DeviceCommandGroup.SetDeviceNameCommand.Execute(ViewModel);
                    }
                    finally
                    {
                        _committing = false;
                    }
                }
                else
                {
                    _deviceName.Text = ViewModel.ActiveLtoFlashDevice.Name;
                }
            }
        }

        private void DeviceOwnerEditorClosed(object sender, INTV.Shared.Behavior.InPlaceEditorClosedEventArgs e)
        {
            if (!_updating)
            {
                if (e.CommitedChanges)
                {
                    try
                    {
                        _committing = true;
                        ViewModel.ActiveLtoFlashDevice.Owner = _deviceOwner.Text;
                        DeviceCommandGroup.SetDeviceOwnerCommand.Execute(ViewModel);
                    }
                    finally
                    {
                        _committing = false;
                    }
                }
                else
                {
                    _deviceOwner.Text = ViewModel.ActiveLtoFlashDevice.Owner;
                }
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
