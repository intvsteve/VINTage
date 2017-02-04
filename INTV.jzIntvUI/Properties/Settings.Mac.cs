// <copyright file="Settings.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2016 All Rights Reserved
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
#if !__UNIFIED
using MonoMac.Foundation;
#endif
using INTV.Core.ComponentModel;
using INTV.JzIntvUI.ViewModel;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.JzIntvUI.Properties
{
    /// <summary>
    /// Settings for the jzIntv launcher.
    /// </summary>
    public class Settings : PropertyChangedNotifier
    {
        private Settings()
        {
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.EmulatorPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.ExecRomPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.GromRomPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.EcsRomPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.DefaultKeyboardConfigPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.InitialKeyboardMapSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.UseEcsKeymapForECSGamesSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.DisplaySizeSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.DisplayModeSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.MuteAudioSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.EnableMouseSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.EnableIntellivoiceSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.EnableEcsSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.EnableJlpSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.AllowMultipleInstancesSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.Joystick0ConfigSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.Joystick1ConfigSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.Joystick2ConfigSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.Joystick3ConfigSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.ClassicGameController0ConfigPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.ClassicGameController1ConfigPathSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.CommandLineModeSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.AdditionalCommandLineArgumentsSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.CustomCommandLineSettingName, RaisePropertyChanged);
            NSUserDefaultsObserver.AddPreferenceChangedNotification(JzIntvSettingsPageViewModel.UseROMFeatureSettingsWithCustomCommandLineSettingName, RaisePropertyChanged);
        }

        /// <summary>
        /// Gets the default settings object.
        /// </summary>
        public static Settings Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }
        private static Settings _instance;

        private static NSUserDefaults UserDefaults
        {
            get
            {
                if (_userDefaults == null)
                {
                    _userDefaults = NSUserDefaults.StandardUserDefaults;
                    var defaults = new NSMutableDictionary();
                    // Don't assign to NULL directly -- MonoMac crashes. Just leave it alone.
                    ////defaults[JzIntvSettingsPageViewModel.EmulatorPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.ExecRomPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.GromRomPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.EcsRomPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.DefaultKeyboardConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.InitialKeyboardMapSettingName] = (NSString)null;
                    defaults[JzIntvSettingsPageViewModel.UseEcsKeymapForECSGamesSettingName] = new NSNumber(true);
                    ////defaults[JzIntvSettingsPageViewModel.DisplaySizeSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.DisplayModeSettingName] = (NSString)null;
                    defaults[JzIntvSettingsPageViewModel.MuteAudioSettingName] = new NSNumber(false);
                    defaults[JzIntvSettingsPageViewModel.EnableMouseSettingName] = new NSNumber(false);
                    defaults[JzIntvSettingsPageViewModel.AllowMultipleInstancesSettingName] = new NSNumber(false);
                    ////defaults[JzIntvSettingsPageViewModel.EnableIntellivoiceSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.EnableEcsSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.EnableJlpSettingName] = (NSString)null;
                    _userDefaults.RegisterDefaults(defaults);
                    ////defaults[JzIntvSettingsPageViewModel.Joystick0ConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.Joystick1ConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.Joystick2ConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.Joystick3ConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.ClassicGameController0ConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.ClassicGameController1ConfigPathSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.CommandLineMode] =(NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.AdditionalCommandLineArgumentsSettingName] = (NSString)null;
                    ////defaults[JzIntvSettingsPageViewModel.CustomCommandLineSettingName] = (NSString)null;
                    defaults[JzIntvSettingsPageViewModel.UseROMFeatureSettingsWithCustomCommandLineSettingName] = new NSNumber(true);
                }
                return _userDefaults;
            }
        }
        private static NSUserDefaults _userDefaults;

        /// <summary>
        /// Gets or sets the absolute path to the emulator executable.
        /// </summary>
        public string EmulatorPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.EmulatorPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.EmulatorPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets absolute path to the EXEC ROM.
        /// </summary>
        public string ExecRomPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.ExecRomPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.ExecRomPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the absolute path to the GROM ROM.
        /// </summary>
        public string GromRomPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.GromRomPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.GromRomPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the absolute path to the ECS ROM.
        /// </summary>
        public string EcsRomPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.EcsRomPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.EcsRomPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the default keyboard config path.
        /// </summary>
        public string DefaultKeyboardConfigPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.DefaultKeyboardConfigPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.DefaultKeyboardConfigPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the initial keyboard map.
        /// </summary>
        public string InitialKeyboardMap
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.InitialKeyboardMapSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.InitialKeyboardMapSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        public bool UseEcsKeymapForEcsRoms
        {
            get { return UserDefaults.BoolForKey(JzIntvSettingsPageViewModel.UseEcsKeymapForECSGamesSettingName); }
                            set { throw new InvalidOperationException("set UseEcsKeymapForECSGames not supported"); }
        }

        /// <summary>
        /// Gets or sets the display size setting.
        /// </summary>
        public string DisplaySize
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.DisplaySizeSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.DisplaySizeSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the display mode setting.
        /// </summary>
        public string DisplayMode
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.DisplayModeSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.DisplayModeSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Get whether to mute sound in the emulator.
        /// </summary>
        public bool MuteAudio
        {
            get { return UserDefaults.BoolForKey(JzIntvSettingsPageViewModel.MuteAudioSettingName); }
            set { throw new InvalidOperationException("set MuteAudio not supported"); }
        }

        /// <summary>
        /// Get whether to enable mouse in the emulator.
        /// </summary>
        public bool EnableMouse
        {
            get { return UserDefaults.BoolForKey(JzIntvSettingsPageViewModel.EnableMouseSettingName); }
            set { throw new InvalidOperationException("set EnableMouse not supported"); }
        }

        /// <summary>
        /// Gets or sets the Intellivoice emulation policy.
        /// </summary>
        public string EnableIntellivoice
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.EnableIntellivoiceSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.EnableIntellivoiceSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the ECS emulation policy.
        /// </summary>
        public string EnableEcs
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.EnableEcsSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.EnableEcsSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the JLP emulation policy.
        /// </summary>
        public string EnableJlp
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.EnableJlpSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.EnableJlpSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Get whether to allow multiple instances of the emulator to run.
        /// </summary>
        public bool AllowMultipleInstances
        {
            get { return UserDefaults.BoolForKey(JzIntvSettingsPageViewModel.AllowMultipleInstancesSettingName); }
            set { throw new InvalidOperationException("set AllowMultipleInstances not supported"); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 0.
        /// </summary>
        public string Joystick0Config
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.Joystick0ConfigSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.Joystick0ConfigSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 1.
        /// </summary>
        public string Joystick1Config
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.Joystick1ConfigSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.Joystick1ConfigSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 2.
        /// </summary>
        public string Joystick2Config
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.Joystick2ConfigSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.Joystick2ConfigSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 3.
        /// </summary>
        public string Joystick3Config
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.Joystick3ConfigSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.Joystick3ConfigSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the configuration file for CGC 0.
        /// </summary>
        public string ClassicGameController0ConfigPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.ClassicGameController0ConfigPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.ClassicGameController0ConfigPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the configuration file for CGC 1.
        /// </summary>
        public string ClassicGameController1ConfigPath
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.ClassicGameController1ConfigPathSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.ClassicGameController1ConfigPathSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets how to provide command line arguments to jzIntv.
        /// </summary>
        public string CommandLineMode
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.CommandLineModeSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.CommandLineModeSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }
            
        /// <summary>
        /// Gets or sets the additional command line arguments to use for jzIntv.
        /// </summary>
        public string AdditionalCommandLineArguments
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.AdditionalCommandLineArgumentsSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.AdditionalCommandLineArgumentsSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Gets or sets the custom command line to use for jzIntv.
        /// </summary>
        public string CustomCommandLine
        {
            get { return UserDefaults.StringForKey(JzIntvSettingsPageViewModel.CustomCommandLineSettingName); }
            set { UpdateProperty(JzIntvSettingsPageViewModel.CustomCommandLineSettingName, value, string.Empty, (s, v) => UpdateStringSetting(v, s)); }
        }

        /// <summary>
        /// Get whether to override game-defined settings when using a custom command line to launch jzIntv.
        /// </summary>
        public bool UseROMFeatureSettingsWithCustomCommandLine
        {
            get { return UserDefaults.BoolForKey(JzIntvSettingsPageViewModel.UseROMFeatureSettingsWithCustomCommandLineSettingName); }
            set { throw new InvalidOperationException("set UseROMFeatureSettingsWithCustomCommandLine not supported"); }
        }

        private void UpdateStringSetting(string value, string settingName)
        {
            if (string.IsNullOrEmpty(value))
            {
                UserDefaults.RemoveObject(settingName);
            }
            else
            {
                UserDefaults.SetString(value, settingName);
            }
        }
    }
}
