// <copyright file="ExecuteDeviceCommandAsyncTaskData.cs" company="INTV Funhouse">
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

////#define REPORT_COMMAND_PERFORMANCE

using System;
using System.Linq;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    #region Delegates

    /// <summary>
    /// A delegate called when a device command ends without an error.
    /// </summary>
    /// <param name="cancelled">If <c>true</c>, the operation was cancelled before it was complete, but no errors occurred.</param>
    /// <param name="didShowProgress">If <c>true</c>, the progress bar was made visible during the operation.</param>
    /// <param name="result">The result returned from the command.</param>
    public delegate void DeviceCommandCompleteHandler(bool cancelled, bool didShowProgress, object result);

    /// <summary>
    /// A delegate used to handle errors that may occur when executing various high-level commands.
    /// </summary>
    /// <param name="errorMessage">A description of the error that occurred.</param>
    /// <param name="exception">The exception that caused the error, if applicable.</param>
    /// <returns>If this delegate returns <c>true</c>, the underlying error handler will not throw an exception.</returns>
    public delegate bool DeviceCommandErrorHandler(string errorMessage, Exception exception);

    #endregion // Delegates

    /// <summary>
    /// This class is used to execute Locutus commands asynchronously.
    /// </summary>
    /// <remarks>Upon creation, the device upon which the command will execute is marked as being
    /// "busy" executing a command. It is incumbent upon the asynchronous task's completion routine to
    /// dispose this object directly to ensure that the device recognizes that its command has completed
    /// in a timely fashion. Otherwise, further device commands will be blocked until the object is
    /// garbage collected. In the event of a command's execution resulting in an exception, if the
    /// exception is properly handled, the device will recover its ability to execute commands.</remarks>
    internal class ExecuteDeviceCommandAsyncTaskData : AsyncTaskData, IDisposable
    {
        /// <summary>
        /// How long to wait (in seconds) before command execution will cause a progress bar to appear.
        /// </summary>
        public const int ProgressBarDelayTime = 2;

        private ProtocolCommandId _commandId;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of UpdateFileSystemAsyncTaskData.
        /// </summary>
        /// <param name="device">The device to deliver the update to.</param>
        /// <param name="initialCommand">The command intended to execute.</param>
        public ExecuteDeviceCommandAsyncTaskData(Device device, ProtocolCommandId initialCommand)
            : base(null)
        {
            device.Port.LogPortMessage("<<<< ExecuteDeviceCommandAsyncTaskData Created: CommandInProgress: TRUE");
            device.IsCommandInProgress = true;
            Device = device;
            CurrentlyExecutingCommand = initialCommand;
            ProgressUpdateMode = (initialCommand == ProtocolCommandId.MultistagePseudoCommand) ? ExecuteDeviceCommandProgressUpdateMode.Multistage : ExecuteDeviceCommandProgressUpdateMode.Default;
        }

        ~ExecuteDeviceCommandAsyncTaskData()
        {
            Dispose(false);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the device to update.
        /// </summary>
        public Device Device { get; private set; }

        /// <summary>
        /// Gets or sets the command being executed. For complex operations, this may change over the lifetime of the data.
        /// </summary>
        public ProtocolCommandId CurrentlyExecutingCommand
        {
            get
            {
                return _commandId;
            }

            set
            {
                _commandId = value;
                UpdateTaskProgress();
            }
        }

        /// <summary>
        /// Gets or sets the progress update mode.
        /// </summary>
        public ExecuteDeviceCommandProgressUpdateMode ProgressUpdateMode { get; set; }

        /// <summary>
        /// Gets or sets a custom title to use for the progress bar.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets command data.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the update operation succeeded.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets additional failure message information.
        /// </summary>
        public string FailureMessage { get; set; }

        /// <summary>
        /// Gets or sets additional information about the exception to report.
        /// </summary>
        public string DeviceExceptionDetail { get; set; }

        /// <summary>
        /// Gets or sets the action to execute if the command succeeds.
        /// </summary>
        public DeviceCommandCompleteHandler OnSuccess { get; set; }

        /// <summary>
        /// Gets or sets the action to execute if the command fails.
        /// </summary>
        /// <remarks>If this method is present, it will execute to allow handling of errors. If this method is not provided,
        /// or returns false, DeviceCommandFailedException will be thrown.</remarks>
        /// <exception cref="DeviceCommandFailedException">Thrown if an error occurs executing a command, and the OnFailure handler is not defined, or returns <c>false</c>.</exception>
        public DeviceCommandErrorHandler OnFailure { get; set; }

        /// <summary>
        /// Gets or sets the result returned by the command, if applicable.
        /// </summary>
        public object Result { get; set; }

        private bool ExitedCommand { get; set; }

#if REPORT_COMMAND_PERFORMANCE
        private string DoWorkMethodName { get; set; }

        private System.Diagnostics.Stopwatch Stopwatch { get; set; }
#endif // REPORT_COMMAND_PERFORMANCE

        #endregion // Properties

        /// <summary>
        /// Creates an asynchronous task using this data and starts it. A progress bar appears after ProgressBarDelayTime seconds.
        /// </summary>
        /// <param name="doWork">The actual work to execute in the asynchronous task.</param>
        public void StartTask(Action<AsyncTaskData> doWork)
        {
            StartTask(doWork, ProgressBarDelayTime);
        }

        /// <summary>
        /// Creates an asynchronous task using this data and starts it. A progress bar appears after ProgressBarDelayTime seconds.
        /// </summary>
        /// <param name="doWork">The actual work to execute in the asynchronous task.</param>
        /// <param name="allowsCancel">If <c>true</c>, allow the operation to be cancelled.</param>
        public void StartTask(Action<AsyncTaskData> doWork, bool allowsCancel)
        {
            StartTask(doWork, allowsCancel, ProgressBarDelayTime);
        }

        /// <summary>
        /// Creates an asynchronous task using this data and starts it. A progress bar will appear after the specified amount of time passes.
        /// </summary>
        /// <param name="doWork">The actual work to execute in the asynchronous task.</param>
        /// <param name="progressBarDelayTime">How long to wait (in seconds) before the progress bar appears.</param>
        public void StartTask(Action<AsyncTaskData> doWork, double progressBarDelayTime)
        {
            StartTask(doWork, false, progressBarDelayTime);
        }

        /// <summary>
        /// Creates an asynchronous task using this data and starts it. A progress bar will appear after the specified amount of time passes.
        /// </summary>
        /// <param name="doWork">The actual work to execute in the asynchronous task.</param>
        /// <param name="allowsCancel">If <c>true</c>, allow the operation to be cancelled.</param>
        /// <param name="progressBarDelayTime">How long to wait (in seconds) before the progress bar appears.</param>
        public void StartTask(Action<AsyncTaskData> doWork, bool allowsCancel, double progressBarDelayTime)
        {
            var taskName = GetTitleForUpdate();
            var executeCommandTask = new AsyncTaskWithProgress(taskName, allowsCancel, true, progressBarDelayTime);
#if REPORT_COMMAND_PERFORMANCE
            DoWorkMethodName = doWork.Method.Name;
            System.Diagnostics.Debug.WriteLine(Device.Port.Name + ": DEVICECOMMAND START: " + DoWorkMethodName);
            Stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif // REPORT_COMMAND_PERFORMANCE
            executeCommandTask.RunTask(this, (d) => SyncWithTimerThenExecute(d, doWork), OnComplete);
        }

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion // IDisposable

        /// <summary>
        /// This is a general-purpose command completion routine for use with the asynchronous task system for executing commands on a Locutus device.
        /// </summary>
        /// <param name="taskData">The task data.</param>
        /// <remarks>This function will be called on the main thread when a Locutus command has finished executing. It will report whether the command
        /// successfully executed, and execute an optional callback upon success.</remarks>
        private static void OnComplete(AsyncTaskData taskData)
        {
            var data = (ExecuteDeviceCommandAsyncTaskData)taskData;
#if REPORT_COMMAND_PERFORMANCE
            var portName = data.Device.Port == null ? "NO_PORT" : data.Device.Port.Name;
            try
            {
#endif // REPORT_COMMAND_PERFORMANCE
                data.Device.IsCommandInProgress = false;
                data.ExitedCommand = true;
                data.Device.Port.LogPortMessage("<<<< ExecuteDeviceCommandAsyncTaskData COMPLETE: CommandInProgress: FALSE");
                var exception = data.Error;
                var exceptionErrorDetail = data.DeviceExceptionDetail;
                var failureMessage = data.FailureMessage;
                var currentlyExecutingCommand = data.CurrentlyExecutingCommand;
                bool succeeded = data.Succeeded && (exception == null);
                if (succeeded && (data.OnSuccess != null))
                {
                    data.OnSuccess(data.Cancelled, data.DidShowProgress, data.Result);
                }
                var goIdle = !succeeded;
                if (goIdle)
                {
                    data.Device.ConnectionState = ConnectionState.Idle;
                }
                if (!succeeded && (data.OnFailure != null))
                {
                    var errorMessageBuilder = new System.Text.StringBuilder();
                    errorMessageBuilder.Append(string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DeviceCommand_Generic_FailedFormat, currentlyExecutingCommand));
                    if (!string.IsNullOrWhiteSpace(exceptionErrorDetail))
                    {
                        if (errorMessageBuilder.Length > 0)
                        {
                            errorMessageBuilder.AppendLine();
                        }
                        errorMessageBuilder.Append(exceptionErrorDetail.Trim());
                    }
                    exceptionErrorDetail = errorMessageBuilder.ToString();
                    if (exception != null)
                    {
                        ErrorLog errorLog = null;
                        var errorLogMessageString = string.Empty;
                        if (exception.GetType() == typeof(DeviceCommandExecuteFailedException))
                        {
                            try
                            {
                                data.Device.Port.LogPortMessage(">>>> ExecuteDeviceCommandAsyncTaskData COMPLETE: FETCH ERROR LOG: CommandInProgress: TRUE");
                                data.Device.IsCommandInProgress = true;
                                var gotErrorSucceeded = false;
                                errorLog = Commands.DownloadErrorLog.Instance.Execute(data.Device.Port, null, out gotErrorSucceeded) as ErrorLog;
                                const int MaxDownloadErrorRetryCount = 3;
                                for (int i = 0; !gotErrorSucceeded && (errorLog == null) && (i < MaxDownloadErrorRetryCount); ++i)
                                {
                                    System.Diagnostics.Debug.WriteLine("Retry download error log attempt #" + i + 1);

                                    // Wait for a beacon and try again.
                                    if (data.Device.WaitForBeacon(ProtocolCommand.WaitForBeaconTimeout))
                                    {
                                        errorLog = Commands.DownloadErrorLog.Instance.Execute(data.Device.Port, null, out gotErrorSucceeded) as ErrorLog;
                                    }
                                }
                                errorMessageBuilder.AppendLine();
                                if (gotErrorSucceeded)
                                {
                                    if (errorLog.ErrorIds.All(id => id == ErrorLogId.Luigi))
                                    {
                                        errorLogMessageString = Resources.Strings.ErrorLog_DecodeLuigiErrorMessage;
                                    }
                                    else
                                    {
                                        var errorLogContents = errorLog.GetDetailedErrorReport(data.Device.FirmwareRevisions.Current);
                                        if (string.IsNullOrWhiteSpace(errorLogContents))
                                        {
                                            errorLogContents = Resources.Strings.ErrorBufferReport_Empty;
                                        }
                                        errorMessageBuilder.AppendLine().AppendFormat(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.ErrorBufferReport_Format, errorLogContents);
                                        errorLog = null; // we don't want to use the error log mechanism in this case
                                    }
                                }
                                else
                                {
                                    errorMessageBuilder.AppendLine().AppendLine(Resources.Strings.ErrorBufferReport_FailedToRetrieve);
                                }
                            }
                            catch (Exception e)
                            {
                                errorMessageBuilder.AppendLine().AppendLine(Resources.Strings.ErrorBufferReport_ThrewException).AppendLine().AppendLine(e.ToString());
                            }
                            finally
                            {
                                data.Device.IsCommandInProgress = false;
                                data.Device.Port.LogPortMessage(">>>> ExecuteDeviceCommandAsyncTaskData COMPLETE: FETCH ERROR LOG: CommandInProgress: FALSE");
                            }
                            exceptionErrorDetail = errorMessageBuilder.ToString();
                        }
                        if ((errorLog != null) && !string.IsNullOrEmpty(errorLogMessageString))
                        {
                            exception = new DeviceCommandFailedException(errorLogMessageString, errorLog, exception as DeviceCommandExecuteFailedException);
                        }
                        else
                        {
                            exception = new DeviceCommandFailedException(currentlyExecutingCommand, exception, exceptionErrorDetail);
                        }
                    }
                    else
                    {
                        exception = new DeviceCommandFailedException(currentlyExecutingCommand, exceptionErrorDetail);
                    }
                    if (string.IsNullOrWhiteSpace(failureMessage))
                    {
                        var endOfFirstLine = exceptionErrorDetail.IndexOf(Environment.NewLine);
                        if (endOfFirstLine > 0)
                        {
                            failureMessage = exceptionErrorDetail.Substring(0, endOfFirstLine).Trim();
                        }
                        else
                        {
                            failureMessage = exceptionErrorDetail.Trim();
                        }
                    }
                    succeeded = data.OnFailure(failureMessage, exception);
                }
                if (goIdle)
                {
                    data.Device.ConnectionState = ConnectionState.WaitForBeacon;
                }
                data.Dispose();
                if (!succeeded)
                {
                    if (string.IsNullOrWhiteSpace(failureMessage))
                    {
                        throw new DeviceCommandFailedException(currentlyExecutingCommand, exception, exceptionErrorDetail);
                    }
                    else
                    {
                        throw new DeviceCommandFailedException(currentlyExecutingCommand, failureMessage, exception, exceptionErrorDetail);
                    }
                }
#if REPORT_COMMAND_PERFORMANCE
            }
            finally
            {
                data.Stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(portName + ": DEVICECOMMAND FINISH: " + data.DoWorkMethodName + " DURATION: " + data.Stopwatch.Elapsed.ToString());
            }
#endif // REPORT_COMMAND_PERFORMANCE
            // I don't like doing this here -- it's too "UI-ey" for being in the Model, but
            // it fixes various problems and is easier than adding a universal 'on command complete' handler.
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
        }

        #region IDisposable

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                System.GC.SuppressFinalize(this);
            }
            if (!ExitedCommand)
            {
                Device.IsCommandInProgress = false;
            }
        }

        #endregion // IDisposable

        private void SyncWithTimerThenExecute(AsyncTaskData data, Action<AsyncTaskData> doWork)
        {
            const long TimeoutMilliseconds = 4000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (Device.InTimer && (stopwatch.ElapsedMilliseconds < TimeoutMilliseconds))
            {
                System.Diagnostics.Debug.WriteLine("Waiting for timer!");
                System.Threading.Thread.Sleep(68);
            }
            stopwatch.Stop();
            if ((stopwatch.ElapsedMilliseconds > TimeoutMilliseconds) && Device.InTimer)
            {
                throw new TimeoutException(Resources.Strings.SyncWithTimer_Execute_ErrorMessage);
            }
            doWork(data);
        }

        private void UpdateTaskProgress()
        {
            switch (ProgressUpdateMode)
            {
                case ExecuteDeviceCommandProgressUpdateMode.Default:
                    UpdateTaskTitle(GetTitleForUpdate());
                    break;
                case ExecuteDeviceCommandProgressUpdateMode.Multistage:
                    UpdateTaskProgress(0, CurrentlyExecutingCommand.GetProgressTitle());
                    break;
                case ExecuteDeviceCommandProgressUpdateMode.Custom:
                    break;
            }
        }

        private string GetTitleForUpdate()
        {
            var title = string.IsNullOrWhiteSpace(Title) ? CurrentlyExecutingCommand.GetProgressTitle() : Title;
            return title;
        }
    }
}
