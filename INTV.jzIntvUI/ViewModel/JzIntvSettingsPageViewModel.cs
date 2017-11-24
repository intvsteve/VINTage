// <copyright file="JzIntvSettingsPageViewModel.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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
using System.Collections.ObjectModel;
using System.Linq;
using INTV.JzIntv.Model;
using INTV.JzIntvUI.Commands;
using INTV.JzIntvUI.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

#if WIN
using OSColor = System.Windows.Media.Color;
using Settings = INTV.JzIntvUI.Properties.Settings;
using SettingsPageVisualType = INTV.JzIntvUI.View.JzIntvSettingsPage;
#elif MAC
#if __UNIFIED__
using OSColor = AppKit.NSColor;
#else
using OSColor = MonoMac.AppKit.NSColor;
#endif // __UNIFIED__
using Settings = INTV.JzIntvUI.Properties.Settings;
using SettingsPageVisualType = INTV.JzIntvUI.View.JzIntvSettingsPageController;
#endif // WIN

namespace INTV.JzIntvUI.ViewModel
{
    [System.ComponentModel.Composition.Export(typeof(ISettingsPage))]
    [LocalizedName(typeof(Resources.Strings), "SettingsPage_Title")]
    [Weight(0.15)]
    [Icon("Resources/Images/jzIntvUI_32xMD.png")]
    public class JzIntvSettingsPageViewModel : INTV.Shared.ViewModel.SettingsPageViewModel<SettingsPageVisualType>
    {
        public const DisplayResolution DefaultResolution = DisplayResolution.Default;

        public const DisplayMode DefaultMode = DisplayMode.Default;

        #region Property Names

        public const string EnableIntellivoicePropertyName = "EnableIntellivoice";
        public const string EnableEcsPropertyName = "EnableEcs";
        public const string EnableJlpPropertyName = "EnableJlp";
        public const string InitialKeyboardMapPropertyName = "InitialKeyboardMap";
        public const string StatusPropertyName = "Status";
        public const string ConfigurationStatusColorPropertyName = "ConfigurationStatusColor";
        public const string EmulatorPathPropertyName = "EmulatorPath";
        public const string ExecRomPathPropertyName = "ExecRomPath";
        public const string GromRomPathPropertyName = "GromRomPath";
        public const string EcsRomPathPropertyName = "EcsRomPath";
        public const string DefaultKeyboardConfigPathPropertyName = "DefaultKeyboardConfigPath";
        public const string Joystick0ConfigPropertyName = "Joystick0Config";
        public const string Joystick1ConfigPropertyName = "Joystick1Config";
        public const string Joystick2ConfigPropertyName = "Joystick2Config";
        public const string Joystick3ConfigPropertyName = "Joystick3Config";
        public const string Joystick4ConfigPropertyName = "Joystick4Config";
        public const string Joystick5ConfigPropertyName = "Joystick5Config";
        public const string Joystick6ConfigPropertyName = "Joystick6Config";
        public const string Joystick7ConfigPropertyName = "Joystick7Config";
        public const string Joystick8ConfigPropertyName = "Joystick8Config";
        public const string Joystick9ConfigPropertyName = "Joystick9Config";
        public const string JzIntvCgc0ConfigPathPropertyName = "JzIntvCgc0ConfigPath";
        public const string JzIntvCgc1ConfigPathPropertyName = "JzIntvCgc1ConfigPath";
        public const string JzIntvDisplaySizePropertyName = "JzIntvDisplaySize";
        public const string JzIntvDisplayModePropertyName = "JzIntvDisplayMode";
        public const string LastSelectedPageIndexPropertyName = "LastSelectedPageIndex";

        #endregion // Property Names

        #region UI Strings

        public static readonly string EmulatorPathLabel = "jzIntv";

        #region General Tab Strings

        public static readonly string GeneralTabName = Resources.Strings.SettingsPage_GeneralTabName;
        public static readonly string EnableFeatureUseRomSettingName = Resources.Strings.EnableFeature_UseRomSetting;
        public static readonly string EnableFeatureAlways = Resources.Strings.EnableFeature_Always;
        public static readonly string EnableFeatureNever = Resources.Strings.EnableFeature_Never;

