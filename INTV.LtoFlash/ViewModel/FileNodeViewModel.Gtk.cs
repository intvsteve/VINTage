// <copyright file="FileNodeViewModel.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class FileNodeViewModel
    {
        /// <summary>
        /// Gets a Gtk.TreeIter for the node.
        /// </summary>
        /// <param name="iter">Receives the iterator to the node within the Gtk.TreeStore.</param>
        /// <param name="treeStore">The tree store containing reference to this node.</param>
        /// <returns><c>true</c>, if iter was successfully retrieved, or this node is the root, <c>false</c> otherwise.</returns>
        /// <remarks>If this is the root node, <paramref name="iter"/> contains Gtk.TreeIter.Zero. Caller must check for this.</remarks>
        internal bool GetIterForItem(out Gtk.TreeIter iter, Gtk.TreeStore treeStore)
        {
            iter = Gtk.TreeIter.Zero;
            var path = new Gtk.TreePath();
            var node = Model;
            var parent = Parent;
            while (parent != null)
            {
                var index = parent.IndexOfChild(node);
                path.PrependIndex(index);
                node = parent;
                parent = node.Parent;
            }

            var succeeded = (path.Depth == 0) || treeStore.GetIter(out iter, path);
            return succeeded;
        }
    }
}
