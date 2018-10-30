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
using System.IO;
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

        /// <summary>
        /// Initializes the storage manager and registers it for use, caching the given resources as files.
        /// </summary>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="additionalResourcePaths">Additional resource paths to register with the storage access and subsequently cache.</param>
        /// <returns>The instance of the storage access that was created.</returns>
        /// <remarks>This assumes the resources are located in the same assembly as the type parameter <typeparamref name="T"/>.</remarks>
        public static T Initialize(string resourcePath, params string[] additionalResourcePaths)
        {
            return Initialize(null, resourcePath, additionalResourcePaths);
        }

        /// <summary>
        /// Initializes the storage manager and registers it for use, caching the given resources as files.
        /// </summary>
        /// <param name="typeForLocatingResources">If not <c>null</c>, the assembly containing the given type will be used to
        /// locate the resources named by the <paramref name="resourcePath"/> and <paramref name="additionalResourcePaths"/> arguments.
        /// Otherwise, the assembly that implements the type specified by the type parameter <typeparamref name="T"/> will be used
        /// to locate the resources.</param>
        /// <param name="resourcePath">A resource path to register with the storage access and subsequently cache.</param>
        /// <param name="additionalResourcePaths">Additional resource paths to register with the storage access and subsequently cache.</param>
        /// <returns>The instance of the storage access that was created.</returns>
        public static T Initialize(Type typeForLocatingResources, string resourcePath, params string[] additionalResourcePaths)
        {
            var storageAccess = new T();
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
        /// specified by the type parameter <typeparamref name="T"/> will be used to locate the resource.</param>
        public void CreateCopyOfResource(string resourcePath, string newPath, Type typeForLocatingResource = null)
        {
            AddCachedResource(resourcePath, newPath, typeForLocatingResource);
        }

        private void AddCachedResource(string resourcePath, string destinationPath, Type typeForLocatingResource)
        {
            if (!string.IsNullOrEmpty(resourcePath))
            {
                if (typeForLocatingResource == null)
                {
                    typeForLocatingResource = typeof(T);
                }
                var assembly = typeForLocatingResource.Assembly;
                var resource = assembly.GetName().Name + resourcePath.Replace("/", ".");
                using (var resourceStream = assembly.GetManifestResourceStream(resource))
                {
                    var stream = Open(destinationPath);
                    resourceStream.CopyTo(stream);
                    _streamsCache.Add(stream);
                }
            }
        }
    }
}
