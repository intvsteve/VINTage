// <copyright file="PropertyChangedNotifierHelpers.cs" company="INTV Funhouse">
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
using INTV.Core.ComponentModel;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Extension methods for PropertyChangeNotifer that use clever Linq / Reflection tricks to ease implementation.
    /// </summary>
    public static class PropertyChangedNotifierHelpers
    {
        /// <summary>
        /// Extension method for easing the use of the UpdateProperty method of the PropertyChangedNotifier type.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="notifier">The object upon which to update a property.</param>
        /// <param name="name">This clever bit uses a lambda and reflection so you can pass the property from which the name is harvested, rather than hard-coding strings.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property. Will equal newValue upon completion of the function.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        public static bool UpdateProperty<T>(this PropertyChangedNotifier notifier, System.Linq.Expressions.Expression<Func<T>> name, T newValue, ref T currentValue)
        {
            return UpdateProperty(notifier, name, newValue, ref currentValue, null);
        }

        /// <summary>
        /// Extension method for easing the use of the UpdateProperty method of the PropertyChangedNotifier type.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="notifier">The object upon which to update a property.</param>
        /// <param name="name">This clever bit uses a lambda and reflection so you can pass the property from which the name is harvested, rather than hard-coding strings.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property. Will equal newValue upon completion of the function.</param>
        /// <param name="customAction">Custom action to execute when raising the property changed event.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        public static bool UpdateProperty<T>(this PropertyChangedNotifier notifier, System.Linq.Expressions.Expression<Func<T>> name, T newValue, ref T currentValue, Action<string, T> customAction)
        {
            var propertyName = ((System.Linq.Expressions.MemberExpression)name.Body).Member.Name;
            return notifier.AssignAndUpdateProperty(propertyName, newValue, ref currentValue, customAction);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="notifier">The object upon which to update a property.</param>
        /// <param name="name">This clever bit uses a lambda and reflection so you can pass the property from which the name is harvested, rather than hard-coding strings.</param>
        public static void RaisePropertyChanged<T>(this PropertyChangedNotifier notifier, System.Linq.Expressions.Expression<Func<T>> name)
        {
            var propertyName = ((System.Linq.Expressions.MemberExpression)name.Body).Member.Name;
            notifier.RaisePropertyChanged<T>(propertyName);
        }
    }
}
