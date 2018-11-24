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
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    /// <summary>
    /// Test data for XmlRomInformationHelpersTests.
    /// </summary>
    public partial class XmlRomInformationHelpersTests
    {
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
    }
}
