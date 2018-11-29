// <copyright file="XmlRomInformationHelpersTests.Data.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;

namespace INTV.Core.Tests.Model.Program
{
    /// <summary>
    /// Test data for XmlRomInformationHelpersTests.
    /// </summary>
    public partial class XmlRomInformationHelpersTests
    {
        #region Get Features Test Data

        public static IEnumerable<object[]> FeatureCompatibilityTestData
        {
            get
            {
                yield return new object[] { null, FeatureCompatibility.Tolerates };
                yield return new object[] { string.Empty, FeatureCompatibility.Tolerates };
                yield return new object[] { " d ", FeatureCompatibility.Tolerates };
                yield return new object[] { "0", FeatureCompatibility.Incompatible };
                yield return new object[] { "1", FeatureCompatibility.Tolerates };
                yield return new object[] { "2", FeatureCompatibility.Enhances };
                yield return new object[] { "3", FeatureCompatibility.Requires };
                yield return new object[] { "4", FeatureCompatibility.Incompatible };
                yield return new object[] { "5", FeatureCompatibility.Tolerates };
                yield return new object[] { "32", FeatureCompatibility.Incompatible };
            }
        }

        public static IEnumerable<object[]> NtscPalCompatibilityTestData
        {
            get
            {
                yield return new object[] { null, FeatureCompatibility.Tolerates };
                yield return new object[] { string.Empty, FeatureCompatibility.Tolerates };
                yield return new object[] { " d ", FeatureCompatibility.Tolerates };
                yield return new object[] { "0", FeatureCompatibility.Incompatible };
                yield return new object[] { "1", FeatureCompatibility.Tolerates };
                yield return new object[] { "2", FeatureCompatibility.Enhances };
                yield return new object[] { "3", FeatureCompatibility.Enhances }; // for NTSC and PAL, Requires is coerced to Enhances
                yield return new object[] { "4", FeatureCompatibility.Incompatible };
                yield return new object[] { "5", FeatureCompatibility.Tolerates };
                yield return new object[] { "32", FeatureCompatibility.Incompatible };
            }
        }

        public static IEnumerable<object[]> GeneralFeaturesTestData
        {
            get
            {
                yield return new object[] { null, GeneralFeatures.None };
                yield return new object[] { string.Empty, GeneralFeatures.None };
                yield return new object[] { " d ", GeneralFeatures.None };
                yield return new object[] { "0", GeneralFeatures.None };
                yield return new object[] { "1", GeneralFeatures.UnrecognizedRom };
                yield return new object[] { "2", GeneralFeatures.PageFlipping };
                yield return new object[] { "3", GeneralFeatures.UnrecognizedRom | GeneralFeatures.PageFlipping };
                yield return new object[] { "4", GeneralFeatures.OnboardRam };
                yield return new object[] { "5", GeneralFeatures.UnrecognizedRom | GeneralFeatures.OnboardRam };
                yield return new object[] { "6", GeneralFeatures.PageFlipping | GeneralFeatures.OnboardRam };
                yield return new object[] { "7", GeneralFeatures.UnrecognizedRom | GeneralFeatures.PageFlipping | GeneralFeatures.OnboardRam };
                yield return new object[] { "8", GeneralFeatures.None };
                yield return new object[] { "2147483648", GeneralFeatures.None }; // the type column is used for System ROM
                yield return new object[] { "4294967295", GeneralFeatures.None }; // the type column is used for System ROM
            }
        }

