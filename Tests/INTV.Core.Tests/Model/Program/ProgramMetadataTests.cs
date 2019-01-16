// <copyright file="ProgramMetadataTests.cs" company="INTV Funhouse">
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
    public class ProgramMetadataTests
    {
        [Fact]
        public void ProgramMetadata_GetValues_VerifyEmpty()
        {
            var metadata = new ProgramMetadata();

            Assert.False(metadata.LongNames.Any());
            Assert.False(metadata.ShortNames.Any());
            Assert.False(metadata.Descriptions.Any());
            Assert.False(metadata.Publishers.Any());
            Assert.False(metadata.Programmers.Any());
            Assert.False(metadata.Designers.Any());
            Assert.False(metadata.Graphics.Any());
            Assert.False(metadata.Music.Any());
            Assert.False(metadata.SoundEffects.Any());
            Assert.False(metadata.Voices.Any());
            Assert.False(metadata.Documentation.Any());
            Assert.False(metadata.Artwork.Any());
            Assert.False(metadata.ReleaseDates.Any());
            Assert.False(metadata.Licenses.Any());
            Assert.False(metadata.ContactInformation.Any());
            Assert.False(metadata.Versions.Any());
            Assert.False(metadata.BuildDates.Any());
            Assert.False(metadata.AdditionalInformation.Any());
        }

        public static IEnumerable<object[]> ReplaceStringMetadataTestData
        {
            get
            {
                var idsToIgnore = new[] { IProgramMetadataFieldId.None, IProgramMetadataFieldId.ReleaseDates, IProgramMetadataFieldId.BuildDates };
                foreach (var field in AllStringFieldsTestData.Select(d => d[0]))
                {
                    var stringData = new[] { field.ToString(), "Squee!" };
                    yield return new object[] { field, stringData };
                }
            }
        }

        [Theory]
        [MemberData("ReplaceStringMetadataTestData")]
        public void ProgramMetadata_ReplaceStringMetadata_ReplacesMetadata(IProgramMetadataFieldId field, IEnumerable<string> replacementData)
        {
            var metadata = new ProgramMetadata();

            metadata.ReplaceValue(field, replacementData);

            Assert.Equal(replacementData, GetStringMetadata(metadata, field));
        }

        [Fact]
        public void ProgramMetadata_ReplaceReleaseDatesMetadata_ReplacesMetadata()
        {
            var metadata = new ProgramMetadata();

            var dates = new[]
            {
                new MetadataDateTimeBuilder(1999).WithMonth(4).Build(),
                new MetadataDateTimeBuilder(1989).WithMonth(8).Build()
            };
            metadata.ReplaceReleaseDates(dates);

            Assert.Equal(dates, metadata.ReleaseDates);
        }

        [Fact]
        public void ProgramMetadata_ReplaceBuildDatesMetadata_ReplacesMetadata()
        {
            var metadata = new ProgramMetadata();

            var dates = new[]
            {
                new MetadataDateTimeBuilder(2001).WithMonth(2).Build(),
                new MetadataDateTimeBuilder(1984).WithMonth(6).Build()
            };
            metadata.ReplaceBuildDates(dates);

            Assert.Equal(dates, metadata.BuildDates);
        }

        public static IEnumerable<object[]> AllStringFieldsTestData
        {
            get
            {
                var idsToIgnore = new[] { IProgramMetadataFieldId.None, IProgramMetadataFieldId.ReleaseDates, IProgramMetadataFieldId.BuildDates };
                return Enum.GetValues(typeof(IProgramMetadataFieldId)).Cast<IProgramMetadataFieldId>().Except(idsToIgnore).Select(f => new object[] { f });
            }
        }

        [Theory]
        [MemberData("AllStringFieldsTestData")]
        public void ProgramMetadata_ReplaceStringMetadataWithNull_ResultIsNotNull(IProgramMetadataFieldId field)
        {
            var metadata = new ProgramMetadata();

            metadata.ReplaceValue(field, null);

            Assert.NotNull(GetStringMetadata(metadata, field));
        }

        [Fact]
        public void ProgramMetadata_ReplaceReleaseDatesMetadataWithNull_ResultIsNotNull()
        {
            var metadata = new ProgramMetadata();

            metadata.ReplaceReleaseDates(null);

            Assert.NotNull(metadata.ReleaseDates);
        }

        [Fact]
        public void ProgramMetadata_ReplaceBuildDatesMetadataWithNull_ResultIsNotNull()
        {
            var metadata = new ProgramMetadata();

            metadata.ReplaceBuildDates(null);

            Assert.NotNull(metadata.BuildDates);
        }

        [Theory]
        [InlineData(IProgramMetadataFieldId.None)]
        [InlineData((IProgramMetadataFieldId)123)]
        public void ProgramMetadata_ReplaceBogusStringMetadata_ThrowsInvalidOperationException(IProgramMetadataFieldId field)
        {
            var metadata = new ProgramMetadata();

            Assert.Throws<InvalidOperationException>(() => metadata.ReplaceValue(field, new[] { "Nanu nanu" }));
        }

        private IEnumerable<string> GetStringMetadata(ProgramMetadata metadata, IProgramMetadataFieldId field)
        {
            IEnumerable<string> stringData = null;
            switch (field)
            {
                case IProgramMetadataFieldId.LongNames:
                    stringData = metadata.LongNames;
                    break;
                case IProgramMetadataFieldId.ShortNames:
                    stringData = metadata.ShortNames;
                    break;
                case IProgramMetadataFieldId.Descriptions:
                    stringData = metadata.Descriptions;
                    break;
                case IProgramMetadataFieldId.Publishers:
                    stringData = metadata.Publishers;
                    break;
                case IProgramMetadataFieldId.Programmers:
                    stringData = metadata.Programmers;
                    break;
                case IProgramMetadataFieldId.Designers:
                    stringData = metadata.Designers;
                    break;
                case IProgramMetadataFieldId.Graphics:
                    stringData = metadata.Graphics;
                    break;
                case IProgramMetadataFieldId.Music:
                    stringData = metadata.Music;
                    break;
                case IProgramMetadataFieldId.SoundEffects:
                    stringData = metadata.SoundEffects;
                    break;
                case IProgramMetadataFieldId.Voices:
                    stringData = metadata.Voices;
                    break;
                case IProgramMetadataFieldId.Documentation:
                    stringData = metadata.Documentation;
                    break;
                case IProgramMetadataFieldId.Artwork:
                    stringData = metadata.Artwork;
                    break;
                case IProgramMetadataFieldId.Licenses:
                    stringData = metadata.Licenses;
                    break;
                case IProgramMetadataFieldId.ContactInformation:
                    stringData = metadata.ContactInformation;
                    break;
                case IProgramMetadataFieldId.Versions:
                    stringData = metadata.Versions;
                    break;
                case IProgramMetadataFieldId.AdditionalInformation:
                    stringData = metadata.AdditionalInformation;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return stringData;
        }
    }
}
