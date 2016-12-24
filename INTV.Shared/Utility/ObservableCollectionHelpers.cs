// <copyright file="ObservableCollectionHelpers.cs" company="INTV Funhouse">
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

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Some useful extension methods for ObservableCollection.
    /// </summary>
    public static partial class ObservableCollectionHelpers
    {
        /// <summary>
        /// Offers a simple one-way sync operation to add / remove items to / from a collection via the collection changed event data.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="targetCollection">The collection to be updated, based upon the collection changed arguments.</param>
        /// <param name="collectionChangedArgs">Event arguments describing how a source collection has changed.</param>
        /// <remarks>Does not support move operations.</remarks>
        /// <exception cref="NotImplementedException">Thrown if the collection changed event arguments describe a move operation.</exception>
        public static void SynchronizeCollection<T>(this ObservableCollection<T> targetCollection, NotifyCollectionChangedEventArgs collectionChangedArgs) where T : class
        {
            switch (collectionChangedArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < collectionChangedArgs.NewItems.Count; i++)
                    {
                        var item = collectionChangedArgs.NewItems[i] as T;
                        if (item != null)
                        {
                            targetCollection.Add(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    ErrorReporting.ReportNotImplementedError("ObservableColletionHelpers.SynchronizeCollection:NotifyCollectionChangedAction.Move");
                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < collectionChangedArgs.OldItems.Count; i++)
                    {
                        var item = collectionChangedArgs.OldItems[i] as T;
                        if (item != null)
                        {
                            targetCollection.Remove(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // remove
                    for (int i = 0; i < collectionChangedArgs.OldItems.Count; i++)
                    {
                        var item = collectionChangedArgs.OldItems[i] as T;
                        if (item != null)
                        {
                            targetCollection.Remove(item);
                        }
                    }

                    // add
                    for (int i = 0; i < collectionChangedArgs.NewItems.Count; i++)
                    {
                        var item = collectionChangedArgs.NewItems[i] as T;
                        if (item != null)
                        {
                            targetCollection.Add(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    targetCollection.Clear();
                    if (collectionChangedArgs.NewItems != null)
                    {
                        for (int i = 0; i < collectionChangedArgs.NewItems.Count; i++)
                        {
                            var item = collectionChangedArgs.NewItems[i] as T;
                            if (item != null)
                            {
                                targetCollection.Add(item);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
