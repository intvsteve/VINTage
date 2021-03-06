﻿// <copyright file="DeviceSettingsPage.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2019 All Rights Reserved
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Device settings page visual for GTK.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    public partial class DeviceSettingsPage : Gtk.Bin, IFakeDependencyObject
    {
        private Dictionary<RelayCommand, bool> _blockWhenBusy = new Dictionary<RelayCommand, bool>();
        private Dictionary<Gtk.Widget, VisualRelayCommand> _controlCommandMap = new Dictionary<Gtk.Widget, VisualRelayCommand>();

        private bool _updating;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.DeviceSettingsPage"/> class.
        /// </summary>
        public DeviceSettingsPage()
        {
            this.Build();

            InitializeShowTitleScreenComboBox();
            InitializeIntellivisionIICompatibilityComboBox();
            InitializeEcsCompatibilityComboBox();
            InitializeSaveMenuPositionComboBox();
            _keyClicks.Active = (bool)ConfigurableLtoFlashFeatures.Default[Device.KeyclicksPropertyName].FactoryDefaultValue;
            _backgroundGC.Active = (bool)ConfigurableLtoFlashFeatures.Default[Device.BackgroundGCPropertyName].FactoryDefaultValue;
            _enableCartConfigMenu.Active = (bool)ConfigurableLtoFlashFeatures.Default[Device.EnableConfigMenuOnCartPropertyName].FactoryDefaultValue;
            _randomizeLtoFlashRam.Active = !(bool)ConfigurableLtoFlashFeatures.Default[Device.ZeroLtoFlashRamPropertyName].FactoryDefaultValue;
            InitializeCommandVisualsToCommandsMap();
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
                _titleScreenSetting.Active = _titleScreenSetting.GetIndexOfValue(ViewModel.ActiveLtoFlashDevice.ShowTitleScreen);
                _intellivisionIICompatibility.Active = _intellivisionIICompatibility.GetIndexOfValue(ViewModel.ActiveLtoFlashDevice.IntvIICompatibility);
                _ecsCompatibility.Active = _ecsCompatibility.GetIndexOfValue(ViewModel.ActiveLtoFlashDevice.EcsCompatibility);
                _saveMenuPositionSetting.Active = _saveMenuPositionSetting.GetIndexOfValue(ViewModel.ActiveLtoFlashDevice.SaveMenuPosition);
                _keyClicks.Active = ViewModel.ActiveLtoFlashDevice.Keyclicks;
                _backgroundGC.Active = ViewModel.ActiveLtoFlashDevice.BackgroundGC;
                _enableCartConfigMenu.Active = ViewModel.ActiveLtoFlashDevice.EnableConfigMenuOnCart;
                _randomizeLtoFlashRam.Active = ViewModel.ActiveLtoFlashDevice.RandomizeLtoFlashRam;
            }
            finally
            {
                _updating = false;
            }
        }

        /// <inheritdoc/>
        protected override void OnDestroyed()
        {
            CommandManager.RequerySuggested -= HandleRequerySuggested;
            foreach (var command in _controlCommandMap.Values.Reverse())
            {
                command.PropertyChanged -= HandleVisualRelayCommandPropertyChanged;
            }
            foreach (var blockWhenBusy in _blockWhenBusy)
            {
                blockWhenBusy.Key.BlockWhenAppIsBusy = blockWhenBusy.Value;
            }
            base.OnDestroyed();
        }

        /// <summary>
        /// Handles the show title screen setting changed.
        /// </summary>
        /// <param name="sender">The title screen setting combo box.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleShowTitleScreenChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _titleScreenSetting), "Got value change from wrong control! Expected ShowTitleScreen.");
            ShowTitleScreenFlags newFlags;
            if ((ViewModel != null) && !_updating && _titleScreenSetting.GetActiveValue(out newFlags))
            {
                ViewModel.ActiveLtoFlashDevice.ShowTitleScreen = newFlags;
            }
        }

        /// <summary>
        /// Handles the Intellivision II compatibility setting changed.
        /// </summary>
        /// <param name="sender">The Intellivision II compatibility setting combo box.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleIntellivisionIICompatibilityChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _intellivisionIICompatibility), "Got value change from wrong control! Expected IntellivisionII.");
            IntellivisionIIStatusFlags newFlags;
            if ((ViewModel != null) && !_updating && _intellivisionIICompatibility.GetActiveValue(out newFlags))
            {
                ViewModel.ActiveLtoFlashDevice.IntvIICompatibility = newFlags;
            }
        }

        /// <summary>
        /// Handles the ECS compatibility setting changed.
        /// </summary>
        /// <param name="sender">The ECS compatibility setting combo box.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleEcsCompatibilityChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _ecsCompatibility), "Got value change from wrong control! Expected ECS.");
            EcsStatusFlags newFlags;
            if ((ViewModel != null) && !_updating && _ecsCompatibility.GetActiveValue(out newFlags))
            {
                ViewModel.ActiveLtoFlashDevice.EcsCompatibility = newFlags;
            }
        }

        /// <summary>
        /// Handles the save menu position setting changed.
        /// </summary>
        /// <param name="sender">The save menu position setting combo box.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleSaveMenuPositionChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _saveMenuPositionSetting), "Got value change from wrong control! Expected SaveMenuPosition.");
            SaveMenuPositionFlags newFlags;
            if ((ViewModel != null) && !_updating && _saveMenuPositionSetting.GetActiveValue(out newFlags))
            {
                ViewModel.ActiveLtoFlashDevice.SaveMenuPosition = newFlags;
            }
        }

        /// <summary>
        /// Handles the keyclicks setting changed.
        /// </summary>
        /// <param name="sender">The keyclicks checkbox.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleKeyclicksChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _keyClicks), "Got value change from wrong control! Expected Keyclicks.");
            if ((ViewModel != null) && !_updating)
            {
                ViewModel.ActiveLtoFlashDevice.Keyclicks = _keyClicks.Active;
            }
        }

        /// <summary>
        /// Handles the randomize LTO Flash! RAM changed event.
        /// </summary>
        /// <param name="sender">The Randomize RAM checkbox.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleRandomizeLtoFlashRamChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _randomizeLtoFlashRam), "Got value change from wrong control! Expected Randomize LTO Flash! RAM.");
            if ((ViewModel != null) && !_updating)
            {
                ViewModel.ActiveLtoFlashDevice.RandomizeLtoFlashRam = _randomizeLtoFlashRam.Active;
            }
        }

        /// <summary>
        /// Handles the enable config menu changed event.
        /// </summary>
        /// <param name="sender">The Enable configuration menu checkbox.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleEnableConfigMenuChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _enableCartConfigMenu), "Got value change from wrong control! Expected Enable configuration menu.");
            if ((ViewModel != null) && !_updating)
            {
                ViewModel.ActiveLtoFlashDevice.EnableConfigMenuOnCart = _enableCartConfigMenu.Active;
            }
        }

        /// <summary>
        /// Handles the do background GC setting changed.
        /// </summary>
        /// <param name="sender">The do background GC checkbox.</param>
        /// <param name="e">Not applicable.</param>
        protected void HandleBackgroundGCChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Assert(object.ReferenceEquals(sender, _backgroundGC), "Got value change from wrong control! Expected FileSystemMaintenance.");
            if ((ViewModel != null) && !_updating)
            {
                ViewModel.ActiveLtoFlashDevice.BackgroundGC = _backgroundGC.Active;
            }
        }

        private void InitializeCommandVisualsToCommandsMap()
        {
            _blockWhenBusy[DeviceCommandGroup.SetDeviceNameCommand] = DeviceCommandGroup.SetDeviceNameCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetDeviceOwnerCommand] = DeviceCommandGroup.SetDeviceOwnerCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetShowTitleScreenCommand] = DeviceCommandGroup.SetShowTitleScreenCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetEcsCompatibilityCommand] = DeviceCommandGroup.SetEcsCompatibilityCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetIntellivisionIICompatibilityCommand] = DeviceCommandGroup.SetIntellivisionIICompatibilityCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetKeyclicksCommand] = DeviceCommandGroup.SetKeyclicksCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetEnableConfigMenuOnCartCommand] = DeviceCommandGroup.SetEnableConfigMenuOnCartCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetRandomizeLtoFlashRamCommand] = DeviceCommandGroup.SetRandomizeLtoFlashRamCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetSaveMenuPositionCommand] = DeviceCommandGroup.SetSaveMenuPositionCommand.BlockWhenAppIsBusy;
            _blockWhenBusy[DeviceCommandGroup.SetBackgroundGarbageCollectCommand] = DeviceCommandGroup.SetBackgroundGarbageCollectCommand.BlockWhenAppIsBusy;

            foreach (var command in _blockWhenBusy.Keys)
            {
                command.BlockWhenAppIsBusy = false;
            }

            _controlCommandMap[_ecsCompatibility] = DeviceCommandGroup.SetEcsCompatibilityCommand;
            _controlCommandMap[_intellivisionIICompatibility] = DeviceCommandGroup.SetIntellivisionIICompatibilityCommand;
            _controlCommandMap[_titleScreenSetting] = DeviceCommandGroup.SetShowTitleScreenCommand;
            _controlCommandMap[_saveMenuPositionSetting] = DeviceCommandGroup.SetSaveMenuPositionCommand;
            _controlCommandMap[_keyClicks] = DeviceCommandGroup.SetKeyclicksCommand;
            _controlCommandMap[_backgroundGC] = DeviceCommandGroup.SetBackgroundGarbageCollectCommand;
            _controlCommandMap[_enableCartConfigMenu] = DeviceCommandGroup.SetEnableConfigMenuOnCartCommand;
            _controlCommandMap[_randomizeLtoFlashRam] = DeviceCommandGroup.SetRandomizeLtoFlashRamCommand;

            _ecsCompatibilityLabel.TooltipText = Resources.Strings.SetEcsCompatibilityCommand_TipDescription;
            _intellivisionIICompatibilityLabel.TooltipText = Resources.Strings.SetIntellivisionIICompatibilityCommand_TipDescription;
            _titleScreenSettingLabel.TooltipText = Resources.Strings.SetShowTitleScreenCommand_TipDescription;
            _saveMenuPositionSettingLabel.TooltipText = Resources.Strings.SetSaveMenuPositionCommand_TipDescription;

            foreach (var controlCommandMapEntry in _controlCommandMap)
            {
                var commandVisual = controlCommandMapEntry.Key;
                var controlCommand = controlCommandMapEntry.Value;
                commandVisual.TooltipText = controlCommand.ToolTipDescription;
                controlCommand.PropertyChanged += HandleVisualRelayCommandPropertyChanged;
            }
        }

        private void HandleVisualRelayCommandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var command = sender as VisualRelayCommand;
            if ((sender != null) && (e.PropertyName == "ToolTipDescription"))
            {
                foreach (var entry in _controlCommandMap.Where(c => c.Value.UniqueId == command.UniqueId))
                {
                    entry.Key.TooltipText = command.ToolTipDescription;
                }
            }
        }

        private void HandleRequerySuggested(object sender, System.EventArgs args)
        {
            foreach (var controlCommandMapEntry in _controlCommandMap)
            {
                var commandVisual = controlCommandMapEntry.Key;
                var command = controlCommandMapEntry.Value;
                commandVisual.Sensitive = command.CanExecute(ViewModel);
            }
        }

        private void InitializeShowTitleScreenComboBox()
        {
            var options = new[] { ShowTitleScreenFlags.Always, ShowTitleScreenFlags.OnPowerUp, ShowTitleScreenFlags.Never };
            InitializeComboBox(_titleScreenSetting, options, ShowTitleScreenFlags.Default, ShowTitleScreenFlagsHelpers.ToDisplayString);
        }

        private void InitializeIntellivisionIICompatibilityComboBox()
        {
            var options = new[] { IntellivisionIIStatusFlags.None, IntellivisionIIStatusFlags.Conservative, IntellivisionIIStatusFlags.Aggressive };
            InitializeComboBox(_intellivisionIICompatibility, options, IntellivisionIIStatusFlags.Default, IntellivisionIIStatusFlagsHelpers.ToDisplayString);
        }

        private void InitializeEcsCompatibilityComboBox()
        {
            var options = new[] { EcsStatusFlags.None, EcsStatusFlags.EnabledForRequiredAndOptional, EcsStatusFlags.EnabledForRequired, EcsStatusFlags.Disabled };
            InitializeComboBox(_ecsCompatibility, options, EcsStatusFlags.Default, EcsStatusFlagsHelpers.ToDisplayString);
        }

        private void InitializeSaveMenuPositionComboBox()
        {
            var options = new[] { SaveMenuPositionFlags.Always, SaveMenuPositionFlags.DuringSessionOnly, SaveMenuPositionFlags.Never };
            InitializeComboBox(_saveMenuPositionSetting, options, SaveMenuPositionFlags.Default, SaveMenuPositionFlagsHelpers.ToDisplayString);
        }

        private void InitializeComboBox<T>(Gtk.ComboBox comboBox, IEnumerable<T> values, T activeValue, Func<T, string> toDisplayString)
        {
            var cellRenderer = new Gtk.CellRendererCombo();
            comboBox.PackStart(cellRenderer, true);
            comboBox.SetCellDataFunc(cellRenderer, (l, e, m, i) => VisualHelpers.CellEnumRenderer<T>(l, e, m, i, toDisplayString));

            var model = new Gtk.ListStore(typeof(T));
            model.SynchronizeCollection(new ObservableCollection<T>(values));
            comboBox.Model = model;

            comboBox.Active = comboBox.GetIndexOfValue(activeValue);
        }
    }
}
