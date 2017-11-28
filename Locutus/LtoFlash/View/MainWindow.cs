// <copyright file="MainWindow.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

namespace Locutus.View
{
    /// <summary>
    /// Platform-agnostic portion of the implementation.
    /// </summary>
    public partial class MainWindow : IMainWindow
    {
        #region IMainWindow

        /// <inheritdoc/>
        public void AddPrimaryComponentVisuals(IPrimaryComponent primaryComponent, IEnumerable<ComponentVisual> visuals)
        {
            OSAddPrimaryComponentVisuals(primaryComponent, visuals);
        }

        #endregion // IMainWindow
    }
}
