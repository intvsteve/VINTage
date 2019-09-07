// <copyright file="ResourceHelpersTests.cs" company="INTV Funhouse">
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
using System.IO;
using System.Linq;
using INTV.Shared.Utility;
using INTV.TestHelpers.Shared.Utility;
using INTV.TestHelpers.Shared.Xunit;
using Xunit;

namespace INTV.Shared.Tests.Utility
{
    public class ResourceHelpersTests
    {
        private const string PackedResourceHeader = "pack://application:,,,/";
        private const string PackedResourceHeaderForTestResources = PackedResourceHeader + "INTV.TestHelpers.Shared;component/";

        #region CreatePackedResourceString Tests

        [Fact]
        public void ResourceHelpers_CreatePackedResourceStringWithNullType_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => ResourceHelpers.CreatePackedResourceString(null, "path"));
        }

        [Fact]
        public void ResourceHelpers_CreatePackedResourceStringWithNullRelativeResourcePath_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => this.CreatePackedResourceString(null));
        }

        [Theory]
        [InlineData(PackedResourceHeader)]
        [InlineData(PackedResourceHeader + "boring_resource.png")]
        [InlineData(PackedResourceHeader + "flubber!")]
        [InlineData(PackedResourceHeader + "\n")]
        public void ResourceHelpers_CreatePackedResourceStringWithAbsoluteResourcePath_ReturnsUnchangedResourceString(string relativeResourcePath)
        {
            var resourceUri = TestResource.Invalid.CreatePackedResourceString(relativeResourcePath);

            Assert.Equal(relativeResourcePath, resourceUri);
        }

        [Fact]
        public void ResourceHelpers_CreatePackedResourceStringWithNullObject_ReturnsValidResourceString()
        {
            object thing = null;

            var resourceUri = thing.CreatePackedResourceString("path");

            var expectedResourceUri = "pack://application:,,,/mscorlib;component/path";
            Assert.Equal(expectedResourceUri, resourceUri);
        }

        [Theory]
        [InlineData("")]
        [InlineData("flim")]
        [InlineData("tests.jpg")]
        [InlineData("what evs man, it's your test")]
        public void ResourceHelpers_CreatePackedResourceStringWithRelativeResourcePath_ReturnsExpectedResourceString(string relativeResourcePath)
        {
            var resourceUri = TestResource.Invalid.CreatePackedResourceString(relativeResourcePath);

            var expectedPackedResourceUri = PackedResourceHeaderForTestResources + relativeResourcePath;
            Assert.Equal(expectedPackedResourceUri, resourceUri);
        }

        #endregion // CreatePackedResourceString Tests

        #region LoadImageResource Tests

        [STAFact(UsePackUriApplication = true)]
        public void ResourceHelpers_LoadImageResourceWithInvalidName_ThrowsIOException()
        {
            var missingResource = TestResource.Invalid;

            Assert.Throws<IOException>(() => missingResource.LoadImageResource(missingResource.Name));
        }

        [STAFact(UsePackUriApplication = true)]
        public void ResourceHelpers_LoadImageResourceWithInvalidFormat_ThrowsNotSupportedException()
        {
            var notAnImageImageResource = TestResource.TextResourceFile;

            Assert.Throws<NotSupportedException>(() => notAnImageImageResource.LoadImageResource(notAnImageImageResource.Name));
        }

        [STAFact(UsePackUriApplication = true)]
        public void ResourceHelpers_LoadImageResource_LoadsImage()
        {
            var iconResource = TestResource.ConsoleIcon;

            var image = iconResource.LoadImageResource(iconResource.Name);

            Assert.NotNull(image);
        }

        #endregion // LoadImageResource Tests

        #region GetResourceString Tests

        [Fact]
        public void ResourceHelpers_GetResourceStringForNullType_ThrowsNullReferenceException()
        {
            Type nullType = null;

            Assert.Throws<NullReferenceException>(() => nullType.GetResourceString("key"));
        }

        [Fact]
        public void ResourceHelpers_GetResourceStringForNullKey_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => this.GetType().GetResourceString(null));
        }

        [Fact]
        public void ResourceHelpers_GetResourceStringForDoubleResource_ReturnsNotAStringValue()
        {
            var testResource = TestResource.TestDoubleInResx;
            var type = testResource.GetType();

            var resourceString = type.GetResourceString(testResource.Name);

            var expectedResourceString = GetNotAStringResourceValue(GetResourceName(type, testResource.ResourceName), testResource.Name);
            Assert.Equal(expectedResourceString, resourceString);
        }

        [Fact]
        public void ResourceHelpers_GetResourceStringForEmptyKey_ReturnsNoResourcesFound()
        {
            var type = this.GetType();

            var resourceString = type.GetResourceString(string.Empty);

            var expectedResourceString = GetMissingManifestResourceStringValue(GetResourceName(type, TestResource.StringsResourceName), string.Empty);
            Assert.Equal(expectedResourceString, resourceString);
        }

        [Fact]
        public void ResourceHelpers_GetResourceString_ReturnsExpectedString()
        {
            var testResource = TestResource.TestString00InResx;
            var type = testResource.GetType();

            var resourceString = type.GetResourceString(testResource.Name);

            Assert.Equal(testResource.ExpectedValue, resourceString);
        }

        [Fact]
        public void ResourceHelpers_GetResourceStringNotInSatellite_ReturnsExpectedString()
        {
            using (new TestCultureInfo("de"))
            {
                var testResource = TestResource.TestStringOriginalOnlyInResx;
                var type = testResource.GetType();

                var resourceString = type.GetResourceString(testResource.Name);

                Assert.Equal(testResource.ExpectedValue, resourceString);
            }
        }

        [Fact]
        public void ResourceHelpers_GetResourceStringFromSatellite_ReturnsExpectedString()
        {
            using (new TestCultureInfo("de-DE"))
            {
                var testResource = TestResource.TestString00InResx;
                var type = testResource.GetType();

                var resourceString = type.GetResourceString(testResource.Name);

                Assert.Equal("Ein Teststring", resourceString);
            }
        }

        [Fact]
        public void ResourceHelpers_GetResourceStringInUnavailableLanguage_ReturnsDefaultValue()
        {
            using (new TestCultureInfo("mk"))
            {
                var testResource = TestResource.TestString00InResx;
                var type = testResource.GetType();

                var resourceString = type.GetResourceString(testResource.Name);

                Assert.Equal(testResource.ExpectedValue, resourceString);
            }
        }

        #endregion // GetResourceString Tests

        #region GetResources Tests

        [Fact]
        public void ResourceHelpers_GetResourcesWithNullType_ThrowsNullReferenceException()
        {
            Type type = null;

            Assert.Throws<NullReferenceException>(() => type.GetResources("filter"));
        }

        [Fact]
        public void ResourceHelpers_GetResourcesWithNullPrefix_ThrowsArgumentNullException()
        {
            var type = TestResource.Invalid.GetType();

            Assert.Throws<ArgumentNullException>(() => type.GetResources(null).FirstOrDefault());
        }

        [Fact]
        public void ResourceHelpers_GetResourcesFromAssemblyWithUnrecognizedPrefix_ReturnsEmptyCollection()
        {
            var type = TestResource.Invalid.GetType();

            var matchingResources = type.GetResources("prankster");

            Assert.False(matchingResources.Any());
        }

        [Fact]
        public void ResourceHelpers_GetResourcesFromAssemblyWithPrefix_ExpectedResources()
        {
            var type = TestResource.Invalid.GetType();

            var matchingResources = type.GetResources(TestResource.ResourcePrefix + "tagalong");

            Assert.True(matchingResources.Count() >= 3);
        }

        [Fact]
        public void ResourceHelpers_GetResourcesFromAssemblyWithNoResourcesWithNullPrefix_ReturnsEmptyList()
        {
            var type = this.GetType();

            Assert.Empty(type.GetResources(null));
        }

        #endregion // GetResources Tests

        #region ExtractResourcesToFiles Tests

        [Fact]
        public void ResourceHelpers_ExtractResourcesToFilesUsingNullType_ThrowsNullReferenceException()
        {
            Type type = null;
            var resourcesToExtract = new[] { "dummy.txt" };

            Assert.Throws<NullReferenceException>(() => type.ExtractResourcesToFiles(resourcesToExtract, TestResource.ResourcePrefix, Path.GetTempPath()));
        }

        [Fact]
        public void ResourceHelpers_ExtractResourcesToFilesUsingNullResourcesToExtract_ThrowsArgumentNullException()
        {
            var type = typeof(TemporaryFile);

            Assert.Throws<ArgumentNullException>(() => type.ExtractResourcesToFiles(null, TestResource.ResourcePrefix, Path.GetTempPath()));
        }

        [Fact]
        public void ResourceHelpers_ExtractResourcesToFilesUsingNullDestinationDirectory_ThrowsArgumentNullReference()
        {
            var type = typeof(TemporaryFile);
            var resourcesToExtract = new[] { "dummy.txt" };

            Assert.Throws<ArgumentNullException>(() => type.ExtractResourcesToFiles(resourcesToExtract, TestResource.ResourcePrefix, null));
        }

        [Fact]
        public void ResourceHelpers_ExtractResourcesToFilesUsingNullPrefixAndGetNameCallback_ThrowsNullReferenceException()
        {
            var type = typeof(TemporaryFile);
            var resourcesToExtract = new[] { "dummy.txt" };

            Assert.Throws<NullReferenceException>(() => type.ExtractResourcesToFiles(resourcesToExtract, null, Path.GetTempPath()));
        }

        [Fact]
        public void ResourceHelpers_ExtractResourcesToFilesUsingEmptyResourcesToExtract_ReturnsEmptyResults()
        {
            var type = typeof(TemporaryFile);

            var extractedResourceFiles = type.ExtractResourcesToFiles(Enumerable.Empty<string>(), TestResource.ResourcePrefix, Path.GetTempPath());

            Assert.False(extractedResourceFiles.Any());
        }

        [Fact]
        public void ResourceHelpers_ExtractResourcesToFile_ProducesExpectedFile()
        {
            var testResource = TestResource.TextEmbeddedResourceFile;
            var type = testResource.GetType();

            var expectedFilePath = Path.Combine(Path.GetTempPath(), "embedded resource file.txt");
            using (var temporaryFile = TemporaryFile.CreateTemporaryFileWithPath(expectedFilePath, createEmptyFile: false))
            {
                var extractedResourceFile = type.ExtractResourcesToFiles(new[] { testResource.Name }, TestResource.ResourcePrefix, Path.GetTempPath());

                Assert.True(File.Exists(expectedFilePath));
            }
        }

        [Fact]
        public void ResourceHelpers_ExtractResourcesToExistingFilesWithCustomFileNames_ProducesExpectedFilePathssAndDoesNotChangeContents()
        {
            var testResources = new[] { TestResource.TagalongZip, TestResource.TagalongDirZip };
            var type = testResources[0].GetType();
            var expectedFilePaths = testResources.Select(r => Path.Combine(Path.GetTempPath(), GetFileNameForResource(r.Name, null))).ToArray();

            using (var firstFile = TemporaryFile.CreateTemporaryFileWithPath(expectedFilePaths[0], createEmptyFile: true))
            using (var secondFile = TemporaryFile.CreateTemporaryFileWithPath(expectedFilePaths[1], createEmptyFile: true))
            {
                var extractedResourceFiles = type.ExtractResourcesToFiles(testResources.Select(r => r.Name), TestResource.ResourcePrefix, Path.GetTempPath(), GetFileNameForResource);

                Assert.Equal(expectedFilePaths, extractedResourceFiles);
                Assert.All(extractedResourceFiles, f => Assert.True(File.Exists(f)));
                Assert.All(extractedResourceFiles, f => Assert.Equal(0L, new FileInfo(f).Length));
            }
        }

        #endregion // ExtractResourcesToFiles Tests

        #region Helpers

        private static string GetFileNameForResource(string resourceName, Stream resourceStream)
        {
            var fileExtension = Path.GetExtension(resourceName);
            var fileNamePrefix = Path.GetFileNameWithoutExtension(resourceName).Replace('.', '_');
            var fileName = fileNamePrefix + fileExtension;
            return fileName;
        }

        private static string GetResourceName(Type type, string resourceName)
        {
            var assembly = type.Assembly;
            resourceName = assembly.GetName().Name + ".Resources." + resourceName;
            return resourceName;
        }

        private static string GetNotAStringResourceValue(string resourceName, string key)
        {
            var missingStringResourceValue = "!!RESOURCE '" + resourceName + "." + key + " ' IS NOT A STRING!!";
            return missingStringResourceValue;
        }

        private static string GetMissingManifestResourceStringValue(string resourceName, string key)
        {
            var missingManifestResourceStringValue = "!!NO RESOURCES FOUND FOR '" + resourceName + "." + key + "'!!";
            return missingManifestResourceStringValue;
        }

        #endregion // Helpers
    }
}
