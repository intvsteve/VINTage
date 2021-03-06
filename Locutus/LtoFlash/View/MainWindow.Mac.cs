// <copyright file="MainWindow.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2021 All Rights Reserved
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
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using Locutus.ViewModel;
#if __UNIFIED__
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__

namespace Locutus.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class MainWindow : NSWindow, System.ComponentModel.INotifyPropertyChanged, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public MainWindow(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public MainWindow(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
#if __UNIFIED__
            var setAllowsAutomaticWindowTabbingSelector = new Selector("setAllowsAutomaticWindowTabbing:");
            if (RespondsToSelector(setAllowsAutomaticWindowTabbingSelector))
            {
                NSWindow.AllowsAutomaticWindowTabbing = false;
            }
#endif // __UNIFIED__
            var viewModel = new Locutus.ViewModel.MainWindowViewModel();
            SingleInstanceApplication.Instance.DataContext = viewModel;
            DataContext = viewModel;
            CollectionBehavior = NSWindowCollectionBehavior.FullScreenPrimary;
        }

        #endregion // Constructors

        #region IFakeDependencyObject Properties

        /// <summary>
        /// Gets or sets the data context (WPF-ism).
        /// </summary>
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContext(value, PropertyChanged); }
        }

        #endregion // IFakeDependencyObject Properties

#if __UNIFIED__
        /// <inheritdoc />
        public override NSWindowTabbingMode TabbingMode
        {
            get { return NSWindowTabbingMode.Disallowed; }
            set { }
        }
#else
        /// <summary>
        /// Gets or sets the tabbing mode.
        /// </summary>
        /// <remarks>The tabbing mode was introduced in macOS Sierra (10.12) and is not desired. Since we're targeting 10.7 and later,
        /// the property is not part of Xamarin.Mac (as of this writing), so just directly implement it.</remarks>
        public NSWindowTabbingMode TabbingMode
        {
            [Export("tabbingMode")]
            get { return NSWindowTabbingMode.Disallowed; }

            [Export("setTabbingMode:")]
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Locutus.View.MainWindow"/> allows automatic window tabbing.
        /// </summary>
        /// <remarks>The tabbing mode was introduced in macOS Sierra (10.12) and is not desired. Since we're targeting 10.7 and later,
        /// the property is not part of Xamarin.Mac (as of this writing), so just directly implement it.</remarks>
        public static bool AllowsAutomaticWindowTabbing
        {
            [Export("allowsAutomaticWindowTabbing")]
            get { return false; }

            [Export("setAllowsAutomaticWindowTabbing:")]
            set { }
        }
#endif // __UNIFIED__

        /// <summary>
        /// Gets the view model for the main window.
        /// </summary>
        internal MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)DataContext; }
        }

        /// <summary>
        /// Overrides the default toolbar validation method. The command system takes care of enabling / disabling toolbar items.
        /// </summary>
        /// <param name="toolbarItem">The toolbar item to enable or disable. Not used.</param>
        /// <returns><c>false</c> -- the command system is responsible for this.</returns>
        [Export("validateToolbarItem:")]
        public bool ValidateToolbarItem(NSToolbarItem toolbarItem)
        {
            return false;
        }

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        #region IFakeDependencyObject Methods

        /// <inheritdoc />
        public object GetValue(string propertyName)
        {
            return this.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void SetValue(string propertyName, object value)
        {
            this.SetPropertyValue(propertyName, value, PropertyChanged);
        }

        #endregion // IFakeDependencyObject Methods

        /// <inheritdoc />
        /// <remarks>DO NOT REMOVE THIS! WITHOUT IT, THE NIB CONSTRUCTOR WILL NOT BE CALLED AND
        /// THE MAIN WINDOW IS NOT PROPERLY CREATED!!!!!!
        /// At this time, no specific initialization needs to be done here - it is accomplished in the controller class instead.</remarks>
        public override void AwakeFromNib()
        {
            // HACK This is required for the NIB constructor to be called!
            ////base.AwakeFromNib(); // There is no need to call the base class implementation -- it is a no-op. We only need this method to be present on the type.
        }

#if !__UNIFIED__
        /// <summary>
        /// Beginning in macOS Sierra (10.12) all apps get this mode "for free". We're opting out.
        /// </summary>
        /// <remarks>From NSWindow.h:
        ///  typedef NS_ENUM(NSInteger, NSWindowTabbingMode) {
        /// NSWindowTabbingModeAutomatic, // The system automatically prefers to tab this window when appropriate
        /// NSWindowTabbingModePreferred, // The window explicitly should prefer to tab when shown
        /// NSWindowTabbingModeDisallowed // The window explicitly should not prefer to tab when shown
        /// }  NS_ENUM_AVAILABLE_MAC(10_12);
        /// </remarks>
        public enum NSWindowTabbingMode
        {
            /// <summary>The system automatically prefers to tab this window when appropriate.</summary>
            Automatic,

            /// <summary>The window explicitly should prefer to tab when shown.</summary>
            Preferred,

            /// <summary>The window explicitly should not prefer to tab when shown.</summary>
            Disallowed
        }
#endif // !__UNIFIED__

        private void OSAddPrimaryComponentVisuals(IPrimaryComponent primaryComponent, IEnumerable<ComponentVisual> visuals)
        {
            // TODO: OSAddPrimaryComponentVisuals is not yet implemented.
        }
    }
}
