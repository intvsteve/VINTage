// <copyright file="LtoFlashViewModel.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2018 All Rights Reserved
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

using INTV.Shared.ComponentModel;
using INTV.Shared.View;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IPrimaryComponent))]
    public partial class LtoFlashViewModel
    {
        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        partial void OSCreateVisuals()
        {
            // TODO: This is wrong... ViewModel should not create the visual - it should be the other way around
            var view = new INTV.LtoFlash.View.MenuLayoutView(this);
            _visual = new System.WeakReference(view);
            OSInitializeVisual(view);
        }

        private void OSInitializeVisual(OSVisual visual)
        {
        }
    }
}