        public static IEnumerable<object[]> KeyboardComponentFeaturesTestData
        {
            get
            {
                yield return new object[] { null, KeyboardComponentFeatures.Tolerates };
                yield return new object[] { string.Empty, KeyboardComponentFeatures.Tolerates };
                yield return new object[] { " kc 1149 ", KeyboardComponentFeatures.Tolerates };
                yield return new object[] { "0", KeyboardComponentFeatures.Incompatible };
                yield return new object[] { "1", KeyboardComponentFeatures.Tolerates };
                yield return new object[] { "2", KeyboardComponentFeatures.Enhances };
                yield return new object[] { "3", KeyboardComponentFeatures.Requires };
                yield return new object[] { "4", KeyboardComponentFeatures.TapeOptional };
                yield return new object[] { "8", KeyboardComponentFeatures.TapeRequired };
                yield return new object[] { "16", KeyboardComponentFeatures.Microphone };
                yield return new object[] { "32", KeyboardComponentFeatures.BasicIncompatible };
                yield return new object[] { "64", KeyboardComponentFeatures.BasicTolerated };
                yield return new object[] { "128", KeyboardComponentFeatures.BasicRequired };
                yield return new object[] { "256", KeyboardComponentFeatures.Printer };
                yield return new object[] { "512", KeyboardComponentFeatures.Incompatible };
                yield return new object[] { "2147483648", KeyboardComponentFeatures.Tolerates };
                yield return new object[] { "4294967295", KeyboardComponentFeatures.Tolerates };
            }
        }

        public static IEnumerable<object[]> EcsFeaturesTestData
        {
            get
            {
                yield return new object[] { null, EcsFeatures.Tolerates };
                yield return new object[] { string.Empty, EcsFeatures.Tolerates };
                yield return new object[] { " ecs ", EcsFeatures.Tolerates };
                yield return new object[] { "0", EcsFeatures.Incompatible };
                yield return new object[] { "1", EcsFeatures.Tolerates };
                yield return new object[] { "2", EcsFeatures.Enhances };
                yield return new object[] { "3", EcsFeatures.Requires };
                yield return new object[] { "4", EcsFeatures.Synthesizer };
                yield return new object[] { "8", EcsFeatures.Tape };
                yield return new object[] { "16", EcsFeatures.Printer };
                yield return new object[] { "32", EcsFeatures.SerialPortEnhanced };
                yield return new object[] { "64", EcsFeatures.SerialPortRequired };
                yield return new object[] { "128", EcsFeatures.Incompatible };
                yield return new object[] { "256", EcsFeatures.Incompatible };
                yield return new object[] { "512", EcsFeatures.Incompatible };
                yield return new object[] { "2147483648", EcsFeatures.Tolerates };
                yield return new object[] { "4294967295", EcsFeatures.Tolerates };
            }
        }

        public static IEnumerable<object[]> IntellicartFeaturesTestData
        {
            get
            {
                yield return new object[] { null, IntellicartCC3Features.Tolerates };
                yield return new object[] { string.Empty, IntellicartCC3Features.Tolerates };
                yield return new object[] { " Intellicart ", IntellicartCC3Features.Tolerates };
                yield return new object[] { "0", IntellicartCC3Features.Incompatible };
                yield return new object[] { "1", IntellicartCC3Features.Tolerates };
                yield return new object[] { "2", IntellicartCC3Features.Enhances };
                yield return new object[] { "3", IntellicartCC3Features.Requires };
                yield return new object[] { "4", IntellicartCC3Features.Bankswitching };
                yield return new object[] { "8", IntellicartCC3Features.SixteenBitRAM };
                yield return new object[] { "16", IntellicartCC3Features.SerialPortEnhanced };
                yield return new object[] { "32", IntellicartCC3Features.SerialPortRequired };
                yield return new object[] { "64", IntellicartCC3Features.Incompatible };
                yield return new object[] { "128", IntellicartCC3Features.Incompatible };
                yield return new object[] { "256", IntellicartCC3Features.Incompatible };
                yield return new object[] { "512", IntellicartCC3Features.Incompatible };
                yield return new object[] { "2147483648", IntellicartCC3Features.Tolerates };
                yield return new object[] { "4294967295", IntellicartCC3Features.Tolerates };
            }
        }

