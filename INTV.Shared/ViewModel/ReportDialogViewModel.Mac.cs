// <copyright file="ReportDialogViewModel.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

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

        private void Initialize()
        {
        }
    }
}
