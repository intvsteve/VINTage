// <copyright file="CommandProvider.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Partial implementation of the ICommandProvider interface.
    /// </summary>
    public abstract partial class CommandProvider : ICommandProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Commands.CommandProvider"/> class.
        /// </summary>
        /// <param name="name">Name of the command provider.</param>
        protected CommandProvider(string name)
        {
            Name = name;
            Groups = new List<ICommandGroup>();
        }

        #endregion // Constructors

        #region ICommandProvider

        /// <inheritdoc/>
        public string Name
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public abstract double Weight { get; }

        /// <inheritdoc/>
        public IEnumerable<ICommandGroup> CommandGroups
        {
            get { return Groups; }
        }

        /// <inheritdoc/>
        public IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            foreach (var group in Groups)
            {
                foreach (var command in group.CreateContextMenuCommands(target, context))
                {
                    yield return command;
                }
            }
        }

        #endregion // ICommandProvider

        /// <summary>
        /// Gets the <see cref="ICommandGroup"/> instances that belong to this command provider.
        /// </summary>
        protected IList<ICommandGroup> Groups
        {
            get;
            private set;
        }
    }
}
