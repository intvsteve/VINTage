// <copyright file="SelectBackupDialog.Mac.cs" company="INTV Funhouse">
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

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class SelectBackupDialog : NSPanel, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SelectBackupDialog(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SelectBackupDialog(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        #region INotifyPropertyChanged

        /// <inheritdoc/>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        #region Properties

        #region IFakeDependencyObject Properties

        /// <inheritdoc/>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
        }

        #endregion // IFakeDependencyObject Properties

        /// <summary>
        /// Gets or sets the view controller.
        /// </summary>
        internal SelectBackupDialogController Controller { get; set; }

        #endregion // Properties

        /// <summary>
        /// Creates an instance of the dialog pointed to backup data in the specified directory.
        /// </summary>
        /// <param name="backupDirectory">The backup directory.</param>
        /// <param name="backupFileName">Name of the backup file to identify valid backup subdirectories.</param>
        /// <param name="fileExtensions">If <paramref name="showItemsCount"/> is <c>true</c>, these file extensions are used to identify the number of backed up items in the backup directory.</param>
        /// <param name="showItemsCount">If <c>true</c>, include item count in selection dialog.</param>
        /// <returns>A new instance of the dialog.</returns>
        public static SelectBackupDialog Create(string backupDirectory, string backupFileName, IEnumerable<string> fileExtensions, bool showItemsCount)
        {
            var controller = new SelectBackupDialogController();
            controller.Initialize(backupDirectory, backupFileName, fileExtensions, showItemsCount);
            var window = controller.Window;
            return window;
        }

        #region IFakeDependencyObject Methods

        /// <inheritdoc/>
        public object GetValue (string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc/>
        public void SetValue (string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject Methods

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (Controller != null))
            {
                Controller.Dispose();
                Controller = null;
            }
            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }
    }
}
