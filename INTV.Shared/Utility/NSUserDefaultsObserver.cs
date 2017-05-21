// <copyright file="NSUserDefaultsObserver.cs" company="INTV Funhouse">
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
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using System.Collections.Generic;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Implements an observer for changes to NSUserDefaults.
    /// </summary>
    public class NSUserDefaultsObserver : NSObject
    {
        private static readonly NSUserDefaultsObserver Instance = new NSUserDefaultsObserver();

        private Dictionary<string, System.Action> _notifiers = new Dictionary<string, System.Action>();

        private NSUserDefaultsObserver()
        {
        }

        /// <summary>
        /// Add a notification for a given preference.
        /// </summary>
        /// <param name="preference">The preference to observe.</param>
        /// <param name="notifier">The notification function to call when the preference changes.</param>
        public static void AddPreferenceChangedNotification(string preference, System.Action<string> notifier)
        {
            AddPreferenceChangedNotification(preference, () => notifier(preference));
        }

        /// <summary>
        /// Add a notification for a given preference.
        /// </summary>
        /// <param name="preference">The preference to observe.</param>
        /// <param name="notifier">The notification function to call when the preference changes.</param>
        public static void AddPreferenceChangedNotification(string preference, System.Action notifier)
        {
            var settings = NSUserDefaults.StandardUserDefaults;
            Instance._notifiers[preference] = notifier;
            settings.AddObserver(Instance, (NSString)preference, NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, Instance.Handle);
        }

        /// <inheritdoc />
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, System.IntPtr context)
        {
            if (context == Handle)
            {
                System.Action notifier = null;
                if (_notifiers.TryGetValue(keyPath, out notifier))
                {
                    if (notifier != null)
                    {
                        notifier();
                    }
                }
            }
        }
    }
}
