// <copyright file="FTDIUtilities.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

////#define DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace INTV.LtoFlash.Utility
{
    /// <summary>
    /// Windows-specific implementation to get FTDI Driver version.
    /// </summary>
    public static partial class FTDIUtilities
    {
#if DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
        // The following are from: https://msdn.microsoft.com/en-us/library/windows/hardware/ff553426(v=vs.85).aspx
        private static readonly string PortsDeviceClassName = "PORTS";
        private static readonly string PortsClassGuid = "{4d36e978-e325-11ce-bfc1-08002be10318}";
#endif // DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE

        /// <summary>
        /// Gets the FTDI driver version as a string.
        /// </summary>
        public static string DriverVersionString
        {
            get
            {
#if DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
                var driverVersions = GetDriverVersions();
                var version = driverVersions.Count() > 1 ? driverVersions.Max() : driverVersions.FirstOrDefault();
                string versionString = version == null ? Resources.Strings.UnknownFTDIDriverVersion : version.ToString();
#else
                var versionStrings = GetDriverVersionStrings();
                var versionString = versionStrings.Count() > 1 ? versionStrings.Max() : versionStrings.FirstOrDefault();
                versionString = versionString ?? Resources.Strings.FtdiDriverVersionUnknown;
#endif // DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
                return versionString;
            }
        }

        /// <summary>
        /// Gets the FTDI driver version.
        /// </summary>
        public static Version DriverVersion
        {
            get
            {
                var version = GetDriverVersions().Max();
                if (version == null)
                {
                    version = new Version();
                }
                return version;
            }
        }

        private static IEnumerable<string> GetDriverVersionStrings()
        {
            var driverVersions = Enumerable.Empty<string>();
            try
            {
                // This code only works when device is attached. It will specifically only get FTDI PnP VCP drivers.
#if DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver WHERE Manufacturer = 'FTDI'");
                var drivers = searcher.Get().Cast<ManagementObject>().Where(d => (string.Compare(PortsDeviceClassName, (string)d["DeviceClass"], true) == 0) && (string.Compare(PortsClassGuid, (string)d["ClassGuid"], true) == 0));
                driverVersions = drivers.Select(d => (string)d["DriverVersion"]);
#else
                driverVersions = GetDriverFileVersions().Select(v => v.ProductVersion);
#endif // DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
            }
            catch (ManagementException e)
            {
                var errDlg = INTV.Shared.View.ReportDialog.Create(Resources.Strings.FtdiDriverIdentification_Error_Title, Resources.Strings.FtdiDriverIdentification_ErrorMessage);
                errDlg.Exception = e;
                errDlg.Show();
            }
            return driverVersions;
        }

        private static IEnumerable<Version> GetDriverVersions()
        {
#if DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
            var driverVersions = GetDriverVersionStrings().Select(s => new Version(s));
#else
            var driverVersions = GetDriverFileVersions().Select(v => new Version(v.ProductMajorPart, v.ProductMinorPart, v.ProductBuildPart, v.ProductPrivatePart));
#endif // DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
            return driverVersions;
        }

#if !DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE

        private static IEnumerable<System.Diagnostics.FileVersionInfo> GetDriverFileVersions()
        {
            var driverFileVersions = new List<System.Diagnostics.FileVersionInfo>();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SystemDriver WHERE Name LIKE 'FTDIBUS'");
            foreach (var driver in searcher.Get().Cast<ManagementObject>())
            {
                var driverPath = driver["PathName"] as string;
                if (!string.IsNullOrEmpty(driverPath) && System.IO.File.Exists(driverPath))
                {
                    var driverFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(driverPath);
                    driverFileVersions.Add(driverFileVersion);
                }
            }

            if (!driverFileVersions.Any())
            {
                try
                {
                    // check driver cache
                    var driverStoreDir = "DriverStore\\FileRepository";
                    if (System.Environment.OSVersion.Version.Major == 5)
                    {
                        driverStoreDir = "DRVSTORE";
                    }
                    var driverStorePath = System.IO.Path.Combine(System.Environment.SystemDirectory, driverStoreDir);
                    if (System.IO.Directory.Exists(driverStorePath))
                    {
                        var ftdiDir = System.IO.Directory.EnumerateDirectories(driverStorePath, "ftdibus*").FirstOrDefault();
                        if (!string.IsNullOrEmpty(ftdiDir))
                        {
                            var driverPath = System.IO.Directory.EnumerateFiles(ftdiDir, "ftdibus.sys", System.IO.SearchOption.AllDirectories).FirstOrDefault();
                            if (System.IO.File.Exists(driverPath))
                            {
                                var driverFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(driverPath);
                                driverFileVersions.Add(driverFileVersion);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                }
            }
            return driverFileVersions;
        }

#endif // !DETECT_DRIVER_VERSION_ONLY_WHEN_ACTIVE
    }
}
