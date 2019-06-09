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
        /// <returns></returns>
        public Stream OpenResourceForReading()
        {
            var assembly = this.GetType().Assembly;
            var resourceStream = assembly.GetManifestResourceStream(Name);
            return resourceStream;
        }
    }
}
