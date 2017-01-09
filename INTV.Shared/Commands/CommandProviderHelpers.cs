// <copyright file="CommandProviderHelpers.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Extension methods to assist with the implementation of command providers.
    /// </summary>
    public static partial class CommandProviderHelpers
    {
        /// <summary>
        /// Additional text to append to controls such as RibbonSplitButton to indicate more commands are available.
        /// </summary>
        public static readonly string RibbonSplitButtonExtraToolTipDescription = Resources.Strings.RibbonSplitButton_ExtraToolTipDescription;

        /// <summary>
        /// Gets the command for a given unique identifier from a specific command provider.
        /// </summary>
        /// <param name="commandProvider">The command provider to search for the unique identifier.</param>
        /// <param name="uniqueIdentifier">The unique identifier to locate.</param>
        /// <returns>The command, or <c>null</c> if none is found.</returns>
        public static ICommand GetCommandForUniqueIdentifier(this ICommandProvider commandProvider, string uniqueIdentifier)
        {
            ICommand command = null;
            foreach (var commandGroup in commandProvider.CommandGroups)
            {
                command = commandGroup.Commands.OfType<RelayCommand>().FirstOrDefault(c => c.UniqueId == uniqueIdentifier);
                if (command != null)
                {
                    break;
                }
            }
            return command;
        }

        /// <summary>
        /// Gets the command for a given unique identifier from a collection of command providers.
        /// </summary>
        /// <param name="commandProviders">The command providers to search for the unique identifier.</param>
        /// <param name="uniqueIdentifier">he unique identifier to locate.</param>
        /// <returns>The command, or <c>null</c> if none is found.</returns>
        public static ICommand GetCommandForUniqueIdentifier(this IEnumerable<ICommandProvider> commandProviders, string uniqueIdentifier)
        {
            ICommand command = null;
            foreach (var commandProvider in commandProviders)
            {
                command = commandProvider.GetCommandForUniqueIdentifier(uniqueIdentifier);
                if (command != null)
                {
                    break;
                }
            }
            return command;
        }

        /// <summary>
        /// Create a menu separator item relative to an existing command.
        /// </summary>
        /// <param name="command">The command needing a separator placed next to it.</param>
        /// <param name="location">Specifies the location of the separator relative to the command.</param>
        /// <returns>A separator pseudo-command.</returns>
        public static VisualRelayCommand CreateSeparator(this VisualRelayCommand command, CommandLocation location)
        {
            var separator = RootCommandGroup.MenuSeparatorCommand.Clone();
            var delta = (location == CommandLocation.After) ? RootCommandGroup.MenuSeparatorDelta : -RootCommandGroup.MenuSeparatorDelta;
            separator.Weight = command.Weight + delta;
            separator.MenuParent = command.MenuParent;
            return separator;
        }

        /// <summary>
        /// Get the context menu commands for a target object type.
        /// </summary>
        /// <param name="target">The target object for which a context menu is being generated.</param>
        /// <param name="context">Context used for creating the command.</param>
        /// <returns>A command to place in a context menu.</returns>
        public static IEnumerable<ICommand> GetContextMenuCommands(this object target, object context)
        {
            var commandProviders = SingleInstanceApplication.Instance == null ? null : SingleInstanceApplication.Instance.CommandProviders;
            if (commandProviders != null)
            {
                foreach (var commandProvider in commandProviders.Select(p => p.Value))
                {
                    foreach (var command in commandProvider.CreateContextMenuCommands(target, context))
                    {
                        yield return command;
                    }
                }
            }
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
    }
}