        public static IEnumerable<object[]> CuttleCart3FeaturesTestData
        {
            get
            {
                yield return new object[] { null, CuttleCart3Features.Tolerates };
                yield return new object[] { string.Empty, CuttleCart3Features.Tolerates };
                yield return new object[] { " cc3 ", CuttleCart3Features.Tolerates };
                yield return new object[] { "0", CuttleCart3Features.Incompatible };
                yield return new object[] { "1", CuttleCart3Features.Tolerates };
                yield return new object[] { "2", CuttleCart3Features.Enhances };
                yield return new object[] { "3", CuttleCart3Features.Requires };
                yield return new object[] { "4", CuttleCart3Features.Bankswitching };
                yield return new object[] { "8", CuttleCart3Features.MattelBankswitching };
                yield return new object[] { "16", CuttleCart3Features.SixteenBitRAM };
                yield return new object[] { "32", CuttleCart3Features.EightBitRAM };
                yield return new object[] { "64", CuttleCart3Features.SerialPortEnhanced };
                yield return new object[] { "128", CuttleCart3Features.SerialPortRequired };
                yield return new object[] { "256", CuttleCart3Features.Incompatible };
                yield return new object[] { "512", CuttleCart3Features.Incompatible };
                yield return new object[] { "2147483648", CuttleCart3Features.Tolerates };
                yield return new object[] { "4294967295", CuttleCart3Features.Tolerates };
            }
        }

