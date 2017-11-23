﻿// <copyright file="IMainWindow.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    /// <summary>
    /// Generic interface to the main window of an application.
    /// </summary>
    public interface IMainWindow
    {
        /// <summary>
        /// This is called to instruct the main window that the visual for a primary component
        /// has been created, and should be added to the window.
        /// </summary>
        /// <param name="primaryComponent">The component the visual is associated with.</param>
        /// <param name="visuals">The visuals for the component.</param>
        void AddPrimaryComponentVisuals(IPrimaryComponent primaryComponent, IEnumerable<ComponentVisual> visuals);
    }
}
