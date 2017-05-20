// <copyright file="NativeMethods.WPF.cs" company="INTV Funhouse">
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
        #region devpropdef.h

        #region Property type modifiers used to modify base DEVPROP_TYPE_ values, as appropriate. Not valid as standalone DEVPROPTYPE values.

        private const DEVPROP_TYPE DEVPROP_TYPEMOD_ARRAY = (DEVPROP_TYPE)0x00001000; // array of fixed-sized data elements
        private const DEVPROP_TYPE DEVPROP_TYPEMOD_LIST = (DEVPROP_TYPE)0x00002000; // list of variable-sized data elements

        #endregion

        #region Max base DEVPROP_TYPE_ and DEVPROP_TYPEMOD_ values.

        private const DEVPROP_TYPE MAX_DEVPROP_TYPE = (DEVPROP_TYPE)0x00000019; // max valid DEVPROP_TYPE_ value
        private const DEVPROP_TYPE MAX_DEVPROP_TYPEMOD = (DEVPROP_TYPE)0x00002000; // max valid DEVPROP_TYPEMOD_ value

        #endregion

        #region Bitmasks for extracting DEVPROP_TYPE_ and DEVPROP_TYPEMOD_ values.

        private const DEVPROP_TYPE DEVPROP_MASK_TYPE = (DEVPROP_TYPE)0x00000FFF; // range for base DEVPROP_TYPE_ values
        private const DEVPROP_TYPE DEVPROP_MASK_TYPEMOD = (DEVPROP_TYPE)0x0000F000; // mask for DEVPROP_TYPEMOD_ type modifiers

        #endregion

        #endregion // devpropdef.h

        #region devpkey.h

        /// <summary>Common DEVPKEY used to retrieve the display name for an object.</summary>
        internal static readonly DEVPROPKEY DEVPKEY_NAME = CreateDevPropKey(0xb725f130, 0x47ef, 0x101a, 0xa5, 0xf1, 0x02, 0x60, 0x8c, 0x9e, 0xeb, 0xac, 10);    // DEVPROP_TYPE_STRING

        #region SetupAPI SPDRP_XXX Device Properties

        private static readonly DEVPROPKEY DEVPKEY_Device_DeviceDesc = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 2);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_HardwareIds = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 3);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_Device_CompatibleIds = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 4);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_Device_Service = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 6);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_Class = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 9);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_ClassGuid = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 10);    // DEVPROP_TYPE_GUID
        private static readonly DEVPROPKEY DEVPKEY_Device_Driver = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 11);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_ConfigFlags = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 12);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_Manufacturer = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 13);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_FriendlyName = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_LocationInfo = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 15);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_PDOName = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 16);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_Capabilities = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 17);    // DEVPROP_TYPE_UNINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_UINumber = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 18);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_UpperFilters = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 19);    // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_Device_LowerFilters = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 20);    // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_Device_BusTypeGuid = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 21);    // DEVPROP_TYPE_GUID
        private static readonly DEVPROPKEY DEVPKEY_Device_LegacyBusType = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 22);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_BusNumber = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 23);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_EnumeratorName = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 24);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_Security = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 25);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR
        private static readonly DEVPROPKEY DEVPKEY_Device_SecuritySDS = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 26);    // DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DevType = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 27);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_Exclusive = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 28);    // DEVPROP_TYPE_BOOLEAN
        private static readonly DEVPROPKEY DEVPKEY_Device_Characteristics = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 29);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_Address = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 30);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_UINumberDescFormat = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 31);    // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_PowerData = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 32);    // DEVPROP_TYPE_BINARY
        private static readonly DEVPROPKEY DEVPKEY_Device_RemovalPolicy = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 33);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_RemovalPolicyDefault = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 34);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_RemovalPolicyOverride = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 35);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_InstallState = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 36);    // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_LocationPaths = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 37);    // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_Device_BaseContainerId = CreateDevPropKey(0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 38);    // DEVPROP_TYPE_GUID

        #endregion // SetupAPI SPDRP_XXX Device Properties

        /// <summary>Property key for a device's instance ID.</summary>
        internal static readonly DEVPROPKEY DEVPKEY_Device_InstanceId = CreateDevPropKey(0x78c34fc8, 0x104a, 0x4aca, 0x9e, 0xa4, 0x52, 0x4d, 0x52, 0x99, 0x6e, 0x57, 256);   // DEVPROP_TYPE_STRING

        #region Device Experience IDs

        private static readonly DEVPROPKEY DEVPKEY_Device_ModelId = CreateDevPropKey(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 2);     // DEVPROP_TYPE_GUID
        private static readonly DEVPROPKEY DEVPKEY_Device_FriendlyNameAttributes = CreateDevPropKey(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 3);     // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_ManufacturerAttributes = CreateDevPropKey(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 4);     // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_PresenceNotForDevice = CreateDevPropKey(0x80d81ea6, 0x7473, 0x4b0c, 0x82, 0x16, 0xef, 0xc1, 0x1a, 0x2c, 0x4c, 0x8b, 5);     // DEVPROP_TYPE_BOOLEAN

        #endregion // Device Experience IDs

        #region Device relations

        private static readonly DEVPROPKEY PKEY_Device_EjectionRelations = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 4);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY PKEY_Device_RemovalRelations = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 5);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY PKEY_Device_PowerRelations = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 6);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY PKEY_Device_BusRelations = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 7);     // DEVPROP_TYPE_STRING_LIST

        /// <summary>Property key for a device's parent's instance ID.</summary>
        internal static readonly DEVPROPKEY PKEY_Device_Parent = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 8);     // DEVPROP_TYPE_STRING

        private static readonly DEVPROPKEY PKEY_Device_Children = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 9);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY PKEY_Device_Siblings = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 10);    // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY PKEY_Device_TransportRelations = CreateDevPropKey(0x4340a6c5, 0x93fa, 0x4706, 0x97, 0x2c, 0x7b, 0x64, 0x80, 0x08, 0xa5, 0xa7, 11);    // DEVPROP_TYPE_STRING_LIST

        #endregion // Device relations

        #region Other Device properties

        private static readonly DEVPROPKEY DEVPKEY_Numa_Proximity_Domain = CreateDevPropKey(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 1);     // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_DHP_Rebalance_Policy = CreateDevPropKey(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 2);     // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_Numa_Node = CreateDevPropKey(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 3);     // DEVPROP_TYPE_UINT32

        /// <summary>Property key for a device's "bus device description". In the case of LTO Flash! hardware, the value should be... get this... LTO Flash!</summary>
        internal static readonly DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc = CreateDevPropKey(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2, 4);     // DEVPROP_TYPE_STRING

        #endregion

        #region Driver Properties

        private static readonly DEVPROPKEY DEVPKEY_Device_DriverDate = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 2);      // DEVPROP_TYPE_FILETIME
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverVersion = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 3);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverDesc = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 4);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverInfPath = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 5);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverInfSection = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 6);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverInfSectionExt = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 7);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_MatchingDeviceId = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 8);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverProvider = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 9);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverPropPageProvider = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 10);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverCoInstallers = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 11);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_Device_ResourcePickerTags = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 12);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_ResourcePickerExceptions = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 13);   // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverRank = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 14);     // DEVPROP_TYPE_UINT32
        private static readonly DEVPROPKEY DEVPKEY_Device_DriverLogoLevel = CreateDevPropKey(0xa8b865dd, 0x2e3d, 0x4094, 0xad, 0x97, 0xe5, 0x93, 0xa7, 0xc, 0x75, 0xd6, 15);     // DEVPROP_TYPE_UINT32

        #endregion // Driver Properties

        #region Device Properties

        private static readonly DEVPROPKEY DEVPKEY_DrvPkg_Model = CreateDevPropKey(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 2);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DrvPkg_VendorWebSite = CreateDevPropKey(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 3);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DrvPkg_DetailedDescription = CreateDevPropKey(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 4);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DrvPkg_DocumentationLink = CreateDevPropKey(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 5);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DrvPkg_Icon = CreateDevPropKey(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 6);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_DrvPkg_BrandingIcon = CreateDevPropKey(0xcf73bb51, 0x3abf, 0x44a2, 0x85, 0xe0, 0x9a, 0x3d, 0xc7, 0xa1, 0x21, 0x32, 7);     // DEVPROP_TYPE_STRING_LIST

        #endregion // Device Properties

        #region Device setup class properties

        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_Name = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 2);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_ClassName = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 3);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_Icon = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 4);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_ClassInstaller = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 5);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_PropPageProvider = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 6);      // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_NoInstallClass = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 7);      // DEVPROP_TYPE_BOOLEAN
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_NoDisplayClass = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 8);      // DEVPROP_TYPE_BOOLEAN
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_SilentInstall = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 9);      // DEVPROP_TYPE_BOOLEAN
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_NoUseClass = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 10);     // DEVPROP_TYPE_BOOLEAN
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_DefaultService = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 11);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_IconPath = CreateDevPropKey(0x259abffc, 0x50a7, 0x47ce, 0xaf, 0x8, 0x68, 0xc9, 0xa7, 0xd7, 0x33, 0x66, 12);     // DEVPROP_TYPE_STRING_LIST
        private static readonly DEVPROPKEY DEVPKEY_DeviceClass_DHPRebalanceOptOut = CreateDevPropKey(0xd14d3ef3, 0x66cf, 0x4ba2, 0x9d, 0x38, 0x0d, 0xdb, 0x37, 0xab, 0x47, 0x01, 2);    // DEVPROP_TYPE_BOOLEAN

        #endregion // Device setup class properties

        #region Device Interface Properties

        private static readonly DEVPROPKEY DEVPKEY_DeviceInterface_FriendlyName = CreateDevPropKey(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 2);     // DEVPROP_TYPE_STRING
        private static readonly DEVPROPKEY DEVPKEY_DeviceInterface_Enabled = CreateDevPropKey(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 3);     // DEVPROP_TYPE_BOOLEAN
        private static readonly DEVPROPKEY DEVPKEY_DeviceInterface_ClassGuid = CreateDevPropKey(0x026e516e, 0xb814, 0x414b, 0x83, 0xcd, 0x85, 0x6d, 0x6f, 0xef, 0x48, 0x22, 4);     // DEVPROP_TYPE_GUID

        #endregion // Device Interface Properties

        /// <summary>
        /// Defines a device property key.
        /// </summary>
        /// <param name="l">Long part of fmtid GUID.</param>
        /// <param name="w1">First short part of fmtid GUID.</param>
        /// <param name="w2">Second short part of fmtid GUID.</param>
        /// <param name="b1">First byte part of fmtid GUID.</param>
        /// <param name="b2">Second byte part of fmtid GUID.</param>
        /// <param name="b3">Third byte part of fmtid GUID.</param>
        /// <param name="b4">Fourth byte part of fmtid GUID.</param>
        /// <param name="b5">Fifth byte part of fmtid GUID.</param>
        /// <param name="b6">Sixth byte part of fmtid GUID.</param>
        /// <param name="b7">Seventh byte part of fmtid GUID.</param>
        /// <param name="b8">Eighth byte part of fmtid GUID.</param>
        /// <param name="pid">Property ID.</param>
        /// <returns>The device property key.</returns>
        /// <remarks>Plays the role of the macro from devpkey.h and similar.</remarks>
        private static DEVPROPKEY CreateDevPropKey(uint l, ushort w1, ushort w2, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, uint pid)
        {
            var key = new DEVPROPKEY();
            key.Fmtid = new Guid(l, w1, w2, b1, b2, b3, b4, b5, b6, b7, b8);
            key.Pid = pid;
            return key;
        }

        #endregion // devpkey.h

        /// <summary>
        /// Retrieves a device instance property.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to a device information set that contains a device instance for which to retrieve a device instance property.</param>
        /// <param name="deviceInfoData">A pointer to the SP_DEVINFO_DATA structure that represents the device instance for which to retrieve a device instance property.</param>
        /// <param name="propertyKey">A pointer to a DEVPROPKEY structure that represents the device property key of the requested device instance property.</param>
        /// <param name="propertyType">A pointer to a DEVPROPTYPE-typed variable that receives the property-data-type identifier of the requested device instance property, where the property-data-type identifier is the bitwise OR between a base-data-type identifier and, if the base-data type is modified, a property-data-type modifier.</param>
        /// <param name="propertyBuffer">A pointer to a buffer that receives the requested device instance property. SetupDiGetDeviceProperty retrieves the requested property only if the buffer is large enough to hold all the property value data. The pointer can be NULL. If the pointer is set to NULL and RequiredSize is supplied, SetupDiGetDeviceProperty returns the size of the property, in bytes, in *RequiredSize.</param>
        /// <param name="propertyBufferSize">The size, in bytes, of the PropertyBuffer buffer. If PropertyBuffer is set to NULL, PropertyBufferSize must be set to zero.</param>
        /// <param name="requiredSize">A pointer to a DWORD-typed variable that receives the size, in bytes, of either the device instance property if the property is retrieved or the required buffer size if the buffer is not large enough. This pointer can be set to NULL.</param>
        /// <param name="flags">This parameter must be set to zero.</param>
        /// <returns>TRUE if it is successful. Otherwise, it returns FALSE, and the logged error can be retrieved by calling GetLastError.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551963(v=vs.85).aspx"/></remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDevicePropertyW(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData, [In] ref DEVPROPKEY propertyKey, [Out] out DEVPROP_TYPE propertyType, byte[] propertyBuffer, uint propertyBufferSize, out uint requiredSize, uint flags);

        /// <summary>
        /// Retrieves a device instance property.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to a device information set that contains a device instance for which to retrieve a device instance property.</param>
        /// <param name="deviceInfoData">A pointer to the SP_DEVINFO_DATA structure that represents the device instance for which to retrieve a device instance property.</param>
        /// <param name="propertyKey">A pointer to a DEVPROPKEY structure that represents the device property key of the requested device instance property.</param>
        /// <param name="propertyType">A pointer to a DEVPROPTYPE-typed variable that receives the property-data-type identifier of the requested device instance property, where the property-data-type identifier is the bitwise OR between a base-data-type identifier and, if the base-data type is modified, a property-data-type modifier.</param>
        /// <param name="propertyBuffer">A pointer to a buffer that receives the requested device instance property. SetupDiGetDeviceProperty retrieves the requested property only if the buffer is large enough to hold all the property value data. The pointer can be NULL. If the pointer is set to NULL and RequiredSize is supplied, SetupDiGetDeviceProperty returns the size of the property, in bytes, in *RequiredSize.</param>
        /// <param name="propertyBufferSize">The size, in bytes, of the PropertyBuffer buffer. If PropertyBuffer is set to NULL, PropertyBufferSize must be set to zero.</param>
        /// <param name="requiredSize">A pointer to a DWORD-typed variable that receives the size, in bytes, of either the device instance property if the property is retrieved or the required buffer size if the buffer is not large enough. This pointer can be set to NULL.</param>
        /// <param name="flags">This parameter must be set to zero.</param>
        /// <returns>TRUE if it is successful. Otherwise, it returns FALSE, and the logged error can be retrieved by calling GetLastError.</returns>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/library/windows/hardware/ff551963(v=vs.85).aspx"/>. This overload is expected to be called with the
        /// value of <paramref name="propertyBuffer"/> set to <c>IntPtr.Zero</c> in order to retrieve the actual required buffer size.</remarks>
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDevicePropertyW(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData, [In] ref DEVPROPKEY propertyKey, [Out] out DEVPROP_TYPE propertyType, IntPtr propertyBuffer, uint propertyBufferSize, out uint requiredSize, uint flags);

        /// <summary>
        /// Gets a device property.
        /// </summary>
        /// <param name="deviceInfoSet">A handle to a device information set that contains a device instance for which to retrieve a device instance property.</param>
        /// <param name="deviceInfoData">A SP_DEVINFO_DATA structure.</param>
        /// <param name="propertyKey">The property to read.</param>
        /// <returns>The value of the property.</returns>
        /// <remarks>Actual parsing of the raw data returned by the p/Invoked method is only implemented for a few data types.</remarks>
        internal static object GetDeviceProperty(IntPtr deviceInfoSet, object deviceInfoData, object propertyKey)
        {
            object propertyValue = null;
            var deviceData = (SP_DEVINFO_DATA)deviceInfoData;
            var key = (DEVPROPKEY)propertyKey;
            DEVPROP_TYPE type;
            uint requiredSize;

            if (!SetupDiGetDevicePropertyW(deviceInfoSet, ref deviceData, ref key, out type, IntPtr.Zero, 0, out requiredSize, 0))
            {
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    var data = new byte[requiredSize];
                    if (SetupDiGetDevicePropertyW(deviceInfoSet, ref deviceData, ref key, out type, data, (uint)data.Length, out requiredSize, 0))
                    {
                        propertyValue = ConvertPropertyValue(data, type);
                    }
                }
            }
            return propertyValue;
        }

        private static object ConvertPropertyValue(byte[] rawData, DEVPROP_TYPE type)
        {
            object value = null;
            var baseType = type & DEVPROP_MASK_TYPE;
            var typeModifiers = type & DEVPROP_MASK_TYPEMOD;
            switch (baseType)
            {
                case DEVPROP_TYPE.DEVPROP_TYPE_EMPTY:
                case DEVPROP_TYPE.DEVPROP_TYPE_NULL:
                    break;
                case DEVPROP_TYPE.DEVPROP_TYPE_SBYTE:
                    if (typeModifiers.HasFlag(DEVPROP_TYPEMOD_ARRAY))
                    {
                        value = rawData.Cast<sbyte>();
                    }
                    else
                    {
                        value = (sbyte)rawData[0];
                    }
                    break;
                case DEVPROP_TYPE.DEVPROP_TYPE_BYTE:
                    if (typeModifiers.HasFlag(DEVPROP_TYPEMOD_ARRAY))
                    {
                        value = rawData.Cast<byte>();
                    }
                    else
                    {
                        value = (byte)rawData[0];
                    }
                    break;
                case DEVPROP_TYPE.DEVPROP_TYPE_BOOLEAN:
                    if (typeModifiers.HasFlag(DEVPROP_TYPEMOD_ARRAY))
                    {
                        value = rawData.Select(b => b != 0);
                    }
                    else
                    {
                        value = rawData[0] != 0;
                    }
                    break;
                case DEVPROP_TYPE.DEVPROP_TYPE_STRING:
                case DEVPROP_TYPE.DEVPROP_TYPE_STRING_INDIRECT:
                    // NOTE: Strings and lists of strings always have a NULL-terminator -- two bytes.
                    var s = Encoding.Unicode.GetString(rawData, 0, rawData.Length);
                    var dataAsString = Encoding.Unicode.GetString(rawData, 0, rawData.Length - 2);
                    if (typeModifiers.HasFlag(DEVPROP_TYPEMOD_LIST))
                    {
                        var stringValues = dataAsString.Split('\0');
                        value = stringValues;
                    }
                    else
                    {
                        value = dataAsString;
                    }
                    break;
                case DEVPROP_TYPE.DEVPROP_TYPE_DEVPROPKEY:
                    System.Diagnostics.Debug.WriteLine("Conversion for DEVPROP_TYPE not implemented: " + baseType);
                    break;
                case DEVPROP_TYPE.DEVPROP_TYPE_INT16:
                case DEVPROP_TYPE.DEVPROP_TYPE_UINT16:
                case DEVPROP_TYPE.DEVPROP_TYPE_INT32:
                case DEVPROP_TYPE.DEVPROP_TYPE_UINT32:
                case DEVPROP_TYPE.DEVPROP_TYPE_INT64:
                case DEVPROP_TYPE.DEVPROP_TYPE_UINT64:
                case DEVPROP_TYPE.DEVPROP_TYPE_FLOAT:
                case DEVPROP_TYPE.DEVPROP_TYPE_DOUBLE:
                case DEVPROP_TYPE.DEVPROP_TYPE_DECIMAL:
                case DEVPROP_TYPE.DEVPROP_TYPE_GUID:
                case DEVPROP_TYPE.DEVPROP_TYPE_CURRENCY:
                case DEVPROP_TYPE.DEVPROP_TYPE_DATE:
                case DEVPROP_TYPE.DEVPROP_TYPE_FILETIME:
                case DEVPROP_TYPE.DEVPROP_TYPE_SECURITY_DESCRIPTOR:
                case DEVPROP_TYPE.DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING:
                case DEVPROP_TYPE.DEVPROP_TYPE_DEVPROPTYPE:
                case DEVPROP_TYPE.DEVPROP_TYPE_BINARY:
                case DEVPROP_TYPE.DEVPROP_TYPE_ERROR:
                case DEVPROP_TYPE.DEVPROP_TYPE_NTSTATUS:
                default:
                    System.Diagnostics.Debug.WriteLine("Conversion for DEVPROP_TYPE not implemented: " + baseType);
                    value = rawData;
                    break;
            }
            return value;
        }

