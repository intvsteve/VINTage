// <copyright file="SettingsDialog.Gtk.cs" company="INTV Funhouse">
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

using INTV.Shared.ComponentModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class SettingsDialog : Gtk.Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.View.SettingsDialog"/> class.
        /// </summary>
        public SettingsDialog()
            : base(Resources.Strings.SettingsDialog_Title, INTV.Shared.Utility.SingleInstanceApplication.Instance.MainWindow, Gtk.DialogFlags.Modal)
        {
            this.Build();
            _pages.RemovePage(0);
            this.DoImport();
            _pages.SwitchPage += HandleSwitchPage;
            this.Show();
        }

        /// <summary>
        /// Creates a new instance of the SettingsDialog which will activate a given page.
        /// </summary>
        /// <param name="initialPage">The FullName of type used to identify the page to show.</param>
        /// <returns>A new instance of SettingsDialog.</returns>
        public static SettingsDialog Create(string initialPage)
        {
            if (!string.IsNullOrEmpty(initialPage))
            {
                LastSelectedPreferencesPage = initialPage;
            }
            var settingsDialog = new SettingsDialog();
            return settingsDialog;
        }

        /// <inheritdoc/>
        protected override void OnResponse(Gtk.ResponseType response_id)
        {
            base.OnResponse(response_id);
            VisualHelpers.Close(this);
        }

        private void HandleSwitchPage(object o, Gtk.SwitchPageArgs args)
        {
            var notebook = o as Gtk.Notebook;
            var pageNum = args.PageNum;
            var pageWidget = notebook.GetNthPage((int)pageNum);
            LastSelectedPreferencesPage = pageWidget.GetType().FullName;
        }

        /// <inheritdoc/>
        /// <remarks>GTK-specific implementation</remarks>
        partial void AddTab(string pageName, string icon, object page)
        {
            var pageWidget = (OSVisual)page;
            var pageLabel = new Gtk.Label(pageName);
            var x = _pages.AppendPage(pageWidget, pageLabel);
        }


        /// <inheritdoc/>
        /// <remarks>GTK-specific implementation</remarks>
        partial void AllTabsAdded()
        {
            var pageToSelectIndex = -1;
            if (!string.IsNullOrEmpty(LastSelectedPreferencesPage))
            {
                for (var i = 0; (pageToSelectIndex < 0) && (i < _pages.NPages); ++i)
                {
                    var page = _pages.GetNthPage(i);
                    var pageId = page.GetType().FullName;
                    if (pageId == LastSelectedPreferencesPage)
                    {
                        pageToSelectIndex = i;
                    }
                }
            }
            _pages.ShowAll();
            if (pageToSelectIndex >= 0)
            {
                _pages.CurrentPage = pageToSelectIndex;
            }
        }
    }
}
