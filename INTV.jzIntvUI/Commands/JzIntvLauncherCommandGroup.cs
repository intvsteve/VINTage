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
using INTV.Core.Model.Program;
using INTV.JzIntv.Model;
using INTV.JzIntvUI.Model;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model.Program;
using INTV.Shared.Utility;
using INTV.Shared.ViewModel;

namespace INTV.JzIntvUI.Commands
{
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
            Name = "Play", // Resources.Strings.DownloadCommand_Name,
            ContextMenuItemName = "Play in jzIntv", // Resources.Strings.DownloadCommand_ContextMenuItemName,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
//            ToolTip = Resources.Strings.DownloadCommand_TipDescription,
//            ToolTipTitle = Resources.Strings.DownloadCommand_TipTitle,
//            ToolTipDescription = Resources.Strings.DownloadCommand_TipDescription,
//            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
//            Weight = 0.2,
            MenuParent = JzIntvToolsMenuCommand,
            KeyboardShortcutKey = "J",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
        };

        private static void OnLaunch(object parameter)
        {
            var programDescription = GetProgramDescription(parameter);
            if (CanLaunch(programDescription))
            {
                if (!Properties.Settings.Default.AllowMultipleInstances && Emulator.Instances().Any())
                {
                    var message = "Another instance of jzIntv is already running!";
                    INTV.Shared.View.OSMessageBox.Show(message, "jzIntv Error");
                    return;
                }
                var programName = "<Unknown Program>";
                try
                {
                    programName = programDescription.Name;
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

                    var romPath = programDescription.Rom.RomPath;
                    var jzIntvPath = ConfigurationCommandGroup.ResolvePathSetting(Properties.Settings.Default.EmulatorPath);
                    var jzIntv = new Emulator(jzIntvPath, LaunchInJzIntvErrorHandler);
                    var process = RunExternalProgram.CreateProcess(jzIntv.Path);
                    jzIntv.Launch(process, options, programDescription);
                }
                catch (Exception e)
                {
                    var message = string.Format("Unable to launch jzIntv to run '{0}'.\nThe error message was:\n\n{1}", programName, e.Message);
                    INTV.Shared.View.OSMessageBox.Show(message, "Error Launching jzInv");
                }
            }
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

        private static void LaunchInJzIntvErrorHandler(Emulator emulator, string message, Exception exception)
        {
            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(() =>
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        if (exception != null)
                        {
                            message = string.Format("jzIntv is to run '{0}'.\nThe error message was:\n\n{1}", emulator.Rom.Rom.RomPath, exception.Message);
                        }
                        else
                        {
                            message = string.Format("jzIntv is unable to run '{0}'. No further information is available.", emulator.Rom.Rom.RomPath);
                        }
                    }
                    var reportDialog = INTV.Shared.View.ReportDialog.Create("jzInv Error", "An error occurred running jzIntv.");
                    reportDialog.ReportText = message;
                    reportDialog.ShowSendEmailButton = false;
                    reportDialog.ShowDialog("OK");
                });
        }

        #endregion // LaunchInJzIntvCommand

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            if (((target is ProgramDescriptionViewModel) || (target == null)) && (context is RomListViewModel))
            {
                yield return CreateContextMenuCommand(target, LaunchInJzIntvCommand, context);
            }
            //            // A NULL target is allowed for the case of an empty list.
            //            if (((target is INTV.Shared.ViewModel.ProgramDescriptionViewModel) || (target == null)) && (context is INTV.Shared.ViewModel.RomListViewModel))
            //            {
            //                yield return CreateContextMenuCommand(null, DownloadCommand, IntellicartViewModel, null, 0.022);
            //            }
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
