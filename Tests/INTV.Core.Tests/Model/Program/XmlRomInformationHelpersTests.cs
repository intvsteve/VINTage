// <copyright file="XmlRomInformationHelpersTests.cs" company="INTV Funhouse">
// Copyright (c) 2018-2019 All Rights Reserved
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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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
        public XmlRomInformationHelpersTests()
        {
            StringUtilities.RegisterHtmlDecoder(HtmlStripTagsAndDecode);
            StringUtilities.RegisterHtmlEncoder(HtmlEncode);
        }

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
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueRom;

            Assert.Equal(expectedProgramIdentifier, info.GetProgramIdentifier());
        }

        [Fact]
        public void XmlRomInformation_GetProgramIdentifierWithValidCrcAndInvalidCrc2AndRomFormat_ReturnsExpectedIdentifier()
        {
            var info = XmlRomInformation.CreateDefault();

            var expectedProgramIdentifier = new ProgramIdentifier(1024u);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc).Value = expectedProgramIdentifier.DataCrc.ToString(CultureInfo.InvariantCulture);
            info.GetColumn(XmlRomInformationDatabaseColumnName.crc_2).Value = "-2";
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueRom;

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
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueBin;

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
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueBin;

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
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueBin;

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
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueBin;

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
            info.GetColumn(XmlRomInformationDatabaseColumnName.format).Value = XmlRomInformationDatabaseColumn.RomFormatValueBin;

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
        [InlineData(XmlRomInformationDatabaseColumn.RomFormatValueRom, RomFormat.Rom)]
        [InlineData(XmlRomInformationDatabaseColumn.RomFormatValueBin, RomFormat.Bin)]
        [InlineData(XmlRomInformationDatabaseColumn.RomFormatValueLuigi, RomFormat.Luigi)]
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
        [InlineData(XmlRomInformationDatabaseColumn.RomTypeValueSystem, GeneralFeatures.SystemRom)] // case sensitive
        [InlineData(XmlRomInformationDatabaseColumn.RomTypeValueRom, GeneralFeatures.None)] // case sensitive
        public void XmlRomInformation_GetProgramFeaturesWithValidTypeColumnValue_ReturnsDefaultFeatures(string validRomType, GeneralFeatures expectedGeneralFeatures)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.type).Value = validRomType;

            Assert.Equal(expectedGeneralFeatures, info.GetProgramFeatures(null).GeneralFeatures);
        }

        [Theory]
        [InlineData(XmlRomInformationDatabaseColumn.RomTypeValueSystem, GeneralFeatures.SystemRom)] // case sensitive
        [InlineData(XmlRomInformationDatabaseColumn.RomTypeValueRom, GeneralFeatures.None)] // case sensitive
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

        #region JLP Features Tests

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

        #region Set General Features Tests

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesWithAllGeneralFeaturesSet_ProducesExpectedColumnValues()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.GeneralFeatures = GeneralFeatures.UnrecognizedRom | GeneralFeatures.PageFlipping | GeneralFeatures.OnboardRam | GeneralFeatures.SystemRom;

            info.SetProgramFeatures(features);

            Assert.Equal(XmlRomInformationDatabaseColumn.RomTypeValueSystem, info.GetColumn(XmlRomInformationDatabaseColumnName.type).Value);
            Assert.Equal("7", info.GetColumn(XmlRomInformationDatabaseColumnName.general_features).Value);
        }

        [Theory]
        [MemberData("SetGeneralFeaturesTestData")]
        public void XmlRomInformation_SetProgramFeaturesWithGeneralFeature_ProducesExpectedColumnValues(GeneralFeatures generalFeatures, string expectedGeneralFeatures, string expectedType)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.GeneralFeatures = generalFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedType, info.GetColumn(XmlRomInformationDatabaseColumnName.type).Value);
            Assert.Equal(expectedGeneralFeatures, info.GetColumn(XmlRomInformationDatabaseColumnName.general_features).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidGeneralFeature_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidGeneralFeature = 1u << 6;
            Assert.Equal(0u, testInvalidGeneralFeature & GeneralFeaturesHelpers.FeaturesMask);
            features.GeneralFeatures = (GeneralFeatures)testInvalidGeneralFeature;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.general_features).Value);
        }

        #endregion // Set General Features Tests

        #region Set NTSC Compatibility Tests

        [Theory]
        [MemberData("SetNtscPalCompatibilityTestData")]
        public void SetProgramFeaturesWithNtscCompatibility_ProducesExpectedColumnValue(FeatureCompatibility ntscCompatibility, string expectedNtscColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Ntsc = ntscCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedNtscColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.ntsc).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidNtscCompatibility_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCompatibility = 1u << 6;
            Assert.Equal(0u, testInvalidCompatibility & FeatureCompatibilityHelpers.CompatibilityMask);
            features.Ntsc = (FeatureCompatibility)testInvalidCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.ntsc).Value);
        }

        #endregion // Set NTSC Compatibility Tests

        #region Set PAL Compatibility Tests

        [Theory]
        [MemberData("SetNtscPalCompatibilityTestData")]
        public void SetProgramFeaturesWithPalCompatibility_ProducesExpectedColumnValue(FeatureCompatibility palCompatibility, string expectedPalColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Pal = palCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedPalColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.pal).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidPalCompatibility_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCompatibility = 1u << 6;
            Assert.Equal(0u, testInvalidCompatibility & FeatureCompatibilityHelpers.CompatibilityMask);
            features.Pal = (FeatureCompatibility)testInvalidCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.pal).Value);
        }

        #endregion // Set PAL Compatibility Tests

        #region Set Keyboard Component Features Tests

        [Theory]
        [MemberData("SetKeyboardComponentFeaturesTestData")]
        public void SetProgramFeaturesWithKeyboardComponentFeatures_ProducesExpectedColumnValue(KeyboardComponentFeatures keybardComponentFeatures, string expectedKeyboardComponentFeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.KeyboardComponent = keybardComponentFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedKeyboardComponentFeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.kc).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidKeyboardComponentFeatures_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidKeyboardComponentFeatures = 1u << 12;
            Assert.Equal(0u, testInvalidKeyboardComponentFeatures & KeyboardComponentFeaturesHelpers.FeaturesMask);
            features.KeyboardComponent = (KeyboardComponentFeatures)testInvalidKeyboardComponentFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.kc).Value);
        }

        #endregion // Set Keyboard Component Features Tests

        #region Set Super Video Arcade Compatibility Tests

        [Theory]
        [MemberData("CommonSetFeatureCompatibilityTestData")]
        public void SetProgramFeaturesWithSearsSuperVideoArcadeCompatibility_ProducesExpectedColumnValue(FeatureCompatibility superVideoArcadeCompatibility, string expectedSuperVideoArcadeCompatibilityColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.SuperVideoArcade = superVideoArcadeCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedSuperVideoArcadeCompatibilityColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.sva).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidSuperVideoArcadeCompatibility_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCompatibility = 1u << 6;
            Assert.Equal(0u, testInvalidCompatibility & FeatureCompatibilityHelpers.CompatibilityMask);
            features.SuperVideoArcade = (FeatureCompatibility)testInvalidCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.sva).Value);
        }

        #endregion // Set Super Video Arcade Compatibility Tests

        #region Set Intellivoice Compatibility Tests

        [Theory]
        [MemberData("CommonSetFeatureCompatibilityTestData")]
        public void SetProgramFeaturesWithIntellivoiceCompatibility_ProducesExpectedColumnValue(FeatureCompatibility intellivoiceCompatibility, string expectedIntellivoiceCompatibilityColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Intellivoice = intellivoiceCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedIntellivoiceCompatibilityColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.ivoice).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidIntellivoiceCompatibility_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCompatibility = 1u << 6;
            Assert.Equal(0u, testInvalidCompatibility & FeatureCompatibilityHelpers.CompatibilityMask);
            features.Intellivoice = (FeatureCompatibility)testInvalidCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.ivoice).Value);
        }

        #endregion // Set Intellivoice Compatibility Tests

        #region Set Intellivision II Compatibility Tests

        [Theory]
        [MemberData("CommonSetFeatureCompatibilityTestData")]
        public void SetProgramFeaturesWithIntellivisionIICompatibility_ProducesExpectedColumnValue(FeatureCompatibility intellivisionIICompatibility, string expectedIntellivisionIICompatibilityColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.IntellivisionII = intellivisionIICompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedIntellivisionIICompatibilityColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.intyii).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidIntellivisionIICompatibility_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCompatibility = 1u << 6;
            Assert.Equal(0u, testInvalidCompatibility & FeatureCompatibilityHelpers.CompatibilityMask);
            features.IntellivisionII = (FeatureCompatibility)testInvalidCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.intyii).Value);
        }

        #endregion // Set Intellivision II Compatibility Tests

        #region Set ECS Features Tests

        [Theory]
        [MemberData("SetEcsFeaturesTestData")]
        public void SetProgramFeaturesWithEcsFeatures_ProducesExpectedColumnValue(EcsFeatures ecsFeatures, string expectedEcsFeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Ecs = ecsFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedEcsFeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.ecs).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidEcsFeatures_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidEcsFeatures = 1u << 12;
            Assert.Equal(0u, testInvalidEcsFeatures & EcsFeaturesHelpers.FeaturesMask);
            features.Ecs = (EcsFeatures)testInvalidEcsFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.ecs).Value);
        }

        #endregion // Set ECS Features Tests

        #region Set Tutorvision Compatibility Tests

        [Theory]
        [MemberData("CommonSetFeatureCompatibilityTestData")]
        public void SetProgramFeaturesWithTutorvisionCompatibility_ProducesExpectedColumnValue(FeatureCompatibility tutorvisionCompatibility, string expectedTutorvisionCompatibilityColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Tutorvision = tutorvisionCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedTutorvisionCompatibilityColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.tutor).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidTutorvisionCompatibility_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCompatibility = 1u << 6;
            Assert.Equal(0u, testInvalidCompatibility & FeatureCompatibilityHelpers.CompatibilityMask);
            features.Tutorvision = (FeatureCompatibility)testInvalidCompatibility;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.tutor).Value);
        }

        #endregion // Set Tutorvision Compatibility Tests

        #region Set Intellicart Features Tests

        [Theory]
        [MemberData("SetIntellicartFeaturesTestData")]
        public void SetProgramFeaturesWithIntellicartFeatures_ProducesExpectedColumnValue(IntellicartCC3Features intellicartFeatures, string expectedIntellicartFeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Intellicart = intellicartFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedIntellicartFeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.icart).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidIntellicartFeatures_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidIntellicartFeatures = 1u << 12;
            Assert.Equal(0u, testInvalidIntellicartFeatures & IntellicartCC3FeaturesHelpers.FeaturesMask);
            features.Intellicart = (IntellicartCC3Features)testInvalidIntellicartFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.icart).Value);
        }

        #endregion // Set Intellicart Features Tests

        #region Set CuttleCart3 Features Tests

        [Theory]
        [MemberData("SetCuttleCart3FeaturesTestData")]
        public void SetProgramFeaturesWithCuttleCart3Features_ProducesExpectedColumnValue(CuttleCart3Features cuttleCart3Features, string expectedCuttleCart3FeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.CuttleCart3 = cuttleCart3Features;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedCuttleCart3FeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.cc3).Value);
        }

        [Fact]
        public void XmlRomInformation_SetProgramFeaturesToInvalidCuttleCart3Features_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidCuttleCart3Features = 1u << 12;
            Assert.Equal(0u, testInvalidCuttleCart3Features & CuttleCart3FeaturesHelpers.FeaturesMask);
            features.CuttleCart3 = (CuttleCart3Features)testInvalidCuttleCart3Features;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.cc3).Value);
        }

        #endregion // Set CuttleCart3 Features Tests

        #region Set JLP Features Tests

        [Theory]
        [MemberData("SetJlpFeaturesTestData")]
        public void SetProgramFeaturesWithJlpFeatures_ProducesExpectedColumnValue(JlpFeatures jlpFeatures, JlpHardwareVersion jlpHardwareVersion, string expectedJlpFeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Jlp = jlpFeatures;
            features.JlpHardwareVersion = jlpHardwareVersion;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedJlpFeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.jlp).Value);
        }

        [Fact]
        public void SetProgramFeaturesWithJlpFeaturesWithHighFlashSectorCount_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Jlp = JlpFeatures.FsdBit9 | JlpFeatures.FsdBit8;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.jlp).Value);
        }

        [Theory]
        [MemberData("SetJlpFlashSectorUsageCountTestData")]
        public void SetProgramFeaturesWithJlpFlashSectorsFeatures_ProducesExpectedColumnValue(ushort jlpFlashSectors, string expectedJlpFlashSectorsColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.JlpFlashMinimumSaveSectors = jlpFlashSectors;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedJlpFlashSectorsColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.jlp_savegame).Value);
        }

        #endregion // Set JLP Features Tests

        #region Set LTO Flash! Features Tests

        [Theory]
        [MemberData("SetLtoFlashFeaturesTestData")]
        public void SetProgramFeaturesWithLtoFlashFeatures_ProducesExpectedColumnValue(LtoFlashFeatures ltoFlashFeatures, string expectedLtoFlashFeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.LtoFlash = ltoFlashFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedLtoFlashFeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.lto_flash).Value);
        }

        [Fact]
        public void SetProgramFeaturesWithLtoFlashFeaturesWithHighFlashSectorCount_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.LtoFlash = LtoFlashFeatures.FsdBit9 | LtoFlashFeatures.FsdBit8;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.lto_flash).Value);
        }

        #endregion // Set LTO Flash! Features Tests

        #region Set Bee3 Features Tests

        [Theory]
        [MemberData("SetBee3FeaturesTestData")]
        public void SetProgramFeaturesWithBee3Features_ProducesExpectedColumnValue(Bee3Features bee3Features, string expectedBee3FeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Bee3 = bee3Features;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedBee3FeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.bee3).Value);
        }

        [Fact]
        public void SetProgramFeaturesWithInvalidBee3Features_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidBee3Features = 1u << 12;
            Assert.Equal(0u, testInvalidBee3Features & Bee3FeaturesHelpers.FeaturesMask);
            features.Bee3 = (Bee3Features)testInvalidBee3Features;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.bee3).Value);
        }

        #endregion // Set Bee3 Features Tests

        #region Set Hive Features Tests

        [Theory]
        [MemberData("SetHiveFeaturesTestData")]
        public void SetProgramFeaturesWithHiveFeatures_ProducesExpectedColumnValue(HiveFeatures hiveFeatures, string expectedHiveFeaturesColumnValue)
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            features.Hive = hiveFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal(expectedHiveFeaturesColumnValue, info.GetColumn(XmlRomInformationDatabaseColumnName.hive).Value);
        }

        [Fact]
        public void SetProgramFeaturesWithInvalidHiveFeatures_ProducesExpectedColumnValue()
        {
            var info = XmlRomInformation.CreateDefault();
            var features = ProgramFeatures.DefaultFeatures.Clone();
            var testInvalidHiveFeatures = 1u << 12;
            Assert.Equal(0u, testInvalidHiveFeatures & HiveFeaturesHelpers.FeaturesMask);
            features.Hive = (HiveFeatures)testInvalidHiveFeatures;

            info.SetProgramFeatures(features);

            Assert.Equal("0", info.GetColumn(XmlRomInformationDatabaseColumnName.hive).Value);
        }

        #endregion // Set Hive Features Tests

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

        [Theory]
        [MemberData("LongNamesTestData")]
        public void XmlRomInformation_GetProgramMetadataWithTitleColumn_ReturnsExpectedTitles(string longNamesValue, IEnumerable<string> expectedLongNames)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.title).Value = longNamesValue;

            Assert.Equal(expectedLongNames, info.GetProgramMetadata(new ProgramMetadata()).LongNames);
        }

        [Theory]
        [MemberData("ShortNamesTestData")]
        public void XmlRomInformation_GetProgramMetadataWithShortNameColumn_ReturnsExpectedShortNames(string shortNamesValue, IEnumerable<string> expectedShortNames)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.short_name).Value = shortNamesValue;

            Assert.Equal(expectedShortNames, info.GetProgramMetadata(new ProgramMetadata()).ShortNames);
        }

        [Theory]
        [MemberData("DescriptionsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithDescriptionColumn_ReturnsExpectedDescriptions(string descriptionsValue, IEnumerable<string> expectedDescriptions)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.description).Value = descriptionsValue;

            Assert.Equal(expectedDescriptions, info.GetProgramMetadata(new ProgramMetadata()).Descriptions);
        }

        [Theory]
        [MemberData("VendorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithVendorColumn_ReturnsExpectedVendors(string vendorsValue, IEnumerable<string> expectedVendors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.vendor).Value = vendorsValue;

            Assert.Equal(expectedVendors, info.GetProgramMetadata(new ProgramMetadata()).Publishers);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithProgramColumn_ReturnsExpectedProgrammers(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.program).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Programmers);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithConceptColumn_ReturnsExpectedDesigners(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.concept).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Designers);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithGraphicsColumn_ReturnsExpectedGraphicsDesigners(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.game_graphics).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Graphics);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithMusicColumn_ReturnsExpectedMusicComposers(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.music).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Music);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithSoundColumn_ReturnsExpectedSoundEffects(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.soundfx).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).SoundEffects);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithVoicesColumn_ReturnsExpectedVoiceActors(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.voices).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Voices);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithDocumentationColumn_ReturnsExpectedDocumentationAuthors(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.game_docs).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Documentation);
        }

        [Theory]
        [MemberData("ContributorsTestData")]
        public void XmlRomInformation_GetProgramMetadataWithBoxArtColumn_ReturnsExpectedOtherArtists(string contributorsValue, IEnumerable<string> expectedContributors)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.box_art).Value = contributorsValue;

            Assert.Equal(expectedContributors, info.GetProgramMetadata(new ProgramMetadata()).Artwork);
        }

        [Theory]
        [MemberData("MetadataDateTestData")]
        public void XmlRomInformation_GetProgramMetadataWithReleaseDateColumn_ReturnsExpectedReleaseDates(string releaseDateValue, IEnumerable<MetadataDateTime> expectedReleaseDates)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.release_date).Value = releaseDateValue;

            Assert.Equal(expectedReleaseDates, info.GetProgramMetadata(new ProgramMetadata()).ReleaseDates);
        }

        [Theory]
        [MemberData("LicensesTestData")]
        public void XmlRomInformation_GetProgramMetadataWithLicenseColumn_ReturnsExpectedLicenses(string licensesValue, IEnumerable<string> expectedLicenses)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.license).Value = licensesValue;

            Assert.Equal(expectedLicenses, info.GetProgramMetadata(new ProgramMetadata()).Licenses);
        }

        [Theory]
        [MemberData("ContactInfoTestData")]
        public void XmlRomInformation_GetProgramMetadataWithContactInfoColumn_ReturnsExpectedContactInformation(string contactInformationValue, IEnumerable<string> expectedContactInformation)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.contact_info).Value = contactInformationValue;

            Assert.Equal(expectedContactInformation, info.GetProgramMetadata(new ProgramMetadata()).ContactInformation);
        }

        [Theory]
        [MemberData("SourcesTestData")]
        public void XmlRomInformation_GetProgramMetadataWithSourceColumn_ReturnsExpectedAdditionalInformation(string sourcesValue, IEnumerable<string> expectedAdditionalInformation)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.source).Value = sourcesValue;

            Assert.Equal(expectedAdditionalInformation, info.GetProgramMetadata(new ProgramMetadata()).AdditionalInformation);
        }

        [Theory]
        [MemberData("RomVariantNamesTestData")]
        public void XmlRomInformation_GetProgramMetadataWithRomVariantNameColumn_ReturnsExpectedRomVersions(string romVariantNamesValue, IEnumerable<string> expectedVersions)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.name).Value = romVariantNamesValue;

            Assert.Equal(expectedVersions, info.GetProgramMetadata(new ProgramMetadata()).Versions);
        }

        [Theory]
        [MemberData("MetadataDateTestData")]
        public void XmlRomInformation_GetProgramMetadataWithBuildDateColumn_ReturnsExpectedBuildDates(string buildDateValue, IEnumerable<MetadataDateTime> expectedBuildDates)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.build_date).Value = buildDateValue;

            Assert.Equal(expectedBuildDates, info.GetProgramMetadata(new ProgramMetadata()).BuildDates);
        }

        [Theory]
        [MemberData("OtherInformationTestData")]
        public void XmlRomInformation_GetProgramMetadataWithOtherColumn_ReturnsExpectedAdditionalInformation(string otherInformationValue, IEnumerable<string> expectedAdditionalInformation)
        {
            var info = XmlRomInformation.CreateDefault();

            info.GetColumn(XmlRomInformationDatabaseColumnName.other).Value = otherInformationValue;

            Assert.Equal(expectedAdditionalInformation, info.GetProgramMetadata(new ProgramMetadata()).AdditionalInformation);
        }

        [Fact]
        public void XmlRomInformation_GetProgramMetadataWithSourceAndOtherColumns_ReturnsExpectedAdditionalInformation()
        {
            var info = new XmlRomInformation();

            info.AddColumn(XmlRomInformationDatabaseColumnName.source, "BSRs|Mattel Electronics|Old Magazines");
            info.AddColumn(XmlRomInformationDatabaseColumnName.other, "|Various Websites|old magazines");

            var expectedAdditionalInformation = new[] { "BSRs", "Mattel Electronics", "Old Magazines", "Various Websites" };
            Assert.Equal(expectedAdditionalInformation, info.GetProgramMetadata(new ProgramMetadata()).AdditionalInformation);
        }

        [Fact]
        public void XmlRomInformation_GetProgramMetadataWithOtherAndSourceColumns_ReturnsExpectedAdditionalInformation()
        {
            var info = new XmlRomInformation();

            info.AddColumn(XmlRomInformationDatabaseColumnName.other, "|Various Websites|old magazines");
            info.AddColumn(XmlRomInformationDatabaseColumnName.source, "BSRs|Mattel Electronics|Old Magazines");

            var expectedAdditionalInformation = new[] { "Various Websites", "old magazines", "BSRs", "Mattel Electronics" };
            Assert.Equal(expectedAdditionalInformation, info.GetProgramMetadata(new ProgramMetadata()).AdditionalInformation);
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

        [Fact]
        public void XmlRomInformation_SetProgramMetadatWithAllColumnsAndNullMetadataDataData_LeavesMetadataUnchanged()
        {
            var info = XmlRomInformation.CreateDefault();
            VerifyEmptyMetadata(info.GetProgramMetadata(null));

            info.SetProgramMetadata(new FakeProgramInformation());

            VerifyEmptyMetadata(info.GetProgramMetadata(null));
        }

        [Fact]
        public void XmlRomInformation_SetProgramMetadatWithAllColumnsAndBadStringMetadataData_LeavesMetadataUnchanged()
        {
            var info = XmlRomInformation.CreateDefault();
            VerifyEmptyMetadata(info.GetProgramMetadata(null));

            info.SetProgramMetadata(new FakeProgramInformation(new string[] { null }));

            VerifyEmptyMetadata(info.GetProgramMetadata(null));
        }

        [Fact]
        public void XmlRomInformation_SetProgramMetadatWithAllColumnsAndDateMetadataData_IncludesDateMetadata()
        {
            var info = XmlRomInformation.CreateDefault();
            VerifyEmptyMetadata(info.GetProgramMetadata(null));

            info.SetProgramMetadata(new FakeProgramInformation(new[] { MetadataDateTime.MinValue }));

            Assert.Equal("0001-01-01", info.GetColumn(XmlRomInformationDatabaseColumnName.release_date).Value);
            Assert.Equal("0001-01-01", info.GetColumn(XmlRomInformationDatabaseColumnName.build_date).Value);
        }

        #region Set LongNames Metadata Tests

        [Theory]
        [MemberData("SetLongNameMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithLongNames_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.title, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        [Theory]
        [MemberData("SetLongNameMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithLongNamesHavingExistingTitle_DoesNotChangeTitle(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            var existingTitle = "Best Game EVAH!";
            info.AddColumn(column, existingTitle);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.title, column);
            Assert.Equal(existingTitle, info.GetColumn(column).Value);
            Assert.NotEqual(expectedColumnValue, existingTitle);
        }

        #endregion // Set LongNames Metadata Tests

        #region Set ShortNames Metadata Tests

        [Theory]
        [MemberData("SetShortNameMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithShortNames_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.short_name, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        [Theory]
        [MemberData("SetShortNameMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithShortNamesHavingExistingShortName_DoesNotChangeShortName(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            var existingShortName = "Bestie!";
            info.AddColumn(column, existingShortName);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.short_name, column);
            Assert.Equal(existingShortName, info.GetColumn(column).Value);
            Assert.NotEqual(expectedColumnValue, existingShortName);
        }

        #endregion // Set ShortNames Metadata Tests

        #region Set Publisher Metadata Tests

        [Theory]
        [MemberData("SetPublisherMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithPublishers_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.vendor, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        [Theory]
        [MemberData("SetPublisherMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithPublishersHavingExistingVendor_DoesNotChangeVendor(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            var existingVendor = "INTV Funhouse";
            info.AddColumn(column, existingVendor);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.vendor, column);
            Assert.Equal(existingVendor, info.GetColumn(column).Value);
            Assert.NotEqual(expectedColumnValue, existingVendor);
        }

        #endregion // Set Publisher Metadata Tests

        #region Set Description Metadata Tests

        [Theory]
        [MemberData("SetDescriptionMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithDescriptions_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.description, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        [Theory]
        [MemberData("SetDescriptionMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithDescriptionHavingExistingDescription_DoesNotChangeDescription(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            var existingDescription = "This is really the best game you are ever going to play.\n\nEver.";
            info.AddColumn(column, existingDescription);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.description, column);
            Assert.Equal(existingDescription, info.GetColumn(column).Value);
            Assert.NotEqual(expectedColumnValue, existingDescription);
        }

        #endregion // Set Description Metadata Tests

        #region Set Release Date Metadata Tests

        [Theory]
        [MemberData("SetReleaseDateMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithReleaseDates_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.release_date, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Release Date Metadata Tests

        #region Set Programmers Metadata Tests

        [Theory]
        [MemberData("SetProgrammersMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithProgrammers_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.program, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Programmers Metadata Tests

        #region Set Designers Metadata Tests

        [Theory]
        [MemberData("SetDesignersMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithDesigners_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.concept, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Designers Metadata Tests

        #region Set Graphics Metadata Tests

        [Theory]
        [MemberData("SetGraphicsMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithGraphics_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.game_graphics, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Graphics Metadata Tests

        #region Set Sound Effects Metadata Tests

        [Theory]
        [MemberData("SetSoundEffectsMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithSoundEffects_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.soundfx, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Sound Effects Metadata Tests

        #region Set Music Metadata Tests

        [Theory]
        [MemberData("SetMusicMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithMusic_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.music, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Music Metadata Tests

        #region Set Voices Metadata Tests

        [Theory]
        [MemberData("SetVoicesMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithVoices_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.voices, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Voices Metadata Tests

        #region Set Documentation Metadata Tests

        [Theory]
        [MemberData("SetDocumentationMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithDocumentation_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.game_docs, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Documentation Metadata Tests

        #region Set Artwork Metadata Tests

        [Theory]
        [MemberData("SetArtworkMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithArtwork_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.box_art, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Artwork Metadata Tests

        #region Set Version Metadata Tests

        [Theory]
        [MemberData("SetVersionsMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithVersions_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.name, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        [Theory]
        [MemberData("SetVersionsMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithVersionsHavingExistingVersion_DoesNotChangeVersion(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            var existingVersion = "Fixed ROM";
            info.AddColumn(column, existingVersion);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.name, column);
            Assert.Equal(existingVersion, info.GetColumn(column).Value);
            Assert.NotEqual(expectedColumnValue, existingVersion);
        }

        #endregion // Set Version Metadata Tests

        #region Set Build Date Metadata Tests

        [Theory]
        [MemberData("SetBuildDateMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithBuildDates_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.build_date, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Build Date Metadata Tests

        #region Set Additional Data Metadata Tests

        [Theory]
        [MemberData("SetAdditionalMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithAdditionalData_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.other, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Additional Data Metadata Tests

        #region Set Licenses Metadata Tests

        [Theory]
        [MemberData("SetLicenseMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithLicenses_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.license, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Licenses Metadata Tests

        #region Set Contact Information Metadata Tests

        [Theory]
        [MemberData("SetContactInformationMetadataTestData")]
        public void XmlRomInformation_SetProgramMetadatWithContactInformation_ProducesExpectedColumnValue(IProgramMetadata metadata, XmlRomInformationDatabaseColumnName column, string expectedColumnValue)
        {
            var info = new XmlRomInformation();
            info.AddColumn(column, null);

            info.SetProgramMetadata(metadata);

            Assert.Equal(XmlRomInformationDatabaseColumnName.contact_info, column);
            Assert.Equal(expectedColumnValue, info.GetColumn(column).Value);
        }

        #endregion // Set Contact Information Metadata Tests

        #endregion // SetProgramMetadata Tests

        private static string HtmlStripTagsAndDecode(string encodedHtmlString)
        {
            var decodedString = string.Empty;
            if (!string.IsNullOrEmpty(encodedHtmlString))
            {
                decodedString = Regex.Replace(encodedHtmlString, "<.*?>", string.Empty);
                decodedString = HttpUtility.HtmlDecode(decodedString);
            }
            return decodedString;
        }

        private static string HtmlEncode(string stringToEncode)
        {
            var decodedString = string.Empty;
            if (!string.IsNullOrEmpty(stringToEncode))
            {
                decodedString = HttpUtility.HtmlEncode(stringToEncode);
            }
            return decodedString;
        }

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
            private IEnumerable<string> _strings;
            private IEnumerable<MetadataDateTime> _dates;

            public FakeProgramInformation()
            {
            }

            public FakeProgramInformation(IEnumerable<string> strings)
                : this(strings, null)
            {
            }

            public FakeProgramInformation(IEnumerable<MetadataDateTime> dates)
                : this(null, dates)
            {
            }

            public FakeProgramInformation(IEnumerable<string> strings, IEnumerable<MetadataDateTime> dates)
            {
                _strings = strings;
                _dates = dates;
            }

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

            public override string ShortName
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

            public override IEnumerable<CrcData> Crcs
            {
                get { throw new NotImplementedException(); }
            }

            public override IEnumerable<string> LongNames
            {
                get { return _strings; }
            }

            public override IEnumerable<string> ShortNames
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Descriptions
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Publishers
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Programmers
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Designers
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Graphics
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Music
            {
                get { return _strings; }
            }

            public override IEnumerable<string> SoundEffects
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Voices
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Documentation
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Artwork
            {
                get { return _strings; }
            }

            public override IEnumerable<MetadataDateTime> ReleaseDates
            {
                get { return _dates; }
            }

            public override IEnumerable<string> Licenses
            {
                get { return _strings; }
            }

            public override IEnumerable<string> ContactInformation
            {
                get { return _strings; }
            }

            public override IEnumerable<string> Versions
            {
                get { return _strings; }
            }

            public override IEnumerable<MetadataDateTime> BuildDates
            {
                get { return _dates; }
            }

            public override IEnumerable<string> AdditionalInformation
            {
                get { return _strings; }
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
