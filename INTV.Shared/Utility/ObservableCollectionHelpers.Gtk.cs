// <copyright file="ObservableCollectionHelpers.Gtk.cs" company="INTV Funhouse">
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

////#define ENABLE_DIAGNOSTIC_OUTPUT

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace INTV.Shared.Utility
{
    public static partial class ObservableCollectionHelpers
    {
        /// <summary>
        /// Synchronize data into a GTK model.
        /// </summary>
        /// <typeparam name="T">Type of element in the collection.</typeparam>
        /// <param name="model">The model to synchronize into.</param>
        /// <param name="collectionChangedArgs">Description of what has changed.</param>
        /// <remarks>Wacky idea: Wrap TreeModel in an INotifyCollection directly?</remarks>
        public static void SynchronizeCollection<T>(this Gtk.TreeModel model, NotifyCollectionChangedEventArgs collectionChangedArgs) where T : class
        {
            if (model == null)
            {
                return;
            }
            if (!(model is Gtk.ListStore))
            {
                throw new System.InvalidOperationException("Only implemented for Gtk.ListStore!");
            }
            var listStore = (Gtk.ListStore)model;
            switch (collectionChangedArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    //var newItems = collectionChangedArgs.NewItems.Cast<T>();
                    foreach (T item in collectionChangedArgs.NewItems)
                    {
                        listStore.AppendValues(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    ErrorReporting.ReportNotImplementedError("ObservableColletionHelpers.SynchronizeCollection:NotifyCollectionChangedAction.Move");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    listStore.RemoveItems<T>(collectionChangedArgs.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // remove
                    listStore.RemoveItems<T>(collectionChangedArgs.OldItems);
 
                    // add
                    foreach (T item in collectionChangedArgs.NewItems)
                    {
                        listStore.AppendValues(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    listStore.Clear();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Synchronize data into a GTK model.
        /// </summary>
        /// <typeparam name="T">Type of element in the collection.</typeparam>
        /// <param name="model">The model data to synchronize into.</param>
        /// <param name="collection">The collection to copy into the model.</param>
        /// <remarks>Wacky idea: Wrap TreeModel in an INotifyCollection directly?</remarks>
        /// <remarks>TODO / BUG: Blindly adds everything! Should confirm what's already in <paramref name="controller"/> and remove what's not!</remarks>
        public static void SynchronizeCollection<T>(this Gtk.TreeModel model, ObservableCollection<T> collection)
        {
            if (!(model is Gtk.ListStore))
            {
                throw new System.InvalidOperationException("Only implemented for Gtk.ListStore!");
            }
            var listStore = (Gtk.ListStore)model;
            foreach (var item in collection)
            {
                listStore.AppendValues(item);
            }
        }

        private static void RemoveItems<T>(this Gtk.ListStore model, IList itemsToRemove) where T : class
        {
            foreach (T itemToRemove in itemsToRemove)
            {
                Gtk.TreeIter iter;
                if (model.GetIterFirst(out iter))
                {
                    var deleted = false;
                    while (!deleted)
                    {
                        var item = model.GetValue(iter, 0) as T;
                        deleted = object.ReferenceEquals(item, itemToRemove);
                        if (deleted)
                        {
                            model.Remove(ref iter); // refers to next element if Remove() returns true
                        }
                        else
                        {
                            if (!model.IterNext(ref iter))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
