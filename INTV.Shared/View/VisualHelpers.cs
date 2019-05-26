// <copyright file="VisualHelpers.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;

#if WIN
using NativeVisual = System.Windows.DependencyObject;
using NativeWindow = System.Windows.Window;
#elif MAC
#if __UNIFIED__
using NativeVisual = AppKit.NSView;
using NativeWindow = AppKit.NSWindow;
#else
using NativeVisual = MonoMac.AppKit.NSView;
using NativeWindow = MonoMac.AppKit.NSWindow;
#endif // __UNIFIED__
#elif GTK
using NativeVisual = Gtk.Widget;
using NativeWindow = Gtk.Window;
#endif // WIN

namespace INTV.Shared.View
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
        public static T GetParent<T>(this NativeVisual visual) where T : NativeVisual
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
        public static T GetParent<T>(this NativeVisual visual, Predicate<T> filter) where T : NativeVisual
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
        /// <typeparam name="T">The data type of the child visual to locate.</typeparam>
        /// <param name="visual">The visual whose child of a specific type is desired.</param>
        /// <returns>The first child in the visual tree of the requested type.</returns>
        public static T FindChild<T>(this NativeVisual visual) where T : NativeVisual
        {
            return FindChild<T>(visual, filter: null);
        }

        /// <summary>
        /// Finds a child visual of the given type, applying a filter.
        /// </summary>
        /// <typeparam name="T">The data type of the child visual to locate.</typeparam>
        /// <param name="visual">The visual whose child of a specific type is desired.</param>
        /// <param name="filter">A filter to be applied to determine if a child of the requested type shall be accepted. If <c>null</c>, the first child of the requested type matches.</param>
        /// <returns>The first child in the visual tree of the requested type that also satisfies the condition of the filter.</returns>
        public static T FindChild<T>(this NativeVisual visual, Predicate<T> filter) where T : NativeVisual
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
                    child = FindChild<T>(visual.GetChildAtIndex<NativeVisual>(i), filter);
                }
            }
            return child;
        }

        /// <summary>
        /// Finds the child visuals of the given type.
        /// </summary>
        /// <typeparam name="T">The data type of the child visuals to locate.</typeparam>
        /// <param name="visual">The visual whose children of a specific type is desired.</param>
        /// <returns>The child visuals that match the desired type.</returns>
        public static IEnumerable<T> FindChildren<T>(this NativeVisual visual) where T : NativeVisual
        {
            return visual.FindChildren<T>(filter: null);
        }

        /// <summary>
        /// Finds the child visuals of the given type based on the supplied filter functon.
        /// </summary>
        /// <typeparam name="T">The data type of the child visuals to locate.</typeparam>
        /// <param name="visual">The visual whose children of a specific type is desired.</param>
        /// <param name="filter">A filter to be applied to determine if a child of the requested type shall be accepted. If <c>null</c>, all children of the requested type match.</param>
        /// <returns>The child visuals that satisfy the given <paramref name="filter"/>.</returns>
        public static IEnumerable<T> FindChildren<T>(this NativeVisual visual, Predicate<T> filter) where T : NativeVisual
        {
            var children = new List<T>();
            var numChildren = visual.GetChildVisualCount();
            for (int i = 0; i < numChildren; ++i)
            {
                var child = visual.GetChildAtIndex<NativeVisual>(i);
                var typedChild = child as T;
                if ((typedChild != null) && ((filter == null) || filter(typedChild)))
                {
                    children.Add(typedChild);
                }
                if (child != null)
                {
                    children.AddRange(child.FindChildren<T>(filter));
                }
            }
            return children;
        }

        /// <summary>
        /// Creates a new instance of a window type, setting the window's owner and marking it to center on the owner, if applicable.
        /// </summary>
        /// <typeparam name="T">The class of window to create.</typeparam>
        /// <param name="ownerWindow">The owner window. If <c>null</c>, the current window is used.</param>
        /// <returns>A new instance of the type.</returns>
        public static T Create<T>(this NativeWindow ownerWindow) where T : NativeWindow, new()
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
        static partial void GetWindowOwner(ref NativeWindow owner);

        /// <summary>
        /// Platform-specific ownership and position setup.
        /// </summary>
        /// <param name="newWindow">A window being created.</param>
        /// <param name="owner">The owner to assign to the window.</param>
        static partial void SetWindowOwner(this NativeWindow newWindow, NativeWindow owner);

        /// <summary>
        /// Adds commands to the main window.
        /// </summary>
        /// <param name="window">The application window to which command visuals, menus, etc. are to be added.</param>
        /// <param name="commandProviders">The command providers.</param>
        public static void AddCommandsToMainWindow(this NativeWindow window, IEnumerable<ICommandProvider> commandProviders)
        {
            // Visit providers by weight.
            foreach (var commandProvider in commandProviders.OrderBy(c => c.Weight))
            {
                // Collect all the command groups and sort them by weight.
                var commandGroups = commandProvider.CommandGroups.OrderBy(g => g.Weight);
                window.AddCommandGroups(commandGroups);
            }
            window.AddCommandsToMainWindowComplete();
        }

        /// <summary>
        /// Gets the primary display information, which is a Tuple of width, height, and color depth in bits per pixel.
        /// </summary>
        /// <returns>The primary display info.</returns>
        public static Tuple<int, int, int> GetPrimaryDisplayInfo()
        {
            return OSGetPrimaryDisplayInfo();
        }

        private static void AddCommandGroups(this NativeWindow window, IEnumerable<ICommandGroup> commandGroups)
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
                    if (((OSVisual)command.Visual).IsEmpty)
                    {
                        command.Visual = commandGroup.CreateVisualForCommand(command);
                    }
                    if (command.MenuItem.IsEmpty)
                    {
                        command.MenuItem = commandGroup.CreateMenuItemForCommand(command);
                    }
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

        /// <summary>
        /// Called after provider commands have been added in the AddCommandGroups method.
        /// </summary>
        /// <param name="window">Window whose commands have just been added.</param>
        static partial void AddCommandsToMainWindowComplete(this NativeWindow window);
    }
}
