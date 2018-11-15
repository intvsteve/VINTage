// <copyright file="CachedResourceStorageAccess`T.cs" company="INTV Funhouse">
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
using System.Globalization;
using System.IO;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Core.Utility;

namespace INTV.TestHelpers.Core.Utility
{
    /// <summary>
    /// Specialized implementation of <see cref="TestStorageAccess"/> for implementing various ROM format file tests.
    /// </summary>
    /// <typeparam name="T">The specific specialization of a storage for a particular testing scenario.</typeparam>
    public class CachedResourceStorageAccess<T> : TestStorageAccess where T : CachedResourceStorageAccess<T>, new()
    {
        private readonly List<Stream> _streamsCache = new List<Stream>();
        private T _self;

        public static IReadOnlyList<string> InitializeStorageWithCopiesOfResources(string resourcePath, params string[] additionalResourcePaths)
        {
            IReadOnlyList<string> copiedResourcePaths;
            Initialize(out copiedResourcePaths, resourcePath, additionalResourcePaths);
            return copiedResourcePaths;
        }

        /// <summary>
        /// Initializes the storage manager and registers it for use, caching the given resources as files and creating copies for use by tests.
        /// </summary>
        /// <param name="copiedResourcePaths">Receives the temporary path copies for the corresponding resources</param>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="additionalResourcePaths">Additional resource paths to register with the storage access and subsequently cache.</param>
        /// <returns>The instance of the storage access that was created.</returns>
        /// <remarks>This assumes the resources are located in the same assembly as the type <see cref="INTV.TestHelpers.Core.Utility.TestRomResources"/>.</remarks>
        public static T Initialize(out IReadOnlyList<string> copiedResourcePaths, string resourcePath, params string[] additionalResourcePaths)
        {
            return Initialize(out copiedResourcePaths, null, resourcePath, additionalResourcePaths);
        }

        /// <summary>
        /// Initializes the storage manager and registers it for use, caching the given resources as files and creating copies for use by tests.
        /// </summary>
        /// <param name="copiedResourcePaths">Receives the temporary path copies for the corresponding resources</param>
        /// <param name="typeForLocatingResources">If not <c>null</c>, the assembly containing the given type will be used to
        /// locate the resources named by the <paramref name="resourcePath"/> and <paramref name="additionalResourcePaths"/> arguments.
        /// Otherwise, the assembly that implements the type <see cref="INTV.TestHelpers.Core.Utility.TestRomResources"/> will be used
        /// to locate the resources.</param>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="additionalResourcePaths">Additional resource paths to register with the storage access and subsequently cache.</param>
        /// <returns>The instance of the storage access that was created.</returns>
        /// <remarks>This assumes the resources are located in the same assembly as the type <see cref="INTV.TestHelpers.Core.Utility.TestRomResources"/>.</remarks>
        public static T Initialize(out IReadOnlyList<string> copiedResourcePaths, Type typeForLocatingResources, string resourcePath, params string[] additionalResourcePaths)
        {
            var storageAccess = Initialize(resourcePath, additionalResourcePaths).WithStockCfgResources();

            var fileExtension = Path.GetExtension(resourcePath);
            var randomFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            var directory = Path.GetDirectoryName(resourcePath);
            var randomPath = Path.Combine(directory, Path.ChangeExtension(randomFileName, fileExtension));

            storageAccess.CreateCopyOfResource(resourcePath, randomPath);
            var copiedPaths = new List<string>() { randomPath };
            copiedResourcePaths = copiedPaths;

            if (additionalResourcePaths != null)
            {
                foreach (var additionalResourcePath in additionalResourcePaths.Where(p => !string.IsNullOrEmpty(p)))
                {
                    fileExtension = Path.GetExtension(additionalResourcePath);
                    randomFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                    directory = Path.GetDirectoryName(additionalResourcePath);
                    randomPath = Path.Combine(directory, Path.ChangeExtension(randomFileName, fileExtension));

                    storageAccess.CreateCopyOfResource(additionalResourcePath, randomPath);
                    copiedPaths.Add(randomPath);
                }
            }

            return storageAccess;
        }

        /// <summary>
        /// Initializes the storage manager and registers it for use, caching the given resources as files.
        /// </summary>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="additionalResourcePaths">Additional resource paths to register with the storage access and subsequently cache.</param>
        /// <returns>The instance of the storage access that was created.</returns>
        /// <remarks>This assumes the resources are located in the same assembly as the type <see cref="INTV.TestHelpers.Core.Utility.TestRomResources"/>.</remarks>
        public static T Initialize(string resourcePath, params string[] additionalResourcePaths)
        {
            return Initialize(null, resourcePath, additionalResourcePaths);
        }

