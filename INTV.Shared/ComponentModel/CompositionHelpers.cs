// <copyright file="CompositionHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2018 All Rights Reserved
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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Provides helper methods for working with composition (MEF).
    /// </summary>
    public static partial class CompositionHelpers
    {
        /// <summary>
        /// The name to use to indicate whether a ComposablePartDefinition should be discovered only once.
        /// The value of this metadata must be a string containing one or more contract names that should
        /// be considered for discovery only once. If more than one contract name is specified by the value,
        /// separate the names using a comma.
        /// </summary>
        public const string DiscoverOneTimeOnlyMetadataName = "DiscoverOneTimeOnly";

        /// <summary>
        /// Gets the composition container to use for MEF.
        /// </summary>
        public static CompositionContainer Container
        {
            get
            {
                if (_container == null)
                {
                    // Create the catalog that aggregates everywhere we care to look.
                    var catalog = new AggregateCatalog();

                    // Create the CompositionContainer with the parts in the catalog.
                    var filterCatalog = new FilterCatalog(catalog);

                    // Add all the parts discovered in the assemblies in the same directory as the application.
                    var defaultPartsDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    var directoryCatalog = new DirectoryCatalog(System.IO.Path.GetDirectoryName(defaultPartsDirectory));
                    PreflightPluginsCheck(directoryCatalog, filterCatalog, false);

                    // Also, support loading parts from the app's plugins directory, if it exists.
                    var app = INTV.Shared.Utility.SingleInstanceApplication.Instance;
                    var appPluginsDir = app == null ? null : app.PluginsLocation;
                    if (!string.IsNullOrEmpty(appPluginsDir) && System.IO.Directory.Exists(appPluginsDir))
                    {
                        directoryCatalog = new DirectoryCatalog(appPluginsDir, "*.dll");
                        PreflightPluginsCheck(directoryCatalog, filterCatalog, true);
                    }

                    _container = new CompositionContainer(filterCatalog);
                }
                return _container;
            }
        }

        private static CompositionContainer _container;

        /// <summary>
        /// Gets or sets a value indicating whether to suppress the import error dialog.
        /// </summary>
        public static bool SuppressImportErrorDialog { get; set; }

        /// <summary>
        /// Checks the assemblies in the given directory catalog, excluding any that may have bad implementations of interfaces being imported.
        /// </summary>
        /// <param name="catalogToCheck">The directory catalog whose plugins are to be pre-flight checked.</param>
        /// <param name="filterCatalog">The catalog used to filter the valid plugins.</param>
        /// <param name="plugins">If <c>true</c>, indicates we're running the preflight check on externally loaded plugins, not
        /// those shipped with the application.</param>
        /// <remarks>From a helpful article at - you guessed it - StackOverflow:
        /// http://stackoverflow.com/questions/4020532/mef-unable-to-load-one-or-more-of-the-requested-types-retrieve-the-loaderexce
        /// </remarks>
        private static void PreflightPluginsCheck(DirectoryCatalog catalogToCheck, FilterCatalog filterCatalog, bool plugins)
        {
            var pluginsToCheck = System.IO.Directory.EnumerateFiles(catalogToCheck.FullPath, "*.dll");
            foreach (var pluginToCheck in pluginsToCheck)
            {
                try
                {
                    var assembly = System.Reflection.Assembly.LoadFile(pluginToCheck);
                    var assemblyCatalog = new AssemblyCatalog(assembly);
                    var parts = catalogToCheck.Parts.ToArray(); // this may cause a ReflectionTypeLoadException if the plugin is bad
                    if (plugins)
                    {
                        OSInitializeParts(assembly, parts);
                    }
                    filterCatalog.SourceCatalog.Catalogs.Add(assemblyCatalog);
                }
                catch (System.Reflection.ReflectionTypeLoadException e)
                {
                    System.Diagnostics.Debug.WriteLine("Bad assembly: " + e);
                }
                catch (System.Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Error checking assembly: " + e);
                }
            }
        }

        /// <summary>
        /// Import contracts.
        /// </summary>
        /// <param name="importer">The entity for which composition is going to occur.</param>
        public static void DoImport(this object importer)
        {
            try
            {
                Container.ComposeParts(importer);
            }
            catch (System.Exception e)
            {
                if (!SuppressImportErrorDialog)
                {
                    var errorDialog = INTV.Shared.View.ReportDialog.Create("Composition Error", null);
                    errorDialog.Exception = e;
                    errorDialog.ShowDialog();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Filter catalog to assist with excluding imports we don't want.
        /// </summary>
        /// <remarks>Based on the handy tutorial from PluralSight here:
        /// https://www.youtube.com/watch?v=bP9LCccByzA
        /// The primary purpose is to prevent multiple copies of the same parts from being found in situations such as
        /// if a part is somehow present in both the application directory as well as the plugins directory. In such
        /// a case, the first part discovered is kept, while the later one will be ignored.</remarks>
        private class FilterCatalog : ComposablePartCatalog
        {
            private Dictionary<string, ComposablePartDefinition> _foundUniqueContractParts;
            private HashSet<string> _discoverOncePartDefinitions;

            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="INTV.Shared.ComponentModel.CompositionHelpers+FilterCatalog"/> class.
            /// </summary>
            /// <param name="sourceCatalog">Source catalog to filter.</param>
            internal FilterCatalog(AggregateCatalog sourceCatalog)
            {
                _foundUniqueContractParts = new Dictionary<string, ComposablePartDefinition>();
                SourceCatalog = sourceCatalog;
                _discoverOncePartDefinitions = new HashSet<string>();
            }

            internal AggregateCatalog SourceCatalog { get; private set; }

            /// <inheritdoc/>
            public override IQueryable<ComposablePartDefinition> Parts
            {
                get { return FilterParts(); }
            }

            private IQueryable<ComposablePartDefinition> FilterParts()
            {
                var parts = SourceCatalog.Parts.Where(p => ShouldKeepPart(p));
                return parts;
            }

            /// <summary>
            /// This predicate is used to prevent multiple instances of discoverable parts from being found.
            /// </summary>
            /// <param name="part">The part to check.</param>
            /// <returns><c>true</c>, if keep part should be loaded, <c>false</c> otherwise.</returns>
            private bool ShouldKeepPart(ComposablePartDefinition part)
            {
                // Check the part definition to see if we should only keep one instance if multiple are found. If it is one
                // of these "special" parts, add it to the "discover once" list.
                var keepPart = true;
                var metadata = part.Metadata;
                object discoverOnlyOnce;
                if ((metadata != null) && metadata.TryGetValue(DiscoverOneTimeOnlyMetadataName, out discoverOnlyOnce))
                {
                    var discoverOnceTypes = discoverOnlyOnce as string;
                    if (!string.IsNullOrEmpty(discoverOnceTypes))
                    {
                        foreach (var discoverOnceType in discoverOnceTypes.Split(','))
                        {
                            _discoverOncePartDefinitions.Add(discoverOnceType.Trim());
                        }
                    }
                }

                // If this part meets a "discover only once" contract, check to see if it's one we've already
                // found. If it is, only keep the first one found.
                var forceUniqueContracts = part.ExportDefinitions.Select(e => e.ContractName).Where(c => _discoverOncePartDefinitions.Contains(c));
                foreach (var forceUniqueContract in forceUniqueContracts)
                {
                    ComposablePartDefinition keptPart;
                    if (_foundUniqueContractParts.TryGetValue(forceUniqueContract, out keptPart))
                    {
                        keepPart = object.ReferenceEquals(keptPart, part);
                    }
                    else
                    {
                        _foundUniqueContractParts[forceUniqueContract] = part;
                    }
                }
                return keepPart;
            }
        }

        /// <summary>
        /// Peform operationg-system-specific initialization of composition parts.
        /// </summary>
        /// <param name="assembly">The assembly that may contain parts for which custom initializtion may be needed.</param>
        /// <param name="parts">The composable parts, potentially needed for custom initialization.</param>
        static partial void OSInitializeParts(System.Reflection.Assembly assembly, ComposablePartDefinition[] parts);
    }
}
