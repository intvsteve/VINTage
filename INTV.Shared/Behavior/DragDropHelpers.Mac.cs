// <copyright file="DragDropHelpers.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
#if __UNIFIED__
using AppKit;
#else
using MonoMac.AppKit;
#endif
using INTV.Core.Utility;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Mac-specific helper methods for drag-drop operations.
    /// </summary>
    public static partial class DragDropHelpers
    {
        private static Dictionary<NSPasteboard, NSDataWrapper> PasteboardHelperData = new Dictionary<NSPasteboard, NSDataWrapper>();

        /// <summary>
        /// Prepares the pasteboard.
        /// </summary>
        /// <param name="pasteboard">The pasteboard to use.</param>
        /// <param name="dataType">The data type to put on the pasteboard.</param>
        /// <param name="data">The wrapped data to put onto the pasteboard.</param>
        public static void PreparePasteboard(NSPasteboard pasteboard, string dataType, NSDataWrapper data)
        {
            pasteboard.ClearContents();
            pasteboard.SetDataForType(data, dataType);
            PasteboardHelperData[pasteboard] = data;
        }

        /// <summary>
        /// Gets the data from a pasteboard of a specific type.
        /// </summary>
        /// <typeparam name="T">The data type of the data to retrieve.</typeparam>
        /// <param name="pasteboard">The pasteboard from which to retrieve data.</param>
        /// <param name="dataTypes">The data types expected to be supported by the pasteboard.</param>
        /// <returns>The data for given pasteboard data type, or <c>null</c> if the data is not supported or found.</returns>
        public static T GetDataForType<T>(NSPasteboard pasteboard, string[] dataTypes) where T : class
        {
            T data = default(T);
            if (pasteboard.CanReadItemWithDataConformingToTypes(dataTypes))
            {
                NSDataWrapper dataWrapper = null;
                if (PasteboardHelperData.TryGetValue(pasteboard, out dataWrapper))
                {
                    if (dataWrapper != null)
                    {
                        data = dataWrapper.GetWrappedObject<T>();
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Inidcates that the drag & drop operation has finished using the pasteboard.
        /// </summary>
        /// <param name="pasteboard">The pasteboard that's no longer being used.</param>
        public static void FinishedWithPasteboard(NSPasteboard pasteboard)
        {
            var removedEntry = PasteboardHelperData.Remove(pasteboard);
            if (removedEntry)
            {
                System.Diagnostics.Debug.WriteLine("Cleaned up pasteboard.");
            }
        }
    }
}
