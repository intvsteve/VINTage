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
            var dataColumn = xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.crc, requiredColumn: true);
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
            dataColumn = xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.crc_2);
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
                dataColumn = xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.format, requiredColumn: true);
                var romFormat = StringToRomFormatConverter.Instance.Convert(dataColumn.Value);
                if (romFormat == RomFormat.Bin)
                {
                    dataColumn = xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.bin_cfg);
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
        /// Strategy to extract a ROM's format from a rominfo database entry.
        /// </summary>
        /// <param name="xmlRomInformation">A database entry from the INTV Funhouse rominfo database.</param>
        /// <returns>The ROM format of the database entry.</returns>
        /// <exception cref="System.NullReferenceException">Thrown if required database column is missing.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if database column value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if an unrecognized ROM format is specified.</exception>
        public static RomFormat GetRomFormat(this XmlRomInformation xmlRomInformation)
        {
            var dataColumn = xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.format, requiredColumn: true);
            var romFormat = StringToRomFormatConverter.Instance.Convert(dataColumn.Value);
            return romFormat;
        }

        /// <summary>
        /// Strategy to extract the INTV Funhouse database code from a rominfo database entry.
        /// </summary>
        /// <param name="xmlRomInformation">A database entry from the INTV Funhouse rominfo database.</param>
        /// <returns>The value of the 'code' column. Only non-<c>null</c>, non-empty values should be considered valid.</returns>
        public static string GetDatabaseCode(this XmlRomInformation xmlRomInformation)
        {
            var code = string.Empty;
            var dataColumn = xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.code);
            if (dataColumn != null)
            {
                code = dataColumn.Value.Trim();
            }
            return code;
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
                            programFeaturesBuilder.WithJlpHardwareVersion(StringToJlpFeaturesConverter.Instance.GetJlpHardwareVersion(column.Value));
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
        /// Sets the ROM features-related columns given an existing set of <see cref="IProgramFeatures"/>.
        /// </summary>
        /// <param name="xmlRomInformation">The instance of <see cref="XmlRomInformation"/> whose features-related column values are to be set.</param>
        /// <param name="features">The features to use to set the column values.</param>
        public static void SetProgramFeatures(this XmlRomInformation xmlRomInformation, IProgramFeatures features)
        {
            if (features != null)
            {
                foreach (var column in xmlRomInformation.RomInfoDatabaseColumns)
                {
                    var columnName = column.Name.ToRomInfoDatabaseColumnName();
                    try
                    {
                        switch (columnName)
                        {
                            case XmlRomInformationDatabaseColumnName.ntsc:
                                column.Value = FeatureCompatibilityToStringConverter.Instance.Convert(features.Ntsc);
                                break;
                            case XmlRomInformationDatabaseColumnName.pal:
                                column.Value = FeatureCompatibilityToStringConverter.Instance.Convert(features.Pal);
                                break;
                            case XmlRomInformationDatabaseColumnName.general_features:
                                column.Value = GeneralFeaturesToStringConverter.Instance.Convert(features.GeneralFeatures);
                                break;
                            case XmlRomInformationDatabaseColumnName.kc:
                                column.Value = KeyboardComponentFeaturesToStringConverter.Instance.Convert(features.KeyboardComponent);
                                break;
                            case XmlRomInformationDatabaseColumnName.sva:
                                column.Value = FeatureCompatibilityToStringConverter.Instance.Convert(features.SuperVideoArcade);
                                break;
                            case XmlRomInformationDatabaseColumnName.ivoice:
                                column.Value = FeatureCompatibilityToStringConverter.Instance.Convert(features.Intellivoice);
                                break;
                            case XmlRomInformationDatabaseColumnName.intyii:
                                column.Value = FeatureCompatibilityToStringConverter.Instance.Convert(features.IntellivisionII);
                                break;
                            case XmlRomInformationDatabaseColumnName.ecs:
                                column.Value = EcsFeaturesToStringConverter.Instance.Convert(features.Ecs);
                                break;
                            case XmlRomInformationDatabaseColumnName.tutor:
                                column.Value = FeatureCompatibilityToStringConverter.Instance.Convert(features.Tutorvision);
                                break;
                            case XmlRomInformationDatabaseColumnName.icart:
                                column.Value = IntellicartCC3FeaturesToStringConverter.Instance.Convert(features.Intellicart);
                                break;
                            case XmlRomInformationDatabaseColumnName.cc3:
                                column.Value = CuttleCart3FeaturesToStringConverter.Instance.Convert(features.CuttleCart3);
                                break;
                            case XmlRomInformationDatabaseColumnName.jlp:
                                column.Value = JlpFeaturesToStringConverter.Instance.Convert(features.Jlp, features.JlpHardwareVersion);
                                break;
                            case XmlRomInformationDatabaseColumnName.jlp_savegame:
                                column.Value = MinimumFlashSectorsToStringConverter.Instance.Convert(features.JlpFlashMinimumSaveSectors);
                                break;
                            case XmlRomInformationDatabaseColumnName.lto_flash:
                                column.Value = LtoFlashFeaturesToStringConverter.Instance.Convert(features.LtoFlash);
                                break;
                            case XmlRomInformationDatabaseColumnName.bee3:
                                column.Value = Bee3FeaturesToStringConverter.Instance.Convert(features.Bee3);
                                break;
                            case XmlRomInformationDatabaseColumnName.hive:
                                column.Value = HiveFeaturesToStringConverter.Instance.Convert(features.Hive);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (FormatException)
                    {
                    }
                    catch (OverflowException)
                    {
                    }
                }
            }
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

        /// <summary>
        /// Sets the ROM metadata-related columns given an existing <see cref="IProgramMetadata"/>.
        /// </summary>
        /// <param name="xmlRomInformation">The instance of <see cref="XmlRomInformation"/> whose metadata-related column values are to be set.</param>
        /// <param name="metadata">The metadata to use to set the column values.</param>
        public static void SetProgramMetadata(this XmlRomInformation xmlRomInformation, IProgramMetadata metadata)
        {
            if (metadata != null)
            {
                foreach (var column in xmlRomInformation.RomInfoDatabaseColumns)
                {
                    var columnName = column.Name.ToRomInfoDatabaseColumnName();
                    try
                    {
                        switch (columnName)
                        {
                            case XmlRomInformationDatabaseColumnName.title:
                                if (string.IsNullOrEmpty(column.Value) && metadata.LongNames.Any())
                                {
                                    column.Value = metadata.LongNames.First().EncodeHtmlString();
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.short_name:
                                if (string.IsNullOrEmpty(column.Value) && metadata.ShortNames.Any())
                                {
                                    column.Value = metadata.ShortNames.First().EncodeHtmlString();
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.vendor:
                                if (string.IsNullOrEmpty(column.Value) && metadata.Publishers.Any())
                                {
                                    column.Value = metadata.Publishers.First().EncodeHtmlString();
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.description:
                                if (string.IsNullOrEmpty(column.Value) && metadata.Descriptions.Any())
                                {
                                    column.Value = metadata.Descriptions.First().EncodeHtmlString();
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.release_date:
                                if (metadata.ReleaseDates.Any())
                                {
                                    column.Value = MetadataDateTimeToStringConverter.Instance.Convert(metadata.ReleaseDates.First());
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.program:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Programmers);
                                break;
                            case XmlRomInformationDatabaseColumnName.concept:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Designers);
                                break;
                            case XmlRomInformationDatabaseColumnName.game_graphics:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Graphics);
                                break;
                            case XmlRomInformationDatabaseColumnName.soundfx:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.SoundEffects);
                                break;
                            case XmlRomInformationDatabaseColumnName.music:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Music);
                                break;
                            case XmlRomInformationDatabaseColumnName.voices:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Voices);
                                break;
                            case XmlRomInformationDatabaseColumnName.game_docs:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Documentation);
                                break;
                            case XmlRomInformationDatabaseColumnName.box_art:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.Artwork);
                                break;
                            case XmlRomInformationDatabaseColumnName.name:
                                if (string.IsNullOrEmpty(column.Value) && metadata.Versions.Any())
                                {
                                    column.Value = metadata.Versions.First().EncodeHtmlString();
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.build_date:
                                if (metadata.BuildDates.Any())
                                {
                                    column.Value = MetadataDateTimeToStringConverter.Instance.Convert(metadata.BuildDates.First());
                                }
                                break;
                            case XmlRomInformationDatabaseColumnName.other:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.AdditionalInformation);
                                break;
                            case XmlRomInformationDatabaseColumnName.license:
                                break;
                            case XmlRomInformationDatabaseColumnName.contact_info:
                                column.Value = StringEnumerableToStringConverter.Instance.Convert(metadata.ContactInformation);
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
            }
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
                        romFormat = RomFormat.Bin;
                        break;
                    case "ROM":
                        // NOTE: This is a catch-all for .ROM and its sub-types (CC3 and CC3-Advanced).
                        romFormat = RomFormat.Rom;
                        break;
                    case "LUIGI":
                        romFormat = RomFormat.Luigi;
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

        #region String to Features Converters

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
            /// <summary>
            /// Performs the core conversion of a string to raw bits, applying the given bit mask.
            /// </summary>
            /// <param name="source">The string to convert.</param>
            /// <param name="mask">The mask to apply to the bits parsed from the string.</param>
            /// <returns>The raw feature bits.</returns>
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

            /// <summary>
            /// Extracts JLP hardware revision that may have been encoded into a numeric string.
            /// </summary>
            /// <param name="source">The string that contains encoded JLP feature bits and possibly hardware revision.</param>
            /// <returns>The hardware revision.</returns>
            /// <exception cref="System.ArgumentException">Thrown if the string describes a raw bits value that is invalid.</exception>
            /// <exception cref="System.FormatException">Thrown if the string cannot be parsed.</exception>
            /// <exception cref="System.OverflowException">Thrown if the raw bits are out of range.</exception>
            public JlpHardwareVersion GetJlpHardwareVersion(string source)
            {
                var rawFeatureBits = StringToRawFeatureBitsConverter.Instance.Convert(source);
                var jlpFeatureBits = System.Convert.ToUInt32(rawFeatureBits);
                var jlpHardwareVersionBits = jlpFeatureBits >> JlpFeaturesHelpers.FlashSaveDataSectorsOffset;
                if ((jlpHardwareVersionBits >= (uint)JlpHardwareVersion.Jlp03) && (jlpHardwareVersionBits <= (uint)JlpHardwareVersion.Jlp05))
                {
                    return (JlpHardwareVersion)jlpHardwareVersionBits;
                }
                throw new ArgumentOutOfRangeException();
            }
        }

        private class StringToMinimumFlashSectorsConverter : Converter<StringToMinimumFlashSectorsConverter, string, ushort>
        {
            /// <inheritdoc />
            public override ushort Convert(string source)
            {
                var value = ushort.Parse(source, CultureInfo.InvariantCulture);
                if (value > JlpFeaturesHelpers.MaxTheoreticalJlpFlashSectorUsage)
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

        #endregion // String to Features Converters

        #region Features to String Converters

        private class RawFeatureBitsToStringConverter : Converter<RawFeatureBitsToStringConverter, short, string>
        {
            /// <inheritdoc />
            public override string Convert(short source)
            {
                var stringValue = source.ToString(CultureInfo.InvariantCulture);
                return stringValue;
            }
        }

        private abstract class FeatureBitsToStringConverter<T, TFeatureBits> : Converter<T, TFeatureBits, string> where T : IConverter<TFeatureBits, string>, new()
        {
            /// <inheritdoc />
            public override string Convert(TFeatureBits source)
            {
                var rawFeatureBits = GetRawFeatureBits(source);
                var featureBitsToConvert = System.Convert.ToInt16(rawFeatureBits);
                var featureBitsString = RawFeatureBitsToStringConverter.Instance.Convert(featureBitsToConvert);
                return featureBitsString;
            }

            /// <summary>
            /// Gets the raw feature bits to convert to a string.
            /// </summary>
            /// <param name="source">The feature bits to convert to a string.</param>
            /// <returns>The feature bits.</returns>
            protected abstract uint GetRawFeatureBits(TFeatureBits source);
        }

        private class FeatureCompatibilityToStringConverter : FeatureBitsToStringConverter<FeatureCompatibilityToStringConverter, FeatureCompatibility>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(FeatureCompatibility source)
            {
                var rawFeatureBits = source & FeatureCompatibilityHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class GeneralFeaturesToStringConverter : FeatureBitsToStringConverter<GeneralFeaturesToStringConverter, GeneralFeatures>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(GeneralFeatures source)
            {
                // System ROM is handled in a separate conversion.
                var rawFeatureBits = (source & GeneralFeaturesHelpers.ValidFeaturesMask) & ~GeneralFeatures.SystemRom;
                return (uint)rawFeatureBits;
            }
        }

        private class KeyboardComponentFeaturesToStringConverter : FeatureBitsToStringConverter<KeyboardComponentFeaturesToStringConverter, KeyboardComponentFeatures>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(KeyboardComponentFeatures source)
            {
                var rawFeatureBits = source & KeyboardComponentFeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class EcsFeaturesToStringConverter : FeatureBitsToStringConverter<EcsFeaturesToStringConverter, EcsFeatures>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(EcsFeatures source)
            {
                var rawFeatureBits = source & EcsFeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class IntellicartCC3FeaturesToStringConverter : FeatureBitsToStringConverter<IntellicartCC3FeaturesToStringConverter, IntellicartCC3Features>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(IntellicartCC3Features source)
            {
                var rawFeatureBits = source & IntellicartCC3FeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class CuttleCart3FeaturesToStringConverter : FeatureBitsToStringConverter<CuttleCart3FeaturesToStringConverter, CuttleCart3Features>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(CuttleCart3Features source)
            {
                var rawFeatureBits = source & CuttleCart3FeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class JlpFeaturesToStringConverter : FeatureBitsToStringConverter<JlpFeaturesToStringConverter, JlpFeatures>
        {
            /// <summary>
            /// Converts JLP features to a string, encoding the JLP hardware version as well.
            /// </summary>
            /// <param name="source">The JLP feature bits to convert to a string.</param>
            /// <param name="hardwareVersion">The JLP hardware version to include in the string.</param>
            /// <returns>The JLP features and hardware revision encoded as a numeric string.</returns>
            public string Convert(JlpFeatures source, JlpHardwareVersion hardwareVersion)
            {
                var rawFeatureBits = GetRawFeatureBits(source);
                var rawHardwareVersion = (uint)hardwareVersion;
                if ((rawHardwareVersion > 0) && (rawHardwareVersion <= 15))
                {
                    var hardwareVersionFeatureBits = rawHardwareVersion << JlpFeaturesHelpers.FlashSaveDataSectorsOffset; // kinda icky, but we stash flash usage separately
                    rawFeatureBits |= hardwareVersionFeatureBits;
                }
                var featureBitsToConvert = System.Convert.ToInt16(rawFeatureBits);
                var featureBitsString = RawFeatureBitsToStringConverter.Instance.Convert(featureBitsToConvert);
                return featureBitsString;
            }

            /// <inheritdoc />
            protected override uint GetRawFeatureBits(JlpFeatures source)
            {
                var rawFeatureBits = source & JlpFeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class MinimumFlashSectorsToStringConverter : Converter<MinimumFlashSectorsToStringConverter, ushort, string>
        {
            /// <inheritdoc />
            public override string Convert(ushort source)
            {
                var value = "-1";
                if ((source > 0) && (source <= JlpFeaturesHelpers.MaxJlpFlashSectorUsage))
                {
                    value = source.ToString(CultureInfo.InvariantCulture);
                }
                return value;
            }
        }

        private class LtoFlashFeaturesToStringConverter : FeatureBitsToStringConverter<LtoFlashFeaturesToStringConverter, LtoFlashFeatures>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(LtoFlashFeatures source)
            {
                var rawFeatureBits = source & LtoFlashFeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class Bee3FeaturesToStringConverter : FeatureBitsToStringConverter<Bee3FeaturesToStringConverter, Bee3Features>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(Bee3Features source)
            {
                var rawFeatureBits = source & Bee3FeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        private class HiveFeaturesToStringConverter : FeatureBitsToStringConverter<HiveFeaturesToStringConverter, HiveFeatures>
        {
            /// <inheritdoc />
            protected override uint GetRawFeatureBits(HiveFeatures source)
            {
                var rawFeatureBits = source & HiveFeaturesHelpers.ValidFeaturesMask;
                return (uint)rawFeatureBits;
            }
        }

        #endregion // Features to String Converters

        private class StringToStringEnumerableConverter : Converter<StringToStringEnumerableConverter, string, IEnumerable<string>>
        {
            /// <inheritdoc />
            public override IEnumerable<string> Convert(string source)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var strings = source.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.DecodeHtmlString().Trim()).Where(s => s.Length > 0);
                    if (!strings.Any())
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
                    if (!strings.Any())
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

        private class StringEnumerableToStringConverter : Converter<StringEnumerableToStringConverter, IEnumerable<string>, string>
        {
            /// <inheritdoc />
            public override string Convert(IEnumerable<string> source)
            {
                var serializedStringEnumerable = string.Empty;
                if ((source != null) && source.Any())
                {
                    serializedStringEnumerable = string.Join("|", source.Select(s => s.EncodeHtmlString()));
                }
                return serializedStringEnumerable;
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

        private class MetadataDateTimeToStringConverter : Converter<MetadataDateTimeToStringConverter, MetadataDateTime, string>
        {
            public override string Convert(MetadataDateTime source)
            {
                var dateTimeString = string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}", source.Date.Year, source.Date.Month, source.Date.Day);
                return dateTimeString;
            }
        }

        #endregion // Converters
    }
}
