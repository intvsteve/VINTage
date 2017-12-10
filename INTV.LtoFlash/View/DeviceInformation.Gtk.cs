// <copyright file="DeviceInformation.Gtk.cs" company="INTV Funhouse">
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
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// GTK implementation of the Device Information dialog.
    /// </summary>
    public partial class DeviceInformation : Gtk.Dialog, IFakeDependencyObject
    {
        private DeviceViewModel _device;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.View.DeviceInformation"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to use.</param>
        private DeviceInformation(LtoFlashViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.PropertyChanged += HandleViewModelPropertyChanged;
            this.Build();
            _informationPages.Page = (int)SelectedPageIndex;
            UpdateDisplay();
        }

        private static uint SelectedPageIndex { get; set; }

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        #endregion // IFakeDependencyObject Properties

        private LtoFlashViewModel ViewModel
        {
            get { return DataContext as LtoFlashViewModel; }
        }

        /// <summary>
        /// Creates a new instance of DeviceInformation.
        /// </summary>
        /// <param name="viewModel">The ViewModel to monitor for device information.</param>
        /// <returns>A new instance of the dialog.</returns>
        public static DeviceInformation Create(LtoFlashViewModel viewModel)
        {
            var dialog = new DeviceInformation(viewModel);
            return dialog;
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

        /// <summary>
        /// Show the dialog.
        /// </summary>
        /// <remarks>TODO: Delete this and use the extension methods!</remarks>
        public void ShowWindow()
        {
            this.ShowDialog();
        }

        public override void Dispose()
        {
            if (_device != null)
            {
                _device.PropertyChanged -= HandleDevicePropertyChanged;
            }
            ViewModel.PropertyChanged -= HandleViewModelPropertyChanged;
            base.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnResponse(Gtk.ResponseType response_id)
        {
            base.OnResponse(response_id);
            VisualHelpers.Close(this);
            Dispose();
        }

        protected void HandleSwitchPage(object o, Gtk.SwitchPageArgs args)
        {
            SelectedPageIndex = args.PageNum;
        }

        private void HandleViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ////this.HandleEventOnMainThread(sender, e, HandleViewModelPropertyChangedCore);
            HandleViewModelPropertyChangedCore(sender, e);
        }

        private void HandleViewModelPropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case LtoFlashViewModel.ActiveLtoFlashDevicePropertyName:
                    UpdateDisplay();
                    break;
                default:
                    break;
            }
        }

        private void HandleDevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ////this.HandleEventOnMainThread(sender, e, HandleDevicePropertyChangedCore);
            HandleDevicePropertyChangedCore(sender, e);
        }

        private void HandleDevicePropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "DisplayName":
                case Device.NamePropertyName:
                case Device.OwnerPropertyName:
                case Device.UniqueIdPropertyName:
                    UpdateInfo();
                    break;
                case Device.FirmwareRevisionsPropertyName:
                    UpdateFirmwareInfo();
                    break;
                case Device.FileSystemStatisticsPropertyName:
                    UpdateFileSystemInfo();
                    break;
                case Device.EcsCompatibilityPropertyName:
//                    ECSCompatibilityButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.EcsCompatibility);
                    break;
                case Device.IntvIICompatibilityPropertyName:
//                    IntellivisionIICompatibilityButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.IntvIICompatibility);
                    break;
                case Device.ShowTitleScreenPropertyName:
//                    ShowTitleScreenButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.ShowTitleScreen);
                    break;
                case Device.SaveMenuPositionPropertyName:
//                    SaveMenuPositionButton.SelectItemWithTag((byte)ViewModel.ActiveLtoFlashDevice.SaveMenuPosition);
                    break;
                case Device.BackgroundGCPropertyName:
//                    BackgroundGC = ViewModel.ActiveLtoFlashDevice.BackgroundGC;
                    break;
                case Device.KeyclicksPropertyName:
//                    Keyclicks = ViewModel.ActiveLtoFlashDevice.Keyclicks;
                    break;
                default:
                    break;
            }
        }

        private void UpdateDisplay()
        {
            if (_device != null)
            {
                _device.PropertyChanged -= HandleDevicePropertyChanged;
            }
            _device = ViewModel.ActiveLtoFlashDevice;
            if (_device != null)
            {
                _device.PropertyChanged += HandleDevicePropertyChanged;
            }

            UpdateInfo();
            UpdateFirmwareInfo();
            UpdateFileSystemInfo();
        }

        private void UpdateInfo()
        {
            var connectionName = Resources.Strings.NoDevice;
            if (ViewModel.ActiveLtoFlashDevice.IsValid)
            {
                var port = ViewModel.ActiveLtoFlashDevice.Device.Port;
                connectionName = port.ToString();
            }
            _deviceConnection.Text = connectionName;

            _infoPage.SetDeviceName(ViewModel.ActiveLtoFlashDevice.Name.SafeString());
            _infoPage.SetDeviceOwner(ViewModel.ActiveLtoFlashDevice.Owner.SafeString());
            _infoPage.SetUniqueId(ViewModel.ActiveLtoFlashDevice.UniqueId.SafeString());
        }

        private void UpdateFirmwareInfo()
        {
//            FactoryFirmwareVersion = new NSString(ViewModel.FirmwareRevisions.Primary.SafeString());
//            SecondaryFirmwareVersion = new NSString(ViewModel.FirmwareRevisions.Secondary.SafeString());
//            CurrentFirmwareVersion = new NSString(ViewModel.FirmwareRevisions.Current.SafeString());
        }

        private void UpdateFileSystemInfo()
        {
            /*
            PhysBlocksAvail = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksAvailable);
            PhysBlocksInUse = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksInUse);
            PhysCleanBlocks = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksClean);
            PhysTotalBlocks = new NSString(ViewModel.FileSystemStatistics.PhysicalBlocksTotal);
            VirtBlocksAvail = new NSString(ViewModel.FileSystemStatistics.VirtualBlocksAvailable);
            VirtBlocksInUse = new NSString(ViewModel.FileSystemStatistics.VirtualBlocksInUse);
            VirtTotalBlocks = new NSString(ViewModel.FileSystemStatistics.VirtualBlocksTotal);
            PhysSectorErasures = new NSString(ViewModel.FileSystemStatistics.PhysicalSectorErasures);
            MetadataSectorErasures = new NSString(ViewModel.FileSystemStatistics.MetadataSectorErasures);
            VirtToPhysMapVer = new NSString(ViewModel.FileSystemStatistics.VirtualToPhysicalMapVersion);
            PBlockErasures = new NSString(ViewModel.FileSystemStatistics.PercentFlashLifetimeUsedByPhysicalBlockErasures);
            VtoPLogWraps = new NSString(ViewModel.FileSystemStatistics.PercentageFlashLifetimeUsedByVirtualToPhysicalMap);
            LifeRemaining = new NSString(ViewModel.FileSystemStatistics.PercentageLifetimeRemaining);
            */
        }
    }
}
