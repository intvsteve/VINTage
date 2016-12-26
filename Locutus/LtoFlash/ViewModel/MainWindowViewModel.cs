// <copyright file="MainWindowViewModel.cs" company="INTV Funhouse">
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

using System.Collections.Specialized;
using System.Linq;
using INTV.Core.Model.Program;
using INTV.Intellicart.ViewModel;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.ViewModel;

namespace Locutus.ViewModel
{
    /// <summary>
    /// Logic for the main application window.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {
            Initialize();
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the URI for the full credits file to open from the main Credits display.
        /// </summary>
        public static System.Uri CreditsFilePath
        {
            get
            {
                var creditsFileName = "full_credits.html";
                if (INTV.Shared.Utility.SingleInstanceApplication.Instance != null)
                {
                    var programDir = INTV.Shared.Utility.SingleInstanceApplication.Instance.ProgramDirectory;
                    creditsFileName = System.IO.Path.Combine(programDir, creditsFileName);
                }
                var uri = new System.Uri(creditsFileName, System.UriKind.RelativeOrAbsolute);
                return uri;
            }
        }

        #region Main Window Properties

        /// <summary>
        /// Gets or sets the title of the application window.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { AssignAndUpdateProperty("Title", value, ref _title); }
        }

        private string _title;

        #endregion // Main Window Properties

        /// <summary>
        /// Gets the ROM list ViewModel.
        /// </summary>
        public RomListViewModel RomList
        {
            get { return _romList; }
        }
        private RomListViewModel _romList;

        #region LTO Flash!-Specific Properties

        /// <summary>
        /// Gets the menu layout ViewModel.
        /// </summary>
        public INTV.LtoFlash.ViewModel.MenuLayoutViewModel MenuLayout
        {
            get { return _ltoFlashViewModel.HostPCMenuLayout; }
        }

        /// <summary>
        /// Gets the LTO Flash ViewModel
        /// </summary>
        public INTV.LtoFlash.ViewModel.LtoFlashViewModel LtoFlash
        {
            get { return _ltoFlashViewModel; }
        }
        private INTV.LtoFlash.ViewModel.LtoFlashViewModel _ltoFlashViewModel;

        #endregion // LTO Flash!-Specific Properties

        /// <summary>
        /// Gets the Intellicart ViewModel.
        /// </summary>
        public INTV.Intellicart.ViewModel.IntellicartViewModel Intellicart
        {
            get
            {
                if (_intellicartViewModel == null)
                {
                    _intellicartViewModel = new IntellicartViewModel();
                }
                return _intellicartViewModel;
            }
        }
        private IntellicartViewModel _intellicartViewModel;

        [System.ComponentModel.Composition.ImportMany]
        private System.Collections.Generic.IEnumerable<System.Lazy<IPrimaryComponent>> Components { get; set; }

        #endregion // Properties

        private void Initialize()
        {
            _title = Resources.Strings.MainWindowTitle;
            _romList = new RomListViewModel();
            _romList.PropertyChanged += HandleRomListViewModelPropertyChanged;
            _ltoFlashViewModel = new INTV.LtoFlash.ViewModel.LtoFlashViewModel();
            _ltoFlashViewModel.PropertyChanged += HandleLtoFlashViewModelPropertyChanged;
            _romList.CollectionChanged += HandleRomListChanged;
            this.DoImport();
        }

        private void HandleRomListViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSelection")
            {
                if (LtoFlash != null)
                {
                    LtoFlash.CurrentSelection = RomList.CurrentSelection;
                }
            }
        }

        private void HandleLtoFlashViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void HandleRomListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    MenuLayout.StartItemsUpdate();
                    for (int i = 0; i < e.OldItems.Count; ++i)
                    {
                        var program = e.OldItems[i] as ProgramDescription;
                        MenuLayout.MarkProgramMissing(program, LtoFlash.ActiveLtoFlashDevices.Select(vm => vm.Device).Where(m => m.IsValid));
                        ////MenuLayout.MenuLayout.RemoveChildFromHierarchy(f => (f is INTV.LtoFlash.Model.Program) && ((f as INTV.LtoFlash.Model.Program).Description == program));
                    }
                    MenuLayout.FinishItemsUpdate(false); // The menu isn't really changing here...
                    break;
                case NotifyCollectionChangedAction.Add:
                    MenuLayout.StartItemsUpdate();
                    for (int i = 0; i < e.NewItems.Count; ++i)
                    {
                        var program = e.NewItems[i] as ProgramDescription;
                        MenuLayout.MarkProgramAvailable(program, LtoFlash.ActiveLtoFlashDevices.Select(vm => vm.Device).Where(m => m.IsValid));
                    }
                    MenuLayout.FinishItemsUpdate(false); // The menu isn't actually changing
                    break;
                default:
                    ////MenuLayout.MarkProgramMissing(program);
                    break;
            }
        }
    }
}
