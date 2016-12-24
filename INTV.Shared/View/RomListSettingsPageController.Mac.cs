// <copyright file="RomListSettingsPageController.Mac.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace INTV.Shared.View
{
    /// <summary>
    /// ROM list settings page controller.
    /// </summary>
    public partial class RomListSettingsPageController : MonoMac.AppKit.NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public RomListSettingsPageController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public RomListSettingsPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public RomListSettingsPageController()
            : base ("RomListSettingsPage", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new RomListSettingsPage View { get { return (RomListSettingsPage)base.View; } }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value); }
        }

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            View.Controller = this;
        }

        partial void RemoveSearchDirectory (MonoMac.Foundation.NSObject sender)
        {
            var table = sender as NSTableView;
            if (table != null)
            {
                var clickedRow = table.ClickedRow;
                var searchDirs = this.SearchDirectoriesArrayController.ArrangedObjects();
                if ((clickedRow >= 0) && (clickedRow < searchDirs.Length))
                {
                    var itemToDelete = searchDirs[clickedRow] as NSString;
                    Properties.Settings.Default.RomListSearchDirectories.Remove(itemToDelete);
                }
            }
        }
    }
}
