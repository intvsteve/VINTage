// <copyright file="CheckForDevicesTaskData.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Device;
using INTV.LtoFlash.Model;
using INTV.Shared.Model.Device;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Data for asynchronous device detection.
    /// </summary>
    internal class CheckForDevicesTaskData : INTV.Shared.Utility.AsyncTaskData
    {
        /// <summary>
        /// Number of times to retry connecting in light of certain errors.
        /// </summary>
        public const int RetryCount = 4;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CheckForDevicesTaskData class.
        /// </summary>
        /// <param name="lastKnownPort">Last known device port.</param>
        /// <param name="ltoFlashViewModel">ViewModel for propagating data.</param>
        public CheckForDevicesTaskData(string lastKnownPort, LtoFlashViewModel ltoFlashViewModel)
            : this(lastKnownPort, ltoFlashViewModel, Enumerable.Empty<string>(), true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CheckForDevicesTaskData class.
        /// </summary>
        /// <param name="lastKnownPort">Last known device port.</param>
        /// <param name="ltoFlashViewModel">ViewModel for propagating data.</param>
        /// <param name="currentDevices">A list of current devices, used to exclude from port searching.</param>
        /// <param name="autoConnect">If <c>true</c> automatically connect to a discovered device.</param>
        public CheckForDevicesTaskData(string lastKnownPort, LtoFlashViewModel ltoFlashViewModel, IEnumerable<string> currentDevices, bool autoConnect)
            : base(null)
        {
            LastKnownPort = lastKnownPort;
            CurrentDevices = currentDevices;
            LtoFlashViewModel = ltoFlashViewModel;
            AutoConnect = autoConnect;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to show a dialog indicating that no devices were found.
        /// </summary>
        public bool ReportNoneFound { get; set; }

        /// <summary>
        /// Gets or sets the valid device ports discovered during the probe.
        /// </summary>
        private IEnumerable<string> ValidDevicePorts { get; set; }

        /// <summary>
        /// Gets or sets the ViewModel to communicate with, if necessary.
        /// </summary>
        private LtoFlashViewModel LtoFlashViewModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether any devices were reported for auto-connecting.
        /// </summary>
        private bool ReportedAnyAutoConnectDevices { get; set; }

        private string LastKnownPort { get; set; }

        private IEnumerable<string> CurrentDevices { get; set; }

        private bool AutoConnect { get; set; }

        #endregion // Properties

        /// <summary>
        /// Starts the search for devices.
        /// </summary>
        public void Start()
        {
            var task = new AsyncTaskWithProgress(Resources.Strings.DeviceSearch_Task_Title, true, true, 1.0);
            task.RunTask(this, CheckDevices, OnComplete);
        }

        private static void CheckDevices(AsyncTaskData taskData)
        {
            var data = (CheckForDevicesTaskData)taskData;
            var validPorts = new HashSet<string>();
            data.ValidDevicePorts = validPorts;
            var potentialPorts = data.LtoFlashViewModel.AvailableDevicePorts.Except(data.CurrentDevices).ToList();
            if (!string.IsNullOrWhiteSpace(data.LastKnownPort) && (potentialPorts.Count > 1) && potentialPorts.Remove(data.LastKnownPort))
            {
                potentialPorts.Insert(0, data.LastKnownPort);
            }

            foreach (var portName in potentialPorts)
            {
                var maxRetries = 1;
                for (var retry = 0; retry < maxRetries; ++retry)
                {
                    if (data.AcceptCancelIfRequested())
                    {
                        break;
                    }
                    var errorLogger = Properties.Settings.Default.EnablePortLogging ? new Logger(Configuration.Instance.GetPortLogPath(portName)) : null;
                    try
                    {
                        var isValidDevicePort = false;
                        var configData = new Dictionary<string, object>() { { SerialPortConnection.PortNameConfigDataName, portName } };
                        using (var port = SerialPortConnection.Create(configData))
                        {
                            if (Properties.Settings.Default.EnablePortLogging)
                            {
                                port.EnableLogging(Configuration.Instance.GetPortLogPath(portName));
                            }
                            port.LogPortMessage("CheckForDevices: BEGIN");
                            data.UpdateTaskProgress(0, string.Format(Resources.Strings.DeviceSearch_Task_Progress_Format, portName));
                            port.BaudRate = Device.DefaultBaudRate;
                            port.Handshake = Device.Handshake;
                            port.Open();
                            var numRetry = 4;
                            var timeout = 1100;
                            for (var i = 0; !data.AcceptCancelIfRequested() && (i < numRetry); ++i)
                            {
                                if (!data.AcceptCancelIfRequested())
                                {
                                    if (port.IsOpen && port.WaitForBeacon(timeout))
                                    {
                                        isValidDevicePort = true;
                                    }
                                }
                            }
                            port.LogPortMessage("CheckForDevices: END");
                        }
                        if (!data.AcceptCancelIfRequested() && isValidDevicePort)
                        {
                            if (Properties.Settings.Default.AutomaticallyConnectToDevices && data.AutoConnect)
                            {
                                data.ReportedAnyAutoConnectDevices |= true;
                                var creationInfo = new Dictionary<string, object>() { { DeviceCreationInfo.ConfigName, new DeviceCreationInfo(true, true, ActivationMode.ActivateIfFirst) } };
                                INTV.Shared.Interop.DeviceManagement.DeviceChange.ReportDeviceAdded(data, portName, Core.Model.Device.ConnectionType.Serial, creationInfo);
                                data.Task.CancelTask();
                            }
                            else
                            {
                                validPorts.Add(portName);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Access is denied to the port or the current process, or another process on the system,
                        // already has the specified COM port open either by a SerialPort instance or in unmanaged code.
                        if (errorLogger != null)
                        {
                            errorLogger.Log("CheckForDevices: UnauthorizedAccessException on port " + portName);
                        }
                        maxRetries = RetryCount;
                        System.Threading.Thread.Sleep(500);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // One or more of the properties for this instance are invalid. For example, the Parity, DataBits,
                        // or Handshake properties are not valid values; the BaudRate is less than or equal to zero; the
                        // ReadTimeout or WriteTimeout property is less than zero and is not InfiniteTimeout.
                        if (errorLogger != null)
                        {
                            errorLogger.Log("CheckForDevices: ArgumentOutOfRangeException on port " + portName);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // The port name does not begin with "COM", or the file type of the port is not supported.
                        if (errorLogger != null)
                        {
                            errorLogger.Log("CheckForDevices: ArgumentException on port " + portName);
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        // The port is in an invalid state, or an attempt to set the state of the underlying port failed.
                        // For example, the parameters passed from this SerialPort object were invalid.
                        if (errorLogger != null)
                        {
                            errorLogger.Log("CheckForDevices: IOException on port " + portName);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // The specified port on the current instance of the SerialPort is already open.
                        if (errorLogger != null)
                        {
                            errorLogger.Log("CheckForDevices: InvalidOperationException on port " + portName);
                        }
                    }
                    catch (Exception e)
                    {
                        // Caught some unexpected exception.
                        if (errorLogger != null)
                        {
                            errorLogger.Log("CheckForDevices: Exception on port " + portName + " with message: " + e.Message);
                        }
                        throw;
                    }
                }
            }
            if (data.CancelRequsted)
            {
                validPorts.Clear();
            }
        }

        private static void OnComplete(AsyncTaskData taskData)
        {
            var data = (CheckForDevicesTaskData)taskData;
            Action reportDialogAction = null;
            var connectedPortNames = data.LtoFlashViewModel.ActiveLtoFlashDevices.Where(d => d.Device.Port != null).Select(d => d.Device.Port.Name);
            var devicePorts = data.LtoFlashViewModel.Devices.Except(data.LtoFlashViewModel.ActiveLtoFlashDevices).Where(d => d.Device.Port != null).Select(d => d.Device.Port.Name).ToList(); // .Where(d => d.IsValid && (d != data.LtoFlashViewModel.ActiveLtoFlashDevice)).Select(d => d.Device.Port.Name).ToList();
            devicePorts.AddRange(data.ValidDevicePorts);
            var validPorts = devicePorts.Distinct();

            // Show dialog to connect to a port if we do NOT auto-connect, and were not cancelled, and have a valid port...
            // ... OR we weren't cancelled AND have a valid port that's not already in the active devices list and
            // did not already issue a call to connect to a device during the search.
            var promptToConnect = !data.Cancelled;
            if (promptToConnect)
            {
                promptToConnect = !Properties.Settings.Default.AutomaticallyConnectToDevices && validPorts.Any();
                if (!promptToConnect && !data.ReportedAnyAutoConnectDevices)
                {
                    promptToConnect = validPorts.Any();
                }
            }
            if (promptToConnect)
            {
                reportDialogAction = new Action(() => data.LtoFlashViewModel.PromptForDeviceSelection(validPorts.ToList()));
            }
            if (data.ReportNoneFound && !data.ReportedAnyAutoConnectDevices && !validPorts.Any())
            {
                if (data.LtoFlashViewModel.ActiveLtoFlashDevices.Any())
                {
                    var dialog = INTV.Shared.View.SerialPortSelectorDialog.Create(Resources.Strings.CheckForDevices_PortBrowser_Title, Resources.Strings.CheckForDevices_PortBrowser_Message, connectedPortNames, connectedPortNames, Device.DefaultBaudRate, (p) => data.LtoFlashViewModel.IsLtoFlashSerialPort(p));
                    reportDialogAction = new Action(() => dialog.ShowDialog());
                }
                else
                {
                    reportDialogAction = new Action(() => INTV.Shared.View.OSMessageBox.Show(Resources.Strings.CheckForDevices_PortBrowserNoneFound_Message, Resources.Strings.CheckForDevices_PortBrowser_Title));
                }
            }
            if (reportDialogAction != null)
            {
                // BeginInvoke so the task's progress dialog can be closed.
                SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(reportDialogAction);
            }
        }
    }
}
