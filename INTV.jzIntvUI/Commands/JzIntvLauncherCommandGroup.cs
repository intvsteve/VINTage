// <copyright file="jzIntvLauncherCommandGroup.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.JzIntv.Model;
using INTV.JzIntvUI.Model;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

namespace INTV.JzIntvUI.Commands
{
    /// <summary>
    /// Commands for the jzIntv launcher.
    /// </summary>
    public partial class JzIntvLauncherCommandGroup : CommandGroup
    {
        private const string UniqueNameBase = "INTV.JzIntvUI.Commands.JzIntvLauncherCommandGroup";

        private JzIntvLauncherCommandGroup()
            : base(UniqueNameBase, CommandGroupName)
        {
            var roms = INTV.Shared.Model.Program.ProgramCollection.Roms;
            roms.AddInvokeProgramHandler(HandleInvokeProgram, 10); // favor hardware over editor
        }

        public static readonly string CommandGroupName = "jzIntv";

        /// <summary>
        /// The instance of the command group.
        /// </summary>
        internal static readonly JzIntvLauncherCommandGroup Group = new JzIntvLauncherCommandGroup();

        #region JzIntvToolsMenuCommand

        /// <summary>
        /// Grouping command for Intellicart-related commands.
        /// </summary>
        public static readonly VisualRelayCommand JzIntvToolsMenuCommand = new VisualRelayCommand(RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".JzIntvTools",
            Name = CommandGroupName,
            Weight = 0.84,
        };

        #region JzIntvCommandGroupCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualRelayCommand JzIntvCommandGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".JzIntvCommandGroupCommand",
            Name = CommandGroupName,
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/jzIntvUI_32xMD.png"),
            Weight = 0
        };

        #endregion // JzIntvCommandGroupCommand

        #endregion // IntellicartToolsMenuCommand

        #region LaunchInJzIntvCommand

        /// <summary>
        /// Command to run a ROM in jzIntv.
        /// </summary>
        public static readonly VisualRelayCommand LaunchInJzIntvCommand = new VisualRelayCommand(OnLaunch, CanLaunch)
        {
            UniqueId = UniqueNameBase + ".LaunchInJzIntvCommand",
            Name = Resources.Strings.LaunchInJzIntvCommand_Name,
            ContextMenuItemName = Resources.Strings.LaunchInJzIntvCommand_MenuItemName,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
            ToolTip = Resources.Strings.LaunchInJzIntvCommand_TipDescription,
            ToolTipTitle = Resources.Strings.LaunchInJzIntvCommand_TipTitle,
            ToolTipDescription = Resources.Strings.LaunchInJzIntvCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            ////Weight = 0.2,
            KeyboardShortcutKey = "j",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
        };

