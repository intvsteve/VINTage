// <copyright file="ComponentVisual.cs" company="INTV Funhouse">
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

using INTV.Shared.View;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Type to describe a visual used by a component.
    /// </summary>
    public class ComponentVisual : System.Tuple<string, OSVisual, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.ComponentModel.ComponentVisual"/> class.
        /// </summary>
        /// <param name="uniqueId">Unique identifier.</param>
        /// <param name="visual">The visual.</param>
        /// <param name="displayName">Display name.</param>
        public ComponentVisual(string uniqueId, OSVisual visual, string displayName)
            : base(uniqueId, visual, displayName)
        {
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public string UniqueId
        {
            get { return Item1; }
        }

        /// <summary>
        /// Gets the visual.
        /// </summary>
        public OSVisual Visual
        {
            get { return Item2; }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName
        {
            get { return Item3; }
        }
    }
}