        /// <summary>
        /// Initializes the storage manager and registers it for use, caching the given resources as files.
        /// </summary>
        /// <param name="typeForLocatingResources">If not <c>null</c>, the assembly containing the given type will be used to
        /// locate the resources named by the <paramref name="resourcePath"/> and <paramref name="additionalResourcePaths"/> arguments.
        /// Otherwise, the assembly that implements the type <see cref="INTV.TestHelpers.Core.Utility.TestRomResources"/> will be used
        /// to locate the resources.</param>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="additionalResourcePaths">Additional resource paths to register with the storage access and subsequently cache.</param>
        /// <returns>The instance of the storage access that was created.</returns>
        public static T Initialize(Type typeForLocatingResources, string resourcePath, params string[] additionalResourcePaths)
        {
            var storageAccess = new T();
            storageAccess._self = storageAccess;
            StreamUtilities.Initialize(storageAccess);
            storageAccess.AddCachedResource(resourcePath, resourcePath, typeForLocatingResources);

            if (additionalResourcePaths != null)
            {
                foreach (var additionalResourcePath in additionalResourcePaths)
                {
                    storageAccess.AddCachedResource(additionalResourcePath, additionalResourcePath, typeForLocatingResources);
                }
            }
            return storageAccess;
        }

        /// <summary>
        /// Create a copy of a resource, appending additional data to it.
        /// </summary>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="newPath">The new path to use for the modified copy.</param>
        /// <param name="typeForLocatingResource">If provided, the assembly containing the given type will be used to locate the
        /// resource named by the <paramref name="resourcePath"/> argument. Otherwise, the assembly that implements the type
        /// <see cref="INTV.TestHelpers.Core.Utility.TestRomResources"/> will be used to locate the resource.</param>
        public void CreateCopyOfResource(string resourcePath, string newPath, Type typeForLocatingResource = null)
        {
            AddCachedResource(resourcePath, newPath, typeForLocatingResource);
        }

        /// <summary>
        /// Registers the tools location.
        /// </summary>
        /// <param name="toolsFilesLocation">Receives the tools location directory.</param>
        /// <returns>This instance.</returns>
        public T WithDefaultToolsDirectory(out string toolsFilesLocation)
        {
            toolsFilesLocation = null;
            var codeBase = typeof(IRomHelpers).Assembly.CodeBase;
            var toolsFilesLocationUriPath = Path.Combine(Path.GetDirectoryName(codeBase), "tools\\");
            Uri toolsFilesLocationUri;
            if (Uri.TryCreate(toolsFilesLocationUriPath, UriKind.Absolute, out toolsFilesLocationUri))
            {
                toolsFilesLocation = Uri.UnescapeDataString(toolsFilesLocationUri.AbsolutePath); // Need to unescape spaces.
                IRomHelpers.SetConfigurationEntry(IRomHelpers.DefaultToolsDirectoryKey, toolsFilesLocation);
            }
            else
            {
                throw new InvalidOperationException();
            }
            return _self;
        }

        /// <summary>
        /// Registers the stock config files deployed with INTV.Core to be accessible via test code.
        /// </summary>
        /// <param name="stockCfgFileNumbersToExclude">If non-empty, indicates stock CFG files to leave out of the storage.</param>
        /// <returns>This instance.</returns>
        public T WithStockCfgResources(IEnumerable<int> stockCfgFileNumbersToExclude = null)
        {
            string toolsFilesLocation;
            WithDefaultToolsDirectory(out toolsFilesLocation);
            var stockCfgFilesToExclude = Enumerable.Empty<string>();
            if (stockCfgFileNumbersToExclude != null)
            {
                stockCfgFilesToExclude = stockCfgFileNumbersToExclude.Select(n => Path.ChangeExtension(n.ToString(CultureInfo.InvariantCulture), ProgramFileKind.CfgFile.FileExtension()));
            }
            foreach (var toolsFile in Directory.EnumerateFiles(toolsFilesLocation))
            {
                var exclude = stockCfgFilesToExclude.Contains(Path.GetFileName(toolsFile));
                if (exclude)
                {
                    if (Exists(toolsFile))
                    {
                        Rename(toolsFile, toolsFile + ".ded");
                    }
                    var toolsFileCopy = toolsFile.Replace('/', '\\');
                    if (Exists(toolsFileCopy))
                    {
                        Rename(toolsFileCopy, toolsFileCopy + ".ded");
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(toolsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        // register twice so we don't need to care about file separators.
                        var stream = Open(toolsFile);
                        fileStream.CopyTo(stream);
                        _streamsCache.Add(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        var toolsFileCopy = toolsFile.Replace('/', '\\');
                        stream = Open(toolsFileCopy);
                        fileStream.Seek(0, SeekOrigin.Begin);
                        fileStream.CopyTo(stream);
                        _streamsCache.Add(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }
            return _self;
        }

        private void AddCachedResource(string resourcePath, string destinationPath, Type typeForLocatingResource)
        {
            if (!string.IsNullOrEmpty(resourcePath))
            {
                if (typeForLocatingResource == null)
                {
                    typeForLocatingResource = typeof(TestRomResources);
                }
                var assembly = typeForLocatingResource.Assembly;
                var resource = assembly.GetName().Name + resourcePath.Replace("/", ".");
                using (var resourceStream = assembly.GetManifestResourceStream(resource))
                {
                    var stream = Open(destinationPath);
                    resourceStream.CopyTo(stream);
                    _streamsCache.Add(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
        }
    }
}
