// <copyright file="XmlRomInformationHelpersTests.cs" company="INTV Funhouse">
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;
using INTV.TestHelpers.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    /// <summary>
    /// Tests and support code for XmlRomInformationHelpers.
    /// </summary>
    public partial class XmlRomInformationHelpersTests
    {
        #region GetProgramIdentifier Tests

        [Fact]
        public void XmlRomInformation_IsNull_GetProgramIdentifierThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithNoColumns_ThrowsInvalidOperationException()
        {
            var info = new XmlRomInformation();

            Assert.Throws<InvalidOperationException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithDefaultCrcColumnValue_ThrowsInvalidOperationException()
        {
            var info = XmlRomInformation.CreateDefault();

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value);
            Assert.Throws<InvalidOperationException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithNullCrcColumnValue_ThrowsArgumentNullException()
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = null;

            Assert.Throws<ArgumentNullException>(() => info.GetProgramIdentifier());
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("a")]
        [InlineData("$")]
        [InlineData("-")]
        [InlineData("{}kjaslhedr")]
        [InlineData(" 9 09 3,")]
        public void XmlRomInformation_GetProgramIdentifierWithNonNumericCrcColumnValue_ThrowsFormatException(string nonNumericValue)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = nonNumericValue;

            Assert.Throws<FormatException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcColumnOtherColumnsDefault_ThrowsArgumentOutOfRangeException()
        {
            var info = XmlRomInformation.CreateDefault();
            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value);
            Assert.True(string.IsNullOrEmpty(info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value));

            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = "1024";

            Assert.Throws<ArgumentOutOfRangeException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcColumnNoCrc2ColumnOtherColumnsDefault_ThrowsArgumentOutOfRangeException()
        {
            var info = XmlRomInformation.CreateDefault();
            Assert.True(string.IsNullOrEmpty(info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value));

            Assert.True(info.RemoveColumn(XmlRomInformationDatabaseColumnName.crc_2));
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = "1024";

            Assert.Throws<ArgumentOutOfRangeException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcColumnAndInvalidCrc2String_ThrowsArgumentOutOfRangeException()
        {
            var info = XmlRomInformation.CreateDefault();

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value = "not a valid thing";

            Assert.Throws<ArgumentOutOfRangeException>(() => info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcColumnAndRomFormat_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "ROM";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndInvalidCrc2AndRomFormat_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value = "-2";
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "ROM";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndInvalidCrc2AndBinFormat_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();
            Assert.True(string.IsNullOrEmpty(info.GetColumn(XmlRomInformationDatabaseColumnName.bin_cfg).Value));

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value = "-2";
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "BIN+CFG";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndInvalidCrc2AndNoBinCfgColumnAndBinFormat_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            Assert.True(info.RemoveColumn(XmlRomInformationDatabaseColumnName.bin_cfg));
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value = "-2";
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "BIN+CFG";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndZeroCrc2AndContentInBinCfgColumnAndBinFormat_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();
            var cfgContent =
@"
[mapping]
$0000 - $1FFF = $5000   ;  8K to $5000 - $6FFF
$2000 - $4FFF = $9000   ; 12K to $9000 - $BFFF
$5000 - $5FFF = $D000   ;  4K to $D000 - $DFFF

[vars]
name = ""Tag-A-Long Toady""
";

            var expectedProgramIdentifier = new ProgramIdentifier(1024u, Crc32.OfBlock(Encoding.UTF8.GetBytes(cfgContent)));
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.bin_cfg).Value = cfgContent;
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "BIN+CFG";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndZeroCrc2AndStockCfgNumberInBinCfgColumnAndBinFormat_ReturnsExpectedIdentifier()
        {
            var storageAccess = XmlRomInformationHelpersTestStorageAccess.Initialize().WithStockCfgResources();
            var info = XmlRomInformation.CreateDefault();
            var stockCfgFile = 5;

            var expectedProgramIdentifier = new ProgramIdentifier(1024u, Crc32.OfFile(IRomHelpers.GetStockCfgFilePath(stockCfgFile)));
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.bin_cfg).Value = stockCfgFile.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "BIN+CFG";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndZeroCrc2AndInvalidStockCfgNumberInBinCfgColumnAndBinFormat_ReturnsExpectedIdentifier()
        {
            var storageAccess = XmlRomInformationHelpersTestStorageAccess.Initialize().WithStockCfgResources();
            var info = XmlRomInformation.CreateDefault();
            var stockCfgFile = 42;

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.bin_cfg).Value = stockCfgFile.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = "BIN+CFG";

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndValidCrc2_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();

            var expectedProgramIdentifier = new ProgramIdentifier(1024u, 2048u);
            Assert.True(info.RemoveColumn(XmlRomInformationDatabaseColumnName.bin_cfg));
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value = expectedProgramIdentifier.OtherData.ToString(CultureInfo.InvariantCulture);

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        #endregion // GetProgramIdentifier Tests

        #region GetRomFormat Tests

        [Fact]
        public void XmlRomInformation_IsNull_GetRomFormatThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.GetRomFormat());
        }

        [Fact]
        public void XmlRomInformation_GetRomFormatWithNoColumns_ThrowsInvalidOperationException()
        {
            var info = new XmlRomInformation();

            Assert.Throws<InvalidOperationException>(() => info.GetRomFormat());
        }

        [Fact]
        public void XmlRomInformation_GetRomFormatWithDefaultFormatColumnValue_ThrowsArgumentOutOfRangeException()
        {
            var info = XmlRomInformation.CreateDefault();

            Assert.True(string.IsNullOrEmpty(info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value));
            Assert.Throws<ArgumentOutOfRangeException>(() => info.GetRomFormat());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("int")]
        [InlineData("itv")]
        [InlineData("cc3")]
        [InlineData("adv")]
        [InlineData("Rom")] // yes, it's case sensitive!
        [InlineData("bin")]
        [InlineData("BIN+CFg")]
        [InlineData("luigi")]
        public void XmlRomInformation_GetRomFormatWithInvalidFormatColumnValue_ThrowsArgumentOutOfRangeException(string invalidRomFormatString)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = invalidRomFormatString;

            Assert.Throws<ArgumentOutOfRangeException>(() => info.GetRomFormat());
        }

        [Theory]
        [InlineData("ROM", RomFormat.Rom)]
        [InlineData("BIN+CFG", RomFormat.Bin)]
        [InlineData("LUIGI", RomFormat.Luigi)]
        public void XmlRomInformation_GetRomFormatWithValidFormatColumnValue_ReturnsExpectedRomFormat(string romFormatString, RomFormat expectedRomFormat)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = romFormatString;

            Assert.Equal(expectedRomFormat, info.GetRomFormat());
        }

        #endregion // GetRomFormat Tests

        #region GetDatabaseCode Tests

        [Fact]
        public void XmlRomInformation_IsNull_GetDatabaseCodeThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.GetDatabaseCode());
        }

        [Fact]
        public void XmlRomInformation_GetDatabaseCodeWithNoColumns_ReturnsEmptyCode()
        {
            var info = new XmlRomInformation();

            Assert.True(string.IsNullOrEmpty(info.GetDatabaseCode()));
        }

        [Fact]
        public void XmlRomInformation_GetDatabaseCodeWithDefaultCodeColumnValue_ReturnsEmptyCode()
        {
            var info = XmlRomInformation.CreateDefault();

            Assert.True(string.IsNullOrEmpty(info.GetColumn(XmlRomInformationDatabaseColumnName.code).Value));
            Assert.True(string.IsNullOrEmpty(info.GetDatabaseCode()));
        }

        [Fact]
        public void XmlRomInformation_GetDatabaseCodeWithCodeValueContainingLeadingAndTrailingWhitespace_WhitespaceIsTrimmed()
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.code).Value = "\n \r aabbcc \t ";

            Assert.Equal("aabbcc", info.GetDatabaseCode());
        }

        #endregion // GetDatabaseCode Tests

        #region GetProgramFeatures Tests

        [Fact]
        public void XmlRomInformation_IsNull_GetProgramFeaturesThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.GetProgramFeatures(null));
        }

        [Fact]
        public void XmlRomInformation_GetProgramFeaturesWithNoColumns_ReturnsDefaultFeatures()
        {
            var info = new XmlRomInformation();

            Assert.Equal(ProgramFeatures.DefaultFeatures, info.GetProgramFeatures(null));
        }

        [Fact]
        public void XmlRomInformation_GetProgramFeaturesWithDefaultColumns_ReturnsDefaultFeatures()
        {
            var info = XmlRomInformation.CreateDefault();

            Assert.Equal(ProgramFeatures.DefaultFeatures, info.GetProgramFeatures(null));
        }

        #region General Features Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("bios")] // case sensitive
        [InlineData("program")] // case sensitive
        [InlineData(" ")]
        [InlineData("system")]
        [InlineData("unknown ROM type")]
        public void XmlRomInformation_GetProgramFeaturesWithInvalidTypeColumnValue_ReturnsDefaultFeatures(string invalidRomType)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.type).Value = invalidRomType;

            Assert.Equal(ProgramFeatures.DefaultFeatures, info.GetProgramFeatures(null));
        }

        [Theory]
        [InlineData("BIOS", GeneralFeatures.SystemRom)] // case sensitive
        [InlineData("Program", GeneralFeatures.None)] // case sensitive
        public void XmlRomInformation_GetProgramFeaturesWithValidTypeColumnValue_ReturnsDefaultFeatures(string validRomType, GeneralFeatures expectedGeneralFeatures)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.type).Value = validRomType;

            Assert.Equal(expectedGeneralFeatures, info.GetProgramFeatures(null).GeneralFeatures);
        }

        [Theory]
        [InlineData("BIOS", GeneralFeatures.SystemRom)] // case sensitive
        [InlineData("Program", GeneralFeatures.None)] // case sensitive
        public void XmlRomInformation_GetProgramFeaturesWithFeaturesToOverrideAndValidTypeColumnValue_CorrectlyOverridesGeneralFeatures(string validRomType, GeneralFeatures expectedGeneralFeatures)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.type).Value = validRomType;

            var featuresToOverride = ProgramFeatures.DefaultFeatures.Clone();
            featuresToOverride.GeneralFeatures = featuresToOverride.GeneralFeatures ^ (~expectedGeneralFeatures & GeneralFeatures.SystemRom);

            Assert.Equal(expectedGeneralFeatures, info.GetProgramFeatures(featuresToOverride).GeneralFeatures);
        }

        [Theory]
        [MemberData("GeneralFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithGeneralFeaturesSet_ReturnsExpectedGeneralFeatures(string generalFeaturesValue, GeneralFeatures expectedGeneralFeatures)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.general_features).Value = generalFeaturesValue;

            Assert.Equal(expectedGeneralFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).GeneralFeatures);
        }

        #endregion // General Features Tests

        #region NTSC Compatibility Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ntsc")]
        [InlineData("pal")]
        [InlineData(" ")]
        [InlineData("789456")]
        public void XmlRomInformation_GetProgramFeaturesWithInvalidNtscColumnValue_ReturnsDefaultFeatures(string invalidNtscValue)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.ntsc).Value = invalidNtscValue;

            Assert.Equal(ProgramFeatures.DefaultFeatures, info.GetProgramFeatures(null));
        }

        [Theory]
        [MemberData("NtscPalCompatibilityTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithNtscCompatibilitySet_ReturnsExpectedNtscCompatibility(string ntscCompatibilityValue, FeatureCompatibility expectedNtscCompatibility)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.ntsc).Value = ntscCompatibilityValue;

            Assert.Equal(expectedNtscCompatibility, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Ntsc);
        }

        #endregion //  NTSC Compatibility Tests

        #region PAL Compatibility Tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ntsc")]
        [InlineData("pal")]
        [InlineData(" ")]
        [InlineData("789456")]
        public void XmlRomInformation_GetProgramFeaturesWithInvalidPalColumnValue_ReturnsDefaultFeatures(string invalidPalValue)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.pal).Value = invalidPalValue;

            Assert.Equal(ProgramFeatures.DefaultFeatures, info.GetProgramFeatures(null));
        }

        [Theory]
        [MemberData("NtscPalCompatibilityTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithPalCompatibilitySet_ReturnsExpectedNtscCompatibility(string palCompatibilityValue, FeatureCompatibility expectedPalCompatibility)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.pal).Value = palCompatibilityValue;

            Assert.Equal(expectedPalCompatibility, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Pal);
        }

        #endregion //  PAL Compatibility Tests

        #region Keyboard Component Features Tests

        [Theory]
        [MemberData("KeyboardComponentFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithKeyboardComponentFeaturesSet_ReturnsExpectedKeyboardComponentFeatures(string keyboardComponentFeaturesValue, KeyboardComponentFeatures expectedKeyboardComponentFeatures)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.kc).Value = keyboardComponentFeaturesValue;

            Assert.Equal(expectedKeyboardComponentFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).KeyboardComponent);
        }

        #endregion // Keyboard Component Features Tests

        #region Super Video Arcade Compatibility Tests

        [Theory]
        [MemberData("FeatureCompatibilityTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithSearsSuperVideoArcadeCompatibilitySet_ReturnsExpectedSearsSuperVideoArcadeCompatibility(string svaCompatibilityValue, FeatureCompatibility expectedSvaCompatibility)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.sva).Value = svaCompatibilityValue;

            Assert.Equal(expectedSvaCompatibility, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).SuperVideoArcade);
        }

        #endregion // Super Video Arcade Compatibility Tests

        #region Intellivoice Compatibility Tests

        [Theory]
        [MemberData("FeatureCompatibilityTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithIntellivoiceCompatibilitySet_ReturnsExpectedIntellivoiceCompatibility(string ivoiceCompatibilityValue, FeatureCompatibility expectedIvoiceCompatibility)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.ivoice).Value = ivoiceCompatibilityValue;

            Assert.Equal(expectedIvoiceCompatibility, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Intellivoice);
        }

        #endregion // Intellivoice Compatibility Tests

        #region Intellivision II Compatibility Tests

        [Theory]
        [MemberData("FeatureCompatibilityTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithIntellivisionIICompatibilitySet_ReturnsExpectedIntellivisionIICompatibility(string intellivisionIICompatibilityValue, FeatureCompatibility expectedIntellivisionIICompatibility)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.intyii).Value = intellivisionIICompatibilityValue;

            Assert.Equal(expectedIntellivisionIICompatibility, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).IntellivisionII);
        }

        #endregion // Intellivision II Compatibility Tests

        #region ECS Features Tests

        [Theory]
        [MemberData("EcsFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithEcsFeaturesSet_ReturnsExpectedEcsFeatures(string ecsFeaturesValue, EcsFeatures expectedEcsFeatures)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.ecs).Value = ecsFeaturesValue;

            Assert.Equal(expectedEcsFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Ecs);
        }

        #endregion // ECS Features Tests

        #region Tutorvision Compatibility Tests

        [Theory]
        [MemberData("FeatureCompatibilityTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithTutorvisionCompatibilitySet_ReturnsExpectedTutorvisionCompatibility(string tutorvisionCompatibilityValue, FeatureCompatibility expectedTutorvisionCompatibility)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.tutor).Value = tutorvisionCompatibilityValue;

            Assert.Equal(expectedTutorvisionCompatibility, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Tutorvision);
        }

        #endregion // Tutorvision Compatibility Tests

        #region Intellicart Features Tests

        [Theory]
        [MemberData("IntellicartFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithIntellicartFeaturesSet_ReturnsExpectedIntellicartFeatures(string intellicartFeaturesValue, IntellicartCC3Features expectedIntellicartFeatures)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.icart).Value = intellicartFeaturesValue;

            Assert.Equal(expectedIntellicartFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Intellicart);
        }

        #endregion // Intellicart Features Tests

        #region CuttleCart3 Features Tests

        [Theory]
        [MemberData("CuttleCart3FeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithCuttleCart3FeaturesSet_ReturnsExpectedCuttleCart3Features(string cc3FeaturesValue, CuttleCart3Features expectedCuttleCart3Features)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.cc3).Value = cc3FeaturesValue;

            Assert.Equal(expectedCuttleCart3Features, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).CuttleCart3);
        }

        #endregion // CuttleCart3 Features Tests

        #region // JLP Features Tests

        [Theory]
        [MemberData("JlpFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithJlpFeaturesSet_ReturnsExpectedJlpFeatures(string jlpFeaturesValue, JlpFeatures expectedJlpFeatures, JlpHardwareVersion expectedJlpHardwareVersion)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.jlp).Value = jlpFeaturesValue;

            Assert.Equal(expectedJlpFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Jlp);
            Assert.Equal(expectedJlpHardwareVersion, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).JlpHardwareVersion);
        }

        [Theory]
        [InlineData(null, (ushort)0)]
        [InlineData("", (ushort)0)]
        [InlineData("1", (ushort)1)]
        [InlineData("128", (ushort)128)]
        [InlineData("2048", (ushort)0)]
        public void XmlRomInformation_GetProgramFeaturesWithJlpFlashSaveSectorsSet_ReturnsExpectedJlpFlashSaveSectors(string jlpFlashSaveSectorsValue, ushort expectedJlpFlashSaveSectors)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.jlp_savegame).Value = jlpFlashSaveSectorsValue;

            Assert.Equal(expectedJlpFlashSaveSectors, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).JlpFlashMinimumSaveSectors);
        }

        #endregion // JLP Features Tests

        #region LTO Flash! Feature Tests

        [Theory]
        [MemberData("LtoFlashFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithLtoFlashFeaturesSet_ReturnsExpectedLtoFlashFeatures(string ltoFlashFeaturesValue, LtoFlashFeatures expectedLtoFlashFeatures)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.lto_flash).Value = ltoFlashFeaturesValue;

            Assert.Equal(expectedLtoFlashFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).LtoFlash);
        }

        #endregion // LTO Flash! Feature Tests

        #region Bee3 Features Tests

        [Theory]
        [MemberData("Bee3FeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithBee3FeaturesSet_ReturnsExpectedBee3Features(string bee3FeaturesValue, Bee3Features expectedBee3Features)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.bee3).Value = bee3FeaturesValue;

            Assert.Equal(expectedBee3Features, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Bee3);
        }

        #endregion // Bee3 Features Tests

        #region Hive Features Tests

        [Theory]
        [MemberData("HiveFeaturesTestData")]
        public void XmlRomInformation_GetProgramFeaturesWithHiveFeaturesSet_ReturnsExpectedHiveFeatures(string hiveFeaturesValue, HiveFeatures expectedHiveFeatures)
        {
            var info = XmlRomInformation.CreateDefault();
            info.GetColumn(XmlRomInformationDatabaseColumnName.hive).Value = hiveFeaturesValue;

            Assert.Equal(expectedHiveFeatures, info.GetProgramFeatures(ProgramFeatures.DefaultFeatures.Clone()).Hive);
        }

        #endregion // Hive Features Tests

        #endregion // GetProgramFeatures Tests

        #region SetProgramFeatures Tests

        [Fact]
        public void XmlRomInformation_IsNull_SetProgramFeaturesToNullDoesNotThrow()
        {
            XmlRomInformation info = null;

            info.SetProgramFeatures(null);
        }

        [Fact]
        public void XmlRomInformation_IsNull_SetProgramFeaturesToNonNullThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.SetProgramFeatures(new ProgramFeatures()));
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesWithNoColumns_LeavesFeaturesUnchanged()
        {
            var info = new XmlRomInformation();

            info.SetProgramFeatures(new ProgramFeatures());

            Assert.Equal(ProgramFeatures.DefaultFeatures, info.GetProgramFeatures(null));
        }

        #endregion // SetProgramFeatures Tests

        #region GetProgramMetadata Tests

        [Fact]
        public void XmlRomInformation_IsNull_GetProgramMetadataThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.GetProgramMetadata(null));
        }

        [Fact]
        public void XmlRomInformation_GetProgramMetadataWithNoColumns_ReturnsEmptyMetadata()
        {
            var info = new XmlRomInformation();

            var metadata = info.GetProgramMetadata(null);

            VerifyEmptyMetadata(metadata);
        }

        #endregion // GetProgramMetadata Tests

        #region SetProgramMetadata Tests

        [Fact]
        public void XmlRomInformation_IsNull_SetProgramMetadataToNullDoesNotThrow()
        {
            XmlRomInformation info = null;

            info.SetProgramMetadata(null);
        }

        [Fact]
        public void XmlRomInformation_IsNull_SetProgramMetadataToNonNullThrowsNullReferenceException()
        {
            XmlRomInformation info = null;

            Assert.Throws<NullReferenceException>(() => info.SetProgramMetadata(new FakeProgramInformation()));
        }

        [Fact]
        public void XmlRomInformation_SetProgramMetadataNoColumns_LeavesMetadataUnchanged()
        {
            var info = new XmlRomInformation();
            VerifyEmptyMetadata(info.GetProgramMetadata(null));

            info.SetProgramMetadata(new FakeProgramInformation());

            VerifyEmptyMetadata(info.GetProgramMetadata(null));
        }

        #endregion // SetProgramMetadata Tests

        private void VerifyEmptyMetadata(IProgramMetadata metadata)
        {
            var propertyInfos = metadata.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var metadataValues = propertyInfo.GetValue(metadata) as IEnumerable;
                var n = 0;
                var i = metadataValues.GetEnumerator();
                while (n == 0 && i.MoveNext())
                {
                    ++n;
                }
                Assert.Equal(0, n);
            }
        }

        private class FakeProgramInformation : ProgramInformation
        {
            public override ProgramInformationOrigin DataOrigin
            {
                get { throw new NotImplementedException(); }
            }

            public override string Title
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override string Year
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override ProgramFeatures Features
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override System.Collections.Generic.IEnumerable<CrcData> Crcs
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> LongNames
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> ShortNames
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Descriptions
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Publishers
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Programmers
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Designers
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Graphics
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Music
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> SoundEffects
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Voices
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Documentation
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Artwork
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<Core.Model.MetadataDateTime> ReleaseDates
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Licenses
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> ContactInformation
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> Versions
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<Core.Model.MetadataDateTime> BuildDates
            {
                get { throw new NotImplementedException(); }
            }

            public override System.Collections.Generic.IEnumerable<string> AdditionalInformation
            {
                get { throw new NotImplementedException(); }
            }

            public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
            {
                throw new NotImplementedException();
            }
        }

        private class XmlRomInformationHelpersTestStorageAccess : CachedResourceStorageAccess<XmlRomInformationHelpersTestStorageAccess>
        {
        }
    }
}
