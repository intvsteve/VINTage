// <copyright file="CommandProviderHelpers.WPF.cs" company="INTV Funhouse">
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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Windows-specific implementation.
    /// </summary>
    public static partial class CommandProviderHelpers
    {
        /// <summary>
        /// Adds an input binding given a command with keyboard shortcut information defined.
        /// </summary>
        /// <param name="command">The command containing a keyboard shortcut.</param>
        /// <param name="visual">The visual for which to register the binding.</param>
        /// <remarks>If the visual is null, the shortcut will be registered using the
        /// current application's MainWindow.</remarks>
        public static void AddInputBinding(this VisualRelayCommand command, UIElement visual)
        {
            if (!string.IsNullOrEmpty(command.KeyboardShortcutKey))
            {
                ErrorReporting.ReportErrorIf(command.KeyboardShortcutKey.Length != 1, "Invalid keyboard shortcut!");
                var modfiers = (ModifierKeys)command.KeyboardShortcutModifiers;
                if (char.IsUpper(command.KeyboardShortcutKey[0]))
                {
                    modfiers |= ModifierKeys.Shift;
                }
                var keyConverter = new KeyConverter();
                var key = (Key)keyConverter.ConvertFromInvariantString(command.KeyboardShortcutKey);
                if (visual == null)
                {
                    visual = SingleInstanceApplication.Current.MainWindow;
                }
                visual.InputBindings.Add(new KeyBinding(command, key, modfiers));
            }
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <param name="target">The data type for which a context menu is desired.</param>
        /// <param name="menuName">The name of the menu to create.</param>
        /// <param name="context">The context used for creating the menu item.</param>
        /// <returns>The menu, containing the menu items.</returns>
        public static ContextMenu CreateContextMenu(this object target, string menuName, object context)
        {
            var menu = new ContextMenu();
            foreach (var command in target.GetContextMenuCommands(context).OfType<VisualRelayCommand>().OrderBy(c => c.Weight))
            {
                menu.Items.Add(command.MenuItem);
            }
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested(); // Ensure items in context menu properly updated
            return menu;
        }

        /// <summary>
        /// Creates a version of the given command for use in a context menu.
        /// </summary>
        /// <param name="command">The command to clone for use in a context menu.</param>
        /// <param name="name">Overriding name for the command; if <c>null</c> or empty, use command's ContextMenuItemName.</param>
        /// <param name="weight">Weight for the command. If this value is <c>double.NaN</c>, the weight is the same as the original command's weight.</param>
        /// <returns>The command to use in a context menu.</returns>
        public static VisualRelayCommand CreateContextMenuItemCommand(this VisualRelayCommand command, string name, double weight)
        {
            var contextCommand = command.Clone();
            contextCommand.MenuParent = null;
            contextCommand.VisualParent = null;
            contextCommand.Visual = null;
            contextCommand.MenuItem = null;
            contextCommand.KeyboardShortcutKey = null;
            contextCommand.KeyboardShortcutModifiers = OSModifierKeys.None;
            if (!double.IsNaN(weight))
            {
                contextCommand.Weight = weight;
            }
            if (command.UniqueId != RootCommandGroup.MenuSeparatorCommand.UniqueId)
            {
                var menuItem = new System.Windows.Controls.MenuItem();
                name = name ?? command.ContextMenuItemName;
                name = name ?? command.MenuItemName;
                name = name ?? command.Name;
                menuItem.Header = name ?? command.ContextMenuItemName;
                if (command.SmallIcon != null)
                {
                    menuItem.Icon = new Image() { Source = command.SmallIcon };
                }
                menuItem.Command = command;
                contextCommand.MenuItem = menuItem;
            }
            else
            {
                contextCommand.MenuItem = new Separator();
            }
            return contextCommand;
        }
    }
}
