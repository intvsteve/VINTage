// <copyright file="ResourceHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Helper methods for retrieving resources from assemblies.
    /// </summary>
    public static partial class ResourceHelpers
    {
        /// <summary>
        /// Creates a properly formatted string to locate a resource in an assembly.
        /// </summary>
        /// <typeparam name="T">A type whose assembly is expected to contain a specific resource.</typeparam>
        /// <param name="type">Any object whose implementation is in the assembly in which the resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the resource within the type's assembly.</param>
        /// <returns>The packed resource string suitable to locate the resource.</returns>
        public static string CreatePackedResourceString<T>(this T type, string relativeResourcePath)
        {
            return CreatePackedResourceString(typeof(T), relativeResourcePath);
        }

        /// <summary>
        /// Loads an image resource from an assembly.
        /// </summary>
        /// <typeparam name="T">A type whose assembly is expected to contain a specific image resource.</typeparam>
        /// <param name="type">Any object whose implementation is in the assembly in which the image resource is supposed to exist.</param>
        /// <param name="relativeResourcePath">The relative path to the image resource within the type's assembly.</param>
        /// <returns>The image.</returns>
        /// <exception cref="IOException">Thrown if <paramref name="relativeResourcePath"/> refers to a resource that cannot be found.</exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="relativeResourcePath"/> refers to a resource that is not a valid image format.</exception>
        public static OSImage LoadImageResource<T>(this T type, string relativeResourcePath)
        {
            return LoadImageResource(typeof(T), relativeResourcePath);
        }

        /// <summary>
        /// Retrieves a string resource.
        /// </summary>
        /// <param name="type">A type that is in the assembly containing the desired string resource.</param>
        /// <param name="key">The name of the string resource.</param>
        /// <returns>The string, or <c>null</c> if not found. May also be a bogus string starting with !! if an exception occurs.</returns>
        /// <remarks>This function assumes that the string is in a resource that can be located in a namespace of [assemblyName].Resources.Strings.</remarks>
        public static string GetResourceString(this Type type, string key)
        {
            return type.GetResourceString("Strings", key);
        }

        /// <summary>
        /// Retrieves a string resource.
        /// </summary>
        /// <param name="type">A type that is in the assembly containing the desired string resource.</param>
        /// <param name="resourceName">The name of the specific resource containing the string.</param>
        /// <param name="key">The name of the string resource.</param>
        /// <returns>The string, or <c>null</c> if not found. May also be a bogus string starting with !! if an exception occurs.</returns>
        /// <remarks>This function assumes that the string is in a resource that can be located in a namespace of [assemblyName].Resources.resourceName.</remarks>
        public static string GetResourceString(this Type type, string resourceName, string key)
        {
            var value = "!!MISSING STRING!!";
            try
            {
                var assembly = type.Assembly;
                resourceName = assembly.GetName().Name + ".Resources." + resourceName;
                var resourceManager = new System.Resources.ResourceManager(resourceName, type.Assembly);
                value = resourceManager.GetString(key);
            }
            catch (InvalidOperationException)
            {
                value = "!!RESOURCE '" + resourceName + "." + key + " ' IS NOT A STRING!!";
            }
            catch (System.Resources.MissingManifestResourceException)
            {
                value = "!!NO RESOURCES FOUND FOR '" + resourceName + "." + key + "'!!";
            }
            return value;
        }

        /// <summary>
        /// Gets an enumerable of the resources whose names begin with the specified prefix.
        /// </summary>
        /// <param name="typeForLocatingResources">Type for locating resources. The resources must be in the assembly that defines this type.</param>
        /// <param name="resourceNameFilter">This prefix is used to identify resources. Only resources whose names start with exactly this prefix will match.</param>
        /// <returns>The resources with names that start with <paramref name="resourceNameFilter"/>.</returns>
        public static IEnumerable<string> GetResources(this Type typeForLocatingResources, string resourceNameFilter)
        {
            var resources = typeForLocatingResources.Assembly.GetManifestResourceNames().Where(r => r.StartsWith(resourceNameFilter, StringComparison.InvariantCulture));
            return resources;
        }

        /// <summary>
        /// Extracts the resources with the given names into files on disk.
        /// </summary>
        /// <param name="typeForLocatingResources">Type for locating resources. The resources must be in the assembly that defines this type.</param>
        /// <param name="resourcesToExtract">Names of the resources to extract.</param>
        /// <param name="resourcePrefix">Resource prefix to strip to create a file name. Text remaining after this prefix will be used as the file name</param>
        /// <param name="destinationDirectory">Destination directory in which to create files containing the resource data.</param>
        /// <param name="getFileNameForResource">An optional delegate to call to get the file name to use for the resource to extract to a file.</param>
        /// <returns>An enumerable of absolute paths to the files containing the given resources that were extracted.</returns>
        public static IEnumerable<string> ExtractResourcesToFiles(this Type typeForLocatingResources, IEnumerable<string> resourcesToExtract, string resourcePrefix, string destinationDirectory, Func<string, Stream, string> getFileNameForResource = null)
        {
            var extractedResourceFiles = new List<string>();
            if (resourcesToExtract.Any())
            {
                if (System.IO.Directory.CreateDirectory(destinationDirectory).Exists)
                {
                    var assembly = typeForLocatingResources.Assembly;
                    foreach (var resource in resourcesToExtract)
                    {
                        using (var resourceStream = assembly.GetManifestResourceStream(resource))
                        {
                            var fileName = getFileNameForResource == null ? resource.Substring(resourcePrefix.Length) : getFileNameForResource(resource, resourceStream);
                            var filePath = System.IO.Path.Combine(destinationDirectory, fileName);
                            if (!System.IO.File.Exists(filePath))
                            {
                                using (var fileStream = System.IO.File.Create(filePath))
                                {
                                    resourceStream.Seek(0, SeekOrigin.Begin);
                                    resourceStream.CopyTo(fileStream);
                                }
                            }
                            extractedResourceFiles.Add(filePath);
                        }
                    }
                }
            }
            return extractedResourceFiles;
        }
    }
}
