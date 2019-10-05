// <copyright file="EnabledCompressedArchiveFormatsTests.cs" company="INTV Funhouse">
// Copyright (c) 2019 All Rights Reserved
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
using INTV.Shared.CompressedArchiveAccess;
using INTV.Shared.Model;
using Xunit;

namespace INTV.Shared.Tests.Model
{
    public class EnabledCompressedArchiveFormatsTests
    {
        [Fact]
        public void EnabledCompressedArchiveFormatsNone_ToCompressedArchiveFormats_ReturnsEmptyFormats()
        {
            Assert.Empty(EnabledCompressedArchiveFormats.None.ToCompressedArchiveFormats());
        }

        [Fact]
        public void EnabledCompressedArchiveFormatsAll_ToCompressedArchiveFormatsOnlySupported_ReturnsOnlySupportedFormats()
        {
            Assert.Equal(GetSupportedStandardFormats(), EnabledCompressedArchiveFormats.All.ToCompressedArchiveFormats());
        }

        [Fact]
        public void EnabledCompressedArchiveFormatsAll_ToCompressedArchiveFormats_ReturnsAllFormats()
        {
            Assert.Equal(GetAllStandardFormats(), EnabledCompressedArchiveFormats.All.ToCompressedArchiveFormats(onlyIncludeAvailableFormats: false));
        }

        [Fact]
        public void EnabledCompressedArchiveFormats_FromNullCompressedArchiveFormats_ThrowsArgumentNullException()
        {
            IEnumerable<CompressedArchiveFormat> nullFormats = null;

            Assert.Throws<ArgumentNullException>(() => nullFormats.FromCompressedArchiveFormats());
        }

        [Fact]
        public void EnabledCompressedArchiveFormats_FromEmptyCompressedArchiveFormats_ProdcuesEnabledCompressedArchiveFormatsNone()
        {
            Assert.Equal(EnabledCompressedArchiveFormats.None, Enumerable.Empty<CompressedArchiveFormat>().FromCompressedArchiveFormats());
        }

