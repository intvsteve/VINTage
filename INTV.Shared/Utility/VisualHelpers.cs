// <copyright file="VisualHelpers.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;

#if WIN
using OSVisual = System.Windows.DependencyObject;
using OSWindow = System.Windows.Window;
#elif MAC
using OSVisual = MonoMac.AppKit.NSView;
using OSWindow = MonoMac.AppKit.NSWindow;
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Useful extension methods for working with visual / event handler types.
    /// </summary>
    public static partial class VisualHelpers
    {
        /// <summary>
        /// Finds a parent of the given type by moving up the visual tree.
        /// </summary>
        /// <typeparam name="T">The data type of the parent visual to locate.</typeparam>
        /// <param name="visual">The visual whose parent of a specific type is desired.</param>
        /// <returns>The first ancestor in the visual tree of the requested type.</returns>
        public static T GetParent<T>(this OSVisual visual) where T : OSVisual
        {
            return visual.GetParent<T>(null);
        }

        /// <summary>
        /// Finds a parent of the given type by moving up the visual tree, applying a filter.
        /// </summary>
        /// <typeparam name="T">The data type of the parent visual to locate.</typeparam>
        /// <param name="visual">The visual whose parent of a specific type is desired.</param>
        /// <param name="filter">A filter to be applied to determine if a parent of the requested type shall be accepted. If <c>null</c>, the first parent of the requested type matches.</param>
        /// <returns>The first ancestor in the visual tree of the requested type that also satisfies the condition of the filter.</returns>
        public static T GetParent<T>(this OSVisual visual, Func<T, bool> filter) where T : OSVisual
        {
            T parentOfType = null;
            var current = visual;
            while ((current != null) && (parentOfType == null))
            {
                current = current.GetParentVisual();
                parentOfType = current as T;
                if ((filter != null) && (parentOfType != null) && !filter(parentOfType))
                {
                    parentOfType = null;
                }
            }
            return parentOfType;
        }

        /// <summary>
        /// Finds a child visual of the given type.
        /// </summary>
        /// <typeparam name="T">The data type of the parent visual to locate.</typeparam>
        /// <param name="visual">The visual whose child of a specific type is desired.</param>
        /// <returns>The first child in the visual tree of the requested type.</returns>
        public static T FindChild<T>(this OSVisual visual) where T : OSVisual
        {
            return FindChild<T>(visual, null);
        }

        /// <summary>
        /// Finds a child visual of the given type, applying a filter.
        /// </summary>
        /// <typeparam name="T">The data type of the parent visual to locate.</typeparam>
        /// <param name="visual">The visual whose child of a specific type is desired.</param>
        /// <param name="filter">A filter to be applied to determine if a child of the requested type shall be accepted. If <c>null</c>, the first child of the requested type matches.</param>
        /// <returns>The first child in the visual tree of the requested type that also satisfies the condition of the filter.</returns>
        public static T FindChild<T>(this OSVisual visual, Func<T, bool> filter) where T : OSVisual
        {
            var child = default(T);
            var numChildren = visual.GetChildVisualCount();
            for (int i = 0; (i < numChildren) && (child == null); ++i)
            {
                var typedChild = visual.GetChildAtIndex<T>(i);
                if ((typedChild != null) && ((filter == null) || filter(typedChild)))
                {
                    child = typedChild;
                }
            }
            if (child == null)
            {
                for (int i = 0; (i < numChildren) && (child == null); ++i)
                {
                    child = FindChild<T>(visual.GetChildAtIndex<OSVisual>(i), filter);
                }
            }
            return child;
        }

        /// <summary>
        /// Creates a new instance of a window type, setting the window's owner and marking it to center on the owner, if applicable.
        /// </summary>
        /// <typeparam name="T">The class of window to create.</typeparam>
        /// <param name="ownerWindow">The owner window. If <c>null</c>, the current window is used.</param>
        /// <returns>A new instance of the type.</returns>
        public static T Create<T>(this OSWindow ownerWindow) where T : OSWindow, new()
        {
            GetWindowOwner(ref ownerWindow);
            var newWindow = new T();
            newWindow.SetWindowOwner(ownerWindow);
            return newWindow;
        }

        /// <summary>
        /// Platform-specific method to get a window's owner if provided owner is <c>null.</c>
        /// </summary>
        /// <param name="owner">The owner for the window. Set to current MainWindow if <c>null</c>.</param>
        static partial void GetWindowOwner(ref OSWindow owner);

        /// <summary>
        /// Platform-specific ownership and position setup.
        /// </summary>
        /// <param name="newWindow">A window being created.</param>
        /// <param name="owner">The owner to assign to the window.</param>
        static partial void SetWindowOwner(this OSWindow newWindow, OSWindow owner);

        /// <summary>
        /// Adds commands to the main window.
        /// </summary>
        /// <param name="window">The application window to which command visuals, menus, etc. are to be added.</param>
        /// <param name="commandProviders">The command providers.</param>
        public static void AddCommandsToMainWindow(this OSWindow window, IEnumerable<ICommandProvider> commandProviders)
        {
            // Collect all the command groups and sort them by weight.
            var commandGroups = new List<ICommandGroup>(commandProviders.SelectMany(c => c.CommandGroups).OrderBy(g => g.Weight));
            window.AddCommandGroups(commandGroups);
        }

        private static void AddCommandGroups(this OSWindow window, IEnumerable<ICommandGroup> commandGroups)
        {
            int totalCommandCount = 0;
            foreach (var commandGroup in commandGroups)
            {
                totalCommandCount += commandGroup.Commands.Count();
            }
            foreach (var commandGroup in commandGroups)
            {
                foreach (var command in commandGroup.Commands.Select(c => (VisualRelayCommand)c).OrderBy(c => c.Weight))
                {
                    command.Visual = commandGroup.CreateVisualForCommand(command);
                    command.MenuItem = commandGroup.CreateMenuItemForCommand(command);
                    AttachCanExecuteHandler(commandGroup as CommandGroup, command as RelayCommand);
                }
            }
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Attaches a CanExecute handler to the given command.
        /// </summary>
        /// <param name="commandGroup">The command group to which <paramref name="command"/> belongs.</param>
        /// <param name="command">The command to which an can execute handler needs to be attached.</param>
        static partial void AttachCanExecuteHandler(CommandGroup commandGroup, RelayCommand command);
    }
}
