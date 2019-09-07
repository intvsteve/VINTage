// <copyright file="CompressedArchiveFormatTests.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class CompressedArchiveFormatTests : ICompressedArchiveTest
    {
        [Theory]
        [InlineData(CompressedArchiveFormat.None, false)]
        [InlineData(CompressedArchiveFormat.Zip, true)]
        [InlineData(CompressedArchiveFormat.GZip, true)]
        [InlineData(CompressedArchiveFormat.Tar, true)]
        [InlineData(CompressedArchiveFormat.BZip2, false)]
        public void CompressedArchiveFormat_IsCompressedArchiveFormatSupported_ReturnsCorrectValue(CompressedArchiveFormat format, bool expectedIsSupported)
        {
            Assert.Equal(expectedIsSupported, format.IsCompressedArchiveFormatSupported());
        }

        public static IEnumerable<object[]> CompressedArchiveFormatFileExtensionsTestData
        {
            get
            {
                yield return new object[] { CompressedArchiveFormat.None, Enumerable.Empty<string>() };
                yield return new object[] { CompressedArchiveFormat.Zip, new[] { ".zip" } };
                yield return new object[] { CompressedArchiveFormat.GZip, new[] { ".gz", ".gzip" } };
                yield return new object[] { CompressedArchiveFormat.Tar, new[] { ".tar" } };
                yield return new object[] { CompressedArchiveFormat.BZip2, Enumerable.Empty<string>() }; // new[] { ".bz2" }
                yield return new object[] { (CompressedArchiveFormat)123456, Enumerable.Empty<string>() };
            }
        }

        [Theory]
        [MemberData("CompressedArchiveFormatFileExtensionsTestData")]
        public void CompressedArchiveFormat_FileExtensions_ReturnsExpectedExtensions(CompressedArchiveFormat format, IEnumerable<string> expectedExtensions)
        {
            var extensions = format.FileExtensions();

            Assert.Equal(expectedExtensions, extensions, StringComparer.InvariantCultureIgnoreCase);
        }

        public static IEnumerable<object[]> CompressedFormatsFromFileNameTestData
        {
            get
            {
                yield return new object[] { null, Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { string.Empty, Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { ".", Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { @"\", Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { "/", Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { "foo.zip", new[] { CompressedArchiveFormat.Zip } };
                yield return new object[] { "foo.ZiP", new[] { CompressedArchiveFormat.Zip } };
                yield return new object[] { "foo.Bz2", new[] { CompressedArchiveFormat.BZip2 } };
                yield return new object[] { "foo.Gz", new[] { CompressedArchiveFormat.GZip } };
                yield return new object[] { "bar.tAr", new[] { CompressedArchiveFormat.Tar } };
                yield return new object[] { "baz.Tar.zip", new[] { CompressedArchiveFormat.Zip, CompressedArchiveFormat.Tar } };
                yield return new object[] { "baz.Tar.gZ", new[] { CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar } };
                yield return new object[] { "baz.Tar.bZ", Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { "baz.Tar.bZ2", new[] { CompressedArchiveFormat.BZip2, CompressedArchiveFormat.Tar } };
                yield return new object[] { "baz.Tar.zip.", Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { "baz.Tar.a.zip", new[] { CompressedArchiveFormat.Zip } };
                yield return new object[] { "baz..zip", new[] { CompressedArchiveFormat.Zip } };
                yield return new object[] { "tar.gz", new[] { CompressedArchiveFormat.GZip } };
                yield return new object[] { ".tar.gz", new[] { CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar } };
                yield return new object[] { "baz.txt.zip", new[] { CompressedArchiveFormat.Zip } };
                yield return new object[] { @"x:\baz.zip.txt", Enumerable.Empty<CompressedArchiveFormat>() };
                yield return new object[] { "baz.zip.tar.gz.tar.zip.bz2", new[] { CompressedArchiveFormat.BZip2, CompressedArchiveFormat.Zip, CompressedArchiveFormat.Tar, CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar, CompressedArchiveFormat.Zip } };
                yield return new object[] { ".TgZ", new[] { CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar } };
                yield return new object[] { ".tBZ", new[] { CompressedArchiveFormat.BZip2, CompressedArchiveFormat.Tar } };
                yield return new object[] { ".tbZ2", new[] { CompressedArchiveFormat.BZip2, CompressedArchiveFormat.Tar } };
                yield return new object[] { ".zip.tgz", new[] { CompressedArchiveFormat.GZip, CompressedArchiveFormat.Tar, CompressedArchiveFormat.Zip } };
                yield return new object[] { ".tbz2.zip", new[] { CompressedArchiveFormat.Zip, CompressedArchiveFormat.BZip2, CompressedArchiveFormat.Tar } };
            }
        }

        [Theory]
        [MemberData("CompressedFormatsFromFileNameTestData")]
        public void CompressedArchiveFormat_GetCompressedArchiveFormatsFromFileName_ReturnsExpectedFormts(string fileName, IEnumerable<CompressedArchiveFormat> expectedFormats)
        {
            var formats = fileName.GetCompressedArchiveFormatsFromFileName();

            Assert.Equal(expectedFormats, formats);
        }

        public static IEnumerable<object[]> CompressedArchiveFormatImplementationsTestData
        {
            get
            {
                yield return new object[] { CompressedArchiveFormat.None, Enumerable.Empty<CompressedArchiveAccessImplementation>() };
                yield return new object[] { CompressedArchiveFormat.Zip, new[] { CompressedArchiveAccessImplementation.Native, CompressedArchiveAccessImplementation.SharpZipLib } };
                yield return new object[] { CompressedArchiveFormat.GZip, new[] { CompressedArchiveAccessImplementation.Native, CompressedArchiveAccessImplementation.SharpZipLib } };
                yield return new object[] { CompressedArchiveFormat.Tar, new[] { CompressedArchiveAccessImplementation.SharpZipLib } };
                yield return new object[] { CompressedArchiveFormat.BZip2, Enumerable.Empty<CompressedArchiveAccessImplementation>() }; // new[] { CompressedArchiveAccessImplementation.SharpZipLib }
                yield return new object[] { (CompressedArchiveFormat)987654, Enumerable.Empty<CompressedArchiveAccessImplementation>() };
            }
        }

        [Theory]
        [MemberData("CompressedArchiveFormatImplementationsTestData")]
        public void CompressedArchiveFormat_GetAvailableCompressedArchiveImplementations_ReturnsExpectedImplementations(CompressedArchiveFormat format, IEnumerable<CompressedArchiveAccessImplementation> expectedImplementations)
        {
            var implementations = format.GetAvailableCompressedArchiveImplementations();

            Assert.Equal(expectedImplementations, implementations);
        }

        [Theory]
        [InlineData(CompressedArchiveFormat.None, CompressedArchiveAccessImplementation.None)]
        [InlineData(CompressedArchiveFormat.Zip, CompressedArchiveAccessImplementation.Native)]
        [InlineData(CompressedArchiveFormat.GZip, CompressedArchiveAccessImplementation.Native)] // SharpZipLib
        [InlineData(CompressedArchiveFormat.Tar, CompressedArchiveAccessImplementation.SharpZipLib)] // SharpZipLib
        [InlineData(CompressedArchiveFormat.BZip2, CompressedArchiveAccessImplementation.None)] // SharpZipLib
        [InlineData((CompressedArchiveFormat)24680, CompressedArchiveAccessImplementation.None)]
        public void CompressedArchiveFormat_GetPreferredCompressedArchiveImplementation_ReturnsExpectedPreferredImplementation(CompressedArchiveFormat format, CompressedArchiveAccessImplementation expectedPreferredImplementation)
        {
            var preferredImplementation = format.GetPreferredCompressedArchiveImplementation();

            Assert.Equal(expectedPreferredImplementation, preferredImplementation);
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormatWithInvalidFormat_ThrowsArgumentOutOfRangeException()
        {
            var fileExtensions = new[] { ".none", ".NONE" };
            var implementations = new[] { CompressedArchiveAccessImplementation.Other, CompressedArchiveAccessImplementation.Native };

            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveFormat.None.RegisterCompressedArchiveFormat(fileExtensions, implementations));
            Assert.Equal("format", exception.ParamName);
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormatWithNullFileExtensions_ThrowsArgumentNullException()
        {
            var implementations = new[] { CompressedArchiveAccessImplementation.Other, CompressedArchiveAccessImplementation.Native };

            var exception = Assert.Throws<ArgumentNullException>(() => CompressedArchiveFormat.Zip.RegisterCompressedArchiveFormat(null, implementations));
            Assert.Equal("fileExtensions", exception.ParamName);
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormatWithEmptyFileExtensions_ThrowsArgumentException()
        {
            var implementations = new[] { CompressedArchiveAccessImplementation.Other, CompressedArchiveAccessImplementation.Native };

            var exception = Assert.Throws<ArgumentException>(() => CompressedArchiveFormat.GZip.RegisterCompressedArchiveFormat(Enumerable.Empty<string>(), implementations));
            Assert.Equal("fileExtensions", exception.ParamName);
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormatWithNullImplementations_ThrowsArgumentNullException()
        {
            var fileExtensions = new[] { ".tar", ".TAR" };

            var exception = Assert.Throws<ArgumentNullException>(() => CompressedArchiveFormat.Tar.RegisterCompressedArchiveFormat(fileExtensions, null));
            Assert.Equal("implementations", exception.ParamName);
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormatWithEmptyImplementations_ThrowsArgumentException()
        {
            var fileExtensions = new[] { ".bz2", ".BZ2" };

            var exception = Assert.Throws<ArgumentException>(() => CompressedArchiveFormat.BZip2.RegisterCompressedArchiveFormat(fileExtensions, Enumerable.Empty<CompressedArchiveAccessImplementation>()));
            Assert.Equal("implementations", exception.ParamName);
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveWithConflictingFileExtension_ThrowsArgumentException()
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();
            var fileExtensions = new[] { ".zippy", ".ZIP" };
            var implementations = new[] { this.GetFakeCompressedArchiveAccessImplementationForTest() };

            var exception = Assert.Throws<ArgumentException>(() => format.RegisterCompressedArchiveFormat(fileExtensions, implementations));
            Assert.Equal(exception.ParamName, "fileExtension");
            Assert.True(exception.Message.Contains("'.ZIP'"));
            Assert.True(exception.Message.Contains("'Zip'"));
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormat_Succeeds()
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();
            var implementation0 = this.GetFakeCompressedArchiveAccessImplementationForTest();
            var implementation1 = this.GetFakeCompressedArchiveAccessImplementationForTest();
            var defaultFileExtension = ".xyz" + implementation0;
            var fileExtensions = new[] { defaultFileExtension, ".xyz" + implementation1 };

            var registered = format.RegisterCompressedArchiveFormat(fileExtensions, new[] { implementation0, implementation1 });

            Assert.True(registered);
            Assert.Equal(defaultFileExtension, format.FileExtensions().First());
            Assert.Equal(implementation0, format.GetPreferredCompressedArchiveImplementation());
        }

        [Fact]
        public void CompressedArchiveFormat_RegisterCompressedArchiveFormatSecondTimeWithSameData_ReturnsFalseAndDoesNotAlterData()
        {
            var format = this.RegisterTestCompressedArchiveFormat(numberOfFileExtensionsAndImplementations: 3);
            var fileExtensions = format.FileExtensions();
            var implementations = format.GetAvailableCompressedArchiveImplementations();

            var registered = format.RegisterCompressedArchiveFormat(fileExtensions, implementations);

            Assert.False(registered);
            Assert.Equal(fileExtensions, format.FileExtensions());
            Assert.Equal(implementations, format.GetAvailableCompressedArchiveImplementations());
            Assert.Equal(implementations.First(), format.GetPreferredCompressedArchiveImplementation());
        }

        [Fact]
        public void CompressedArchiveFormat_AddExtensionToFormatNone_ThrowsArgumentOutOfRangeException()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveFormat.None.AddFileExtension(null, makeDefault: false));
            Assert.Equal(exception.ParamName, "format");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("dud")]
        [InlineData(".\b\r\t\n")]
        [InlineData(".biff.bar")]
        public void CompressedArchiveFormat_AddInvalidFileExtension_ThrowsArgumentException(string fileExtension)
        {
            var exception = Assert.Throws<ArgumentException>(() => CompressedArchiveFormat.BZip2.AddFileExtension(fileExtension, makeDefault: false));
            Assert.Equal(exception.ParamName, "fileExtension");
        }

        [Fact]
        public void CompressedArchiveFormat_AddFileExtensionForUnregisteredFormat_ThrowsArgumentException()
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();

            var exception = Assert.Throws<ArgumentException>(() => format.AddFileExtension(".googoo", makeDefault: false));
            Assert.Equal(exception.ParamName, "format");
            Assert.True(exception.Message.Contains("'" + format + "'"));
        }

        [Fact]
        public void CompressedArchiveFormat_AddFileExtension_AddsFileExtension()
        {
            var format = this.RegisterTestCompressedArchiveFormat();
            var fileExtension = format.FileExtensions().First();
            var newFileExtension = fileExtension + format.GetPreferredCompressedArchiveImplementation();

            var added = format.AddFileExtension(newFileExtension, makeDefault: false);

            Assert.True(added);
            Assert.Equal(fileExtension, format.FileExtensions().First());
            Assert.Equal(newFileExtension, format.FileExtensions().Last());
        }

        [Fact]
        public void CompressedArchiveFormat_AddDuplicateFileExtension_DoesNotAddFileExtension()
        {
            var format = this.RegisterTestCompressedArchiveFormat();
            var fileExtension = format.FileExtensions().First();
            var newFileExtension = fileExtension.ToUpperInvariant();

            var added = format.AddFileExtension(newFileExtension, makeDefault: true);

            Assert.False(added);
            Assert.Equal(fileExtension, format.FileExtensions().First());
            Assert.Equal(1, format.FileExtensions().Count());
        }

        [Fact]
        public void CompressedArchiveFormat_AddNewFileExtensionAsDefault_MakesNewFileExtensionDefault()
        {
            var format = this.RegisterTestCompressedArchiveFormat(numberOfFileExtensionsAndImplementations: 2);
            var lastFileExtension = format.FileExtensions().Last();
            var newFileExtension = format.FileExtensions().First() + "abc123";

            var added = format.AddFileExtension(newFileExtension, makeDefault: true);

            Assert.True(added);
            Assert.Equal(newFileExtension, format.FileExtensions().First());
        }

        [Fact]
        public void CompressedArchiveFormat_AddNewFileExtensionToChangeDefault_ChangesDefaultFileExtension()
        {
            var format = this.RegisterTestCompressedArchiveFormat(numberOfFileExtensionsAndImplementations: 2);
            var firstFileExtension = format.FileExtensions().First();
            var newDefaultFileExtension = format.FileExtensions().Last();

            var added = format.AddFileExtension(newDefaultFileExtension, makeDefault: true);

            Assert.False(added);
            Assert.Equal(newDefaultFileExtension, format.FileExtensions().First());
        }

        [Fact]
        public void CompressedArchiveFormat_AddImplementationToFormatNone_ThrowsArgumentOutOfRangeException()
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveFormat.None.AddImplementation(CompressedArchiveAccessImplementation.Native, makePreferred: false));
            Assert.Equal(exception.ParamName, "format");
        }

        [Theory]
        [InlineData(CompressedArchiveAccessImplementation.None)]
        [InlineData(CompressedArchiveAccessImplementation.Any)]
        public void CompressedArchiveFormat_AddInvalidImplementationToFormat_ThrowsArgumentOutOfRangeException(CompressedArchiveAccessImplementation implementation)
        {
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => CompressedArchiveFormat.BZip2.AddImplementation(implementation, makePreferred: false));
            Assert.Equal(exception.ParamName, "implementation");
        }

        [Fact]
        public void CompressedArchiveFormat_AddImplementationToUnregisteredFormat_ThrowsArgumentException()
        {
            var format = this.GetFakeCompressedArchiveFormatForTest();

            var exception = Assert.Throws<ArgumentException>(() => format.AddImplementation(CompressedArchiveAccessImplementation.Native, makePreferred: false));
            Assert.Equal(exception.ParamName, "format");
        }

        [Fact]
        public void CompressedArchiveFormat_AddNewPreferredImplementation_ChangesPreferredImplementation()
        {
            var format = this.RegisterTestCompressedArchiveFormat(numberOfFileExtensionsAndImplementations: 2);
            var preferredImplementation = format.GetPreferredCompressedArchiveImplementation();
            var newPreferredImplementation = this.GetFakeCompressedArchiveAccessImplementationForTest();

            var added = format.AddImplementation(newPreferredImplementation, makePreferred: true);

            Assert.True(added);
            Assert.Equal(newPreferredImplementation, format.GetPreferredCompressedArchiveImplementation());
            Assert.Equal(newPreferredImplementation, format.GetAvailableCompressedArchiveImplementations().First());
        }

        [Fact]
        public void CompressedArchiveFormat_ChangePreferredImplementation_ChangesPreferredImplementation()
        {
            var format = this.RegisterTestCompressedArchiveFormat(numberOfFileExtensionsAndImplementations: 3);
            var preferredImplementation = format.GetPreferredCompressedArchiveImplementation();
            var newPreferredImplementation = format.GetAvailableCompressedArchiveImplementations().Last();

            var added = format.AddImplementation(newPreferredImplementation, makePreferred: true);

            Assert.False(added);
            Assert.Equal(newPreferredImplementation, format.GetPreferredCompressedArchiveImplementation());
            Assert.Equal(newPreferredImplementation, format.GetAvailableCompressedArchiveImplementations().First());
        }
    }
}
