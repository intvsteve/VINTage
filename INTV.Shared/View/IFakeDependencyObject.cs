// <copyright file="IFakeDependencyObject.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    /// <summary>
    /// This interface mimics a tiny subset of the WPF System.Windows.DependencyObject
    /// and System.Windows.FrameworkElement types to help with sharing code across platforms.
    /// </summary>
    public interface IFakeDependencyObject
    {
        /// <summary>
        /// Gets or sets an arbitrary data object associated with the object. It is the analog of
        /// System.Windows.FrameworkElement.DataContext, though it is not implemented as an
        /// attached System.Windows.DependencyProperty -- it is far more simplistic.
        /// </summary>
        object DataContext { get; set; }

        /// <summary>
        /// Gets an arbitrary data value with the given name. Very poor approximation of the
        /// System.Windows.DependencyProperty system.
        /// </summary>
        /// <param name="propertyName">The name of the value to get.</param>
        /// <returns>The value identified by the given name.</returns>
        object GetValue(string propertyName);

        /// <summary>
        /// Sets an arbitrary data value, associating it with the given name. Very poor approximation
        /// of a System.Windows.DependencyProperty system.
        /// </summary>
        /// <param name="propertyName">The name of the value, used to retrieve it later.</param>
        /// <param name="value">The value to store.</param>
        void SetValue(string propertyName, object value);
    }
}
