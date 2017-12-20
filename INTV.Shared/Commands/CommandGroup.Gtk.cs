// <copyright file="CommandGroup.Gtk.cs" company="INTV Funhouse">
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

using System;
using INTV.Shared.ComponentModel;
using INTV.Shared.View;

using OSCommandVisual = INTV.Shared.View.OSVisual;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public abstract partial class CommandGroup
    {
        /// <summary>
        /// The name of the attached command property.
        /// </summary>
        public const string AttachedCommandPropertyName = "Command";

        /// <summary>
        /// Gets the general data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public abstract object Context { get; }

        #region ICommandGroup

        /// <summary>
        /// Creates the visual for a command, if applicable.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <returns>The visual for the command.</returns>
        public virtual OSCommandVisual CreateVisualForCommand(ICommand command)
        {
            Gtk.Widget visual = null;
            var visualCommand = command as VisualRelayCommand;
            if ((visualCommand != null) && visualCommand.Visual.IsEmpty)
            {
                visual = visualCommand.CreateVisualForCommand(visualCommand.VisualParent != null);
            }
            return visual;
        }

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item must be created.</param>
        /// <returns>The menu item.</returns>
        public virtual OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            Gtk.MenuItem menuItem = null;
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand != null)
            {
                menuItem = visualCommand.CreateMenuItemForCommand(visualCommand.MenuParent != null);
            }
            return menuItem;
        }

        #endregion // ICommandGroup

        /// <summary>
        /// Creates the toolbar item for command.
        /// </summary>
        /// <param name="command">The command whose toolbar visual is created.</param>
        /// <returns>The toolbar visual for the given command.</returns>
        public virtual OSCommandVisual CreateToolbarItemForCommand(ICommand command)
        {
            Gtk.ToolItem toolItem = null;
            var visualCommand = command as VisualRelayCommand;
            if (visualCommand != null)
            {
                toolItem = visualCommand.CreateToolbarButtonForCommand();
            }
            return toolItem;
        }

        /// <summary>
        /// Gets the command data context.
        /// </summary>
        /// <param name="command">Command to get data context for.</param>
        /// <param name="context">A default value for the data context.</param>
        /// <returns>The command data context.</returns>
        internal object GetCommandContext(ICommand command, object context)
        {
            context = GetContextForCommand(null, command, context);
            return context;
        }

        /// <summary>
        /// Attaches an event handler for the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="command">The command for which a 'CanExecute' event handler should be attached.</param>
        internal virtual void AttachCanExecuteChangeHandler(RelayCommand command)
        {
            if ((command.UniqueId != RootCommandGroup.MenuSeparatorCommand.UniqueId) &&
                (command.UniqueId != RootCommandGroup.ToolbarSeparatorCommand.UniqueId))
            {
                command.CanExecuteChanged += HandleCanExecuteChanged;
            }
        }

        /// <summary>
        /// Event handler for updating the enabled state of a visual associated with an <see cref=">ICommand"/>.
        /// </summary>
        /// <param name="sender">The command whose associated visuals must be updated to reflect command availability.</param>
        /// <param name="e">Unused argument.</param>
        /// <remarks>Updates the enabled state of NSControl, NSToolbarItem and NSMenuItem visuals.</remarks>
        protected virtual void HandleCanExecuteChanged(object sender, System.EventArgs e)
        {
            var command = sender as VisualRelayCommand;
            if (command != null)
            {
                var control = command.Visual.NativeVisual;
                if (!command.Visual.IsEmpty)
                {
                    var canExecute = HandleCanExecuteChangedForCommand(command, command.Visual);
                    control.Sensitive = canExecute;
                }
                if (!command.MenuItem.IsEmpty)
                {
                    var canExecute = HandleCanExecuteChangedForCommand(command, command.MenuItem.NativeMenuItem);
                    command.MenuItem.NativeMenuItem.Sensitive = canExecute;
                }
            }
        }

        /// <summary>
        /// Gets a Boolean value indicating of the given command is allowed to execute.
        /// </summary>
        /// <param name="command">The command of interest.</param>
        /// <param name="visual">The visual with which the command is associated</param>
        /// <returns><c>true</c> if the command should be allowed to execute, <c>false</c> otherwise.</returns>
        protected virtual bool HandleCanExecuteChangedForCommand(VisualRelayCommand command, OSVisual visual)
        {
            object dataContext = null;
            if (!visual.IsEmpty)
            {
                dataContext = visual.NativeVisual.GetInheritedValue(IFakeDependencyObjectHelpers.DataContextPropertyName);
            }
            if (dataContext == null)
            {
                dataContext = GetContextForCommand(dataContext, command, Context);
            }
            return command.CanExecute(dataContext);
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
            throw new NotImplementedException("InitializeMenuItem");
            ////command.SetValue("DataContext", target ?? context);
            ////AttachActivateHandler(command, command.MenuItem);
            ////AttachCanExecuteChangeHandler(command);
        }

        private object OSGetContextForCommand(object target, ICommand command, object context)
        {
            object commandContext = null;
            var visualCommand = command as VisualRelayCommand;
            if (!visualCommand.Visual.IsEmpty)
            {
                commandContext = visualCommand.Visual.NativeVisual.GetInheritedValue(IFakeDependencyObjectHelpers.DataContextPropertyName);
            }
            if ((commandContext == null) && !visualCommand.MenuItem.IsEmpty)
            {
                commandContext = visualCommand.MenuItem.NativeMenuItem.GetInheritedValue(IFakeDependencyObjectHelpers.DataContextPropertyName);
            }
            if (commandContext == null)
            {
                commandContext = target ?? context ?? Context;
            }
            return commandContext;
        }
    }
}
