// <copyright file="MenuLayoutView.Mac.cs" company="INTV Funhouse">
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
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.View
{
    [System.ComponentModel.Composition.Export(typeof(IFakeDependencyObject))]
    [System.ComponentModel.Composition.ExportMetadata("Type", typeof(MenuLayoutView))]
    public partial class MenuLayoutView : MonoMac.AppKit.NSView, System.ComponentModel.INotifyPropertyChanged, IFakeDependencyObject
    {
        /// <summary>
        /// Name of the file used to store the colors available in the color picker.
        /// </summary>
        internal const string MenuColors = "MenuColors.clr";

        private const string MenuColorPaletteName = "Menu Item Colors";

        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public MenuLayoutView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public MenuLayoutView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
            var cfg = SingleInstanceApplication.Instance.GetConfiguration<INTV.LtoFlash.Model.Configuration>();
            var colorsPath = System.IO.Path.Combine(cfg.ApplicationConfigurationPath, MenuColors);
            NSColorList colorList = null;
            if (!System.IO.File.Exists(colorsPath))
            {
                // We haven't stashed the color list, so make one.
                colorList =  new NSColorList(MenuColorPaletteName);
                var colors = INTV.LtoFlash.ViewModel.MenuLayoutViewModel.Colors;
                int i = 0;
                foreach (var color in colors)
                {
                    colorList.InsertColor(color.ToColor(), color.ToString(), i);
                    ++i;
                }
                if (colorList.WriteToFile(colorsPath))
                {
                    // save it and mark read-only so users can't muck with it.
                    var attributes = System.IO.File.GetAttributes(colorsPath);
                    attributes |= System.IO.FileAttributes.ReadOnly;
                    System.IO.File.SetAttributes(colorsPath, attributes);
                }
            }
            else
            {
                // just in case, mark read-only
                var attributes = System.IO.File.GetAttributes(colorsPath);
                attributes |= System.IO.FileAttributes.ReadOnly;
                System.IO.File.SetAttributes(colorsPath, attributes);
            }
            // Load from read-only color list
            colorList =  new NSColorList(MenuColorPaletteName, colorsPath);
            NSColorPanel.SetPickerStyle(NSColorPanelFlags.ColorList);
            NSColorPanel.SetPickerMode(NSColorPanelMode.ColorList);
            var panel = NSColorPanel.SharedColorPanel;
            panel.AttachColorList(colorList);
            foreach (var defaultColorList in NSColorList.AvailableColorLists)
            {
                panel.DetachColorList(defaultColorList);
            }
            var button = panel.ContentView.FindChild<NSButton>();
            button.Enabled = false;
        }
        
        #endregion // Constructors

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        internal MenuLayoutViewController Controller { get; set; }

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        /// <summary>
        /// Gets the ViewModel.
        /// </summary>
        public INTV.LtoFlash.ViewModel.MenuLayoutViewModel ViewModel
        {
            get { return (INTV.LtoFlash.ViewModel.MenuLayoutViewModel)DataContext; }
        }

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
        }

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

        #endregion // IFakeDependencyObject
    }
}
