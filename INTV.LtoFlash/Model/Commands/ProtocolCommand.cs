// <copyright file="ProtocolCommand.cs" company="INTV Funhouse">
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

////#define ENABLE_DIAGNOSTIC_OUTPUT
////#define REPORT_COMMAND_PERFORMANCE
#define LOG_COMMAND_DATA

using System;
using System.IO;
using System.Linq;
using INTV.Shared.Model;

namespace INTV.LtoFlash.Model.Commands
{
    /// <summary>
    /// Defines basic services and structure for commands to communicate with Locutus hardware.
    /// </summary>
    internal abstract class ProtocolCommand : INTV.Core.Utility.ByteSerializer
    {
        #region Constants

        /// <summary>
        /// Timeout to wait for a LOCUTUS beacon.
        /// </summary>
        public const int WaitForBeaconTimeout = 4000;

        /// <summary>
        /// A CommandRecord consists of four bytes of information.
        /// 0 : Lead byte of 0xAA
        /// 1 : ProtocolCommandId
        /// 2 : ProtocolCommandId ^ 0xFF
        /// 3 : Closing byte of 0x55
        /// </summary>
        private const int CommandRecordSize = sizeof(uint);

        /// <summary>
        /// There are four unsigned 32-bit integer arguments as data for a command.
        /// </summary>
        private const int ArgumentRecordSize = 4 * sizeof(uint);

        /// <summary>
        /// The command also contains a 32-bit CRC.
        /// </summary>
        private const int CrcSize = sizeof(uint);

        /// <summary>
        /// Total command data size in bytes.
        /// </summary>
        private const int ProtocolCommandRecordSize = CommandRecordSize + ArgumentRecordSize + CrcSize;

        private const byte CommandRecordHeadByte = 0xAA;
        private const byte CommandRecordTailByte = 0x55;
        private const byte Ack = 0xAA;
        private const byte Nak = 0xEE;
        private const byte Success = 0x00;
        private const byte Error = 0xFF;

        #endregion // Constants

        private static object _lock = new object();

        #region Constructors

        /// <summary>
        /// Creates a new instance of ProtocolCommand.
        /// </summary>
        /// <param name="command">Command identifier in the wire protocol.</param>
        /// <param name="responseTimeout">How long to wait before receiving a response from the device.</param>
        protected ProtocolCommand(ProtocolCommandId command, int responseTimeout)
            : this(command, responseTimeout, 0xffffffff)
        {
        }

        /// <summary>
        /// Creates a new instance of ProtocolCommand.
        /// </summary>
        /// <param name="command">Command identifier in the wire protocol.</param>
        /// <param name="responseTimeout">How long to wait before receiving a response from the device.</param>
        /// <param name="arg0">First argument in the data packet sent via the protocol.</param>
        protected ProtocolCommand(ProtocolCommandId command, int responseTimeout, uint arg0)
            : this(command, responseTimeout, arg0, 0xffffffff)
        {
        }

        /// <summary>
        /// Creates a new instance of ProtocolCommand.
        /// </summary>
        /// <param name="command">Command identifier in the wire protocol.</param>
        /// <param name="responseTimeout">How long to wait before receiving a response from the device.</param>
        /// <param name="arg0">First argument in the data packet sent via the protocol.</param>
        /// <param name="arg1">second argument in the data packet sent via the protocol.</param>
        protected ProtocolCommand(ProtocolCommandId command, int responseTimeout, uint arg0, uint arg1)
            : this(command, responseTimeout, arg0, arg1, 0xffffffff)
        {
        }

        /// <summary>
        /// Creates a new instance of ProtocolCommand.
        /// </summary>
        /// <param name="command">Command identifier in the wire protocol.</param>
        /// <param name="responseTimeout">How long to wait before receiving a response from the device.</param>
        /// <param name="arg0">First argument in the data packet sent via the protocol.</param>
        /// <param name="arg1">second argument in the data packet sent via the protocol.</param>
        /// <param name="arg2">Third argument in the data packet sent via the protocol.</param>
        protected ProtocolCommand(ProtocolCommandId command, int responseTimeout, uint arg0, uint arg1, uint arg2)
            : this(command, responseTimeout, arg0, arg1, arg2, 0xffffffff)
        {
        }

