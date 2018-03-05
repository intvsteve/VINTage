// <copyright file="IOMessage.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// Values from the IOMessage.h, IOReturn.h and related headers in IOKit.
    /// </summary>
    /// <remarks>Unused and deprecated values are not included.</remarks>
    public enum IOMessage : uint
    {
        #region General Interest Messages

        /// <summary>
        /// The IO service is terminated.
        /// </summary>
        KIOMessageServiceIsTerminated = IOMessageHelpers.CommonMessage | 0x010,

        /// <summary>
        /// The IO service is suspended.
        /// </summary>
        KIOMessageServiceIsSuspended = IOMessageHelpers.CommonMessage | 0x020,

        /// <summary>
        /// The IO message service is resumed.
        /// </summary>
        KIOMessageServiceIsResumed = IOMessageHelpers.CommonMessage | 0x030,

        /// <summary>
        /// The IO service is requesting close.
        /// </summary>
        KIOMessageServiceIsRequestingClose = IOMessageHelpers.CommonMessage | 0x100,

        /// <summary>
        /// The IO service is attempting open.
        /// </summary>
        KIOMessageServiceIsAttemptingOpen = IOMessageHelpers.CommonMessage | 0x101,

        /// <summary>
        /// The IO service was closed.
        /// </summary>
        KIOMessageServiceWasClosed = IOMessageHelpers.CommonMessage | 0x110,

        /// <summary>
        /// The IO service busy state changed.
        /// </summary>
        KIOMessageServiceBusyStateChange = IOMessageHelpers.CommonMessage | 0x120,

        /// <summary>
        /// The IO console security changed.
        /// </summary>
        KIOMessageConsoleSecurityChange = IOMessageHelpers.CommonMessage | 0x128,

        /// <summary>
        /// An IO service property changed.
        /// </summary>
        KIOMessageServicePropertyChange = IOMessageHelpers.CommonMessage | 0x130,

        /// <summary>
        /// The IO copy client ID message.
        /// </summary>
        KIOMessageCopyClientID = IOMessageHelpers.CommonMessage | 0x330,

        /// <summary>
        /// The system capability changed.
        /// </summary>
        KIOMessageSystemCapabilityChange = IOMessageHelpers.CommonMessage | 0x340,

        /// <summary>
        /// The IO device signaled wakeup.
        /// </summary>
        KIOMessageDeviceSignaledWakeup = IOMessageHelpers.CommonMessage | 0x350,

        /// <summary>
        /// The device will move to a lower power state.
        /// Sent to notification clients of kind kIOAppPowerStateInterest and kIOGeneralInterest.
        /// </summary>
        KIOMessageDeviceWillPowerOff = IOMessageHelpers.CommonMessage | 0x210,

        /// <summary>
        /// The device has must moved to a higher power state.
        /// Sent to notification clients of kind kIOAppPowerStateInterest and kIOGeneralInterest.
        /// </summary>
        KIOMessageDeviceHasPoweredOn = IOMessageHelpers.CommonMessage | 0x230,

        #endregion General Interest Messages

        #region In-Kernel System Shutdown and Restart Notifications

        /// <summary>
        /// Indicates an imminent system shutdown. Recipients have a limited 
        /// amount of time to respond, otherwise the system will timeout and
        /// shutdown even without a response. Never delivered to user space notification clients.
        /// </summary>
        KIOMessageSystemWillPowerOff = IOMessageHelpers.CommonMessage | 0x250,

        /// <summary>
        /// Indicates an imminent system restart. Recipients have a limited
        /// amount of time to respond, otherwise the system will timeout and
        /// restart even without a response. 
        /// Never delivered to user space notification clients.
        /// </summary>
        KIOMessageSystemWillRestart = IOMessageHelpers.CommonMessage | 0x310,

        /// <summary>
        /// Indicates an imminent system shutdown, paging device now unavailable.
        /// Recipients have a limited amount of time to respond, otherwise the
        /// system will timeout and shutdown even without a response.
        /// Never delivered to user space notification clients.
        /// </summary>
        KIOMessageSystemPagingOff = IOMessageHelpers.CommonMessage | 0x255,

        #endregion // In-Kernel System Shutdown and Restart Notifications

        #region System Sleep and Wake Notifications

        /// <summary>
        /// Announces/Requests permission to proceed to system sleep.
        /// Delivered to user clients of IORegisterForSystemPower.
        /// </summary>
        KIOMessageCanSystemSleep = IOMessageHelpers.CommonMessage | 0x270,

        /// <summary>
        /// Announces that the system has retracted a previous attempt to sleep,
        /// it follows kIOMessageCanSystemSleep. Delivered to user clients of IORegisterForSystemPower.
        /// </summary>
        KIOMessageSystemWillNotSleep = IOMessageHelpers.CommonMessage | 0x290,

        /// <summary>
        /// Announces that sleep is beginning.
        /// Delivered to user clients of IORegisterForSystemPower.
        /// </summary>
        KIOMessageSystemWillSleep = IOMessageHelpers.CommonMessage | 0x280,

        /// <summary>
        /// TAnnounces that the system is beginning to power the device tree, most 
        /// devices are unavailable at this point. Delivered to user clients of IORegisterForSystemPower.
        /// </summary>
        KIOMessageSystemWillPowerOn = IOMessageHelpers.CommonMessage | 0x320,

        /// <summary>
        /// Announces that the system and its devices have woken up.
        /// Delivered to user clients of IORegisterForSystemPower.
        /// </summary>
        KIOMessageSystemHasPoweredOn = IOMessageHelpers.CommonMessage | 0x300,

        #endregion // System Sleep and Wake Notifications
    }

    /// <summary>
    /// Some helper values for use with defining IOMessage. Note that the general macros
    /// were not used. Instead, the values specifically needed for IOKit power monitoring are
    /// presented,
    /// </summary>
    internal static class IOMessageHelpers
    {
        private const uint SysIOKit = (0x38u & 0x3f) << 26;
        private const uint SubIOKitCommon = 0;

        /// <summary>
        /// The base value used to OR with a specific message value to produce an IOKit IOMessage value.
        /// </summary>
        public const uint CommonMessage = SysIOKit | SubIOKitCommon;
    }
}
