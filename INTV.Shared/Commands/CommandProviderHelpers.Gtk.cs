// <copyright file="CommandProviderHelpers.Gtk.cs" company="INTV Funhouse">
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

using System.Collections;
using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// GTK-specific implementation
    /// </summary>
    public static partial class CommandProviderHelpers
    {
        /// <summary>
        /// Delete character.
        /// </summary>
        public const char GtkDeleteCharacter = (char)Gdk.Key.Delete;

        /// <summary>
        /// GtkDeleteCharacter as a string.
        /// </summary>
        public static readonly string GtkDeleteCharacterString = new string(GtkDeleteCharacter, 1);

        /// <summary>
        /// Backspace character.
        /// </summary>
        public const char GtkBackspaceCharacter = (char)Gdk.Key.BackSpace;

        /// <summary>
        /// GtkBackspaceCharacter as a string.
        /// </summary>
        public static readonly string GtkBackspaceCharacterString = new string(GtkBackspaceCharacter, 1);

        /// <summary>
        /// Adds an accelerator given a command with keyboard shortcut information defined.
        /// </summary>
        /// <param name="command">The command containing a keyboard shortcut.</param>
        /// <param name="visual">The visual for which to register the accelerator.</param>
        /// <remarks>If the visual is null, the shortcut will be registered using the
        /// current application's MainWindow. In any case, all accelerators are placed in
        /// the first accelerator group associated with the application's main window.</remarks>
        public static void AddAccelerator(this VisualRelayCommand command, Gtk.Widget visual)
        {
            var accelKey = command.GetAcceleratorKey();
            command.AddAccelerator(visual, accelKey);
        }

        /// <summary>
        /// Adds an accelerator given a command with keyboard shortcut information defined.
        /// </summary>
        /// <param name="command">For convenience - not actually used.</param>
        /// <param name="visual">The visual for which to register the accelerator.</param>
        /// <param name="key">The keyboard shortcut.</param>
        /// <remarks>If the visual is null, the shortcut will be registered using the
        /// current application's MainWindow. In any case, all accelerators are placed in
        /// the first accelerator group associated with the application's main window.</remarks>
        public static void AddAccelerator(this VisualRelayCommand command, Gtk.Widget visual, Gtk.AccelKey key)
        {
            var mainWindow = SingleInstanceApplication.Instance.MainWindow;
            visual = visual ?? mainWindow;
            var groups = Gtk.Accel.GroupsFromObject(mainWindow);
            var createAccelGroup = (groups == null) || (groups.Length < 1);
            var accelGroup = createAccelGroup ? new Gtk.AccelGroup() : groups[0];
            if (createAccelGroup)
            {
                mainWindow.AddAccelGroup(accelGroup);
            }
            visual.AddAccelerator("activate", accelGroup, (uint)key.Key, key.AccelMods, key.AccelFlags);
        }

        /// <summary>
        /// Removes an accelerator from all accelerator groups associated with the main window.
        /// </summary>
        /// <param name="command">For convenience - not actually used.</param>
        /// <param name="visual">The visual from which to remove the accelerator.</param>
        /// <param name="key">The keyboard shortcut.</param>
        /// <remarks>If the visual is null, the shortcut will be registered using the
        /// current application's MainWindow.</remarks>
        public static void RemoveAccelerator(this VisualRelayCommand command, Gtk.Widget visual, Gtk.AccelKey key)
        {
            var mainWindow = SingleInstanceApplication.Instance.MainWindow;
            visual = visual ?? mainWindow;
            var groups = Gtk.Accel.GroupsFromObject(mainWindow);
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    visual.RemoveAccelerator(group, (uint)key.Key, key.AccelMods);
                }
            }
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
                name = name ?? command.ContextMenuItemName;
                name = name ?? command.MenuItemName;
                name = name ?? command.Name;
                var menuItem = new Gtk.MenuItem(name ?? command.ContextMenuItemName);
                if (command.SmallIcon != null)
                {
                    throw new System.NotImplementedException("CreateContextMenuItemCommand with Image");
                    //menuItem.Icon = new Image() { Source = command.SmallIcon };
                }
                throw new System.NotImplementedException("CreateContextMenuItemCommand wire up command to MenuItem");
