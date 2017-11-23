// <copyright file="IPrimaryComponent.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Shared.View;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// This interface is used to define a top-level component that the VINTage application framework recognizes.
    /// Don't confuse this with the generic System.Component IComponent-based approach.
    /// </summary>
    public interface IPrimaryComponent
    {
        /// <summary>
        /// Gets the unique identifier of the component.
        /// </summary>
        string UniqueId { get; }

        /// <summary>
        /// Components that have any post-construction initialization should implement it in this method.
        /// </summary>
        /// <remarks>This may be called asynchronously on worker threads. Take care!</remarks>
        void Initialize();

        /// <summary>
        /// Gets the main visual that represents the component. May be empty.
        /// </summary>
        /// <returns>The primary visual of the component. If a component does not
        /// present a main visual working area, it may return <see cref="OSVisual.Empty"/>.</returns>
        IEnumerable<ComponentVisual> GetVisuals();
    }
}