        /// <summary>
        /// Creates a new instance of ProtocolCommand.
        /// </summary>
        /// <param name="command">Command identifier in the wire protocol.</param>
        /// <param name="responseTimeout">How long to wait before receiving a response from the device.</param>
        /// <param name="arg0">First argument in the data packet sent via the protocol.</param>
        /// <param name="arg1">second argument in the data packet sent via the protocol.</param>
        /// <param name="arg2">Third argument in the data packet sent via the protocol.</param>
        /// <param name="arg3">Fourth argument in the data packet sent via the protocol.</param>
        protected ProtocolCommand(ProtocolCommandId command, int responseTimeout, uint arg0, uint arg1, uint arg2, uint arg3)
        {
            Command = command;
            ResponseTimeout = responseTimeout;
#if DEBUG
            if (FailNextCommand)
            {
                arg0 = 0x00100001;
                arg1 = 0x00100001;
                arg2 = 0x00100001;
                arg3 = 0x00100001;
                ////arg0 = 0xDEADBEEF;
                ////arg1 = 0xDEADBEEF;
                ////arg2 = 0xDEADBEEF;
                ////arg3 = 0xDEADBEEF;
            }
#endif
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the command's identifier. Locutus hardware identifies the command with this value.
        /// </summary>
        public ProtocolCommandId Command { get; private set; }

        /// <summary>
        /// Gets a value indicating he amount of time allowed to pass before reading the response of a command before it is assumed that the command has failed.
        /// </summary>
        public int ResponseTimeout { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the command can be cancelled.
        /// </summary>
        public virtual bool CanCancel
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the first data argument for the command.
        /// </summary>
        protected uint Arg0 { get; private set; }

        /// <summary>
        /// Gets the second data argument for the command.
        /// </summary>
        protected uint Arg1 { get; private set; }

        /// <summary>
        /// Gets the third data argument for the command.
        /// </summary>
        protected uint Arg2 { get; private set; }

        /// <summary>
        /// Gets the fourth data argument for the command.
        /// </summary>
        protected uint Arg3 { get; private set; }

        /// <summary>
        /// Gets the CRC of the command packet.
        /// </summary>
        protected uint Crc { get; private set; }

        #region ByteSerializer Properties

        /// <inheritdoc />
        public override int SerializeByteCount
        {
            get { return ProtocolCommandRecordSize; }
        }

        /// <inheritdoc />
        public override int DeserializeByteCount
        {
            get { return ProtocolCommandRecordSize; }
        }

        #endregion // ByteSerializer Properties

        #endregion // Properties

#if DEBUG

        private static bool FailNextCommand
        {
            get
            {
                lock (_lock)
                {
                    var failNextCommand = _failNextCommand;
                    _failNextCommand = false;
                    return failNextCommand;
                }
            }

            set
            {
                lock (_lock)
                {
                    _failNextCommand = value;
                }
            }
        }
        private static bool _failNextCommand;

        /// <summary>
        /// Sets up the next protocol command to fail. The approach is simplistic, in that the arguments
        /// for the function being called are set to what should be bad values. This does not guarantee
        /// failure, but for testing purposes, it suffices. All commands that involve reading or writing
        /// blocks of data to / from device RAM will fail, as the data alignment requirements will be violated.
        /// </summary>
        internal static void InjectCommandFailure()
        {
            FailNextCommand = true;
        }

        private static bool ReturnNakForNextCommand
        {
            get
            {
                lock (_lock)
                {
                    var returnNakForNextCommand = _returnNakForNextCommand;
                    _returnNakForNextCommand = false;
                    return returnNakForNextCommand;
                }
            }

            set
            {
                lock (_lock)
                {
                    _returnNakForNextCommand = value;
                }
            }
        }
        private static bool _returnNakForNextCommand;

        /// <summary>
        /// Sets up the next protocol command (excluding Garbage Collect / Ping) to act as if a NAK
        /// was returned on the port. This is only for testing error handling.
        /// </summary>
        internal static void InjectCommandNak()
        {
            ReturnNakForNextCommand = true;
        }

#endif // DEBUG

        #region ByteSerializer Overrides

        /// <inheritdoc />
        public override int Serialize(INTV.Core.Utility.BinaryWriter writer)
        {
            var protocolCommandBuffer = new byte[CommandRecordSize + ArgumentRecordSize];

            // The try/finally pattern avoids potential double-dispose issues with the memory stream.
            System.IO.MemoryStream commandWithArgsStream = null;
            try
            {
                commandWithArgsStream = new System.IO.MemoryStream(protocolCommandBuffer);
                using (var tempBinaryWriter = new INTV.Shared.Utility.ASCIIBinaryWriter(commandWithArgsStream))
                {
                    commandWithArgsStream = null;
                    tempBinaryWriter.Write(CommandRecordHeadByte);
                    tempBinaryWriter.Write((byte)Command);
                    tempBinaryWriter.Write((byte)(((int)Command) ^ 0xFF));
                    tempBinaryWriter.Write(CommandRecordTailByte);
                    tempBinaryWriter.Write(Arg0);
                    tempBinaryWriter.Write(Arg1);
                    tempBinaryWriter.Write(Arg2);
                    tempBinaryWriter.Write(Arg3);
                    tempBinaryWriter.Seek(0, SeekOrigin.Begin);
                    Crc = INTV.Core.Utility.Crc32.OfStream(tempBinaryWriter.BaseStream);
                    writer.Write(protocolCommandBuffer, 0, protocolCommandBuffer.Length);
#if LOG_COMMAND_DATA
                    var dataSent = new System.Text.StringBuilder();
                    foreach (var value in protocolCommandBuffer)
                    {
                        dataSent.AppendFormat("{0} ", value.ToString("X2"));
                    }
                    dataSent.AppendFormat("CRC: {0}", Crc.ToString("X8"));
                    _dataSent = dataSent.ToString();
#endif // LOG_COMMAND_DATA
                }
            }
            finally
            {
                if (commandWithArgsStream != null)
                {
                    commandWithArgsStream.Dispose();
                }
            }
            writer.Write(Crc);

            return ProtocolCommandRecordSize;
        }

        /// <inheritdoc />
        protected override int Deserialize(INTV.Core.Utility.BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        #endregion // ByteSerializer Overrides

        /// <summary>
        /// Sends the command to the specified target.
        /// </summary>
        /// <param name="target">The target to which to send the command.</param>
        /// <param name="taskData">If non-<c>null</c>, execution data to update for error reporting.</param>
        /// <param name="succeeded">Receives whether the command was sent, and response received, successfully.</param>
        /// <returns>The data returned by the command. In the case of commands that do not send additional results, this will be the same as the succeeded result.</returns>
        public virtual object Execute(IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            var result = ExecuteCommand(target, taskData, out succeeded);
            return result;
        }

        /// <summary>
        /// Sends the command to the specified target.
        /// </summary>
        /// <typeparam name="T">The data type of the response data from the command.</typeparam>
        /// <param name="target">The target to which to send the command.</param>
        /// <param name="taskData">If non-<c>null</c>, execution data to update for error reporting.</param>
        /// <returns>The data returned by the command. In the case of commands that do not send additional results, this will be the same as the succeeded result.</returns>
        public T Execute<T>(IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData)
        {
            bool succeeded = false;
            var result = (T)Execute(target, taskData, out succeeded);
            if (!succeeded)
            {
                throw new InvalidOperationException("Command " + Command + " failed!", taskData.Error);
            }
            return result;
        }

        /// <summary>
        /// Sends the command to the specified target.
        /// </summary>
        /// <param name="target">The target to which to send the command.</param>
        /// <param name="taskData">If non-<c>null</c>, execution data to update for error reporting.</param>
        /// <param name="succeeded">Receives whether the command was sent, and response received, successfully.</param>
        /// <returns><c>true</c> if the command and its data were successfully sent, <c>false</c> otherwise.</returns>
        protected bool ExecuteCommand(IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, out bool succeeded)
        {
            succeeded = ExecuteCommandWithData(target, taskData, (System.IO.Stream)null, null);
            return succeeded;
        }

        /// <summary>
        /// Sends the command to the specified target. The command sends additional data following the initial packet.
        /// </summary>
        /// <param name="target">The target to which to send the command.</param>
        /// <param name="taskData">If non-<c>null</c>, execution data to update for error reporting.</param>
        /// <param name="sourceDataStream">The stream containing the additional data to send to the target. If this value is null, no additional data needs to be sent.</param>
        /// <param name="onSuccess">Action to execute upon successful command completion.</param>
        /// <returns><c>true</c> if the command and its data were successfully sent, <c>false</c> otherwise.</returns>
        protected bool ExecuteCommandWithData(IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, System.IO.Stream sourceDataStream, Action onSuccess)
        {
            bool succeeded = false;
            ExecuteCore<bool>(target, taskData, sourceDataStream, null, onSuccess, out succeeded);
            return succeeded;
        }

        /// <summary>
        /// Sends the command to the specified target and reads the response. The command responds with additional data.
        /// </summary>
        /// <typeparam name="T">The data type of the response data from the command.</typeparam>
        /// <param name="target">The target to which to send the command.</param>
        /// <param name="taskData">If non-<c>null</c>, execution data to update for error reporting.</param>
        /// <param name="inflate">The function to inflate the flat byte array response to response type.</param>
        /// <param name="succeeded">Receives whether the command was sent, and response received, successfully.</param>
        /// <returns>The reply data. If the command fails, this value will be default(T).</returns>
        protected T ExecuteWithResponse<T>(IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, Func<System.IO.Stream, T> inflate, out bool succeeded)
        {
            var response = ExecuteCore(target, taskData, null, inflate, null, out succeeded);
            return response;
        }

        /// <summary>
        /// If a command expects to receive a response payload, this function must be implemented to read the response data.
        /// </summary>
        /// <param name="reader">The binary reader to retrieve the response data.</param>
        /// <returns>The raw byte stream response returned in response to a command.</returns>
        protected virtual byte[] ReadResponseData(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reads the response to a command - ACK, NAK, or other. See remarks.
        /// </summary>
        /// <param name="reader">The reader to use to get the response.</param>
        /// <param name="command">The command that is executing.</param>
        /// <param name="errorDetail">Receives detailed error information if the command is not acknowledged.</param>
        /// <param name="timedOut">Receives a value indicating whether or not command execution timed out.</param>
        /// <returns>The response, which could be Ack, Nak, or possibly another invalid value.</returns>
        /// <remarks>From Joe's implementation of lc_wait_acknak:
        /// This function is weird. It must consume some number of partial
        /// beacons w/out advancing the CRC, and then an ACK/NAK. Only the ACK
        /// advances the CRC. If we get an unexpected byte, for now just flag
        /// an error.
        /// To simplify things, this function does not update the CRC. The
        /// caller should update the CRC.
        /// </remarks>
        private static byte GetCommandAcknowledgement(INTV.Shared.Utility.ASCIIBinaryReader reader, ProtocolCommandId command, ref string errorDetail, out bool timedOut)
        {
            byte result = 0xFF; // unknown state
            timedOut = false;
#if false
            try
            {
                do
                {
                    result = reader.ReadByte();
                } while (Device.BeaconCharacters.Contains((char)result));
            }
            catch (TimeoutException)
            {
                result = Nak;
            }
#else
            int state = -1;
            var prevTimeout = reader.BaseStream.ReadTimeout;

            if (reader.BaseStream.CanTimeout)
            {
                reader.BaseStream.ReadTimeout = 200;
            }
            try
            {
                const int RetryCount = 6;
                for (int i = 0; i < RetryCount; ++i)
                {
                    try
                    {
                        do
                        {
                            timedOut = false;
                            result = reader.ReadByte();
#if DEBUG
                            var forceNak = false;
                            switch (command)
                            {
                                case ProtocolCommandId.Ping:
                                case ProtocolCommandId.GarbageCollect:
                                    // never inject a fake NAK for these
                                    break;
                                default:
                                    forceNak = ReturnNakForNextCommand;
                                    break;
                            }
                            if (forceNak)
                            {
                                result = Nak;
                            }
#endif // DEBUG
                            if ((result == Ack) || (result == Nak))
                            {
                                i = RetryCount;
                                var errorMessage = "Command returned NAK directly on port getting command acknowledgement for: " + command + ", not 'virtual' Nak due to garbage.";
                                if (!string.IsNullOrEmpty(errorDetail))
                                {
                                    errorDetail += Environment.NewLine;
                                }
                                errorDetail += errorMessage;
                                DebugOutputIf(result == Nak, errorMessage);
                                break;
                            }
                            if ((state == -1 || state == '\n') && result == 'L')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'L') && result == 'O')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'O') && result == 'C')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'C') && result == 'U')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'U') && result == 'T')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'T') && result == 'U')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'U') && result == 'S')
                            {
                                state = result;
                            }
                            else if ((state == -1 || state == 'S') && result == '\n')
                            {
                                state = result;
                            }
                            else
                            {
                                var errorMessage = "Unexpected response when executing command: " + command + ", returned: " + result;
                                if (!string.IsNullOrEmpty(errorDetail))
                                {
                                    errorDetail += Environment.NewLine;
                                }
                                errorDetail += errorMessage;
                                DebugOutput(errorMessage);
                                state = -1;
                                i = RetryCount;
                            }
                        }
                        while (Device.BeaconCharacters.Contains((char)state));
                    }
                    catch (TimeoutException)
                    {
                        var errorMessage = "Command response for: " + command + " timed out! Returning 'virtual' Nak";
                        errorDetail += errorMessage;
                        DebugOutput(errorMessage);
                        timedOut = true;
                        result = Nak;
                    }
                }
            }
            finally
            {
                // reader.BaseStream may go bad if cord was pulled during communication.
                if ((reader.BaseStream != null) && reader.BaseStream.CanTimeout)
                {
                    reader.BaseStream.ReadTimeout = prevTimeout;
                }
            }
