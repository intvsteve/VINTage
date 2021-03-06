﻿// <copyright file="ApplicationCommandsProvider.cs" company="INTV Funhouse">
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

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Application-wide commands provider. All applications have these commands.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ICommandProvider))]
    public partial class ApplicationCommandsProvider : CommandProvider
    {
        public ApplicationCommandsProvider()
            : base("INTV.Shared.Commands.ApplicationCommandsProvider")
        {
            Groups.Add(RootCommandGroup.Group);
            Groups.Add(ApplicationCommandGroup.Group);
#if DEBUG
            Groups.Add(DebugCommandGroup.Group);
#endif // DEBUG
        }

        /// <inheritdoc />
        /// <remarks>This one should always be first.</remarks>
        public override double Weight
        {
            get { return -1000; }
        }
    }
}
