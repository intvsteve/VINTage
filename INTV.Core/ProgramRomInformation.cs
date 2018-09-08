// <copyright file="ProgramRomInformation.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Implements <see cref="IProgramRomInformation"/>.
    /// </summary>
    internal class ProgramRomInformation : IProgramRomInformation
    {
        #region Properties

        #region IProgramRomInformation

        /// <inheritdoc />
        public string Title { get; internal set; }

        /// <inheritdoc />
        public string Vendor { get; internal set; }

        /// <inheritdoc />
        public string Year { get; internal set; }

        /// <inheritdoc />
        public string LongName { get; internal set; }

        /// <inheritdoc />
        public string ShortName { get; internal set; }

        /// <inheritdoc />
        public string VariantName { get; internal set; }

        /// <inheritdoc />
        public RomFormat Format { get; internal set; }

        /// <inheritdoc />
        public ProgramIdentifier Id { get; internal set; }

        /// <inheritdoc />
        public IProgramFeatures Features { get; internal set; }

        /// <inheritdoc />
        public IProgramMetadata Metadata { get; internal set; }

        /// <inheritdoc />
        #endregion // IProgramRomInformation

        #endregion // Properties
    }
}
