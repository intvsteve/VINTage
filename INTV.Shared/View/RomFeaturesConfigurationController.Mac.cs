// <copyright file="RomFeaturesConfigurationController.Mac.cs" company="INTV Funhouse">
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
using MonoMac.Foundation;
using MonoMac.AppKit;
using INTV.Core.ComponentModel;
using INTV.Shared.Behavior;
using INTV.Shared.Commands;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// NSViewController for <see cref="RomFeaturesConfiguration"/>.
    /// </summary>
    public partial class RomFeaturesConfigurationController : MonoMac.AppKit.NSWindowController, IFakeDependencyObject, IInPlaceEditor
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public RomFeaturesConfigurationController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public RomFeaturesConfigurationController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public RomFeaturesConfigurationController()
            : base("RomFeaturesConfiguration")
        {
            Initialize();
        }
        
        private void Initialize()
        {
            ViewModel = new RomFeaturesConfigurationViewModel();
            ViewModel.PropertyChanged += ViewModelPropertyChanged;
            DataContext = ViewModel;
        }

        #endregion

        /// <summary>
        /// Gets the window as a strongly typed value.
        /// </summary>
        public new RomFeaturesConfiguration Window { get { return (RomFeaturesConfiguration)base.Window; } }

        /// <summary>
        /// Gets or sets the index of the currently selected feature page.
        /// </summary>
        [OSExport("CurrentFeaturePageIndex")]
        public NSIndexSet CurrentFeaturePageIndex
        {
            get
            {
                return _currentFeaturePageIndex;
            }

            set
            {
                if (_currentFeaturePageIndex.FirstIndex != value.FirstIndex)
                {
                    _currentFeaturePageIndex = value;
                    UpdateCurrentSelection((int)value.FirstIndex);
                }
            }
        }
        private NSIndexSet _currentFeaturePageIndex;

        [OSExport("RevertChangesImage")]
        private NSImage RevertChangesImage { get { return RomListCommandGroup.RevertProgramFeaturesCommand.SmallIcon; } }

        [OSExport("RevertToDatabaseImage")]
        private NSImage RevertToDatabaseImage { get { return RomListCommandGroup.RevertToDatabaseFeaturesCommand.SmallIcon; } }

        [OSExport("CommitImage")]
        private NSImage CommitImage { get { return RomListCommandGroup.UpdateProgramFeaturesCommand.SmallIcon; } }

        [OSExport("CancelImage")]
        private NSImage CancelImage { get { return RomListCommandGroup.CancelUpdateProgramFeaturesCommand.SmallIcon; } }

        [OSExport("EnableRevertToDefault")]
        private NSNumber EnableRevertToDefault { get { return new NSNumber(RomListCommandGroup.RevertProgramFeaturesCommand.CanExecute(RomListCommandGroup.Group.Context)); } }

        [OSExport("Name")]
        private string Name { get { return ViewModel.Description.Name; } }

        private RomFeaturesConfigurationViewModel ViewModel { get; set; }

        private readonly HashSet<NSViewController> _pageControllers = new HashSet<NSViewController>();

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

        #region IInPlaceEditor

        /// <inheritdoc />
        public event EventHandler<InPlaceEditorClosedEventArgs> EditorClosed;

        /// <inheritdoc />
        public NSView EditedElement { get { return null; } }

        /// <inheritdoc />
        public NSView ElementOwner { get; set; }

        /// <inheritdoc />
        public void BeginEdit()
        {
        }

        /// <inheritdoc />
        public void CancelEdit()
        {
            var pages = FeaturePagesTabView.Items;
            foreach (var page in _pageControllers)
            {
                page.DiscardEditing();
            }
            Window.EndDialog(NSRunResponse.Stopped);
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(false));
            }
        }

        /// <inheritdoc />
        public void CommitEdit()
        {
            var committed = true;
            foreach (var page in _pageControllers)
            {
                if (!page.CommitEditing())
                {
                    committed = false;
                }
            }
            if (committed)
            {
                var changed = ViewModel.CommitChangesToProgramDescription();
                Window.EndDialog(NSRunResponse.Stopped);
                var editorClosed = EditorClosed;
                if (editorClosed != null)
                {
                    editorClosed(this, new InPlaceEditorClosedEventArgs(changed));
                }
            }
        }

        #endregion // IInPlaceEditor

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            FeaturePagesArrayController.SynchronizeCollection(ViewModel.FeatureGroups);
            ViewModelPropertyChanged(ViewModel, new System.ComponentModel.PropertyChangedEventArgs("CurrentSelection"));
            ViewModelPropertyChanged(ViewModel, new System.ComponentModel.PropertyChangedEventArgs("CurrentSelectionVisual"));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            ViewModel.PropertyChanged -= ViewModelPropertyChanged;
            // MonoMac has some problems w/ lifetime. This was an attempt to prevent leaking dialogs.
            // However, there are cases that result in over-release that are not easily identified.
            // So, leak it is! :(
            // base.Dispose(disposing);
        }

        private void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentSelection":
                    UpdateCurrentFeaturePageIndex();
                    break;
                case "CurrentSelectionVisual":
                    UpdateSelectedTab();
                    break;
                case "ShowRevertToDefault":
                    this.RaiseChangeValueForKey("ShowRevertToDefault");
                    break;
            }
        }

        private void UpdateCurrentSelection(int newSelectionIndex)
        {
            var newSelection = ViewModel.FeatureGroups[newSelectionIndex];
            ViewModel.CurrentSelection = newSelection;
        }

        private void UpdateCurrentFeaturePageIndex()
        {
            var index = ViewModel.FeatureGroups.IndexOf(ViewModel.CurrentSelection);
            if ((CurrentFeaturePageIndex == null) || (CurrentFeaturePageIndex.FirstIndex != (uint)index))
            {
                _currentFeaturePageIndex = new NSIndexSet(index);
                this.RaiseChangeValueForKey("CurrentFeaturePageIndex");
            }
        }

        private void UpdateSelectedTab()
        {
            var controller = ViewModel.CurrentSelectionVisual;
            var visual = controller.View;
            var pages = FeaturePagesTabView.Items;
            var selectedPage = pages.FirstOrDefault(p => p.View == visual);
            if (selectedPage == null)
            {
                _pageControllers.Add(controller);
                selectedPage = new NSTabViewItem();
                selectedPage.View = visual;
                FeaturePagesTabView.Add(selectedPage);
            }
            FeaturePagesTabView.Select(selectedPage);
        }

        partial void OnRevertToInternalDatabase(NSObject sender)
        {
            if (RomListCommandGroup.RevertToDatabaseFeaturesCommand.CanExecute(ViewModel))
            {
                RomListCommandGroup.RevertToDatabaseFeaturesCommand.Execute(ViewModel);
            }
        }

        partial void OnRevertChanges(NSObject sender)
        {
            RomListCommandGroup.RevertProgramFeaturesCommand.Execute(ViewModel);
        }

        partial void OnUpdateFeatures(NSObject sender)
        {
            RomListCommandGroup.UpdateProgramFeaturesCommand.Execute(ViewModel);
        }

        partial void OnCancelChanges(NSObject sender)
        {
            RomListCommandGroup.CancelUpdateProgramFeaturesCommand.Execute(ViewModel);
        }
    }
}