        public static IEnumerable<object[]> JlpFeaturesTestData
        {
            get
            {
                yield return new object[] { null, JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { string.Empty, JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { " cc3 ", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "0", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "1", JlpFeatures.Tolerates, JlpHardwareVersion.None };
                yield return new object[] { "2", JlpFeatures.Enhances, JlpHardwareVersion.None };
                yield return new object[] { "3", JlpFeatures.Requires, JlpHardwareVersion.None };
                yield return new object[] { "4", JlpFeatures.SaveDataOptional, JlpHardwareVersion.None };
                yield return new object[] { "8", JlpFeatures.SaveDataRequired, JlpHardwareVersion.None };
                yield return new object[] { "16", JlpFeatures.Bankswitching, JlpHardwareVersion.None };
                yield return new object[] { "32", JlpFeatures.SixteenBitRAM, JlpHardwareVersion.None };
                yield return new object[] { "64", JlpFeatures.SerialPortEnhanced, JlpHardwareVersion.None };
                yield return new object[] { "128", JlpFeatures.SerialPortRequired, JlpHardwareVersion.None };
                yield return new object[] { "256", JlpFeatures.UsesLEDs, JlpHardwareVersion.None };
                yield return new object[] { "512", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "1024", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "1536", JlpFeatures.Incompatible, JlpHardwareVersion.Jlp03 };
                yield return new object[] { "2048", JlpFeatures.Incompatible, JlpHardwareVersion.Jlp04 };
                yield return new object[] { "2560", JlpFeatures.Incompatible, JlpHardwareVersion.Jlp05 };
                yield return new object[] { "3072", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "4096", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "5120", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "6144", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "7168", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "2147483648", JlpFeatures.Incompatible, JlpHardwareVersion.None };
                yield return new object[] { "4294967295", JlpFeatures.Incompatible, JlpHardwareVersion.None };
            }
        }

        public static IEnumerable<object[]> LtoFlashFeaturesTestData
        {
            get
            {
                yield return new object[] { null, LtoFlashFeatures.Incompatible };
                yield return new object[] { string.Empty, LtoFlashFeatures.Incompatible };
                yield return new object[] { " cc3 ", LtoFlashFeatures.Incompatible };
                yield return new object[] { "0", LtoFlashFeatures.Incompatible };
                yield return new object[] { "1", LtoFlashFeatures.Tolerates };
                yield return new object[] { "2", LtoFlashFeatures.Enhances };
                yield return new object[] { "3", LtoFlashFeatures.Requires };
                yield return new object[] { "4", LtoFlashFeatures.SaveDataOptional };
                yield return new object[] { "8", LtoFlashFeatures.SaveDataRequired };
                yield return new object[] { "16", LtoFlashFeatures.Bankswitching };
                yield return new object[] { "32", LtoFlashFeatures.SixteenBitRAM };
                yield return new object[] { "64", LtoFlashFeatures.UsbPortEnhanced };
                yield return new object[] { "128", LtoFlashFeatures.UsbPortRequired };
                yield return new object[] { "256", LtoFlashFeatures.LtoFlashMemoryMapped };
                yield return new object[] { "512", LtoFlashFeatures.Incompatible };
                yield return new object[] { "262144", LtoFlashFeatures.Incompatible };
                yield return new object[] { "2147483648", LtoFlashFeatures.Incompatible };
                yield return new object[] { "4294967295", LtoFlashFeatures.Incompatible };
            }
        }

        public static IEnumerable<object[]> Bee3FeaturesTestData
        {
            get
            {
                yield return new object[] { null, Bee3Features.Incompatible };
                yield return new object[] { string.Empty, Bee3Features.Incompatible };
                yield return new object[] { " cc3 ", Bee3Features.Incompatible };
                yield return new object[] { "0", Bee3Features.Incompatible };
                yield return new object[] { "1", Bee3Features.Tolerates };
                yield return new object[] { "2", Bee3Features.Enhances };
                yield return new object[] { "3", Bee3Features.Requires };
                yield return new object[] { "4", Bee3Features.SaveDataOptional };
                yield return new object[] { "8", Bee3Features.SaveDataRequired };
                yield return new object[] { "16", Bee3Features.SixteenBitRAM };
                yield return new object[] { "32", Bee3Features.Incompatible };
                yield return new object[] { "64", Bee3Features.Incompatible };
                yield return new object[] { "128", Bee3Features.Incompatible };
                yield return new object[] { "256", Bee3Features.Incompatible };
                yield return new object[] { "2147483648", Bee3Features.Incompatible };
                yield return new object[] { "4294967295", Bee3Features.Incompatible };
            }
        }

        public static IEnumerable<object[]> HiveFeaturesTestData
        {
            get
            {
                yield return new object[] { null, HiveFeatures.Incompatible };
                yield return new object[] { string.Empty, HiveFeatures.Incompatible };
                yield return new object[] { " cc3 ", HiveFeatures.Incompatible };
                yield return new object[] { "0", HiveFeatures.Incompatible };
                yield return new object[] { "1", HiveFeatures.Tolerates };
                yield return new object[] { "2", HiveFeatures.Enhances };
                yield return new object[] { "3", HiveFeatures.Requires };
                yield return new object[] { "4", HiveFeatures.SaveDataOptional };
                yield return new object[] { "8", HiveFeatures.SaveDataRequired };
                yield return new object[] { "16", HiveFeatures.SixteenBitRAM };
                yield return new object[] { "32", HiveFeatures.Incompatible };
                yield return new object[] { "64", HiveFeatures.Incompatible };
                yield return new object[] { "128", HiveFeatures.Incompatible };
                yield return new object[] { "256", HiveFeatures.Incompatible };
                yield return new object[] { "2147483648", HiveFeatures.Incompatible };
                yield return new object[] { "4294967295", HiveFeatures.Incompatible };
            }
        }

        #endregion // Get Features Test Data

        #region Set Features Test Data

        public static IEnumerable<object[]> CommonSetFeatureCompatibilityTestData
        {
            get
            {
                var values = Enum.GetValues(typeof(FeatureCompatibility)).Cast<FeatureCompatibility>().Where(c => c != FeatureCompatibility.NumCompatibilityModes);
                foreach (var value in values)
                {
                    yield return new object[] { value, ((uint)value).ToString(CultureInfo.InvariantCulture) };
                }
            }
        }

        public static IEnumerable<object[]> SetGeneralFeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(
                    GeneralFeaturesHelpers.FeaturesMask,
                    f =>
                    {
                        var flag = (GeneralFeatures)f;
                        var expectedGeneralFeatures = flag == GeneralFeatures.SystemRom ? 0u : f;
                        var expectedType = flag == GeneralFeatures.SystemRom ? XmlRomInformationDatabaseColumn.RomTypeValueSystem : XmlRomInformationDatabaseColumn.RomTypeValueRom;
                        return new object[] { flag, expectedGeneralFeatures.ToString(CultureInfo.InvariantCulture), expectedType };
                    });
            }
        }

