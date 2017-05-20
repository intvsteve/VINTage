// <copyright file="NativeMethods.cs" company="INTV Funhouse">
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

////#define EXPERIMENT_GET_ALL_PROPERTIES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace INTV.Shared.Interop.SetupDi
{
    /// <summary>
    /// A minimal (and arbitrary, ad-hoc) set of functions and types from the SetupAPI in Win32 needed to help refine device identification in VINTage.
    /// </summary>
    /// <remarks>Adapted from: http://stackoverflow.com/questions/26732291/how-to-get-bus-reported-device-description-using-c-sharp and p/Invoke.net.</remarks>
    internal static partial class NativeMethods
    {
        /// <summary>Expose property for device name.</summary>
        internal static readonly object SPDRP_DEVPKEY_NAME = SPDRP_PROPERTY.SPDRP_FRIENDLYNAME;

        /// <summary>Expose property for device description.</summary>
        internal static readonly object SPDRP_DEVPKEY_Device_BusReportedDeviceDesc = SPDRP_PROPERTY.SPDRP_DEVICEDESC;

        /// <summary>Expose property for device hardware ID.</summary>
        internal static readonly object SPDRP_DEVPKEY_Device_InstanceId = SPDRP_PROPERTY.SPDRP_HARDWAREID;

        /// <summary>The data area passed to a system call is too small.</summary>
        private const int ERROR_INSUFFICIENT_BUFFER = 122; // (0x7A)

        /// <summary>
        /// Retrieves the GUID(s) associated with the specified class name. This list is built based on the classes currently installed on the system.
        /// </summary>
        /// <param name="className">The name of the class for which to retrieve the class GUID.</param>
        /// <param name="classGuidArray1stItem">A pointer to an array to receive the list of GUIDs associated with the specified class name.</param>
        /// <param name="classGuidArraySize">The number of GUIDs in the array whose first element is the address of <paramref name="classGuidArray1stItem"/>.</param>
        /// <param name="requiredSize">Receives the number of GUIDs associated with the class name. If this number is greater than the size of the supplied buffer, the number indicates how large the array must be in order to store all the GUIDs.</param>
        /// <returns>TRUE if it is successful. Otherwise, it returns FALSE and the logged error can be retrieved by making a call to GetLastError.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff550937(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiClassGuidsFromNameW([MarshalAs(UnmanagedType.LPWStr)] string className, ref Guid classGuidArray1stItem, uint classGuidArraySize, out uint requiredSize);

        /// <summary>
        /// Gets the GUIDs associated with the given class name.
        /// </summary>
        /// <param name="className">The name of the class for which to retrieve the class GUIDs.</param>
        /// <returns>An enumeration of the matching class GUIDs.</returns>
        internal static IEnumerable<Guid> GetClassGuids(string className)
        {
            var classGuids = new Guid[1];
            uint requiredSize;

            if (!NativeMethods.SetupDiClassGuidsFromNameW(className, ref classGuids[0], (uint)classGuids.Length, out requiredSize))
            {
                var success = false;
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    classGuids = new Guid[requiredSize];
                    success = NativeMethods.SetupDiClassGuidsFromNameW(className, ref classGuids[0], requiredSize, out requiredSize);
                }
                if (!success)
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }

            return classGuids;
        }

        /// <summary>
        /// The SetupDiGetClassDevs function returns a handle to a device information set that contains requested device information elements for a local computer.
        /// </summary>
        /// <param name="classGuid">A pointer to the GUID for a device setup class or a device interface class. For this form of the invocation, this value must be valid.</param>
        /// <param name="iEnumerator">Enumerator name. For this invocation, set the value to IntPtr.Zero.</param>
        /// <param name="hParent">Parent class. For this invocation, set the value to IntPtr.Zero.</param>
        /// <param name="nFlags">Options for the enumeration.</param>
        /// <returns>Device information set pointer, which must be deallocated using SetupDiDestroyDeviceInfoList.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551069(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr iEnumerator, IntPtr hParent, DiGetClassFlags nFlags);

        /// <summary>
        /// Gets the device information set for the requested class of devices on the local computer.
        /// </summary>
        /// <param name="classGuid">The class GUID.</param>
        /// <returns>The device information set.</returns>
        internal static IntPtr SetupDiGetClassDevs(Guid classGuid)
        {
            var guid = classGuid;
            return SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, DiGetClassFlags.DIGCF_PRESENT);
        }

        /// <summary>
        /// Deletes a device information set and frees all associated memory.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to the device information set to delete.</param>
        /// <returns>The function returns TRUE if it is successful. Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff550996(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll")]
        internal static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        /// <summary>
        /// Retrieves a SP_DEVINFO_DATA structure that specifies a device information element in a device information set.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to the device information set for which to return an SP_DEVINFO_DATA structure that represents a device information element.</param>
        /// <param name="memberIndex">A zero-based index of the device information element to retrieve.</param>
        /// <param name="deviceInterfaceData">A pointer to an SP_DEVINFO_DATA structure to receive information about an enumerated device information element. The caller must set DeviceInfoData.cbSize to sizeof(SP_DEVINFO_DATA).</param>
        /// <returns>The function returns TRUE if it is successful. Otherwise, it returns FALSE and the logged error can be retrieved with a call to GetLastError.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551010(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, uint memberIndex, ref SP_DEVINFO_DATA deviceInterfaceData);

        /// <summary>
        /// Gets device info data structure for the given information set and index.
        /// </summary>
        /// <param name="deviceInfoSet">The device information set.</param>
        /// <param name="memberIndex">Zero-based index of the device information element to retrieve.</param>
        /// <returns>If successful, the information data structure; otherwise <c>null</c>.</returns>
        internal static object SetupDiEnumDeviceInfo(IntPtr deviceInfoSet, uint memberIndex)
        {
            SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
            deviceInfoData.cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            var success = SetupDiEnumDeviceInfo(deviceInfoSet, memberIndex, ref deviceInfoData);
            if (!success)
            {
                // No more devices in the device information set
                return null;
            }
            return deviceInfoData;
        }

