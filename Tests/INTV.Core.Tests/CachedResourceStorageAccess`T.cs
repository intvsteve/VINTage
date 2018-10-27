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

using System.Collections.Generic;
using System.IO;
using INTV.Core.Utility;

namespace INTV.Core.Tests
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
        public static void Initialize(string resourcePath, params string[] additionalResourcePaths)
        {
            var storageAccess = new T();
            StreamUtilities.Initialize(storageAccess);
            var assembly = typeof(T).Assembly;
            var resource = assembly.GetName().Name + resourcePath.Replace("/", ".");
            using (var resourceStream = assembly.GetManifestResourceStream(resource))
            {
                var stream = storageAccess.Open(resourcePath);
                resourceStream.CopyTo(stream);
                storageAccess._streamsCache.Add(stream);
            }

            if (additionalResourcePaths != null)
            {
                foreach (var additionalResourcePath in additionalResourcePaths)
                {
                    resource = assembly.GetName().Name + additionalResourcePath.Replace("/", ".");
                    using (var resourceStream = assembly.GetManifestResourceStream(resource))
                    {
                        var stream = storageAccess.Open(additionalResourcePath);
                        resourceStream.CopyTo(stream);
                        storageAccess._streamsCache.Add(stream);
                    }
                }
            }
        }
    }
}
