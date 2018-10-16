// <copyright file="CfgVarMetadataIdTagHelpersTests.cs" company="INTV Funhouse">
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests
{
    public class CfgVarMetadataIdTagHelpersTests
    {
        [Theory]
        [InlineData(CfgVarMetadataIdTag.Invalid, "<--!INVALID!-->")]
        [InlineData(CfgVarMetadataIdTag.Name, "name")]
        [InlineData(CfgVarMetadataIdTag.ShortName, "short_name")]
        [InlineData(CfgVarMetadataIdTag.Author, "author")]
        [InlineData(CfgVarMetadataIdTag.GameArt, "game_art_by")]
        [InlineData(CfgVarMetadataIdTag.Music, "music_by")]
        [InlineData(CfgVarMetadataIdTag.SoundEffects, "sfx_by")]
        [InlineData(CfgVarMetadataIdTag.VoiceActing, "voices_by")]
        [InlineData(CfgVarMetadataIdTag.Documentation, "docs_by")]
        [InlineData(CfgVarMetadataIdTag.BoxOrOtherArtwork, "box_art_by")]
        [InlineData(CfgVarMetadataIdTag.ConceptDesign, "concept_by")]
        [InlineData(CfgVarMetadataIdTag.MoreInfo, "more_info_at")]
        [InlineData(CfgVarMetadataIdTag.Publisher, "publisher")]
        [InlineData(CfgVarMetadataIdTag.License, "license")]
        [InlineData(CfgVarMetadataIdTag.Description, "desc")]
        [InlineData(CfgVarMetadataIdTag.ReleaseDate, "release_date")]
        [InlineData(CfgVarMetadataIdTag.Year, "year")]
        [InlineData(CfgVarMetadataIdTag.BuildDate, "build_date")]
        [InlineData(CfgVarMetadataIdTag.Version, "version")]
        [InlineData(CfgVarMetadataIdTag.EcsCompatibility, "ecs_compat")]
        [InlineData(CfgVarMetadataIdTag.IntellivoiceCompatibility, "voice_compat")]
        [InlineData(CfgVarMetadataIdTag.IntellivisionIICompatibility, "intv2_compat")]
        [InlineData(CfgVarMetadataIdTag.KeyboardComponentCompatibility, "kc_compat")]
        [InlineData(CfgVarMetadataIdTag.TutorvisionCompatibility, "tv_compat")]
        [InlineData(CfgVarMetadataIdTag.Ecs,  "ecs")]
        [InlineData(CfgVarMetadataIdTag.Voice, "voice")]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "intv2")]
        [InlineData(CfgVarMetadataIdTag.LtoFlashMapper, "lto_mapper")]
        [InlineData(CfgVarMetadataIdTag.JlpAccelerators, "jlp_accel")]
        [InlineData(CfgVarMetadataIdTag.Jlp, "jlp")]
        [InlineData(CfgVarMetadataIdTag.JlpFlash, "jlpflash")]
        [InlineData((CfgVarMetadataIdTag)127, null)]
        public void CfgVarMetadtaIdTag_ToCfgVarString_ProducesCorrectString(CfgVarMetadataIdTag idTag, string expectedCfgVarString)
        {
            var actualCfgVarString = idTag.ToCfgVarString();

            Assert.Equal(expectedCfgVarString, actualCfgVarString);
        }

        [Theory]
        [InlineData(CfgVarMetadataIdTag.Invalid, "<--!INVALID!-->")]
        [InlineData(CfgVarMetadataIdTag.Name, "name")]
        [InlineData(CfgVarMetadataIdTag.ShortName, "short_name")]
        [InlineData(CfgVarMetadataIdTag.Author, "author")]
        [InlineData(CfgVarMetadataIdTag.GameArt, "game_art_by")]
        [InlineData(CfgVarMetadataIdTag.Music, "music_by")]
        [InlineData(CfgVarMetadataIdTag.SoundEffects, "sfx_by")]
        [InlineData(CfgVarMetadataIdTag.VoiceActing, "voices_by")]
        [InlineData(CfgVarMetadataIdTag.Documentation, "docs_by")]
        [InlineData(CfgVarMetadataIdTag.BoxOrOtherArtwork, "box_art_by")]
        [InlineData(CfgVarMetadataIdTag.ConceptDesign, "concept_by")]
        [InlineData(CfgVarMetadataIdTag.MoreInfo, "more_info_at")]
        [InlineData(CfgVarMetadataIdTag.Publisher, "publisher")]
        [InlineData(CfgVarMetadataIdTag.Publisher, "publishers")]
        [InlineData(CfgVarMetadataIdTag.License, "license")]
        [InlineData(CfgVarMetadataIdTag.Description, "desc")]
        [InlineData(CfgVarMetadataIdTag.Description, "description")]
        [InlineData(CfgVarMetadataIdTag.ReleaseDate, "release_date")]
        [InlineData(CfgVarMetadataIdTag.Year, "year")]
        [InlineData(CfgVarMetadataIdTag.BuildDate, "build_date")]
        [InlineData(CfgVarMetadataIdTag.Version, "version")]
        [InlineData(CfgVarMetadataIdTag.Version, "versions")]
        [InlineData(CfgVarMetadataIdTag.EcsCompatibility, "ecs_compat")]
        [InlineData(CfgVarMetadataIdTag.IntellivoiceCompatibility, "voice_compat")]
        [InlineData(CfgVarMetadataIdTag.IntellivisionIICompatibility, "intv2_compat")]
        [InlineData(CfgVarMetadataIdTag.IntellivisionIICompatibility, "intv2_copmat")]
        [InlineData(CfgVarMetadataIdTag.KeyboardComponentCompatibility, "kc_compat")]
        [InlineData(CfgVarMetadataIdTag.TutorvisionCompatibility, "tv_compat")]
        [InlineData(CfgVarMetadataIdTag.Ecs, "ecs")]
        [InlineData(CfgVarMetadataIdTag.Voice, "voice")]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, "intv2")]
        [InlineData(CfgVarMetadataIdTag.LtoFlashMapper, "lto_mapper")]
        [InlineData(CfgVarMetadataIdTag.JlpAccelerators, "jlp_accel")]
        [InlineData(CfgVarMetadataIdTag.Jlp, "jlp")]
        [InlineData(CfgVarMetadataIdTag.JlpFlash, "jlpflash")]
        [InlineData(CfgVarMetadataIdTag.JlpFlash, "jlp_flash")]
        [InlineData(CfgVarMetadataIdTag.Invalid, null)]
        [InlineData(CfgVarMetadataIdTag.Invalid, "some random bogus string")]
        public void CfgVarMetadataIdTag_StringToCfgVarMetadataIdTag_ProducesCorrectCfgVarMetadataIdTag(CfgVarMetadataIdTag expectedIdTag, string stringToConvert)
        {
            var actualIdTag = stringToConvert.ToCfgVarMetadataIdTag();

            Assert.Equal(expectedIdTag, actualIdTag);
        }

        [Theory]
        [InlineData("nAme")]
        [InlineData("sHort_name")]
        [InlineData("aUthor")]
        [InlineData("gaMe_art_by")]
        [InlineData("musIc_by")]
        [InlineData("sfx_By")]
        [InlineData("voiceS_by")]
        [InlineData("docs_bY")]
        [InlineData("box_aRT_by")]
        [InlineData("conCEpt_by")]
        [InlineData("MORe_info_at")]
        [InlineData("PUBlisher")]
        [InlineData("pubLishers")]
        [InlineData("lIcEnse")]
        [InlineData("dEsC")]
        [InlineData("deSCRIPTion")]
        [InlineData("reLease_date")]
        [InlineData("yeaR")]
        [InlineData("buILd_date")]
        [InlineData("VErsion")]
        [InlineData("VERsions")]
        [InlineData("ecs_Compat")]
        [InlineData("voICE_compat")]
        [InlineData("inTV2_COMpat")]
        [InlineData("kc_COMPAT")]
        [InlineData("tV_compat")]
        [InlineData("eCs")]
        [InlineData("voiCe")]
        [InlineData("intV2")]
        [InlineData("LTO_mapper")]
        [InlineData("JLP_accel")]
        [InlineData("JLP")]
        [InlineData("JLPFlash")]
        [InlineData("JLP_Flash")]
        public void CfgVarMetadataIdTag_StringToCfgVarMetadataIdTag_IsCaseSensitive(string misspelledCfgVarMetadataIdTagName)
        {
            var idTag = misspelledCfgVarMetadataIdTagName.ToCfgVarMetadataIdTag();

            Assert.Equal(CfgVarMetadataIdTag.Invalid, idTag);
        }

        [Theory]
        [InlineData(CfgVarMetadataIdTag.Name, RomMetadataIdTag.Title, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.ShortName, RomMetadataIdTag.ShortTitle, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Author, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.Author)]
        [InlineData(CfgVarMetadataIdTag.GameArt, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.GameArt)]
        [InlineData(CfgVarMetadataIdTag.Music, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.Music)]
        [InlineData(CfgVarMetadataIdTag.SoundEffects, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.SoundEffects)]
        [InlineData(CfgVarMetadataIdTag.VoiceActing, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.VoiceActing)]
        [InlineData(CfgVarMetadataIdTag.Documentation, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.Documentation)]
        [InlineData(CfgVarMetadataIdTag.BoxOrOtherArtwork, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.BoxOrOtherArtwork)]
        [InlineData(CfgVarMetadataIdTag.ConceptDesign, RomMetadataIdTag.Credits, FeatureCategory.None, CfgVarMetadataIdTag.ConceptDesign)]
        [InlineData(CfgVarMetadataIdTag.MoreInfo, RomMetadataIdTag.UrlContactInfo, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Publisher, RomMetadataIdTag.Publisher, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.License, RomMetadataIdTag.License, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Description, RomMetadataIdTag.Description, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.ReleaseDate, RomMetadataIdTag.ReleaseDate, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Year, RomMetadataIdTag.ReleaseDate, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.BuildDate, RomMetadataIdTag.BuildDate, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Version, RomMetadataIdTag.Version, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.EcsCompatibility, RomMetadataIdTag.Features, FeatureCategory.Ecs, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.IntellivoiceCompatibility, RomMetadataIdTag.Features, FeatureCategory.Intellivoice, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionIICompatibility, RomMetadataIdTag.Features, FeatureCategory.IntellivisionII, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.KeyboardComponentCompatibility, RomMetadataIdTag.Features, FeatureCategory.KeyboardComponent, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.TutorvisionCompatibility, RomMetadataIdTag.Features, FeatureCategory.Tutorvision, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Ecs, RomMetadataIdTag.Features, FeatureCategory.EcsLegacy, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Voice, RomMetadataIdTag.Features, FeatureCategory.IntellivoiceLegacy, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, RomMetadataIdTag.Features, FeatureCategory.IntellivisionIILegacy, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.LtoFlashMapper, RomMetadataIdTag.Features, FeatureCategory.LtoFlash, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.JlpAccelerators, RomMetadataIdTag.Features, FeatureCategory.Jlp, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Jlp, RomMetadataIdTag.Features, FeatureCategory.Jlp, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.JlpFlash, RomMetadataIdTag.Features, FeatureCategory.JlpFlashCapacity, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Invalid, RomMetadataIdTag.Ignore, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        public void CfgVarMetadataIdTag_ConvertToRomMetadataIdTag_ProducesExpectedRomMetadataIdTag(CfgVarMetadataIdTag idTag, RomMetadataIdTag expectedRomMetadataIdTag, FeatureCategory expectedFeatureCategory, CfgVarMetadataIdTag expectedUnsupportedMetadataValue)
        {
            CfgVarMetadataIdTag actualUnsupportedMetadataValue;
            FeatureCategory actualFeatureCategory;
            var actualRomMetadataIdTag = idTag.ToRomMetadataIdTag(out actualFeatureCategory, out actualUnsupportedMetadataValue);

            Assert.Equal(expectedRomMetadataIdTag, actualRomMetadataIdTag);
        }

        [Fact]
        public void CfgVarMetadataIdTag_ConvertBogusCfgVarMetadataIdTagToRomMetadataIdTag_ThrowsInvalidOperationException()
        {
            CfgVarMetadataIdTag actualUnsupportedMetadataValue;
            FeatureCategory actualFeatureCategory;
            var bogusIdTag = (CfgVarMetadataIdTag)234;

            Assert.Throws<InvalidOperationException>(() => bogusIdTag.ToRomMetadataIdTag(out actualFeatureCategory, out actualUnsupportedMetadataValue));
        }

        [Theory]
        [InlineData(CfgVarMetadataIdTag.Name, LuigiMetadataIdTag.Name, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.ShortName, LuigiMetadataIdTag.ShortName, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Author, LuigiMetadataIdTag.Author, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.GameArt, LuigiMetadataIdTag.Graphics, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Music, LuigiMetadataIdTag.Music, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.SoundEffects, LuigiMetadataIdTag.SoundEffects, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.VoiceActing, LuigiMetadataIdTag.VoiceActing, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Documentation, LuigiMetadataIdTag.Documentation, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.BoxOrOtherArtwork, LuigiMetadataIdTag.BoxOrOtherArtwork, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.ConceptDesign, LuigiMetadataIdTag.ConceptDesign, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.MoreInfo, LuigiMetadataIdTag.UrlContactInfo, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Publisher, LuigiMetadataIdTag.Publisher, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.License, LuigiMetadataIdTag.License, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Description, LuigiMetadataIdTag.Description, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.ReleaseDate,  LuigiMetadataIdTag.Date, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Year,  LuigiMetadataIdTag.Date, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.BuildDate, LuigiMetadataIdTag.Miscellaneous, FeatureCategory.None, CfgVarMetadataIdTag.BuildDate)]
        [InlineData(CfgVarMetadataIdTag.Version, LuigiMetadataIdTag.Miscellaneous, FeatureCategory.None, CfgVarMetadataIdTag.Version)]
        [InlineData(CfgVarMetadataIdTag.EcsCompatibility, LuigiMetadataIdTag.Unknown, FeatureCategory.Ecs, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.IntellivoiceCompatibility, LuigiMetadataIdTag.Unknown, FeatureCategory.Intellivoice, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionIICompatibility, LuigiMetadataIdTag.Unknown, FeatureCategory.IntellivisionII, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.KeyboardComponentCompatibility, LuigiMetadataIdTag.Unknown, FeatureCategory.KeyboardComponent, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.TutorvisionCompatibility, LuigiMetadataIdTag.Unknown, FeatureCategory.Tutorvision, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Ecs, LuigiMetadataIdTag.Unknown, FeatureCategory.EcsLegacy, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Voice, LuigiMetadataIdTag.Miscellaneous, FeatureCategory.IntellivoiceLegacy, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.IntellivisionII, LuigiMetadataIdTag.Miscellaneous, FeatureCategory.IntellivisionIILegacy, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.LtoFlashMapper, LuigiMetadataIdTag.Unknown, FeatureCategory.LtoFlash, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.JlpAccelerators, LuigiMetadataIdTag.Unknown, FeatureCategory.Jlp, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Jlp, LuigiMetadataIdTag.Unknown, FeatureCategory.Jlp, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.JlpFlash, LuigiMetadataIdTag.Unknown, FeatureCategory.JlpFlashCapacity, CfgVarMetadataIdTag.Invalid)]
        [InlineData(CfgVarMetadataIdTag.Invalid, LuigiMetadataIdTag.Unknown, FeatureCategory.None, CfgVarMetadataIdTag.Invalid)]
        public void CfgVarMetadataIdTag_ConvertToLuigiMetadataIdTag_ProducesExpectedLuigiMetadataIdTag(CfgVarMetadataIdTag idTag, LuigiMetadataIdTag expectedLuigiMetadataIdTag, FeatureCategory expectedFeatureCategory, CfgVarMetadataIdTag expectedUnsupportedMetadataValue)
        {
            CfgVarMetadataIdTag actualUnsupportedMetadataValue;
            FeatureCategory actualFeatureCategory;
            var actualRomMetadataIdTag = idTag.ToLuigiMetadataIdTag(out actualFeatureCategory, out actualUnsupportedMetadataValue);

            Assert.Equal(expectedLuigiMetadataIdTag, actualRomMetadataIdTag);
        }

        [Fact]
        public void CfgVarMetadataIdTag_ConvertBogusCfgVarMetadataIdTagToLuigiMetadataIdTag_ThrowsInvalidOperationException()
        {
            CfgVarMetadataIdTag actualUnsupportedMetadataValue;
            FeatureCategory actualFeatureCategory;
            var bogusIdTag = (CfgVarMetadataIdTag)124;

            Assert.Throws<InvalidOperationException>(() => bogusIdTag.ToLuigiMetadataIdTag(out actualFeatureCategory, out actualUnsupportedMetadataValue));
        }
    }
}
