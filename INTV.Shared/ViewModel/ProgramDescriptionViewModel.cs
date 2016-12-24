// <copyright file="ProgramDescriptionViewModel.cs" company="INTV Funhouse">
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
using INTV.Core.ComponentModel;
using INTV.Core.Model;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.Shared.Utility;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
using OSImage = MonoMac.AppKit.NSImage;
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for a ProgramDescription.
    /// </summary>
    public partial class ProgramDescriptionViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Describes the data format for dragging a ProgramDescription. When used with
        /// Drag and Drop operations, the data must be an IEnumerable of ProgramDescription objects.
        /// </summary>
        public static readonly string DragDataFormat = "Intellivision.ProgramDescriptions";

        private static readonly Dictionary<ProgramSupportFileState, OSImage> StatusIcons;
        private static readonly Dictionary<ProgramSupportFileState, string> StatusMessages;
        private ProgramDescription _description;

        #region Constructors

        static ProgramDescriptionViewModel()
        {
            StatusIcons = new Dictionary<ProgramSupportFileState, OSImage>()
            {
                { ProgramSupportFileState.Missing, typeof(ProgramDescriptionViewModel).LoadImageResource("ViewModel/Resources/Images/error_16xLG_color.png") },
                { ProgramSupportFileState.PresentButModified, typeof(ProgramDescriptionViewModel).LoadImageResource("ViewModel/Resources/Images/warning_16xLG_color.png") },
                { ProgramSupportFileState.PresentAndUnchanged, null },
                { ProgramSupportFileState.None, null },
                { ProgramSupportFileState.RequiredPeripheralNotAttached, typeof(ProgramDescriptionViewModel).LoadImageResource("Resources/Images/lto_rom_only_9xSM.png") },
                { ProgramSupportFileState.RequiredPeripheralAvailable, typeof(ProgramDescriptionViewModel).LoadImageResource("Resources/Images/lto_rom_compatible_9xSM.png") },
                { ProgramSupportFileState.RequiredPeripheralIncompatible, typeof(ProgramDescriptionViewModel).LoadImageResource("Resources/Images/lto_rom_incompatible_9xSM.png") },
                { ProgramSupportFileState.RequiredPeripheralUnknown, typeof(ProgramDescriptionViewModel).LoadImageResource("Resources/Images/lto_rom_unknown_device_9xSM.png") },
            };

            StatusMessages = new Dictionary<ProgramSupportFileState, string>()
            {
                { ProgramSupportFileState.Missing, Resources.Strings.ProgramSupportFileState_Missing },
                { ProgramSupportFileState.PresentButModified, Resources.Strings.ProgramSupportFileState_PresentButModified },
                { ProgramSupportFileState.PresentAndUnchanged, null },
                { ProgramSupportFileState.None, null },
                { ProgramSupportFileState.RequiredPeripheralNotAttached, Resources.Strings.ProgramSupportFileState_RequiredPeripheralNotAttached },
                { ProgramSupportFileState.RequiredPeripheralAvailable, Resources.Strings.ProgramSupportFileState_RequiredPeripheralAvailable },
                { ProgramSupportFileState.RequiredPeripheralIncompatible, Resources.Strings.ProgramSupportFileState_RequiredPeripheralIncompatible },
                { ProgramSupportFileState.RequiredPeripheralUnknown, Resources.Strings.ProgramSupportFileState_RequiredPeripheralUnknown },
            };
        }

        /// <summary>
        /// Initializes a new instance of the ProgramDescriptionViewModel class.
        /// </summary>
        /// <param name="programDescription">The ProgramDescription model to model a view for.</param>
        public ProgramDescriptionViewModel(ProgramDescription programDescription)
        {
            _description = programDescription;
            _description.PropertyChanged += OnPropertyChanged;
            if (Properties.Settings.Default.RomListValidateAtStartup)
            {
                ProgramDescription.Validate(programDescription, null, null, false);
            }
            var state = programDescription.Files.GetSupportFileState(ProgramFileKind.Rom);
            RomFileStatus = GetRomFileStatus(state);
            RomFileStatusIcon = StatusIcons[state];
            Features = new ComparableObservableCollection<ProgramFeatureImageViewModel>(_description.Features.ToFeatureViewModels());
            Initialize();
        }

        #endregion // Constructors

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        #region Properties

        /// <summary>
        /// Gets the maximum length for a program name.
        /// </summary>
        public static int MaxProgramNameLength
        {
            get { return ProgramDescription.MaxProgramNameLength; }
        }

        /// <summary>
        /// Gets the maximum length for a vendor string.
        /// </summary>
        public static int MaxVendorNameLength
        {
            get { return ProgramDescription.MaxVendorNameLength; }
        }

        /// <summary>
        /// Gets the maximum length for a game's year.
        /// </summary>
        public static int MaxYearTextLength
        {
            get { return ProgramDescription.MaxYearTextLength; }
        }

        /// <summary>
        /// Gets the ROM file this object describes.
        /// </summary>
        public IRom Rom
        {
            get { return Model.Rom; }
        }

        /// <summary>
        /// Gets or sets the descriptive name of the program.
        /// </summary>
        [OSExportAttribute("Name")]
        public string Name
        {
            get { return GetDisplayName().SafeString(); }
            set { Model.Name = value; }
        }

        /// <summary>
        /// Gets or sets the vendor or other entity who produced the program.
        /// </summary>
        [OSExportAttribute("Vendor")]
        public string Vendor
        {
            get { return Model.Vendor.SafeString(); }
            set { Model.Vendor = value; }
        }

        /// <summary>
        /// Gets or sets the copyright date in the program.
        /// </summary>
        /// <remarks>In some cases, this may also include a year of release on cartridge format for previously unreleased programs.</remarks>
        [OSExportAttribute("Year")]
        public string Year
        {
            get { return Model.Year.SafeString(); }
            set { Model.Year = value; }
        }

        /// <summary>
        /// Gets the features of the program.
        /// </summary>
        public ComparableObservableCollection<ProgramFeatureImageViewModel> Features { get; private set; }

        /// <summary>
        /// Gets the model we're a view model for.
        /// </summary>
        public ProgramDescription Model
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets a value indicating whether the ROM file is unchanged from its last observed state.
        /// </summary>
        public bool RomFileIsUnchanged
        {
            get { return string.IsNullOrEmpty(RomFileStatus); }
        }

        /// <summary>
        /// Gets the status of the ROM file.
        /// </summary>
        public string RomFileStatus { get; private set; }

        /// <summary>
        /// Gets the icon to use to display the ROM's status.
        /// </summary>
        [OSExportAttribute("RomFileStatusIcon")]
        public OSImage RomFileStatusIcon { get; private set; }

        /// <summary>
        /// Gets the maximum length for a program name.
        /// </summary>
        /// <remarks>Stupid xp and .NET 4.0 don't support bindings to static properties.</remarks>
        public int MaxProgramNameLength_xp
        {
            get { return MaxProgramNameLength; }
        }

        /// <summary>
        /// Gets the maximum length for a vendor string.
        /// </summary>
        /// <remarks>Stupid xp and .NET 4.0 don't support bindings to static properties.</remarks>
        public int MaxVendorNameLength_xp
        {
            get { return MaxVendorNameLength; }
        }

        /// <summary>
        /// Gets the maximum length for a game's year.
        /// </summary>
        /// <remarks>Stupid xp and .NET 4.0 don't support bindings to static properties.</remarks>
        public int MaxYearTextLength_xp
        {
            get { return MaxYearTextLength; }
        }

        #endregion Properties

        /// <inheritdoc />
        public static bool operator ==(ProgramDescriptionViewModel lhs, ProgramDescriptionViewModel rhs)
        {
            bool areEqual = object.ReferenceEquals(lhs, rhs) || (!object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null) && object.ReferenceEquals(lhs.Model, rhs.Model));
            if (!areEqual && !object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null))
            {
                areEqual = lhs.Model == rhs.Model;
            }
            return areEqual;
        }

        /// <inheritdoc />
        public static bool operator !=(ProgramDescriptionViewModel lhs, ProgramDescriptionViewModel rhs)
        {
            return !(lhs == rhs);
        }

        #region object Overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ProgramDescriptionViewModel))
            {
                return false;
            }
            return Model.Equals(((ProgramDescriptionViewModel)obj).Model);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        #endregion // object Overrides

        /// <summary>
        /// Refreshes the status of the program description by checking the file system.
        /// </summary>
        /// <param name="peripherals">Peripherals available for compatibility checks.</param>
        /// <returns><c>true</c>, if file status changed, <c>false</c> otherwise.</returns>
        public bool RefreshFileStatus(IEnumerable<IPeripheral> peripherals)
        {
            var currentStatus = RomFileStatus;
            var currentIcon = RomFileStatusIcon;
            ProgramDescription.Validate(Model, peripherals, SingleInstanceApplication.Instance.GetConnectedDevicesHistory(), true);
            var state = Model.Files.GetSupportFileState(ProgramFileKind.Rom);
            RomFileStatus = GetRomFileStatus(state);
            RomFileStatusIcon = StatusIcons[state];
            var statusChanged = (currentIcon != RomFileStatusIcon) || (currentStatus != RomFileStatus);
            if (statusChanged)
            {
                this.RaisePropertyChanged(PropertyChanged, "RomFileStatus");
                this.RaisePropertyChanged(PropertyChanged, "RomFileStatusIcon");
            }
            return statusChanged;
        }

        /// <summary>
        /// Platform-specific initialization.
        /// </summary>
        partial void Initialize();

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Features")
            {
                Features = new ComparableObservableCollection<ProgramFeatureImageViewModel>(_description.Features.ToFeatureViewModels());
            }
            this.RaisePropertyChanged(PropertyChanged, e.PropertyName);
        }

        private string GetDisplayName()
        {
            var name = Properties.Settings.Default.DisplayRomFileNameForTitle ? System.IO.Path.GetFileNameWithoutExtension(Model.Rom.RomPath) : Model.Name;
            return name;
        }

        private string GetRomFileStatus(ProgramSupportFileState state)
        {
            string stateString = null;
            switch (state)
            {
                case ProgramSupportFileState.RequiredPeripheralIncompatible:
                case ProgramSupportFileState.RequiredPeripheralUnknown:
                    stateString = string.Format(StatusMessages[state], Rom.GetTargetDeviceUniqueId());
                    break;
                default:
                    stateString = StatusMessages[state];
                    break;
            }
            return stateString;
        }
    }
}
