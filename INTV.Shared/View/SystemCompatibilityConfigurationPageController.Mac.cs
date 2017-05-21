// <copyright file="SystemCompatibilityConfigurationPageController.Mac.cs" company="INTV Funhouse">
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
using System.Collections.ObjectModel;
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Core.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.Shared.View
{
    /// <summary>
    /// NSViewController for <see cref="SystemCompatibilityConfigurationPage"/>.
    /// </summary>
    public partial class SystemCompatibilityConfigurationPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public SystemCompatibilityConfigurationPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public SystemCompatibilityConfigurationPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public SystemCompatibilityConfigurationPageController()
            : base("SystemCompatibilityConfigurationPage", NSBundle.MainBundle)
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
        public new SystemCompatibilityConfigurationPage View { get { return (SystemCompatibilityConfigurationPage)base.View; } }

        /// <summary>
        /// Gets or sets the the Intellivision II compatibility.
        /// </summary>
        [OSExport(SystemCompatibilityConfigurationPageViewModel.IntellivisionIIPropertyName)]
        public NSNumber IntellivisionII
        {
            get
            {
                return _intellivisionII;
            }

            set
            {
                if (_intellivisionII.Int32Value != value.Int32Value)
                {
                    ViewModel.IntellivisionII = ViewModel.IntellivisionIIOptions[value.Int32Value];
                }
                _intellivisionII = value;
            }
        }
        private NSNumber _intellivisionII;

        /// <summary>
        /// Gets or sets the Sears Super Video Arcade compatibility.
        /// </summary>
        [OSExport(SystemCompatibilityConfigurationPageViewModel.SuperVideoArcadePropertyName)]
        public NSNumber SuperVideoArcade
        {
            get
            {
                return _superVideoArcade;
            }

            set
            {
                if (_superVideoArcade.Int32Value != value.Int32Value)
                {
                    ViewModel.SuperVideoArcade = ViewModel.SuperVideoArcadeOptions[value.Int32Value];
                }
                _superVideoArcade = value;
            }
        }
        private NSNumber _superVideoArcade;

        /// <summary>
        /// Gets or sets the Tutorvision compatibility.
        /// </summary>
        [OSExport(SystemCompatibilityConfigurationPageViewModel.TutorvisionPropertyName)]
        public NSNumber Tutorvision
        {
            get
            {
                return _tutorvision;
            }

            set
            {
                if (_tutorvision.Int32Value != value.Int32Value)
                {
                    ViewModel.Tutorvision = ViewModel.TutorvisionOptions[value.Int32Value];
                }
                _tutorvision = value;
            }
        }
        private NSNumber _tutorvision;

        #region IFakeDependencyObject

        /// <inheritdoc />
        public object DataContext
        {
            get { return this.GetDataContext(); }
            set { this.SetDataContextWithDataContextPropertyChangedHandler(value, ViewModelPropertyChanged); }
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

        private SystemCompatibilityConfigurationPageViewModel ViewModel { get { return DataContext as SystemCompatibilityConfigurationPageViewModel; } }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            var initializationData = new[] {
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(IntellivisionIIPopUpButton, ViewModel.IntellivisionIIOptions, ViewModel.IntellivisionII),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(SuperVideoArcadePopUpButton, ViewModel.SuperVideoArcadeOptions, ViewModel.SuperVideoArcade),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(TutorvisionPopUpButton, ViewModel.TutorvisionOptions, ViewModel.Tutorvision)
            };
            initializationData.InitializePopupButtons();
            ViewModel.RaisePropertyChangedForVisualInit();
        }

        private void ViewModelPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case SystemCompatibilityConfigurationPageViewModel.IntellivisionIIPropertyName:
                    _intellivisionII = new NSNumber(ViewModel.IntellivisionIIOptions.IndexOf(ViewModel.IntellivisionII));
                    break;
                case SystemCompatibilityConfigurationPageViewModel.SuperVideoArcadePropertyName:
                    _superVideoArcade = new NSNumber(ViewModel.SuperVideoArcadeOptions.IndexOf(ViewModel.SuperVideoArcade));
                    break;
                case SystemCompatibilityConfigurationPageViewModel.TutorvisionPropertyName:
                    _tutorvision = new NSNumber(ViewModel.TutorvisionOptions.IndexOf(ViewModel.Tutorvision));
                    break;
            }
            this.RaiseChangeValueForKey(e.PropertyName);
        }
    }
}
