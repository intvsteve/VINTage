// <copyright file="IFakeDependencyObjectHelpers.Gtk.cs" company="INTV Funhouse">
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace INTV.Shared.View
{
    /// <summary>
    /// Extension methods for sort of kind of DependencyObject-like properties that can also assist
    /// with implementing IFakeDependencyObject.
    /// </summary>
    public static partial class IFakeDependencyObjectHelpers
    {
        private static bool GetInheritedValue(this Gtk.Widget visual, string property, out object propertyValue)
        {
            bool gotValue = false;
            propertyValue = null;
            Gtk.Widget window = visual as Gtk.Window;
            Gtk.Widget widget = visual;
            var root = window;
            if (root == null)
            {
                if (widget != null)
                {
                    root = widget.Toplevel;
                }
                if (root == null)
                {
                    root = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
                }
            }
            while ((visual != null) && !gotValue)
            {
                gotValue = visual.TryGetAttachedValue(property, out propertyValue);
                if (!gotValue)
                {
                    if (widget != null)
                    {
                        window = widget.Toplevel as Gtk.Window;
                        widget = widget.Parent;
                        if (widget == null)
                        {
                            visual = window;
                        }
                        if (visual == null)
                        {
                            visual = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
                        }
                    }
                    else if (window != null)
                    {
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
                        window = null;
                    }
                    else
                    {
                        visual = null;
                    }
                }
            }
            return gotValue;
        }

        private static bool TryGetAttachedValue(this object o, string property, out object value)
        {
            var data = ((GLib.Object)o).Data;
            var gotValue = data.ContainsKey(property);
            value = null;
            if (gotValue)
            {
                value = data[property];
            }
            return gotValue;
        }

        private static void SetAttachedPropertyValue(this object o, string property, object value)
        {
            var data = ((GLib.Object)o).Data;
            data[property] = value;
        }
    }
}
