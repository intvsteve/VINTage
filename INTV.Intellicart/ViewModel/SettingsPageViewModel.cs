// <copyright file="SettingsPageViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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

using INTV.Intellicart.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.ViewModel;

#if WIN
using SettingsPageVisualType = INTV.Intellicart.View.IntellicartSettingsPage;
#elif MAC
using SettingsPageVisualType = INTV.Intellicart.View.IntellicartSettingsPageController;
#endif // WIN

namespace INTV.Intellicart.ViewModel
{
    /// <summary>
    /// ViewModel for Intellicart settings page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ISettingsPage))]
    [LocalizedName(typeof(Resources.Strings), "SettingsPage_Title")]
    [Weight(0.2)]
    [Icon("Resources/Images/intellicart_32xMD.png")]
    public sealed partial class SettingsPageViewModel : INTV.Shared.ViewModel.SettingsPageViewModel<SettingsPageVisualType>
    {
        #region Property Names

        public const string IntellicartSerialPortPropertyName = "IntellicartSerialPort";
        public const string IntellicartBaudRatePropertyName = "IntellicartBaudRate";
        public const string IntellicartWriteTimeoutPropertyName = "IntellicartWriteTimeout";

        #endregion // Property Names

        #region UI Strings

        public static readonly string SerialPortGroupName = Resources.Strings.SettingsPage_SerialPortGroup_Name;
        public static readonly string PortNameLabel = Resources.Strings.SettingsPage_PortName_Label;
        public static readonly string BaudRateLabel = Resources.Strings.SettingsPage_BaudRate_Label;
        public static readonly string SelectPortButtonLabel = Resources.Strings.SettingsPage_SelectPortButton_Label;
        public static readonly string WriteTimeoutLabel = Resources.Strings.SettingsPage_SerialWriteTimeout_Label;
        public static readonly string ResetWriteTimeout = Resources.Strings.SettingsPage_ResetTimeoutToDefault_Label;
        public static readonly string WriteTimeoutInfo = Resources.Strings.SettingsPage_PortWriteTimeoutInfo;

        #endregion // UI Strings

        #region ResetToDefaultWriteTimeoutCommand

        /// <summary>
        /// Lazy creation of the Reset to defult write timeout command.
        /// </summary>
        /// <remarks>When making <see cref="INTV.Core.ComponentModel.RelayCommandBase"/> be a subclass of NSObject,
        /// creation of this command during MEF instantiation of this type would cause problems in newer versions
        /// of Xamarin.Mac (5.8.0). By using <see cref="System.Lazy{T}"/> we defer long enough to allow RelayCommandBase
        /// and NSObject types to be prepared for use. (?) Have not examined this in close detail. The issue was a
        /// <see cref="System.ArgumentNullException"/> in NSObject's constructor when RelayCommandBase was invoking
        /// the base class constructor.</remarks>
        private static readonly System.Lazy<RelayCommand> LazyResetToDefultWriteTimeoutCommand = new System.Lazy<RelayCommand>(() =>
        {
            return new RelayCommand(OnResetToDefaultWriteTimeout)
                {
                    UniqueId = "INTV.Intellicart.ViewModel.ResetToDefaultWriteTimeoutCommand",
                    BlockWhenAppIsBusy = false
                };
        });

        /// <summary>
        /// Gets the command to reset the serial port write timeout to its default value.
        /// </summary>
        public static RelayCommand ResetToDefaultWriteTimeoutCommand
        {
            get { return LazyResetToDefultWriteTimeoutCommand.Value; }
        }

        private static void OnResetToDefaultWriteTimeout(object parameter)
        {
            var viewModel = parameter as SettingsPageViewModel;
            viewModel.Timeout = IntellicartModel.DefaultWriteTimeout;
        }

        #endregion // ResetToDefaultWriteTimeoutCommand

        private SettingsPageViewModel()
        {
            Intellicart = new IntellicartViewModel();
            Intellicart.PropertyChanged += IntellicartPropertyChanged;
        }

        /// <summary>
        /// Gets the ViewModel for an Intellicart.
        /// </summary>
        public IntellicartViewModel Intellicart { get; private set; }

        /// <summary>
        /// Gets the serial port used to communicate with an Intellicart.
        /// </summary>
        public string SerialPort
        {
            get { return Intellicart.SerialPort; }
        }

        /// <summary>
        /// Gets the baud rate used to communicate with an Intellicart.
        /// </summary>
        public int BaudRate
        {
            get { return Intellicart.BaudRate; }
        }

        /// <summary>
        /// Gets or sets the serial port write timeout (in seconds) when loading a ROM onto the Intellicart.
        /// </summary>
        public int Timeout
        {
            get { return Intellicart.Timeout; }
            set { Intellicart.Timeout = value; }
        }

        #region SettingsPageViewModel

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
        }

        #endregion // SettingsPageViewModel

        private void IntellicartPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private int UpdateValue(int newValue)
        {
            var updatedValue = System.Math.Max(newValue, IntellicartModel.MinWriteTimeout);
            updatedValue = System.Math.Min(updatedValue, IntellicartModel.MaxWriteTimeout);
            return updatedValue;
        }
    }
}