        public static readonly string EnableIntellivoiceLabel = Resources.Strings.SettingsPage_General_EnableIntellivoice_Label;
        public static readonly string EnableIntellivoiceTip = Resources.Strings.SettingsPage_General_EnableIntellivoice_Tip;
        public static readonly string EnableEcsLabel = Resources.Strings.SettingsPage_General_EnableECS_Label;
        public static readonly string EnableEcsTip = Resources.Strings.SettingsPage_General_EnableEcs_Tip;
        public static readonly string EnableEcsKeyboardMapLabel = Resources.Strings.SettingsPage_General_EnableECSKeyboardMap_Label;
        public static readonly string EnableEcsKeyboardMapNote = Resources.Strings.SettingsPage_General_EnableECSKeyboardMap_Note;
        public static readonly string JlpFeatures = Resources.Strings.SettingsPage_General_JLPFeatures_Label;
        public static readonly string EnableJlpFeaturesTip = Resources.Strings.SettingsPage_General_EnableJlp_Tip;

        public static readonly string MuteAudioLabel = Resources.Strings.SettingsPage_MuteSound_Label;
        public static readonly string AllowMultipleInstancesLabel = Resources.Strings.SettingsPage_AllowMultipleInstances_Label;

        #endregion // General Tab Strings

        #region Paths Tab Strings

        public static readonly string PathsTabName = Resources.Strings.SettingsPage_PathsTabName;
        public static readonly string JzIntvPathLabel = Resources.Strings.SettingsPage_Paths_JzIntv_Label;
        public static readonly string JzIntvPathHint = Resources.Strings.SettingsPage_Paths_JzIntv_Hint;
        public static readonly string JzIntvBrowseTip = Resources.Strings.SettingsPage_Paths_BrowseForJzIntv_Tip;
        public static readonly string JzIntvResetTip = Resources.Strings.SettingsPage_Paths_ResetJzIntvPath_Tip;

        public static readonly string ExecPathLabel = Resources.Strings.SettingsPage_Paths_Exec_Label;
        public static readonly string ExecBrowseTip = Resources.Strings.SettingsPage_Paths_BrowseForExec_Tip;
        public static readonly string ExecResetTip = Resources.Strings.SettingsPage_Paths_ResetExecPath_Tip;

        public static readonly string GromPathLabel = Resources.Strings.SettingsPage_Paths_Grom_Label;
        public static readonly string GromBrowseTip = Resources.Strings.SettingsPage_Paths_BrowseForGrom_Tip;
        public static readonly string GromResetTip = Resources.Strings.SettingsPage_Paths_ResetGromPath_Tip;

        public static readonly string EcsPathLabel = Resources.Strings.SettingsPage_Paths_Ecs_Label;
        public static readonly string EcsBrowseTip = Resources.Strings.SettingsPage_Paths_BrowseForEcs_Tip;
        public static readonly string EcsResetTip = Resources.Strings.SettingsPage_Paths_ResetEcsPath_Tip;

        public static readonly string RomPathHint = Resources.Strings.SettingsPage_Paths_Rom_Hint;

        #endregion // Paths Tab Strings

        #region Display Tab Strings

        public static readonly string DisplayTabName = Resources.Strings.SettingsPage_DisplayTabName;
        public static readonly string DisplayResolutionLabel = Resources.Strings.SettingsPage_Display_Resolution_Label;
        public static readonly string DisplayResolutionTip = Resources.Strings.SettingsPage_Display_Resolution_Tip;
        public static readonly string DisplayResoutionResetLabel = Resources.Strings.SettingsPage_Display_ResetResolution_Label;
        public static readonly string DisplayResolutionResetTip = Resources.Strings.SettingsPage_Display_ResetResolution_Tip;
        public static readonly string DisplayModeLabel = Resources.Strings.SettingsPage_Display_Mode_Label;
        public static readonly string DisplayModeTip = Resources.Strings.SettingsPage_Display_Mode_Tip;
        public static readonly string DisplayModeWindowedLabel = DisplayMode.Windowed.ToDisplayString();
        public static readonly string DisplayModeFullscreenLabel = DisplayMode.Fullscreen.ToDisplayString();
        public static readonly string EnableMouseLabel = Resources.Strings.SettingsPage_Display_EnableMouse_Label;

