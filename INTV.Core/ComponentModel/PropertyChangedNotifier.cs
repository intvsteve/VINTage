// <copyright file="PropertyChangedNotifier.cs" company="INTV Funhouse">
// Copyright (c) 2014 All Rights Reserved
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
    /// Implements INotifyPropertyChanged. Additionally, updates to values may be automatically stored in the application's settings.
    /// </summary>
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        #region Events

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // Events

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        public bool UpdateProperty<T>(string name, T newValue, T currentValue)
        {
            return this.UpdateProperty(PropertyChanged, name, newValue, currentValue);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <param name="customAction">Custom action to execute after raising the property changed event.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes after the PropertyChange event is raised.</remarks>
        public bool UpdateProperty<T>(string name, T newValue, T currentValue, Action<string, T> customAction)
        {
            return this.UpdateProperty(PropertyChanged, name, newValue, currentValue, customAction);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="customAction">Custom action to execute prior to raising the property changed event.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes before the PropertyChange event is raised.</remarks>
        public bool UpdateProperty<T>(string name, Action<string, T> customAction, T newValue, T currentValue)
        {
            return this.UpdateProperty(PropertyChanged, name, customAction, newValue, currentValue);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property. Will equal newValue upon completion of the function.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        public bool AssignAndUpdateProperty<T>(string name, T newValue, ref T currentValue)
        {
            return this.AssignAndUpdateProperty(PropertyChanged, name, newValue, ref currentValue);
        }

        /// <summary>
        /// Updates a property and raises the PropertyChanged event if the current value is modified.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value of the property. Will equal newValue upon completion of the function.</param>
        /// <param name="customAction">Custom action to execute after raising the property changed event.</param>
        /// <returns>If the value is changed, return <c>true</c>, otherwise <c>false</c>.</returns>
        /// <remarks>The custom action executes after the PropertyChange event is raised.</remarks>
        public bool AssignAndUpdateProperty<T>(string name, T newValue, ref T currentValue, Action<string, T> customAction)
        {
            return this.AssignAndUpdateProperty(PropertyChanged, name, newValue, ref currentValue, customAction);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        public void RaisePropertyChanged<T>(string name)
        {
            this.RaisePropertyChanged(PropertyChanged, name);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="name">The name of the property.</param>
        /// <param name="customAction">Custom action to execute after raising the property changed event.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <remarks>The custom action executes after the PropertyChange event is raised.</remarks>
        protected void RaisePropertyChanged<T>(string name, Action<string, T> customAction, T newValue)
        {
            this.RaisePropertyChanged(PropertyChanged, name, customAction, newValue);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            INotifyPropertyChangedHelpers.RaisePropertyChanged(this, PropertyChanged, propertyName);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="sender">The entity whose property has been changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged(object sender, string propertyName)
        {
            this.RaisePropertyChanged(PropertyChanged, sender, propertyName);
        }
    }
}
