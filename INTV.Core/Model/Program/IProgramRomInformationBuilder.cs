// <copyright file="IProgramRomInformationBuilder.cs" company="INTV Funhouse">
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
    /// This interface specifies a builder pattern for creating an instance of <see cref="IProgramRomInformation"/>.
    /// </summary>
    public interface IProgramRomInformationBuilder
    {
        /// <summary>
        /// Adds the title to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="title">The title (name) of the program.</param>
        /// <returns>The builder.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="title"/> is null or empty.</exception>
        IProgramRomInformationBuilder WithTitle(string title);

        /// <summary>
        /// Adds the vendor to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="vendor">The vendor that published the program.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithVendor(string vendor);

        /// <summary>
        /// Adds the year to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="year">The year in which the program was released. Expected to be greater than or equal to 1978.</param>
        /// <returns>The builder.</returns>
        /// <remarks>Values less than zero are ignored.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="year"/> is greater than or equal to zero and less than 1978.</exception>
        IProgramRomInformationBuilder WithYear(int year);

        /// <summary>
        /// Adds the long name to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="longName">The long name for the program, capped at 60 characters.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithLongName(string longName);

        /// <summary>
        /// Adds the short name to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="shortName">The short name for the program, capped at 18 characters.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithShortName(string shortName);

        /// <summary>
        /// Adds the variant name to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="variantName">The name of the program variation.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithVariantName(string variantName);

        /// <summary>
        /// Adds the ROM format to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="romFormat">The format of the ROM being described.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithFormat(RomFormat romFormat);

        /// <summary>
        /// Adds the program identifier to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="id">The unique identifier for the ROM.</param>
        /// <returns>The builder.</returns>
        /// <remarks>Implementations are allowed to validate this value for the specific ROM in question.</remarks>
        IProgramRomInformationBuilder WithId(ProgramIdentifier id);

        /// <summary>
        /// Adds the features to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="features">The features of the program.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithFeatures(IProgramFeatures features);

        /// <summary>
        /// Adds the metadata to the instance of <see cref="IProgramRomInformation"/> being built.
        /// </summary>
        /// <param name="metadata">The metadata of the program.</param>
        /// <returns>The builder.</returns>
        IProgramRomInformationBuilder WithMetadata(IProgramMetadata metadata);

        /// <summary>
        /// Creates the concrete instance of the program ROM information.
        /// </summary>
        /// <returns>The <see cref="IProgramRomInformation"/> defined by the builder.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if, during the final build, the program's configured data is incorrect. Consult the
        /// exception's message for details about any errors that are found.</exception>
        IProgramRomInformation Build();
    }
}