        #endregion // Display Tab Strings

        #region Input Tab Strings

        public static readonly string InputTabName = Resources.Strings.SettingsPage_InputTabName;
        public static readonly string KeyboardHackfileLabel = Resources.Strings.SettingsPage_Input_KeyboardHackfilePath_Label;
        public static readonly string KeyboardHackfileTip = Resources.Strings.SettingsPage_Input_KeyboardHackfilePath_BrowseTip;
        public static readonly string KeyboardHackfileResetTip = Resources.Strings.SettingsPage_Input_KeyboardHackfilePath_ClearTip;
        public static readonly string KeyboardHackfileHint = Resources.Strings.SettingsPage_Input_KeyboardHackfilePath_Hint;
        public static readonly string InitialKeyboardMapLabel = Resources.Strings.SettingsPage_Input_InitialKeyboardMap_Label;
        public static readonly string InitialKeyboardMapTip = Resources.Strings.SettingsPage_Input_InitialKeyboardMap_Tip;
        public static readonly string Joystick0ConfigLabel = Resources.Strings.SettingsPage_Input_Joystick0Config_Label;
        public static readonly string Joystick0ConfigHint = Resources.Strings.SettingsPage_Input_Joystick0Config_Hint;
        public static readonly string Joystick1ConfigLabel = Resources.Strings.SettingsPage_Input_Joystick1Config_Label;
        public static readonly string Joystick1ConfigHint = Resources.Strings.SettingsPage_Input_Joystick1Config_Hint;
        public static readonly string Joystick2ConfigLabel = Resources.Strings.SettingsPage_Input_Joystick2Config_Label;
        public static readonly string Joystick2ConfigHint = Resources.Strings.SettingsPage_Input_Joystick2Config_Hint;
        public static readonly string Joystick3ConfigLabel = Resources.Strings.SettingsPage_Input_Joystick3Config_Label;
        public static readonly string Joystick3ConfigHint = Resources.Strings.SettingsPage_Input_Joystick3Config_Hint;
        public static readonly string JoystickConfigTip = Resources.Strings.SettingsPage_Input_JoystickConfigTip;

        #endregion // Input Tag Strings

        #region Advanced Tab Strings

        public static readonly string AdvancedTabName = Resources.Strings.SettingsPage_AdvancedTabName;
        public static readonly string CommandLineModeAutomatic = Resources.Strings.CommandLineMode_Automatic_DisplayName;
        public static readonly string CommandLineModeAutomaticWithAdditional = Resources.Strings.CommandLineMode_AutomaticWithAdditionalArguments_DisplayName;
        public static readonly string AdditionalCommandLineArgsTip = Resources.Strings.SettingsPage_Advanced_AdditionalArguments_Tip;
        public static readonly string AdditionalCommandLineArgsHint = Resources.Strings.SettingsPage_Advanced_AdditionalArguments_Hint;
        public static readonly string CommandLineModeCustom = Resources.Strings.CommandLineMode_Custom_DisplayName;
        public static readonly string CustomCommandLineTip = Resources.Strings.SettingsPage_Advanced_CustomCommandLine_Tip;
        public static readonly string CustomCommandLineHint = Resources.Strings.SettingsPage_Advanced_CustomCommandLine_Hint;
        public static readonly string UseRomSettingsWithCustomCommandLine = Resources.Strings.SettingsPage_Advanced_UseRomSettingsWithCustomCommandLine_Label;
        public static readonly string UseRomSettingsWIthCustomNote = Resources.Strings.SettingsPage_Advanced_UseRomSettingsNote_Label;

        #endregion // Advanced Tab Strings

        #endregion // UI Strings

