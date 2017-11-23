// <copyright file="Settings.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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
    /// Settings for the jzIntv launcher.
    /// </summary>
    /// <remarks>The "Alt" names are used by Windows, and do not use a prefix to 'namespace' the setting.
    /// We could do the same for the GTK build, as the infrastructure already adds namespacing of a sort,
    /// but that would just be extra work for no compelling reason... unless the cosmetics of settings names
    /// is compelling.</remarks>
    internal sealed partial class Settings
    {
        #region Setting Names

        public const string EmulatorPathSettingName = "EmulatorPath";
        public const string ExecRomPathSettingName = "ExecRomPath";
        public const string GromRomPathSettingName = "GromRomPath";
        public const string EcsRomPathSettingName = "EcsRomPath";
        public const string DefaultKeyboardConfigPathSettingName = "DefaultKeyboardConfigPath";
        public const string InitialKeyboardMapSettingName = "JzIntvInitialKeyboardMap";
        public const string InitialKeyboardMapSettingNameAlt = "InitialKeyboardMap";
        public const string UseEcsKeymapForECSGamesSettingName = "JzIntvUseECSMapForECSRoms";
        public const string UseEcsKeymapForECSGamesSettingNameAlt = "UseEcsKeymapForEcsRoms";
        public const string DisplaySizeSettingName = "JzIntvDisplaySize";
        public const string DisplaySizeSettingNameAlt = "DisplaySize";
        public const string DisplayModeSettingName = "JzIntvDisplayMode";
        public const string DisplayModeSettingNameAlt = "DisplayMode";
        public const string MuteAudioSettingName = "JzIntvMuteAudio";
        public const string MuteAudioSettingNameAlt = "MuteAudio";
        public const string EnableMouseSettingName = "jzIntvEnableMouse";
        public const string EnableMouseSettingNameAlt = "EnableMouse";
        public const string EnableIntellivoiceSettingName = "JzIntvEnableIntellivoice";
        public const string EnableIntellivoiceSettingNameAlt = "EnableIntellivoice";
        public const string EnableEcsSettingName = "JzIntvEnableEcs";
        public const string EnableEcsSettingNameAlt = "EnableEcs";
        public const string EnableJlpSettingName = "jzIntvEnableJlp";
        public const string EnableJlpSettingNameAlt = "EnableJlp";
        public const string AllowMultipleInstancesSettingName = "jzIntvAllowMultipleInstances";
        public const string AllowMultipleInstancesSettingNameAlt = "AllowMultipleInstances";

        public const string Joystick0ConfigSettingName = "JzIntvJoystick0Config";
        public const string Joystick0ConfigSettingNameAlt = "Joystick0Config";
        public const string Joystick1ConfigSettingName = "JzIntvJoystick1Config";
        public const string Joystick1ConfigSettingNameAlt = "Joystick1Config";
        public const string Joystick2ConfigSettingName = "JzIntvJoystick2Config";
        public const string Joystick2ConfigSettingNameAlt = "Joystick2Config";
        public const string Joystick3ConfigSettingName = "JzIntvJoystick3Config";
        public const string Joystick3ConfigSettingNameAlt = "Joystick3Config";
        public const string Joystick4ConfigSettingName = "JzIntvJoystick4Config";
        public const string Joystick4ConfigSettingNameAlt = "Joystick4Config";
        public const string Joystick5ConfigSettingName = "JzIntvJoystick5Config";
        public const string Joystick5ConfigSettingNameAlt = "Joystick5Config";
        public const string Joystick6ConfigSettingName = "JzIntvJoystick6Config";
        public const string Joystick6ConfigSettingNameAlt = "Joystick6Config";
        public const string Joystick7ConfigSettingName = "JzIntvJoystick7Config";
        public const string Joystick7ConfigSettingNameAlt = "Joystick7Config";
        public const string Joystick8ConfigSettingName = "JzIntvJoystick8Config";
        public const string Joystick8ConfigSettingNameAlt = "Joystick8Config";
        public const string Joystick9ConfigSettingName = "JzIntvJoystick9Config";
        public const string Joystick9ConfigSettingNameAlt = "Joystick9Config";

        public const string ClassicGameController0ConfigPathSettingName = "JzIntvCgc0ConfigPath";
        public const string ClassicGameController0ConfigPathSettingNameAlt = "ClassicGameController0ConfigPath";
        public const string ClassicGameController1ConfigPathSettingName = "JzIntvCgc1ConfigPath";
        public const string ClassicGameController1ConfigPathSettingNameAlt = "ClassicGameController1ConfigPath";

        public const string CommandLineModeSettingName = "JzIntvCommandLineMode";
        public const string CommandLineModeSettingNameAlt = "CommandLineMode";
        public const string AdditionalCommandLineArgumentsSettingName = "JzIntvAdditionalCommandLineArguments";
        public const string AdditionalCommandLineArgumentsSettingNameAlt = "AdditionalCommandLineArguments";
        public const string CustomCommandLineSettingName = "JzIntvCustomCommandLine";
        public const string CustomCommandLineSettingNameAlt = "CustomCommandLine";
        public const string UseROMFeatureSettingsWithCustomCommandLineSettingName = "JzIntvUseROMFeatureSettingsWithCustomCommandLine";
        public const string UseROMFeatureSettingsWithCustomCommandLineSettingNameAlt = "UseROMFeatureSettingsWithCustomCommandLine";

        #endregion // Setting Names
    }
}
