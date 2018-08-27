// <copyright file="RomInformationDatabaseColumn.cs" company="INTV Funhouse">
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
using INTV.Core.Resources;
using INTV.Core.Utility;

namespace INTV.Core.Model.Program
{
    [System.Xml.Serialization.XmlType(RomInfoColumnXmlTypeName)]
    public class RomInformationDatabaseColumn
    {
        /// <summary>
        /// The XML element name of a rominfo property in the MySql database as exported by phpMyAdmin.
        /// </summary>
        internal const string RomInfoColumnXmlTypeName = "column";

        private const string RomInfoColumnNameAttributeName = "name";
        private static readonly Dictionary<RomInformationDatabaseColumnName, RomInfoColumnConverter> ColumnValueConverters = new Dictionary<RomInformationDatabaseColumnName, RomInfoColumnConverter>()
        {
            { RomInformationDatabaseColumnName.Invalid, new RomInfoColumnConverter(typeof(void), InvalidFromString, InvalidToString) },
            { RomInformationDatabaseColumnName.crc, new RomInfoColumnConverter(typeof(uint), UnsignedIntFromString, UnsignedIntToString) },
            { RomInformationDatabaseColumnName.crc_2, new RomInfoColumnConverter(typeof(uint), UnsignedIntFromString, UnsignedIntToString) },
            { RomInformationDatabaseColumnName.code, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.title, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.name, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.variant, new RomInfoColumnConverter(typeof(sbyte), SignedByteFromString, SignedByteToString) },
            { RomInformationDatabaseColumnName.platform, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) }, // enum('Intellivision')
            { RomInformationDatabaseColumnName.format, new RomInfoColumnConverter(typeof(RomFormat), RomFormatFromString, RomFormatToString) }, // enum('BIN+CFG','ROM','LUIGI','')
            { RomInformationDatabaseColumnName.type, new RomInfoColumnConverter(typeof(GeneralFeatures), GeneralFeaturesRomTypeFromString, GeneralFeaturesRomTypeToString) }, // enum('BIOS','Program') NOT NULL DEFAULT 'Program'
            { RomInformationDatabaseColumnName.origin, new RomInfoColumnConverter(typeof(IEnumerable<ProgramInformationOrigin>), DataOriginFromString, DataOriginToString) }, // set('INTV Funhouse','Intellivision Lives','manual entry','e-mail','intvname','ROM','CFG','LUIGI','Catalog','other')
            { RomInformationDatabaseColumnName.description, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.release_date, new RomInfoColumnConverter(typeof(MetadataDateTime), MetadataDateTimeFromString, MetadataDateTimeToString) },
            { RomInformationDatabaseColumnName.source, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.ntsc, new RomInfoColumnConverter(typeof(FeatureCompatibility), FeatureCompatibilityFromString, FeatureCompatibilityToString) },
            { RomInformationDatabaseColumnName.pal, new RomInfoColumnConverter(typeof(FeatureCompatibility), FeatureCompatibilityFromString, FeatureCompatibilityToString) },
            { RomInformationDatabaseColumnName.general_features, new RomInfoColumnConverter(typeof(GeneralFeatures), GeneralFeaturesFromString, GeneralFeaturesToString) },
            { RomInformationDatabaseColumnName.kc, new RomInfoColumnConverter(typeof(KeyboardComponentFeatures), KeyboardComponentFeaturesFromString, KeyboardComponentFeaturesToString) },
            { RomInformationDatabaseColumnName.sva, new RomInfoColumnConverter(typeof(FeatureCompatibility), FeatureCompatibilityFromString, FeatureCompatibilityToString) },
            { RomInformationDatabaseColumnName.ivoice, new RomInfoColumnConverter(typeof(FeatureCompatibility), FeatureCompatibilityFromString, FeatureCompatibilityToString) },
            { RomInformationDatabaseColumnName.intyii, new RomInfoColumnConverter(typeof(FeatureCompatibility), FeatureCompatibilityFromString, FeatureCompatibilityToString) },
            { RomInformationDatabaseColumnName.ecs, new RomInfoColumnConverter(typeof(EcsFeatures), EcsFeaturesFromString, EcsFeaturesToString) },
            { RomInformationDatabaseColumnName.tutor, new RomInfoColumnConverter(typeof(FeatureCompatibility), FeatureCompatibilityFromString, FeatureCompatibilityToString) },
            { RomInformationDatabaseColumnName.icart, new RomInfoColumnConverter(typeof(IntellicartCC3Features), IntellicartCC3FeaturesFromString, IntellicartCC3FeaturesToString) },
            { RomInformationDatabaseColumnName.cc3, new RomInfoColumnConverter(typeof(CuttleCart3Features), CuttleCart3FeaturesFromString, CuttleCart3FeaturesToString) },
            { RomInformationDatabaseColumnName.jlp, new RomInfoColumnConverter(typeof(JlpFeatures), JlpFeaturesFromString, JlpFeaturesToString) },
            { RomInformationDatabaseColumnName.jlp_savegame, new RomInfoColumnConverter(typeof(ushort), UnsignedShortFromString, UnsignedShortToString) },
            { RomInformationDatabaseColumnName.lto_flash, new RomInfoColumnConverter(typeof(LtoFlashFeatures), LtoFlashFeaturesFromString, LtoFlashFeaturesToString) },
            { RomInformationDatabaseColumnName.bee3, new RomInfoColumnConverter(typeof(Bee3Features), Bee3FeaturesFromString, Bee3FeaturesToString) },
            { RomInformationDatabaseColumnName.hive, new RomInfoColumnConverter(typeof(HiveFeatures), HiveFeaturesFromString, HiveFeaturesToString) },
            { RomInformationDatabaseColumnName.program, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.concept, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.game_graphics, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.soundfx, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.music, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.voices, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.game_docs, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.box_art, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.other, new RomInfoColumnConverter(typeof(IEnumerable<string>), StringEnumerableFromString, StringEnumerableToString) },
            { RomInformationDatabaseColumnName.bin_cfg, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.box_variant, new RomInfoColumnConverter(typeof(sbyte), SignedByteFromString, SignedByteToString) },
            { RomInformationDatabaseColumnName.screenshot, new RomInfoColumnConverter(typeof(sbyte), SignedByteFromString, SignedByteToString) },
            { RomInformationDatabaseColumnName.preview, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
            { RomInformationDatabaseColumnName.get_rom, new RomInfoColumnConverter(typeof(string), StringFromString, StringToString) },
        };

