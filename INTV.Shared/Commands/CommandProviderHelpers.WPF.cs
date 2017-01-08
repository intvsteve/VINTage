// <copyright file="CommandProviderHelpers.WPF.cs" company="INTV Funhouse">
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

using System.Collections;
using System.Linq;
using System.Reflection;
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
        /// Additional text to append to controls such as RibbonSplitButton to indicate more commands are available.
        /// </summary>
        public static readonly string RibbonSplitButtonExtraToolTipDescription = Resources.Strings.RibbonSplitButton_ExtraToolTipDescription;

        /// <summary>
        /// Generic tool tip text for the dropdown split part of a RibbonSplitButton.
        /// </summary>
        public static readonly string RibbonSplitButtonToggleButtonTip = Resources.Strings.RibbonSplitButton_ToggleButtonToolTip;

        /// <summary>
        /// This attached property stores the command associated with a visual to assist with data-driven command creation.
        /// </summary>
        private static readonly DependencyProperty VisualCommandProperty = DependencyProperty.RegisterAttached("VisualCommand", typeof(VisualRelayCommand), typeof(CommandProviderHelpers));

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

        /// <summary>
        /// Creates a visual for a command.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent visual.</param>
        /// <returns>The visual for the command.</returns>
        public static UIElement CreateVisualForCommand(this VisualRelayCommand command, bool requiresParentCommand)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(command.UniqueId), "Command's UniqueId is not defined.");
            UIElement visual = null;
            var parentCommand = (VisualRelayCommand)command.VisualParent;
            UIElement parentVisual = null;
            if (parentCommand != null)
            {
                if (parentCommand.Visual == null)
                {
                    var group = parentCommand.GetCommandGroup();
                    if (group != null)
                    {
                        parentCommand.Visual = group.CreateVisualForCommand(parentCommand);
                    }
                }
                ErrorReporting.ReportErrorIf(requiresParentCommand && (parentCommand.Visual == null), "Failed to create parent visual for command: " + command.Name + "(" + command.UniqueId + ")");
                parentVisual = parentCommand.Visual;
            }

            DebugOutputIf(requiresParentCommand && (parentCommand == null), "No parent visual for command: " + command.Name + "(" + command.UniqueId + ")");

            if ((visual == null) && command.UseXamlResource)
            {
                visual = command.CreateVisualFromResource();

                if (visual != null)
                {
                    visual.SetValue(VisualCommandProperty, command);
                    command.AddChildCommandVisual(visual);
                }
            }

            return visual;
        }

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item is to be created.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent menu item.</param>
        /// <returns>The menu item for the command.</returns>
        public static Control CreateMenuItemForCommand(this VisualRelayCommand command, bool requiresParentCommand)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(command.UniqueId), "Command's UniqueId is not defined.");
            Control menuItemVisual = null;

            // Menu items must have a valid parent command already defined.
            var parentCommand = (VisualRelayCommand)command.MenuParent;
            if (parentCommand != null)
            {
                Control parentMenuItemVisual = null;
                if (parentCommand != null)
                {
                    if (parentCommand.MenuItem == null)
                    {
                        var group = parentCommand.GetCommandGroup();
                        if (group != null)
                        {
                            parentCommand.MenuItem = group.CreateMenuItemForCommand(parentCommand);
                        }
                    }
                    DebugOutputIf(requiresParentCommand && (parentCommand.MenuItem == null) && (parentCommand.Visual == null), "Failed to create parent menu item for command: " + command.Name + "(" + command.UniqueId + ")");
                    parentMenuItemVisual = parentCommand.MenuItem;
                }

                DebugOutputIf(requiresParentCommand && (parentCommand == null) && (parentCommand.Visual == null), "No parent menu item for command: " + command.Name + "(" + command.UniqueId + ")");

                if (menuItemVisual == null)
                {
                    menuItemVisual = command.CreateMenuItemFromResource();

                    if (menuItemVisual != null)
                    {
                        menuItemVisual.SetValue(VisualCommandProperty, command);
                        command.AddChildCommandMenuItem(menuItemVisual);
                    }
                }
            }
            return menuItemVisual;
        }

        /// <summary>
        /// Adds a child visual element the parent's visual. If the command's parent visual has not been created,
        /// the function will attempt to do so.
        /// </summary>
        /// <param name="command">The command whose child visual is to be parented</param>
        /// <param name="child">The visual to be parented by the parent command's visual.</param>
        public static void AddChildCommandVisual(this VisualRelayCommand command, UIElement child)
        {
            var parentCommand = command.VisualParent as VisualRelayCommand;
            UIElement parent = null;
            if (parentCommand != null)
            {
                if (parentCommand.Visual == null)
                {
                    var group = parentCommand.GetCommandGroup();
                    if (group != null)
                    {
                        parentCommand.Visual = group.CreateVisualForCommand(parentCommand);
                    }
                }
                parent = parentCommand.Visual;
            }
            if ((parent != null) && (child != null))
            {
                var items = GetItemVisuals(parent);
                if (items != null)
                {
                    var insertLocation = parentCommand.FindInsertLocation(command, false);
                    if (insertLocation < 0)
                    {
                        items.Add(child);
                    }
                    else
                    {
                        items.Insert(insertLocation, child);
                    }
                }
            }
        }

        private static IList GetItemVisuals(UIElement visual)
        {
            var itemsControl = visual as ItemsControl;
            IList items = null;
            if (itemsControl != null)
            {
                items = itemsControl.Items;
            }
            else
            {
                var panel = visual as Panel;
                if (panel != null)
                {
                    items = panel.Children;
                }
            }
            return items;
        }

        /// <summary>
        /// Adds a child menu item visual to the parent's menu item visual. If the command's parent menu item has not been
        /// created, the function will attempt to do so.
        /// </summary>
        /// <param name="command">The command whose child menu item visual is to be parented</param>
        /// <param name="child">The menu item visual to be parented by the parent command's menu item visual.</param>
        public static void AddChildCommandMenuItem(this VisualRelayCommand command, UIElement child)
        {
            var parentCommand = command.MenuParent as VisualRelayCommand;
            Control parent = null;
            if (parentCommand != null)
            {
                if (parentCommand.MenuItem == null)
                {
                    var group = parentCommand.GetCommandGroup();
                    if (group != null)
                    {
                        parentCommand.MenuItem = group.CreateMenuItemForCommand(parentCommand);
                    }
                }
                parent = parentCommand.MenuItem;
            }
            if ((parent != null) && (child != null))
            {
                var itemsControl = parent as ItemsControl;
                if (itemsControl != null)
                {
                    var insertLocation = parentCommand.FindInsertLocation(command, true);
                    if (insertLocation < 0)
                    {
                        itemsControl.Items.Add(child);
                    }
                    else
                    {
                        itemsControl.Items.Insert(insertLocation, child);
                    }
                }
            }
        }

        /// <summary>
        /// Create a menu separator item relative to an existing command.
        /// </summary>
        /// <param name="command">The command needing a separator placed next to it.</param>
        /// <param name="location">Specifies the location of the separator relative to the command.</param>
        /// <returns>A separator pseudo-command.</returns>
        public static VisualRelayCommand CreateRibbonSeparator(this VisualRelayCommand command, CommandLocation location)
        {
            var separator = RootCommandGroup.RibbonSeparatorCommand.Clone();
            var delta = (location == CommandLocation.After) ? RootCommandGroup.MenuSeparatorDelta : -RootCommandGroup.MenuSeparatorDelta;
            separator.Weight = command.Weight + delta;
            separator.VisualParent = command.VisualParent;
            separator.Visual = null; // reset so new visual will always be created
            separator.UseXamlResource = RootCommandGroup.RibbonSeparatorCommand.UseXamlResource; // deficiency in Clone() is that WPF-specific properties aren't cloned
            return separator;
        }

        /// <summary>
        /// Create a menu separator item relative to an existing command.
        /// </summary>
        /// <param name="command">The command needing a separator placed next to it.</param>
        /// <param name="location">Specifies the location of the separator relative to the command.</param>
        /// <param name="forAppMenu">If <c>true</c>, indicates the ribbon separator is for use in the application's ribbon
        /// menu, as opposed to the general ribbon toolbar.</param>
        /// <returns>A separator pseudo-command.</returns>
        public static VisualRelayCommand CreateRibbonMenuSeparator(this VisualRelayCommand command, CommandLocation location, bool forAppMenu)
        {
            var separator = RootCommandGroup.RibbonMenuSeparatorCommand.Clone();
            var delta = (location == CommandLocation.After) ? RootCommandGroup.MenuSeparatorDelta : -RootCommandGroup.MenuSeparatorDelta;
            separator.Weight = command.Weight + delta;
            if (forAppMenu)
            {
                separator.MenuParent = command.MenuParent;
                separator.VisualParent = null;
                separator.UseXamlResource = false;
            }
            else
            {
                separator.MenuParent = null;
                separator.VisualParent = command.VisualParent;
                separator.UseXamlResource = RootCommandGroup.RibbonMenuSeparatorCommand.UseXamlResource; // deficiency in Clone() is that WPF-specific properties aren't cloned
            }
            separator.MenuItem = null; // reset so new menu item will always be created
            separator.Visual = null; // reset so new visual will always be created
            return separator;
        }

        /// <summary>
        /// The application menu for a Ribbon style menu that uses the auxilliary display area may require a minumum number
        /// of menu items in order to avoid clipping the additional content. This method places empty menu items into the
        /// menu in order to accomplish the desired cosmetic appearance.
        /// </summary>
        internal static void PadMenuIfNecessary()
        {
            var menu = RootCommandGroup.ApplicationMenuCommand.MenuItem;
            var items = GetItemVisuals(menu);
            var minNumberMenuItems = 12; // looks good and ensures the auxilliary visual isn't clipped
            if ((items != null) && (items.Count < minNumberMenuItems))
            {
                var resourceName = "INTV.Shared.Commands.Resources.EmptyRibbonMenuItem_MenuItem.xaml";
                for (int i = 0; i < (minNumberMenuItems - items.Count); ++i)
                {
                    var blankMenuItem = CreateVisualFromResource(typeof(CommandProviderHelpers).Assembly, resourceName);
                    items.Add(blankMenuItem);
                }
            }
        }

        private static UIElement CreateVisualFromResource(this VisualRelayCommand command)
        {
            var assembly = typeof(CommandProviderHelpers).Assembly;
            if ((command.UniqueId != RootCommandGroup.RibbonSeparatorCommand.UniqueId) && (command.UniqueId != RootCommandGroup.RibbonMenuSeparatorCommand.UniqueId))
            {
                var group = command.GetCommandGroup();
                assembly = group.GetType().Assembly;
            }
            var resourceName = command.MakeCommandResourceName(string.Empty);
            return CreateVisualFromResource(assembly, resourceName);
        }

        private static Control CreateMenuItemFromResource(this VisualRelayCommand command)
        {
            var assembly = typeof(CommandProviderHelpers).Assembly;
            if (command.UniqueId != RootCommandGroup.RibbonMenuSeparatorCommand.UniqueId)
            {
                var group = command.GetCommandGroup();
                assembly = group.GetType().Assembly;
            }
            var resourceName = command.MakeCommandResourceName("_MenuItem");
            return CreateVisualFromResource(assembly, resourceName) as Control;
        }

        private static UIElement CreateVisualFromResource(Assembly assembly, string resourceName)
        {
            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream != null)
                {
                    var visual = System.Windows.Markup.XamlReader.Load(resourceStream) as UIElement;
                    return visual;
                }
                else
                {
                    DebugOutput("Failed to get resource: " + resourceName);
                }
                return null;
            }
        }

        private static string MakeCommandResourceName(this VisualRelayCommand command, string suffix)
        {
            var resourceName = string.Empty;
            var assembly = typeof(CommandProviderHelpers).Assembly;
            if (command.UniqueId == RootCommandGroup.RibbonSeparatorCommand.UniqueId)
            {
                resourceName = "INTV.Shared.Commands.Resources.RibbonSeparatorCommand.xaml";
            }
            else if (command.UniqueId == RootCommandGroup.RibbonMenuSeparatorCommand.UniqueId)
            {
                resourceName = "INTV.Shared.Commands.Resources.RibbonMenuSeparatorCommand.xaml";
            }
            else
            {
                var group = command.GetCommandGroup();
                assembly = group.GetType().Assembly;
                resourceName = assembly.GetName().Name + ".Commands.Resources." + command.UniqueId.Split('.').Last() + suffix + ".xaml";
            }
            return resourceName;
        }

        /// <summary>
        /// Finds the index to use as an insert location into a list of commands (e.g. position in submenu) based on the command's weight.
        /// </summary>
        /// <param name="parentCommand">The parent command, which is used to locate other child commands for determining insert location.</param>
        /// <param name="commandToInsert">The command to insert.</param>
        /// <param name="useMenuItem">If <c>true</c>, use the parent menu item visual; otherwise, use the standard visual.</param>
        /// <returns>The insertion index, or -1 if the command is to be placed at the end of the list.</returns>
        private static int FindInsertLocation(this VisualRelayCommand parentCommand, VisualRelayCommand commandToInsert, bool useMenuItem)
        {
            int insertLocation = -1; // default to unknown
            if (parentCommand != null)
            {
                var parentVisual = useMenuItem ? parentCommand.MenuItem : parentCommand.Visual;
                var items = GetItemVisuals(parentVisual);
                if (items != null)
                {
                    var itemCommands = items.Cast<UIElement>().Select(i => i.GetValue(VisualCommandProperty) as VisualRelayCommand).ToList();
                    for (int i = 0; (i < itemCommands.Count) && (insertLocation < 0); ++i)
                    {
                        var itemCommand = itemCommands[i];
                        if (itemCommand != null)
                        {
                            var weight = itemCommand.Weight;
                            if (weight > commandToInsert.Weight)
                            {
                                insertLocation = i;
                            }
                        }
                    }
                }
            }
            return insertLocation;
        }

        private static ICommandGroup GetCommandGroup(this RelayCommand command)
        {
            var commandProviders = SingleInstanceApplication.Instance.CommandProviders.Select(p => p.Value);
            var groups = commandProviders.SelectMany(p => p.CommandGroups);
            var group = groups.FirstOrDefault(g => g.Commands.Contains(command));
            return group;
        }
    }
}
