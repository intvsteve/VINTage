// <copyright file="IProgramInformationToXmlRomInformationConverter.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    public class IProgramInformationToXmlRomInformationConverter : ConvertToMultiple<IProgramInformationToXmlRomInformationConverter, IProgramInformation, XmlRomInformation>
    {
        /// <inheritdoc />
        public override IEnumerable<XmlRomInformation> Convert(IProgramInformation source)
        {
            return Convert(source, convertAll: true, setFeatures: true, setMetadata: true);
        }

        /// <summary>
        ///  Converts an instance of <see cref="IProgramInformation"/> to an enumerable of <see cref="XmlRomInformation"/> objects.
        /// </summary>
        /// <param name="source">An instance of <see cref="IProgramInformation"/> to convert.</param>
        /// <param name="convertAll">If <c>true</c>, convert all CRC variants within <paramref name="source"/>, otherwise only the first instance.</param>
        /// <param name="setFeatures">If <c>true</c>, set feature values.</param>
        /// <param name="setMetadata">If <c>true</c>, set metadata values.</param>
        /// <returns>An enumerable containing the <see cref="XmlRomInformation"/> objects that result from converting <paramref name="source"/>.</returns>
        public IEnumerable<XmlRomInformation> Convert(IProgramInformation source, bool convertAll, bool setFeatures, bool setMetadata)
        {
            var romVariants = convertAll ? source.Crcs : source.Crcs.Take(1);
            foreach (var romVariant in source.Crcs)
            {
                yield return Convert(source, romVariant, setFeatures, setMetadata);
            }
        }

        /// <summary>
        ///  Converts an instance of <see cref="IProgramInformation"/> to an instance of <see cref="XmlRomInformation"/>.
        /// </summary>
        /// <param name="source">An instance of <see cref="IProgramInformation"/> to convert.</param>
        /// <param name="romVariant">The specific ROM variant to convert.</param>
        /// <param name="setFeatures">If <c>true</c>, set feature values.</param>
        /// <param name="setMetadata">If <c>true</c>, set metadata values.</param>
        /// <returns>An enumerable containing the <see cref="XmlRomInformation"/> objects that result from converting <paramref name="source"/>.</returns>
        public XmlRomInformation Convert(IProgramInformation source, CrcData romVariant, bool setFeatures, bool setMetadata)
        {
            var xmlRomInformation = CreateInitialXmlRomInformation(source, romVariant);
            if (setFeatures)
            {
                xmlRomInformation.SetProgramFeatures(source.Features);
            }
            if (setMetadata)
            {
                xmlRomInformation.SetProgramMetadata(source as IProgramMetadata);
            }
            return xmlRomInformation;
        }

        private static XmlRomInformation CreateInitialXmlRomInformation(IProgramInformation source, CrcData romVariant)
        {
            var xmlRomInformation = XmlRomInformation.CreateDefault();

            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.title).Value = source.Title.EncodeHtmlString();
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.vendor).Value = source.Vendor.EncodeHtmlString();
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.short_name).Value = source.ShortName.EncodeHtmlString();
            xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.origin).Value = ProgramInformationOriginToDatabaseString.Instance.Convert(source.DataOrigin);

            if (source.Features != null)
            {
                xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.type).Value = GeneralFeaturesToRomTypeStringConverter.Instance.Convert(source.Features.GeneralFeatures);
            }

            if (!string.IsNullOrEmpty(source.Year))
            {
                int year;
                if ((source.Year.Length == 4) && int.TryParse(source.Year, NumberStyles.None, CultureInfo.InvariantCulture, out year))
                {
                    // Only have year, so assume 01 Jan. This will be superseded if metadata contains release date (taking first one).
                    xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.release_date).Value = string.Format(CultureInfo.InvariantCulture, "{0:0000}-01-01", year);
                }
            }

            if (!string.IsNullOrEmpty(romVariant.Description))
            {
                xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.name).Value = romVariant.Description.EncodeHtmlString();
            }

            return xmlRomInformation;
        }

        private class ProgramInformationOriginToDatabaseString : Converter<ProgramInformationOriginToDatabaseString, ProgramInformationOrigin, string>
        {
            public override string Convert(ProgramInformationOrigin source)
            {
                var originString = string.Empty;
                switch (source)
                {
                    case ProgramInformationOrigin.IntvFunhouse:
                    case ProgramInformationOrigin.Embedded:
                        originString = XmlRomInformationDatabaseColumn.OriginIntvFunhouse;
                        break;
                    case ProgramInformationOrigin.IntellivisionProductions:
                        originString = XmlRomInformationDatabaseColumn.OriginBlueSkyRangers;
                        break;
                    case ProgramInformationOrigin.UserDefined:
                        originString = XmlRomInformationDatabaseColumn.OriginUserDefined;
                        break;
                    case ProgramInformationOrigin.UserEmail:
                        originString = XmlRomInformationDatabaseColumn.OriginUserEmail;
                        break;
                    case ProgramInformationOrigin.JzIntv:
                        originString = XmlRomInformationDatabaseColumn.OriginIntvName;
                        break;
                    case ProgramInformationOrigin.RomMetadataBlock:
                        originString = XmlRomInformationDatabaseColumn.OriginRomFormatMetadata;
                        break;
                    case ProgramInformationOrigin.CfgVarMetadataBlock:
                        originString = XmlRomInformationDatabaseColumn.OriginCfgFormatMetadata;
                        break;
                    case ProgramInformationOrigin.LuigiMetadataBlock:
                        originString = XmlRomInformationDatabaseColumn.OriginLuigiFormatMetadata;
                        break;
                    case ProgramInformationOrigin.GameCatalog:
                        originString = XmlRomInformationDatabaseColumn.OriginCatalog;
                        break;
                    default:
                        originString = XmlRomInformationDatabaseColumn.OriginOther;
                        break;
                }
                return originString;
            }
        }

        private class GeneralFeaturesToRomTypeStringConverter : Converter<GeneralFeaturesToRomTypeStringConverter, GeneralFeatures, string>
        {
            /// <inheritdoc />
            public override string Convert(GeneralFeatures source)
            {
                // Database schema info: enum('BIOS','Program') DEFAULT 'Program'
                var generalFeatures = source & GeneralFeaturesHelpers.ValidFeaturesMask;
                var romTypeString = generalFeatures.HasFlag(GeneralFeatures.SystemRom) ? XmlRomInformationDatabaseColumn.RomTypeValueSystem : XmlRomInformationDatabaseColumn.RomTypeValueRom;
                return romTypeString;
            }
        }
    }
}