        public JzIntvSettingsPageViewModel()
        {
            _emulatorPath = JzIntvLauncherConfiguration.Instance.EmulatorPath;
            _execRomPath = Properties.Settings.Default.ExecRomPath;
            _gromRomPath = Properties.Settings.Default.GromRomPath;
            _ecsRomPath = Properties.Settings.Default.EcsRomPath;

            _defaultKeyboardConfigPath = Properties.Settings.Default.DefaultKeyboardConfigPath;
            _initialKeyboardMap = KeyboardMapHelpers.FromSettingString(Properties.Settings.Default.InitialKeyboardMap);
            var maps = new[] { KeyboardMap.Default, KeyboardMap.LeftControllerOnly, KeyboardMap.EcsKeyboard };
            AvailableKeyboardMaps = new ObservableCollection<KeyboardMapViewModel>(maps.Select(m => new KeyboardMapViewModel(m)));
            _selectedKeyboardMapViewModel = AvailableKeyboardMaps.First(m => m.KeyboardMap == _initialKeyboardMap);

            _jzIntvJoystick0Config = Properties.Settings.Default.Joystick0Config;
            _jzIntvJoystick1Config = Properties.Settings.Default.Joystick1Config;
            _jzIntvJoystick2Config = Properties.Settings.Default.Joystick2Config;
            _jzIntvJoystick3Config = Properties.Settings.Default.Joystick3Config;

            _jzIntvCgc0ConfigPath = Properties.Settings.Default.ClassicGameController0ConfigPath;
            _jzIntvCgc1ConfigPath = Properties.Settings.Default.ClassicGameController1ConfigPath;

            var resolutions = Enum.GetValues(typeof(DisplayResolution)).Cast<DisplayResolution>().Distinct();
            AvailableDisplayResolutions = new ObservableCollection<DisplayResolutionViewModel>(resolutions.Select(r => new DisplayResolutionViewModel(r)));
            var resolutionFromSettings = DisplayResolutionHelpers.FromLongCommandLineArgumentString(Properties.Settings.Default.DisplaySize);
            _selectedDisplayResolutionViewModel = AvailableDisplayResolutions.First(r => r.Resolution == resolutionFromSettings);
            _jzIntvDisplaySize = _selectedDisplayResolutionViewModel.DisplayResolution;

            var displayModes = Enum.GetValues(typeof(DisplayMode)).Cast<DisplayMode>().Distinct();
            AvailableDisplayModes = new ObservableCollection<DisplayModeViewModel>(displayModes.Select(m => new DisplayModeViewModel(m)));
            var modeFromSettings = DisplayModeHelpers.FromSettingString(Properties.Settings.Default.DisplayMode);
            _selectedDisplayModeViewModel = AvailableDisplayModes.First(m => m.DisplayMode == modeFromSettings);
            _jzIntvDisplayMode = _selectedDisplayModeViewModel.Mode;

            _enableIntellivoice = EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableIntellivoice);
            _enableEcs = EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableEcs);
            _enableJlp = EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableJlp);

            _commandLineMode = CommandLineModeHelpers.FromSettingsString(Properties.Settings.Default.CommandLineMode);

            if ((_lastSelectedPageIndex < 0) && !ConfigurationCommandGroup.AreRequiredEmulatorPathsValid(false))
            {
                _lastSelectedPageIndex = 1;
            }

            _status = Commands.ConfigurationCommandGroup.GetConfigurationStatus(true);
            UpdateStatusColor();

            Properties.Settings.Default.PropertyChanged += HandleEmulatorSettingsChanged;
        }

        #region Properties

        public int LastSelectedPageIndex
        {
            get
            {
                var lastSelectedPageIndex = _lastSelectedPageIndex;
                if (_lastSelectedPageIndex < 0)
                {
                    lastSelectedPageIndex = ConfigurationCommandGroup.AreRequiredEmulatorPathsValid(false) ? 0 : 1;
                }
                return lastSelectedPageIndex;
            }

            set
            {
                AssignAndUpdateProperty(LastSelectedPageIndexPropertyName, value, ref _lastSelectedPageIndex);
            }
        }
        private static int _lastSelectedPageIndex = -1;

        /// <summary>
        /// Gets or sets the configuration status string.
        /// </summary>
        /// <remarks>The status string is useful to display in the configuration dialog whether the basic required configuration]
        /// for jzIntv is correct.</remarks>
        public string Status
        {
            get { return _status; }
            set { AssignAndUpdateProperty(StatusPropertyName, value, ref _status, (p, v) => UpdateStatusColor()); }
        }
        private string _status;

        public OSColor ConfigurationStatusColor
        {
            get { return _statusColor; }
            set { AssignAndUpdateProperty(ConfigurationStatusColorPropertyName, value, ref _statusColor); }
        }
        private OSColor _statusColor;

        /// <summary>
        /// Gets or sets the absolute path to the emulator executable.
        /// </summary>
        public string EmulatorPath
        {
            get { return _emulatorPath; }
            set { AssignAndUpdateProperty(EmulatorPathPropertyName, value, ref _emulatorPath, (n, v) => JzIntvLauncherConfiguration.Instance.EmulatorPath = v); }
        }
        private string _emulatorPath;

        /// <summary>
        /// Gets or sets absolute path to the EXEC ROM.
        /// </summary>
        public string ExecRomPath
        {
            get { return _execRomPath; }
            set { AssignAndUpdateProperty(ExecRomPathPropertyName, value, ref _execRomPath, (n, v) => Properties.Settings.Default.ExecRomPath = v); }
        }
        private string _execRomPath;

        /// <summary>
        /// Gets or sets the absolute path to the GROM ROM.
        /// </summary>
        public string GromRomPath
        {
            get { return _gromRomPath; }
            set { AssignAndUpdateProperty(GromRomPathPropertyName, value, ref _gromRomPath, (n, v) => Properties.Settings.Default.GromRomPath = v); }
        }
        private string _gromRomPath;

        /// <summary>
        /// Gets or sets the absolute path to the ECS ROM.
        /// </summary>
        public string EcsRomPath
        {
            get { return _ecsRomPath; }
            set { AssignAndUpdateProperty(EcsRomPathPropertyName, value, ref _ecsRomPath, (n, v) => Properties.Settings.Default.EcsRomPath = v); }
        }
        private string _ecsRomPath;

        /// <summary>
        /// Gets or sets the default keyboard hackfile.
        /// </summary>
        public string DefaultKeyboardConfigPath
        {
            get { return _defaultKeyboardConfigPath; }
            set { AssignAndUpdateProperty(DefaultKeyboardConfigPathPropertyName, value, ref _defaultKeyboardConfigPath, (n, v) => Properties.Settings.Default.DefaultKeyboardConfigPath = v); }
        }
        private string _defaultKeyboardConfigPath;

        public KeyboardMap InitialKeyboardMap
        {
            get { return _initialKeyboardMap; }
            set { AssignAndUpdateProperty(InitialKeyboardMapPropertyName, value, ref _initialKeyboardMap, (p, v) => Properties.Settings.Default.InitialKeyboardMap = v == KeyboardMap.Default ? null : v.ToString()); }
        }
        private KeyboardMap _initialKeyboardMap;

        /// <summary>
        /// Gets or sets the selected keyboard map's ViewModel.
        /// </summary>
        /// <remarks>In WPF, weird problems arise with RibbonComboBox unless the ItemsSource and SelectedValue are the same type.
        /// The simple binding for displayed value does not work without adding a bunch of templating in the XAML, which is a royal PITA.
        /// Perhaps writing an IValueConverter would be better.</remarks>
        public KeyboardMapViewModel SelectedKeyboardMapViewModel
        {
            get { return _selectedKeyboardMapViewModel; }
            set { AssignAndUpdateProperty("SelectedKeyboardMapViewModel", value, ref _selectedKeyboardMapViewModel, (p, v) => InitialKeyboardMap = v.KeyboardMap); }
        }
        private KeyboardMapViewModel _selectedKeyboardMapViewModel;

        /// <summary>
        /// Gets the available display resolutions.
        /// </summary>
        public ObservableCollection<KeyboardMapViewModel> AvailableKeyboardMaps { get; private set; }

        /// <summary>
        /// Gets or sets the configuration for joystick 0 (Master Component left controller).
        /// </summary>
        public string JzIntvJoystick0Config
        {
            get { return _jzIntvJoystick0Config; }
            set { AssignAndUpdateProperty(Joystick0ConfigPropertyName, value, ref _jzIntvJoystick0Config, (n, v) => Properties.Settings.Default.Joystick0Config = v); }
        }
        private string _jzIntvJoystick0Config;

        /// <summary>
        /// Gets or sets the configuration for joystick 1 (Master Component right controller).
        /// </summary>
        public string JzIntvJoystick1Config
        {
            get { return _jzIntvJoystick1Config; }
            set { AssignAndUpdateProperty(Joystick1ConfigPropertyName, value, ref _jzIntvJoystick1Config, (n, v) => Properties.Settings.Default.Joystick1Config = v); }
        }
        private string _jzIntvJoystick1Config;

        /// <summary>
        /// Gets or sets the configuration for joystick 2 (ECS left controller).
        /// </summary>
        public string JzIntvJoystick2Config
        {
            get { return _jzIntvJoystick2Config; }
            set { AssignAndUpdateProperty(Joystick2ConfigPropertyName, value, ref _jzIntvJoystick2Config, (n, v) => Properties.Settings.Default.Joystick2Config = v); }
        }
        private string _jzIntvJoystick2Config;

        /// <summary>
        /// Gets or sets the configuration for joystick 3 (ECS right controller).
        /// </summary>
        public string JzIntvJoystick3Config
        {
            get { return _jzIntvJoystick3Config; }
            set { AssignAndUpdateProperty(Joystick3ConfigPropertyName, value, ref _jzIntvJoystick3Config, (n, v) => Properties.Settings.Default.Joystick3Config = v); }
        }
        private string _jzIntvJoystick3Config;

        /// <summary>
        /// Gets or sets the configuration file for CGC 0 (Master Component).
        /// </summary>
        public string JzIntvCgc0ConfigPath
        {
            get { return _jzIntvCgc0ConfigPath; }
            set { AssignAndUpdateProperty(JzIntvCgc0ConfigPathPropertyName, value, ref _jzIntvCgc0ConfigPath, (n, v) => Properties.Settings.Default.ClassicGameController0ConfigPath = v); }
        }
        private string _jzIntvCgc0ConfigPath;

        /// <summary>
        /// Gets or sets the configuration file for CGC 1 (ECS).
        /// </summary>
        public string JzIntvCgc1ConfigPath
        {
            get { return _jzIntvCgc1ConfigPath; }
            set { AssignAndUpdateProperty(JzIntvCgc1ConfigPathPropertyName, value, ref _jzIntvCgc1ConfigPath, (n, v) => Properties.Settings.Default.ClassicGameController1ConfigPath = v); }
        }
        private string _jzIntvCgc1ConfigPath;

        /// <summary>
        /// Gets or sets the selected jzIntv resolution.
        /// </summary>
        [OSExport(JzIntvDisplaySizePropertyName)]
        public string JzIntvDisplaySize
        {
            get { return _jzIntvDisplaySize; }
            set { AssignAndUpdateProperty(JzIntvDisplaySizePropertyName, value, ref _jzIntvDisplaySize); }
        }
        private string _jzIntvDisplaySize;

        /// <summary>
        /// Gets or sets the selected display resolution's ViewModel.
        /// </summary>
        /// <remarks>In WPF, weird problems arise with RibbonComboBox unless the ItemsSource and SelectedValue are the same type.
        /// The simple binding for displayed value does not work without adding a bunch of templating in the XAML, which is a royal PITA.
        /// Perhaps writing an IValueConverter would be better.</remarks>
        public DisplayResolutionViewModel SelectedDisplayResolutionViewModel
        {
            get { return _selectedDisplayResolutionViewModel; }
            set { AssignAndUpdateProperty("SelectedDisplayResolutionViewModel", value, ref _selectedDisplayResolutionViewModel, (p, v) => UpdateDisplayResolution(v == null ? DisplayResolution.Default : v.Resolution)); }
        }
        private DisplayResolutionViewModel _selectedDisplayResolutionViewModel;

        /// <summary>
        /// Gets the available display resolutions.
        /// </summary>
        public ObservableCollection<DisplayResolutionViewModel> AvailableDisplayResolutions { get; private set; }

        /// <summary>
        /// Gets or sets the selected jzIntv resolution.
        /// </summary>
        ////[INTV.Shared.Utility.OSExport(JzIntvDisplayModePropertyName)]
        public string JzIntvDisplayMode
        {
            get { return _jzIntvDisplayMode; }
            set { AssignAndUpdateProperty(JzIntvDisplayModePropertyName, value, ref _jzIntvDisplayMode); }
        }
        private string _jzIntvDisplayMode;

        /// <summary>
        /// Gets or sets the selected display mode's ViewModel.
        /// </summary>
        /// <remarks>In WPF, weird problems arise with RibbonComboBox unless the ItemsSource and SelectedValue are the same type.
        /// The simple binding for displayed value does not work without adding a bunch of templating in the XAML, which is a royal PITA.
        /// Perhaps writing an IValueConverter would be better.</remarks>
        public DisplayModeViewModel SelectedDisplayModeViewModel
        {
            get { return _selectedDisplayModeViewModel; }
            set { AssignAndUpdateProperty("SelectedDisplayModeViewModel", value, ref _selectedDisplayModeViewModel, (p, v) => UpdateDisplayMode(v == null ? DisplayMode.Default : v.DisplayMode)); }
        }
        private DisplayModeViewModel _selectedDisplayModeViewModel;

        /// <summary>
        /// Gets the available display modes.
        /// </summary>
        public ObservableCollection<DisplayModeViewModel> AvailableDisplayModes { get; private set; }

        /// <summary>
        /// Gets or sets the Intellivoice emulation policy.
        /// </summary>
        public EnableFeature EnableIntellivoice
        {
            get { return _enableIntellivoice; }
            set { AssignAndUpdateProperty(EnableIntellivoicePropertyName, value, ref _enableIntellivoice, (p, v) => Properties.Settings.Default.EnableIntellivoice = v.ToString()); }
        }
        private EnableFeature _enableIntellivoice;

        /// <summary>
        /// Gets or sets the ECS emulation policy.
        /// </summary>
        public EnableFeature EnableEcs
        {
            get { return _enableEcs; }
            set { AssignAndUpdateProperty(EnableEcsPropertyName, value, ref _enableEcs, (p, v) => Properties.Settings.Default.EnableEcs = v.ToString()); }
        }
        private EnableFeature _enableEcs;

        /// <summary>
        /// Gets or sets the JLP emulation policy.
        /// </summary>
        public EnableFeature EnableJlp
        {
            get { return _enableJlp; }
            set { AssignAndUpdateProperty(EnableJlpPropertyName, value, ref _enableJlp, (p, v) => Properties.Settings.Default.EnableJlp = v.ToString()); }
        }
        private EnableFeature _enableJlp;

        public CommandLineMode CommandLineMode
        {
            get { return _commandLineMode; }
            set { AssignAndUpdateProperty("CommandLineMode", value, ref _commandLineMode, (p, v) => Properties.Settings.Default.CommandLineMode = v.ToString()); }
        }
        private CommandLineMode _commandLineMode;

        #endregion // Properties

        /// <inheritdoc />
        protected override void RaiseAllPropertiesChanged()
        {
        }

        private void UpdateDisplayResolution(DisplayResolution displayResolution)
        {
            JzIntvDisplaySize = displayResolution.ToLongCommandLineArgumentString();
            Properties.Settings.Default.DisplaySize = displayResolution.ToLongCommandLineArgumentString();
        }

        private void UpdateDisplayMode(DisplayMode displayMode)
        {
            JzIntvDisplayMode = displayMode.ToString();
            Properties.Settings.Default.DisplayMode = displayMode.ToString();
        }

        private void HandleEmulatorSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case Settings.EmulatorPathSettingName:
                    EmulatorPath = JzIntvLauncherConfiguration.Instance.EmulatorPath;
                    break;
                case Settings.ExecRomPathSettingName:
                    ExecRomPath = Properties.Settings.Default.ExecRomPath;
                    break;
                case Settings.GromRomPathSettingName:
                    GromRomPath = Properties.Settings.Default.GromRomPath;
                    break;
                case Settings.EcsRomPathSettingName:
                    EcsRomPath = Properties.Settings.Default.EcsRomPath;
                    break;
                case Settings.DefaultKeyboardConfigPathSettingName:
                    DefaultKeyboardConfigPath = Properties.Settings.Default.DefaultKeyboardConfigPath;
                    break;
                case Settings.Joystick0ConfigSettingName:
                case Settings.Joystick0ConfigSettingNameAlt:
                    JzIntvJoystick0Config = Properties.Settings.Default.Joystick0Config;
                    break;
                case Settings.Joystick1ConfigSettingName:
                case Settings.Joystick1ConfigSettingNameAlt:
                    JzIntvJoystick1Config = Properties.Settings.Default.Joystick1Config;
                    break;
                case Settings.Joystick2ConfigSettingName:
                case Settings.Joystick2ConfigSettingNameAlt:
                    JzIntvJoystick2Config = Properties.Settings.Default.Joystick2Config;
                    break;
                case Settings.Joystick3ConfigSettingName:
                case Settings.Joystick3ConfigSettingNameAlt:
                    JzIntvJoystick3Config = Properties.Settings.Default.Joystick3Config;
                    break;
                case Settings.ClassicGameController0ConfigPathSettingName:
                case Settings.ClassicGameController0ConfigPathSettingNameAlt:
                    JzIntvCgc0ConfigPath = Properties.Settings.Default.ClassicGameController0ConfigPath;
                    break;
                case Settings.ClassicGameController1ConfigPathSettingName:
                case Settings.ClassicGameController1ConfigPathSettingNameAlt:
                    JzIntvCgc1ConfigPath = Properties.Settings.Default.ClassicGameController1ConfigPath;
                    break;
                case Settings.DisplaySizeSettingName:
                case Settings.DisplaySizeSettingNameAlt:
                    var resolutionFromSettings = DisplayResolutionHelpers.FromLongCommandLineArgumentString(Properties.Settings.Default.DisplaySize);
                    SelectedDisplayResolutionViewModel = AvailableDisplayResolutions.First(r => r.Resolution == resolutionFromSettings);
                    break;
                case Settings.DisplayModeSettingName:
                case Settings.DisplayModeSettingNameAlt:
                    var modeFromSettings = DisplayModeHelpers.FromSettingString(Properties.Settings.Default.DisplayMode);
                    SelectedDisplayModeViewModel = AvailableDisplayModes.First(m => m.DisplayMode == modeFromSettings);
                    break;
                case Settings.EnableMouseSettingName:
                case Settings.EnableMouseSettingNameAlt:
                    break;
                case Settings.MuteAudioSettingName:
                case Settings.MuteAudioSettingNameAlt:
                    break;
                case Settings.EnableIntellivoiceSettingName:
                case Settings.EnableIntellivoiceSettingNameAlt:
                    EnableIntellivoice = EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableIntellivoice);
                    break;
                case Settings.EnableEcsSettingName:
                case Settings.EnableEcsSettingNameAlt:
                    EnableEcs = EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableEcs);
                    break;
                case Settings.EnableJlpSettingName:
                case Settings.EnableJlpSettingNameAlt:
                    EnableJlp = EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableJlp);
                    break;
                default:
                    break;
            }
            Status = Commands.ConfigurationCommandGroup.GetConfigurationStatus(true);
        }

        private void UpdateStatusColor()
        {
            var color = INTV.Core.Model.Stic.Color.Black.ToColor();
            if (!ConfigurationCommandGroup.IsEmulatorPathValid() || !ConfigurationCommandGroup.IsExecRomPathvalid(ExecRomPath) || !ConfigurationCommandGroup.IsGromRomPathValid(GromRomPath))
            {
                color = INTV.Core.Model.Stic.Color.Red.ToColor();
            }
            else if (!ConfigurationCommandGroup.IsEcsRomPathValid())
            {
                color = INTV.Core.Model.Stic.Color.Orange.ToColor();
            }
            ConfigurationStatusColor = color;
        }
    }
}
