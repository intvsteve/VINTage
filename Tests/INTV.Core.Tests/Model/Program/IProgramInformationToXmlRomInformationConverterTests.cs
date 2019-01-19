// <copyright file="IProgramInformationToXmlRomInformationConverterTests.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model.Program
{
    public class IProgramInformationToXmlRomInformationConverterTests
    {
        [Fact]
        public void IProgramInformationToXmlRomInformationConverter_ConvertNullInformation_ThrowsNullReferenceException()
        {
            IProgramInformation information = null;

            Assert.Throws<NullReferenceException>(() => IProgramInformationToXmlRomInformationConverter.Instance.Convert(information).Any());
        }

        [Fact]
        public void IProgramInformationToXmlRomInformationConverter_ConvertInformationWithNoCrcData_ProducesNoResults()
        {
            var information = new TestProgramInformation() { Title = "Nothing to see here" };

            var xmlRomInformation = IProgramInformationToXmlRomInformationConverter.Instance.Convert(information);

            Assert.False(xmlRomInformation.Any());
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public void IProgramInformationToXmlRomInformationConverter_ConvertInformationWithoutCrcs_ProducesNoResultsRegardlessOfSettings(bool convertAll, bool setFeatures, bool setMetadata)
        {
            var information = new TestProgramInformation() { Title = "What, me worry?" };

            var xmlRomInformation = IProgramInformationToXmlRomInformationConverter.Instance.Convert(information, convertAll, setFeatures, setMetadata);

            Assert.False(xmlRomInformation.Any());
        }

        public static IEnumerable<object[]> ConvertWithInformationOriginTestData
        {
            get
            {
                var xmlInformationStrings = new Dictionary<ProgramInformationOrigin, string>()
                {
                    { ProgramInformationOrigin.None, XmlRomInformationDatabaseColumn.OriginOther },
                    { ProgramInformationOrigin.Embedded, XmlRomInformationDatabaseColumn.OriginIntvFunhouse },
                    { ProgramInformationOrigin.UpdateFragment, XmlRomInformationDatabaseColumn.OriginOther },
                    { ProgramInformationOrigin.UserDefined, XmlRomInformationDatabaseColumn.OriginUserDefined },
                    { ProgramInformationOrigin.UserEmail, XmlRomInformationDatabaseColumn.OriginUserEmail },
                    { ProgramInformationOrigin.IntvFunhouse, XmlRomInformationDatabaseColumn.OriginIntvFunhouse },
                    { ProgramInformationOrigin.JzIntv, XmlRomInformationDatabaseColumn.OriginIntvName },
                    { ProgramInformationOrigin.Lto, XmlRomInformationDatabaseColumn.OriginOther },
                    { ProgramInformationOrigin.Elektronite, XmlRomInformationDatabaseColumn.OriginOther },
                    { ProgramInformationOrigin.Intelligentvision, XmlRomInformationDatabaseColumn.OriginOther },
                    { ProgramInformationOrigin.CollectorVision, XmlRomInformationDatabaseColumn.OriginOther },
                    { ProgramInformationOrigin.IntellivisionProductions, XmlRomInformationDatabaseColumn.OriginBlueSkyRangers },
                    { ProgramInformationOrigin.GameCatalog, XmlRomInformationDatabaseColumn.OriginCatalog },
                    { ProgramInformationOrigin.LuigiMetadataBlock, XmlRomInformationDatabaseColumn.OriginLuigiFormatMetadata },
                    { ProgramInformationOrigin.RomMetadataBlock, XmlRomInformationDatabaseColumn.OriginRomFormatMetadata },
                    { ProgramInformationOrigin.CfgVarMetadataBlock, XmlRomInformationDatabaseColumn.OriginCfgFormatMetadata },
                };

                foreach (var informationOriginValue in Enum.GetValues(typeof(ProgramInformationOrigin)).Cast<ProgramInformationOrigin>())
                {
                    yield return new object[] { informationOriginValue, xmlInformationStrings[informationOriginValue] };
                }
            }
        }

        [Theory]
        [MemberData("ConvertWithInformationOriginTestData")]
        public void IProgramInformationToXmlRomInformationConverter_ConvertInformationWithOneCrc_XmlRomInformationHasExpectedOriginString(ProgramInformationOrigin originToTest, string expectedOriginString)
        {
            var information = new TestProgramInformation() { Title = "Origin tester info" };
            information.SetOrigin(originToTest);
            information.AddCrcs(1, setVersionMetadata: false);

            var xmlRomInformation = IProgramInformationToXmlRomInformationConverter.Instance.Convert(information).Single();

            Assert.Equal(expectedOriginString, xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.origin).Value);
        }

        [Theory]
        [InlineData("", "0000-00-00")]
        [InlineData("78", "0000-00-00")]
        [InlineData("a45@", "0000-00-00")]
        [InlineData("1980", "1980-01-01")]
        [InlineData("8888", "8888-01-01")]
        [InlineData("10000", "0000-00-00")]
        public void IProgramInformationToXmlRomInformationConverter_ConvertInformationWithOneCrc_XmlRomInformationHasExpectedReleaseDateString(string programInfoYear, string expectedYearString)
        {
            var information = new TestProgramInformation() { Title = "Year tester info", Year = programInfoYear };
            information.AddCrcs(1, setVersionMetadata: false);

            var xmlRomInformation = IProgramInformationToXmlRomInformationConverter.Instance.Convert(information).Single();

            Assert.Equal(expectedYearString, xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.release_date).Value);
        }

        [Theory]
        [InlineData(GeneralFeatures.None, XmlRomInformationDatabaseColumn.RomTypeValueRom)]
        [InlineData(GeneralFeatures.SystemRom, XmlRomInformationDatabaseColumn.RomTypeValueSystem)]
        [InlineData(GeneralFeatures.PageFlipping, XmlRomInformationDatabaseColumn.RomTypeValueRom)]
        [InlineData(GeneralFeatures.PageFlipping | GeneralFeatures.SystemRom, XmlRomInformationDatabaseColumn.RomTypeValueSystem)]
        public void IProgramInformationToXmlRomInformationConverter_ConvertInformationWithOneCrc_XmlRomInformationHasExpectedTypeString(GeneralFeatures generalFeatures, string expectedRomTypeString)
        {
            var features = new ProgramFeaturesBuilder().WithGeneralFeatures(generalFeatures).Build();
            var information = new TestProgramInformation() { Title = "Rom Type tester info", Features = (ProgramFeatures)features };
            information.AddCrcs(1, setVersionMetadata: false);

            var xmlRomInformation = IProgramInformationToXmlRomInformationConverter.Instance.Convert(information, convertAll: false, setFeatures: false, setMetadata: false).Single();

            Assert.Equal(expectedRomTypeString, xmlRomInformation.GetColumn(XmlRomInformationDatabaseColumnName.type).Value);
        }

        private class TestProgramInformation : ProgramInformation
        {
            /// <inheritdoc />
            public override ProgramInformationOrigin DataOrigin
            {
                get { return _origin; }
            }
            private ProgramInformationOrigin _origin = ProgramInformationOrigin.None;

            /// <inheritdoc />
            public override string Title { get; set; }

            /// <inheritdoc />
            public override string Year { get; set; }

            /// <inheritdoc />
            public override ProgramFeatures Features { get; set; }

            /// <inheritdoc />
            public override string ShortName { get; set; }

            /// <inheritdoc />
            public override IEnumerable<CrcData> Crcs
            {
                get { return _crcs.Values; }
            }
            private Dictionary<uint, CrcData> _crcs = new Dictionary<uint, CrcData>();

            /// <inheritdoc />
            public override IEnumerable<string> LongNames
            {
                get { return _longNames; }
            }
            private IEnumerable<string> _longNames = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> ShortNames
            {
                get { return _shortNames; }
            }
            private IEnumerable<string> _shortNames = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Descriptions
            {
                get { return _descriptions; }
            }
            private IEnumerable<string> _descriptions = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Publishers
            {
                get { return _publishers; }
            }
            private IEnumerable<string> _publishers = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Programmers
            {
                get { return _programmers; }
            }
            private IEnumerable<string> _programmers = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Designers
            {
                get { return _designers; }
            }
            private IEnumerable<string> _designers = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Graphics
            {
                get { return _graphics; }
            }
            private IEnumerable<string> _graphics = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Music
            {
                get { return _music; }
            }
            private IEnumerable<string> _music = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> SoundEffects
            {
                get { return _soundEffects; }
            }
            private IEnumerable<string> _soundEffects = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Voices
            {
                get { return _voices; }
            }
            private IEnumerable<string> _voices = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Documentation
            {
                get { return _documentation; }
            }
            private IEnumerable<string> _documentation = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Artwork
            {
                get { return _artwork; }
            }
            private IEnumerable<string> _artwork = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<MetadataDateTime> ReleaseDates
            {
                get { return _releaseDates; }
            }
            private IEnumerable<MetadataDateTime> _releaseDates = Enumerable.Empty<MetadataDateTime>();

            /// <inheritdoc />
            public override IEnumerable<string> Licenses
            {
                get { return _licenses; }
            }
            private IEnumerable<string> _licenses = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> ContactInformation
            {
                get { return _contactInformation; }
            }
            private IEnumerable<string> _contactInformation = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<string> Versions
            {
                get { return _versions; }
            }
            private IEnumerable<string> _versions = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override IEnumerable<MetadataDateTime> BuildDates
            {
                get { return _buildDates; }
            }
            private IEnumerable<MetadataDateTime> _buildDates = Enumerable.Empty<MetadataDateTime>();

            /// <inheritdoc />
            public override IEnumerable<string> AdditionalInformation
            {
                get { return _additionalInformation; }
            }
            private IEnumerable<string> _additionalInformation = Enumerable.Empty<string>();

            /// <inheritdoc />
            public override bool AddCrc(uint newCrc, string crcDescription, IncompatibilityFlags incompatibilities)
            {
                var added = !_crcs.ContainsKey(newCrc);
                if (added)
                {
                    _crcs[newCrc] = new CrcData(newCrc, crcDescription, incompatibilities);
                }
                return added;
            }

            public void SetOrigin(ProgramInformationOrigin origin)
            {
                _origin = origin;
            }

            public void AddCrcs(int numberOfCrcsToAdd, bool setVersionMetadata)
            {
                var crc = 0x1000u;
                _crcs.Clear();
                for (var i = 0; i < numberOfCrcsToAdd; ++i)
                {
                    var crcName = "Version " + i;
                    AddCrc(crc + (uint)i, crcName, IncompatibilityFlags.None);
                }
                if (setVersionMetadata)
                {
                    _versions = _crcs.Select(c => c.Value.Description).ToList();
                }
            }

            public void PopulateMetadata(int numEntries)
            {
                var entryIndices = Enumerable.Range(1, numEntries);

                _longNames = entryIndices.Select(i => "This is the ROM " + i);
                _shortNames = entryIndices.Select(i => "ROM " + i);
                _descriptions = entryIndices.Select(i => "Coolest ROM ever #" + i);
                _publishers = entryIndices.Select(i => "Published by Vendo-" + i);
                _programmers = entryIndices.Select(i => "Programmer " + i);
                _designers = entryIndices.Select(i => "Designer " + i);
                _graphics = entryIndices.Select(i => "GfxDude " + i);
                _music = entryIndices.Select(i => "Musician " + i);
                _soundEffects = entryIndices.Select(i => "Soundette " + i);
                _voices = entryIndices.Select(i => "Vocals " + i);
                _documentation = entryIndices.Select(i => "Writer " + i);
                _artwork = entryIndices.Select(i => "Artiste " + i);
                _licenses = entryIndices.Select(i => "License " + i);
                _contactInformation = entryIndices.Select(i => "Contact " + i);
                _versions = entryIndices.Select(i => "Ver " + i);
                _additionalInformation = entryIndices.Select(i => "Moar INFO " + i);

                _releaseDates = entryIndices.Select(i => new MetadataDateTimeBuilder(2018).WithMonth(12).WithDay(i).Build());
                _buildDates = entryIndices.Select(i => new MetadataDateTimeBuilder(2018).WithMonth(11).WithDay(i).Build());
            }
        }
    }
}
