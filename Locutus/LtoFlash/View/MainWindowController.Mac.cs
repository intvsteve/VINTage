// <copyright file="MainWindowController.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_DEBUG_SPAM

using System;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;
using INTV.LtoFlash.Commands;
using INTV.LtoFlash.View;
using INTV.Shared.Commands;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace Locutus.View
{
    /// <summary>
    /// Main window controller.
    /// </summary>
    public partial class MainWindowController : MonoMac.AppKit.NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public MainWindowController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [MonoMac.Foundation.Export("initWithCoder:")]
        public MainWindowController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public MainWindowController()
            : base("MainWindow")
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new MainWindow Window { get { return (MainWindow)base.Window; } }

        private RomListViewController RomListController { get; set; }

        /// <summary>
        /// Called to enable or disable a toolbar item.
        /// </summary>
        /// <param name="toolbarItem">The toolbar item whose status is desired.</param>
        /// <returns>Whether to enable the item or not.</returns>
        [MonoMac.Foundation.Export ("validateToolbarItem:")]
        public bool ValidateToolbarItem(NSToolbarItem toolbarItem)
        {
            return false;
        }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            var commandProviders = SingleInstanceApplication.Instance.CommandProviders;
            Window.AddCommandsToMainWindow(commandProviders.Select(c => c.Value));

            string[] desiredToolbarOrder = {
                DownloadCommandGroup.DownloadAndPlayCommand.UniqueId,
                "_0",
                RomListCommandGroup.AddRomFilesCommand.UniqueId,
                RomListCommandGroup.AddRomFoldersCommand.UniqueId,
                MenuLayoutCommandGroup.AddRomsToMenuCommand.UniqueId,
                DownloadCommandGroup.SyncHostToDeviceCommand.UniqueId,
                "_1",
                DeviceCommandGroup.SearchForDevicesCommand.UniqueId,
                DeviceCommandGroup.DisconnectDeviceCommand.UniqueId,
                "_2",
                MenuLayoutCommandGroup.NewDirectoryCommand.UniqueId,
                MenuLayoutCommandGroup.DeleteItemsCommand.UniqueId,
            };

            var toolbar = RootCommandGroup.RootCommand.Visual as NSToolbar;

            foreach(var toolbarItemId in desiredToolbarOrder.Reverse())
            {
                var items = toolbar.Items.ToList();
                var index = items.FindIndex(i => i.Identifier == toolbarItemId);
                NSToolbarItem toolbarItem = null;
                if (index > 0)
                {
                    toolbarItem = items[index];
                    toolbar.RemoveItem(index);
                }
                else if (toolbarItemId[0] == '_')
                {
                    toolbarItem = new NSToolbarItem(NSToolbar.NSToolbarSpaceItemIdentifier); // separator has been deprecated
                }
                if (index != 0)
                {
                    var prevWillAdd = toolbar.WillInsertItem;
                    toolbar.WillInsertItem = (t, i, b) => toolbarItem;
                    toolbar.InsertItem(toolbarItemId, 0);
                    toolbar.WillInsertItem = prevWillAdd;
                }
            }

            RomListController = new INTV.Shared.View.RomListViewController();
            NSViewController viewController = RomListController;
            var romListView = (RomListView)viewController.View;
            romListView.DataContext = Window.ViewModel.RomList;
            romListView.Frame = RomListSplitView.Frame;
            SplitView.ReplaceSubviewWith(RomListSplitView, romListView);
            RomListSplitView = romListView;

            viewController = new INTV.LtoFlash.View.MenuLayoutViewController();
            ((MenuLayoutViewController)viewController).LtoFlashViewModel = Window.ViewModel.LtoFlash;
            var menuLayoutView = (MenuLayoutView)viewController.View;
            menuLayoutView.DataContext = Window.ViewModel.MenuLayout;
            Window.ViewModel.MenuLayout.PropertyChanged += HandleMenuLayoutPropertyChanged;;
            menuLayoutView.Frame = MenuLayoutSplitView.Frame;
            SplitView.ReplaceSubviewWith(MenuLayoutSplitView, menuLayoutView);
            MenuLayoutSplitView = menuLayoutView;

            var progressViewController = new INTV.Shared.View.ProgressIndicatorController();
            var progressIndicatorView = progressViewController.View;
            progressViewController.InitializeDataContext(Window, OverlayLayer.Bounds);
            OverlayLayer.AddSubview(progressIndicatorView);
            Window.LayoutIfNeeded(); // Ensure that we get a refresh of layout after tinkering with the visual tree.
#if ENABLE_DEBUG_SPAM
            Window.DidBecomeMain += (object sender, EventArgs e) => System.Diagnostics.Debug.WriteLine("**** BECAME MAIN");
            Window.DidResignMain += (object sender, EventArgs e) => System.Diagnostics.Debug.WriteLine("**** RESIGNED MAIN");
#endif // ENABLE_DEBUG_SPAM
        }

        private void HandleMenuLayoutPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == INTV.LtoFlash.ViewModel.MenuLayoutViewModel.CurrentSelectionPropertyName)
            {
                var currentSelection = Window.ViewModel.MenuLayout.CurrentSelection;
                var data = RomListController.DataController;
                var clearSelection = true;
                if (currentSelection != null)
                {
                    var selectedProgram = currentSelection as INTV.LtoFlash.ViewModel.ProgramViewModel;
                    if (selectedProgram != null)
                    {
                        var menuLayoutProgramDescription = selectedProgram.ProgramDescription;
                        var matchingRom = Window.ViewModel.RomList.Programs.FirstOrDefault(d => d.Model == menuLayoutProgramDescription);
                        clearSelection = matchingRom == null;
                        if (matchingRom != null)
                        {
                            var index = data.ArrangedObjects().ToList().IndexOf(matchingRom);
                            var romTable = RomListController.View.FindChild<NSTableView>();
                            romTable.SelectRow(index, false);
                            romTable.ScrollRowToVisible(index);
                        }
                    }
                }
                if (clearSelection)
                {
                    data.SelectionIndexes = new NSIndexSet();
                }
            }
        }
    }
}
