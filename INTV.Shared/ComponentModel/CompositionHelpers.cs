// <copyright file="CompositionHelpers.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Provides helper methods for working with composition (MEF).
    /// </summary>
    public static class CompositionHelpers
    {
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
                    
                    // Add all the parts discovered in the assemblies in the same directory as the application.
                    var directoryCatalog = new DirectoryCatalog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
                    catalog.Catalogs.Add(directoryCatalog);
                    
                    // Create the CompositionContainer with the parts in the catalog.
                    _container = new CompositionContainer(catalog);
                }
                return _container;
            }
        }
        private static CompositionContainer _container;

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
                var errorDialog = INTV.Shared.View.ReportDialog.Create("Composition Error", null);
                errorDialog.Exception = e;
                errorDialog.ShowDialog();
            }
        }
    }
}