        [Theory]
        [InlineData(CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.None)]
        [InlineData(CompressedArchiveFormat.Zip, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.Zip)]
        [InlineData(CompressedArchiveFormat.GZip, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.GZip)]
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveFormat.GZip, CompressedArchiveFormat.None, (CompressedArchiveFormat)(16), EnabledCompressedArchiveFormats.GZip | EnabledCompressedArchiveFormats.Tar)]
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.Tar)]
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveFormat.Zip, CompressedArchiveFormat.None, (CompressedArchiveFormat)(78), EnabledCompressedArchiveFormats.Zip | EnabledCompressedArchiveFormats.Tar)]
        [InlineData((CompressedArchiveFormat)17, (CompressedArchiveFormat)7931, (CompressedArchiveFormat)319, (CompressedArchiveFormat)(972), EnabledCompressedArchiveFormats.None)]
        [InlineData(CompressedArchiveFormat.Zip, CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar, (CompressedArchiveFormat)(42), EnabledCompressedArchiveFormats.Zip | EnabledCompressedArchiveFormats.GZip | EnabledCompressedArchiveFormats.Tar)]
        public void EnabledCompressedArchiveFormats_FromCompressedArchiveFormats_ProducesExpectedSupportedFormats(
            CompressedArchiveFormat first,
            CompressedArchiveFormat second,
            CompressedArchiveFormat third,
            CompressedArchiveFormat fourth,
            EnabledCompressedArchiveFormats expectedFormats)
        {
            var compressedArchiveFormats = new[] { first, second, third, fourth };

            Assert.Equal(expectedFormats, compressedArchiveFormats.FromCompressedArchiveFormats());
        }

        [Theory]
        [InlineData(CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.None)]
        [InlineData(CompressedArchiveFormat.Zip, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.Zip)]
        [InlineData(CompressedArchiveFormat.GZip, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.GZip)]
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveFormat.GZip, CompressedArchiveFormat.BZip2, (CompressedArchiveFormat)(16), EnabledCompressedArchiveFormats.GZip | EnabledCompressedArchiveFormats.BZip2 | EnabledCompressedArchiveFormats.Tar | EnabledCompressedArchiveFormats.Unknown)]
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveFormat.None, CompressedArchiveFormat.None, CompressedArchiveFormat.None, EnabledCompressedArchiveFormats.Tar)]
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveFormat.Zip, CompressedArchiveFormat.None, (CompressedArchiveFormat)(78), EnabledCompressedArchiveFormats.Zip | EnabledCompressedArchiveFormats.Tar | EnabledCompressedArchiveFormats.Unknown)]
        [InlineData((CompressedArchiveFormat)17, (CompressedArchiveFormat)7931, (CompressedArchiveFormat)319, (CompressedArchiveFormat)(972), EnabledCompressedArchiveFormats.Unknown)]
        [InlineData(CompressedArchiveFormat.Zip, CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar, (CompressedArchiveFormat)(42), EnabledCompressedArchiveFormats.Zip | EnabledCompressedArchiveFormats.GZip | EnabledCompressedArchiveFormats.Tar | EnabledCompressedArchiveFormats.Unknown)]
        public void EnabledCompressedArchiveFormats_FromCompressedArchiveFormatsAllowUnsupporte_ProducesExpectedSupportedFormats(
            CompressedArchiveFormat first,
            CompressedArchiveFormat second,
            CompressedArchiveFormat third,
            CompressedArchiveFormat fourth,
            EnabledCompressedArchiveFormats expectedFormats)
        {
            var compressedArchiveFormats = new[] { first, second, third, fourth };

            Assert.Equal(expectedFormats, compressedArchiveFormats.FromCompressedArchiveFormats(onlyIncludeAvailableFormats: false));
        }

        [Fact]
        public void EnabledCompressedArchiveFormats_UpdateAvailableCompressedArchiveFormats_UpdatesAvailablityCorrectly()
        {
            var supportedFormats = GetSupportedStandardFormats();
            try
            {
                var supportedFormatsStates = supportedFormats.ToDictionary(f => f, f => f.IsCompressedArchiveFormatEnabled());
                Assert.All(supportedFormatsStates, f => Assert.True(f.Key.IsCompressedArchiveFormatSupportedAndEnabled()));
                var currentEnabledCompressedArchiveFormats = supportedFormats.FromCompressedArchiveFormats();
                var updatedEnabledCompressedArchiveFormats = EnabledCompressedArchiveFormats.Zip | EnabledCompressedArchiveFormats.Tar;

                updatedEnabledCompressedArchiveFormats.UpdateAvailableCompressedArchiveFormats();

                var updatedSupportedFormatsStates = supportedFormats.ToDictionary(f => f, f => f.IsCompressedArchiveFormatEnabled());
                var expectedStates = new Dictionary<CompressedArchiveFormat, bool>(supportedFormatsStates);
                expectedStates[CompressedArchiveFormat.Zip] = true;
                expectedStates[CompressedArchiveFormat.GZip] = false;
                expectedStates[CompressedArchiveFormat.Tar] = true;
                expectedStates[CompressedArchiveFormat.BZip2] = false;
                Assert.All(updatedSupportedFormatsStates, s => Assert.Equal(expectedStates[s.Key], s.Value));
            }
            finally
            {
                foreach (var supportedFormat in supportedFormats)
                {
                    supportedFormat.EnableCompressedArchiveFormat();
                }
            }
        }

        private static IEnumerable<CompressedArchiveFormat> GetSupportedStandardFormats()
        {
            return GetAllStandardFormats().Where(f => f.IsCompressedArchiveFormatSupported());
        }

        private static IEnumerable<CompressedArchiveFormat> GetAllStandardFormats()
        {
            var values = Enum.GetValues(typeof(CompressedArchiveFormat)).Cast<CompressedArchiveFormat>();
            return values.Where(f => f != CompressedArchiveFormat.None);
        }
    }
}
