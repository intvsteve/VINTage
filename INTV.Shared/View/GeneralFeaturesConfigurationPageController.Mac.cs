// <copyright file="GeneralFeaturesConfigurationPageController.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using INTV.Core.ComponentModel;
using INTV.Shared.ViewModel;
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// NSViewController for <see cref="GeneralFeaturesConfigurationPage"/>.
    /// </summary>
    public partial class GeneralFeaturesConfigurationPageController : NSViewController, IFakeDependencyObject
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public GeneralFeaturesConfigurationPageController(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public GeneralFeaturesConfigurationPageController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>
        /// Call to load from the XIB/NIB file.
        /// </summary>
        public GeneralFeaturesConfigurationPageController()
            : base("GeneralFeaturesConfigurationPage", NSBundle.MainBundle)
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
        public new GeneralFeaturesConfigurationPage View { get { return (GeneralFeaturesConfigurationPage)base.View; } }

        /// <summary>
        /// Gets or sets Intellivoice compatibility.
        /// </summary>
        [OSExport(GeneralFeaturesConfigurationPageViewModel.IntellivoicePropertyName)]
        public NSNumber Intellivoice
        {
            get
            {
                return _intellivoice;
            }

            set
            {
                if (_intellivoice.Int32Value != value.Int32Value)
                {
                    ViewModel.Intellivoice = ViewModel.IntellivoiceOptions[value.Int32Value];
                }
                _intellivoice = value;
            }
        }
        private NSNumber _intellivoice;

        /// <summary>
        /// Gets or sets the NTSC compatibility.
        /// </summary>
        [OSExport(GeneralFeaturesConfigurationPageViewModel.NtscPropertyName)]
        public NSNumber Ntsc
        {
            get
            {
                return _ntsc;
            }

            set
            {
                if (_ntsc.Int32Value != value.Int32Value)
                {
                    ViewModel.Ntsc = ViewModel.NtscOptions[value.Int32Value];
                }
                _ntsc = value;
            }
        }
        private NSNumber _ntsc;

        /// <summary>
        /// Gets or sets the PAL compatibility.
        /// </summary>
        [OSExport(GeneralFeaturesConfigurationPageViewModel.PalPropertyName)]
        public NSNumber Pal
        {
            get
            {
                return _pal;
            }

            set
            {
                if (_pal.Int32Value != value.Int32Value)
                {
                    ViewModel.Pal = ViewModel.PalOptions[value.Int32Value];
                }
                _pal = value;
            }
        }
        private NSNumber _pal;

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

        private GeneralFeaturesConfigurationPageViewModel ViewModel { get { return DataContext as GeneralFeaturesConfigurationPageViewModel; } }

        /// <inheritdoc />
        public override void AwakeFromNib()
        {
            var initializationData = new[] {
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(IntellivoicePopUpButton, ViewModel.IntellivoiceOptions, ViewModel.Intellivoice),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(NtscPopUpButton, ViewModel.NtscOptions, ViewModel.Ntsc),
                new System.Tuple<NSPopUpButton, IList<ProgramFeatureImageViewModel>, ProgramFeatureImageViewModel>(PalPopUpButton, ViewModel.PalOptions, ViewModel.Pal)
            };
            initializationData.InitializePopupButtons();
            ViewModel.RaisePropertyChangedForVisualInit();
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case GeneralFeaturesConfigurationPageViewModel.IntellivoicePropertyName:
                    _intellivoice = new NSNumber(ViewModel.IntellivoiceOptions.IndexOf(ViewModel.Intellivoice));
                    break;
                case GeneralFeaturesConfigurationPageViewModel.NtscPropertyName:
                    _ntsc = new NSNumber(ViewModel.NtscOptions.IndexOf(ViewModel.Ntsc));
                    break;
                case GeneralFeaturesConfigurationPageViewModel.PalPropertyName:
                    _pal = new NSNumber(ViewModel.PalOptions.IndexOf(ViewModel.Pal));
                    break;
            }
            this.RaiseChangeValueForKey(e.PropertyName);
        }
    }
}
