// <copyright file="NativeMethods.cs" company="INTV Funhouse">
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

using System.Runtime.InteropServices;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Interop.IOKit
{
    /// <summary>
    /// Provides bindings to various IOKit APIs.
    /// </summary>
    internal static partial class NativeMethods
    {
        /// <summary>
        /// Return value from IOKit APIs indicating success.
        /// </summary>
        public const int Success = 0;

        #region IOSerialKeys.h

        /// <summary>
        /// Service Matching That is the 'IOProviderClass'.
        /// </summary>
        /// <remarks>From IOSerialKeys.h.</remarks>
        public static readonly string kIOSerialBSDServiceValue = "IOSerialBSDClient";

        /// <summary>
        /// Matching keys.
        /// </summary>
        /// <remarks>From IOSerialKeys.h.</remarks>
        public static readonly string kIOSerialBSDTypeKey = "IOSerialBSDClientType";

        /// <summary>
        /// [One of the c]urrently possible kIOSerialBSDTypeKey values.
        /// </summary>
        /// <remarks>From IOSerialKeys.h.</remarks>
        public static readonly string kIOSerialBSDRS232Type = "IORS232SerialStream";

        /// <summary>
        /// [One of the p]roperties that resolve to a /dev device node to open for a particular service
        /// </summary>
        /// <remarks>From IOSerialKeys.h.</remarks>
        public static readonly string kIOTTYDeviceKey = "IOTTYDevice";

        #endregion // IOSerialKeys.h

        #region IOUSBLib.h

        /// <summary>
        /// Not documented in header.
        /// </summary>
        public static readonly string kIOUSBDeviceClassName = "IOUSBDevice";

        /// <summary>
        /// Not documented in header.
        /// </summary>
        public static readonly string kIOUSBInterfaceClassName = "IOUSBInterface";

        #endregion // IOUSBLib.h

        #region USBSpec.h

        public static readonly string kUSBDeviceClass = "bDeviceClass";
        public static readonly string kUSBDeviceSubClass = "bDeviceSubClass";
        public static readonly string kUSBDeviceProtocol = "bDeviceProtocol";
        public static readonly string kUSBDeviceMaxPacketSize = "bMaxPacketSize0";
        public static readonly string kUSBVendorID = "idVendor";          // good name
        public static readonly string kUSBVendorName = kUSBVendorID;        // bad name - keep for backward compatibility
        public static readonly string kUSBProductID = "idProduct";         // good name
        public static readonly string kUSBProductName = kUSBProductID;       // bad name - keep for backward compatibility
        public static readonly string kUSBDeviceReleaseNumber = "bcdDevice";
        public static readonly string kUSBManufacturerStringIndex = "iManufacturer";
        public static readonly string kUSBProductStringIndex = "iProduct";
        public static readonly string kUSBSerialNumberStringIndex = "iSerialNumber";
        public static readonly string kUSBDeviceNumConfigs = "bNumConfigurations";
        public static readonly string kUSBInterfaceNumber = "bInterfaceNumber";
        public static readonly string kUSBAlternateSetting = "bAlternateSetting";
        public static readonly string kUSBNumEndpoints = "bNumEndpoints";
        public static readonly string kUSBInterfaceClass = "bInterfaceClass";
        public static readonly string kUSBInterfaceSubClass = "bInterfaceSubClass";
        public static readonly string kUSBInterfaceProtocol = "bInterfaceProtocol";
        public static readonly string kUSBInterfaceStringIndex = "iInterface";
        public static readonly string kUSBConfigurationValue = "bConfigurationValue";
        public static readonly string kUSB1284DeviceID = "1284 Device ID";

        #endregion // USBSpec.h

        #region ioctl

        /*
         * Sets the input speed and output speed to a non-traditional baud rate
         */
        /*
        #define IOSSIOSPEED    _IOW('T', 2, speed_t)
        #define IOSSIOSPEED_32    _IOW('T', 2, user_shspeed_t)
        #define IOSSIOSPEED_64    _IOW('T', 2, user_speed_t)
        */
        /* from preprocessor: ((__uint32_t)0x80000000 | ((sizeof(speed_t) & 0x1fff) << 16) | ((('T')) << 8) | ((2))) */

        /// <summary>
        /// This is an expansion of the macro used to define custom baud rates for serial ports on Mac.
        /// </summary>
        public const uint IOSSIOSPEED = 0x80000000 | ((sizeof(uint) & 0x1fff) << 16) | ((uint)'T' << 8) | 2;

        /// <summary>
        /// C# binding to the ioctl function used to set custom baud rates on serial ports on Mac.
        /// </summary>
        /// <param name="fileNumber">The file representing the serial port.</param>
        /// <param name="request">The kind of ioctl request being made. Only the IOSSIOSPEED request has been defined here.</param>
        /// <param name="baudRate">The baud rate to set.</param>
        /// <returns>If the function succeeds, returns zero (<c>0</c>); non-zero value indicates an error.</returns>
        /// <remarks>The C API for ioctl is actually a varargs function. Only the overload needed to specify a custom baud rate is implemented.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int ioctl(int fileNumber, uint request, ref int baudRate);

        #endregion // ioctl

        /// <summary>
        /// The default Mach port.
        /// </summary>
        public static readonly System.IntPtr MACH_PORT_NULL = System.IntPtr.Zero;

        /// <summary>
        /// Returns the Mac port used to initiate communication with IOKit.
        /// </summary>
        /// <param name="bootStrapPort">Pass MACH_PORT_NULL for the default.</param>
        /// <param name="masterPort">Receives the master port.</param>
        /// <returns>If successful, returns zero. Otherwise, one of the IOKit's kern_return_t error codes.</returns>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int IOMasterPort(System.IntPtr bootStrapPort, out System.IntPtr masterPort);

        /// <summary>
        /// Create a matching dictionary that specifies an IOService class match.
        /// </summary>
        /// <param name="name">The class name, as a const C-string. Class matching is successful on IOService's of this class or any subclass.</param>
        /// <returns>The IntPtr value returned is a native CFMutableDictionaryRef. The matching dictionary created, is returned on success, or zero on failure.</returns>
        /// <remarks> The dictionary is commonly passed to IOServiceGetMatchingServices or IOServiceAddNotification which will consume a reference, otherwise it should be released with CFRelease by the caller.
        /// A very common matching criteria for IOService is based on its class. IOServiceMatching will create a matching dictionary that specifies any IOService of a class, or its subclasses. The class is specified by C-string name.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static System.IntPtr IOServiceMatching([MarshalAs(UnmanagedType.LPStr)] string name);

        /*
        public static NSMutableDictionary IOServiceGetMatching(string name)
        {
            NSMutableDictionary dictionary = null;
            var dictionaryPointer = IOServiceMatching(name);
            if (dictionaryPointer != null)
            {
                dictionary = MonoMac.ObjCRuntime.Runtime.GetNSObject(dictionaryPointer) as NSMutableDictionary;
            }
            return dictionary;
        }*/

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int IOServiceGetMatchingServices(System.IntPtr masterPort, System.IntPtr matching, out System.IntPtr existing);
        /*
         kern_return_t
        IOServiceGetMatchingServices(
            mach_port_t masterPort,
            CFDictionaryRef matching,
            io_iterator_t * existing ); */

        public static int IOServicesMatchingDictionary(System.IntPtr masterPort, NSMutableDictionary matching, out System.IntPtr iterator)
        {
            var result = IOServiceGetMatchingServices(masterPort, matching.Handle, out iterator);
            return result;
        }

        /*! @typedef IOServiceMatchingCallback
            @abstract Callback function to be notified of IOService publication.
            @param refcon The refcon passed when the notification was installed.
            @param iterator The notification iterator which now has new objects.

        typedef void
        (*IOServiceMatchingCallback)(
            void *          refcon,
            io_iterator_t       iterator ); */


        /*! @function IOServiceAddMatchingNotification
            @abstract Look up registered IOService objects that match a matching dictionary, and install a notification request of new IOServices that match.
            @discussion This is the preferred method of finding IOService objects that may arrive at any time. The type of notification specifies the state change the caller is interested in, on IOService's that match the match dictionary. Notification types are identified by name, and are defined in IOKitKeys.h. The matching information used in the matching dictionary may vary depending on the class of service being looked up.
            @param notifyPort A IONotificationPortRef object that controls how messages will be sent when the armed notification is fired. When the notification is delivered, the io_iterator_t representing the notification should be iterated through to pick up all outstanding objects. When the iteration is finished the notification is rearmed. See IONotificationPortCreate.
            @param notificationType A notification type from IOKitKeys.h
                            <br>    kIOPublishNotification Delivered when an IOService is registered.
                            <br>    kIOFirstPublishNotification Delivered when an IOService is registered, but only once per IOService instance. Some IOService's may be reregistered when their state is changed.
                            <br>    kIOMatchedNotification Delivered when an IOService has had all matching drivers in the kernel probed and started.
                            <br>    kIOFirstMatchNotification Delivered when an IOService has had all matching drivers in the kernel probed and started, but only once per IOService instance. Some IOService's may be reregistered when their state is changed.
                            <br>    kIOTerminatedNotification Delivered after an IOService has been terminated.
            @param matching A CF dictionary containing matching information, of which one reference is always consumed by this function (Note prior to the Tiger release there was a small chance that the dictionary might not be released if there was an error attempting to serialize the dictionary). IOKitLib can construct matching dictionaries for common criteria with helper functions such as IOServiceMatching, IOServiceNameMatching, IOBSDNameMatching.
            @param callback A callback function called when the notification fires.
            @param refCon A reference constant for the callbacks use.
            @param notification An iterator handle is returned on success, and should be released by the caller when the notification is to be destroyed. The notification is armed when the iterator is emptied by calls to IOIteratorNext - when no more objects are returned, the notification is armed. Note the notification is not armed when first created.
            @result A kern_return_t error code.

        kern_return_t
        IOServiceAddMatchingNotification(
            IONotificationPortRef   notifyPort,
            const io_name_t     notificationType,
            CFDictionaryRef     matching,
            IOServiceMatchingCallback callback,
            void *          refCon,
            io_iterator_t *     notification ); */
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int IOServiceAddMatchingNotification(System.IntPtr notifyPort, [MarshalAs(UnmanagedType.LPStr)] string notificationType, System.IntPtr matching, System.IntPtr callback, System.IntPtr refCon, out System.IntPtr iterator);

        #region IOObject

        /* kern_return_t IOObjectRelease(io_object_t object );*/
        /// <summary>
        /// Releases an object handle previously returned by IOKitLib.
        /// </summary>
        /// <returns>A kern_return_t error code.</returns>
        /// <param name="nativeObject">Native object.</param>
        /// <remarks>All objects returned by IOKitLib should be released with this function when access to them is
        /// no longer needed. Using the object after it has been released may or may not return an error,
        /// depending on how many references the task has to the same object in the kernel.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int IOObjectRelease(System.IntPtr nativeObject);

        /* kern_return_t IOObjectRetain(io_object_t object); */
        /// <summary>
        /// Retains an object handle previously returned by IOKitLib.
        /// </summary>
        /// <returns>The object retain.</returns>
        /// <param name="nativeObject">The IOKit object to retain.</param>
        /// <remarks>Gives the caller an additional reference to an existing object handle previously returned by IOKitLib.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int IOObjectRetain(System.IntPtr nativeObject);

        /* uint32_t IOObjectGetUserRetainCount(io_object_t object )AVAILABLE_MAC_OS_X_VERSION_10_6_AND_LATER; */
        /// <summary>
        /// Returns the retain count for the current process of an IOKit object.
        /// </summary>
        /// <returns>If the object handle is valid, the objects user retain count is returned, otherwise zero is returned.</returns>
        /// <param name="nativeObject">An IOKit object.</param>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static uint IOObjectGetUserRetainCount(System.IntPtr nativeObject);

        #endregion // IOObject

        #region IOIterator

        /* io_object_t IOIteratorNext(io_iterator_t iterator ); */
        /// <summary>
        /// Returns the next object in an iteration.
        /// </summary>
        /// <returns>If the iterator handle is valid, the next element in the iteration is returned, otherwise zero is returned.</returns>
        /// <param name="iterator">An IOKit iterator handle.</param>
        /// <remarks>The element should be released by the caller when it is finished.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static System.IntPtr IOIteratorNext(System.IntPtr iterator);

        /* void IOIteratorReset(io_iterator_t iterator); */
        /// <summary>
        /// Resets an iteration back to the beginning.
        /// </summary>
        /// <param name="iterator">An IOKit iterator handle.</param>
        /// <remarks>If an iterator is invalid, or if the caller wants to start over, IOIteratorReset will set the iteration back to the beginning.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static void IOIteratorReset(System.IntPtr iterator);

        /* boolean_t IOIteratorIsValid(io_iterator_t iterator); */
        /// <summary>
        /// Checks an iterator is still valid.
        /// </summary>
        /// <returns>True if the iterator handle is valid, otherwise false is returned.</returns>
        /// <param name="iterator">Iterator.</param>
        /// <remarks>ome iterators will be made invalid if changes are made to the structure they are iterating over.
        /// This function checks the iterator is still valid and should be called when IOIteratorNext returns zero.
        /// An invalid iterator can be reset and the iteration restarted.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static int IOIteratorIsValid(System.IntPtr iterator);

        #endregion // IOIterator

        #region IORegistryEntry

        /* CFTypeRef IORegistryEntryCreateCFProperty(io_registry_entry_t entry, CFStringRef key, CFAllocatorRef allocator, IOOptionBits options); */
        /// <summary>
        ///  Create a CF representation of a registry entry's property.
        /// </summary>
        /// <returns>A CF container is created and returned the caller on success. The caller should release with CFRelease.</returns>
        /// <param name="entry">The registry entry handle whose property to copy.</param>
        /// <param name="key">A CFString specifying the property name.</param>
        /// <param name="allocator">The CF allocator to use when creating the CF container.</param>
        /// <param name="options">No options are currently defined.</param>
        /// <remarks>This function creates an instantaneous snapshot of a registry entry property, creating a
        /// CF container analogue in the caller's task. Not every object available in the kernel is represented
        /// as a CF container; currently OSDictionary, OSArray, OSSet, OSSymbol, OSString, OSData, OSNumber,
        /// OSBoolean are created as their CF counterparts.</remarks>
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static System.IntPtr IORegistryEntryCreateCFProperty(System.IntPtr entry, System.IntPtr key, System.IntPtr allocator, int options);

        #endregion // IORegistryEntry

        #region IONotificationPort

        /* @function IONotificationPortCreate
           @abstract Creates and returns a notification object for receiving IOKit notifications of new devices or state changes.
           @discussion Creates the notification object to receive notifications from IOKit of new device arrivals or state changes. The notification object can be supply a CFRunLoopSource, or mach_port_t to be used to listen for events.
           @param masterPort The master port obtained from IOMasterPort(). Pass kIOMasterPortDefault to look up the default master port.
           @result A reference to the notification object.

        IONotificationPortRef
        IONotificationPortCreate(
            mach_port_t     masterPort ); */
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static System.IntPtr IONotificationPortCreate(System.IntPtr masterPort);

        /*! @function IONotificationPortDestroy
            @abstract Destroys a notification object created with IONotificationPortCreate.
                      Also destroys any mach_port's or CFRunLoopSources obatined from 
                      <code>@link IONotificationPortGetRunLoopSource @/link</code>
                      or <code>@link IONotificationPortGetMachPort @/link</code>
            @param notify A reference to the notification object. 

        void
        IONotificationPortDestroy(
            IONotificationPortRef   notify ); */

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static void IONotificationPortDestroy(System.IntPtr notify);


        /*! @function IONotificationPortGetRunLoopSource
            @abstract Returns a CFRunLoopSource to be used to listen for notifications.
            @discussion A notification object may deliver notifications to a CFRunLoop 
                        by adding the run loop source returned by this function to the run loop.

                        The caller should not release this CFRunLoopSource. Just call 
                        <code>@link IONotificationPortDestroy @/link</code> to dispose of the
                        IONotificationPortRef and the CFRunLoopSource when done.
            @param notify The notification object.
            @result A CFRunLoopSourceRef for the notification object.

        CFRunLoopSourceRef
        IONotificationPortGetRunLoopSource(
            IONotificationPortRef   notify ); */
        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        extern public static System.IntPtr IONotificationPortGetRunLoopSource(System.IntPtr notify);

        #endregion // IONotificationPort

        #region Power State

        /*! @function       IORegisterForSystemPower
            @abstract       Connects the caller to the Root Power Domain IOService for the purpose of receiving sleep & wake notifications for the system.
                            Does not provide system shutdown and restart notifications.
            @discussion     Provides sleep/wake notifications to applications. Requires that applications acknowledge
                            some, but not all notifications. Register for sleep/wake notifications will deliver these messages
                            over the sleep/wake lifecycle:

            - kIOMessageSystemWillSleep is delivered at the point the system is initiating a 
                non-abortable sleep.
                Callers MUST acknowledge this event by calling @link IOAllowPowerChange @/link.
                If a caller does not acknowledge the sleep notification, the sleep will continue anyway after
                a 30 second timeout (resulting in bad user experience). 
                Delivered before any hardware is powered off.

            - kIOMessageSystemWillPowerOn is delivered at early wakeup time, before most hardware has been
                powered on. Be aware that any attempts to access disk, network, the display, etc. may result
                in errors or blocking your process until those resources become available.
                Caller must NOT acknowledge kIOMessageSystemWillPowerOn; the caller must simply return from its handler.
            
            - kIOMessageSystemHasPoweredOn is delivered at wakeup completion time, after all device drivers and
                hardware have handled the wakeup event. Expect this event 1-5 or more seconds after initiating
                system wakeup.
                Caller must NOT acknowledge kIOMessageSystemHasPoweredOn; the caller must simply return from its handler.

            - kIOMessageCanSystemSleep indicates the system is pondering an idle sleep, but gives apps the
                chance to veto that sleep attempt. 
                Caller must acknowledge kIOMessageCanSystemSleep by calling @link IOAllowPowerChange @/link
                or @link IOCancelPowerChange @/link. Calling IOAllowPowerChange will not veto the sleep; any
                app that calls IOCancelPowerChange will veto the idle sleep. A kIOMessageCanSystemSleep 
                notification will be followed up to 30 seconds later by a kIOMessageSystemWillSleep message.
                or a kIOMessageSystemWillNotSleep message.

            - kIOMessageSystemWillNotSleep is delivered when some app client has vetoed an idle sleep
                request. kIOMessageSystemWillNotSleep may follow a kIOMessageCanSystemSleep notification,
                but will not otherwise be sent.
                Caller must NOT acknowledge kIOMessageSystemWillNotSleep; the caller must simply return from its handler.
                
              To deregister for sleep/wake notifications, the caller must make two calls, in this order:
                - Call IODeregisterForSystemPower with the 'notifier' argument returned here.
                - Then call IONotificationPortDestroy passing the 'thePortRef' argument
                    returned here.

            @param refcon       Caller may provide data to receive s an argument to 'callback' on power state changes.
            @param thePortRef   On return, thePortRef is a pointer to an IONotificationPortRef, which will deliver the power notifications. 
                                The port is allocated by this function and must be later released by the caller (after calling <code>@link IODeregisterForSystemPower @/link</code>). 
                                The caller should also enable IONotificationPortRef by calling <code>@link IONotificationPortGetRunLoopSource @/link</code>, or 
                                <code>@link IONotificationPortGetMachPort @/link</code>, or <code>@link IONotificationPortSetDispatchQueue @/link</code>.
            @param callback     A c-function which is called during the notification.
            @param notifier     On success, returns a pointer to a unique notifier which caller must keep and pass to a subsequent call to IODeregisterForSystemPower.
            @result             Returns a io_connect_t session for the IOPMrootDomain or MACH_PORT_NULL if request failed. 
                                Caller must close return value via IOServiceClose() after calling IODeregisterForSystemPower on the notifier argument.

        io_connect_t IORegisterForSystemPower ( void * refcon, IONotificationPortRef * thePortRef, IOServiceInterestCallback callback, io_object_t * notifier ) AVAILABLE_MAC_OS_X_VERSION_10_0_AND_LATER;
        */

        /*! @function           IODeregisterForSystemPower
            @abstract           Disconnects the caller from the Root Power Domain IOService after receiving system power state change notifications. (Caller must also destroy the IONotificationPortRef returned from IORegisterForSystemPower.)
            @param notifier     The object returned from IORegisterForSystemPower.
            @result             Returns kIOReturnSuccess or an error condition if request failed.

        IOReturn IODeregisterForSystemPower ( io_object_t * notifier ) AVAILABLE_MAC_OS_X_VERSION_10_0_AND_LATER;
        */

        #endregion Power State
    }
}
