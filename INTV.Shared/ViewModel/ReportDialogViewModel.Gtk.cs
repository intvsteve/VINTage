// <copyright file="ReportDialogViewModel.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2021 All Rights Reserved
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

using System.IO;
using System.Linq;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class ReportDialogViewModel
    {
        private static void PlatformCopyToClipboard(object parameter)
        {
            try
            {
                var atom = Gdk.Atom.Intern("CLIPBOARD", true);
                Gtk.Clipboard.Get(atom).Text = ((ReportDialogViewModel)parameter).ReportText;
            }
            catch (System.Exception e)
            {
                // This happened in VirtualBox running on Mac...
                var message = string.Format(Resources.Strings.ReportDialog_CopyToClipboard_Failed_MessageFormat, e.Message);
                PlatformPanic(message, Resources.Strings.ReportDialog_CopyToClipboard_Failed_Title);
            }
        }
        
        // Directly use OS-specific dialog. The fancier OSMessageBox may cause a crash if we report a crash while we're reporting a crash. Because of its fancy fanciness.
        private static void PlatformPanic(string message, string title)
        {
            using (var dialog = new Gtk.MessageDialog(null, Gtk.DialogFlags.Modal, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "{0}", message))
            {
                dialog.Title = title;
                dialog.Run();
            }
        }

        private static string GetSystemInfoFromDmiFile(string fileName, string defaultValue)
        {
            var info = defaultValue;
            try
            {
                if (File.Exists(fileName))
                {
                    var allData = File.ReadAllLines(fileName);
                    var data = string.Join(" ", allData.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l))).Trim();
                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        info = data;
                    }
                }
            }
            catch (System.Exception)
            {
                // ignore failures
            }
            return info;
        }

        private static string GetProcessorModel()
        {
            // Assuming standard format... hope this works!
            var processor = "Unknown";
            const string CpuInfo = "/proc/cpuinfo";
            if (File.Exists(CpuInfo))
            {
                try
                {
                    var allCpuInfo = File.ReadAllLines(CpuInfo);
                    var cpuModelNameLine = allCpuInfo.FirstOrDefault(l => l.Trim().StartsWith("model name", System.StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(cpuModelNameLine))
                    {
                        var cpuModelNameParts = cpuModelNameLine.Split(':');
                        if (cpuModelNameParts.Length > 1)
                        {
                            var cpuModelName = cpuModelNameParts[1].Trim();
                            if (!string.IsNullOrEmpty(cpuModelName))
                            {
                                processor = cpuModelName;
                            }
                        }
                    }
                }
                catch (System.Exception)
                {
                    // ignore failures
                }
            }
            return processor;
        }

        private static double GetMemoryInGigabytes()
        {
            var memory = 0.0;
            try
            {
                var memoryPerformanceCounter = new System.Diagnostics.PerformanceCounter("Mono Memory", "Total Physical Memory");
                var memoryInBytes = memoryPerformanceCounter.RawValue;
                if (memoryInBytes > 0)
                {
                    const double BytesPerGigabyte = 1024 * 1024 * 1024;
                    memory = memoryInBytes / BytesPerGigabyte;
                }
            }
            catch (System.Exception)
            {
                // ignore errors
            }
            return memory;
        }

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        /// <param name="message">The string builder used to generate the messsage.</param>
        static partial void AppendSystemInformation(System.Text.StringBuilder message)
        {
            var systemManufacturer = GetSystemInfoFromDmiFile("/sys/devices/virtual/dmi/id/sys_vendor", "Unknown");
            var systemModel = GetSystemInfoFromDmiFile("/sys/devices/virtual/dmi/id/product_name", "Unknown");
            var cpuKind = GetProcessorModel();
            var memory = GetMemoryInGigabytes();
            message.AppendFormat(
                System.Globalization.CultureInfo.InvariantCulture,
                "Computer: Manufacturer: {0} Model: {1}; Processor: {2}; RAM: {3:N1} GB",
                systemManufacturer,
                systemModel,
                cpuKind,
                memory);
            message.AppendLine();
        }

        private void Initialize()
        {
        }
    }
}
