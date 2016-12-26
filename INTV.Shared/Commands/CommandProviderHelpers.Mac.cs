// <copyright file="CommandProviderHelpers.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

////#define ENABLE_DIAGNOSTIC_OUTPUT

using System.Collections.Generic;
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mac-specific extension methods to assist with command implementation.
    /// </summary>
    public static partial class CommandProviderHelpers
    {
        /// <summary>
        /// Delete character (see NSText.h from the AppKit).
        /// </summary>
        public const char NSDeleteCharacter = (char)0x007f;

        /// <summary>
        /// NSDeleteCharacter as a string.
        /// </summary>
        public static readonly string NSDeleteCharacterString = new string(NSDeleteCharacter, 1);

        /// <summary>
        /// Backspace character (see NSText.h from the AppKit).
        /// </summary>
        public const char NSBackspaceCharacter = (char)0x0008;

        /// <summary>
        /// NSBackspaceCharacter as a string.
        /// </summary>
        public static readonly string NSBackspaceCharacterString = new string(NSBackspaceCharacter, 1);

        /// <summary>
        /// This value is used to assist with menu item tags. Perhaps this can be removed...
        /// </summary>
        private const double WeightScaleFactor = 1000;

        /// <summary>
        /// Adds an item to a toolbar.
        /// </summary>
        /// <param name="toolbar">The <see cref=">NSToolbar"/> to which an item is to be added.</param>
        /// <param name="toolbarItem">The <see cref="NSToolbarItem"/> to be added to the toolbar.</param>
        /// <param name="itemIdentifier">The item's unique identifier.</param>
        /// <param name="insertLocation">The location in the toolbar at which the item is to be inserted.</param>
        public static void AddToolbarItem(this NSToolbar toolbar, NSToolbarItem toolbarItem, string itemIdentifier, int insertLocation)
        {
            if (toolbar.DefaultItemIdentifiers == null)
            {
                toolbar.DefaultItemIdentifiers = NSToolbarIdentifiers;
            }
            if (toolbar.AllowedItemIdentifiers == null)
            {
                toolbar.AllowedItemIdentifiers = NSToolbarIdentifiers;
            }
            if (toolbar.SelectableItemIdentifiers == null)
            {
                // HACK WORKAROUND for bug https://bugzilla.xamarin.com/show_bug.cgi?id=21680
                toolbar.SelectableItemIdentifiers = BugWorkaround;
            }
            var prevWillAdd = toolbar.WillInsertItem;
            toolbar.WillInsertItem = (t, i, b) => toolbarItem;
            toolbar.InsertItem(itemIdentifier, insertLocation);
            toolbar.WillInsertItem = prevWillAdd;
        }

        /// <summary>
        /// Creates a visual for a command.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent visual.</param>
        /// <returns>The visual for the command.</returns>
        public static NSObject CreateVisualForCommand(this VisualRelayCommand command, bool requiresParentCommand)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(command.UniqueId), "Command's UniqueId is not defined.");
            NSObject visual = null;
            var parentCommand = (VisualRelayCommand)command.VisualParent;
            NSObject parentVisual = null;
            if (parentCommand != null)
            {
                ErrorReporting.ReportErrorIf(parentCommand.Visual == null, "Failed to create parent visual for command: " + command.Name + "(" + command.UniqueId + ")");
                parentVisual = parentCommand.Visual;
            }

            DebugOutputIf(parentCommand == null, "No parent visual for command: " + command.Name + "(" + command.UniqueId + ")");

            var insertLocation = parentCommand.FindInsertLocation(command);
            if (parentVisual is NSToolbar)
            {
                var toolbar = (NSToolbar)parentVisual;
                var toolbarItem = new NSToolbarItem(command.UniqueId);
                toolbarItem.Autovalidates = false;
                toolbarItem.Tag = (int)(command.Weight * WeightScaleFactor);
                toolbarItem.Label = command.Name.TrimEnd(new char[] { '.' });
                toolbarItem.Image = command.LargeIcon;
                toolbarItem.MinSize = command.LargeIcon.Size;
                toolbarItem.MaxSize = command.LargeIcon.Size;
                if (!string.IsNullOrEmpty(command.ToolTip))
                {
                    toolbarItem.ToolTip = command.ToolTip;
                }
                visual = toolbarItem;
#if false
                if (toolbar.DefaultItemIdentifiers == null)
                {
                    toolbar.DefaultItemIdentifiers = NSToolbarIdentifiers;
                }
                if (toolbar.AllowedItemIdentifiers == null)
                {
                    toolbar.AllowedItemIdentifiers = NSToolbarIdentifiers;
                }
                if (toolbar.SelectableItemIdentifiers == null)
                {
                    // WORKAROUND for bug https://bugzilla.xamarin.com/show_bug.cgi?id=21680
                    toolbar.SelectableItemIdentifiers = BugWorkaround;
                }
                var prevWillAdd = toolbar.WillInsertItem;
                toolbar.WillInsertItem = (t, i, b) => toolbarItem;
                toolbar.InsertItem(((VisualRelayCommand)command).UniqueId, insertLocation);
                toolbar.WillInsertItem = prevWillAdd;
#endif
                toolbar.AddToolbarItem(toolbarItem, ((VisualRelayCommand)command).UniqueId, insertLocation);
            }
            if (parentVisual is NSToolbarItem)
            {
                parentVisual = ((NSToolbarItem)parentVisual).View;
            }
            if (parentVisual is NSSegmentedControl)
            {
                var toolbarItem = parentCommand.Visual as NSToolbarItem;
                var segmentedControl = (NSSegmentedControl)parentVisual;
                // We don't actually try to insert at correct location, since segmented control
                // will always add new cells at the end and it's just a PITA to deal w/ generally
                // solving the shuffling of cell data at this time. Instead, always stick the new
                // cell at the end.
                insertLocation = segmentedControl.SegmentCount;
                segmentedControl.SegmentCount = insertLocation + 1;
                segmentedControl.SetImage(command.SmallIcon, insertLocation);
                segmentedControl.Cell.SetToolTip(command.Name, insertLocation);
                segmentedControl.Cell.SetTag(command.GetHashCode(), insertLocation);
                segmentedControl.SizeToFit();
                toolbarItem.MinSize = segmentedControl.Bounds.Size;
                visual = segmentedControl;
            }
            return visual;
        }

        /// <summary>
        /// Finds the index to use as an insert location into a list of commands (e.g. position in submenu) based on the command's weight.
        /// </summary>
        /// <param name="parentCommand">The parent command, which is used to locate other child commands for determining insert location.</param>
        /// <param name="commandToInsert">The command to insert.</param>
        /// <returns>The insertion index, or -1 if the command is to be placed at the end of the list.</returns>
        public static int FindInsertLocation(this VisualRelayCommand parentCommand, VisualRelayCommand commandToInsert)
        {
            int insertLocation = -1; // default to unknown
            if (parentCommand != null)
            {
                var parentVisual = parentCommand.Visual;
                if (parentVisual is NSToolbar)
                {
                    var toolbar = (NSToolbar)parentCommand.Visual;
                    var items = toolbar.Items;
                    for (int i = 0; (i < items.Length) && (insertLocation < 0); ++i)
                    {
                        var weight = (double)items[i].Tag / WeightScaleFactor;
                        if (weight > commandToInsert.Weight)
                        {
                            insertLocation = i;
                        }
                    }
                    if (insertLocation < 0)
                    {
                        insertLocation = items.Length;
                    }
                }
                if (parentVisual is NSToolbarItem)
                {
                    parentVisual = ((NSToolbarItem)parentVisual).View;
                }
                if (parentVisual is NSSegmentedControl)
                {
                    var segmentedControl = (NSSegmentedControl)parentVisual;
                    insertLocation = segmentedControl.SegmentCount;
                }
                if (insertLocation < 0)
                {
                    insertLocation = 0;
                }
            }
            return insertLocation;
        }

        /// <summary>
        /// Creates a context menu.
        /// </summary>
        /// <param name="target">The target object for which a context menu is desired.</param>
        /// <param name="menuName">The name of the menu to create.</param>
        /// <param name="context">The context used for creating the menu item.</param>
        /// <returns>The menu, containing the menu items.</returns>
        public static NSMenu CreateContextMenu(this object target, string menuName, object context)
        {
            var menu = new NSMenu(menuName);
            foreach (var command in target.GetContextMenuCommands(context).OfType<VisualRelayCommand>().OrderBy(c => c.Weight))
            {
                menu.AddItem(command.MenuItem);
                command.MenuItem.Menu.AutoEnablesItems = false;
            }
            CommandManager.InvalidateRequerySuggested(); // Ensure items in context menu properly updated
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
            var menuItem = contextCommand.CreateMenuItemForCommand(null, false, string.IsNullOrEmpty(name) ? command.ContextMenuItemName : name);
            contextCommand.MenuItem = menuItem;
            return contextCommand;
        }

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item is to be created.</param>
        /// <param name="group">The group to which the command belongs.</param>
        /// <param name="requiresParentMenu">If <c>true</c>, must be placed in a submenu.</param>
        /// <returns>The menu item for the command.</returns>
        public static NSMenuItem CreateMenuItemForCommand(this VisualRelayCommand command, ICommandGroup group, bool requiresParentMenu, string itemName)
        {
            NSMenuItem menuItem = command.MenuItem;
            if (((command.MenuParent != null) || !requiresParentMenu) && (menuItem == null))
            {
                if (command.UniqueId == RootCommandGroup.MenuSeparatorCommand.UniqueId)
                {
                    menuItem = NSMenuItem.SeparatorItem;
                }
                else if (!string.IsNullOrEmpty(command.Name))
                {
                    var name = itemName ?? command.MenuItemName;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = command.Name;
                    }
                    menuItem = new NSMenuItem(name);
                    if (!string.IsNullOrWhiteSpace(command.KeyboardShortcutKey))
                    {
                        // NOTE! InsertItem may reset key equivalent if another item already
                        // exists with the same one! Bleargh!
                        menuItem.KeyEquivalent = command.KeyboardShortcutKey;
                        var modifiers = command.KeyboardShortcutModifiers;
                        menuItem.KeyEquivalentModifierMask  = (NSEventModifierMask)modifiers;
                    }
                }
                if (menuItem != null)
                {
                    menuItem.RepresentedObject = new NSObjectWrapper<ICommand>(command);
                    if (requiresParentMenu)
                    {
                        var menuCommand = (VisualRelayCommand)command.MenuParent;
                        if (menuCommand.MenuItem == null)
                        {
                            menuCommand.MenuItem = group.CreateMenuItemForCommand(menuCommand);
                        }
                        var menu = menuCommand.MenuItem != null ? menuCommand.MenuItem.Submenu : null;
                        if ((menu == null) && (menuCommand.UniqueId == RootCommandGroup.RootMenuCommand.UniqueId))
                        {
                            menu = menuCommand.Visual as NSMenu;
                        }
                        if (menu == null)
                        {
                            var name = menuCommand.MenuItemName;
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                name = menuCommand.Name;
                            }
                            menu = new NSMenu(name);
                            menuCommand.MenuItem.Submenu = menu;
                        }
                        if (menu.Delegate == null)
                        {
                            menu.Delegate = MenuDelegate.Instance;
                        }
                        var insertLocation = command.FindMenuInsertLocation(menu);
                        menu.InsertItem(menuItem, insertLocation); // <<<< Might nuke key equivalent!
                    }
                }
            }
            return menuItem;
        }

        /// <summary>
        /// Finds the insertion location for a command within a containing menu.
        /// </summary>
        /// <param name="command">The command whose insertion point is desired.</param>
        /// <param name="menu">The menu in which the command is to be inserted.</param>
        /// <returns>The insertion index, or -1 if it should be at the end.</returns>
        public static int FindMenuInsertLocation(this VisualRelayCommand command, NSMenu menu)
        {
            int index = -1;
            var items = menu.ItemArray();
            for (int i = 0; i < menu.Count; ++i)
            {
                var item = items[i];
                var menuCommand = item.RepresentedObject as NSObjectWrapper<ICommand>;
                if (menuCommand != null)
                {
                    if (command.Weight >= ((VisualRelayCommand)menuCommand.WrappedObject).Weight)
                    {
                        index = i + 1;
                    }
                    /*
                    if (((VisualRelayCommand)menuCommand.WrappedObject).Weight > command.Weight)
                    {
                        index = i;
                    }
                    else
                    {
                        index = i + 1;
                    }
                    */
                }
            }
            if (index < 0)
            {
                index = 0; // default to top of menu
            }
            return index;
        }

        /// <summary>
        /// Adds the elements associated with a command to a NSPopUpButton.
        /// </summary>
        /// <param name="command">The command to associate with the elements in the popup button.</param>
        /// <param name="button">The button to which the command is to be added.</param>
        /// <param name="itemTitles">The user-visible names of the items to add to the popup button.</param>
        /// <param name="itemTags">The identifying information for the choices added to the popup button.</param>
        /// <param name="itemToolTips">Tool tips for the choices in the popup button.</param>
        /// <remarks>The NSPopUpButton presents a choice for the user to select. The same command is associated with each choice in the
        /// button. When the command is invoked, the Tag associated with the item in the popup button is part of the data used in executing the command.</remarks>
        public static void PopulatePopUpButton(this ICommand command, NSPopUpButton button, string[] itemTitles, int[] itemTags, string[] itemToolTips)
        {
            button.RemoveAllItems();
            button.AddItems(itemTitles);
            var menuItems = button.Items();
            for (int i = 0; i < menuItems.Length; ++i)
            {
                var menuItem = menuItems[i];
                menuItem.RepresentedObject = new NSObjectWrapper<ICommand>(command);
                menuItem.Tag = itemTags[i];
                menuItem.ToolTip = itemToolTips[i];
            }
        }

        private static string[] BugWorkaround(NSToolbar toolbar)
        {
            return Enumerable.Empty<string>().ToArray();
        }

        private static string[] NSToolbarIdentifiers(NSToolbar toolbar)
        {
            var identifiers = toolbar.VisibleItems.Select(i => i.Identifier).ToArray();
            return identifiers;
        }

        /// <summary>
        /// This method is used to initialize commands that must be associated with visuals that are not available until the
        /// NSObject.AwakeFromNib method is called.
        /// </summary>
        /// <param name="controller">The controller of the visuals that need to have commands associated with them.</param>
        public static void InitializeCommandsInAwakeFromNib(this NSViewController controller)
        {
            var commandProviders = SingleInstanceApplication.Instance.CommandProviders.Select(p => p.Value);
            foreach (var commandProvider in commandProviders)
            {
                foreach (var commandGroup in commandProvider.CommandGroups.OfType<CommandGroup>())
                {
                    commandGroup.AwakeFromNib(controller);
                }
            }
        }

        /// <summary>If the standard PerformKeyEquivalent doesn't work, then use this
        /// to try to execute the given command directly. Used as a workaround.</summary>
        /// <remarks>This is to work around an as-yet unsuccessfully diagnosed problem with the
        /// 'delete' key shortcut that arises when cancelling cell edits in the NSOutlineView / NSTableView.
        /// It seems that other shortcuts aside from delete will work. It may be that when the
        /// field editor becomes active, the cancel isn't being processed properly, or the
        /// re-enabling of keyboard shortcuts isn't happening in the right order, or something
        /// about the editor itself and its temporary requisition of the delete key is at play here.
        /// Blather aside, this will explicitly check for the backspace / delete key and treat
        /// it as if it were a command shortcut.</remarks>
        public static bool PerformKeyEquivalentForDelete(this NSResponder responder, NSEvent theEvent, VisualRelayCommand command, object context)
        {
            var didIt = false;
            if (theEvent.Window.FirstResponder == responder)
            {
                var isDeleteOrBackspace = (theEvent.Characters == NSBackspaceCharacterString) || (theEvent.Characters == NSDeleteCharacterString);
                var deleteCommandUsesBackspaceOrDelete = (command.KeyboardShortcutKey == NSBackspaceCharacterString) || (command.KeyboardShortcutKey == NSDeleteCharacterString);
                if (isDeleteOrBackspace && deleteCommandUsesBackspaceOrDelete)
                {
                    didIt = command.CanExecute(context);
                    if (didIt)
                    {
                        command.Execute(context);
                    }
                }
            }
            return didIt;
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutputIf(bool condition, object message)
        {
            System.Diagnostics.Debug.WriteLineIf(condition, message);
        }

        /// <remarks>See https://bugzilla.xamarin.com/show_bug.cgi?id=39507 for notes about
        /// a hard crash due to bad binding defined for HasKeyEquivalentForEvent().</remarks>
        private class MenuDelegate : NSMenuDelegate
        {
            private static readonly MenuDelegate _instance = new MenuDelegate();

            private MenuDelegate()
            {
            }

            /// <summary>
            /// Gets the instance of this stateless delegate.
            /// </summary>
            public static MenuDelegate Instance { get { return _instance; } }

            /// <inheritdoc/>
            public override void MenuWillOpen(NSMenu menu)
            {
                var groups = GetCommandGroups();
                var items = menu.ItemArray().Where(i => (i.RepresentedObject as NSObjectWrapper<ICommand>) != null);
                foreach (var item in items)
                {
                    var commandWrapper = item.RepresentedObject as NSObjectWrapper<ICommand>;
                    var command = commandWrapper.WrappedObject;
                    var group = GetGroupForCommand(command, groups);
                    if (group != null)
                    {
                        group.UpdateCanExecute(command);
                    }
                }
            }

            /// <inheritdoc/>
            public override void MenuWillHighlightItem(NSMenu menu, NSMenuItem item)
            {
            }

            private static IEnumerable<ICommandGroup> GetCommandGroups()
            {
                var commandProviders = SingleInstanceApplication.Instance.CommandProviders.Select(p => p.Value);
                var groups = Enumerable.Empty<ICommandGroup>();
                foreach (var provider in commandProviders)
                {
                    groups = groups.Union(provider.CommandGroups);
                }
                return groups;
            }

            private static CommandGroup GetGroupForCommand(ICommand command, IEnumerable<ICommandGroup> groups)
            {
                var commandGroup = groups.FirstOrDefault(g => g.Commands.Contains(command)) as CommandGroup;
                return commandGroup;
            }
        }
    }
}