#if EXPERIMENT_GET_ALL_PROPERTIES

        internal static IEnumerable<object> GetDeviceRegistryPropertyKeys(IntPtr deviceInfoSet, object deviceInfoData)
        {
            var values = Enum.GetValues(typeof(SPDRP_PROPERTY)).Cast<object>();
            return values;
        }

#endif // EXPERIMENT_GET_ALL_PROPERTIES

        /// <summary>
        /// Retrieves a specified Plug and Play device property.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to a device information set that contains a device information element that represents the device for which to retrieve a Plug and Play property.</param>
        /// <param name="deviceInfoData">A pointer to an SP_DEVINFO_DATA structure that specifies the device information element in DeviceInfoSet.</param>
        /// <param name="property">One of the values that specifies the property to be retrieved.</param>
        /// <param name="propertyRegDataType">A variable that receives the data type of the property that is being retrieved. This is one of the standard registry data types. This parameter is optional and can be NULL.</param>
        /// <param name="propertyBuffer">A pointer to a buffer that receives the property that is being retrieved. If this parameter is set to NULL, and PropertyBufferSize is also set to zero, the function returns the required size for the buffer in RequiredSize.</param>
        /// <param name="propertyBufferSize">The size, in bytes, of the PropertyBuffer buffer.</param>
        /// <param name="requiredSize">A pointer to a variable of type DWORD that receives the required size, in bytes, of the PropertyBuffer buffer that is required to hold the data for the requested property. This parameter is optional and can be NULL.</param>
        /// <returns>TRUE if the call was successful. Otherwise, it returns FALSE and the logged error can be retrieved by making a call to GetLastError. SetupDiGetDeviceRegistryProperty returns the ERROR_INVALID_DATA error code if the requested property does not exist for a device or if the property data is not valid.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551967(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryPropertyW(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, SPDRP_PROPERTY property, out REGISTRY_TYPE propertyRegDataType, byte[] propertyBuffer, uint propertyBufferSize, out uint requiredSize);

        /// <summary>
        /// Retrieves a specified Plug and Play device property.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to a device information set that contains a device information element that represents the device for which to retrieve a Plug and Play property.</param>
        /// <param name="deviceInfoData">A pointer to an SP_DEVINFO_DATA structure that specifies the device information element in DeviceInfoSet.</param>
        /// <param name="property">One of the values that specifies the property to be retrieved.</param>
        /// <param name="propertyRegDataType">A variable that receives the data type of the property that is being retrieved. This is one of the standard registry data types. This parameter is optional and can be NULL.</param>
        /// <param name="propertyBuffer">A pointer to a buffer that receives the property that is being retrieved. If this parameter is set to NULL, and PropertyBufferSize is also set to zero, the function returns the required size for the buffer in RequiredSize.</param>
        /// <param name="propertyBufferSize">The size, in bytes, of the PropertyBuffer buffer.</param>
        /// <param name="requiredSize">A pointer to a variable of type DWORD that receives the required size, in bytes, of the PropertyBuffer buffer that is required to hold the data for the requested property. This parameter is optional and can be NULL.</param>
        /// <returns>TRUE if the call was successful. Otherwise, it returns FALSE and the logged error can be retrieved by making a call to GetLastError. SetupDiGetDeviceRegistryProperty returns the ERROR_INVALID_DATA error code if the requested property does not exist for a device or if the property data is not valid.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551967(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryPropertyW(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, SPDRP_PROPERTY property, out REGISTRY_TYPE propertyRegDataType, IntPtr propertyBuffer, uint propertyBufferSize, out uint requiredSize);

        internal static object GetDeviceRegistryProperty(IntPtr deviceInfoSet, object deviceInfoData, object property)
        {
            object propertyValue = null;
            var deviceData = (SP_DEVINFO_DATA)deviceInfoData;
            var key = (SPDRP_PROPERTY)property;
            REGISTRY_TYPE type;
            uint requiredSize;

            if (!SetupDiGetDeviceRegistryPropertyW(deviceInfoSet, ref deviceData, key, out type, IntPtr.Zero, 0, out requiredSize))
            {
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    var data = new byte[requiredSize];
                    if (SetupDiGetDeviceRegistryPropertyW(deviceInfoSet, ref deviceData, key, out type, data, (uint)data.Length, out requiredSize))
                    {
                        propertyValue = ConvertPropertyValue(data, type);
                    }
                }
            }
            return propertyValue;
        }

        private static object ConvertPropertyValue(byte[] rawData, REGISTRY_TYPE type)
        {
            object value = null;
            switch (type)
            {
                case REGISTRY_TYPE.REG_NONE:
                case REGISTRY_TYPE.REG_SZ:
                case REGISTRY_TYPE.REG_EXPAND_SZ:
                case REGISTRY_TYPE.REG_LINK:
                    // NOTE: Strings and lists of strings always have a NULL-terminator -- two bytes.
                    value = Encoding.Unicode.GetString(rawData, 0, rawData.Length - 2);
                    break;
                case REGISTRY_TYPE.REG_MULTI_SZ:
                    var dataAsString = Encoding.Unicode.GetString(rawData, 0, rawData.Length - 2);
                    var stringValues = dataAsString.Split('\0');
                    value = stringValues;
                    break;
                case REGISTRY_TYPE.REG_DWORD_LITTLE_ENDIAN: // same as REG_DWORD
                case REGISTRY_TYPE.REG_DWORD_BIG_ENDIAN:
                case REGISTRY_TYPE.REG_QWORD: // same as REG_QWORD_LITTLE_ENDIAN
                case REGISTRY_TYPE.REG_BINARY:
                case REGISTRY_TYPE.REG_RESOURCE_LIST:
                case REGISTRY_TYPE.REG_FULL_RESOURCE_DESCRIPTOR:
                case REGISTRY_TYPE.REG_RESOURCE_REQUIREMENTS_LIST:
                default:
                    System.Diagnostics.Debug.WriteLine("Conversion for REGISTRY_TYPE not implemented: " + type);
                    value = rawData;
                    break;
            }
            return value;
        }

        /// <summary>
        /// Flags controlling what is included in the device information set built by SetupDiGetClassDevs. (from SetupAPI.h)
        /// </summary>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551069(v=vs.85).aspx"/></remarks>
        [Flags]
        private enum DiGetClassFlags
        {
            /// <summary>
            /// Return only the device that is associated with the system default device interface, if one is set, for the specified device interface classes.
            /// </summary>
            /// <remarks>Only valid with DIGCF_DEVICEINTERFACE</remarks>
            DIGCF_DEFAULT = 0x00000001,

            /// <summary>
            /// Return only devices that are currently present in a system.
            /// </summary>
            DIGCF_PRESENT = 0x00000002,

            /// <summary>
            /// Return a list of installed devices for all device setup classes or all device interface classes.
            /// </summary>
            DIGCF_ALLCLASSES = 0x00000004,

            /// <summary>
            /// Return only devices that are a part of the current hardware profile.
            /// </summary>
            DIGCF_PROFILE = 0x00000008,

            /// <summary>
            /// Return devices that support device interfaces for the specified device interface classes. This flag must be set in the Flags parameter if the Enumerator parameter specifies a device instance ID.
            /// </summary>
            DIGCF_DEVICEINTERFACE = 0x00000010,
        }

        /// <summary>
        /// The SP_DEVINFO_DATA structure defines a device instance that is a member of a device information set.
        /// </summary>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551010(v=vs.85).aspx"/></remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public UIntPtr Reserved;
        }

        /// <summary>
        /// Registry data types.
        /// </summary>
        private enum REGISTRY_TYPE : uint
        {
            /// <summary>No value type</summary>
            REG_NONE = 0,

            /// <summary>Unicode nul terminated string.</summary>
            REG_SZ = 1,

            /// <summary>Unicode nul terminated string (with environment variable references)</summary>
            REG_EXPAND_SZ = 2,

            /// <summary>Free form binary.</summary>
            REG_BINARY = 3,

            /// <summary>32-bit number.</summary>
            REG_DWORD = 4,

            /// <summary>32-bit number (same as REG_DWORD).</summary>
            REG_DWORD_LITTLE_ENDIAN = 4,

            /// <summary>32-bit number</summary>
            REG_DWORD_BIG_ENDIAN = 5,

            /// <summary>Symbolic Link (unicode).</summary>
            REG_LINK = 6,

            /// <summary>Multiple Unicode strings</summary>
            REG_MULTI_SZ = 7,

            /// <summary>Resource list in the resource map.</summary>
            REG_RESOURCE_LIST = 8,

            /// <summary>Resource list in the hardware description</summary>
            REG_FULL_RESOURCE_DESCRIPTOR = 9,

            /// <summary>Not documented.</summary>
            REG_RESOURCE_REQUIREMENTS_LIST = 10,

            /// <summary>64-bit number</summary>
            REG_QWORD = 11,

            /// <summary>64-bit number (same as REG_QWORD)</summary>
            REG_QWORD_LITTLE_ENDIAN = 11
        }

        /// <summary>
        /// Device registry property codes. (from SetupAPI.h)
        /// </summary>
        private enum SPDRP_PROPERTY : uint
        {
            /// <summary>Retrieves a REG_SZ string that contains the description of a device. </summary>
            SPDRP_DEVICEDESC = 0x00000000,

            /// <summary>Retrieves a REG_MULTI_SZ string that contains the list of hardware IDs for a device. For information about hardware IDs, see Device Identification Strings.</summary>
            SPDRP_HARDWAREID = 0x00000001,

            /// <summary>Retrieves a REG_MULTI_SZ string that contains the list of compatible IDs for a device. For information about compatible IDs, see Device Identification Strings.</summary>
            SPDRP_COMPATIBLEIDS = 0x00000002,

            /// <summary>Unused value.</summary>
            SPDRP_UNUSED0 = 0x00000003,

            /// <summary>Retrieves a REG_SZ string that contains the service name for a device.</summary>
            SPDRP_SERVICE = 0x00000004,

            /// <summary>Unused value.</summary>
            SPDRP_UNUSED1 = 0x00000005,

            /// <summary>Unused value.</summary>
            SPDRP_UNUSED2 = 0x00000006,

            /// <summary>Retrieves a REG_SZ string that contains the device setup class of a device.</summary>
            SPDRP_CLASS = 0x00000007,

            /// <summary>Retrieves a REG_SZ string that contains the GUID that represents the device setup class of a device.</summary>
            SPDRP_CLASSGUID = 0x00000008,

            /// <summary>Retrieves a string that identifies the device's software key (sometimes called the driver key). For more information about driver keys, see Registry Trees and Keys for Devices and Drivers.</summary>
            SPDRP_DRIVER = 0x00000009,

            /// <summary>Retrieves a bitwise OR of a device's configuration flags in a DWORD value. The configuration flags are represented by the CONFIGFLAG_Xxx bitmasks that are defined in Regstr.h.</summary>
            SPDRP_CONFIGFLAGS = 0x0000000A,

            /// <summary>Retrieves a REG_SZ string that contains the name of the device manufacturer.</summary>
            SPDRP_MFG = 0x0000000B,

            /// <summary>Retrieves a REG_SZ string that contains the friendly name of a device.</summary>
            SPDRP_FRIENDLYNAME = 0x0000000C,

            /// <summary>Retrieves a REG_SZ string that contains the hardware location of a device. </summary>
            SPDRP_LOCATION_INFORMATION = 0x0000000D,

            /// <summary>Retrieves a REG_SZ string that contains the name that is associated with the device's PDO. For more information, see IoCreateDevice.</summary>
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E,

            /// <summary>Retrieves a bitwise OR of the following CM_DEVCAP_Xxx flags in a DWORD. The device capabilities that are represented by these flags correspond to the device capabilities that are represented by the members of the DEVICE_CAPABILITIES structure. The CM_DEVCAP_Xxx constants are defined in Cfgmgr32.h.</summary>
            SPDRP_CAPABILITIES = 0x0000000F,

            /// <summary>Retrieves a DWORD value set to the value of the UINumber member of the device's DEVICE_CAPABILITIES structure.</summary>
            SPDRP_UI_NUMBER = 0x00000010,

            /// <summary>Retrieves a REG_MULTI_SZ string that contains the names of a device's upper filter drivers.</summary>
            SPDRP_UPPERFILTERS = 0x00000011,

            /// <summary>Retrieves a REG_MULTI_SZ string that contains the names of a device's lower-filter drivers.</summary>
            SPDRP_LOWERFILTERS = 0x00000012,

            /// <summary>The GUID for the device's bus type.</summary>
            SPDRP_BUSTYPEGUID = 0x00000013,

            /// <summary>Retrieves the device's legacy bus type as an INTERFACE_TYPE value (defined in Wdm.h and Ntddk.h).</summary>
            SPDRP_LEGACYBUSTYPE = 0x00000014,

            /// <summary>The device's bus number</summary>
            SPDRP_BUSNUMBER = 0x00000015,

            /// <summary>Retrieves a REG_SZ string that contains the name of the device's enumerator.</summary>
            SPDRP_ENUMERATOR_NAME = 0x00000016,

            /// <summary>Retrieves a SECURITY_DESCRIPTOR structure for a device.</summary>
            SPDRP_SECURITY = 0x00000017,

            /// <summary>Retrieves a REG_SZ string that contains the device's security descriptor. For information about security descriptor strings, see Security Descriptor Definition Language (Windows). For information about the format of security descriptor strings, see Security Descriptor Definition Language (Windows).</summary>
            SPDRP_SECURITY_SDS = 0x00000018,

            /// <summary>Retrieves a DWORD value that represents the device's type. For more information, see Specifying Device Types.</summary>
            SPDRP_DEVTYPE = 0x00000019,

            /// <summary>Retrieves a DWORD value that indicates whether a user can obtain exclusive use of the device. The returned value is one if exclusive use is allowed, or zero otherwise. For more information, see IoCreateDevice.</summary>
            SPDRP_EXCLUSIVE = 0x0000001A,

            /// <summary>Retrieves a bitwise OR of a device's characteristics flags in a DWORD. For a description of these flags, which are defined in Wdm.h and Ntddk.h, see the DeviceCharacteristics parameter of the IoCreateDevice function.</summary>
            SPDRP_CHARACTERISTICS = 0x0000001B,

            /// <summary>The device's address.</summary>
            SPDRP_ADDRESS = 0x0000001C,

            /// <summary>Retrieves a format string (REG_SZ) used to display the UINumber value.</summary>
            SPDRP_UI_NUMBER_DESC_FORMAT = 0X0000001D,

            /// <summary>Retrieves a CM_POWER_DATA structure that contains the device's power management information.</summary>
            SPDRP_DEVICE_POWER_DATA = 0x0000001E,

            /// <summary>Retrieves the device's current removal policy as a DWORD that contains one of the CM_REMOVAL_POLICY_Xxx values that are defined in Cfgmgr32.h.</summary>
            SPDRP_REMOVAL_POLICY = 0x0000001F,

            /// <summary>Retrieves the device's hardware-specified default removal policy as a DWORD that contains one of the CM_REMOVAL_POLICY_Xxx values that are defined in Cfgmgr32.h.</summary>
            SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020,

            /// <summary>Retrieves the device's override removal policy (if it exists) from the registry, as a DWORD that contains one of the CM_REMOVAL_POLICY_Xxx values that are defined in Cfgmgr32.h.</summary>
            SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021,

            /// <summary>Retrieves a DWORD value that indicates the installation state of a device. The installation state is represented by one of the CM_INSTALL_STATE_Xxx values that are defined in Cfgmgr32.h. The CM_INSTALL_STATE_Xxx values correspond to the DEVICE_INSTALL_STATE enumeration values. </summary>
            SPDRP_INSTALL_STATE = 0x00000022,

            /// <summary>Retrieves a REG_MULTI_SZ string that represents the location of the device in the device tree.</summary>
            SPDRP_LOCATION_PATHS = 0x00000023,

            /// <summary>The device's base container ID.</summary>
            SPDRP_BASE_CONTAINERID = 0x00000024,

            /// <summary>Upper bound on ordinal values.</summary>
            SPDRP_MAXIMUM_PROPERTY = 0x00000025,
        }

        /// <summary>Capability flags. (from cfgmgr32.h)</summary>
        [Flags]
        private enum CM_DEVCAP : uint
        {
            /// <summary>LockSupported flag</summary>
            CM_DEVCAP_LOCKSUPPORTED = 0x00000001,

            /// <summary>EjectSupported flag</summary>
            CM_DEVCAP_EJECTSUPPORTED = 0x00000002,

            /// <summary>Removable flag</summary>
            CM_DEVCAP_REMOVABLE = 0x00000004,

            /// <summary>DockDevice flag</summary>
            CM_DEVCAP_DOCKDEVICE = 0x00000008,

            /// <summary>UniqueID flag</summary>
            CM_DEVCAP_UNIQUEID = 0x00000010,

            /// <summary>SilentInstall flag</summary>
            CM_DEVCAP_SILENTINSTALL = 0x00000020,

            /// <summary>RawDeviceOK flag</summary>
            CM_DEVCAP_RAWDEVICEOK = 0x00000040,

            /// <summary>SurpriseRemovalOK flag</summary>
            CM_DEVCAP_SURPRISEREMOVALOK = 0x00000080,

            /// <summary>HardwareDisabled flag</summary>
            CM_DEVCAP_HARDWAREDISABLED = 0x00000100,

            /// <summary>NonDynamic flag</summary>
            CM_DEVCAP_NONDYNAMIC = 0x00000200,
        }
    }
}
