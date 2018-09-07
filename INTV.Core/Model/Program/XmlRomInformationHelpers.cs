// <copyright file="XmlRomInformationHelpers.cs" company="INTV Funhouse">
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
using System.IO;
using System.Linq;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    /// <summary>
    /// Extension and helper methods for working with <see cref="XmlRomInformation"/>.
    /// </summary>
    internal static class XmlRomInformationHelpers
    {
        /// <summary>
        /// Strategy to extract a <see cref="ProgramIdentifier"/> from a rominfo database entry.
        /// </summary>
        /// <param name="xmlRomInformation">A database entry from the INTV Funhouse rominfo database.</param>
        /// <returns>A (hopefully unique) program identifier extracted from <paramref name="xmlRomInformation"/>.</returns>
        /// <exception cref="System.NullReferenceException">Thrown if required database column is missing.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if database column value is null.</exception>
        /// <exception cref="System.FormatException">Thrown if data cannot be parsed.</exception>
        /// <exception cref="System.OverflowException">Thrown if CRC values are larger than <c>uint.MaxValue</c>.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if the primary CRC is zero, or ROM format is invalid.</exception>
        public static ProgramIdentifier GetProgramIdentifier(this XmlRomInformation xmlRomInformation)
        {
            // There must always be a non-zero value for the primary CRC.
            var dataColumn = xmlRomInformation.RomInfoDatabaseColumns.First(c => c.Name == XmlRomInformationDatabaseColumnName.crc.ToString());
            var crc = uint.Parse(dataColumn.Value, CultureInfo.InvariantCulture);
            if (crc == 0)
            {
                throw new InvalidOperationException();
            }

            // Next, attempt to get the secondary CRC value. This may go through a series of fallbacks, as follows:
            // 1:   Value is directly defined => done
            // 2:   If not defined, check ROM format
            // 2.a: If BIN+CFG format, examine the bin_cfg column
            // 2.b: If bin_cfg parses as an integer, treat as a "stock" .cfg file
            // 2.c: If stock .cfg file exists, use its CRC as otherCRC, else zero
            // 2.d: If bin_cfg is not an integer, treat text as something to compute CRC of as if it were a 'canonical' config file
            var otherCrc = 0u;
            dataColumn = xmlRomInformation.RomInfoDatabaseColumns.FirstOrDefault(c => c.Name == XmlRomInformationDatabaseColumnName.crc_2.ToString());
            if ((dataColumn != null) && !string.IsNullOrEmpty(dataColumn.Value))
            {
                if (!uint.TryParse(dataColumn.Value, NumberStyles.None, CultureInfo.InvariantCulture, out otherCrc))
                {
                    otherCrc = 0;
                }
            }

            // The second part of the CRC does not appear to have been defined, or is malformed. If ROM is BIN format, keep checking.
            if (otherCrc == 0)
            {
                dataColumn = xmlRomInformation.RomInfoDatabaseColumns.First(c => c.Name == XmlRomInformationDatabaseColumnName.format.ToString());
                var romFormat = StringToRomFormatConverter.Instance.Convert(dataColumn.Value);
                if (romFormat == RomFormat.Bin)
                {
                    dataColumn = xmlRomInformation.RomInfoDatabaseColumns.FirstOrDefault(c => c.Name == XmlRomInformationDatabaseColumnName.bin_cfg.ToString());
                    if ((dataColumn != null) && !string.IsNullOrEmpty(dataColumn.Value))
                    {
                        // If bin_cfg parses does not parse as a number, treat it as canonical config file and compute its CRC.
                        if (!uint.TryParse(dataColumn.Value, NumberStyles.None, CultureInfo.InvariantCulture, out otherCrc))
                        {
                            // PCLs only support UTF8... Spec says ASCII. Let's hope we don't run into anything *too* weird.
                            var cfgContents = System.Text.Encoding.UTF8.GetBytes(dataColumn.Value);
                            using (var s = new MemoryStream(cfgContents))
                            {
                                otherCrc = Crc32.OfStream(s);
                            }
                        }
                        else
                        {
                            // The string specifies a canonical CFG file number that we expect to have shipped with INTV.Core. If that file can be found,
                            // compute its CRC and use that as the otherCRC value. These files are named fileNumber.cfg, e.g. 0.cfg, 1.cfg, et. al.
                            var stockConfigFilePath = IRomHelpers.GetStockCfgFilePath((int)otherCrc);
                            if (!string.IsNullOrEmpty(stockConfigFilePath))
                            {
                                otherCrc = Crc32.OfFile(stockConfigFilePath);
                            }
                            else
                            {
                                // File doesn't exist, so just toss the result and treat as unspecified.
                                otherCrc = 0;
                            }
                        }
                    }
                }
            }

            var programIdentifier = new ProgramIdentifier(crc, otherCrc);
            return programIdentifier;
        }

        /// <summary>
        /// Strategy to extract a ROM's format from an entry from a rominfo database entry.
        /// </summary>
        /// <param name="xmlRomInformation">A database entry from the INTV Funhouse rominfo database.</param>
        /// <returns>The ROM format of the database entry.</returns>
        /// <exception cref="System.NullReferenceException">Thrown if required database column is missing.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if database column value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an unrecognized ROM format is specified.</exception>
        public static RomFormat GetRomFormat(this XmlRomInformation xmlRomInformation)
        {
            var dataColumn = xmlRomInformation.RomInfoDatabaseColumns.First(c => c.Name == XmlRomInformationDatabaseColumnName.format.ToString());
            var romFormat = StringToRomFormatConverter.Instance.Convert(dataColumn.Value);
            return romFormat;
        }

        /// <summary>
        /// Strategy to extract a ROM's <see cref="IProgramFeatures"/> from a rominfo database entry.
        /// </summary>
        /// <param name="xmlRomInformation">A database entry from the INTV Funhouse rominfo database.</param>
        /// <param name="featuresToOverride">Known existing features for the ROM that may be overridden. This value may be <c>null</c>.</param>
        /// <returns>The <see cref="IProgramFeatures"/> for the ROM.</returns>
        /// <remarks>If <paramref name="featuresToOverride"/> is <c>null</c>, a default set of features will act as the starting point.</remarks>
        public static IProgramFeatures GetProgramFeatures(this XmlRomInformation xmlRomInformation, IProgramFeatures featuresToOverride)
        {
            var programFeaturesBuilder = new ProgramFeaturesBuilder().WithInitialFeatures(featuresToOverride);
            GeneralFeatures? programType = null;
            GeneralFeatures? generalFeatures = null;
            foreach (var column in xmlRomInformation.RomInfoDatabaseColumns)
            {
                try
                {
                    switch (column.Name.ToRomInfoDatabaseColumnName())
                    {
                        case XmlRomInformationDatabaseColumnName.type:
                            programType = RomTypeStringToGeneralFeaturesConverter.Instance.Convert(column.Value);
                            break;
                        case XmlRomInformationDatabaseColumnName.general_features:
                            generalFeatures = StringToGeneralFeaturesConverter.Instance.Convert(column.Value);
                            break;
                        case XmlRomInformationDatabaseColumnName.ntsc:
                            programFeaturesBuilder.WithNtscCompatibility(StringToFeatureCompatibilityConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.pal:
                            programFeaturesBuilder.WithPalCompatibility(StringToFeatureCompatibilityConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.kc:
                            programFeaturesBuilder.WithKeyboardComponentFeatures(StringToKeyboardComponentFeaturesConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.sva:
                            programFeaturesBuilder.WithSuperVideoArcadeCompatibility(StringToFeatureCompatibilityConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.ivoice:
                            programFeaturesBuilder.WithIntellivoiceCompatibility(StringToFeatureCompatibilityConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.intyii:
                            programFeaturesBuilder.WithIntellivisionIICompatibility(StringToFeatureCompatibilityConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.ecs:
                            programFeaturesBuilder.WithEcsFeatures(StringToEcsFeaturesConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.tutor:
                            programFeaturesBuilder.WithTutorvisionCompatibility(StringToFeatureCompatibilityConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.icart:
                            programFeaturesBuilder.WithIntellicartFeatures(StringToIntellicartCC3FeaturesConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.cc3:
                            programFeaturesBuilder.WithCuttleCart3Features(StringToCuttleCart3Features.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.jlp:
                            programFeaturesBuilder.WithJlpFeatures(StringToJlpFeaturesConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.jlp_savegame:
                            programFeaturesBuilder.WithMinimumFlashSectors(StringToMinimumFlashSectorsConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.lto_flash:
                            programFeaturesBuilder.WithLtoFlashFeatures(StringToLtoFlashFeaturesConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.bee3:
                            programFeaturesBuilder.WithBee3Features(StringToBee3FeaturesConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.hive:
                            programFeaturesBuilder.WithHiveFeatures(StringToHiveFeaturesConverter.Instance.Convert(column.Value));
                            break;
                        default:
                            break;
                    }
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
                catch (NullReferenceException)
                {
                }
                catch (ArgumentException)
                {
                }
            }

            if (programType.HasValue || generalFeatures.HasValue)
            {
                var newGeneralFeatures = GeneralFeatures.None;
                if (generalFeatures.HasValue)
                {
                    newGeneralFeatures = generalFeatures.Value;
                }
                else if (featuresToOverride != null)
                {
                    // Replace the 'program type' part of existing general features.
                    newGeneralFeatures = featuresToOverride.GeneralFeatures & ~GeneralFeatures.SystemRom;
                }
                if (programType.HasValue)
                {
                    newGeneralFeatures |= programType.Value;
                }
                programFeaturesBuilder.WithGeneralFeatures(newGeneralFeatures);
            }
            return programFeaturesBuilder.Build();
        }

        /// <summary>
        /// Strategy to extract a ROM's <see cref="IProgramMetadata"/> from a rominfo database entry.
        /// </summary>
        /// <param name="xmlRomInformation">A database entry from the INTV Funhouse rominfo database.</param>
        /// <param name="initialMetadata">Known existing metadata for the ROM that may be overridden. This value may be <c>null</c>.</param>
        /// <returns>The <see cref="IProgramMetadata"/> for the ROM.</returns>
        /// <remarks>If <paramref name="initialMetadata"/> is <c>null</c>, empty metadata is used as the starting point. Note also that
        /// the rominfo database entries apply to a specific ROM. Incoming metadata may contain multiple values for some fields for which
        /// a rominfo database entry contains only one value. In such circumstances, the existing metadata is replaced.</remarks>
        public static IProgramMetadata GetProgramMetadata(this XmlRomInformation xmlRomInformation, IProgramMetadata initialMetadata)
        {
            var programMetadataBuilder = new ProgramMetadataBuilder().WithInitialMetadata(initialMetadata);
            List<string> additionalInformation = null;
            foreach (var column in xmlRomInformation.RomInfoDatabaseColumns)
            {
                try
                {
                    switch (column.Name.ToRomInfoDatabaseColumnName())
                    {
                        case XmlRomInformationDatabaseColumnName.title:
                            programMetadataBuilder.WithLongNames(StringToStringEnumerableConverter.Instance.Convert(column.Value, 60));
                            break;
                        case XmlRomInformationDatabaseColumnName.short_name:
                            programMetadataBuilder.WithShortNames(StringToStringEnumerableConverter.Instance.Convert(column.Value, 18));
                            break;
                        case XmlRomInformationDatabaseColumnName.description:
                            programMetadataBuilder.WithDescriptions(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.vendor:
                            programMetadataBuilder.WithPublishers(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.program:
                            programMetadataBuilder.WithProgrammers(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.concept:
                            programMetadataBuilder.WithDesigners(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.game_graphics:
                            programMetadataBuilder.WithGraphics(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.music:
                            programMetadataBuilder.WithMusic(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.soundfx:
                            programMetadataBuilder.WithSoundEffects(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.voices:
                            programMetadataBuilder.WithVoices(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.game_docs:
                            programMetadataBuilder.WithDocumentation(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.box_art:
                            programMetadataBuilder.WithArtwork(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.release_date:
                            programMetadataBuilder.WithReleaseDates(StringToMetadataDateTimeConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.license:
                            programMetadataBuilder.WithLicenses(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.contact_info:
                            programMetadataBuilder.WithContactInformation(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.source: // accumulated in additional info
                            var sources = StringToStringEnumerableConverter.Instance.Convert(column.Value);
                            if (additionalInformation == null)
                            {
                                additionalInformation = new List<string>(sources);
                            }
                            else
                            {
                                additionalInformation.AddRange(sources);
                            }
                            break;
                        case XmlRomInformationDatabaseColumnName.name: // ROM variant name (version)
                            programMetadataBuilder.WithVersions(StringToStringEnumerableConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.build_date:
                            programMetadataBuilder.WithBuildDates(StringToMetadataDateTimeConverter.Instance.Convert(column.Value));
                            break;
                        case XmlRomInformationDatabaseColumnName.other: // accumulated in additional info
                            var otherInformation = StringToStringEnumerableConverter.Instance.Convert(column.Value);
                            if (additionalInformation == null)
                            {
                                additionalInformation = new List<string>(otherInformation);
                            }
                            else
                            {
                                additionalInformation.AddRange(otherInformation);
                            }
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
                catch (NullReferenceException)
                {
                }
            }
            if (additionalInformation != null)
            {
                programMetadataBuilder.WithAdditionalInformation(additionalInformation);
            }
            IProgramMetadata metadata = programMetadataBuilder.Build();
            return metadata;
        }

        #region Converters

        private class StringToRomFormatConverter : Converter<StringToRomFormatConverter, string, RomFormat>
        {
            /// <inheritdoc />
            public override RomFormat Convert(string source)
            {
                var romFormat = RomFormat.None;
                switch (source)
                {
                    case "BIN+CFG":
                        break;
                    case "ROM":
                        break;
                    case "LUIGI":
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("source");
                }
                return romFormat;
            }
        }

        private class RomTypeStringToGeneralFeaturesConverter : Converter<RomTypeStringToGeneralFeaturesConverter, string, GeneralFeatures>
        {
            /// <inheritdoc />
            public override GeneralFeatures Convert(string source)
            {
                // Database schema info: enum('BIOS','Program') DEFAULT 'Program'
                var generalFeatures = GeneralFeatures.None;
                switch (source)
                {
                    case "BIOS":
                        generalFeatures |= GeneralFeatures.SystemRom;
                        break;
                    case "Program":
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return generalFeatures;
            }
        }

        private class StringToRawFeatureBitsConverter : Converter<StringToRawFeatureBitsConverter, string, short>
        {
            /// <inheritdoc />
            public override short Convert(string source)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var rawFeatureBits = short.Parse(source, NumberStyles.Integer, CultureInfo.InvariantCulture);
                    if (rawFeatureBits >= 0)
                    {
                        return rawFeatureBits;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        private abstract class StringToFeatureBitsConverter<T, TFeatureBits> : Converter<T, string, TFeatureBits> where T : IConverter<string, TFeatureBits>, new()
        {
            /// <inheritdoc />
            protected uint ConvertCore(string source, uint mask)
            {
                var rawFeatureBits = StringToRawFeatureBitsConverter.Instance.Convert(source);
                var featureBits = System.Convert.ToUInt32(rawFeatureBits) & mask;
                return featureBits;
            }
        }

        private class StringToFeatureCompatibilityConverter : StringToFeatureBitsConverter<StringToFeatureCompatibilityConverter, FeatureCompatibility>
        {
            /// <inheritdoc />
            public override FeatureCompatibility Convert(string source)
            {
                var featureCompatibilty = (FeatureCompatibility)ConvertCore(source, FeatureCompatibilityHelpers.CompatibilityMask);
                return featureCompatibilty;
            }
        }

        private class StringToGeneralFeaturesConverter : StringToFeatureBitsConverter<StringToGeneralFeaturesConverter, GeneralFeatures>
        {
            /// <inheritdoc />
            public override GeneralFeatures Convert(string source)
            {
                var generalFeatures = (GeneralFeatures)ConvertCore(source, GeneralFeaturesHelpers.FeaturesMask);
                return generalFeatures;
            }
        }

        private class StringToKeyboardComponentFeaturesConverter : StringToFeatureBitsConverter<StringToKeyboardComponentFeaturesConverter, KeyboardComponentFeatures>
        {
            /// <inheritdoc />
            public override KeyboardComponentFeatures Convert(string source)
            {
                var keyboardComponentFeatures = (KeyboardComponentFeatures)ConvertCore(source, KeyboardComponentFeaturesHelpers.FeaturesMask);
                return keyboardComponentFeatures;
            }
        }

        private class StringToEcsFeaturesConverter : StringToFeatureBitsConverter<StringToEcsFeaturesConverter, EcsFeatures>
        {
            /// <inheritdoc />
            public override EcsFeatures Convert(string source)
            {
                var ecsFeatures = (EcsFeatures)ConvertCore(source, EcsFeaturesHelpers.FeaturesMask);
                return ecsFeatures;
            }
        }

        private class StringToIntellicartCC3FeaturesConverter : StringToFeatureBitsConverter<StringToIntellicartCC3FeaturesConverter, IntellicartCC3Features>
        {
            /// <inheritdoc />
            public override IntellicartCC3Features Convert(string source)
            {
                var intellicartCC3Features = (IntellicartCC3Features)ConvertCore(source, IntellicartCC3FeaturesHelpers.FeaturesMask);
                return intellicartCC3Features;
            }
        }

        private class StringToCuttleCart3Features : StringToFeatureBitsConverter<StringToCuttleCart3Features, CuttleCart3Features>
        {
            /// <inheritdoc />
            public override CuttleCart3Features Convert(string source)
            {
                var cuttleCart3Features = (CuttleCart3Features)ConvertCore(source, CuttleCart3FeaturesHelpers.FeaturesMask);
                return cuttleCart3Features;
            }
        }

        private class StringToJlpFeaturesConverter : StringToFeatureBitsConverter<StringToJlpFeaturesConverter, JlpFeatures>
        {
            /// <inheritdoc />
            public override JlpFeatures Convert(string source)
            {
                var jlpFeatures = (JlpFeatures)ConvertCore(source, JlpFeaturesHelpers.FeaturesMask);
                return jlpFeatures;
            }
        }

        private class StringToMinimumFlashSectorsConverter : Converter<StringToMinimumFlashSectorsConverter, string, ushort>
        {
            /// <inheritdoc />
            public override ushort Convert(string source)
            {
                var value = ushort.Parse(source, CultureInfo.InvariantCulture);
                if (value > JlpFeaturesHelpers.JlpFlashBaseSaveDataSectorsCountMask)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return value;
            }
        }

        private class StringToLtoFlashFeaturesConverter : StringToFeatureBitsConverter<StringToLtoFlashFeaturesConverter, LtoFlashFeatures>
        {
            /// <inheritdoc />
            public override LtoFlashFeatures Convert(string source)
            {
                var ltoFlashFeatures = (LtoFlashFeatures)ConvertCore(source, LtoFlashFeaturesHelpers.FeaturesMask);
                return ltoFlashFeatures;
            }
        }

        private class StringToBee3FeaturesConverter : StringToFeatureBitsConverter<StringToBee3FeaturesConverter, Bee3Features>
        {
            /// <inheritdoc />
            public override Bee3Features Convert(string source)
            {
                var bee3Features = (Bee3Features)ConvertCore(source, Bee3FeaturesHelpers.FeaturesMask);
                return bee3Features;
            }
        }

        private class StringToHiveFeaturesConverter : StringToFeatureBitsConverter<StringToHiveFeaturesConverter, HiveFeatures>
        {
            /// <inheritdoc />
            public override HiveFeatures Convert(string source)
            {
                var hiveFeatures = (HiveFeatures)ConvertCore(source, HiveFeaturesHelpers.FeaturesMask);
                return hiveFeatures;
            }
        }

        private class StringToStringEnumerableConverter : Converter<StringToStringEnumerableConverter, string, IEnumerable<string>>
        {
            /// <inheritdoc />
            public override IEnumerable<string> Convert(string source)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var strings = source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.DecodeHtmlString().Trim()).Where(s => s.Length > 0);
                    if (strings.Count() == 0)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    return strings;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            /// <summary>
            /// Splits the given string using the '|' character, limiting entries to <paramref name="maxLength"/> and the GROM character set.
            /// </summary>
            /// <param name="source">The string to convert.</param>
            /// <param name="maxLength">The maximum length of any entry in the returned enumerable.</param>
            /// <returns>An enumerable of strings.</returns>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="source"/> is null or empty, or no viable entries exist.</exception>
            public IEnumerable<string> Convert(string source, int maxLength)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var strings = source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.DecodeHtmlString().Trim().EnforceNameLength(maxLength, restrictToGromCharacters: true)).Where(s => s.Length > 0);
                    if (strings.Count() == 0)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    return strings;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private class StringToMetadataDateTimeConverter : Converter<StringToMetadataDateTimeConverter, string, IEnumerable<MetadataDateTime>>
        {
            public override IEnumerable<MetadataDateTime> Convert(string source)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var dateParts = source.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (dateParts.Length == 3)
                    {
                        var dateTimeOffset = DateTimeOffset.ParseExact(source, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        var dateTime = new MetadataDateTime(dateTimeOffset, MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Month | MetadataDateTimeFlags.Day);
                        yield return dateTime;
                    }
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        #endregion // Converters
    }
}
