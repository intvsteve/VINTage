// <copyright file="ObservableCollectionHelpers.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_DIAGNOSTIC_OUTPUT

using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class ObservableCollectionHelpers
    {
        /// <summary>
        /// Synchronize with an object controller.
        /// </summary>
        /// <typeparam name="T">Type of element in the collection.</typeparam>
        /// <param name="controller">The controller to synchronize with.</param>
        /// <param name="collectionChangedArgs">Description of what has changed.</param>
        public static void SynchronizeCollection<T>(this NSObjectController controller, NotifyCollectionChangedEventArgs collectionChangedArgs) where T : class
        {
            switch (collectionChangedArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < collectionChangedArgs.NewItems.Count; i++)
                    {
                        var item = collectionChangedArgs.NewItems[i] as T;
                        var nsObject = item as NSObject;
                        if (nsObject != null)
                        {
                            controller.AddObject(nsObject);
                            DebugOutput("SynchronizeCollection: added " + collectionChangedArgs.NewItems[i] + " of type " + collectionChangedArgs.NewItems[i].GetType ().FullName);
                            DebugOutput("SynchronizeCollection: item as NSObject is " + nsObject + ", is null? " + (nsObject == null));
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
                        var nsObject = item as NSObject;
                        if (nsObject != null)
                        {
                            controller.RemoveObject(nsObject);
                            DebugOutput("SynchronizeCollection: removed " + collectionChangedArgs.OldItems[i] + " of type " + collectionChangedArgs.OldItems[i].GetType ().FullName);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // remove
                    for (int i = 0; i < collectionChangedArgs.OldItems.Count; i++)
                    {
                        var item = collectionChangedArgs.OldItems[i] as T;
                        var nsObject = item as NSObject;
                        if (nsObject != null)
                        {
                            controller.RemoveObject(nsObject);
                        }
                    }

                    // add
                    for (int i = 0; i < collectionChangedArgs.NewItems.Count; i++)
                    {
                        var item = collectionChangedArgs.NewItems[i] as T;
                        var nsObject = item as NSObject;
                        if (item != null)
                        {
                            controller.AddObject(nsObject);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ErrorReporting.ReportNotImplementedError("ObservableColletionHelpers.SynchronizeCollection:NotifyCollectionChangedAction.Reset");
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Synchronize with an object controller.
        /// </summary>
        /// <typeparam name="T">Type of element in the collection.</typeparam>
        /// <param name="controller">The controller to synchronize with.</param>
        /// <param name="collection">The collection to copy into the controller.</param>
        public static void SynchronizeCollection<T>(this NSObjectController controller, ObservableCollection<T> collection)
        {
            var selectAddedObjects = false;
            var arrayController = controller as NSArrayController;
            if (arrayController != null)
            {
                selectAddedObjects = arrayController.SelectsInsertedObjects;
                arrayController.SelectsInsertedObjects = false;
            }
            foreach (var item in collection.OfType<NSObject>())
            {
                controller.AddObject(item);
            }
            if (arrayController != null)
            {
                arrayController.SelectsInsertedObjects = selectAddedObjects;
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DIAGNOSTIC_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
