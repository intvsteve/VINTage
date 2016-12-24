// <copyright file="OSMessageBoxIcon.WPF.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    public enum OSMessageBoxIcon
    {
        /// <summary>
        /// No icon displayed.
        /// </summary>
        None = System.Windows.MessageBoxImage.None,

        /// <summary>
        /// Question icon displayed.
        /// </summary>
        Question = System.Windows.MessageBoxImage.Question,

        /// <summary>
        /// Information icon displayed.
        /// </summary>
        Information = System.Windows.MessageBoxImage.Information,

        /// <summary>
        /// Exclamation icon displayed.
        /// </summary>
        Exclamation = System.Windows.MessageBoxImage.Exclamation,

        /// <summary>
        /// Error icon displayed.
        /// </summary>
        Error = System.Windows.MessageBoxImage.Error,
    }
}
