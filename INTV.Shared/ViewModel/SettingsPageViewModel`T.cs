// <copyright file="SettingsPageViewModel`T.cs" company="INTV Funhouse">
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
using INTV.Core.ComponentModel;
using INTV.Core.Model.Program;

#if WIN
using BaseClass = System.Object;
using OSVisual = System.Windows.FrameworkElement;
#elif MAC
#if __UNIFIED__
using BaseClass = Foundation.NSObject;
using OSVisual = AppKit.NSViewController;
#else
using BaseClass = MonoMac.Foundation.NSObject;
using OSVisual = MonoMac.AppKit.NSViewController;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Provides the skeleton ViewModel for implementing pages in the settings dialog.
    /// </summary>
    /// <typeparam name="T">The data type of the visual for which this class acts as a ViewModel.</typeparam>
    public abstract partial class SettingsPageViewModel<T> : BaseClass, System.ComponentModel.INotifyPropertyChanged, ISettingsPage where T : OSVisual, new()
    {
        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        #region Properties

        /// <summary>
        /// Gets the features being edited. Useful for doing some simple undo / reset operations, etc.
        /// </summary>
        protected ProgramFeatures Features { get; private set; }

        #endregion Properties

        #region IRomFeaturesConfigurationPage

        /// <inheritdoc />
        public virtual void Initialize(ProgramFeatures features)
        {
            Features = features;
        }

        #endregion // IRomFeaturesConfigurationPage

        /// <summary>
        /// Helper method to encapsulate raising the value change event, assign new value, and update feature bits.
        /// </summary>
        /// <param name="propertyName">Name of the property being updated for INotifyPropertyChanged.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value, which will be assigned a new value.</param>
        protected void UpdateFeatureProperty(string propertyName, ProgramFeatureImageViewModel newValue, ref ProgramFeatureImageViewModel currentValue)
        {
            UpdateFeatureProperty(propertyName, newValue, ref currentValue, null);
        }

        /// <summary>
        /// Helper method to encapsulate raising the value change event, assign new value, and update feature bits.
        /// </summary>
        /// <param name="propertyName">Name of the property being updated for INotifyPropertyChanged.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <param name="currentValue">The current value, which will be assigned a new value.</param>
        /// <param name="customAction">A custom action to execute after property update occurs.</param>
        protected void UpdateFeatureProperty(string propertyName, ProgramFeatureImageViewModel newValue, ref ProgramFeatureImageViewModel currentValue, Action<string, ProgramFeatureImageViewModel> customAction)
        {
            if (currentValue != null)
            {
                Features.UpdateFeatureBits(currentValue.Category, currentValue.Flags, false);
            }
            if (newValue != null)
            {
                Features.UpdateFeatureBits(newValue.Category, newValue.Flags, true);
            }
            this.AssignAndUpdateProperty(PropertyChanged, propertyName, newValue, ref currentValue, customAction);
        }

        /// <summary>
        /// Helper method to update a property's value and raise PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="newValue">New value.</param>
        /// <param name="currentValue">Current value.</param>
        /// <typeparam name="TProperty">The data type of the property to update.</typeparam>
        protected void AssignAndUpdateProperty<TProperty>(string propertyName, TProperty newValue, ref TProperty currentValue)
        {
            AssignAndUpdateProperty(propertyName, newValue, ref currentValue, null);
        }

        /// <summary>
        /// Assigns the and update property.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="newValue">New value.</param>
        /// <param name="currentValue">Current value.</param>
        /// <param name="customAction">Custom action.</param>
        /// <typeparam name="TProperty">The data type of the property to update.</typeparam>
        protected void AssignAndUpdateProperty<TProperty>(string propertyName, TProperty newValue, ref TProperty currentValue, Action<string, TProperty> customAction)
        {
            this.AssignAndUpdateProperty(PropertyChanged, propertyName, newValue, ref currentValue, customAction);
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(PropertyChanged, propertyName);
        }

        /// <summary>
        /// Implementation should raise the PropertyChanged event for all properties on the object. This is
        /// intended for use at the end of the Initialize() method.
        /// </summary>
        protected abstract void RaiseAllPropertiesChanged();
    }
}
