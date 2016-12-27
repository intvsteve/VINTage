// <copyright file="OSMessageBox.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation for OSMessageBox.
    /// </summary>
    public static partial class OSMessageBox
    {
        private static OSMessageBoxResult PlatformShowCore(string message, string title, System.Exception exception, OSMessageBoxButton buttons, Dictionary<OSMessageBoxButton, string> customButtonLabels, OSMessageBoxIcon icon, OSMessageBoxResult defaultResult, System.Action<OSMessageBoxResult> onComplete)
        {
            var result = OSMessageBoxResult.None;
            if (onComplete == null)
            {
                INTV.Shared.Utility.OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                    {
                        using (var messageBox = new NSAlert())
                        {
                            messageBox.MessageText = title;
                            messageBox.InformativeText = message;
                            messageBox.AlertStyle = (NSAlertStyle)icon;
                            messageBox.AddButton(GetCustomTextForButton(OSMessageBoxButton.OK, customButtonLabels, buttons));
                            var defaultButton = messageBox.Buttons[0];
                            NSButton buttonTwo = null;
                            NSButton buttonThree = null;
                            switch (buttons)
                            {
                                case OSMessageBoxButton.OK:
                                    break;
                                case OSMessageBoxButton.YesNo:
                                    buttonTwo = messageBox.AddButton(GetCustomTextForButton(OSMessageBoxButton.YesNo, customButtonLabels, buttons));
                                    break;
                                case OSMessageBoxButton.YesNoCancel:
                                    buttonTwo = messageBox.AddButton(GetCustomTextForButton(OSMessageBoxButton.YesNo, customButtonLabels, buttons));
                                    buttonThree = messageBox.AddButton(GetCustomTextForButton(OSMessageBoxButton.YesNoCancel, customButtonLabels, buttons));
                                    break;
                            }
                            if (customButtonLabels != null)
                            {
                                var buttonText = string.Empty;
                                System.Diagnostics.Debug.WriteLineIf((defaultButton != null) && customButtonLabels.TryGetValue(OSMessageBoxButton.OK, out buttonText) && (buttonText != GetCustomTextForButton(OSMessageBoxButton.OK, customButtonLabels, buttons)), "Custom button1 text not used.");
                                System.Diagnostics.Debug.WriteLineIf((buttonTwo != null) && customButtonLabels.TryGetValue(OSMessageBoxButton.YesNo, out buttonText) && (buttonText != GetCustomTextForButton(OSMessageBoxButton.YesNo, customButtonLabels, buttons)), "Custom button2 text not used.");
                                System.Diagnostics.Debug.WriteLineIf((buttonThree != null) && customButtonLabels.TryGetValue(OSMessageBoxButton.YesNoCancel, out buttonText) && (buttonText != GetCustomTextForButton(OSMessageBoxButton.YesNoCancel, customButtonLabels, buttons)), "Custom button3 text not used.");
                            }
                            result = (OSMessageBoxResult)messageBox.RunModal();
                        }
                    });
            }
            else
            {
                INTV.Shared.Utility.SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() =>
                {
                    result = ShowCore(message, title, exception, null, buttons, customButtonLabels, icon, defaultResult, null);
                    onComplete(result);
                });
            }
            return result;
        }
    }
}
