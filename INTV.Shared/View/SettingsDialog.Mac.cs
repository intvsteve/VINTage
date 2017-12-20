// <copyright file="SettingsDialog.Mac.cs" company="INTV Funhouse">
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
using System.Linq;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

#if __UNIFIED__
using CGSize = CoreGraphics.CGSize;
#else
using CGSize = System.Drawing.SizeF;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation for the settings dialog.
    /// </summary>
    public partial class SettingsDialog : NSPanel
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SettingsDialog(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SettingsDialog(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            SelectableItemIdentifiers = new List<string>();
            PageControllers = new List<NSViewController>();
        }
        
        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the visual that presents the property pages.
        /// </summary>
        internal NSTabView PropertyPages { get; set; }

        private List<string> SelectableItemIdentifiers { get; set; }

        private List<NSViewController> PageControllers { get; set; }

        private NSWindowController Controller { get; set; }

        #endregion // Properties

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
            var settingsDialogController = new INTV.Shared.View.SettingsDialogController();
            var settingsDialog = settingsDialogController.Window;
            settingsDialog.Controller = settingsDialogController;
            return settingsDialog;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            Controller = null;

            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }

        private void ShowDialog()
        {
            this.ShowDialog(false);
        }

        private void SettingsPageSelected(object sender, System.EventArgs e)
        {
            var item = sender as NSToolbarItem;
            LastSelectedPreferencesPage = item.Identifier;
            Title = item.Label;
            PropertyPages.SelectAt(SelectableItemIdentifiers.IndexOf(item.Identifier));
        }

        private string[] SelectableItems(NSToolbar toolbar)
        {
            return SelectableItemIdentifiers.ToArray();
        }

        /// <summary>
        /// Mac-specific implementation
        /// </summary>
        /// <param name="pageName">Page name.</param>
        /// <param name="icon">The icon for the page.</param>
        /// <param name="page">Platform-specific page visual data.</param>
        partial void AddTab(string pageName, string icon, object page)
        {
            var pageController = page as NSViewController;
            PageControllers.Add(pageController);
            var tabViewItem = new NSTabViewItem();
            tabViewItem.Label = pageName;
            tabViewItem.View = pageController.View;
            PropertyPages.Add(tabViewItem);
            NSToolbarItem item = null;
            if (!string.IsNullOrEmpty(icon))
            {
                if (Toolbar.Items.Length < PropertyPages.Count)
                {
                    var pageAsDependencyObject = page as IFakeDependencyObject;
                    var id = pageAsDependencyObject.DataContext.GetType().FullName;
                    item = new NSToolbarItem(id);
                    item.Label = pageName.TrimEnd(new char[] { '.' });
                    item.Image = page.GetType().LoadImageResource(icon);
                    var size = new CGSize(item.Image.Size.Width * 2, item.Image.Size.Height);
                    item.MinSize = size;
                    item.MaxSize = size;
                    Toolbar.AddToolbarItem(item, id, Toolbar.Items.Length);
                }
                else
                {
                    item = Toolbar.Items[PropertyPages.Count - 1];
                }
            }
            else
            {
                item = Toolbar.Items[PropertyPages.Count - 1];
            }
            SelectableItemIdentifiers.Add(item.Identifier);
            item.Activated += SettingsPageSelected;
        }

        /// <summary>
        /// Platform-specific implementation.
        /// </summary>
        partial void AllTabsAdded()
        {
            Toolbar.SelectableItemIdentifiers = SelectableItems;
            var selectedPageIdentifier = LastSelectedPreferencesPage ?? Toolbar.Items[0].Identifier;
            Toolbar.SelectedItemIdentifier = selectedPageIdentifier;
            try
            {
                var items = Toolbar.Items;
                var item = Toolbar.Items.First(i => i.Identifier == Toolbar.SelectedItemIdentifier);
                SettingsPageSelected(item, System.EventArgs.Empty);
            }
            catch (System.Exception e)
            {
                // Should we throw a fit here? Probably...
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}
