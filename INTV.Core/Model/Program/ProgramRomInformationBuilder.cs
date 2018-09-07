// <copyright file="ProgramRomInformationBuilder.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Implements the <see cref="IProgramRomInformationBuilder"/> builder pattern for <see cref="IProgramRomInformation"/>.
    /// </summary>
    public class ProgramRomInformationBuilder : IProgramRomInformationBuilder
    {
        private ProgramRomInformation _programRomInformation = new ProgramRomInformation();

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new InvalidOperationException();
            }
            _programRomInformation.Title = title;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithVendor(string vendor)
        {
            _programRomInformation.Vendor = vendor;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithYear(int year)
        {
            if ((year >= 0) && (year < 1978))
            {
            }
            else if (year >= 1978)
            {
                _programRomInformation.Year = year.ToString(CultureInfo.InvariantCulture);
            }
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithLongName(string longName)
        {
            _programRomInformation.LongName = longName;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithShortName(string shortName)
        {
            _programRomInformation.ShortName = shortName;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithVariantName(string variantName)
        {
            _programRomInformation.VariantName = variantName;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithFormat(RomFormat romFormat)
        {
            _programRomInformation.Format = romFormat;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithId(ProgramIdentifier id)
        {
            _programRomInformation.Id = id;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithFeatures(IProgramFeatures features)
        {
            _programRomInformation.Features = features;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformationBuilder WithMetadata(IProgramMetadata metadata)
        {
            _programRomInformation.Metadata = metadata;
            return this;
        }

        /// <inheritdoc />
        public IProgramRomInformation Build()
        {
            Validate();
            return _programRomInformation;
        }

        private void Validate()
        {
            var invalidFields = new List<string>();
            if (_programRomInformation.Id.DataCrc == 0)
            {
                invalidFields.Add("Id");
            }
            if (_programRomInformation.Format == RomFormat.None)
            {
                invalidFields.Add("Format");
            }
            if (string.IsNullOrEmpty(_programRomInformation.Title))
            {
                invalidFields.Add("Title");
            }
            if (_programRomInformation.Features == null)
            {
                invalidFields.Add("Features");
            }
            if (_programRomInformation.Metadata == null)
            {
                invalidFields.Add("Metadata");
            }
            if (invalidFields.Any())
            {
                var messageBuilder = new StringBuilder(Resources.Strings.ProgramRomValidationFailed_Message).AppendLine().AppendLine();
                foreach (var invalidField in invalidFields)
                {
                    messageBuilder.AppendLine("  " + invalidField);
                }
                throw new InvalidOperationException(messageBuilder.ToString());
            }
        }
    }
}