//                menuItem.Command = command;
                contextCommand.MenuItem = menuItem;
            }
            else
            {
                contextCommand.MenuItem = new Gtk.SeparatorMenuItem();
            }
            return contextCommand;
        }

        /// <summary>
        /// Creates a visual for a command.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent visual.</param>
        /// <returns>The visual for the command.</returns>
        public static Gtk.Widget CreateVisualForCommand(this VisualRelayCommand command, bool requiresParentCommand)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(command.UniqueId), "Command's UniqueId is not defined.");
            Gtk.Widget visual = null;
            var parentCommand = (VisualRelayCommand)command.VisualParent;
            if (parentCommand != null)
            {
                var parentVisual = parentCommand.Visual.NativeVisual;
                if (parentVisual is Gtk.MenuBar)
                {
                    throw new System.InvalidOperationException("This should be done by CreateMenuItem!");
                    // Make a submenu for the command, and add the menu item to the menubar.
                    //var menu = new Gtk.Menu() { Name = command.UniqueId };
                    //var menuItem = new Gtk.MenuItem(command.MenuItemName) { Name = command.UniqueId, Submenu = menu };
                    //((Gtk.MenuBar)parentVisual).Add(menuItem);
                    //visual = menuItem;
                }
                if (parentVisual is Gtk.Toolbar)
                {
                    // The default toolbar command visual will be a button. If a different visual is desired,
                    // the CommandGroup should override visual creation for that command in ICommandGroup.CreateVisualForCommand().
                    var toolbar = parentVisual as Gtk.Toolbar;
                    Gtk.ToolItem toolbarItem;
                    if (command.UniqueId == RootCommandGroup.ToolbarSeparatorCommand.UniqueId)
                    {
                        toolbarItem = new Gtk.SeparatorToolItem();
                    }
                    else
                    {
                        var group = command.GetCommandGroup() as CommandGroup;
                        toolbarItem = group.CreateToolbarItemForCommand(command).AsType<Gtk.ToolItem>();
                    }
                    var insertLocation = FindInsertLocation(parentCommand, command, false);
                    toolbar.Insert(toolbarItem, insertLocation); // if <0, appends
                    visual = toolbarItem;
                }
                if (parentCommand.Visual.IsEmpty)
                {
                    var group = parentCommand.GetCommandGroup();
                    if (group != null)
                    {
                        parentCommand.Visual = group.CreateVisualForCommand(parentCommand);
                    }
                }

                ErrorReporting.ReportErrorIf(requiresParentCommand && (parentCommand.Visual.IsEmpty), "Failed to create parent visual for command: " + command.Name + "(" + command.UniqueId + ")");
                parentVisual = parentCommand.Visual;
            }

            DebugOutputIf(requiresParentCommand && (parentCommand == null), "No parent visual for command: " + command.Name + "(" + command.UniqueId + ")");
            if (visual != null)
            {
                visual.SetValue(CommandGroup.AttachedCommandPropertyName, command);
                var group = command.GetCommandGroup() as CommandGroup;
                if (group != null)
                {
                    var context = group.GetCommandContext(command, null);
                    visual.SetValue(IFakeDependencyObjectHelpers.DataContextPropertyName, context);
                    group.AttachCanExecuteChangeHandler(command);
                }
            }
            return visual;
        }

        /// <summary>
        /// Initializes the toolbar button for a command.
        /// </summary>
        /// <param name="command">The command to associate with the button.</param>
        /// <param name="toolbarButton">The Gtk.ToolButton to associate with the given command.</param>
        /// <param name="addCommandHandler">If set to <c>true</c> add command handler for button click to execute the command.</param>
        public static void InitializeToolbarButtonForCommand(this VisualRelayCommand command, Gtk.ToolButton toolbarButton, bool addCommandHandler)
        {
            toolbarButton.IsImportant = false;
            if (addCommandHandler)
            {
                toolbarButton.Clicked += HandleCommand;
            }
            var accelerator = string.Empty;
            if (!string.IsNullOrEmpty(command.KeyboardShortcutKey))
            {
                var accelKey = command.GetAcceleratorKey();
                accelerator = "  (" + Gtk.Accelerator.GetLabel((uint)accelKey.Key, accelKey.AccelMods) + ')';
            }
            toolbarButton.TooltipMarkup = string.Format("<span weight=\"bold\">{0}</span><span alpha=\"15%\">{1}</span>\n\n{2}", RemoveTrailingEllipses(command.Name), accelerator, command.ToolTipDescription);
        }

        /// <summary>
        /// Create a toolbar button for a command.
        /// </summary>
        /// <param name="command">The command for which a toolbar button is to be created.</param>
        /// <returns>The toolbar button for the command.</returns>
        public static Gtk.ToolButton CreateToolbarButtonForCommand(this VisualRelayCommand command)
        {
            var toolbarButton = new Gtk.ToolButton(new Gtk.Image(command.LargeIcon), command.Name);
            command.InitializeToolbarButtonForCommand(toolbarButton, true);
            return toolbarButton;
        }

        /// <summary>
        /// Create a menu toolbar button for a command.
        /// </summary>
        /// <param name="command">The command for which to create a menu tool button.</param>
        /// <param name="useDefaultCommandHandlerForButton">If set to <c>true</c> clicking the button will associate the button's click handler with the given command.</param>
        /// <param name="updateMenuOnShow">If not null, this delegate will execute when displaying the menu button's menu. It is intended to populate the menu when menu contents may be dynamic.</param>
        /// <returns>The menu tool button for the command.</returns>
        public static Gtk.MenuToolButton CreateMenuToolButtonForCommand(this VisualRelayCommand command, bool useDefaultCommandHandlerForButton, System.Action<Gtk.Menu> updateMenuOnShow)
        {
            var menuToolButton = new Gtk.MenuToolButton(new Gtk.Image(command.LargeIcon), command.Name);
            command.InitializeToolbarButtonForCommand(menuToolButton, useDefaultCommandHandlerForButton);
            var menu = new Gtk.Menu(); // { Name = command.UniqueId };
            menuToolButton.Menu = menu;
            if (updateMenuOnShow != null)
            {
                menuToolButton.ShowMenu += (sender, e) => updateMenuOnShow(menu);
            }
            return menuToolButton;
        }

        /// <summary>
        /// Create a toolbar separator relative to an existing command.
        /// </summary>
        /// <param name="command">The command needing a separator placed next to it.</param>
        /// <param name="location">Specifies the location of the separator relative to the command.</param>
        /// <returns>The toolbar separator pseudo-command.</returns>
        public static VisualRelayCommand CreateToolbarSeparator(this VisualRelayCommand command, CommandLocation location)
        {
            var separator = RootCommandGroup.ToolbarSeparatorCommand.Clone();
            var delta = (location == CommandLocation.After) ? RootCommandGroup.MenuSeparatorDelta : -RootCommandGroup.MenuSeparatorDelta;
            separator.Weight = command.Weight + delta;
            separator.VisualParent = command.VisualParent;
            return separator;
        }

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item is to be created.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent menu item.</param>
        /// <returns>The menu item for the command.</returns>
        public static Gtk.MenuItem CreateMenuItemForCommand(this VisualRelayCommand command, bool requiresParentCommand)
        {
            return command.CreateMenuItemForCommand(Gtk.StockItem.Zero, requiresParentCommand);
        }

        /// <summary>
        /// Creates a menu item for command.
        /// </summary>
        /// <param name="command">The command for which a menu item is to be created.</param>
        /// <param name="stockItem">If non-Zero, defines the stock item to use for the command.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent menu item.</param>
        /// <param name="menuItemType">A specific type of menu item to use.</param>
        /// <returns>The menu item for the command.</returns>
        public static Gtk.MenuItem CreateMenuItemForCommand(this VisualRelayCommand command, Gtk.StockItem stockItem, bool requiresParentCommand, System.Type menuItemType = null, object stateData = null)
        {
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(command.UniqueId), "Command's UniqueId is not defined.");
            Gtk.MenuItem menuItemVisual = null;

            // Menu items must have a valid parent command already defined.
            var parentCommand = (VisualRelayCommand)command.MenuParent;
            if (parentCommand != null)
            {
                Gtk.MenuShell parentMenuShell = null;
                if (parentCommand.MenuItem.IsEmpty)
                {
                    parentMenuShell = parentCommand.Visual.AsType<Gtk.MenuShell>();
                }
                else
                {
                    parentMenuShell = parentCommand.MenuItem.NativeMenuItem.Submenu as Gtk.MenuShell;
                }
                if (parentMenuShell == null)
                {
                    var group = parentCommand.GetCommandGroup();
                    if (group != null)
                    {
                        if (parentCommand.MenuItem.IsEmpty)
                        {
                            parentCommand.MenuItem = group.CreateMenuItemForCommand(parentCommand);
                        }
                        parentMenuShell = new Gtk.Menu() { Name = parentCommand.UniqueId };
                        parentCommand.Visual = parentMenuShell;
                        parentCommand.MenuItem.NativeMenuItem.Submenu = parentMenuShell;
                    }
                }
                menuItemVisual = command.CreateMenuItem(parentMenuShell, parentCommand, stockItem, menuItemType, stateData);
            }
            return menuItemVisual;
        }

        /// <summary>
        /// Creates a menu item for command.
        /// </summary>
        /// <param name="command">The command for which a menu item is to be created.</param>
        /// <param name="parentMenu">The menu to put the menu item into.</param>
        /// <param name="parentCommand">Parent command for the given command.</param>
        /// <param name="stockItem">If non-Zero, defines the stock item to use for the command.</param>
        /// <param name="requiresParentCommand">If <c>true</c>, this command requires a parent menu item.</param>
        /// <param name="menuItemType">A specific type of menu item to use.</param>
        /// <returns>The menu item for the command.</returns>
        public static OSMenuItem CreateMenuItem(this VisualRelayCommand command, Gtk.MenuShell parentMenu, VisualRelayCommand parentCommand, Gtk.StockItem stockItem, System.Type menuItemType = null, object stateData = null)
        {
            var useDefaultMenuHandler = true;
            Gtk.MenuItem menuItemVisual;

            if (command.UniqueId == RootCommandGroup.MenuSeparatorCommand.UniqueId)
            {
                menuItemVisual = new Gtk.SeparatorMenuItem();
                useDefaultMenuHandler = false;
            }
            else if (menuItemType == typeof(Gtk.CheckMenuItem))
            {
                var checkMenuItem = new Gtk.CheckMenuItem(command.MenuItemName);
                checkMenuItem.Active = (bool)stateData;
                menuItemVisual = checkMenuItem;
                useDefaultMenuHandler = false;
            }
            else if ((command.SmallIcon != null) || (stockItem.StockId != Gtk.StockItem.Zero.StockId))
            {
                if (stockItem.StockId == Gtk.StockItem.Zero.StockId)
                {
                    menuItemVisual = new Gtk.ImageMenuItem(command.MenuItemName) { Image = new Gtk.Image(command.SmallIcon) };
                    if (Properties.Settings.Default.EnableMenuIcons)
                    {
                        // NOTE: This causes some internal errors if we try to draw stock items. So don't do it.
                        menuItemVisual.ExposeEvent += ImageMenuItemHackExposeEvent;
                    }
                }
                else
                {
                    menuItemVisual = new Gtk.ImageMenuItem(stockItem.StockId, null);
                }
            }
            else
            {
                menuItemVisual = new Gtk.MenuItem(command.MenuItemName);
            }

            menuItemVisual.Name = command.UniqueId;
            menuItemVisual.SetValue(CommandGroup.AttachedCommandPropertyName, command);
            var submenu = command.Visual.AsType<Gtk.Menu>();
            if ((submenu == null) && (parentMenu is Gtk.MenuBar) && command.Visual.IsEmpty)
            {
                submenu = new Gtk.Menu() { Name = parentCommand.UniqueId };
                command.Visual = submenu;
            }
            menuItemVisual.Submenu = submenu;

            var insertLocation = FindInsertLocation(parentCommand, command, true);
            if ((insertLocation < 0) || (insertLocation >= parentMenu.Children.Length))
            {
                parentMenu.Append(menuItemVisual);
            }
            else
            {
                parentMenu.Insert(menuItemVisual, insertLocation);
            }

            if (useDefaultMenuHandler && (menuItemVisual.Submenu == null))
            {
                menuItemVisual.Activated += HandleCommand;
            }
            if (!string.IsNullOrEmpty(command.KeyboardShortcutKey))
            {
                command.AddAccelerator(menuItemVisual, command.GetAcceleratorKey());
            }
            menuItemVisual.ShowAll(); // because programmatically created, need to show
            //DebugOutputIf(requiresParentCommand && (parentCommand.MenuItem.IsEmpty) && (parentCommand.Visual.IsEmpty), "Failed to create parent menu item for command: " + command.Name + "(" + command.UniqueId + ")");

            //DebugOutputIf(requiresParentCommand && (parentCommand == null) && (parentCommand.Visual.IsEmpty), "No parent menu item for command: " + command.Name + "(" + command.UniqueId + ")");
            if (menuItemVisual != null)
            {
                var group = command.GetCommandGroup() as CommandGroup;
                if (group != null)
                {
                    var context = group.GetCommandContext(command, null);
                    menuItemVisual.SetValue(IFakeDependencyObjectHelpers.DataContextPropertyName, context);
                    group.AttachCanExecuteChangeHandler(command);
                }
            }
            return menuItemVisual;
        }

        /// <summary>
        /// TOTAL HACK to get menu item icons to draw -- at least in Ubuntu 16.04.2.
        /// </summary>
        /// <param name="o">The menu item to draw.</param>
        /// <param name="args">The event arguments.</param>
        /// <remarks>GOOD LUCK TRYING TO DEBUG THIS! Every time you hit a breakoint in MonoDevelop in
        /// this function (at least in a VMware image), it seems the entire Window Manager hangs!</remarks>
        private static void ImageMenuItemHackExposeEvent (object o, Gtk.ExposeEventArgs args)
        {
            var imageMenuItem = o as Gtk.ImageMenuItem;
            var image = imageMenuItem == null ? null : imageMenuItem.Image as Gtk.Image;
            if ((imageMenuItem != null) && (image != null) && (image.Pixbuf != null))
            {
                var mainGraphicsContext = imageMenuItem.Style.ForegroundGCs[(int)Gtk.StateType.Normal];
                var drawRect = args.Event.Area;
                var imageLeft = drawRect.Left + 4;
                var imageTop = drawRect.Top + ((drawRect.Height - image.Pixbuf.Height) / 2);
                args.Event.Window.DrawPixbuf(mainGraphicsContext, image.Pixbuf, 0, 0, imageLeft, imageTop, -1, -1, Gdk.RgbDither.None, 0, 0);
            }
        }

        private static void HandleCommand(object sender, System.EventArgs e)
        {
            var widget = sender as Gtk.Widget;
            var command = widget.GetValue<ICommand>(CommandGroup.AttachedCommandPropertyName);
            var dataContext = widget.GetInheritedValue(IFakeDependencyObjectHelpers.DataContextPropertyName);
            command.Execute(dataContext);
        }

        private static IList GetItemVisuals(Gtk.Widget visual)
        {
            var itemsControl = false ? visual : null; //visual as ItemsControl;
            IList items = null;
            if (itemsControl != null)
            {
                // items = itemsControl.Items;
            }
            else
            {
                var panel = visual as Gtk.Container;
                if (panel != null)
                {
                     items = panel.Children;
                }
            }
            return items;
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
//                var parentVisual = useMenuItem ? parentCommand.MenuItem.NativeMenuItem.Submenu : parentCommand.Visual.NativeVisual;
                var parentVisual = useMenuItem ? parentCommand.Visual : parentCommand.Visual;
                var items = GetItemVisuals(parentVisual);
                if (items != null)
                {
                    var itemCommands = items.Cast<Gtk.Widget>().Select(i => i.GetValue<VisualRelayCommand>(CommandGroup.AttachedCommandPropertyName)).ToList();
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
            var commandProviders = INTV.Shared.Utility.SingleInstanceApplication.Instance.CommandProviders.Select(p => p.Value);
            var groups = commandProviders.SelectMany(p => p.CommandGroups);
            var group = groups.FirstOrDefault(g => g.Commands.Contains(command));
            return group;
        }

        private static string RemoveTrailingEllipses(string text)
        {
            var index = text.LastIndexOf("...");
            if (index > 0)
            {
                text = text.Substring(0, index);
            }
            return text;
        }

        private static Gtk.AccelKey GetAcceleratorKey(this VisualRelayCommand command)
        {
            ErrorReporting.ReportErrorIf(command.KeyboardShortcutKey.Length != 1, "Invalid keyboard shortcut!");
            var modifiers = (Gdk.ModifierType)command.KeyboardShortcutModifiers;
            if (char.IsUpper(command.KeyboardShortcutKey[0]))
            {
                modifiers |= Gdk.ModifierType.ShiftMask;
            }
            var key = (Gdk.Key)command.KeyboardShortcutKey.ToCharArray()[0]; // totes hacky!
            var acceleratorKey = new Gtk.AccelKey(key, modifiers, Gtk.AccelFlags.Visible);
            return acceleratorKey;
        }
    }
}
