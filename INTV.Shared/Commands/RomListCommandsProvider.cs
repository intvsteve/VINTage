// <copyright file="RomListCommandsProvider.cs" company="INTV Funhouse">
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
    /// ROM list commands provider exposes the commands for the ROM list.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(ICommandProvider))]
    public partial class RomListCommandsProvider : CommandProvider
    {
        /// <summary>
        /// The unique name of the provider.
        /// </summary>
        internal const string ProviderName = "INTV.Shared.Commands.RomListCommandsProvider";

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Commands.RomListCommandsProvider"/> class.
        /// </summary>
        public RomListCommandsProvider()
            : base(ProviderName)
        {
            Groups.Add(RomListCommandGroup.Group);
        }

        /// <inheritdoc />
        /// <remarks>This should appear very early - but after app provider.</remarks>
        public override double Weight
        {
            get { return -100; }
        }
    }
}
