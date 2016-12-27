// <copyright file="SettingsPageViewModel`T.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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
using AppKit;
#else
using MonoMac.AppKit;
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class SettingsPageViewModel<T>
    {
        #region ISettingsPage

        /// <inheritdoc />
        public NSViewController CreateVisual()
        {
            var visual = new T();
            var view = visual as INTV.Shared.View.IFakeDependencyObject;
            view.DataContext = this;
            return visual;
        }

        #endregion // ISettingsPage

        /// <summary>
        /// Needed to ensure visual element states are properly enabled.
        /// </summary>
        internal void RaisePropertyChangedForVisualInit()
        {
            RaiseAllPropertiesChanged();
        }
    }
}
