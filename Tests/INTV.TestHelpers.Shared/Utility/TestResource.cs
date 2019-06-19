// <copyright file="TestResource.cs" company="INTV Funhouse">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTV.TestHelpers.Shared.Utility
{
    /// <summary>
    /// Different kinds of resources available in this assembly for test purposes.
    /// </summary>
    public enum TestResourceKind
    {
        /// <summary>Not a valid resource kind.</summary>
        None,

        /// <summary>A string, usually in Strings.resx.</summary>
        ResxString,

        /// <summary>A double-precision number, usually in Strings.resx.</summary>
        ResxDouble,

        /// <summary>An icon, usually in Strings.resx.</summary>
        ResxIcon,

        /// <summary>An image, usually in Strings.resx.</summary>
        ResxImage,

        /// <summary>A file, usually in Strings.resx.</summary>
        ResxFile,

        /// <summary>An image resource.</summary>
        ResourceImage,

        /// <summary>A file resource.</summary>
        ResourceFile,

        /// <summary>An embedded file resource.</summary>
        EmbeddedResourceFile,
    }

    /// <summary>
    /// Some test resources.
    /// </summary>
    public class TestResource
    {
        /// <summary>The default resource container.</summary>
        public const string StringsResourceName = "Strings";

        /// <summary>An alternative resource container.</summary>
        public const string MoreResourcesResourceName = "MoreResources";

        /// <summary>The prefix to use when retrieving an embedded resource.</summary>
        public static readonly string ResourcePrefix = typeof(TestResource).Assembly.GetName().Name + ".Resources.";

        /// <summary>An invalid resource name (as in, one that cannot be located).</summary>
        public static readonly TestResource Invalid = new TestResource(TestResourceKind.None, "<not a valid resource>");

        /// <summary>A double-precision number resource that can be accessed via a pack:// URI.</summary>
        public static readonly TestResource TestDoubleInResx = new TestResource(TestResourceKind.ResxDouble, "TestDouble");

        /// <summary>A image resource that can be accessed via a pack:// URI.</summary>
        public static readonly TestResource ConsoleIcon = new TestResource(TestResourceKind.ResourceImage, "Resources/console_16xLG.png");

        /// <summary>A image resource that can be accessed via a pack:// URI.</summary>
        public static readonly TestResource RunningManIcon = new TestResource(TestResourceKind.ResourceImage, "Resources/inty_icon_white.png");

        /// <summary>An icon resource that can be accessed via a pack:// URI.</summary>
        public static readonly TestResource IconInResx = new TestResource(TestResourceKind.ResxDouble, "TestIconResource", resourceName: MoreResourcesResourceName);

        /// <summary>A file resource that can be accessed via a pack:// URI.</summary>
        public static readonly TestResource TextResourceFile = new TestResource(TestResourceKind.ResourceFile, "Resources/resource_file.txt");

        /// <summary>A string that is in the main and a satellite resource.</summary>
        public static readonly TestResource TestString00InResx = new TestResource(TestResourceKind.ResxDouble, "TestString_00", expectedValue: "A test string");

        /// <summary>A string that is only in the main Strings.resx resource.</summary>
        public static readonly TestResource TestStringOriginalOnlyInResx = new TestResource(TestResourceKind.ResxDouble, "TestString_OriginalOnly", expectedValue: "This string is not in any satellites.");

        /// <summary>An embedded ZIP file.</summary>
        public static readonly TestResource TagalongZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong.zip")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong.cfg" }
        };

        /// <summary>An embedded ZIP file that contains files in a subdirectory.</summary>
        public static readonly TestResource TagalongDirZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_dir.zip")
        {
            ArchiveContents = new[] { "tagalong_dir/", "tagalong_dir/tagalong.luigi", "tagalong_dir/tagalong.rom" }
        };

        /// <summary>An embedded resource ZIP file that only contains an empty directory.</summary>
        public static readonly TestResource TagalongEmptyZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_empty.zip")
        {
            ArchiveContents = new[] { "tagalong_empty" }
        };

        /// <summary>An embedded resource ZIP file that contains another ZIP file.</summary>
        public static readonly TestResource TagalongNestedZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_nested.zip")
        {
            ArchiveContents = new[] { "tagalong.zip" }
        };

        /// <summary>An embedded resource GZIP file that only contains tagalong.bin including the file name.</summary>
        public static readonly TestResource TagalongBinGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong.bin.gz")
        {
            ArchiveContents = new[] { "tagalong.bin" }
        };

        /// <summary>An embedded resource GZIP file that only contains tagalong.cfg including the file name.</summary>
        public static readonly TestResource TagalongCfgGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong.cfg.gz")
        {
            ArchiveContents = new[] { "tagalong.cfg" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin and tagalong.cfg, but without file names.</summary>
        public static readonly TestResource TagalongBinCfgNNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bc_nn.gz")
        {
            ArchiveContents = new[] { "tagalong_bc_nn", "tagalong_bc_nn_1" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin (no file name) and tagalong.cfg (with file name).</summary>
        public static readonly TestResource TagalongBinCfgNYGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bc_ny.gz")
        {
            ArchiveContents = new[] { "tagalong_bc_ny", "tagalong.cfg" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin (with file name) and tagalong.cfg (without file name).</summary>
        public static readonly TestResource TagalongBinCfgYNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bc_yn.gz")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong_bc_yn" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin and tagalong.cfg with file names.</summary>
        public static readonly TestResource TagalongBinCfgYYGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bc_yy.gz")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong.cfg" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with no file names.</summary>
        public static readonly TestResource TagalongBCLRNNNNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_nnnn.gz")
        {
            ArchiveContents = new[] { "tagalong_bclr_nnnn", "tagalong_bclr_nnnn_1", "tagalong_bclr_nnnn_2", "tagalong_bclr_nnnn_3" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with some file names.</summary>
        public static readonly TestResource TagalongBCLRNNYNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_nnyn.gz")
        {
            ArchiveContents = new[] { "tagalong_bclr_nnyn", "tagalong_bclr_nnyn_1", "tagalong.luigi", "tagalong_bclr_nnyn_3" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with  some file names.</summary>
        public static readonly TestResource TagalongBCLRNYNNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_nynn.gz")
        {
            ArchiveContents = new[] { "tagalong_bclr_nynn", "tagalong.cfg", "tagalong_bclr_nynn_2", "tagalong_bclr_nynn_3" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with  some file names.</summary>
        public static readonly TestResource TagalongBCLRNYNYGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_nyny.gz")
        {
            ArchiveContents = new[] { "tagalong_bclr_nyny", "tagalong.cfg", "tagalong_bclr_nyny_2", "tagalong.rom" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with  some file names.</summary>
        public static readonly TestResource TagalongBCLRNYYNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_nyyn.gz")
        {
            ArchiveContents = new[] { "tagalong_bclr_nyyn", "tagalong.cfg", "tagalong.luigi", "tagalong_bclr_nyyn_3" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with  some file names.</summary>
        public static readonly TestResource TagalongBCLRYNNYGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_ynny.gz")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong_bclr_ynny", "tagalong_bclr_ynny_2", "tagalong.rom" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with  some file names.</summary>
        public static readonly TestResource TagalongBCLRYNYNGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_ynyn.gz")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong_bclr_ynyn", "tagalong.luigi", "tagalong_bclr_ynyn_3" }
        };

        /// <summary>An embedded resource GZIP file that contains tagalong.bin, .cfg, .luigi and .rom with file names.</summary>
        public static readonly TestResource TagalongBCLRYYYYGZip = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bclr_yyyy.gz")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong.cfg", "tagalong.luigi", "tagalong.rom" }
        };

        /// <summary>An embedded resource TAR file that contains tagalong.bin and tagalong.cfg. (Created via 7-Zip)</summary>
        public static readonly TestResource TagalongBinCfgTar = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_bc.tar")
        {
            ArchiveContents = new[] { "tagalong.bin", "tagalong.cfg" }
        };

        /// <summary>An embedded resource TAR file that contains tagalong.luigi and tagalong.rom in a subdirectory. (Created on dev machine in PowerShell)</summary>
        public static readonly TestResource TagalongDirLuigiRomTar = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_dir_lr.tar")
        {
            ArchiveContents = new[] { "tagalong_dir/", "tagalong_dir/tagalong.luigi", "tagalong_dir/tagalong.rom" }
        };

        /// <summary>An embedded resource TAR file that contains tagalong.cc3 and tagalong.rom. (Created in MSYS2 (64-bit)</summary>
        public static readonly TestResource TagalongCC3RomTar = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "tagalong_rc.tar")
        {
            ArchiveContents = new[] { "tagalong.rom", "tagalong.cc3" }
        };

        /// <summary>An embedded resource text file with a space in the name.</summary>
        public static readonly TestResource TextEmbeddedResourceFile = new TestResource(TestResourceKind.EmbeddedResourceFile, ResourcePrefix + "embedded resource file.txt");

        private TestResource(TestResourceKind kind, string name, string resourceName = null, string expectedValue = null)
        {
            Kind = kind;
            Name = name;
            ResourceName = resourceName == null ? StringsResourceName : resourceName;
            if (expectedValue != null)
            {
                ExpectedValue = expectedValue;
            }
        }

        /// <summary>
        /// Gets the kind of the resource.
        /// </summary>
        public TestResourceKind Kind { get; private set; }

        /// <summary>
        /// Gets the relative name (key) of the resource in the INTV.TestHelpers.Shared assembly.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the name of the resource containing data at the key <see cref="Name"/>.
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Gets the expected value for a string resource.
        /// </summary>
        public string ExpectedValue { get; private set; }

        /// <summary>
        /// Gets the first level of expected content in an archive resource.
        /// </summary>
        public IEnumerable<string> ArchiveContents { get; private set; }

        /// <summary>
        /// Opens the resource for reading.
        /// </summary>
        /// <param name="typeForResource">The data type whose implementing assembly is checked for the given resource.</param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>A stream for reading the resource.</returns>
        public static Stream OpenExternalResourceForReading(Type typeForResource, string resourceName)
        {
            var assembly = typeForResource.Assembly;
            var resourceStream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources." + resourceName);
            return resourceStream;
        }

        /// <summary>
        /// Opens the resource for reading.
        /// </summary>
        /// <returns>A stream for reading the resource.</returns>
        public Stream OpenResourceForReading()
        {
            var assembly = this.GetType().Assembly;
            var resourceStream = assembly.GetManifestResourceStream(Name);
            return resourceStream;
        }

        /// <summary>
        /// Creates a disk copy of the resource in a temporary location. When returned object disposed, the temporary copy is deleted.
        /// </summary>
        /// <param name="resourceFilePath">Receives the path to the disk copy of the resource.</param>
        /// <returns>An <see cref="IDisposable"/> to remove the temporary file copy.</returns>
        public IDisposable ExtractToTemporaryFile(out string resourceFilePath)
        {
            var temporaryDirectory = new TemporaryDirectoryForResource();
            var resourceFileName = Name.Substring(TestResource.ResourcePrefix.Length);
            resourceFilePath = Path.Combine(temporaryDirectory.Path, resourceFileName);

            using (var resourceStream = OpenResourceForReading())
            using (var fileStream = new FileStream(resourceFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                resourceStream.CopyTo(fileStream);
            }

            return temporaryDirectory;
        }

        private sealed class TemporaryDirectoryForResource : IDisposable
        {
            /// <summary>
            /// Initialize a new instance of <paramref name="TemporaryDirectory"/>.
            /// </summary>
            public TemporaryDirectoryForResource()
            {
                Path = GenerateUniqueDirectoryPath();
                Directory.CreateDirectory(Path);
            }

            ~TemporaryDirectoryForResource()
            {
                Dispose(false);
            }

            /// <summary>
            /// Gets the absolute path to use for the temporary directory.
            /// </summary>
            public string Path { get; private set; }

            /// <summary>
            /// Generates a unique directory path.
            /// </summary>
            /// <returns>A unique directory path.</returns>
            public static string GenerateUniqueDirectoryPath()
            {
                var directoryPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "VINT_" + Guid.NewGuid());
                return directoryPath;
            }

            #region IDispose

            /// <inheritdoc />
            public void Dispose()
            {
                Dispose(true);
            }

            [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // The catch is a CYA that shouldn't be triggered. Rest is covered.
            private void Dispose(bool disposing)
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    if (Path.StartsWith(System.IO.Path.GetTempPath()))
                    {
                        try
                        {
                            if (Directory.Exists(Path))
                            {
                                Directory.Delete(Path, recursive: true);
                            }
                        }
                        catch
                        {
                        }
                    }
                    Path = null;
                }
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }

            #endregion // IDispose
        }
    }
}
