// <copyright file="ControllerKeyValidationFlags.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using INTV.Core.Model.Device;

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// Controller key validation flags used to describe the health of a key combination.
    /// </summary>
    [System.Flags]
    public enum ControllerKeyValidationFlags
    {
        /// <summary>No keys were set.</summary>
        None = 0,

        /// <summary>Only one key is selected.</summary>
        OnlyOneKeySet = 1 << 0,

        /// <summary>Too many keys are selected.</summary>
        TooManyKeysSet = 1 << 1,

        /// <summary>Key combination matches one reserved by the system (e.g. the pause code).</summary>
        ReservedBySystem = 1 << 2,

        /// <summary>The key combination aliases a controller disc direction combined with one or more of the action keys.</summary>
        AliasesDiscPositionPlusActionKey = 1 << 3,

        /// <summary>The key combination is the same as the one used to reset the console.</summary>
        MatchesResetConsole = 1 << 24,

        /// <summary>The key combination is the same as the one used to reset to the LTO Flash! onboard configuration menu.</summary>
        MatchesResetToMenu = 1 << 25,

        /// <summary>A mask to use to check for 'matching' flags.</summary>
        MatchesOthersMask = MatchesResetConsole | MatchesResetToMenu,

        /// <summary>Indicates that the combination is an exact bit pattern match with another key combination.</summary>
        ExactHardwareMatch = 1 << 31,
    }

    /// <summary>
    /// Controller key validation flags helper methods.
    /// </summary>
    public static class ControllerKeyValidationFlagsHelpers
    {
        // TODO: Put ControllerKeyValidationFlags strings into resources.
        private static readonly Dictionary<ControllerKeyValidationFlags, string> DisplayNames = new Dictionary<ControllerKeyValidationFlags, string>()
        {
            { ControllerKeyValidationFlags.None, Resources.Strings.KeyCombination_Valid },
            { ControllerKeyValidationFlags.OnlyOneKeySet, Resources.Strings.KeyCombination_OnlyOneKeySet },
            { ControllerKeyValidationFlags.TooManyKeysSet, Resources.Strings.KeyCombination_TooManyKeysSet },
            { ControllerKeyValidationFlags.ReservedBySystem, Resources.Strings.KeyCombination_ReservedBySystem },
            { ControllerKeyValidationFlags.AliasesDiscPositionPlusActionKey, Resources.Strings.KeyCombination_AliasesDiscPositionPlusActionKey_Format },
            { ControllerKeyValidationFlags.MatchesResetConsole, Resources.Strings.KeyCombination_MatchesResetConsole },
            { ControllerKeyValidationFlags.MatchesResetToMenu, Resources.Strings.KeyCombination_MatchesResetToMenu },
            { ControllerKeyValidationFlags.ExactHardwareMatch, Resources.Strings.KeyCombination_ExactHardwareMatch },
        };

        /// <summary>
        /// Produces a user-friendly string describing the validity of a keypad combination.
        /// </summary>
        /// <param name="flags">The flags to convert to a string.</param>
        /// <param name="keys">The key combination.</param>
        /// <param name="fromWhere">Which specific key combination the validation flags pertain to.</param>
        /// <param name="alias">Used if a keypad combination aliases a disc with action key.</param>
        /// <returns>The display string.</returns>
        public static string ToDisplayString(this ControllerKeyValidationFlags flags, IEnumerable<ControllerKeys> keys, ControllerKeyValidationFlags fromWhere, IEnumerable<ControllerKeys> alias)
        {
            var flagDisplayString = new StringBuilder();
            if (flags == ControllerKeyValidationFlags.None)
            {
                var description = keys.Any() ? string.Format("{0} [{1}]", DisplayNames[ControllerKeyValidationFlags.None], keys.ToDisplayString()) : Resources.Strings.KeyCombination_NotSet;
                switch (fromWhere)
                {
                    case ControllerKeyValidationFlags.MatchesResetConsole:
                        flagDisplayString.AppendFormat(Resources.Strings.KeyCombination_ResetConsole_Format, description);
                        break;
                    case ControllerKeyValidationFlags.MatchesResetToMenu:
                        flagDisplayString.AppendFormat(Resources.Strings.KeyCombination_ResetToMenu_Format, description);
                        break;
                    default:
                        flagDisplayString.Append(DisplayNames[ControllerKeyValidationFlags.None]);
                        break;
                }
            }
            else
            {
                var flagsToCheck = new[]
                {
                    ControllerKeyValidationFlags.OnlyOneKeySet,
                    ControllerKeyValidationFlags.TooManyKeysSet,
                    ControllerKeyValidationFlags.ReservedBySystem,
                    ControllerKeyValidationFlags.AliasesDiscPositionPlusActionKey,
                    ControllerKeyValidationFlags.MatchesResetConsole,
                    ControllerKeyValidationFlags.MatchesResetToMenu,
                };
                var appendLine = false;
                foreach (var flagToCheck in flagsToCheck)
                {
                    if (flags.HasFlag(flagToCheck))
                    {
                        var info = DisplayNames[flagToCheck];
                        if (flagToCheck == ControllerKeyValidationFlags.AliasesDiscPositionPlusActionKey)
                        {
                            info = string.Format(info, alias.ToDisplayString());
                        }
                        if (appendLine)
                        {
                            flagDisplayString.AppendLine();
                        }
                        flagDisplayString.Append(info);
                        appendLine = true;
                    }
                }
            }
            return flagDisplayString.ToString();
        }
    }
}
