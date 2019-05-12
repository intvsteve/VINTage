// <copyright file="VisualRelayCommand.cs" company="INTV Funhouse">
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

using System;
using System.ComponentModel;
using INTV.Core.ComponentModel;
using INTV.Shared.Commands;
using INTV.Shared.Utility;

#if WIN
using INTV.Shared.View;
using NativeImage = System.Windows.Media.ImageSource;
#elif MAC
using INTV.Shared.View;
#if __UNIFIED__
using OSVisual = Foundation.NSObject;
using NativeImage = AppKit.NSImage;
#else
using OSVisual = MonoMac.Foundation.NSObject;
using NativeImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#elif GTK
using INTV.Shared.View;
using NativeImage = Gdk.Pixbuf;
#endif // WIN

namespace INTV.Shared.ComponentModel
{
    /// <summary>
    /// Extend the RelayCommand class to provide additional support when the command is attached to a visual element, such as a button or menu item.
    /// </summary>
    public partial class VisualRelayCommand : INTV.Shared.ComponentModel.RelayCommand, INotifyPropertyChanged
    {
        /// <summary>
        /// A default icon to use for rich ToolTips.
        /// </summary>
        public static readonly NativeImage DefaultToolTipIcon = typeof(VisualRelayCommand).LoadImageResource("ViewModel/Resources/Images/Information_32x.png");

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        public VisualRelayCommand(Action<object> onExecute)
            : this(onExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        public VisualRelayCommand(Action<object> onExecute, Func<object, bool> canExecute)
            : this(onExecute, canExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="onExecute">Action to execute when the command is executed.</param>
        /// <param name="canExecute">Function to call to determine if the command can be executed.</param>
        /// <param name="parameter">Data to pass to the Execute and CanExecute methods.</param>
        public VisualRelayCommand(Action<object> onExecute, Func<object, bool> canExecute, object parameter)
            : base(onExecute, canExecute, parameter)
        {
            _name = string.Empty;
            _menuItemName = string.Empty;
            _contextMenuItemName = string.Empty;
            ToolTip = string.Empty;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class.
        /// </summary>
        /// <param name="toClone">The command to copy.</param>
        /// <remarks>Some members (ToolTipIcon, BlockWhenAppIsBusy, UseXamlResource) are not cloned, but unsure why. If these are oversight, we could just
        /// use MemberwiseClone() instead.</remarks>
        protected VisualRelayCommand(VisualRelayCommand toClone)
            : base(toClone)
        {
            Name = toClone.Name;
            MenuItemName = toClone.MenuItemName;
            ContextMenuItemName = toClone.ContextMenuItemName;
            ToolTip = toClone.ToolTip;
            ToolTipTitle = toClone.ToolTipTitle;
            ToolTipDescription = toClone.ToolTipDescription;
            Weight = toClone.Weight;
            SmallIcon = toClone.SmallIcon;
            LargeIcon = toClone.LargeIcon;
            Visual = toClone.Visual;
            MenuItem = toClone.MenuItem;
            VisualParent = toClone.VisualParent;
            MenuParent = toClone.MenuParent;
            KeyboardShortcutKey = toClone.KeyboardShortcutKey;
            KeyboardShortcutModifiers = toClone.KeyboardShortcutModifiers;
            PreferredParameterType = toClone.PreferredParameterType;
            Initialize();
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the display name of the command.
        /// </summary>
        [OSExport("Name")]
        public string Name
        {
            get { return _name; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "Name", value, ref _name); }
        }
        private string _name;

        /// <summary>
        /// Gets or sets the display name for use in a menu.
        /// </summary>
        [OSExport("MenuItemName")]
        public string MenuItemName
        {
            get
            {
                var menuItemName = _menuItemName;
                if (string.IsNullOrEmpty(menuItemName))
                {
                    menuItemName = GetMenuItemName();
                }
                return string.IsNullOrEmpty(menuItemName) ? Name : menuItemName;
            }

            set
            {
                this.AssignAndUpdateProperty(PropertyChanged, "MenuItemName", value, ref _menuItemName, (p, v) => SetMenuItemName(v));
            }
        }
        private string _menuItemName;

        /// <summary>
        /// Gets or sets the display name when used in a context menu.
        /// </summary>
        [OSExport("ContextMenuItemName")]
        public string ContextMenuItemName
        {
            get
            {
                var menuItemName = _contextMenuItemName;
                if (string.IsNullOrEmpty(menuItemName))
                {
                    menuItemName = GetMenuItemName();
                }
                return menuItemName;
            }

            set
            {
                this.AssignAndUpdateProperty(PropertyChanged, "ContextMenuItemName", value, ref _contextMenuItemName, (p, v) => SetMenuItemName(v));
            }
        }
        private string _contextMenuItemName;

        /// <summary>
        /// Gets or sets a simple tool tip for the command.
        /// </summary>
        /// <remarks>TODO: Have the tool tip setter update the actual menu item if present</remarks>
        [OSExport("ToolTip")]
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets or sets a title for a rich tool tip.
        /// </summary>
        [OSExport("ToolTipTitle")]
        public string ToolTipTitle
        {
            get { return string.IsNullOrEmpty(_toolTipTitle) ? Name : _toolTipTitle; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "ToolTipTitle", value, ref _toolTipTitle); }
        }
        private string _toolTipTitle;

        /// <summary>
        /// Gets or sets the descriptive content for a rich tool tip.
        /// </summary>
        [OSExport("ToolTipDescription")]
        public string ToolTipDescription
        {
            get { return string.IsNullOrEmpty(_toolTipDescription) ? ToolTip : _toolTipDescription; }
            set { this.AssignAndUpdateProperty(PropertyChanged, "ToolTipDescription", value, ref _toolTipDescription); }
        }
        private string _toolTipDescription;

        /// <summary>
        /// Gets or sets the icon to display for a rich tool tip.
        /// </summary>
        [OSExport("ToolTipIcon")]
        public NativeImage ToolTipIcon
        {
            get { return _toolTipIcon == null ? DefaultToolTipIcon : _toolTipIcon; }
            set { _toolTipIcon = value; }
        }
        private NativeImage _toolTipIcon;

        /// <summary>
        /// Gets or sets a small graphical image to display for the command, typically a 16x16 icon.
        /// </summary>
        [OSExport("SmallIcon")]
        public NativeImage SmallIcon { get; set; }

        /// <summary>
        /// Gets or sets a graphical image to display for the command, typically a 32x32 icon.
        /// </summary>
        [OSExport("LargeIcon")]
        public NativeImage LargeIcon { get; set; }

        /// <summary>
        /// Gets or sets the weight of the command.
        /// </summary>
        /// <remarks>Must be a value between [0.0 - 1.0]. The higher the value, the lower (in a menu) or further to the right (horizontal) in L-R systems.</remarks>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets the primary visual presented to the user to execute the command (e.g. a button, etc.).
        /// </summary>
        public OSVisual Visual { get; set; }

        /// <summary>
        /// Gets or sets the parent of the command (e.g. another menu, a ribbon group, etc.).
        /// </summary>
        public ICommand VisualParent { get; set; }

        /// <summary>
        /// Gets or sets the menu item visual for the command.
        /// </summary>
        public OSMenuItem MenuItem { get; set; }

        /// <summary>
        /// Gets or sets the menu that acts as the parent for the command. If null, the root menu is used.
        /// </summary>
        public ICommand MenuParent { get; set; }

        /// <summary>
        /// Gets or sets keyboard shortcut. This is typically a single character.
        /// </summary>
        public string KeyboardShortcutKey { get; set; }

        /// <summary>
        /// Gets or sets the keyboard shortcut modifier keys.
        /// </summary>
        public OSModifierKeys KeyboardShortcutModifiers { get; set; }

        #endregion // Properties

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        /// <summary>
        /// Creates a copy of the command.
        /// </summary>
        /// <returns>A copy of the command.</returns>
        public VisualRelayCommand Clone()
        {
            var clone = new VisualRelayCommand(this);
            return clone;
        }

        /// <summary>
        /// Implementation-specific initialization.
        /// </summary>
        partial void Initialize();
    }
}
