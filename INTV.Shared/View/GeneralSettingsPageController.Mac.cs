// <copyright file="GeneralSettingsPageController.Mac.cs" company="INTV Funhouse">
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

#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// Controller for the GeneralSettingsPage NSView.
    /// </summary>
    public partial class GeneralSettingsPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public GeneralSettingsPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public GeneralSettingsPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public GeneralSettingsPageController()
            : base("GeneralSettingsPage", NSBundle.MainBundle)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Controllers

        /// <summary>
        /// Gets the view as a strongly typed value.
        /// </summary>
        public new GeneralSettingsPage View { get { return (GeneralSettingsPage)base.View; } }

        /// <summary>
        /// Gets the ROMs directory.
        /// </summary>
        [OSExport("RomsDirectory")]
        public string RomsDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.RomsDirectory; } }

        /// <summary>
        /// Gets the manuals directory.
        /// </summary>
        [OSExport("ManualsDirectory")]
        public string ManualsDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.ManualsDirectory; } }

        /// <summary>
        /// Gets the overlays directory.
        /// </summary>
        [OSExport("OverlaysDirectory")]
        public string OverlaysDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.OverlaysDirectory; } }

        /// <summary>
        /// Gets the boxes directory.
        /// </summary>
        [OSExport("BoxesDirectory")]
        public string BoxesDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.BoxesDirectory; } }

        /// <summary>
        /// Gets the labels directory.
        /// </summary>
        [OSExport("LabelsDirectory")]
        public string LabelsDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.LabelsDirectory; } }

        /// <summary>
        /// Gets the backup directory.
        /// </summary>
        [OSExport("BackupDataDirectory")]
        public string BackupDataDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.BackupDataDirectory; } }

        /// <summary>
        /// Gets the error log directory.
        /// </summary>
        [OSExport("ErrorLogDirectory")]
        public string ErrorLogDirectory { get { return INTV.Shared.Model.RomListConfiguration.Instance.ErrorLogDirectory; } }

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

        partial void ShowBackupDirectory(NSObject sender)
        {
            BackupDataDirectory.RevealInFileSystem();
        }

        partial void ShowBoxesDirectory(NSObject sender)
        {
            BoxesDirectory.RevealInFileSystem();
        }

        partial void ShowErrorLogsDirectory(NSObject sender)
        {
            ErrorLogDirectory.RevealInFileSystem();
        }

        partial void ShowLabelsDirectory(NSObject sender)
        {
            LabelsDirectory.RevealInFileSystem();
        }

        partial void ShowManualsDirectory(NSObject sender)
        {
            ManualsDirectory.RevealInFileSystem();
        }

        partial void ShowOverlaysDirectory(NSObject sender)
        {
            OverlaysDirectory.RevealInFileSystem();
        }

        partial void ShowRomsDirectory(NSObject sender)
        {
            RomsDirectory.RevealInFileSystem();
        }
    }
}