        public static IEnumerable<object[]> SetNtscPalCompatibilityTestData
        {
            get
            {
                foreach (var testData in CommonSetFeatureCompatibilityTestData)
                {
                    var setting = (FeatureCompatibility)testData[0];
                    if (setting == FeatureCompatibility.Requires)
                    {
                        yield return new object[] { setting, ((uint)FeatureCompatibility.Enhances).ToString(CultureInfo.InvariantCulture) };
                    }
                    else
                    {
                        yield return testData;
                    }
                }
            }
        }

        public static IEnumerable<object[]> SetKeyboardComponentFeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(KeyboardComponentFeaturesHelpers.FeaturesMask, null);
            }
        }

        public static IEnumerable<object[]> SetEcsFeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(EcsFeaturesHelpers.FeaturesMask, null);
            }
        }

        public static IEnumerable<object[]> SetIntellicartFeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(IntellicartCC3FeaturesHelpers.FeaturesMask, null);
            }
        }

        public static IEnumerable<object[]> SetCuttleCart3FeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(CuttleCart3FeaturesHelpers.FeaturesMask, null);
            }
        }

        public static IEnumerable<object[]> SetJlpFeaturesTestData
        {
            get
            {
                var jlpHardwareVersions = Enum.GetValues(typeof(JlpHardwareVersion)).Cast<JlpHardwareVersion>().Except(new[] { JlpHardwareVersion.Incompatible });
                var jlpFeaturesTestData = GetValuesForSetUintFeatureFlags(JlpFeaturesHelpers.FeaturesMask, null);
                foreach (var jlpFeatureTestData in jlpFeaturesTestData)
                {
                    foreach (var jlpHardwareVersion in jlpHardwareVersions)
                    {
                        // Database reuses the flash sectors bits for hardware version, as it uses a separate field for flash sectors.
                        var bitsForString = (uint)jlpFeatureTestData[0] | ((uint)jlpHardwareVersion << JlpFeaturesHelpers.FlashSaveDataSectorsOffset);
                        yield return new object[] { jlpFeatureTestData[0], jlpHardwareVersion, bitsForString.ToString(CultureInfo.InvariantCulture) };
                    }
                }
            }
        }

        public static IEnumerable<object[]> SetJlpFlashSectorUsageCountTestData
        {
            get
            {
                yield return new object[] { (ushort)0, "-1" };
                for (var i = 0; i < JlpFeaturesHelpers.JlpFlashSaveDataSectorsBitCount; ++i)
                {
                    var sectorCount = (ushort)1 << i;
                    yield return new object[] { sectorCount, sectorCount.ToString(CultureInfo.InvariantCulture) };
                }
                yield return new object[] { (ushort)0x8000, "-1" };
            }
        }

        public static IEnumerable<object[]> SetLtoFlashFeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(
                    LtoFlashFeaturesHelpers.FeaturesMask,
                    f =>
                    {
                        // XML database repositions memory mapped flag down to first unused flash storage bit.
                        var flagsForResult = f == (uint)LtoFlashFeatures.LtoFlashMemoryMapped ? (uint)LtoFlashFeatures.FsdBit0 : f;
                        return new object[] { f, flagsForResult.ToString(CultureInfo.InvariantCulture) };
                    });
            }
        }

        public static IEnumerable<object[]> SetBee3FeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(Bee3FeaturesHelpers.FeaturesMask, null);
            }
        }

        public static IEnumerable<object[]> SetHiveFeaturesTestData
        {
            get
            {
                return GetValuesForSetUintFeatureFlags(HiveFeaturesHelpers.FeaturesMask, null);
            }
        }

        private static IEnumerable<object[]> GetValuesForSetUintFeatureFlags(uint mask, Func<uint, object[]> customGetValues)
        {
            for (var i = 0; i < sizeof(uint) * 8; ++i)
            {
                var flag = 1u << i;
                if ((flag & mask) != 0)
                {
                    if (customGetValues != null)
                    {
                        yield return customGetValues(flag);
                    }
                    else
                    {
                        yield return new object[] { flag, flag.ToString(CultureInfo.InvariantCulture) };
                    }
                }
            }
        }

        #endregion // Set Features Test Data

        #region Get Metadata Test Data

        public static IEnumerable<object[]> LongNamesTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|Pick me!|", new[] { "Pick me!" } };
                yield return new object[] { "Howdy\nDoody", new[] { "Howdy~Doody" } };
                yield return new object[] { "a|b||c", new[] { "a", "b", "c" } };
                yield return new object[] { "&amp;", new[] { "&" } };
                yield return new object[] { "This title is more than sixty characters long and that is really a shame because titles are being restricted to sixty characters in this converter", new[] { "This title is more than sixty characters long and that is re" } };
                yield return new object[] { "&nbsp;", Enumerable.Empty<string>() };
                yield return new object[] { "A|a", new[] { "A" } }; // duplicates are not retained
                yield return new object[] { "b|B|A|a", new[] { "b", "A" } }; // duplicates are not retained
           }
        }

        public static IEnumerable<object[]> ShortNamesTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|Pick me!|", new[] { "Pick me!" } };
                yield return new object[] { "Howdy\nDoody", new[] { "Howdy~Doody" } };
                yield return new object[] { "a|b||c", new[] { "a", "b", "c" } };
                yield return new object[] { "&amp;", new[] { "&" } };
                yield return new object[] { "This title is more than eighteen characters long so it will be truncated", new[] { "This title is more" } };
                yield return new object[] { "&nbsp;", Enumerable.Empty<string>() };
                yield return new object[] { "A|a", new[] { "A" } }; // duplicates are not retained
                yield return new object[] { "b|B|A|a", new[] { "b", "A" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> DescriptionsTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|This is a very short description.|", new[] { "This is a very short description." } };
                yield return new object[] { "A pretty neat game.\n\nYou are challenged to get the high score.", new[] { "A pretty neat game.\n\nYou are challenged to get the high score." } };
                yield return new object[] { "First description.|Second description.||Fourth description.", new[] { "First description.", "Second description.", "Fourth description." } };
                yield return new object[] { "Clink &amp; Clank", new[] { "Clink & Clank" } };
                yield return new object[] { "This&nbsp;has a non-breaking space\nand some newlines\nto test things out.", new[] { "This has a non-breaking space\nand some newlines\nto test things out." } };
                yield return new object[] { "A description|a Description", new[] { "A description" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> VendorsTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|Mattel Electronics|", new[] { "Mattel Electronics" } };
                yield return new object[] { "Imagic", new[] { "Imagic" } };
                yield return new object[] { "Mattel Electronics|Sears||INTV Corp.", new[] { "Mattel Electronics", "Sears", "INTV Corp." } };
                yield return new object[] { "Bob &amp; Betsy", new[] { "Bob & Betsy" } };
                yield return new object[] { "Activision||activision", new[] { "Activision" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> ContributorsTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|Contributor|", new[] { "Contributor" } };
                yield return new object[] { "Contributor", new[] { "Contributor" } };
                yield return new object[] { "Contributor 0|Contributor 1||Contributor 3", new[] { "Contributor 0", "Contributor 1", "Contributor 3" } };
                yield return new object[] { "Con &amp; Tributor", new[] { "Con & Tributor" } };
                yield return new object[] { "Contributor||CONTRIBUTOR", new[] { "Contributor" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> MetadataDateTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<MetadataDateTime>() };
                yield return new object[] { string.Empty, Enumerable.Empty<MetadataDateTime>() };
                yield return new object[] { "1979", Enumerable.Empty<MetadataDateTime>() };
                yield return new object[] { "1980/8/4", Enumerable.Empty<MetadataDateTime>() };
                yield return new object[] { "1981-8-4", Enumerable.Empty<MetadataDateTime>() };
                yield return new object[] { "1982-08-04", new[] { new MetadataDateTimeBuilder(1982).WithDay(4).WithMonth(8).Build() } };
                yield return new object[] { "1983-08", Enumerable.Empty<MetadataDateTime>() };
                yield return new object[] { "1983-18", Enumerable.Empty<MetadataDateTime>() };
            }
        }

        public static IEnumerable<object[]> LicensesTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|GPLv2|", new[] { "GPLv2" } };
                yield return new object[] { "GPLv3+", new[] { "GPLv3+" } };
                yield return new object[] { "GPLv2|GPLv2+||GPLv3+", new[] { "GPLv2", "GPLv2+", "GPLv3+" } };
                yield return new object[] { "CC BY-NC-SA||cc by-nc-sa", new[] { "CC BY-NC-SA" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> ContactInfoTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|a@b.com|", new[] { "a@b.com" } };
                yield return new object[] { "c@d.com", new[] { "c@d.com" } };
                yield return new object[] { "|Dead Letter Office <deadletter@defunct.com>|INTV Funhouse||.com", new[] { "Dead Letter Office <deadletter@defunct.com>", "INTV Funhouse", ".com" } };
                yield return new object[] { "Hankster||hankster", new[] { "Hankster" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> SourcesTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|BSRs|", new[] { "BSRs" } };
                yield return new object[] { "INTV Funhouse", new[] { "INTV Funhouse" } };
                yield return new object[] { "|BSRs|INTV Funhouse||Electronic Games Magazine", new[] { "BSRs", "INTV Funhouse", "Electronic Games Magazine" } };
                yield return new object[] { "BSRs||bsrs", new[] { "BSRs" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> RomVariantNamesTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|Fast Version|", new[] { "Fast Version" } };
                yield return new object[] { "Original release", new[] { "Original release" } };
                yield return new object[] { "|POV Steering|Revised Steering||Car-Oriented Steering", new[] { "POV Steering", "Revised Steering", "Car-Oriented Steering" } };
                yield return new object[] { "beta 1||Beta 1", new[] { "beta 1" } }; // duplicates are not retained
            }
        }

        public static IEnumerable<object[]> OtherInformationTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<string>() };
                yield return new object[] { string.Empty, Enumerable.Empty<string>() };
                yield return new object[] { "||", Enumerable.Empty<string>() };
                yield return new object[] { "|What would you like to know?|", new[] { "What would you like to know?" } };
                yield return new object[] { "You wish you knew it all.", new[] { "You wish you knew it all." } };
                yield return new object[] { "|Nobody||likes|a know-it-all||||", new[] { "Nobody", "likes", "a know-it-all" } };
                yield return new object[] { "Really?||REALLY?", new[] { "Really?" } }; // duplicates are not retained
            }
        }

        #endregion // Get Metadata Test Data

        #region Set Metadata Test Data

        public static IEnumerable<object[]> SetLongNameMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.title, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetShortNameMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.short_name, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetPublisherMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.vendor, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2 , testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetDescriptionMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.description, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetReleaseDateMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.release_date, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetProgrammersMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.program, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetDesignersMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.concept, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetGraphicsMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.game_graphics, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetSoundEffectsMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.soundfx, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetMusicMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.music, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetVoicesMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.voices, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetDocumentationMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.game_docs, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetArtworkMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.box_art, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetVersionsMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.name, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetBuildDateMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.build_date, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetAdditionalMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.other, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetLicenseMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.license, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        public static IEnumerable<object[]> SetContactInformationMetadataTestData
        {
            get
            {
                foreach (var testData in GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName.contact_info, 4))
                {
                    yield return new object[] { testData.Item1, testData.Item2, testData.Item3 };
                }
            }
        }

        private static IEnumerable<Tuple<IProgramMetadata, XmlRomInformationDatabaseColumnName, string>> GetTestMetadataForColumn(XmlRomInformationDatabaseColumnName column, int numValues)
        {
            var strings = Enumerable.Repeat(string.Empty, 1);
            var suffixes = Enumerable.Range(0, numValues);
            var htmlEncode = true;
            var includeFirstOnly = false;
            string expectedString = null;

            for (int i = 0; i < 2; ++i)
            {
                var programMetadataBuilder = new ProgramMetadataBuilder();
                expectedString = null;

                switch (column)
                {
                    case XmlRomInformationDatabaseColumnName.title:
                        programMetadataBuilder.WithLongNames(strings);
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.short_name:
                        programMetadataBuilder.WithShortNames(strings);
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.vendor:
                        programMetadataBuilder.WithPublishers(strings);
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.description:
                        programMetadataBuilder.WithDescriptions(strings);
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.release_date:
                        if (i == 0)
                        {
                            var date = new MetadataDateTimeBuilder(1981).WithMonth(12).WithDay(25).Build();
                            programMetadataBuilder.WithReleaseDates(new[] { date });
                            expectedString = "1981-12-25";
                        }
                        else
                        {
                            var dates = suffixes.Select(d => new MetadataDateTimeBuilder(1982).WithMonth(8).WithDay(d + 1).Build());
                            programMetadataBuilder.WithReleaseDates(dates);
                            expectedString = "1982-08-01";
                        }
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.program:
                        programMetadataBuilder.WithProgrammers(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.concept:
                        programMetadataBuilder.WithDesigners(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.game_graphics:
                        programMetadataBuilder.WithGraphics(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.soundfx:
                        programMetadataBuilder.WithSoundEffects(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.music:
                        programMetadataBuilder.WithMusic(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.voices:
                        programMetadataBuilder.WithVoices(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.game_docs:
                        programMetadataBuilder.WithDocumentation(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.box_art:
                        programMetadataBuilder.WithArtwork(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.name:
                        programMetadataBuilder.WithVersions(strings);
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.build_date:
                        if (i == 0)
                        {
                            var date = new MetadataDateTimeBuilder(1980).WithMonth(8).WithDay(4).Build();
                            programMetadataBuilder.WithBuildDates(new[] { date });
                            expectedString = "1980-08-04";
                        }
                        else
                        {
                            var dates = suffixes.Select(d => new MetadataDateTimeBuilder(1984).WithMonth(6).WithDay(d + 12).Build());
                            programMetadataBuilder.WithBuildDates(dates);
                            expectedString = "1984-06-12";
                        }
                        includeFirstOnly = true;
                        break;
                    case XmlRomInformationDatabaseColumnName.other:
                        programMetadataBuilder.WithAdditionalInformation(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.license:
                        programMetadataBuilder.WithLicenses(strings);
                        break;
                    case XmlRomInformationDatabaseColumnName.contact_info:
                        programMetadataBuilder.WithContactInformation(strings);
                        htmlEncode = false;
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                if (string.IsNullOrEmpty(expectedString))
                {
                    if (includeFirstOnly)
                    {
                        strings = new[] { strings.First() };
                    }
                    if (htmlEncode)
                    {
                        expectedString = string.Join("|", strings.Select(s => HtmlEncode(s)));
                    }
                    else
                    {
                        expectedString = string.Join("|", strings);
                    }
                }
                yield return new Tuple<IProgramMetadata, XmlRomInformationDatabaseColumnName, string>(programMetadataBuilder.Build(), column, expectedString);
                strings = suffixes.Select(s => "Abbott & Costello Part " + s.ToString(CultureInfo.InvariantCulture));
            }

            yield return new Tuple<IProgramMetadata, XmlRomInformationDatabaseColumnName, string>(new ProgramMetadata(), column, includeFirstOnly ? null : string.Empty);

        }

        #endregion // Set Metadata Test Data
    }
}
