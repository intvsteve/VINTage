// <copyright file="INotifyPropertyChangedHelpers.Mac.cs" company="INTV Funhouse">
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

namespace INTV.Core.ComponentModel
{
    /// <summary>
    /// Mac-specific part of INotifyPropertyChangedHelpers.
    /// </summary>
    public static partial class INotifyPropertyChangedHelpers
    {
        /// <summary>
        /// Force KVO update.
        /// </summary>
        /// <param name="sender">NSObject having a property identified by <paramref name="key"/>.</param>
        /// <param name="key">The key (name) for a property exposed via KVO on the <paramref name="sender"/>.</param>
        public static void RaiseChangeValueForKey(this NSObject sender, string key)
        {
            PreValueUpdate(sender, key);
            PostValueUpdate(sender, key);
        }

        static partial void PreValueUpdate(object sender, string propertyName)
        {
            var nsObject = sender as NSObject;
            if (nsObject != null)
            {
                nsObject.WillChangeValue(propertyName);
            }
        }

        static partial void PostValueUpdate(object sender, string propertyName)
        {
            var nsObject = sender as NSObject;
            if (nsObject != null)
            {
                nsObject.DidChangeValue(propertyName);
            }
        }
    }
}
