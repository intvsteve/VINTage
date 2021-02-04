// <copyright file="DeviceHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2021 All Rights Reserved
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

////#define ENABLE_PORTLOGGER_TO_DEBUG_OUTPUT

using System;
using System.Linq;
using INTV.LtoFlash.Model.Commands;
using INTV.Shared.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Extension methods for the Device class.
    /// </summary>
    internal static partial class DeviceHelpers
    {
        #region Utility Functions

        /// <summary>
        /// Checks to determine if it is safe to start executing a device command.
        /// </summary>
        /// <param name="device">The device upon which the command is to execute.</param>
        /// <returns><c>true</c>, if it is safe to start command execution on the given device, <c>false</c> otherwise.</returns>
        public static bool IsSafeToStartCommand(this Device device)
        {
            return (device != null) && !device.IsCommandInProgress;
        }

        /// <summary>
        /// Estimates the amount of time it will take to transfer the given number of bytes, in milliseconds.
        /// </summary>
        /// <param name="port">The port to compute the estimate for.</param>
        /// <param name="numberOfBytes">The number of bytes in the data transfer.</param>
        /// <param name="scottyFactor">The Scotty factor to apply to the estimate.</param>
        /// <returns>The estimated data transfer time in milliseconds including the Scotty Factor.
        /// <see href="http://wiki.c2.com/?ScottyFactor#:~:text=The%20ScottyFactor%20is%20a%20factor%20you%20apply%20to,Star%20Trek%20III,%20when%20Kirk%20asks%20%22Mr.%20Scott."/></returns>
        public static int EstimateDataTransferTime(this IStreamConnection port, long numberOfBytes, int scottyFactor)
        {
            var minTimeout = port.EstimateDataTransferTime(numberOfBytes);
            var scottyTimeout = (int)(minTimeout * scottyFactor);
            return scottyTimeout;
        }

        /// <summary>
        /// Logs a message to the port.
        /// </summary>
        /// <param name="port">The port to log a message for.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>Because this extension method is conditional on ENABLE_PORT_LOG, you may wish to use it rather than
        /// directly using the method on <see cref="IStreamConnection"/>, as the calls and strings passed to it can be
        /// optimized out, if the build is so configured.</remarks>
        [System.Diagnostics.Conditional("ENABLE_PORT_LOG")]
        public static void LogPortMessage(this IStreamConnection port, string message)
        {
            var portName = "<null>";
            if (port != null)
            {
                portName = port.Name;
                port.Log(message);
            }
            LogPortMessageToDebugger(message, portName);
        }

        /// <summary>
        /// Waits for a full Locutus beacon message to be received.
        /// </summary>
        /// <param name="device">The device that should be emitting the beacon.</param>
        /// <param name="beaconTimeoutMS">Timeout (in milliseconds) to listen for a LOCUTUS beacon.</param>
        /// <returns><c>true</c> if the complete beacon was received.</returns>
        public static bool WaitForBeacon(this Device device, int beaconTimeoutMS)
        {
            return device.Port.WaitForBeacon(beaconTimeoutMS);
        }

        /// <summary>
        /// Waits for a full Locutus beacon message to be received.
        /// </summary>
        /// <param name="port">The port at which to listen for a beacon.</param>
        /// <param name="beaconTimeoutMS">Timeout (in milliseconds) to listen for a LOCUTUS beacon.</param>
        /// <returns><c>true</c> if the complete beacon was received.</returns>
        public static bool WaitForBeacon(this INTV.Shared.Model.IStreamConnection port, int beaconTimeoutMS)
        {
            var previousTimeout = port.ReadTimeout;
            port.ReadTimeout = beaconTimeoutMS;
            var reader = new ASCIIBinaryReader(port.ReadStream);
            bool detectedBeacon = false;
            var desiredCharacterIndex = 0;
            try
            {
                using (port.SetInUse(ProtocolCommand.UpdatePortChunkSizeConfigurations))
                {
                    do
                    {
                        var desiredCharacter = Device.BeaconCharacters[desiredCharacterIndex];
                        var character = reader.ReadChar();
                        port.LogPortMessage("WaitForBeacon received: '" + character + "'");
                        if (character == desiredCharacter)
                        {
                            ++desiredCharacterIndex;
                            detectedBeacon = character == Device.BeaconCharacters.Last();
                        }
                    }
                    while (!detectedBeacon);
                }
            }
            catch (TimeoutException)
            {
                port.LogPortMessage("WaitForBeacon: TimeoutException");
            }
            catch (UnauthorizedAccessException)
            {
                port.LogPortMessage("WaitForBeacon: UnauthorizedAccessException");
                throw;
            }
            catch (System.IO.IOException)
            {
                port.LogPortMessage("WaitForBeacon: IOException");
                throw;
            }
            finally
            {
                port.ReadTimeout = previousTimeout;
            }

            return detectedBeacon;
        }

        [System.Diagnostics.Conditional("ENABLE_PORTLOGGER_TO_DEBUG_OUTPUT")]
        private static void LogPortMessageToDebugger(string message, string portName)
        {
            System.Diagnostics.Debug.WriteLine("**** " + message + " " + portName);
        }

        #endregion // Utility Functions
    }
}