        private static void OnLaunch(object parameter)
        {
            var programDescription = GetProgramDescription(parameter);
            if (CanLaunch(programDescription))
            {
                if (!Properties.Settings.Default.AllowMultipleInstances && Emulator.Instances().Any())
                {
                    INTV.Shared.View.OSMessageBox.Show(Resources.Strings.JzIntvAlreadyRunning, Resources.Strings.LaunchInJzIntvCommand_Failed_Title);
                    return;
                }
                var programName = Resources.Strings.UnknownROM;
                try
                {
                    programName = programDescription.Name;
                    var options = GetCommandLineOptionsForRom(programDescription);
                    var romPath = programDescription.Rom.RomPath;
                    var jzIntvPath = ConfigurationCommandGroup.ResolvePathSetting(JzIntvLauncherConfiguration.Instance.EmulatorPath);
                    var jzIntv = new Emulator(jzIntvPath, LaunchInJzIntvErrorHandler);
                    var process = RunExternalProgram.CreateProcess(jzIntv.Path);
                    jzIntv.Launch(process, options, programDescription);
                }
                catch (Exception e)
                {
                    var message = string.Format(Resources.Strings.UnableToLaunchJzIntv_Error_Message_Format, programName, e.Message);
                    INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.UnableToLaunchJzIntv_Error_Title);
                }
            }
        }

        private static IDictionary<CommandLineArgument, object> GetCommandLineOptionsForRom(ProgramDescription programDescription)
        {
            var options = new Dictionary<CommandLineArgument, object>();
            var forceSetting = false;
            var commandLineMode = CommandLineModeHelpers.FromSettingsString(Properties.Settings.Default.CommandLineMode);
            var customCommandLine = commandLineMode == CommandLineMode.Custom;
            var useRomSettingsWithCustomCommandLine = customCommandLine && Properties.Settings.Default.UseROMFeatureSettingsWithCustomCommandLine;

            if (customCommandLine && !string.IsNullOrWhiteSpace(Properties.Settings.Default.CustomCommandLine))
            {
                options[CommandLineArgument.Custom] = Properties.Settings.Default.CustomCommandLine;
            }

            // EXEC argument
            if (!customCommandLine && !string.IsNullOrWhiteSpace(Properties.Settings.Default.ExecRomPath) && ConfigurationCommandGroup.IsExecRomPathvalid(Properties.Settings.Default.ExecRomPath))
            {
                var execPath = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.ExecRomPath);
                options[CommandLineArgument.ExecPath] = execPath;
            }

            // GROM argument
            if (!customCommandLine && !string.IsNullOrWhiteSpace(Properties.Settings.Default.GromRomPath) && ConfigurationCommandGroup.IsGromRomPathValid(Properties.Settings.Default.GromRomPath))
            {
                var gromPath = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.GromRomPath);
                options[CommandLineArgument.GromPath] = gromPath;
            }

            // ECS argument
            var enableEcs = false;
            forceSetting = false;
            switch (EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableEcs))
            {
                case EnableFeature.Always:
                    if (!customCommandLine)
                    {
                        enableEcs = true;
                    }
                    break;
                case EnableFeature.UseRomSetting:
                    enableEcs = programDescription.Features.Ecs > EcsFeatures.Tolerates;
                    if (customCommandLine)
                    {
                        enableEcs &= useRomSettingsWithCustomCommandLine;
                    }
                    break;
                case EnableFeature.Never:
                    if (!customCommandLine)
                    {
                        forceSetting = true;
                    }
                    break;
                default:
                    break;
            }
            if (enableEcs || forceSetting)
            {
                if (enableEcs)
                {
                    if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.EcsRomPath) && ConfigurationCommandGroup.IsEcsRomPathValid())
                    {
                        var ecsPath = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.EcsRomPath);
                        options[CommandLineArgument.EcsPath] = ecsPath;
                    }
                }
                options[CommandLineArgument.EnableEcs] = enableEcs;
            }

            // Intellivoice argument
            var enableIntellivoice = false;
            forceSetting = false;
            switch (EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableIntellivoice))
            {
                case EnableFeature.Always:
                    if (!customCommandLine)
                    {
                        enableIntellivoice = true;
                    }
                    break;
                case EnableFeature.UseRomSetting:
                    enableIntellivoice = programDescription.Features.Intellivoice > FeatureCompatibility.Tolerates;
                    if (customCommandLine)
                    {
                        enableIntellivoice &= useRomSettingsWithCustomCommandLine;
                    }
                    break;
                case EnableFeature.Never:
                    if (!customCommandLine)
                    {
                        forceSetting = true;
                    }
                    break;
                default:
                    break;
            }
            if (enableIntellivoice || forceSetting)
            {
                options[CommandLineArgument.EnableIntellivoice] = enableIntellivoice;
            }

            // JLP argument
            var enableJlp = false;
            forceSetting = false;
            switch (EnableFeatureHelpers.FromSettingString(Properties.Settings.Default.EnableJlp))
            {
                case EnableFeature.Always:
                    if (!customCommandLine)
                    {
                        enableJlp = true;
                    }
                    break;
                case EnableFeature.UseRomSetting:
                    enableJlp = programDescription.Features.Jlp > JlpFeatures.Incompatible;
                    if (customCommandLine)
                    {
                        enableJlp &= useRomSettingsWithCustomCommandLine;
                    }
                    break;
                case EnableFeature.Never:
                default:
                    break;
            }
            if (enableJlp)
            {
                options[CommandLineArgument.Jlp] = enableJlp;
                if (enableJlp && (programDescription.Features.JlpFlashMinimumSaveSectors > 0))
                {
                    var jlpSavegame = System.IO.Path.ChangeExtension(programDescription.Rom.RomPath, ProgramFileKind.SaveData.FileExtension());
                    options[CommandLineArgument.JlpSaveGamePath] = jlpSavegame;
                }
            }

            // Locutus argument
            if (!customCommandLine && (programDescription.Rom.Format == INTV.Core.Model.RomFormat.Luigi))
            {
                options[CommandLineArgument.Locutus] = true;
            }

            // Mute argument
            if (!customCommandLine && Properties.Settings.Default.MuteAudio)
            {
                options[CommandLineArgument.AudioRate] = 0;
            }

            // Display mode and resolution arguments
            if (!customCommandLine)
            {
                var displayMode = DisplayMode.Default;
                var displayModeString = Properties.Settings.Default.DisplayMode;
                if (!string.IsNullOrWhiteSpace(displayModeString))
                {
                    displayMode = DisplayModeHelpers.FromSettingString(displayModeString);
                    options[CommandLineArgument.Fullscreen] = displayMode;
                }
                if (displayMode != DisplayMode.Fullscreen)
                {
                    var displaySizeString = Properties.Settings.Default.DisplaySize;
                    if (!string.IsNullOrWhiteSpace(displaySizeString))
                    {
                        options[CommandLineArgument.DisplaySize] = DisplayResolutionHelpers.FromLongCommandLineArgumentString(displaySizeString);
                    }
                }
                else
                {
                    var mainScreenInfo = VisualHelpers.GetPrimaryDisplayInfo();
                    var resolution = string.Format("{0}x{1},{2}bpp", mainScreenInfo.Item1, mainScreenInfo.Item2, mainScreenInfo.Item3);
                    options[CommandLineArgument.DisplaySize] = resolution; 
                }
            }

            // Keyboard hackfile argument
            if (!customCommandLine && ConfigurationCommandGroup.IsPathValid(Properties.Settings.Default.DefaultKeyboardConfigPath))
            {
                var hackfile = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.DefaultKeyboardConfigPath);
                options[CommandLineArgument.KeyboardHackFile] = hackfile;
            }

            // Keyboard map argument
            if (!customCommandLine)
            {
                var keyboardMap = KeyboardMapHelpers.FromSettingString(Properties.Settings.Default.InitialKeyboardMap);
                if (Properties.Settings.Default.UseEcsKeymapForEcsRoms && (programDescription.Features.Ecs > EcsFeatures.Tolerates))
                {
                    keyboardMap = KeyboardMap.EcsKeyboard;
                }
                if (keyboardMap != KeyboardMap.Default)
                {
                    options[CommandLineArgument.KeyboardMap] = keyboardMap;
                }
            }

            // Joystick configuration file arguments
            if (!customCommandLine)
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.Joystick0Config))
                {
                    options[CommandLineArgument.Joystick0Configuration] = Properties.Settings.Default.Joystick0Config;
                }
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.Joystick1Config))
                {
                    options[CommandLineArgument.Joystick1Configuration] = Properties.Settings.Default.Joystick1Config;
                }
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.Joystick2Config))
                {
                    options[CommandLineArgument.Joystick2Configuration] = Properties.Settings.Default.Joystick2Config;
                }
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.Joystick3Config))
                {
                    options[CommandLineArgument.Joystick3Configuration] = Properties.Settings.Default.Joystick3Config;
                }
            }

            // Enable mouse argument
            if (!customCommandLine && Properties.Settings.Default.EnableMouse)
            {
                options[CommandLineArgument.EnableMouse] = true;
            }

            // Classic Game Controller configuration arguments
            if (!customCommandLine)
            {
                if (ConfigurationCommandGroup.IsPathValid(Properties.Settings.Default.ClassicGameController0ConfigPath))
                {
                    var configfile = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.ClassicGameController0ConfigPath);
                    options[CommandLineArgument.ClassicGameControllerMaster] = configfile;
                }
                if (ConfigurationCommandGroup.IsPathValid(Properties.Settings.Default.ClassicGameController1ConfigPath))
                {
                    var configfile = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.ClassicGameController1ConfigPath);
                    options[CommandLineArgument.ClassicGameControllerEcs] = configfile;
                }
            }

            if (!customCommandLine && (commandLineMode == CommandLineMode.AutomaticWithAdditionalArguments) && !string.IsNullOrWhiteSpace(Properties.Settings.Default.AdditionalCommandLineArguments))
            {
                options[CommandLineArgument.AdditionalArguments] = Properties.Settings.Default.AdditionalCommandLineArguments;
            }

            return options;
        }

        private static bool CanLaunch(object parameter)
        {
            var program = GetProgramDescription(parameter);
            var canLaunch = program != null;
            if (canLaunch)
            {
                canLaunch = (program.Rom != null) && program.Rom.IsValid && ConfigurationCommandGroup.AreRequiredEmulatorPathsValid(program.Features.Ecs == EcsFeatures.Requires);
            }
            return canLaunch;
        }

        private static ProgramDescription GetProgramDescription(object parameter)
        {
            var program = parameter as ProgramDescription;
            if (program == null)
            {
                var roms = INTV.Shared.Model.Program.ProgramCollection.Roms;
                if ((roms != null) && roms.SelectionIndexes.Any())
                {
                    var selectedIndex = roms.SelectionIndexes.First();
                    program = roms[selectedIndex];
                }
            }
            return program;
        }

        private static void HandleInvokeProgram(object sender, InvokeProgramEventArgs e)
        {
            if (CanLaunch(e.Program))
            {
                e.Handled = true;
                OnLaunch(e.Program);
            }
        }

        private static void LaunchInJzIntvErrorHandler(Emulator emulator, string message, int exitCode, Exception exception)
        {
            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() =>
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        if (exception != null)
                        {
                            message = string.Format(Resources.Strings.LaunchInJzIntvCommand_Failed_KnownError_Format, emulator.Rom.Rom.RomPath, exception.Message);
                        }
                        else
                        {
                            message = string.Format(Resources.Strings.LaunchInJzIntvCommand_Failed_UnknownError_Format, emulator.Rom.Rom.RomPath);
                        }
                    }
                    var reportDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.LaunchInJzIntvCommand_Failed_Title, Resources.Strings.LaunchInJzIntvCommand_Failed_Message);
                    reportDialog.ReportText = message;
                    reportDialog.ShowSendEmailButton = false;
                    reportDialog.ShowDialog(Resources.Strings.ReportErrorDialogButtonText);
                    OSErrorHandler(emulator, message, exitCode, exception);
                });
        }

        /// <summary>
        /// Additional OS-specific error handler.
        /// </summary>
        /// <param name="emulator">The instance of Emulator that encountered the error.</param>
        /// <param name="message">The error message.</param>
        /// <param name="exitCode">The process exit code returned from the emulator.</param>
        /// <param name="exception">The exception that was raised.</param>
        static partial void OSErrorHandler(Emulator emulator, string message, int exitCode, Exception exception);

        #endregion // LaunchInJzIntvCommand

        #region BrowseAndLaunchInJzIntvCommand

        /// <summary>
        /// The command to browse for a ROM, then run it in jzIntv.
        /// </summary>
        public static readonly VisualRelayCommand BrowseAndLaunchInJzIntvCommand = new VisualRelayCommand(BrowseAndDownload, CanBrowseAndDownload)
        {
            UniqueId = UniqueNameBase + ".BrowseAndLaunchInJzIntvCommand",
            Name = Resources.Strings.BrowseAndLaunchInJzIntvCommand_Name,
            ToolTip = Resources.Strings.BrowseAndLaunchInJzIntvCommand_TipDescription,
            ToolTipTitle = Resources.Strings.BrowseAndLaunchInJzIntvCommand_Name,
            ToolTipDescription = Resources.Strings.BrowseAndLaunchInJzIntvCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/browse_download_play_32xLG.png"),
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/browse_download_play_16xLG.png"),
            ////Weight = 0.21,
            KeyboardShortcutKey = "J",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
        };

        private static void BrowseAndDownload(object parameter)
        {
            if (CanBrowseAndDownload(parameter))
            {
                var selectedFile = INTV.Shared.Model.IRomHelpers.BrowseForRoms(false, Resources.Strings.BrowseAndLaunchInJzIntvCommand_BrowseDialogPrompt).FirstOrDefault();
                if (selectedFile != null)
                {
                    var rom = selectedFile.GetRomFromPath();
                    IProgramDescription programDescription = null;
                    if (rom != null)
                    {
                        var programInfo = rom.GetProgramInformation();
                        programDescription = new ProgramDescription(rom.Crc, rom, programInfo);
                    }
                    if (programDescription != null)
                    {
                        OnLaunch(programDescription);
                    }
                    else
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.BrowseAndLaunchInJzIntvCommand_Failed_MessageFormat, selectedFile);
                        OSMessageBox.Show(message, string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.BrowseAndLaunchInJzIntvCommand_Failed_Title));
                    }
                }
            }
        }

        private static bool CanBrowseAndDownload(object parameter)
        {
            // Since user will browse, we won't know a priori if ECS ROM is needed.
            var canExecute = ConfigurationCommandGroup.AreRequiredEmulatorPathsValid(false);
            return canExecute;
        }

        #endregion // BrowseAndLaunchInJzIntvCommand

        #region ShowJzIntvCommandLineCommand

        /// <summary>
        /// Command to display the command line used to run a particular ROM in jzIntv to the user.
        /// </summary>
        public static readonly VisualRelayCommand ShowJzIntvCommandLineCommand = new VisualRelayCommand(OnShowCommandLine, CanLaunch)
        {
            UniqueId = UniqueNameBase + ".ShowJzIntvCommandLineCommand",
            Name = Resources.Strings.ShowJzIntvCommandLineCommand_Name,
            MenuItemName = Resources.Strings.ShowJzIntvCommandLineCommand_MenuItemName,
            ContextMenuItemName = Resources.Strings.ShowJzIntvCommandLineCommand_ContextMenuItemName,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/show_command_line_16x.png"),
            ToolTip = Resources.Strings.ShowJzIntvCommandLineCommand_TipDescription,
            ToolTipTitle = Resources.Strings.ShowJzIntvCommandLineCommand_TipTitle,
            ToolTipDescription = Resources.Strings.ShowJzIntvCommandLineCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
        };

        private static void OnShowCommandLine(object parameter)
        {
            var programDescription = GetProgramDescription(parameter);
            if (CanLaunch(programDescription))
            {
                var title = string.Format(Resources.Strings.ShowJzIntvCommandLineCommand_DialogTitle_Format, programDescription.Name);
                var message = string.Format(Resources.Strings.ShowJzIntvCommandLineCommand_DialogMessage_Format, programDescription.Name);
                var dialog = ReportDialog.Create(title, message);
                dialog.ShowSendEmailButton = false;
                var options = GetCommandLineOptionsForRom(programDescription);
                var jzIntvPath = ConfigurationCommandGroup.ResolvePathSetting(JzIntvLauncherConfiguration.Instance.EmulatorPath);
                var commandLine = jzIntvPath + ' ' + options.BuildCommandLineArguments(programDescription.Rom.RomPath, false);
                dialog.ReportText = string.Format(Resources.Strings.ShowJzIntvCommandLineCommand_CommandLineMessage_Format, programDescription.Name, commandLine);
                dialog.ShowDialog(Resources.Strings.ReportErrorDialogButtonText);
            }
        }

        #endregion // ShowJzIntvCommandLineCommand

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            if (((target is ProgramDescriptionViewModel) || (target == null)) && (context is RomListViewModel))
            {
                yield return CreateContextMenuCommand(target, LaunchInJzIntvCommand, context);
                yield return CreateContextMenuCommand(target, ShowJzIntvCommandLineCommand, context);
            }
        }

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(JzIntvToolsMenuCommand);
            CommandList.Add(LaunchInJzIntvCommand);
            AddPlatformCommands();
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}