#endif
            return result;
        }

        private static byte GetCommandSuccess(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            return reader.ReadByte();
        }

        private static uint GetResponseCrc(INTV.Shared.Utility.ASCIIBinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        private static bool ValidateResponse(System.IO.Stream rawResponseData, uint responseCrc)
        {
            rawResponseData.Seek(0, System.IO.SeekOrigin.Begin);
            var dataCrc = INTV.Core.Utility.Crc32.OfStream(rawResponseData);
            return dataCrc == responseCrc;
        }

        private static void RecordErrorResult(ProtocolCommandId command, Exception exception, ExecuteDeviceCommandAsyncTaskData taskData, string errorDetail)
        {
            var errorBuilder = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(errorDetail))
            {
                errorBuilder.AppendLine("Error Detail:").AppendLine(errorDetail);
            }
            var failureMessage = command.GetFailureString();
            errorBuilder.AppendLine(failureMessage);
            var errorMessage = errorBuilder.ToString();
            DebugOutput(errorMessage);
            if (taskData != null)
            {
                taskData.DeviceExceptionDetail = errorDetail;
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutputIf(bool condition, object message)
        {
            System.Diagnostics.Debug.WriteLineIf(condition, message);
        }

        private string _dataSent;

        private T ExecuteCore<T>(IStreamConnection target, ExecuteDeviceCommandAsyncTaskData taskData, System.IO.Stream sourceDataStream, Func<System.IO.Stream, T> inflate, Action onSuccess, out bool succeeded)
        {
            succeeded = false;
            var errorDetail = string.Empty;
            var response = default(T);
            var timedOut = false;
#if REPORT_COMMAND_PERFORMANCE
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var reportSuffix = sourceDataStream == null ? "RESP" : "DATA";
            try
            {
#endif // REPORT_COMMAND_PERFORMANCE
                if (taskData != null)
                {
                    taskData.CurrentlyExecutingCommand = Command;
                }
                lock (_lock)
                {
                    target.LogPortMessage("EXECUTE: " + Command);
                    Exception exception = null;
                    var previousWriteTimeout = -1;
                    if (target.WriteStream.CanTimeout)
                    {
                        previousWriteTimeout = target.WriteStream.WriteTimeout;
                    }
                    try
                    {
                        using (var writer = new INTV.Shared.Utility.ASCIIBinaryWriter(target.WriteStream))
                        {
                            _dataSent = string.Empty;
                            var commandBytesWritten = Serialize(writer);
                            target.LogPortMessage("EXECUTE: wrote " + commandBytesWritten + " bytes for command: " + Command + ", data sent: " + _dataSent);
                        }

                        if (target.ReadStream.CanTimeout)
                        {
                            target.ReadStream.ReadTimeout = ResponseTimeout;
                        }

                        using (var reader = new INTV.Shared.Utility.ASCIIBinaryReader(target.ReadStream))
                        {
                            using (var resultStream = new System.IO.MemoryStream())
                            {
                                var ack = GetCommandAcknowledgement(reader, Command, ref errorDetail, out timedOut);

                                if (ack == Ack)
                                {
                                    resultStream.WriteByte(ack);
                                    byte[] responseData = null;
                                    if (sourceDataStream != null)
                                    {
                                        SendCommandPayload(target, sourceDataStream, resultStream);
                                    }
                                    else if (inflate != null)
                                    {
                                        responseData = ReceiveResponse(reader, resultStream);
                                    }

                                    var success = GetCommandSuccess(reader);
                                    resultStream.WriteByte(success);

                                    var crc = GetResponseCrc(reader);
                                    succeeded = ValidateResponse(resultStream, crc);

                                    if (succeeded && (responseData != null) && (inflate != null))
                                    {
                                        using (var responseStream = new MemoryStream(responseData))
                                        {
                                            response = inflate(responseStream);
                                        }
                                    }
                                    if (succeeded && (onSuccess != null))
                                    {
                                        onSuccess();
                                    }
                                    if (success != Success)
                                    {
                                        var errorMessage = "Command " + Command + " did not succeed: " + success.ToString("X2") + "(" + System.Convert.ToChar(success) + ")";
                                        if (!string.IsNullOrEmpty(errorDetail))
                                        {
                                            errorDetail += Environment.NewLine;
                                        }
                                        errorDetail += errorMessage;
                                        target.LogPortMessage(errorMessage);
                                        DebugOutput(errorMessage);
                                        timedOut = target.WaitForBeacon(WaitForBeaconTimeout); // try to drain any remaining bytes and sync back up again
                                        succeeded = false;
                                        exception = new DeviceCommandExecuteFailedException(Command, Arg0, Arg1, Arg2, Arg3, success);
                                    }
                                }
                                else if (ack == Nak)
                                {
                                    var errorMessage = "Command " + Command + " returned NAK!";
                                    target.LogPortMessage(errorMessage);
                                    DebugOutput(errorMessage);
                                    timedOut = !target.WaitForBeacon(WaitForBeaconTimeout);
                                    if (!string.IsNullOrEmpty(errorDetail))
                                    {
                                        errorDetail += Environment.NewLine;
                                    }
                                    errorDetail += errorMessage;
                                }
                            }
                        }
                    }
                    catch (TimeoutException e)
                    {
                        timedOut = true;
                        var errorMessage = "Timed out executing command: " + Command;
                        target.LogPortMessage(errorMessage);
                        DebugOutput(errorMessage);
                        if (!string.IsNullOrEmpty(errorDetail))
                        {
                            errorDetail += Environment.NewLine;
                        }
                        errorDetail += errorMessage;
                        exception = e;

                        // TODO: Report specific failure message back to user.
                    }
                    catch (System.IO.IOException e)
                    {
                        var errorMessage = "IO Exception executing command: " + Command;
                        target.LogPortMessage(errorMessage);
                        DebugOutput(errorMessage);
                        if (!string.IsNullOrEmpty(errorDetail))
                        {
                            errorDetail += Environment.NewLine;
                        }
                        errorDetail += errorMessage;
                        exception = e;

                        // TODO: Report specific failure message back to user.
                        // One circumstance in which this occurs, and which we do not wish to report, is when killing the simulator application.
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        var errorMessage = "UnauthorizedAccess Exception executing command: " + Command;
                        target.LogPortMessage(errorMessage);
                        DebugOutput(errorMessage);
                        if (!string.IsNullOrEmpty(errorDetail))
                        {
                            errorDetail += Environment.NewLine;
                        }
                        errorDetail += errorMessage;
                        exception = e;

                        // TODO: Report specific failure message back to user.
                        // One circumstance in which this occurs, was after unplugging the device, which we may not want to report.
                    }
                    finally
                    {
                        // target.WriteStream may go to null if cord is pulled during communication w/ the device.
                        if ((target.WriteStream != null) && target.WriteStream.CanTimeout)
                        {
                            target.WriteStream.WriteTimeout = previousWriteTimeout;
                        }
                        if (!succeeded)
                        {
                            RecordErrorResult(Command, exception, taskData, errorDetail);
                            if (exception != null)
                            {
                                throw exception;
                            }
                        }
                    }
                }
                if (taskData != null)
                {
                    taskData.Succeeded = succeeded;
                }
#if REPORT_COMMAND_PERFORMANCE
            }
            finally
            {
                stopwatch.Stop();
                ReportDuration(stopwatch, reportSuffix, target.Name);
            }
#endif // REPORT_COMMAND_PERFORMANCE
            return response;
        }

        private void SendCommandPayload(IStreamConnection target, System.IO.Stream sourceDataStream, System.IO.MemoryStream resultStream)
        {
            if ((target != null) && target.WriteStream.CanTimeout)
            {
                var minTimeout = target.EstimateDataTransferTime(sourceDataStream.Length);
                target.WriteStream.WriteTimeout = (int)(minTimeout * 4);
            }
            sourceDataStream.CopyTo(target.WriteStream);
            var commandBytesWritten = (int)sourceDataStream.Length;
            sourceDataStream.Seek(0, System.IO.SeekOrigin.Begin);
            sourceDataStream.CopyTo(resultStream);
        }

        private byte[] ReceiveResponse(INTV.Shared.Utility.ASCIIBinaryReader reader, System.IO.MemoryStream resultStream)
        {
            var responseData = ReadResponseData(reader);
            resultStream.Write(responseData, 0, responseData.Length);
            return responseData;
        }

        [System.Diagnostics.Conditional("REPORT_COMMAND_PERFORMANCE")]
        private void ReportDuration(System.Diagnostics.Stopwatch stopwatch, string suffix, string port)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(port + ": COMMAND+{0} DURATION: {6} {1}(0x{2}, 0x{3}, 0x{4}, 0x{5})", suffix, Command, Arg0.ToString("X8"), Arg1.ToString("X8"), Arg2.ToString("X8"), Arg3.ToString("X8"), stopwatch.Elapsed));
        }
    }
}
