// <copyright file="OSMessageBox.Gtk.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
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
                        var nativeButtons = Gtk.ButtonsType.Ok;
                        switch (buttons)
                        {
                            case OSMessageBoxButton.OK:
                            case OSMessageBoxButton.YesNo:
                                nativeButtons = (Gtk.ButtonsType)buttons;
                                break;
                            case OSMessageBoxButton.YesNoCancel:
                                nativeButtons = Gtk.ButtonsType.None; // we'll add buttons below
                                break;
                        }

                        var parent = Gtk.Window.ListToplevels().FirstOrDefault(w => w.IsActive || w.IsFocus);
                        if (parent == null)
                        {
                            parent = INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow;
                        }

                        using (var messageBox = new Gtk.MessageDialog(parent, Gtk.DialogFlags.Modal, (Gtk.MessageType)icon, nativeButtons, "{0}", message))
                        {
                            messageBox.Title = title;
                            messageBox.MessageType = (Gtk.MessageType)icon;
                            messageBox.DefaultResponse = (Gtk.ResponseType)defaultResult;
                            switch (buttons)
                            {
                                case OSMessageBoxButton.OK:
                                case OSMessageBoxButton.YesNo:
                                    break;
                                case OSMessageBoxButton.YesNoCancel:
                                    messageBox.AddButton(Resources.Strings.YesButton_Text, Gtk.ResponseType.Yes);
                                    messageBox.AddButton(Resources.Strings.NoButton_Text, Gtk.ResponseType.No);
                                    messageBox.AddButton(Resources.Strings.CancelButtonText, Gtk.ResponseType.Cancel);
                                    break;
                            }
                            result = (OSMessageBoxResult)messageBox.Run();
                            VisualHelpers.Close(messageBox);
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