        /// <summary>
        /// Initialize a new instance of <see cref="RomInformationDatabaseColumn"/>.
        /// </summary>
        public RomInformationDatabaseColumn()
        {
        }

        [System.Xml.Serialization.XmlAttribute(RomInfoColumnNameAttributeName)]
        public string Name { get; set; }

        [System.Xml.Serialization.XmlText]
        public string Value { get; set; }

        /// <summary>
        /// Gets the value of the column as a strongly typed value.
        /// </summary>
        /// <typeparam name="T">The data type of the returned value</typeparam>
        /// <returns>The strongly-typed value represented as a string in the database column.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the column cannot be identified, or a converter cannot be found.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value cannot be converted, such as when it is undefined.</exception>
        /// <exception cref="FormatException">Thrown if a numeric value cannot be parsed.</exception>
        /// <remarks>When a <see cref="ArgumentOutOfRangeException"/> is thrown, the caller should reference another database to get the 'well known' default
        /// value for the requested column. For example, when requesting NTSC compatibility for The Dreadnaught Factor, the 'code' column in the containing
        /// RomInfo object should be used to locate the standard data from the 'game info' databases.</remarks>
        public T GetTypedValue<T>()
        {
            var column = Name.ToRomInfoDatabaseColumnName();
            if (column == RomInformationDatabaseColumnName.Invalid)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.RomInfoColumnConverter_InvalidColumnFormat, Name));
            }

            var converter = GetRomInfoColumnConverter(column);
            var objectValue = converter.ConvertFromString(Value);

            ValidateData(column, converter.ValueType, objectValue.GetType());
            var value = default(T);

            return value;
        }

        public void SetStringValue<T>(T value)
        {
        }

        private static void ValidateData(RomInformationDatabaseColumnName column, Type expectedType, Type valueType)
        {
            if (!expectedType.IsAssignableFrom(valueType))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.RomInfoColumnConverter_IncompatibleConverterFormat, column, expectedType, valueType));
            }
        }

        private static RomInfoColumnConverter GetRomInfoColumnConverter(RomInformationDatabaseColumnName column)
        {
            RomInfoColumnConverter converter;
            if (!ColumnValueConverters.TryGetValue(column, out converter))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.RomInfoColumnConverter_InvalidConverterFormat, column));
            }
            return converter;
        }

        private static object InvalidFromString(string rawData)
        {
            throw new InvalidOperationException();
        }

        private static string InvalidToString(object rawData)
        {
            throw new InvalidOperationException();
        }

        private static object StringFromString(string rawData)
        {
            var stringValue = rawData == null ? string.Empty : rawData;
            return stringValue;
        }

        private static string StringToString(object rawData)
        {
            var stringValue = rawData as string;
            if (stringValue == null)
            {
                stringValue = string.Empty;
            }
            return stringValue;
        }

        private static object StringEnumerableFromString(string rawData)
        {
            var strings = Enumerable.Empty<string>();
            if (!string.IsNullOrEmpty(rawData))
            {
                strings = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.DecodeHtmlString().Trim());
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            return strings;
        }

        private static string StringEnumerableToString(object rawData)
        {
            var serializedStringEnumerable = string.Empty;
            var strings = rawData as IEnumerable<string>;
            if (strings != null)
            {
                serializedStringEnumerable = string.Join("|", strings);
            }
            return serializedStringEnumerable;
        }

        private static object UnsignedIntFromString(string rawData)
        {
            var value = uint.Parse(rawData, CultureInfo.InvariantCulture);
            return value;
        }

        private static string UnsignedIntToString(object rawData)
        {
            var value = (uint)rawData;
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static object UnsignedShortFromString(string rawData)
        {
            var value = ushort.Parse(rawData, CultureInfo.InvariantCulture);
            return value;
        }

        private static string UnsignedShortToString(object rawData)
        {
            var value = (ushort)rawData;
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static object SignedShortFromString(string rawData)
        {
            var value = short.Parse(rawData, CultureInfo.InvariantCulture);
            return value;
        }

        private static string SignedShortToString(object rawData)
        {
            var value = (short)rawData;
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static object SignedByteFromString(string rawData)
        {
            var value = sbyte.Parse(rawData, CultureInfo.InvariantCulture);
            return value;
        }

        private static string SignedByteToString(object rawData)
        {
            var value = (sbyte)rawData;
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static object RomFormatFromString(string rawData)
        {
            var romFormat = RomFormat.None;
            switch (rawData)
            {
                case "BIN+CFG":
                    break;
                case "ROM":
                    break;
                case "LUIGI":
                    break;
                default:
                    throw new ArgumentOutOfRangeException("romFormat");
            }
            return romFormat;
        }

        private static string RomFormatToString(object rawData)
        {
            var databaseRomFormat = string.Empty;
            var romFormat = (RomFormat)rawData;
            switch (romFormat)
            {
                case RomFormat.Bin:
                    databaseRomFormat = "BIN+CFG";
                    break;
                case RomFormat.Rom: // a.k.a. RomFormat.Intellicart
                case RomFormat.CuttleCart3:
                case RomFormat.CuttleCart3Advanced:
                    databaseRomFormat = "ROM";
                    break;
                case RomFormat.Luigi:
                    databaseRomFormat = "LUIGI";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("romFormat");
            }
            return databaseRomFormat;
        }

        private static object MetadataDateTimeFromString(string rawData)
        {
            var dateTime = MetadataDateTime.MinValue;
            if (!string.IsNullOrEmpty(rawData))
            {
                var dateParts = rawData.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (dateParts.Length == 3)
                {
                    var dateTimeOffset = DateTimeOffset.ParseExact(rawData, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    dateTime = new MetadataDateTime(dateTimeOffset, MetadataDateTimeFlags.Year | MetadataDateTimeFlags.Month | MetadataDateTimeFlags.Day);
                }
            }
            return dateTime;
        }

        private static string MetadataDateTimeToString(object rawData)
        {
            var dateTime = (MetadataDateTime)rawData;
            var dateTimeString = string.Format(CultureInfo.InvariantCulture, "{0:0000}-{1:00}-{2:00}", dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day);
            return dateTimeString;
        }

        private static object DataOriginFromString(string rawData)
        {
            var origins = new List<ProgramInformationOrigin>();
            if (!string.IsNullOrEmpty(rawData))
            {
                // set('INTV Funhouse','Intellivision Lives','manual entry','e-mail','intvname','ROM','CFG','LUIGI','Catalog','other')
                var originStrings = rawData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var originString in originStrings)
                {
                    switch (originString)
                    {
                        case "INTV Funhouse":
                            origins.Add(ProgramInformationOrigin.IntvFunhouse);
                            break;
                        case "Intellivision Lives":
                            origins.Add(ProgramInformationOrigin.IntellivisionProductions);
                            break;
                        case "manual entry":
                            origins.Add(ProgramInformationOrigin.UserDefined);
                            break;
                        case "e-mail":
                            origins.Add(ProgramInformationOrigin.UserEmail);
                            break;
                        case "intvname":
                            origins.Add(ProgramInformationOrigin.JzIntv);
                            break;
                        case "ROM":
                            origins.Add(ProgramInformationOrigin.RomMetadataBlock);
                            break;
                        case "CFG":
                            origins.Add(ProgramInformationOrigin.CfgVarMetadataBlock);
                            break;
                        case "LUIGI":
                            origins.Add(ProgramInformationOrigin.LuigiMetadataBlock);
                            break;
                        case "Catalog":
                            origins.Add(ProgramInformationOrigin.GameCatalog);
                            break;
                        case "other":
                            origins.Add(ProgramInformationOrigin.None);
                            break;
                        default:
                            break;
                    }
                }
            }
            if (!origins.Any())
            {
                throw new ArgumentOutOfRangeException();
            }
            return origins;
        }

        private static string DataOriginToString(object rawData)
        {
            var origins = string.Empty;
            var dataOrigins = rawData as IEnumerable<ProgramInformationOrigin>;
            if (dataOrigins != null)
            {
                var originStrings = new HashSet<string>();
                foreach (var dataOrigin in dataOrigins)
                {
                    switch (dataOrigin)
                    {
                        case ProgramInformationOrigin.IntvFunhouse:
                        case ProgramInformationOrigin.Embedded:
                            originStrings.Add("INTV Funhouse");
                            break;
                        case ProgramInformationOrigin.IntellivisionProductions:
                            originStrings.Add("Intellivision Lives");
                            break;
                        case ProgramInformationOrigin.UserDefined:
                            originStrings.Add("manual entry");
                            break;
                        case ProgramInformationOrigin.UserEmail:
                            originStrings.Add("e-mail");
                            break;
                        case ProgramInformationOrigin.JzIntv:
                            originStrings.Add("intvname");
                            break;
                        case ProgramInformationOrigin.RomMetadataBlock:
                            originStrings.Add("ROM");
                            break;
                        case ProgramInformationOrigin.CfgVarMetadataBlock:
                            originStrings.Add("CFG");
                            break;
                        case ProgramInformationOrigin.LuigiMetadataBlock:
                            originStrings.Add("LUIGI");
                            break;
                        case ProgramInformationOrigin.GameCatalog:
                            originStrings.Add("Catalog");
                            break;
                        default:
                            originStrings.Add("other");
                            break;
                    }
                }
                origins = string.Join(",", originStrings);
            }
            return origins;
        }

        private static short GetRawFeatureBits(string rawData)
        {
            if (!string.IsNullOrEmpty(rawData))
            {
                var rawFeatureBits = (short)SignedShortFromString(rawData);
                if (rawFeatureBits >= 0)
                {
                    return rawFeatureBits;
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        private static object FeatureCompatibilityFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var featureBits = Convert.ToUInt32(rawFeatureBits) & FeatureCompatibilityHelpers.CompatibilityMask;
            var featureCompatibilty = (FeatureCompatibility)featureBits;
            return featureCompatibilty;
        }

        private static string FeatureCompatibilityToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var featureCompatibility = (FeatureCompatibility)rawData & FeatureCompatibilityHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)featureCompatibility);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object GeneralFeaturesRomTypeFromString(string rawData)
        {
            // Database schema info: enum('BIOS','Program') DEFAULT 'Program'
            var generalFeatures = GeneralFeatures.None;
            switch (rawData)
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

        private static string GeneralFeaturesRomTypeToString(object rawData)
        {
            if (rawData != null)
            {
                var generalFeatures = (GeneralFeatures)rawData & GeneralFeaturesHelpers.ValidFeaturesMask;
                var romType = generalFeatures.HasFlag(GeneralFeatures.SystemRom) ? "BIOS" : "Program";
                return romType;
            }
            throw new ArgumentOutOfRangeException();
        }

        private static object GeneralFeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var generalFeaturesBits = Convert.ToUInt32(rawFeatureBits) & GeneralFeaturesHelpers.FeaturesMask;
            var generalFeatures = (GeneralFeatures)generalFeaturesBits;
            return generalFeatures;
        }

        private static string GeneralFeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var generalFeatures = (GeneralFeatures)rawData & GeneralFeaturesHelpers.ValidFeaturesMask;

                // SystemRom flag is covered separately via 'rom type' column. Consider stripping out UnrecognizedRom as well?
                featureBits = Convert.ToInt16((uint)(generalFeatures & ~GeneralFeatures.SystemRom));
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object KeyboardComponentFeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var keyboardComponentFeaturesBits = Convert.ToUInt32(rawFeatureBits) & KeyboardComponentFeaturesHelpers.FeaturesMask;
            var keyboardComponentFeatures = (KeyboardComponentFeatures)keyboardComponentFeaturesBits;
            return keyboardComponentFeatures;
        }

        private static string KeyboardComponentFeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var keyboardComponentFeatures = (KeyboardComponentFeatures)rawData & KeyboardComponentFeaturesHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)keyboardComponentFeatures);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object EcsFeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var ecsFeaturesBits = Convert.ToUInt32(rawFeatureBits) & EcsFeaturesHelpers.FeaturesMask;
            var ecsFeatures = (EcsFeatures)ecsFeaturesBits;
            return ecsFeatures;
        }

        private static string EcsFeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var ecsFeatures = (EcsFeatures)rawData & EcsFeaturesHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)ecsFeatures);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object IntellicartCC3FeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var intellicartCC3FeaturesBits = Convert.ToUInt32(rawFeatureBits) & IntellicartCC3FeaturesHelpers.FeaturesMask;
            var intellicartCC3Features = (IntellicartCC3Features)intellicartCC3FeaturesBits;
            return intellicartCC3Features;
        }

        private static string IntellicartCC3FeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var intellicartCC3Features = (IntellicartCC3Features)rawData & IntellicartCC3FeaturesHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)intellicartCC3Features);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object CuttleCart3FeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var cuttleCart3FeaturesBits = Convert.ToUInt32(rawFeatureBits) & CuttleCart3FeaturesHelpers.FeaturesMask;
            var cuttleCart3Features = (CuttleCart3Features)cuttleCart3FeaturesBits;
            return cuttleCart3Features;
        }

        private static string CuttleCart3FeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var cuttleCart3Features = (CuttleCart3Features)rawData & CuttleCart3FeaturesHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)cuttleCart3Features);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object JlpFeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var jlpFeaturesBits = Convert.ToUInt32(rawFeatureBits) & JlpFeaturesHelpers.FeaturesMask;
            var jlpFeatures = (JlpFeatures)jlpFeaturesBits;
            return jlpFeatures;
        }

        private static string JlpFeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var jlpFeatures = (JlpFeatures)rawData & JlpFeaturesHelpers.ValidFeaturesMask;

                // Strip out flash save data sectors count - that's stored separately.
                featureBits = Convert.ToInt16((uint)(jlpFeatures & ~JlpFeaturesHelpers.FlashSaveDataSectorsCountMask));
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object LtoFlashFeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var ltoFlashFeaturesBits = Convert.ToUInt32(rawFeatureBits) & LtoFlashFeaturesHelpers.FeaturesMask;
            var ltoFlashFeatures = (LtoFlashFeatures)ltoFlashFeaturesBits;
            return ltoFlashFeatures;
        }

        private static string LtoFlashFeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var ltoFlashFeatures = (LtoFlashFeatures)rawData & LtoFlashFeaturesHelpers.ValidFeaturesMask;

                // Strip out flash save data sectors count - that's stored separately.
                featureBits = Convert.ToInt16((uint)(ltoFlashFeatures & ~LtoFlashFeaturesHelpers.SaveDataSectorCountMask));
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object Bee3FeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var bee3FeaturesBits = Convert.ToUInt32(rawFeatureBits) & Bee3FeaturesHelpers.FeaturesMask;
            var bee3Features = (Bee3Features)bee3FeaturesBits;
            return bee3Features;
        }

        private static string Bee3FeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var bee3Features = (Bee3Features)rawData & Bee3FeaturesHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)bee3Features);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        private static object HiveFeaturesFromString(string rawData)
        {
            var rawFeatureBits = GetRawFeatureBits(rawData);
            var hiveFeaturesBits = Convert.ToUInt32(rawFeatureBits) & HiveFeaturesHelpers.FeaturesMask;
            var hiveFeatures = (HiveFeatures)hiveFeaturesBits;
            return hiveFeatures;
        }

        private static string HiveFeaturesToString(object rawData)
        {
            short featureBits = -1;
            if (rawData != null)
            {
                var hiveFeatures = (HiveFeatures)rawData & HiveFeaturesHelpers.ValidFeaturesMask;
                featureBits = Convert.ToInt16((uint)hiveFeatures);
            }
            return featureBits.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convenience type for RomInfoColumn data conversion.
        /// </summary>
        private class RomInfoColumnConverter : Tuple<Type, Func<string, object>, Func<object, string>>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="RomInfoColumnConverter"/>.
            /// </summary>
            /// <param name="outputType">The concrete output type of the converter.</param>
            /// <param name="convertFromString">The conversion function to use to convert from a string to a concrete value.</param>
            /// <param name="convertToString">The conversion function to use to convert from a concrete value to a string.</param>
            internal RomInfoColumnConverter(Type outputType, Func<string, object> convertFromString, Func<object, string> convertToString)
                : base(outputType, convertFromString, convertToString)
            {
                if (convertFromString == null)
                {
                    throw new ArgumentNullException("convertFromString");
                }
                if (convertToString == null)
                {
                    throw new ArgumentNullException("convertToString");
                }
            }

            /// <summary>
            /// Gets the concrete data type of a column's value.
            /// </summary>
            internal Type ValueType
            {
                get { return Item1; }
            }

            /// <summary>
            /// Gets the conversion function to convert from string to data.
            /// </summary>
            internal Func<string, object> ConvertFromString
            {
                get { return Item2; }
            }

            /// <summary>
            /// Gets the conversion function to convert of data to string.
            /// </summary>
            internal Func<object, string> ConvertToString
            {
                get { return Item3; }
            }
        }
    }
}
