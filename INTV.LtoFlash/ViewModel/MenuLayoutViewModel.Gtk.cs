// <copyright file="MenuLayoutViewModel.Gtk.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using INTV.LtoFlash.Model;
using INTV.Shared.View;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class MenuLayoutViewModel
    {
        /// <summary>
        /// Pushes contents of the ViewModel into a Gtk.TreeStore.
        /// </summary>
        /// <param name="treeStore">The Gtk.TreeStore that tracks our model.</param>
        internal void SynchronizeToTreeStore(Gtk.TreeStore treeStore)
        {
            InitializeGtkModel(treeStore);
            InsertChildren(treeStore, Items, Gtk.TreeIter.Zero);
        }

        /// <summary>
        /// Inserts the given child FileNodeViewModels into the given Gtk.TreeStore using the supplied Gtk.TreeIter as the point of insertion.
        /// </summary>
        /// <param name="treeStore">The Gtk.TreeStore to add child objects to.</param>
        /// <param name="children">The child ViewModels.</param>
        /// <param name="iter">The Gtk.TreeIter of the parent to which children are added.</param>
        protected static void InsertChildren(Gtk.TreeStore treeStore, ObservableCollection<FileNodeViewModel> children, Gtk.TreeIter iter)
        {
            foreach (var child in children)
            {
                Gtk.TreeIter childIter;
                if (iter.Equals(Gtk.TreeIter.Zero))
                {
                    childIter = treeStore.AppendValues(child);
                }
                else
                {
                    childIter = treeStore.AppendValues(iter, child);
                }
                var folder = child as FolderViewModel;
                if (folder != null)
                {
                    // Ensure the child folder has properly initialized Gtk.TreeStore.
                    folder.InitializeGtkModel(treeStore);
                    InsertChildren(treeStore, folder.Items, childIter);
                }
            }
        }

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        partial void OSFinishedUpdatingItemStates()
        {
            // HACK: Ensure visual redraws to notice updated icons. Perhaps the Renderer should
            // somehow be instructed that the value has changed?
            var visual = LtoFlashViewModel.GetVisuals().First().Item2.NativeVisual;
            var menuLayoutVisual = visual.FindChild<Gtk.TreeView>();
            menuLayoutVisual.QueueDraw();
        }
    }
}
