// <copyright file="ICommandProvider.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Defines the interface to a provider of commands to the application.
    /// </summary>
    public interface ICommandProvider
    {
        /// <summary>
        /// Gets the unique name of the command provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the command groups supplied by the command provider.
        /// </summary>
        IEnumerable<ICommandGroup> CommandGroups { get; }

        /// <summary>
        /// Retrieves an enumerable of the commands that should be included in a context menu for the given type.
        /// </summary>
        /// <param name="target">The target object for which a context menu is being generated.</param>
        /// <param name="context">User-specified data (meaningful to the implementation) used for command creation.</param>
        /// <returns>An enumerable containing the menu items to include in a context menu.</returns>
        IEnumerable<ICommand> CreateContextMenuCommands(object target, object context);
    }
}