#if EXPERIMENT_GET_ALL_PROPERTIES

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDevicePropertyKeys(IntPtr deviceInfoSet, [In] ref SP_DEVINFO_DATA deviceInfoData, ref DEVPROPKEY propertyKeyArray, [In] uint propertyKeyCount, [Out] out uint requiredPropertyKeyCount, uint flags);

        internal static IEnumerable<object> GetDevicePropertyKeys(IntPtr deviceInfoSet, object deviceInfoData)
        {
            var devInfo = (SP_DEVINFO_DATA)deviceInfoData;
            var keys = new DEVPROPKEY[1];
            uint requiredPropertyKeyCount;

            if (!NativeMethods.SetupDiGetDevicePropertyKeys(deviceInfoSet, ref devInfo, ref keys[0], (uint)keys.Length, out requiredPropertyKeyCount, 0))
            {
                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    keys = new DEVPROPKEY[requiredPropertyKeyCount];
                    if (NativeMethods.SetupDiGetDevicePropertyKeys(deviceInfoSet, ref devInfo, ref keys[0], (uint)keys.Length, out requiredPropertyKeyCount, 0))
                    {
                        return keys.Cast<object>(); // .Select(k => (object)k);
                    }
                    else
                    {
                        throw new System.ComponentModel.Win32Exception();
                    }
                }
                else
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }
            return null;
        }

