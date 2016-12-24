// <copyright file="CommandGroup.Mac.cs" company="INTV Funhouse">
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
using MonoMac.AppKit;
using MonoMac.Foundation;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

using OSMenuItem = MonoMac.AppKit.NSMenuItem;
using OSCommandVisual = MonoMac.Foundation.NSObject;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mac-specific implementation of ICommandGroup.
    /// </summary>
    public abstract partial class CommandGroup
    {
        /// <summary>
        /// This name is used to attach additional visuals to a <see cref="VisualRelayCommand"/> beyond the stock supported visual types.
        /// </summary>
        protected static readonly string AdditionalVisualsPropertyName = "AdditionalVisuals";

        /// <summary>
        /// General data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public abstract object Context { get; }

        #region ICommandGroup

        /// <inheritdoc />
        public virtual OSCommandVisual CreateVisualForCommand(ICommand command)
        {
            var visualCommand = (VisualRelayCommand)command;
            var visual = visualCommand.CreateVisualForCommand(true);
            visual.SetValue("Command", command);
            AttachActivateHandler(visualCommand, visual);
            return visual;
        }

        /// <inheritdoc />
        public virtual OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            var visualCommand = (VisualRelayCommand)command;
            var menuItem = visualCommand.CreateMenuItemForCommand(this, true, null);
            AttachActivateHandler(visualCommand, menuItem);
            return menuItem;
        }

        #endregion // ICommandGroup

        /// <inheritdoc />
        public virtual void AwakeFromNib(NSViewController controller)
        {
            foreach (var command in CommandList.OfType<VisualRelayCommand>())
            {
                var visual = controller.GetValue(command.UniqueId) as NSObject;
                if (visual != null)
                {
                    AttachActivateHandler(command, visual);
                    if (command.Visual == null)
                    {
                        command.Visual = visual;
                    }
                    else
                    {
                        var additionalVisuals = command.GetValue(AdditionalVisualsPropertyName) as IList<object>;
                        if (additionalVisuals == null)
                        {
                            additionalVisuals = new List<object>();
                        }
                        additionalVisuals.Add(visual);
                        command.SetValue(AdditionalVisualsPropertyName, additionalVisuals);
                    }
                    visual.SetValue("Command", command);
                    var view = visual as NSView;
                    if ((view != null) && !string.IsNullOrEmpty(command.ToolTip))
                    {
                        view.ToolTip = command.ToolTip.SafeString();
                    }
                }
            }
        }

        /// <summary>
        /// Provide a hook to explicitly update the state of visuals associated with the command.
        /// </summary>
        /// <param name="command">The command whose visual states should be updated.</param>
        internal void UpdateCanExecute(ICommand command)
        {
            HandleCanExecuteChanged(command, EventArgs.Empty);
        }

        /// <summary>
        /// Attaches an event handler for the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command for which a 'CanExecute' event handler should be attached.</param>
        internal virtual void AttachCanExecuteChangeHandler(RelayCommand command)
        {
            if (command.UniqueId != RootCommandGroup.MenuSeparatorCommand.UniqueId)
            {
                command.CanExecuteChanged += HandleCanExecuteChanged;
            }
        }

        /// <summary>
        /// Detaches an event handler for the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command from which a 'CanExecute' event handler should be detached.</param>
        protected virtual void DetachCanExecuteChangedHandler(RelayCommand command)
        {
            if (command.UniqueId != RootCommandGroup.MenuSeparatorCommand.UniqueId)
            {
                command.CanExecuteChanged -= HandleCanExecuteChanged;
            }
        }

        /// <summary>
        /// Attaches an event handler for a visual's 'Activated' event.
        /// </summary>
        /// <param name="command">The command whose <see cref=">ICommand.Execute"/> method should be called from the visual's 'Activated' event handler.</param>
        /// <param name="visual">The visual that will execute the given <paramref name="command"/>.</param>
        /// <remarks>In most cases, the MonoMac bindings to controls and other visual entities have added convenience 'Activated' events. These are simpler
        /// to use in C# than the traditional Cocoa 'Action' callbacks. This stock implementation handles NSControl, NSMenuItem, and NSToolbarItem.</remarks>
        protected virtual void AttachActivateHandler(RelayCommand command, NSObject visual)
        {
            if ((visual != null) && (command.UniqueId != RootCommandGroup.MenuSeparatorCommand.UniqueId))
            {
                var menuItem = visual as NSMenuItem;
                var toolbarItem = visual as NSToolbarItem;
                var control = visual as NSControl;
                if (menuItem != null)
                {
                    menuItem.Activated += HandleCommandActivated;
                    if (menuItem.Menu != null)
                    {
                        menuItem.Menu.AutoEnablesItems = false;
                    }
                }
                else if (toolbarItem != null)
                {
                    toolbarItem.Activated += HandleCommandActivated;
                }
                else if (control != null)
                {
                    control.Activated += HandleCommandActivated;
                }
            }
        }

        /// <summary>
        /// Detaches an event handler from a visual's 'Activated' event.
        /// </summary>
        /// <param name="command">The command whose 'Activated' event handler should be detached.</param>
        /// <param name="visual">The visual that has previously had an Activated handler associated with the execution of the given <paramref name="command"/>.</param>
        protected virtual void DetachActivateHandler(RelayCommand command, NSObject visual)
        {
            if (visual != null)
            {
                var menuItem = visual as NSMenuItem;
                var toolbarItem = visual as NSToolbarItem;
                var control = visual as NSControl;
                if (menuItem != null)
                {
                    menuItem.Activated -= HandleCommandActivated;
                    if (menuItem.Menu != null)
                    {
                        menuItem.Menu.AutoEnablesItems = false;
                    }
                }
                else if (toolbarItem != null)
                {
                    toolbarItem.Activated -= HandleCommandActivated;
                }
                else if (control != null)
                {
                    control.Activated -= HandleCommandActivated;
                }
            }
        }

        /// <summary>
        /// Event handler for updating the enabled state of a visual associated with an <see cref=">ICommand"/>.
        /// </summary>
        /// <param name="sender">The command whose associated visuals must be updated to reflect command availability.</param>
        /// <param name="e">Unused argument.</param>
        /// <remarks>Updates the enabled state of NSControl, NSToolbarItem and NSMenuItem visuals.</remarks>
        protected virtual void HandleCanExecuteChanged(object sender, EventArgs e)
        {
            var command = sender as VisualRelayCommand;
            if (command != null)
            {
                var canExecute = HandleCanExecuteChangedForCommand(command);
                var toolbarItem = command.Visual as NSToolbarItem;
                var control = command.Visual as NSControl;
                if (toolbarItem != null)
                {
                    toolbarItem.Enabled = canExecute;
                    toolbarItem.ToolTip = command.ToolTip.SafeString();
                }
                else if (control != null)
                {
                    control.Enabled = canExecute;
                    control.ToolTip = command.ToolTip.SafeString();
                }
                var menuItem = command.MenuItem;
                if (menuItem != null)
                {
                    menuItem.Enabled = canExecute;
                    menuItem.ToolTip = command.ToolTip.SafeString();
                }
                var additionalVisuals = command.GetValue(AdditionalVisualsPropertyName) as IList<object>;
                if (additionalVisuals != null)
                {
                    foreach(var additionalControl in additionalVisuals.OfType<NSControl>())
                    {
                        additionalControl.Enabled = canExecute;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a Boolean value indicating of the given command is allowed to execute.
        /// </summary>
        /// <param name="command">The command of interest.</param>
        /// <returns><c>true</c> if the command should be allowed to execute, <c>false</c> otherwise.</returns>
        protected virtual bool HandleCanExecuteChangedForCommand(VisualRelayCommand command)
        {
            return command.CanExecute(GetContextForCommand(command.GetValue("DataContext"), command, Context));
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="sender">A visual object associated with an <c>VisualRelayCommand</c>.</param>
        /// <param name="e">Unused argument.</param>
        protected virtual void HandleCommandActivated(object sender, EventArgs e)
        {
            var command = GetCommand(sender);
            command.Execute(GetContextForCommand(command.GetValue("DataContext"), command, Context));
        }

        /// <summary>
        /// Gets the command associated with the sender object, which is usually an NSObject-derived visual of some sort.
        /// </summary>
        /// <param name="sender">The visual or other entity that refers to a command.</param>
        /// <returns>The command, or <c>null</c> if none is found.</returns>
        /// <remarks>If <paramref name="sender"/> is of type <see cref="NSToolbarItem"/>, the command is retrieved via the <see cref="NSToolbarItem.Identifier"/> operating
        /// on the assumption that the value was set from <see cref=">RelayCommand.UniqueId"/>. If <paramref name="sender"/> is of type <see cref=">NSMenuItem"/>,
        /// the implementation will retrieve the command via <see cref="NSMenuItem.ReprsentedObject"/>. Finally, if neither of those succeed, the generic
        /// 'fake attached property' approach will be tried.</remarks>
        protected virtual ICommand GetCommand(object sender)
        {
            ICommand command = null;
            if (sender is NSToolbarItem)
            {
                command = CommandList.OfType<RelayCommand>().First(c => c.UniqueId == ((NSToolbarItem)sender).Identifier);
            }
            else if (sender is NSMenuItem)
            {
                command = ((NSObjectWrapper<ICommand>)((NSMenuItem)sender).RepresentedObject).WrappedObject;
            }
            if (command == null)
            {
                command = sender.GetValue("Command") as ICommand;
            }
            return command;
        }

        /// <summary>
        /// Perform platform- and group-specific initialization for a command's menu item.
        /// </summary>
        /// <param name="command">The command whose menu item may need additional setup.</param>
        /// <param name="target">The target of the command.</param>
        /// <param name="context">The general command context.</param>
        /// <remarks>This should probably be renamed -- it's only used specifically for context menus.</remarks>
        protected virtual void InitializeMenuItem(VisualRelayCommand command, object target, object context)
        {
            command.SetValue("DataContext", target ?? context);
            AttachActivateHandler(command, command.MenuItem);
            AttachCanExecuteChangeHandler(command);
        }

        private object OSGetContextForCommand(object target, ICommand command, object context)
        {
            var commandContext = command.GetValue("DataContext") ?? target ?? context ?? Context;
            return commandContext;
        }
    }
}
