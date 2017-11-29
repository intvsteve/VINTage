// <copyright file="ControllerElementViewModel.cs" company="INTV Funhouse">
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
using INTV.Core.ComponentModel;
using INTV.Core.Model.Device;
using INTV.Shared.Utility;

#if WIN
using NativeImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using NativeImage = AppKit.NSImage;
#else
using NativeImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED
#endif // WIN

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for an element used in the presentation of an Intellivision controller
    /// as a visual in a user interface. This type is only used to represent keys.
    /// </summary>
    public class ControllerElementViewModel : OSViewModelBase, System.ComponentModel.INotifyPropertyChanged
    {
        private static readonly Dictionary<ControllerKeys, string> ResourceStrings = new Dictionary<ControllerKeys, string>()
        {
            { ControllerKeys.Keypad1, "controller_button_1_x64.png" },
            { ControllerKeys.Keypad2, "controller_button_2_x64.png" },
            { ControllerKeys.Keypad3, "controller_button_3_x64.png" },
            { ControllerKeys.Keypad4, "controller_button_4_x64.png" },
            { ControllerKeys.Keypad5, "controller_button_5_x64.png" },
            { ControllerKeys.Keypad6, "controller_button_6_x64.png" },
            { ControllerKeys.Keypad7, "controller_button_7_x64.png" },
            { ControllerKeys.Keypad8, "controller_button_8_x64.png" },
            { ControllerKeys.Keypad9, "controller_button_9_x64.png" },
            { ControllerKeys.KeypadClear, "controller_button_clear_x64.png" },
            { ControllerKeys.Keypad0, "controller_button_0_x64.png" },
            { ControllerKeys.KeypadEnter, "controller_button_enter_x64.png" },
            { ControllerKeys.ActionKeyTop, "controller_action_topleft_24x64.png" },
            { ControllerKeys.ActionKeyBottomLeft, "controller_action_left_24x64.png" },
            { ControllerKeys.ActionKeyTop | ControllerKeys.NoneActive, "controller_action_topright_24x64.png" },
            { ControllerKeys.ActionKeyBottomRight, "controller_action_right_24x64.png" },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.ViewModel.ControllerElementViewModel"/> class.
        /// </summary>
        /// <param name="key">The key represented by this instance.</param>
        public ControllerElementViewModel(ControllerKeys key)
        {
            Key = key;
            Name = (key & ~ControllerKeys.NoneActive).ToDisplayString();
            string imageResource;
            if (ResourceStrings.TryGetValue(key, out imageResource))
            {
                Image = typeof(ControllerElementViewModel).LoadImageResource("ViewModel/Resources/Images/" + imageResource);
            }
        }

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        /// <summary>
        /// Gets the key.
        /// </summary>
        public ControllerKeys Key { get; private set; }

        /// <summary>
        /// Gets the image for the key.
        /// </summary>
        /// <remarks>NOTE: We use NativeImage here because OS-specific code makes direct reference to this property.</remarks>
        [OSExport("Image")]
        public NativeImage Image
        {
            get { return _image; }
            private set { _image = value; }
        }
        private OSImage _image;

        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="INTV.Shared.ViewModel.ControllerElementViewModel"/>
        /// is selected.
        /// </summary>
        [OSExport("Selected")]
        public bool Selected
        {
            get { return _selected; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Selected", value, ref _selected); }
        }
        private bool _selected;
    }
}
