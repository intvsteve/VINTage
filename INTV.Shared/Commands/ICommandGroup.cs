// <copyright file="ICommandGroup.cs" company="INTV Funhouse">
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
using INTV.Shared.ComponentModel;

#if WIN
using OSCommandVisual = System.Windows.UIElement;
using OSImage = System.Windows.Media.Imaging.BitmapImage;
using OSMenuItem = System.Windows.Controls.Control;
#elif MAC
using INTV.Shared.View;
using INTV.Shared.Utility;
#if __UNIFIED__
using OSCommandVisual = Foundation.NSObject;
#else
using OSCommandVisual = MonoMac.Foundation.NSObject;
#endif // __UNIFIED__
#elif GTK
using INTV.Shared.Utility;
using INTV.Shared.View;
using OSCommandVisual = INTV.Shared.View.OSVisual;
#endif // WIN

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Defines an interface for implementing a set of grouped commands.
    /// </summary>
    /// <remarks>Should this subclass the ICommand interface?</remarks>
    public interface ICommandGroup
    {
        /// <summary>
        /// Gets the unique name for a command group's tab in the ribbon.
        /// </summary>
        string TabUniqueName { get; }

        /// <summary>
        /// Gets the user-visible name of a command group's tab in the ribbon.
        /// </summary>
        string TabName { get; }

        /// <summary>
        /// Gets the unique name of the command group.
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Gets the user-visible name of the command group.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the weight of the command group. Must be in the range [0.0-1.0].
        /// </summary>
        double Weight { get; }

        /// <summary>
        /// Gets an enumerable of the commands in the group.
        /// </summary>
        IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// Gets an icon to represent the command group.
        /// </summary>
        OSImage Icon { get; }

        /// <summary>
        /// Creates the visual for a command, if applicable.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <returns>The visual for the command.</returns>
        OSCommandVisual CreateVisualForCommand(ICommand command);

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item must be created.</param>
        /// <returns>The menu item.</returns>
        OSMenuItem CreateMenuItemForCommand(ICommand command);

        /// <summary>
        /// Retrieves an enumerable of the commands that should be included in a context menu for the given type.
        /// </summary>
        /// <param name="target">The target object for which a context menu is being generated.</param>
        /// <param name="context">User-specified data (meaningful to the implementation) used for command creation.</param>
        /// <returns>An enumerable containing the menu items to include in a context menu.</returns>
        IEnumerable<ICommand> CreateContextMenuCommands(object target, object context);
    }
}
