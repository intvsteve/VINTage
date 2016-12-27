// <copyright file="LtoFlashVIewModel.Simulator.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

using System.Linq;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Device;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Implementation support for working with the Locutus software simulator.
    /// </summary>
    public partial class LtoFlashViewModel
    {
#if DEBUG
#if WIN
        private const string SimulatorConnectionName = "Locutus";
        private const byte BeaconCommand = 0xFD;
#endif // WIN

        #region ToggleConsolePowerCommand

        private static RelayCommand ToggleConsolePowerCommand { get; set; }

        private static void OnTogglePower(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            var hardwareFlags = deviceViewModel.Device.HardwareStatus ^ HardwareStatusFlags.ConsolePowerOn;
            SetHardwareStatus(deviceViewModel.Device, hardwareFlags);
        }

        private static bool CanTogglePower(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            return deviceViewModel.IsValid;
        }

        /// <summary>
        /// Helper method to execute emulate hardware status bits changing when attached to the Locutus simulator.
        /// </summary>
        /// <param name="device">The device to send the command to.</param>
        /// <param name="flags">The hardware status flags to assign to the simulator.</param>
        internal static void SetHardwareStatus(Device device, HardwareStatusFlags flags)
        {
            if (!device.IsCommandInProgress)
            {
                var executeCommandTaskData = new ExecuteDeviceCommandAsyncTaskData(device, INTV.LtoFlash.Model.Commands.ProtocolCommandId.DebugSetHardwareStatus)
                {
                    Data = flags,
                    OnFailure = (m, e) =>
                    {
                        INTV.Shared.View.OSMessageBox.Show(m, "Toggle Power Command", SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? e : null, null);
                        return true;
                    }
                };
                executeCommandTaskData.StartTask(SetHardwareStatus);
            }
        }

        /// <summary>
        /// Attempts to set the hardware status bits. Only useful if communicating with the Locutus simulator. Generates error on real hardware.
        /// </summary>
        /// <param name="taskData">The command execution data.</param>
        internal static void SetHardwareStatus(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
            var device = data.Device;
            data.Succeeded = INTV.LtoFlash.Model.Commands.DebugSetHardwareStatus.Create((HardwareStatusFlags)data.Data).Execute<bool>(device.Port, data);
        }

        #endregion // ToggleConsolePowerCommand

#if WIN
        private static readonly string SimulatorAppName = "locutus_sim";

        #region StartLocutusSimulatorCommand

        private static RelayCommand StartLocutusSimulatorCommand { get; set; }

        private static void OnStartSimulator(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            if (!ltoFlashViewModel.Devices.Any(d => d.IsValid && (d.Device.Port != null) && (d.Device.Port.Name == SimulatorConnectionName)))
            {
                var creationData = new System.Collections.Generic.Dictionary<string, object>()
                {
                    { NamedPipeConnection.PipeNameConfigDataName, SimulatorConnectionName },
                    { NamedPipeConnection.PreOpenPortConfigDataName, new System.Func<bool>(() => OnSimulatorStarted(ltoFlashViewModel)) },
                    { NamedPipeConnection.PostOpenPortConfigDataName, new System.Action<INTV.Shared.Model.IStreamConnection>(OnSimulatorConnectionComplete) },
                };
                INTV.Shared.Interop.DeviceManagement.DeviceChange.ReportDeviceAdded(null, SimulatorConnectionName, Core.Model.Device.ConnectionType.NamedPipe, creationData);
            }
        }

        private static bool OnSimulatorStarted(LtoFlashViewModel ltoFlashViewModel)
        {
            var launched = false;
            var simulatorProcess = System.Diagnostics.Process.GetProcesses().FirstOrDefault(p => p.ProcessName == SimulatorAppName);
            if (simulatorProcess == null)
            {
                var simAppPath = System.AppDomain.CurrentDomain.BaseDirectory + SimulatorAppName;
                if (System.IO.File.Exists(simAppPath + PathUtils.ProgramSuffix))
                {
                    ltoFlashViewModel.Simulator = INTV.Shared.Utility.RunExternalProgram.Launch(simAppPath, CreatePipeName(SimulatorConnectionName, "Input") + " " + CreatePipeName(SimulatorConnectionName, "Output"), string.Empty, true, false);
                    launched = ltoFlashViewModel.Simulator != null;
                    ltoFlashViewModel.Simulator.EnableRaisingEvents = true;
                    ltoFlashViewModel.Simulator.Exited += (s, e) => HandleSimulatorExited(ltoFlashViewModel);
                    SingleInstanceApplication.Current.Exit += (s, e) => HandleApplicationExitForSimulator(ltoFlashViewModel, true);
                }
            }
            return launched;
        }

        private static void OnSimulatorConnectionComplete(INTV.Shared.Model.IStreamConnection port)
        {
            if (port.IsOpen)
            {
                port.WriteStream.WriteByte(BeaconCommand);
            }
        }

        private static void HandleSimulatorExited(LtoFlashViewModel ltoFlashViewModel)
        {
            HandleApplicationExitForSimulator(ltoFlashViewModel, false);
        }

        private static void HandleApplicationExitForSimulator(LtoFlashViewModel ltoFlashViewModel, bool appExit)
        {
            var devicesToDisconnect = ltoFlashViewModel.Devices.Where(d => d.Connections.Any(c => c.Type == Core.Model.Device.ConnectionType.NamedPipe));
            foreach (var device in devicesToDisconnect)
            {
                device.Device.Disconnect(appExit);
            }
            var simulator = ltoFlashViewModel.Simulator;
            ltoFlashViewModel.Simulator = null;
            if (simulator != null)
            {
                if (!simulator.HasExited)
                {
                    simulator.Kill();
                }
                simulator.Dispose();
            }
        }

        private static string CreatePipeName(string baseName, string suffix)
        {
            string pipeName = "\\\\.\\pipe\\" + baseName + suffix;
            return pipeName;
        }

        #endregion // StartLocutusSimulatorCommand

        #region GetFileSystemTablesCommand

        private VisualRelayCommand GetFileSystemTablesCommand { get; set; }

        private static void OnGetFileSystemTables(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            deviceViewModel.Device.RetrieveFileSystem("Getting File System...", null, GetFileSystemTablesErrorHandler);
        }

        private static bool CanGetFileSystemTables(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            return deviceViewModel.IsValid;
        }

        private static bool GetFileSystemTablesErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.RetrieveFileSystemCommand_Failed_Message_Format, errorMessage);
            INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.RetrieveFileSystemCommand_Failed_Title);
            return true;
        }

        #endregion // GetFileSystemTablesCommand

        #region ToggleDirtyFlagCommand

        private static RelayCommand ToggleDirtyFlagCommand { get; set; }

        private static bool SetFileSystemFlagsErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.SetFileSystemFlagsCommand_Failed_Message_Format, errorMessage);
            INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.SetFileSystemFlagsCommand_Failed_Title);
            return true;
        }

        private static void OnToggleDirtyFlag(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            var statusFlags = deviceViewModel.Device.FileSystemFlags ^ LfsDirtyFlags.FileSystemUpdateInProgress;
            deviceViewModel.Device.SetFileSystemFlags(statusFlags, SetFileSystemFlagsErrorHandler);
        }

        private static bool CanToggleDirtyFlag(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            return deviceViewModel.IsValid;
        }

        #endregion // ToggleDirtyFlagCommand

        #region GetDirtyFlagsCommand

        private static RelayCommand GetDirtyFlagsCommand { get; set; }

        private static void OnGetDirtyFlags(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            deviceViewModel.Device.GetFileSystemFlags(GetFileSystemFlagsErrorHandler);
        }

        private static bool CanGetDirtyFlags(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            return deviceViewModel.IsValid;
        }

        private static bool GetFileSystemFlagsErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.GetFileSystemFlagsCommand_Failed_Message_Format, errorMessage);
            INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.GetFileSystemFlagsCommand_Failed_Title);
            return true;
        }

        #endregion // GetDirtyFlagsCommand

        #region GetFileSystemStatisticsCommand

        private static RelayCommand GetFileSystemStatisticsCommand { get; set; }

        private static void OnGetFileSystemStatistics(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            deviceViewModel.Device.GetFileSystemStatistics(INTV.LtoFlash.Commands.DownloadCommandGroup.GetFileSystemStatisticsErrorHandler);
        }

        private static bool CanGetFileSystemStatistics(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            return deviceViewModel.IsValid;
        }

        #endregion // GetFileSystemStatisticsCommand

        #region GetErrorLogCommand

        private static RelayCommand GetErrorLogCommand { get; set; }

        private static void OnGetErrorLog(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            deviceViewModel.Device.GetErrorLog(GetErrorLogComplete, GetErrorLogErrorHandler);
        }

        private static bool CanGetErrorLog(object parameter)
        {
            var deviceViewModel = parameter as DeviceViewModel;
            return deviceViewModel.IsValid;
        }

        private static void GetErrorLogComplete(INTV.LtoFlash.Model.Commands.ErrorLog errorLog)
        {
            var message = "Retrieved error log:\n\n";
            if (errorLog == null)
            {
                message += "<null>";
            }
            else
            {
                message += errorLog.GetDetailedErrorReport(INTV.LtoFlash.Model.FirmwareRevisions.UnavailableFirmwareVersion);
            }
            INTV.Shared.View.OSMessageBox.Show(message, "Error Log Retrieved");
        }

        private static bool GetErrorLogErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.GetErrorLogCommand_Failed_Message_Format, errorMessage);
            INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.GetErrorLogCommand_Failed_Title);
            return true;
        }

        #endregion // GetErrorLogCommand

        #region GetErrorAndCrashLogsCommand

        private static readonly RelayCommand GetErrorAndCrashLogsCommand = new RelayCommand(OnGetErrorAndCrashLogs)
        {
            UniqueId = "INTV.LtoFlash.ViewModel.LtoFlashViewModel.GetErrorAndCrashLogsCommand"
        };

        private static void OnGetErrorAndCrashLogs(object parameter)
        {
            var ltoFlash = CompositionHelpers.Container.GetExportedValueOrDefault<LtoFlashViewModel>();
            if (ltoFlash != null && ltoFlash.ActiveLtoFlashDevice.IsValid)
            {
                ltoFlash.ActiveLtoFlashDevice.Device.GetErrorAndCrashLogs(GetErrorAndCrashLogsComplete, null);
            }
        }

        private static void GetErrorAndCrashLogsComplete(INTV.LtoFlash.Model.Commands.ErrorLog errorLog, CrashLog crashLog)
        {
            System.Diagnostics.Debug.WriteLine("Got logs");
        }

        #endregion // GetErrorAndCrashLogsCommand

        private System.Diagnostics.Process Simulator { get; set; }

#endif // WIN
#endif // DEBUG
    }
}