#endif // EXPERIMENT_GET_ALL_PROPERTIES

        /// <summary>
        /// Represents a device property key for a device property in the unified device property model. (from devpropdef.h)
        /// </summary>
        /// <remarks><see cref="https://msdn.microsoft.com/en-us/windows/ff543544(v=vs.90).aspx"/></remarks>
        [StructLayout(LayoutKind.Sequential)]
        internal struct DEVPROPKEY
        {
            /// <summary>
            /// A DEVPROPGUID-typed value that specifies a property category.
            /// </summary>
            public Guid Fmtid;

            /// <summary>
            /// A DEVPROPID-typed value that uniquely identifies the property within the property category. For internal system reasons, a property identifier must be greater than or equal to two.
            /// </summary>
            public uint Pid;

            /// <inheritdoc />
            public override string ToString()
            {
                return Fmtid.ToString() + ", " + Pid.ToString();
            }
        }

        /// <summary>
        /// Property data types. (from devpropdef.h)
        /// </summary>
        private enum DEVPROP_TYPE : uint
        {
            /// <summary>Nothing -- no property data.</summary>
            DEVPROP_TYPE_EMPTY = 0x00000000,

            /// <summary>Null property data.</summary>
            DEVPROP_TYPE_NULL = 0x00000001,

            /// <summary>8-bit signed int (SBYTE).</summary>
            DEVPROP_TYPE_SBYTE = 0x00000002,

            /// <summary>8-bit unsigned int (BYTE).</summary>
            DEVPROP_TYPE_BYTE = 0x00000003,

            /// <summary>16-bit signed int (SHORT).</summary>
            DEVPROP_TYPE_INT16 = 0x00000004,

            /// <summary>16-bit unsigned int (USHORT).</summary>
            DEVPROP_TYPE_UINT16 = 0x00000005,

            /// <summary>32-bit signed int (LONG).</summary>
            DEVPROP_TYPE_INT32 = 0x00000006,

            /// <summary>32-bit unsigned int (ULONG).</summary>
            DEVPROP_TYPE_UINT32 = 0x00000007,

            /// <summary>64-bit signed int (LONG64).</summary>
            DEVPROP_TYPE_INT64 = 0x00000008,

            /// <summary>64-bit unsigned int (ULONG64).</summary>
            DEVPROP_TYPE_UINT64 = 0x00000009,

            /// <summary>32-bit floating-point (FLOAT).</summary>
            DEVPROP_TYPE_FLOAT = 0x0000000A,

            /// <summary>64-bit floating-point (DOUBLE).</summary>
            DEVPROP_TYPE_DOUBLE = 0x0000000B,

            /// <summary>128-bit data (DECIMAL).</summary>
            DEVPROP_TYPE_DECIMAL = 0x0000000C,

            /// <summary>128-bit unique identifier (GUID).</summary>
            DEVPROP_TYPE_GUID = 0x0000000D,

            /// <summary>64-bit signed int currency value (CURRENCY).</summary>
            DEVPROP_TYPE_CURRENCY = 0x0000000E,

            /// <summary>Date (DATE).</summary>
            DEVPROP_TYPE_DATE = 0x0000000F,

            /// <summary>File time (FILETIME)</summary>
            DEVPROP_TYPE_FILETIME = 0x00000010,

            /// <summary>8-bit boolean (DEVPROP_BOOLEAN).</summary>
            DEVPROP_TYPE_BOOLEAN = 0x00000011,  //

            /// <summary><c>null</c>-terminated string.</summary>
            DEVPROP_TYPE_STRING = 0x00000012,

            /// <summary>Multi-sz string list.</summary>
            DEVPROP_TYPE_STRING_LIST = DEVPROP_TYPE_STRING | DEVPROP_TYPEMOD_LIST,

            /// <summary>Self-relative binary SECURITY_DESCRIPTOR.</summary>
            DEVPROP_TYPE_SECURITY_DESCRIPTOR = 0x00000013,

            /// <summary>Security descriptor string (SDDL format).</summary>
            DEVPROP_TYPE_SECURITY_DESCRIPTOR_STRING = 0x00000014,

            /// <summary>Device property key (DEVPROPKEY).</summary>
            DEVPROP_TYPE_DEVPROPKEY = 0x00000015,

            /// <summary>Device property type (DEVPROPTYPE).</summary>
            DEVPROP_TYPE_DEVPROPTYPE = 0x00000016,

            /// <summary>Custom binary data.</summary>
            DEVPROP_TYPE_BINARY = DEVPROP_TYPE_BYTE | DEVPROP_TYPEMOD_ARRAY,

            /// <summary>32-bit Win32 system error code.</summary>
            DEVPROP_TYPE_ERROR = 0x00000017,

            /// <summary>32-bit NTSTATUS code.</summary>
            DEVPROP_TYPE_NTSTATUS = 0x00000018,

            /// <summary>string resource (@[path\]&lt;dllname&gt;,-&lt;strId&gt;).</summary>
            DEVPROP_TYPE_STRING_INDIRECT = 0x00000019,
        }
    }
}
