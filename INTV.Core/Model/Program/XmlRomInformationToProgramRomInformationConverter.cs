// <copyright file="XmlRomInformationToProgramRomInformationConverter.cs" company="INTV Funhouse">
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
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Implements <see cref="IConverter{TSource,TDestination}"/> for converting <see cref="XmlRomInformation"/> to <see cref="IProgramRomInformation"/>.
    /// </summary>
    internal class XmlRomInformationToProgramRomInformationConverter : IConverter<XmlRomInformation, IProgramRomInformation>
    {
        private XmlRomInformationToProgramRomInformationConverter(IProgramInformationTable defaultInformationSource, IEnumerable<ProgramDescription> defaultDescriptionsSource)
        {
            DefaultDescriptionsSource = defaultDescriptionsSource;
            DefaultInformationSource = defaultInformationSource;
        }

        private IEnumerable<ProgramDescription> DefaultDescriptionsSource { get; set; }

        private IProgramInformationTable DefaultInformationSource { get; set; }

        /// <summary>
        /// Creates an instance of the converter that will not refer to any initial sources of information.
        /// </summary>
        /// <returns>An instance of the converter.</returns>
        public static XmlRomInformationToProgramRomInformationConverter Create()
        {
            return Create(null, null);
        }

        /// <summary>
        /// Creates an instance of the converter that will attempt to locate information that is not available via a program information table.
        /// </summary>
        /// <param name="defaultInformationSource">The program information table to use to locate missing information.</param>
        /// <returns>An instance of the converter.</returns>
        /// <remarks>First, <paramref name="defaultInformationSource"/> will be searched for initial values for ROM information. Values supplied directly from the ROM
        /// being converted will override those from the initial source.</remarks>
        public static XmlRomInformationToProgramRomInformationConverter Create(IProgramInformationTable defaultInformationSource)
        {
            return Create(defaultInformationSource, null);
        }

        /// <summary>
        /// Creates an instance of the converter that will attempt to locate information that is not available in a collection of existing <see cref="IProgramDescription"/>s.
        /// </summary>
        /// <param name="initialDescriptionsSource">A collection of program descriptions to use to locate information not directly provided in the ROM information being converted.</param>
        /// <returns>An instance of the converter.</returns>
        /// <remarks>First, <paramref name="initialDescriptionsSource"/> will be searched for initial values for ROM information. Values supplied directly from the ROM
        /// being converted will override those from the initial source.</remarks>
        public static XmlRomInformationToProgramRomInformationConverter Create(IEnumerable<ProgramDescription> initialDescriptionsSource)
        {
            return Create(null, initialDescriptionsSource);
        }

        /// <summary>
        /// Creates an instance of the converter that will attempt to locate information that is not available via a program information table and list of program descriptions.
        /// </summary>
        /// <param name="initialInformationSource">The program information table to use to locate missing information.</param>
        /// <param name="initialDescriptionsSource">A collection of program descriptions to use to locate information not directly provided in the ROM information being converted.</param>
        /// <returns>An instance of the converter.</returns>
        /// <remarks>First, <paramref name="initialDescriptionsSource"/> will be searched, then <paramref name="initialInformationSource"/>, for initial values for ROM information. Values
        /// supplied directly from the ROM being converted will override those from the initial source.</remarks>
        public static XmlRomInformationToProgramRomInformationConverter Create(IProgramInformationTable initialInformationSource, IEnumerable<ProgramDescription> initialDescriptionsSource)
        {
            return new XmlRomInformationToProgramRomInformationConverter(initialInformationSource, initialDescriptionsSource);
        }

        #region IConverter

        /// <inheritdoc />
        /// <remarks>Program ROM information specified in a rominfo database entry must at least specify a crc and ROM format.</remarks>
        /// <exception cref="System.NullReferenceException">Thrown if a required database column is missing.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if required database column value is null.</exception>
        /// <exception cref="System.FormatException">Thrown if data cannot be parsed.</exception>
        /// <exception cref="System.OverflowException">Thrown if CRC values are larger than <c>uint.MaxValue</c>.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if any required fields do not have a valid value, e.g. primary CRC is zero.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an unrecognized ROM format is specified.</exception>
        public IProgramRomInformation Convert(XmlRomInformation source)
        {
            var programIdentifier = source.GetProgramIdentifier();
            var format = source.GetRomFormat();
            var programInformationBuilder = new ProgramRomInformationBuilder().WithId(programIdentifier).WithFormat(format);

            AddDataFromColumns(source, programInformationBuilder);

            var code = source.GetDatabaseCode();
            var programDescription = GetProgramDescription(programIdentifier, format, code);
            var programInformation = GetProgramInformation(programDescription, programIdentifier);

            if (!AddFromIProgramDescription(programDescription, programInformationBuilder))
            {
                AddFromIProgramInformation(programInformation, programInformationBuilder);
            }

            var initialProgramFeatures = GetInitialProgramFeatures(programDescription, programInformation);
            programInformationBuilder.WithFeatures(source.GetProgramFeatures(initialProgramFeatures));

            var initialProgramMetadata = GetInitialProgramMetadatda(programDescription, programInformation);
            programInformationBuilder.WithMetadata(source.GetProgramMetadata(initialProgramMetadata));

            return programInformationBuilder.Build();
        }

        #endregion // IConverter

        private void AddDataFromColumns(XmlRomInformation source, IProgramRomInformationBuilder programInformationBuilder)
        {
            foreach (var column in source.RomInfoDatabaseColumns)
            {
                try
                {
                    switch (column.Name.ToRomInfoDatabaseColumnName())
                    {
                        case XmlRomInformationDatabaseColumnName.title:
                            programInformationBuilder.WithTitle(StringConverter.Instance.Convert(column.Value));
                            programInformationBuilder.WithLongName(StringConverter.Instance.Convert(column.Value, 60));
                            break;
                        case XmlRomInformationDatabaseColumnName.vendor:
                            programInformationBuilder.WithTitle(StringConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.release_date:
                            programInformationBuilder.WithYear(StringToMetadataDateTimeConverter.Instance.Convert(column.Value).Date.Year);
                            break;
                        case XmlRomInformationDatabaseColumnName.short_name:
                            programInformationBuilder.WithShortName(StringConverter.Instance.Convert(column.Value, 18));
                            break;
                        case XmlRomInformationDatabaseColumnName.name:
                            programInformationBuilder.WithVariantName(StringConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.platform: // enum('Intellivision')
                            // Throw if it's not Intellivision?
                            break;
                        case XmlRomInformationDatabaseColumnName.variant: // this is variant number of the ROM, not the name of the variant
                        case XmlRomInformationDatabaseColumnName.origin: // set('INTV Funhouse','Intellivision Lives','manual entry','e-mail','intvname','ROM','CFG','LUIGI','Catalog','other') NOT NULL COMMENT '',
                        case XmlRomInformationDatabaseColumnName.box_variant:
                        case XmlRomInformationDatabaseColumnName.screenshot:
                        case XmlRomInformationDatabaseColumnName.preview:
                        case XmlRomInformationDatabaseColumnName.get_rom:
                            break;
                        default:
                            break;
                    }
                }
                catch (FormatException)
                {
                }
                catch (ArgumentException)
                {
                }
            }
        }

        private IProgramDescription GetProgramDescription(ProgramIdentifier programIdentifier, RomFormat format, string code)
        {
            IProgramDescription programDescription = null;
            if (DefaultDescriptionsSource != null)
            {
                // Try the strictest match first.
                programDescription = DefaultDescriptionsSource.FirstOrDefault(d => d.IsMatchingProgramDescription(programIdentifier, format, true, code));
                if (programDescription == null)
                {
                    programDescription = DefaultDescriptionsSource.FirstOrDefault(d => d.IsMatchingProgramDescription(programIdentifier, format, true));
                }
                if (programDescription == null)
                {
                    programDescription = DefaultDescriptionsSource.FirstOrDefault(d => d.IsMatchingProgramDescription(programIdentifier, format, false));
                }
                if (programDescription == null)
                {
                    programDescription = DefaultDescriptionsSource.FirstOrDefault(d => d.IsMatchingProgramDescription(programIdentifier));
                }
                if (programDescription == null)
                {
                    programDescription = DefaultDescriptionsSource.FirstOrDefault(d => d.Crc == programIdentifier.DataCrc);
                }
            }
            return programDescription;
        }

        private IProgramInformation GetProgramInformation(IProgramDescription programDescription, ProgramIdentifier programIdentifier)
        {
            var programInformation = programDescription == null ? null : programDescription.ProgramInformation;
            if ((DefaultInformationSource != null) && (programInformation == null))
            {
                programInformation = DefaultInformationSource.FindProgram(programIdentifier);
            }
            return programInformation;
        }

        private bool AddFromIProgramDescription(IProgramDescription programDescription, IProgramRomInformationBuilder programInformationBuilder)
        {
            var built = programDescription != null;
            if (built)
            {
                AddBasicData(programInformationBuilder, programDescription.Name, programDescription.Vendor, programDescription.Year, programDescription.ShortName);
            }
            return built;
        }

        private bool AddFromIProgramInformation(IProgramInformation programInformation, IProgramRomInformationBuilder programInformationBuilder)
        {
            var built = programInformation != null;
            if (built)
            {
                AddBasicData(programInformationBuilder, programInformation.Title, programInformation.Vendor, programInformation.Year, programInformation.ShortName);
            }
            return built;
        }

        private void AddBasicData(IProgramRomInformationBuilder programInformationBuilder, string title, string vendor, string yearString, string shortName)
        {
            try
            {
                programInformationBuilder.WithTitle(title);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            try
            {
                programInformationBuilder.WithVendor(vendor);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            int year;
            if (int.TryParse(yearString, out year))
            {
                try
                {
                    programInformationBuilder.WithYear(year);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
            if (!string.IsNullOrEmpty(shortName))
            {
                programInformationBuilder.WithShortName(shortName);
            }
        }

        private IProgramFeatures GetInitialProgramFeatures(IProgramDescription programDescription, IProgramInformation programInformation)
        {
            IProgramFeatures initialProgramFeatures = null;
            if (programDescription != null)
            {
                initialProgramFeatures = programDescription.Features;
            }
            if ((initialProgramFeatures == null) && (programInformation != null))
            {
                initialProgramFeatures = programInformation.Features;
            }
            return initialProgramFeatures;
        }

        private IProgramMetadata GetInitialProgramMetadatda(IProgramDescription programDescription, IProgramInformation programInformation)
        {
            IProgramMetadata initialProgramMetadata = null;
            if ((programDescription != null) && (programDescription.Rom != null))
            {
                initialProgramMetadata = programDescription.Rom.GetProgramMetadata();
            }
            if ((initialProgramMetadata == null) && (programInformation != null))
            {
                // Several implementations of IProgramInformation also implement IProgramMetadata.
                initialProgramMetadata = programInformation as IProgramMetadata;
            }
            return initialProgramMetadata;
        }

        private class StringConverter : Converter<StringConverter, string, string>
        {
            /// <inheritdoc />
            public override string Convert(string source)
            {
                var convertedValue = string.Empty;
                if (!string.IsNullOrEmpty(source))
                {
                    convertedValue = source.DecodeHtmlString().Trim();
                }
                if (string.IsNullOrEmpty(convertedValue))
                {
                    throw new ArgumentOutOfRangeException();
                }
                return convertedValue;
            }

            /// <summary>
            /// Limits the string to <paramref name="maxLength"/> and the GROM character set, decoding any HTML that may be in the string as well.
            /// </summary>
            /// <param name="source">The string to convert.</param>
            /// <param name="maxLength">The maximum length of the returned string.</param>
            /// <returns>A string in which HTML has been decoded, limited to <paramref name="maxLength"/>.</returns>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="source"/> is null or empty, or no viable string results from the conversion.</exception>
            public string Convert(string source, int maxLength)
            {
                var convertedString = string.Empty;
                if (!string.IsNullOrEmpty(source))
                {
                    convertedString = source.DecodeHtmlString().Trim().EnforceNameLength(maxLength, restrictToGromCharacters: true);
                }
                if (string.IsNullOrEmpty(source))
                {
                    throw new ArgumentOutOfRangeException();
                }
                return convertedString;
            }
        }

        private class StringToMetadataDateTimeConverter : Converter<StringToMetadataDateTimeConverter, string, MetadataDateTime>
        {
            public override MetadataDateTime Convert(string source)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var dateParts = source.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dateParts.Length == 3)
                    {
                        var dateTimeOffset = DateTimeOffset.ParseExact(source, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var dateTime = new MetadataDateTime(dateTimeOffset, MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Month | MetadataDateTimeFlags.Day);
                        return dateTime;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
