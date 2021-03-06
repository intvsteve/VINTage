﻿// <copyright file="SettingsDialog.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Shared.ComponentModel;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// The settings dialog. Houses the various pages discovered via the ISettingsPage interface.
    /// </summary>
    public partial class SettingsDialog : System.ComponentModel.Composition.IPartImportsSatisfiedNotification
    {
        private static string LastSelectedPreferencesPage { get; set; }

        /// <summary>
        /// Gets or sets the pages in the dialog.
        /// </summary>
        [System.ComponentModel.Composition.ImportMany]
        public IEnumerable<Lazy<ISettingsPage, ISettingsPageMetadata>> Pages { get; set; }

        /// <summary>
        /// Creates a new instance of the SettingsDialog.
        /// </summary>
        /// <returns>A new instance of SettingsDialog.</returns>
        public static SettingsDialog Create()
        {
            return Create(null);
        }

        /// <summary>
        /// Show the settings dialog with a specific initial page.
        /// </summary>
        /// <param name="initialPage">The page to initially show. If <c>null</c>, use default behavior.</param>
        public static void ShowSettingsDialog(string initialPage)
        {
            Create(initialPage).ShowDialog();
        }

        #region IPartImportsSatisfiedNotification Members

        /// <inheritdoc />
        public void OnImportsSatisfied()
        {
            var tabsToAdd = new List<Tuple<string, double, string, object>>();
            CompositionHelpers.SuppressImportErrorDialog = true;
            foreach (var page in Pages)
            {
                try
                {
                    tabsToAdd.Add(new Tuple<string, double, string, object>(page.Metadata.Name, page.Metadata.Weight, page.Metadata.Icon, page.Value.CreateVisual()));
                }
                catch (Exception e)
                {
                    // Failed to load the preference page... so ignore it.
                    System.Diagnostics.Debug.WriteLine("Failed to import preferences page: " + e);
                }
            }
            CompositionHelpers.SuppressImportErrorDialog = false;
            foreach (var tabToAdd in tabsToAdd.OrderBy(t => t.Item2))
            {
                try
                {
                    AddTab(tabToAdd.Item1, tabToAdd.Item3, tabToAdd.Item4);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
            AllTabsAdded();
        }

        #endregion //  IPartImportsSatisfiedNotification Members

        /// <summary>
        /// Platform-specific method to add tabs to the page.
        /// </summary>
        /// <param name="pageName">Display name of the page.</param>
        /// <param name="icon">Icon for the page.</param> 
        /// <param name="page">Visual for the page.</param>
        partial void AddTab(string pageName, string icon, object page);

        /// <summary>
        /// Called when all tabs have been added to the dialog, in case any platform-specific cleanup is necessary.
        /// </summary>
        partial void AllTabsAdded();
    }
}
