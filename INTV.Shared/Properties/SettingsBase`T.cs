// <copyright file="SettingsBase`T.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Properties
{
    /// <summary>
    /// This generic provides a simple way for derived types to have some common
    /// implementation. Most crucially, it replicates the static 'Default' property
    /// that is widely used throughout this codebase to access settings.
    /// </summary>
    public abstract class SettingsBase<T> : SettingsBase where T : SettingsBase, new()
    {
        /// <summary>
        /// Get the default instance of the settings.
        /// </summary>
        /// <value>The default.</value>
        public static T Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
        private static T _instance;
    }
}
