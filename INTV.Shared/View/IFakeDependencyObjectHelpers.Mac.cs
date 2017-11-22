// <copyright file="IFakeDependencyObjectHelpers.Mac.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using INTV.Core.Utility;

#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class IFakeDependencyObjectHelpers
    {
        private static WeakKeyDictionary<object, Dictionary<string, object>> AttachedProperties
        {
            get
            {
                if (_masterAttachedPropertiesList == null)
                {
                    _masterAttachedPropertiesList = new WeakKeyDictionary<object, Dictionary<string, object>>();
                }
                return _masterAttachedPropertiesList;
            }
        }
        private static WeakKeyDictionary<object, Dictionary<string, object>> _masterAttachedPropertiesList;

        private static bool GetInheritedValue(this NSResponder visual, string property, out object propertyValue)
        {
            bool gotValue = false;
            propertyValue = null;
            var window = visual as NSWindow;
            var view = visual as NSView;
            NSResponder root = window;
            if (root == null)
            {
                if (view != null)
                {
                    root = view.Window;
                }
                if (root == null)
                {
                    root = INTV.Shared.Utility.SingleInstanceApplication.Current;
                }
            }
            while ((visual != null) && !gotValue)
            {
                var properties = AttachedProperties.GetEntry(visual);
                if (properties != null)
                {
                    gotValue = properties.TryGetValue(property, out propertyValue);
                }
                if (!gotValue)
                {
                    if (view != null)
                    {
                        window = view.Window;
                        visual = view.Superview;
                        view = visual as NSView;
                        if (view == null)
                        {
                            visual = window;
                        }
                        if (visual == null)
                        {
                            visual = INTV.Shared.Utility.SingleInstanceApplication.Current;
                        }
                    }
                    else if (window != null)
                    {
                        visual = INTV.Shared.Utility.SingleInstanceApplication.Current;
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

        private static IDictionary<string, object> GetAttachedProperties(this object o)
        {
            System.Diagnostics.Debug.Assert(o.GetType().IsClass);
            var properties = AttachedProperties.GetEntry(o);
            if (properties == null)
            {
                properties = new Dictionary<string, object>();
                AttachedProperties[o] = properties;
            }
            return properties;
        }

        private static bool TryGetAttachedValue(this object o, string property, out object value)
        {
            var gotValue = false;
            value = null;
            try
            {
                var properties = o.GetAttachedProperties();
                if (properties != null)
                {
                    gotValue = properties.TryGetValue(property, out value);
                }
            }
            catch (Exception)
            {
            }
            return gotValue;
        }

        private static void SetAttachedPropertyValue(this object o, string property, object value)
        {
            var properties = o.GetAttachedProperties();
            properties[property] = value;
        }
    }
}
