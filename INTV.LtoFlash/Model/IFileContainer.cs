// <copyright file="IFileContainer.cs" company="INTV Funhouse">
// Copyright (c) 2014-2016 All Rights Reserved
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

namespace INTV.LtoFlash.Model
{
    /// <summary>
    /// This interface describes a container of files for purposes of manipulating a menu layout to be deployed to a Locutus device.
    /// </summary>
    public interface IFileContainer : IFile
    {
        #region Properties

        /// <summary>
        /// Gets the collection of items stored within the IFileContainer.
        /// </summary>
        System.Collections.Generic.IEnumerable<IFile> Items { get; }

        /// <summary>
        /// Gets the number of items stored in the IFileContainer.
        /// </summary>
        int Size { get; }

        #endregion // Properties

        #region Events

        /// <summary>
        /// This event is raised when the contents of the file container is changed.
        /// </summary>
        event EventHandler<EventArgs> ContentsChanged;

        #endregion // Events

        #region Methods

        /// <summary>
        /// Adds a child item at the end of the container's items list.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <param name="updateFileSystemTables">If <c>true</c>, update the underlying file system tables as well.</param>
        /// <returns><c>true</c> if the child was added, otherwise <c>false</c>.</returns>
        /// <remarks>If the child already has a parent, it will be removed from it. When updateFileSystemTables is <c>true</c>, the
        /// intent is to manipulate the underlying Locutus File System tables. For operations such as reparenting, this value should
        /// be <c>false</c>, while for the addition of a newly created child, it should be <c>true</c>.</remarks>
        bool AddChild(IFile child, bool updateFileSystemTables);

        /// <summary>
        /// Inserts the child into the collection at the given location.
        /// </summary>
        /// <param name="index">The index at which to insert the child.</param>
        /// <param name="child">The child to insert.</param>
        /// <param name="updateFileSystemTables">If <c>true</c>, update the underlying file system tables as well.</param>
        /// <remarks>If the child has a parent, it is first removed from that parent before being inserted into this one.</remarks>
        void InsertChild(int index, IFile child, bool updateFileSystemTables);

        /// <summary>
        /// Moves the child to a new parent.
        /// </summary>
        /// <param name="child">The child to move.</param>
        /// <param name="newParent">The new parent.</param>
        /// <param name="updateFileSystemTables">If <c>true</c>, update the underlying file system tables as well.</param>
        /// <returns><c>true</c> if the child was moved, otherwise <c>false</c>.</returns>
        /// <remarks>If the child already has a parent, it will be removed from it. The child will be
        /// placed at the end of the items in its new parent.</remarks>
        bool MoveChildToNewParent(IFile child, IFileContainer newParent, bool updateFileSystemTables);

        /// <summary>
        /// Moves the child to a new parent.
        /// </summary>
        /// <param name="child">The child to move.</param>
        /// <param name="newParent">The new parent.</param>
        /// <param name="locationInNewParent">Location (index) in new parent.</param>
        /// <param name="updateFileSystemTables">If <c>true</c>, update the underlying file system tables as well.</param>
        /// <returns><c>true</c> if the child was moved, otherwise <c>false</c>.</returns>
        /// <remarks>If the child already has a parent, it will be removed from it.</remarks>
        bool MoveChildToNewParent(IFile child, IFileContainer newParent, int locationInNewParent, bool updateFileSystemTables);

        /// <summary>
        /// Removes a child item from the container's items list.
        /// </summary>
        /// <param name="child">The child to remove.</param>
        /// <param name="updateFileSystemTables">If <c>true</c>, update the underlying file system tables as well.</param>
        /// <returns><c>true</c> if the child was removed, otherwise <c>false</c>.</returns>
        bool RemoveChild(IFile child, bool updateFileSystemTables);

        /// <summary>
        /// Recursively removes all items matching the given filter from the file container.
        /// </summary>
        /// <param name="filter">The filter to use to determine whether the item should be removed from the hierarchy.</param>
        void RemoveChildFromHierarchy(Predicate<IFile> filter);

        /// <summary>
        /// Determines whether the IFileContainer contains a specific child.
        /// </summary>
        /// <param name="child">The child item to locate.</param>
        /// <returns><c>true</c> if the child item is contained in the IFileContainer's Items, <c>false</c> otherwise.</returns>
        /// <remarks>This does not perform a recursive check. To determine if the child item exists anywhere in the container's
        /// hierarchy, use ContainsChild.</remarks>
        bool IsParentOfChild(IFile child);

        /// <summary>
        /// Determines whether the IFileContainer contains a specific child anywhere in its contained hierarchy of items.
        /// </summary>
        /// <param name="child">The child item to locate.</param>
        /// <returns><c>true</c> if the item is contained by any element in the IFileContainer's hierarchy.</returns>
        bool ContainsChild(IFile child);

        /// <summary>
        /// Searches the collection for the given child, returning its index.
        /// </summary>
        /// <param name="child">The child item to locate.</param>
        /// <returns>Index of the child in the collection, or <c>-1</c> if not found.</returns>
        int IndexOfChild(IFile child);

        /// <summary>
        /// Searches the collection for children matching the given filter.
        /// </summary>
        /// <param name="filter">A predicate that must return <c>true</c> to include a child in the result. Cannot be <c>null</c>.</param>
        /// <param name="recurse">If <c>true</c>, child elements will also be included in the result. Otherwise, only immediate contents of the container are considered.</param>
        /// <returns>An enumeration of the child objects meeting the condition defined by <paramref name="filter"/>.</returns>
        IEnumerable<IFile> FindChildren(Predicate<IFile> filter, bool recurse);

        #endregion // Methods
    }
}
