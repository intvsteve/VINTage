// <copyright file="ProgramFeaturesHelpers.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
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
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.ViewModel;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific helpers.
    /// </summary>
    public static partial class ProgramFeaturesHelpers
    {
        /// <summary>
        /// Creates a menu containing an item for each program feature.
        /// </summary>
        /// <returns>The menu.</returns>
        /// <param name="programFeatures">Program features to put into a menu.</param>
        /// <param name="name">The title of the menu.</param>
        public static NSMenu ToMenu(this IEnumerable<ProgramFeatureImageViewModel> programFeatures, string name)
        {
            var menu = new NSMenu(name);
            foreach (var programFeature in programFeatures)
            {
                menu.AddItem(programFeature.ToMenuItem());
            }
            return menu;
        }

        /// <summary>
        /// Creates a menu item representing the program feature.
        /// </summary>
        /// <returns>The menu item.</returns>
        /// <param name="programFeature">The program feature to represent as a menu item.</param>
        /// <remarks>The menu's <see cref="RepresentedObject"/> is the <paramref name="programFeature"/>.</remarks>
        public static NSMenuItem ToMenuItem(this ProgramFeatureImageViewModel programFeature)
        {
            var menuItem = new NSMenuItem(programFeature.Name);
            menuItem.Image = programFeature.Image;
            menuItem.ToolTip = programFeature.ToolTip.SafeString();
            menuItem.RepresentedObject = new NSObjectWrapper<ProgramFeatureImageViewModel>(programFeature);
            return menuItem;
        }

        /// <summary>
        /// Initializes the menus for a collection of <see cref="NSPopUpButton"/> objects.
        /// </summary>
        /// <param name="initializationData">An enumerable containing the buttons and the data needed to populate their menus.</param>
        public static void InitializePopupButtons(this IEnumerable<System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>> initializationData)
        {
            foreach (var selectionData in initializationData)
            {
                var button = selectionData.Item1;
                button.RemoveAllItems();
                button.Menu = selectionData.Item2.ToMenu(string.Empty);
                button.SelectItem(selectionData.Item2.IndexOf(selectionData.Item3));
            }
        }

        /// <summary>
        /// Converts a <see cref="ProgramFeatureImageViewModel"/> to a <see cref="NSNumber"/>, whose value is the index in the collection.
        /// </summary>
        /// <param name="programFeatures">A list of program features.</param>
        /// <param name="selection">The selection from the list.</param>
        /// <returns>The index of <paramref name="selection"/> in <paramref name="programFeatures"/> as a NSNumber.</returns>
        public static NSNumber SelectionToNSNumber(this IList<ProgramFeatureImageViewModel> programFeatures, ProgramFeatureImageViewModel selection)
        {
            return new NSNumber(programFeatures.IndexOf(selection));
        }
    }
}
