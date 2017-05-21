// <copyright file="CommandGroup.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Shared.ComponentModel;

#if WIN
using OSImage = System.Windows.Media.Imaging.BitmapImage;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Provides a partial implementation of the ICommandGroup interface.
    /// </summary>
    public abstract partial class CommandGroup : ICommandGroup
    {
        /// <summary>
        /// Initializes a new instance of the CommandGroup class.
        /// </summary>
        /// <param name="uniqueName">The unique name of the command group.</param>
        protected CommandGroup(string uniqueName)
            : this(uniqueName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandGroup class.
        /// </summary>
        /// <param name="uniqueName">The unique name of the command group.</param>
        /// <param name="name">The UI-visible name of the command group.</param>
        protected CommandGroup(string uniqueName, string name)
            : this(uniqueName, name, 0.5)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandGroup class.
        /// </summary>
        /// <param name="uniqueName">The unique name of the command group.</param>
        /// <param name="name">The UI-visible name of the command group.</param>
        /// <param name="weight">The weight of the command group in the range [0.0 - 1.0].</param>
        protected CommandGroup(string uniqueName, string name, double weight)
            : this(uniqueName, name, weight, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandGroup class.
        /// </summary>
        /// <param name="uniqueName">The unique name of the command group.</param>
        /// <param name="name">The UI-visible name of the command group.</param>
        /// <param name="weight">The weight of the command group in the range [0.0 - 1.0].</param>
        /// <param name="icon">The icon to represent the command group.</param>
        protected CommandGroup(string uniqueName, string name, double weight, OSImage icon)
        {
            UniqueName = uniqueName;
            Name = name;
            Weight = weight;
            Icon = icon;
            CommandList = new List<ICommand>();
        }

        #region Properties

        #region ICommandGroup Properties

        /// <inheritdoc />
        public string TabUniqueName { get; set; }

        /// <inheritdoc />
        public string TabName { get; set; }

        /// <inheritdoc />
        public string UniqueName { get; private set; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public OSImage Icon { get; private set; }

        /// <inheritdoc />
        public double Weight { get; private set; }

        /// <inheritdoc />
        public IEnumerable<ICommand> Commands
        {
            get
            {
                if (CommandList.Count == 0)
                {
                    AddCommands();
                }
                return CommandList;
            }
        }

        #endregion // ICommandGroup

        /// <summary>
        /// Gets the concretely typed property for the Commands property.
        /// </summary>
        protected IList<ICommand> CommandList { get; private set; }

        #endregion // Properties

        #region ICommandGroup

        /// <inheritdoc />
        public virtual IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            return Enumerable.Empty<ICommand>();
        }

        #endregion // ICommandGroup

        /// <summary>
        /// Creates a version of the given command for use in a context menu.
        /// </summary>
        /// <param name="target">The target for the command.</param>
        /// <param name="command">The command to clone for use in a context menu.</param>
        /// <param name="context">Context for the command.</param>
        /// <param name="name">Overriding name for the command; if <c>null</c> or empty, use command's ContextMenuItemName.</param>
        /// <param name="weight">Weight for the command. If this value is double.NaN, the weight is the same as the original command's weight.</param>
        /// <returns>The command to use in a context menu.</returns>
        public virtual ICommand CreateContextMenuCommand(object target, ICommand command, object context, string name = null, double weight = double.NaN)
        {
            var visualCommand = (VisualRelayCommand)command;
            var contextCommand = visualCommand.CreateContextMenuItemCommand(name, weight);
            InitializeMenuItem(contextCommand, target, context);
            return contextCommand;
        }

        /// <summary>
        /// Gets the context for a command.
        /// </summary>
        /// <param name="target">The target for the command.</param>
        /// <param name="command">The command for which context is to be retrieved.</param>
        /// <param name="context">Context for the command.</param>
        /// <returns>The context data for the command.</returns>
        /// <remarks>The returned value is usually passed as the parameter to <see cref="ICommand.CanExecute"/> or <see cref="ICommand.Execute"/> methods.
        /// The default implementation simply returns <paramref name="context"/>.</remarks>
        protected virtual object GetContextForCommand(object target, ICommand command, object context)
        {
            return OSGetContextForCommand(target, command, context);
        }

        /// <summary>
        /// Implement this method to populate the CommandsList, and thus provide the Commands for the group.
        /// </summary>
        protected abstract void AddCommands();
    }
}
