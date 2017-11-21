// <copyright file="RomListView.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_DRAGDROP_TRACE

using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IFakeDependencyObject))]
    [System.ComponentModel.Composition.ExportMetadata("Type", typeof(RomListView))]
    public partial class RomListView : NSView, System.ComponentModel.INotifyPropertyChanged, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public RomListView(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public RomListView(NSCoder coder)
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

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        #region IFakeDependencyObject Properties

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
        }

        #endregion // IFakeDependencyObject Properties

        /// <summary>
        /// Gets the ViewModel for the visual.
        /// </summary>
        internal RomListViewModel ViewModel
        {
            get
            {
                var viewModel = DataContext as RomListViewModel;
                if (viewModel == null)
                {
                    var propertyInfo = DataContext.GetType().GetProperties().First(pi => pi.PropertyType == typeof(RomListViewModel));
                    viewModel = (RomListViewModel)propertyInfo.GetValue(DataContext, null);
                }
                return viewModel;
            }
        }

        /// <summary>
        /// Gets or sets the controller for the visual.
        /// </summary>
        internal RomListViewController Controller { get; set; }

        #region IFakeDependencyObject Methods

        /// <inheritdoc />
        public object GetValue (string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue (string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value);
        }

        #endregion // IFakeDependencyObject Methods

        /// <inheritdoc />
        public override NSDragOperation DraggingEntered (NSDraggingInfo sender)
        {
#if ENABLE_DRAGDROP_TRACE
            System.Diagnostics.Debug.WriteLine("ROMLISTVIEW: string for type: " + sender.DraggingPasteboard.GetStringForType(NSPasteboard.NSFilenamesType));
            foreach (var type in sender.DraggingPasteboard.Types)
            {
                System.Diagnostics.Debug.WriteLine("  *** type: " + type);
            }
            foreach (var item in sender.DraggingPasteboard.PasteboardItems)
            {
                var types = item.Types;
                foreach(var type in types)
                {
                    System.Diagnostics.Debug.WriteLine("  *** type: " + type);
                }
                System.Diagnostics.Debug.WriteLine("ROMLISTVIEW: drag enter: formation: " + item.GetStringForType("public.file-url"));
            }
#endif // ENABLE_DRAGDROP_TRACE
            var result = base.DraggingEntered (sender);
            result = ViewModel.GetDragEnterEffects(sender);
            return result;
        }

        /// <inheritdoc />
        public override bool PerformDragOperation(NSDraggingInfo sender)
        {
            var dropped = sender.DraggingPasteboard.CanReadItemWithDataConformingToTypes(new string[] { RomListViewModel.DragDropFilesDataFormat });
            return dropped;
        }

        /// <inheritdoc />
        public override void ConcludeDragOperation(NSDraggingInfo sender)
        {
            base.ConcludeDragOperation(sender);
            // Defer the "heavy lifting" so the drag-drop operation does not time out.
            PerformSelector(new Selector("FinishDrop:"), sender, 0.1);
        }

        [Export("FinishDrop:")]
        private void FinishDrop(NSObject data)
        {
            ViewModel.DropFilesCommand.Execute(data);
        }
    }
}
