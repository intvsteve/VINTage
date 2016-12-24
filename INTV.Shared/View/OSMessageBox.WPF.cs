// <copyright file="OSMessageBox.WPF.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;

namespace INTV.Shared.View
{
    /// <summary>
    /// Windows-specific implementation for OSMessageBox.
    /// </summary>
    public static partial class OSMessageBox
    {
        private static OSMessageBoxResult PlatformShowCore(string message, string title, Exception exception, OSMessageBoxButton buttons, Dictionary<OSMessageBoxButton, string> customButtonLabels, OSMessageBoxIcon icon, OSMessageBoxResult defaultResult, Action<OSMessageBoxResult> onComplete)
        {
            var result = OSMessageBoxResult.None;
            var appInstance = INTV.Shared.Utility.SingleInstanceApplication.Instance;
            INTV.Shared.Utility.OSDispatcher.Current.InvokeOnMainDispatcher(() =>
                {
                    var owner = (appInstance == null) ? null : appInstance.MainWindow;
                    result = OSMessageBoxResult.None;
                    if (onComplete == null)
                    {
                        result = (OSMessageBoxResult)System.Windows.MessageBox.Show(owner, message, title, (System.Windows.MessageBoxButton)buttons, (System.Windows.MessageBoxImage)icon, (System.Windows.MessageBoxResult)defaultResult);
                    }
                    else
                    {
                        INTV.Shared.Utility.SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() =>
                        {
                            result = ShowCore(message, title, exception, null, buttons, customButtonLabels, icon, defaultResult, null);
                            onComplete(result);
                        });
                    }
                });
            return result;
        }
    }
}
