// <copyright file="SelectBackupDialogController.Mac.cs" company="INTV Funhouse">
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
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Controller for the SelectBackupDialog implementation.
    /// </summary>
    public partial class SelectBackupDialogController : NSWindowController
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SelectBackupDialogController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SelectBackupDialogController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public SelectBackupDialogController()
            : base("SelectBackupDialog")
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            DialogDataContext = new SelectBackupDialogViewModel();
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new SelectBackupDialog Window { get { return (SelectBackupDialog)base.Window; } }

        /// <summary>
        /// Gets the title.
        /// </summary>
        [OSExport("Title")]
        public string Title { get { return DialogDataContext.Title; } }

        /// <summary>
        /// Gets the prompt.
        /// </summary>
        [OSExport("Prompt")]
        public string Prompt { get { return DialogDataContext.Prompt; } }

        /// <summary>
        /// Gets whether to enable the Restore button.
        /// </summary>
        [OSExport("CanRestore")]
        public bool CanRestore { get { return DialogDataContext.SelectedBackupDirectory != null; } }

        private SelectBackupDialogViewModel DialogDataContext { get; set; }

        /// <inheritdoc/>
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            BackupDirectoriesArrayController.SynchronizeCollection(DialogDataContext.Backups);
            BackupDirectoriesArrayController.AddObserver(this, (NSString)"selectionIndex", NSKeyValueObservingOptions.New, this.Handle);
            Window.Controller = this;
            Window.DataContext = DialogDataContext;
            if (!DialogDataContext.ShowItemsCount)
            {
                var table = Window.ContentView.FindChild<NSTableView>();
                var columns = table.TableColumns();
                var column = columns.Where(c => !string.IsNullOrEmpty(c.Identifier) && c.Identifier.ToLower() == "numitems").First();
                column.Hidden = true;
            }
        }

        /// <inheritdoc/>
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, System.IntPtr context)
        {
            if (context == this.Handle)
            {
                switch (keyPath)
                {
                    case "selectionIndex":
                        DialogDataContext.SelectedIndex = BackupDirectoriesArrayController.SelectionIndex;
                        this.RaiseChangeValueForKey("CanRestore");
                        break;
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        /// <summary>
        /// Initialize the dialog for use.
        /// </summary>
        /// <param name="backupsDirectory">The directory in which to look for backup data.</param>
        /// <param name="showItemsCount">If <c>true</c>, show items count in backup to restore selection dialog; otherwise hide that column.</param>
        internal void Initialize(string backupsDirectory, string backupFileName, IEnumerable<string> fileExtensions, bool showItemsCount)
        {
            DialogDataContext.Initialize(backupsDirectory, backupFileName, fileExtensions, showItemsCount);
        }

        partial void OnCancel(NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Aborted);
        }

        partial void OnRestore(NSObject sender)
        {
            Window.EndDialog(NSRunResponse.Stopped);
        }
    }
}
