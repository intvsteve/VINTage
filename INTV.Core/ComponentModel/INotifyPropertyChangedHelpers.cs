// <copyright file="INotifyPropertyChangedHelpers.cs" company="INTV Funhouse">
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
using System.ComponentModel;

namespace INTV.Core.ComponentModel
{
    /// <summary>
    /// Provides a rich mix-in implementation for methods that are useful when implementing INotifyPropertyChanged.
    /// These methods specifically help with updating property values and raising events.
    /// </summary>
    public static partial class INotifyPropertyChangedHelpers
    {
        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        public static bool UpdateProperty<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name, T newValue, T currentValue)
        {
            return UpdateProperty(sender, handler, name, newValue, currentValue, null);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="customAction">Custom action to execute after raising the property changed event.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes after the PropertyChange event is raised.</remarks>
        public static bool UpdateProperty<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name, T newValue, T currentValue, Action<string, T> customAction)
        {
            bool valueChanged = SafeDidValueChangeCompare(newValue, currentValue);
            if (valueChanged)
            {
                RaisePropertyChanged(sender, handler, name, customAction, newValue);
            }
            return valueChanged;
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="customAction">Custom action to execute prior to raising the property changed event.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes before the PropertyChange event is raised.</remarks>
        public static bool UpdateProperty<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name, Action<string, T> customAction, T newValue, T currentValue)
        {
            bool valueChanged = SafeDidValueChangeCompare(newValue, currentValue);
            if (valueChanged)
            {
                if (customAction != null)
                {
                    customAction(name, newValue);
                }
                RaisePropertyChanged(sender, handler, name, null, newValue);
            }
            return valueChanged;
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property. Will equal newValue upon completion of the function.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        public static bool AssignAndUpdateProperty<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name, T newValue, ref T currentValue)
        {
            return AssignAndUpdateProperty(sender, handler, name, newValue, ref currentValue, null);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property. Will equal newValue upon completion of the function.</param>
        /// <param name="customAction">Custom action to execute after raising the property changed event.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes after the PropertyChange event is raised.</remarks>
        public static bool AssignAndUpdateProperty<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name, T newValue, ref T currentValue, Action<string, T> customAction)
        {
            bool valueChanged = SafeDidValueChangeCompare(newValue, currentValue);
            if (valueChanged)
            {
                PreValueUpdate(sender, name);
                currentValue = newValue;
                PostValueUpdate(sender, name);
                RaisePropertyChanged(sender, handler, name, customAction, newValue);
            }
            return valueChanged;
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        public static void RaisePropertyChanged<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name)
        {
            RaisePropertyChanged(sender, handler, name, null, default(T));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="customAction">Custom action to execute after raising the property changed event.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <remarks>The custom action executes after the PropertyChange event is raised.</remarks>
        public static void RaisePropertyChanged<T>(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string name, Action<string, T> customAction, T newValue)
        {
            RaisePropertyChanged(sender, handler, name);
            if (null != customAction)
            {
                customAction(name, newValue);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, string propertyName)
        {
            RaisePropertyChanged(sender, handler, sender, propertyName);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="sender">An implementation of INotifyPropertyChanged.</param>
        /// <param name="handler">The event handler for the sender.</param>
        /// <param name="sendingObject">The entity whose property has been changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        public static void RaisePropertyChanged(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, object sendingObject, string propertyName)
        {
            var propertyChanged = handler;
            if (propertyChanged != null)
            {
                propertyChanged(sendingObject, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Safely compare two values for inequality.
        /// </summary>
        /// <typeparam name="T">The type of the values to compare</typeparam>
        /// <param name="newValue">A 'new' value to compare against an existing one.</param>
        /// <param name="oldValue">An 'old' value to compare against.</param>
        /// <returns><c>true</c> if the two values are considered different.</returns>
        /// <remarks>First, an object.ReferenceEquals comparison is made. If the reference equality is <c>true</c> then the value cannot have changed. Otherwise, a more
        /// distinct comparison is made, using object.Equals(). This comparison is done in a <c>null</c>-safe way.</remarks>
        public static bool SafeDidValueChangeCompare<T>(T newValue, T oldValue)
        {
            bool valueChanged = !object.ReferenceEquals(newValue, oldValue);
            if (valueChanged && (newValue != null))
            {
                valueChanged = !newValue.Equals(oldValue);
            }
            else if (valueChanged && (oldValue != null))
            {
                valueChanged = !oldValue.Equals(newValue);
            }
            return valueChanged;
        }

        /// <summary>
        /// This method will be called prior to updating a value in the AssignAndUpdateProperty method.
        /// </summary>
        /// <param name="sender">The sender of the property update.</param>
        /// <param name="propertyName">The name of the property that is about to be updated.</param>
        static partial void PreValueUpdate(object sender, string propertyName);

        /// <summary>
        /// This method will be called after to updating a value in the AssignAndUpdateProperty method.
        /// </summary>
        /// <param name="sender">The sender of the property update.</param>
        /// <param name="propertyName">The name of the property that was updated.</param>
        static partial void PostValueUpdate(object sender, string propertyName);
    }
}
