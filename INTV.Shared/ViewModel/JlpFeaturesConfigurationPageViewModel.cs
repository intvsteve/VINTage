// <copyright file="JlpFeaturesConfigurationPageViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2015-2017 All Rights Reserved
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
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;

#if WIN
using JlpFeaturesVisualType = INTV.Shared.View.JlpFeaturesConfigurationPage;
#elif MAC
using JlpFeaturesVisualType = INTV.Shared.View.JlpFeaturesConfigurationPageController;
#endif // WIN

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// ViewModel for the JLP features configuration page.
    /// </summary>
    [System.ComponentModel.Composition.Export(typeof(IRomFeaturesConfigurationPage))]
    [LocalizedName(typeof(Resources.Strings), "Jlp")]
    [Weight(0.29)]
    [Icon(null)]
    public class JlpFeaturesConfigurationPageViewModel : SettingsPageViewModel<JlpFeaturesVisualType>, IRomFeaturesConfigurationPage
    {
        #region Property Names

        internal const string JlpVersionPropertyName = "JlpVersion";
        internal const string JlpFeaturesConfigurablePropertyName = "JlpFeaturesConfigurable";
        internal const string EnableAcceleratorsAtStartupPropertyName = "EnableAcceleratorsAtStartup";
        internal const string UsesFlashStoragePropertyName = "UsesFlashStorage";
        internal const string MinimumFlashSectorsPropertyName = "MinimumFlashSectors";
        internal const string FlashUsageLevelPropertyName = "FlashUsageLevel";
        internal const string FlashSizeInBytesTipPropertyName = "FlashSizeInBytesTip";
        internal const string CanConfigureFlashStoragePropertyName = "CanConfigureFlashStorage";
        internal const string UsesSerialPortPropertyName = "UsesSerialPort";
        internal const string SerialPortPropertyName = "SerialPort";
        internal const string CanConfigureSerialPortFeaturePropertyName = "CanConfigureSerialPortFeature";
        internal const string CanSelectSerialPortOptionPropertyName = "CanSelectSerialPortOption";
        internal const string UsesLEDsPropertyName = "UsesLEDs";
        internal const string CanConfigureLEDFeaturePropertyName = "CanConfigureLEDFeature";

        #endregion // Property Names

        #region UI Strings

        public static readonly string GroupName = Resources.Strings.JlpFeatures_Name;
        public static readonly string JlpRequiredText = Resources.Strings.JlpFeatures_RequiresJlp;
        public static readonly string JlpVersionText = Resources.Strings.JlpFeatures_MinimumVersion;
        public static readonly string AccelerationText = Resources.Strings.HardwareAccelerationText;
        public static readonly string UsesFlashStorageText = Resources.Strings.JlpFeatures_UsesFlashStorage;
        public static readonly string UsesFlashStorageTipTitle = Resources.Strings.JlpFeatuers_FlashInfoTipTitle;
        public static readonly string UsesSerialPortText = Resources.Strings.JlpFeatures_UsesSerialPort;
        public static readonly string UsesLEDText = Resources.Strings.JlpFeatures_UsesLEDs;

        #endregion // UI Strings

        #region JLP Version Data

        private static readonly Dictionary<JlpHardwareVersion, string> VersionImages = new Dictionary<JlpHardwareVersion, string>()
        {
            { JlpHardwareVersion.Incompatible, "ViewModel/Resources/Images/jlp_incompatible_9xSM.png" },
            { JlpHardwareVersion.None, null },
            { JlpHardwareVersion.Jlp03, "Resources/Images/jlp03_16xLG.png" },
            { JlpHardwareVersion.Jlp04, "Resources/Images/jlp04_16xLG.png" },
            ////{ JlpHardwareVersion.Jlp05, "Resources/Images/jlp05_16xLG.png" }, // not available
        };

        #endregion // JLP Version Data

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the JlpFeaturesConfigurationPageViewModel type.
        /// </summary>
        public JlpFeaturesConfigurationPageViewModel()
        {
            var versions = new List<ProgramFeatureImageViewModel>();
            foreach (var versionImageEntry in VersionImages)
            {
                var image = string.IsNullOrEmpty(versionImageEntry.Value) ? null : typeof(JlpFeaturesConfigurationPageViewModel).LoadImageResource(versionImageEntry.Value);
                var viewModel = new ProgramFeatureImageViewModel(versionImageEntry.Key.ToDisplayString(), image, null, FeatureCategory.Jlp, (uint)versionImageEntry.Key);
                versions.Add(viewModel);
            }
            JlpVersions = new ObservableCollection<ProgramFeatureImageViewModel>(versions);
            var selectableOptions = new[] { JlpFeatures.SerialPortEnhanced, JlpFeatures.SerialPortRequired };
            SerialPortOptions = new ObservableCollection<ProgramFeatureImageViewModel>(selectableOptions.ToFeatureViewModels(JlpFeatureSet.Data));
        }

        #endregion // Constructor

        #region Properties

        #region JLP Hardware Version

        /// <summary>
        /// Gets the JLP version view models.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> JlpVersions { get; private set; }

        /// <summary>
        /// Gets or sets the JLP hardware version required for the game.
        /// </summary>
        public ProgramFeatureImageViewModel JlpVersion
        {
            get { return _jlpVersion; }
            set { AssignAndUpdateProperty(JlpVersionPropertyName, value, ref _jlpVersion, (s, v) => UpdateHardwareVersion((JlpHardwareVersion)v.Flags)); }
        }
        private ProgramFeatureImageViewModel _jlpVersion;

        /// <summary>
        /// Gets a value indicating whether JLP-related features are configurable.
        /// </summary>
        public bool JlpFeaturesConfigurable
        {
            get { return (int)JlpVersion.Flags > 0; }
        }

        #endregion // JLP Hardware Version

        #region JLP RAM and Accelerators

        /// <summary>
        /// Gets or sets a value indicating whether JLP RAM and accelerated instructions are available at program launch.
        /// </summary>
        public bool EnableAcceleratorsAtStartup
        {
            get { return _enableAcceleratorsAtStartup; }
            set { AssignAndUpdateProperty(EnableAcceleratorsAtStartupPropertyName, value, ref _enableAcceleratorsAtStartup, (s, v) => UpdateAcceleration(v)); }
        }
        private bool _enableAcceleratorsAtStartup;

        #endregion // JLP RAM and Accelerators

        #region JLP Flash Configuration

        /// <summary>
        /// Gets or sets a value indicating whether LTO Flash! / JLP onboard flash storage is used.
        /// </summary>
        public bool UsesFlashStorage
        {
            get { return _usesFlashStorage; }
            set { AssignAndUpdateProperty(UsesFlashStoragePropertyName, value, ref _usesFlashStorage, (s, v) => UpdateUsesFlashStorage(v)); }
        }
        private bool _usesFlashStorage;

        /// <summary>
        /// Gets or sets the minimum number of required JLP Flash save sectors.
        /// </summary>
        public ushort MinimumFlashSectors
        {
            get { return _minimumFlashSectors; }
            set { AssignAndUpdateProperty(MinimumFlashSectorsPropertyName, value, ref _minimumFlashSectors, (s, v) => UpdateMinimumFlashSectors(v)); }
        }
        private ushort _minimumFlashSectors;

        /// <summary>
        /// Gets a value indicating in a general sense how much JLP Flash storage is used.
        /// </summary>
        public JlpFlashStorageUsageLevel FlashUsageLevel { get; private set; }

        /// <summary>
        /// Gets the tip strip used to describe JLP Flash storage usage in greater detail.
        /// </summary>
        public string FlashSizeInBytesTip { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance can configure flash storage.
        /// </summary>
        public bool CanConfigureFlashStorage
        {
            get { return JlpFeaturesConfigurable && UsesFlashStorage; }
        }

        #endregion // JLP Flash Configuration

        #region Serial Port Usage

        /// <summary>
        /// Gets or sets a value indicating whether onboard LTO Flash! serial port is used.
        /// </summary>
        public bool UsesSerialPort
        {
            get { return _usesSerialPort; }
            set { AssignAndUpdateProperty(UsesSerialPortPropertyName, value, ref _usesSerialPort, (s, v) => UpdateSerialPortUsage(v)); }
        }
        private bool _usesSerialPort;

        /// <summary>
        /// Gets the available options for LTO Flash! serial port compatibility.
        /// </summary>
        public ObservableCollection<ProgramFeatureImageViewModel> SerialPortOptions { get; private set; }

        /// <summary>
        /// Gets or sets LTO Flash! serial port compatibility.
        /// </summary>
        public ProgramFeatureImageViewModel SerialPort
        {
            get { return _serialPort; }
            set { UpdateFeatureProperty(SerialPortPropertyName, value, ref _serialPort); }
        }
        private ProgramFeatureImageViewModel _serialPort;

        /// <summary>
        /// Gets a value indicating whether or not the serial port feature is used.
        /// </summary>
        public bool CanConfigureSerialPortFeature
        {
            get
            {
                return JlpFeaturesConfigurable && (JlpVersion.Flags >= (uint)JlpHardwareVersion.Jlp04);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the serial port configuration options can be set.
        /// </summary>
        public bool CanSelectSerialPortOption
        {
            get { return CanConfigureSerialPortFeature && UsesSerialPort; }
        }

        #endregion // Serial Port Usage

        #region LED Feature (Unsupported)

        /// <summary>
        /// Gets or sets a value indicating whether the cartridge form of the game uses onboard LEDs.
        /// </summary>
        public bool UsesLEDs
        {
            get { return _usesLEDs; }
            set { AssignAndUpdateProperty(UsesLEDsPropertyName, value, ref _usesLEDs, (s, v) => Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)JlpFeatures.UsesLEDs, v)); }
        }
        private bool _usesLEDs;

        /// <summary>
        /// Gets a value indicating whether the JLP LED feature is configurable.
        /// </summary>
        public bool CanConfigureLEDFeature
        {
            get { return JlpFeaturesConfigurable && (JlpVersion.Flags >= (uint)JlpHardwareVersion.Jlp05); }
        }

        #endregion LED Feature (Unsupported)

        #endregion // Properties

        #region IRomFeaturesConfigurationPage

        /// <inheritdoc />
        [OSExport("Name")]
        public string Name
        {
            get { return Resources.Strings.Jlp; }
        }

        /// <inheritdoc />
        public override void Initialize(ProgramFeatures features)
        {
            base.Initialize(features);
            _jlpVersion = JlpVersions.FirstOrDefault(e => (e != null) && e.Flags == (uint)features.JlpHardwareVersion);
            _enableAcceleratorsAtStartup = features.Jlp.HasFlag(JlpFeatures.Tolerates);
            _usesFlashStorage = features.Jlp.HasFlag(JlpFeatures.SaveDataOptional) || features.Jlp.HasFlag(JlpFeatures.SaveDataRequired);
            _minimumFlashSectors = features.JlpFlashMinimumSaveSectors;
            _usesSerialPort = features.Jlp.HasFlag(JlpFeatures.SerialPortEnhanced) || features.Jlp.HasFlag(JlpFeatures.SerialPortRequired);
            _serialPort = SerialPortOptions.FirstOrDefault(s => UsesSerialPort && (s != null) && features.Jlp.HasFlag((JlpFeatures)s.Flags));
            _usesLEDs = features.Jlp.HasFlag(JlpFeatures.UsesLEDs);
            RaiseAllPropertiesChanged();
            UpdateUsesFlashStorage(_usesFlashStorage);
            UpdateSerialPortUsage(_usesSerialPort);
            UpdateFlashUsageLevel(_minimumFlashSectors);
        }

        #endregion // IRomFeaturesConfigurationPage

        #region SettingsPageViewModel<T>

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
            var propertyNames = new string[]
                {
                    JlpVersionPropertyName,
                    JlpFeaturesConfigurablePropertyName,
                    EnableAcceleratorsAtStartupPropertyName,
                    UsesFlashStoragePropertyName,
                    MinimumFlashSectorsPropertyName,
                    FlashSizeInBytesTipPropertyName,
                    CanConfigureFlashStoragePropertyName,
                    UsesSerialPortPropertyName,
                    SerialPortPropertyName,
                    CanConfigureSerialPortFeaturePropertyName,
                    CanSelectSerialPortOptionPropertyName,
                    UsesLEDsPropertyName,
                    CanConfigureLEDFeaturePropertyName
                };
            foreach (var propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion // SettingsPageViewModel<T>

        private void UpdateHardwareVersion(JlpHardwareVersion version)
        {
            Features.JlpHardwareVersion = version;
            EnableAcceleratorsAtStartup = (version > JlpHardwareVersion.None) && EnableAcceleratorsAtStartup;
            UsesFlashStorage = (version > JlpHardwareVersion.None) && UsesFlashStorage;
            UsesSerialPort = (version > JlpHardwareVersion.Jlp03) && UsesSerialPort;
            if ((version > JlpHardwareVersion.None) && (Features.Jlp == JlpFeatures.Incompatible) && !EnableAcceleratorsAtStartup && !UsesFlashStorage && !UsesSerialPort)
            {
                // If using JLP hardware but haven't specified anything, default to enabling accelerators but no flash.
                EnableAcceleratorsAtStartup = true;
            }
            else if (version <= JlpHardwareVersion.None)
            {
                // If user sets JLP hardware to none or incompatible, be sure to clear JLP bits.
                Features.UpdateFeatureBits(FeatureCategory.Jlp, FeatureCompatibilityHelpers.CompatibilityMask, false);
            }
            RaisePropertyChanged(JlpFeaturesConfigurablePropertyName);
            RaisePropertyChanged(CanConfigureFlashStoragePropertyName);
            RaisePropertyChanged(CanConfigureSerialPortFeaturePropertyName);
            RaisePropertyChanged(CanSelectSerialPortOptionPropertyName);
            RaisePropertyChanged(CanConfigureLEDFeaturePropertyName);
        }

        private void UpdateAcceleration(bool enableAcceleratorsAtStartup)
        {
            var jlpMode = JlpFeatures.Incompatible; // assume none
            if (JlpFeaturesConfigurable)
            {
                // OK... we're going to have non-zero flags. Let's assume basic mode then.
                jlpMode = enableAcceleratorsAtStartup ? JlpFeatures.Tolerates : JlpFeatures.Enhances;
                if (enableAcceleratorsAtStartup && UsesFlashStorage && (MinimumFlashSectors >= MinimumFlashSectors))
                {
                    jlpMode = JlpFeatures.Requires;
                }
            }
            Features.UpdateFeatureBits(FeatureCategory.Jlp, FeatureCompatibilityHelpers.CompatibilityMask, false);
            Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)jlpMode, true);
            if (jlpMode == JlpFeatures.Incompatible)
            {
                EnableAcceleratorsAtStartup = false;
                UsesFlashStorage = false;
                MinimumFlashSectors = 0;
            }
        }

        private void UpdateUsesFlashStorage(bool usesFlashStorage)
        {
            UpdateAcceleration(EnableAcceleratorsAtStartup);
            if (!usesFlashStorage)
            {
                Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)JlpFeaturesHelpers.FlashSaveDataMask, false);
                Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)JlpFeaturesHelpers.FlashSaveDataSectorsCountMask, false);
            }
            else
            {
                UpdateMinimumFlashSectors(MinimumFlashSectors);
            }
            RaisePropertyChanged(CanConfigureFlashStoragePropertyName);
        }

        private void UpdateMinimumFlashSectors(ushort minimumFlashSectors)
        {
            var raiseMinFlashUsageChanged = false;
            if (UsesFlashStorage && (minimumFlashSectors == 0))
            {
                minimumFlashSectors = JlpFeatureSet.DefaultSaveDataSectorCount;
                _minimumFlashSectors = minimumFlashSectors;
                raiseMinFlashUsageChanged = true;
            }
            else if (UsesFlashStorage && (minimumFlashSectors < JlpFeaturesHelpers.MinJlpFlashSectorUsage))
            {
                minimumFlashSectors = JlpFeaturesHelpers.MinJlpFlashSectorUsage;
                _minimumFlashSectors = minimumFlashSectors;
                raiseMinFlashUsageChanged = true;
            }
            Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)JlpFeaturesHelpers.FlashSaveDataMask, false);
            var storageBits = MinimumFlashSectors > 0 ? JlpFeatures.SaveDataRequired : JlpFeatures.SaveDataOptional;
            Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)storageBits, true);
            Features.JlpFlashMinimumSaveSectors = minimumFlashSectors;
            UpdateFlashUsageLevel(minimumFlashSectors);
            if (raiseMinFlashUsageChanged)
            {
                RaisePropertyChanged(MinimumFlashSectorsPropertyName);
            }
        }

        private void UpdateFlashUsageLevel(ushort minimumFlashSectors)
        {
            var flashUsageLevel = minimumFlashSectors > 0 ? JlpFlashStorageUsageLevel.Normal : JlpFlashStorageUsageLevel.None;
            if (minimumFlashSectors > JlpFeaturesHelpers.RecommendedMaxJlpFlashSectorUsage)
            {
                flashUsageLevel = JlpFlashStorageUsageLevel.High;
            }
            if (minimumFlashSectors > JlpFeaturesHelpers.MaxJlpFlashSectorUsage)
            {
                flashUsageLevel = JlpFlashStorageUsageLevel.LtoFlashOnly;
            }
            FlashUsageLevel = flashUsageLevel;
            RaisePropertyChanged(FlashUsageLevelPropertyName);
            UpdateFlashSizeInBytesTip();
        }

        private void UpdateFlashSizeInBytesTip()
        {
            var notes = new List<string>();
            switch (FlashUsageLevel)
            {
                case JlpFlashStorageUsageLevel.None:
                    notes.Add(Resources.Strings.JlpFeatures_FlashInfoTipFlashNotUsed);
                    break;
                case JlpFlashStorageUsageLevel.Normal:
                    notes.Add(GetSectorUsageToolTip());
                    break;
                case JlpFlashStorageUsageLevel.High:
                    notes.Add(GetSectorUsageToolTip());
                    var note = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Strings.JlpFeatures_FlashInfoTipHighFormat,
                        JlpFeaturesHelpers.RecommendedMaxJlpFlashSectorUsage,
                        (uint)JlpFeaturesHelpers.RecommendedMaxJlpFlashSectorUsage.JlpFlashSectorsToKBytes(),
                        JlpFeaturesHelpers.TypicalRomSize / 0x400);
                    notes.Add(note);
                    break;
                case JlpFlashStorageUsageLevel.LtoFlashOnly:
                    notes.Add(GetSectorUsageToolTip());
                    note = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Strings.JlpFeatures_FlashInfoTipLtoFlashOnlyFormat,
                        JlpFeaturesHelpers.MaxJlpFlashSectorUsage,
                        (uint)JlpFeaturesHelpers.MaxJlpFlashSectorUsage.JlpFlashSectorsToKBytes());
                    notes.Add(note);
                    break;
            }
            FlashSizeInBytesTip = string.Join(System.Environment.NewLine + System.Environment.NewLine, notes);
            RaisePropertyChanged(FlashSizeInBytesTipPropertyName);
        }

        private string GetSectorUsageToolTip()
        {
            var sizeInKBytes = MinimumFlashSectors.JlpFlashSectorsToKBytes();
            var toolTip = string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                Resources.Strings.JlpFeatures_FlashInfoTipBaseFormat,
                sizeInKBytes,
                JlpFeaturesHelpers.MinJlpFlashSectorUsage,
                JlpFeatureSet.DefaultSaveDataSectorCount,
                JlpFeaturesHelpers.RecommendedMaxJlpFlashSectorUsage,
                JlpFeaturesHelpers.MaxJlpFlashSectorUsage,
                LtoFlashFeaturesHelpers.MaxFlashSaveDataSectorsCount);
            return toolTip;
        }

        private void UpdateSerialPortUsage(bool usesSerialPort)
        {
            var serialPort = SerialPort;
            if (usesSerialPort && (SerialPort == null))
            {
                SerialPort = SerialPortOptions.First(p => p.Flags == (uint)JlpFeatures.SerialPortEnhanced);
            }
            else if (!usesSerialPort)
            {
                SerialPort = null;
            }
            if (!usesSerialPort)
            {
                Features.UpdateFeatureBits(FeatureCategory.Jlp, (uint)JlpFeaturesHelpers.SerialPortMask, false);
            }
            RaisePropertyChanged(CanConfigureSerialPortFeaturePropertyName);
            RaisePropertyChanged(CanSelectSerialPortOptionPropertyName);
        }
    }
}
