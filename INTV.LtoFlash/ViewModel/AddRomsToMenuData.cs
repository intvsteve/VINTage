// <copyright file="AddRomsToMenuData.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.Utility;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Data structure for an asynchronous task to add ROMs to a menu layout.
    /// </summary>
    internal class AddRomsToMenuData : INTV.Shared.Utility.AsyncTaskData
    {
        /// <summary>
        /// Initializes a new instance of AddRomsToMenuData for adding multiple items to a single destination.
        /// </summary>
        /// <param name="menuLayout">The target menu layout.</param>
        /// <param name="destination">The destination directory within <paramref name="menuLayout"/>.</param>
        /// <param name="itemsToAdd">The items to add to <paramref name="destination"/>.</param>
        /// <param name="insertLocation">The insertion point within <paramref name="destination"/> at which to insert <paramref name="itemsToAdd"/>.
        /// A value if <c>-1</c> indicates at the end of the existing items list.</param>
        public AddRomsToMenuData(MenuLayoutViewModel menuLayout, IFileContainer destination, IEnumerable<ProgramDescription> itemsToAdd, int insertLocation)
            : this(menuLayout, destination, insertLocation, itemsToAdd, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of AddRomsToMenuData for adding multiple items each to its own destination.
        /// </summary>
        /// <param name="menuLayout">The target menu layout.</param>
        /// <param name="destinationDirectories">Paths of destination directories (relative to <paramref name="menuLayout"/>'s root) for each item in <paramref name="itemsToAdd"/>.</param>
        /// <param name="itemsToAdd">The items to add to the directories described by <paramref name="destinationDirectories"/>.</param>
        /// <remarks>The operation must create necessary directories if they do not exist.</remarks>
        public AddRomsToMenuData(MenuLayoutViewModel menuLayout, IEnumerable<string> destinationDirectories, IEnumerable<ProgramDescription> itemsToAdd)
            : this(menuLayout, null, -1, itemsToAdd, destinationDirectories)
        {
        }

        private AddRomsToMenuData(MenuLayoutViewModel menuLayout, IFileContainer destination, int insertLocation, IEnumerable<ProgramDescription> itemsToAdd, IEnumerable<string> destinationDirectories)
            : base(null)
        {
            MenuLayout = menuLayout;
            Destination = destination;
            ItemsToAdd = new List<ProgramDescription>(itemsToAdd);
            InsertLocation = insertLocation;
            DestinationDirectories = destinationDirectories;
            FailedToAdd = new List<System.Tuple<string, string>>();
            AddedItems = new List<FileNode>();
            UIDispatcher = OSDispatcher.Current;
        }

        /// <summary>
        /// Gets the ViewModel for the <see cref="MenuLayoutViewModel"/> operation.
        /// </summary>
        public MenuLayoutViewModel MenuLayout { get; private set; }

        /// <summary>
        /// Gets the destination directory for the items to add. If <c>null</c>, the DestinationDirectories property must be used.
        /// </summary>
        public IFileContainer Destination { get; private set; }

        /// <summary>
        /// Gets an enumerable containing the items to add to the menu layout.
        /// </summary>
        public IEnumerable<ProgramDescription> ItemsToAdd { get; private set; }

        /// <summary>
        /// Gets the insert location for the items to add if the destination directory is not <c>null</c>. If this value
        /// is negative, insertion is after any existing items in the destination.
        /// </summary>
        public int InsertLocation { get; private set; }

        /// <summary>
        /// Gets an enumerable of strings describing the destination directories for the items to add. If this value
        /// is <c>null</c>, then the items are to be added to the destination directory.
        /// </summary>
        public IEnumerable<string> DestinationDirectories { get; private set; }

        /// <summary>
        /// Gets a list of ROMs that could not be added to the menu layout, with an explanation as to why they could not be added.
        /// </summary>
        public IList<System.Tuple<string, string>> FailedToAdd { get; private set; }

        /// <summary>
        /// Gets a list of the items that were added.
        /// </summary>
        public IList<FileNode> AddedItems { get; private set; }

        /// <summary>
        /// Gets or sets the first error encountered while preparing files for adding to the menu.
        /// </summary>
        public System.Exception FirstFilePreparationError { get; set; }

        /// <summary>
        /// Gets the user interface dispatcher to use in case the operation must message the user interface.
        /// </summary>
        public OSDispatcher UIDispatcher { get; private set; }
    }
}
