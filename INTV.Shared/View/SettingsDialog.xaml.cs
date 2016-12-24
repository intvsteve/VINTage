// <copyright file="SettingsDialog.xaml.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Interaction logic for SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : System.Windows.Window
    {
        private static string LastSelectedPreferencesPage { get; set; }
        private bool FinishedAddingTabs { get; set; }

        /// <summary>
        /// Initializes a new instance of the SettingsDialog type.
        /// </summary>
        public SettingsDialog()
        {
            InitializeComponent();
            Title = INTV.Shared.Resources.Strings.SettingsDialog_Title;
            _propertyPages.SelectionChanged += TabSelectionChanged;
            this.DoImport();
        }

        /// <summary>
        /// Creates a new instance of the SettingsDialog.
        /// </summary>
        /// <returns>A new instance of SettingsDialog.</returns>
        public static SettingsDialog Create()
        {
            var settingsDialog = new INTV.Shared.View.SettingsDialog();
            settingsDialog.Owner = System.Windows.Application.Current.MainWindow;
            return settingsDialog;
        }

        private void TabSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                var tab = _propertyPages.SelectedItem as System.Windows.Controls.TabItem;
                LastSelectedPreferencesPage = ((System.Windows.FrameworkElement)tab.Content).DataContext.GetType().FullName;
            }
        }

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="icon">The icon to display for the page. (Currently unused.)</param>
        /// <param name="page">The visual for the page.</param>
        partial void AddTab(string pageName, string icon, object page)
        {
            var tab = new System.Windows.Controls.TabItem() { Header = pageName, Content = page };
            _propertyPages.Items.Add(tab);
        }

        /// <summary>
        /// WPF-specific implementation.
        /// </summary>
        partial void AllTabsAdded()
        {
            FinishedAddingTabs = true;
            if (!string.IsNullOrEmpty(LastSelectedPreferencesPage))
            {
                var pageToSelectIndex = -1;
                for (var i = 0; (pageToSelectIndex < 0) && (i < _propertyPages.Items.Count); ++i)
                {
                    var tab = _propertyPages.Items[i] as System.Windows.Controls.TabItem;
                    var pageId = ((System.Windows.FrameworkElement)tab.Content).DataContext.GetType().FullName;
                    if (pageId == LastSelectedPreferencesPage)
                    {
                        pageToSelectIndex = i;
                    }
                }
                if (pageToSelectIndex >= 0)
                {
                    _propertyPages.SelectedIndex = pageToSelectIndex;
                }
            }
        }
    }
}
