// <copyright file="MainWindow.Gtk.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using INTV.LtoFlash.View;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace Locutus.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class MainWindow : Gtk.Window
    {
        private Dictionary<string, Gdk.Pixbuf> _cachedImages = new Dictionary<string, Gdk.Pixbuf>();
        private int _itemCount;
        private int _selectionCount;
        private Gtk.Image _deviceConnectionStatusImage;
        private DeviceViewModel _activeDevice;
        private Gtk.Label _activeDeviceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LtoFlash.MainWindow"/> class.
        /// </summary>
        public MainWindow()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();
            _mainHPaned.Realized += HandleMainHPanedRealized;
        }

        protected override void OnDestroyed()
        {
            Locutus.Properties.Settings.Default.PrimarySplitterPosition = _mainHPaned.Position;
            base.OnDestroyed();
        }

        private void OSAddPrimaryComponentVisuals(IPrimaryComponent primaryComponent, IEnumerable<ComponentVisual> visuals)
        {
            foreach (var visual in visuals)
            {
                if (visual.UniqueId == RomListView.Id)
                {
                    // NOTE: Possibly due to oddness it Stetic in MonoDevelop, or the original author's
                    // inexperience w/ GTK... but... If the Stetic designer did not have a visual in
                    // the first child, then the automatically generated Build() method simply calls
                    // HPaned.Add() -- which, by default, places the Gtk.Notebook into the first
                    // slot. As a HACK just put a button in there, which we remove and then replace
                    // with the supplied visual.
                    // TODO: Get rid of the hack and see if inserting the visual will work instead.
                    var placeholder = _mainHPaned.FindChild<Gtk.Widget>(c => c.Name == "_child1");
                    _mainHPaned.Remove(placeholder);
                    placeholder.Destroy();
                    _child1 = null;
                    _mainHPaned.Add1(visual.Visual);
                    var romListView = visual.Visual.AsType<RomListView>();
                    GLib.Idle.Add(() =>
                        {
                            var romListTree = visual.Visual.NativeVisual.FindChild<Gtk.TreeView>();
                            romListTree.GrabFocus();
                            return false;
                        });
                    var viewModel = romListView.DataContext as RomListViewModel;
                    viewModel.CollectionChanged += HandleRomListCollectionChanged;
                    viewModel.CurrentSelection.CollectionChanged += HandleRomListSelectionChanged;
                    HandleRomListCollectionChanged(viewModel, null);
                }
                else if (visual.UniqueId == MenuLayoutView.Id)
                {
                    var menuLayout = visual.Visual.AsType<MenuLayoutView>();
                    var viewModel = menuLayout.DataContext as LtoFlashViewModel;
                    var initializePropertyValues = new[]
                    {
                        LtoFlashViewModel.ActiveLtoFlashDevicePropertyName,
                        "LtoDeviceConnectedImage",
                    };
                    foreach (var property in initializePropertyValues)
                    {
                        HandleLtoFlashPropertyChanged(viewModel, new PropertyChangedEventArgs(property));
                    }
                    _layoutsNotebook.RemovePage(0); // this falls apart once >1 page is involved
                    var menuLayoutLabel = new Gtk.Label(visual.DisplayName);
                    _layoutsNotebook.AppendPage(visual.Visual, menuLayoutLabel);
                    viewModel.PropertyChanged += HandleLtoFlashPropertyChanged;
                }
            }
        }

        private void HandleLtoFlashPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleLtoFlashPropertyChangedCore);
        }

        private void HandleLtoFlashPropertyChangedCore(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var viewModel = sender as LtoFlashViewModel;
            switch (e.PropertyName)
            {
                case "LtoDeviceConnectedImage":
                    var type = viewModel.GetType();
                    var imageUri = ResourceHelpers.CreatePackedResourceString(type, viewModel.LtoDeviceConnectedImage);
                    Gdk.Pixbuf image;
                    if (!_cachedImages.TryGetValue(imageUri, out image))
                    {
                        image = type.LoadImageResource(viewModel.LtoDeviceConnectedImage);
                        _cachedImages[imageUri] = image;
                    }
                    if (_deviceConnectionStatusImage == null)
                    {
                        _deviceConnectionStatusImage = new Gtk.Image();
                        _statusbar.PackEnd(_deviceConnectionStatusImage, false, false, 2);
                    }
                    _deviceConnectionStatusImage.Pixbuf = image;
                    break;
                case LtoFlashViewModel.ActiveLtoFlashDevicePropertyName:
                    if (_activeDevice != null)
                    {
                        _activeDevice.PropertyChanged -= HandleActiveDevicePropertyChanged;
                    }
                    _activeDevice = viewModel.ActiveLtoFlashDevice;
                    if (_activeDevice != null)
                    {
                        _activeDevice.PropertyChanged += HandleActiveDevicePropertyChanged;
                        UpdateActiveDeviceName(_activeDevice);
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleActiveDevicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleActiveDevicePropertyChangedCore);
        }

        private void HandleActiveDevicePropertyChangedCore(object sender, PropertyChangedEventArgs e)
        {
            var device = sender as DeviceViewModel;
            switch (e.PropertyName)
            {
                case DeviceViewModel.DisplayNamePropertyName:
                    UpdateActiveDeviceName(device);
                    break;
                default:
                    break;
            }
        }

        private void UpdateActiveDeviceName(DeviceViewModel device)
        {
            if (_activeDeviceName == null)
            {
                _activeDeviceName = new Gtk.Label();
                _statusbar.PackEnd(_activeDeviceName, false, false, 2);
            }
            _activeDeviceName.Text = device.DisplayName;
        }

        private void HandleRomListSelectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleRomListSelectionChangedCore);
        }
        
        private void HandleRomListSelectionChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var selection = sender as ObservableCollection<ProgramDescriptionViewModel>;
            _selectionCount = selection.Count;
            UpdateStatusBar();

            var menuLayout = _layoutsNotebook.Children.OfType<MenuLayoutView>().FirstOrDefault();
            var ltoFlashViewModel = menuLayout.DataContext as LtoFlashViewModel;
            ltoFlashViewModel.CurrentSelection = selection;
        }

        private void HandleRomListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.HandleEventOnMainThread(sender, e, HandleRomListCollectionChangedCore);
        }
        
        private void HandleRomListCollectionChangedCore(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var viewModel = sender as RomListViewModel;
            _itemCount = viewModel.Programs.Count;
            UpdateStatusBar();
        }

        private void HandleMainHPanedRealized(object sender, System.EventArgs e)
        {
            _mainHPaned.SizeAllocated -= HandleMainHPanedRealized;
            var splitterPosition = Locutus.Properties.Settings.Default.PrimarySplitterPosition;
            if ((splitterPosition >= _mainHPaned.MinPosition) && (splitterPosition <= _mainHPaned.MaxPosition))
            {
                _mainHPaned.Position = splitterPosition;
            }
        }

        private void UpdateStatusBar()
        {
            var formatters = RomListViewModel.NumItemsFormat.Split(';');
            var format = _itemCount == 1 ? formatters[1] : formatters[0];
            var itemString = string.Format(format, _itemCount);

            var selectionString = string.Empty;
            if (_selectionCount > 0)
            {
                formatters = RomListViewModel.NumItemsSelectedFormat.Split(';');
                format = _selectionCount == 1 ? formatters[1] : formatters[0];
                selectionString = string.Format(format, _selectionCount);
            }

            _statusbar.Pop(0);
            _statusbar.Push(0, string.Format("{0}    {1}", itemString, selectionString));
        }
    }
}
