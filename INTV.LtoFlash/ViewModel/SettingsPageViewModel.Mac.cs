// <copyright file="SettingsPageViewModel.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for the LTO Flash! settings page.
    /// </summary>
    public sealed partial class SettingsPageViewModel
    {
        /// <summary>
        /// The name of the serial port read chunk size property.
        /// </summary>
        public const string SerialPortReadChunkSizePropertyName = "SerialPortReadChunkSize";

        /// <summary>
        /// Gets the available serial port read block sizes.
        /// </summary>
        public ObservableCollection<SerialPortReadWriteBlockSizeViewModel> AvailableSerialPortReadBlockSizes { get; private set; }

        /// <summary>
        /// Gets or sets the selected read chunk size ViewModel.
        /// </summary>
        public SerialPortReadWriteBlockSizeViewModel SelectedReadChunkSizeViewModel
        {
            get { return _selectedReadChunkSizeViewModel; }
            set { AssignAndUpdateProperty("SelectedReadChunkSizeViewModel", value, ref _selectedReadChunkSizeViewModel, (p, v) => UpdateReadChunkSize(v.BlockSize)); }
        }
        private SerialPortReadWriteBlockSizeViewModel _selectedReadChunkSizeViewModel;

        /// <summary>
        /// Gets or sets the serial port read chunk size.
        /// </summary>
        [OSExport(SerialPortReadChunkSizePropertyName)]
        public int SerialPortReadChunkSize
        {
            get { return _serialPortReadChunkSize; }
            set { AssignAndUpdateProperty(SerialPortReadChunkSizePropertyName, value, ref _serialPortReadChunkSize); }
        }
        private int _serialPortReadChunkSize;

        private void UpdateReadChunkSize(int chunkSize)
        {
            SerialPortReadChunkSize = chunkSize;
            Properties.Settings.Default.LtoFlashSerialReadChunkSize = chunkSize;
        }

        private string GetDisplayNameForByteSize(int readBlockSize, int defaultChunkSize)
        {
            var suffix = readBlockSize == defaultChunkSize ? " *" : string.Empty;
            var displayName = string.Empty;
            if (readBlockSize == 0)
            {
                displayName = "As requested";
            }
            else
            {
                displayName = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} bytes", readBlockSize);
            }
            return displayName + suffix;
        }

        /// <summary>
        /// Initialize Mac-specific settings.
        /// </summary>
        partial void OSInitialize()
        {
            var readBlockSizes = new[] { 0, 768, 512, 256, 128, 64 };
            AvailableSerialPortReadBlockSizes = new ObservableCollection<SerialPortReadWriteBlockSizeViewModel>(readBlockSizes.Select(s => new SerialPortReadWriteBlockSizeViewModel(s, GetDisplayNameForByteSize(s, Properties.Settings.DefaultReadChunkSize))));
            _selectedReadChunkSizeViewModel = AvailableSerialPortReadBlockSizes.FirstOrDefault(s => s.BlockSize == Properties.Settings.Default.LtoFlashSerialReadChunkSize);
            if (_selectedReadChunkSizeViewModel == null)
            {
                _selectedReadChunkSizeViewModel = AvailableSerialPortReadBlockSizes.First(s => s.BlockSize == Properties.Settings.DefaultReadChunkSize);
            }
            _serialPortReadChunkSize = _selectedReadChunkSizeViewModel.BlockSize;
        }
    }
}
