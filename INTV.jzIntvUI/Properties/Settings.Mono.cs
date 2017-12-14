// <copyright file="Settings.Mono.cs" company="INTV Funhouse">
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

namespace INTV.JzIntvUI.Properties
{
    /// <summary>
    /// Mono-specific implementation.
    /// </summary>
    /// <remarks>Various settings using the string data type really could be reworked to expose a proper
    /// enum value. The conversion methods are defined as helpers for the various enum types. The ViewModel
    /// for the settings dialog employs them when working with this type. This is all a side effect of how
    /// these enums were handled on Mac when code was originally written.</remarks>
    internal sealed partial class Settings : INTV.Shared.Properties.SettingsBase<Settings>
    {
        /// <summary>
        /// Gets or sets the absolute path to the emulator executable.
        /// </summary>
        public string EmulatorPath
        {
            get { return GetSetting<string>(EmulatorPathSettingName); }
            set { UpdateProperty(EmulatorPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets absolute path to the EXEC ROM.
        /// </summary>
        public string ExecRomPath
        {
            get { return GetSetting<string>(ExecRomPathSettingName); }
            set { UpdateProperty(ExecRomPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the absolute path to the GROM ROM.
        /// </summary>
        public string GromRomPath
        {
            get { return GetSetting<string>(GromRomPathSettingName); }
            set { UpdateProperty(GromRomPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the absolute path to the ECS ROM.
        /// </summary>
        public string EcsRomPath
        {
            get { return GetSetting<string>(EcsRomPathSettingName); }
            set { UpdateProperty(EcsRomPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the default keyboard config path.
        /// </summary>
        public string DefaultKeyboardConfigPath
        {
            get { return GetSetting<string>(DefaultKeyboardConfigPathSettingName); }
            set { UpdateProperty(DefaultKeyboardConfigPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the initial keyboard map.
        /// </summary>
        public string InitialKeyboardMap
        {
            get { return GetSetting<string>(InitialKeyboardMapSettingName); }
            set { UpdateProperty(InitialKeyboardMapSettingName, value, string.Empty, SetSetting); }
        }

        public bool UseEcsKeymapForEcsRoms
        {
            get { return UserDefaults.BoolForKey(UseEcsKeymapForECSGamesSettingName); }
            set { SetSetting(UseEcsKeymapForECSGamesSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the display size setting.
        /// </summary>
        public string DisplaySize
        {
            get { return GetSetting<string>(DisplaySizeSettingName); }
            set { UpdateProperty(DisplaySizeSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the display mode setting.
        /// </summary>
        public string DisplayMode
        {
            get { return GetSetting<string>(DisplayModeSettingName); }
            set { UpdateProperty(DisplayModeSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to mute sound in the emulator.
        /// </summary>
        public bool MuteAudio
        {
            get { return UserDefaults.BoolForKey(MuteAudioSettingName); }
            set { SetSetting(MuteAudioSettingName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enable mouse in the emulator.
        /// </summary>
        public bool EnableMouse
        {
            get { return UserDefaults.BoolForKey(EnableMouseSettingName); }
            set { SetSetting(EnableMouseSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the Intellivoice emulation policy.
        /// </summary>
        public string EnableIntellivoice
        {
            get { return GetSetting<string>(EnableIntellivoiceSettingName); }
            set { UpdateProperty(EnableIntellivoiceSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the ECS emulation policy.
        /// </summary>
        public string EnableEcs
        {
            get { return GetSetting<string>(EnableEcsSettingName); }
            set { UpdateProperty(EnableEcsSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the JLP emulation policy.
        /// </summary>
        public string EnableJlp
        {
            get { return GetSetting<string>(EnableJlpSettingName); }
            set { UpdateProperty(EnableJlpSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow multiple instances of the emulator to run.
        /// </summary>
        public bool AllowMultipleInstances
        {
            get { return UserDefaults.BoolForKey(AllowMultipleInstancesSettingName); }
            set { SetSetting(AllowMultipleInstancesSettingName, value); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 0.
        /// </summary>
        public string Joystick0Config
        {
            get { return GetSetting<string>(Joystick0ConfigSettingName); }
            set { UpdateProperty(Joystick0ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 1.
        /// </summary>
        public string Joystick1Config
        {
            get { return GetSetting<string>(Joystick1ConfigSettingName); }
            set { UpdateProperty(Joystick1ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 2.
        /// </summary>
        public string Joystick2Config
        {
            get { return GetSetting<string>(Joystick2ConfigSettingName); }
            set { UpdateProperty(Joystick2ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 3.
        /// </summary>
        public string Joystick3Config
        {
            get { return GetSetting<string>(Joystick3ConfigSettingName); }
            set { UpdateProperty(Joystick3ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 4.
        /// </summary>
        public string Joystick4Config
        {
            get { return GetSetting<string>(Joystick4ConfigSettingName); }
            set { UpdateProperty(Joystick4ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 5.
        /// </summary>
        public string Joystick5Config
        {
            get { return GetSetting<string>(Joystick5ConfigSettingName); }
            set { UpdateProperty(Joystick5ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 6.
        /// </summary>
        public string Joystick6Config
        {
            get { return GetSetting<string>(Joystick6ConfigSettingName); }
            set { UpdateProperty(Joystick6ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 7.
        /// </summary>
        public string Joystick7Config
        {
            get { return GetSetting<string>(Joystick7ConfigSettingName); }
            set { UpdateProperty(Joystick7ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 8.
        /// </summary>
        public string Joystick8Config
        {
            get { return GetSetting<string>(Joystick8ConfigSettingName); }
            set { UpdateProperty(Joystick8ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for joystick 9.
        /// </summary>
        public string Joystick9Config
        {
            get { return GetSetting<string>(Joystick9ConfigSettingName); }
            set { UpdateProperty(Joystick9ConfigSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for CGC 0.
        /// </summary>
        public string ClassicGameController0ConfigPath
        {
            get { return GetSetting<string>(ClassicGameController0ConfigPathSettingName); }
            set { UpdateProperty(ClassicGameController0ConfigPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the configuration file for CGC 1.
        /// </summary>
        public string ClassicGameController1ConfigPath
        {
            get { return GetSetting<string>(ClassicGameController1ConfigPathSettingName); }
            set { UpdateProperty(ClassicGameController1ConfigPathSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets how to provide command line arguments to jzIntv.
        /// </summary>
        public string CommandLineMode
        {
            get { return GetSetting<string>(CommandLineModeSettingName); }
            set { UpdateProperty(CommandLineModeSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the additional command line arguments to use for jzIntv.
        /// </summary>
        public string AdditionalCommandLineArguments
        {
            get { return GetSetting<string>(AdditionalCommandLineArgumentsSettingName); }
            set { UpdateProperty(AdditionalCommandLineArgumentsSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets the custom command line to use for jzIntv.
        /// </summary>
        public string CustomCommandLine
        {
            get { return GetSetting<string>(CustomCommandLineSettingName); }
            set { UpdateProperty(CustomCommandLineSettingName, value, string.Empty, SetSetting); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to override game-defined settings when using a custom command line to launch jzIntv.
        /// </summary>
        public bool UseROMFeatureSettingsWithCustomCommandLine
        {
            get { return UserDefaults.BoolForKey(UseROMFeatureSettingsWithCustomCommandLineSettingName); }
            set { SetSetting(UseROMFeatureSettingsWithCustomCommandLineSettingName, value); }
        }

        #region ISettings

        /// <inheritdoc/>
        protected override void InitializeDefaults()
        {
            AddSetting(EmulatorPathSettingName, null);
            AddSetting(ExecRomPathSettingName, null);
            AddSetting(GromRomPathSettingName, null);
            AddSetting(EcsRomPathSettingName, null);
            AddSetting(DefaultKeyboardConfigPathSettingName, null);
            AddSetting(InitialKeyboardMapSettingName, null); // why not use enum here?
            AddSetting(UseEcsKeymapForECSGamesSettingName, true);
            AddSetting(DisplaySizeSettingName, null);
            AddSetting(DisplayModeSettingName, null);
            AddSetting(MuteAudioSettingName, false);
            AddSetting(EnableMouseSettingName, false);
            AddSetting(EnableIntellivoiceSettingName, null); // why not use enum here?
            AddSetting(EnableEcsSettingName, null); // why not use enum here?
            AddSetting(EnableJlpSettingName, null); // why not use enum here?
            AddSetting(AllowMultipleInstancesSettingName, false);

            AddSetting(Joystick0ConfigSettingName, null);
            AddSetting(Joystick1ConfigSettingName, null);
            AddSetting(Joystick2ConfigSettingName, null);
            AddSetting(Joystick3ConfigSettingName, null);
            AddSetting(Joystick4ConfigSettingName, null);
            AddSetting(Joystick5ConfigSettingName, null);
            AddSetting(Joystick6ConfigSettingName, null);
            AddSetting(Joystick7ConfigSettingName, null);
            AddSetting(Joystick8ConfigSettingName, null);
            AddSetting(Joystick9ConfigSettingName, null);

            AddSetting(ClassicGameController0ConfigPathSettingName, null);
            AddSetting(ClassicGameController1ConfigPathSettingName, null);

            AddSetting(CommandLineModeSettingName, null); // why not use enum here?
            AddSetting(AdditionalCommandLineArgumentsSettingName, null);
            AddSetting(CustomCommandLineSettingName, null);
            AddSetting(UseROMFeatureSettingsWithCustomCommandLineSettingName, true);
            OSInitializeDefaults();
        }

        #endregion // ISettings

        /// <summary>
        /// OS-specific default setting initialization.
        /// </summary>
        partial void OSInitializeDefaults();
    }
}
