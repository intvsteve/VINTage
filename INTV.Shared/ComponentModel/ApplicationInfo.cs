// <copyright file="ApplicationInfo.cs" company="INTV Funhouse">
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
//

using System;
using System.Linq;
using INTV.Shared.Properties;
using INTV.Shared.Utility;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// A partial implementation of <see cref="IApplicationInfo"/>.
    /// </summary>
    public abstract class ApplicationInfo : IApplicationInfo
    {
        private static readonly Lazy<string> AppVersion = new Lazy<string>(InitializeVersion);
        private static readonly Lazy<string> AppCopyright = new Lazy<string>(InitializeCopyright);
        private static readonly Lazy<string> AppAuthor = new Lazy<string>(InitializeAuthor);

        /// <summary>
        /// Gets the application version using a standard approach (using assembly attribute data).
        /// </summary>
        public static string StandardVersion
        {
            get { return AppVersion.Value; }
        }

        /// <summary>
        /// Gets the application copyright using a standard approach (using assembly attribute data).
        /// </summary>
        public static string StandardCopyright
        {
            get { return AppCopyright.Value; }
        }

        /// <summary>
        /// Gets the application author(s) using a standard approach (using a custom assembly attribute).
        /// </summary>
        /// <remarks>The standard approach is not supported in Windows xp. In .NET 4.0, if you don't want a
        /// hard-coded author returned, don't use this property!</remarks>
        public static string StandardAuthor
        {
            get { return AppAuthor.Value; }
        }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public virtual string Version
        {
            get { return StandardVersion; }
        }

        /// <inheritdoc />
        public virtual string Copyright
        {
            get { return StandardCopyright; }
        }

        /// <inheritdoc />
        /// <remarks>In Windows xp (.NET 4.0) this is hard-coded - so you may wish to override this property!</remarks>
        public virtual string Author
        {
            get { return StandardAuthor; }
        }

        /// <inheritdoc />
        public abstract string DocumentFolderName { get; }

        /// <inheritdoc />
        public abstract OSVersion MinimumOSVersion { get; }

        /// <inheritdoc />
        public abstract OSVersion RecommendedOSVersion { get; }

        /// <inheritdoc />
        public abstract string ProductUrl { get; }

        /// <inheritdoc />
        public abstract string OnlineHelpUrl { get; }

        /// <inheritdoc />
        public abstract string VersionCheckUrl { get; }

        /// <inheritdoc />
        public abstract ISettings Settings { get; }

        private static string InitializeVersion()
        {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var versionString = System.Diagnostics.FileVersionInfo.GetVersionInfo(entryAssembly.Location).ProductVersion;
            return versionString;
        }

        private static string InitializeCopyright()
        {
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var copyright = entryAssembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false).OfType<System.Reflection.AssemblyCopyrightAttribute>().FirstOrDefault();
            return copyright.Copyright + " " + AppAuthor.Value;
        }

        private static string InitializeAuthor()
        {
            var author = string.Empty;
#if !WIN_XP
            var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var authorAttributes = entryAssembly.GetCustomAttributes(typeof(System.Reflection.AssemblyMetadataAttribute), false).Cast<System.Reflection.AssemblyMetadataAttribute>().Where(a => a.Key == INTV.Core.Utility.ResourceHelpers.AuthorKey);
            var authors = new System.Collections.Generic.HashSet<string>(authorAttributes.Select(a => a.Value), StringComparer.InvariantCultureIgnoreCase);
            author = string.Join(", ", authors);
#endif
            if (string.IsNullOrEmpty(author))
            {
                author = "Steven A. Orth";
            }
            return author;
        }
    }
}
