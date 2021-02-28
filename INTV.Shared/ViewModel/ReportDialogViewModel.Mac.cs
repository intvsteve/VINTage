// <copyright file="ReportDialogViewModel.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

using INTV.Shared.Interop;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class ReportDialogViewModel
    {
        private static void PlatformCopyToClipboard(object parameter)
        {
            try
            {
                var pasteboard = NSPasteboard.GeneralPasteboard;
                pasteboard.ClearContents();
                pasteboard.DeclareTypes(new[] { (string)NSPasteboard.NSStringType }, null);
                pasteboard.SetStringForType(((ReportDialogViewModel)parameter).ReportText, NSPasteboard.NSStringType);
            }
            catch (System.Exception e)
            {
                var message = string.Format(Resources.Strings.ReportDialog_CopyToClipboard_Failed_MessageFormat, e.Message);
                PlatformPanic(message, Resources.Strings.ReportDialog_CopyToClipboard_Failed_Title);
            }
        }

        private static void PlatformPanic(string message, string title)
        {
            var messageBox = new NSAlert();
            messageBox.MessageText = title;
            messageBox.InformativeText = message;
            messageBox.AlertStyle = NSAlertStyle.Critical;
            messageBox.RunModal();
        }

        /// <summary>
        /// Mac-specific implementation.
        /// </summary>
        /// <param name="message">The string builder used to generate the message.</param>
        static partial void AppendSystemInformation(System.Text.StringBuilder message)
        {
            var macModelIdentifier = NativeMethods.GetSystemProperty(NativeMethods.SystemModel);
            var cpuKind = NativeMethods.GetSystemProperty(NativeMethods.SystemMachine);
            var memory = NativeMethods.GetSystemProperty(NativeMethods.SystemRamSize);
            message.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Computer: Model Id: {0}; Processor: {1}; RAM: {2}", macModelIdentifier, cpuKind, memory);
            message.AppendLine();
        }

        private void Initialize()
        {
        }
    }
}
