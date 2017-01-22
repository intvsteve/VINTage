// <copyright file="ConfigurationCommandGroup.cs" company="INTV Funhouse">
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
using INTV.JzIntv.Model;
using INTV.JzIntvUI.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.JzIntvUI.Commands
{
    public partial class ConfigurationCommandGroup : CommandGroup
    {
        private const string UniqueNameBase = "INTV.JzIntvUI.Commands.ConfigurationCommandGroup";

        private ConfigurationCommandGroup()
            : base(UniqueNameBase, "jzIntv")
        {
        }

        /// <summary>
        /// The instance of the command group.
        /// </summary>
        internal static readonly ConfigurationCommandGroup Group = new ConfigurationCommandGroup();

        #region AdvancedCommandGroupCommand

        /// <summary>
        /// Command to act as grouper for various other, specific device commands.
        /// </summary>
        public static readonly VisualRelayCommand AdvancedCommandGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".AdvancedCommandGroupCommand",
            Name = "Advanced",
            SmallIcon = typeof(ConfigurationCommandGroup).LoadImageResource("Resources/Images/settings_16xLG.png"),
            LargeIcon = typeof(ConfigurationCommandGroup).LoadImageResource("Resources/Images/settings_32xMD.png")
        };

        #endregion // AdvancedCommandGroupCommand

        #region OpenSettingsDialogCommand

        /// <summary>
        /// Command to open the settings dialog.
        /// </summary>
        public static readonly VisualRelayCommand OpenSettingsDialogCommand = new VisualRelayCommand(p => INTV.Shared.View.SettingsDialog.ShowSettingsDialog(typeof(JzIntvSettingsPageViewModel).FullName))
        {
            UniqueId = UniqueNameBase + ".OpenSettingsDialogCommand",
            Name = "Settings",
            ToolTip = "Configure jzIntv settings",
            ToolTipTitle = "jzIntv Settings",
            //ToolTipDescription = "Configure jzIntv behaviors",
            //Weight = 0.1,
            SmallIcon = typeof(ConfigurationCommandGroup).LoadImageResource("Resources/Images/settings_16xLG.png"),
            LargeIcon = typeof(ConfigurationCommandGroup).LoadImageResource("Resources/Images/settings_32xMD.png")
        };

        #endregion // OpenSettingsDialogCommand

        #region LocateJzIntvCommand

        /// <summary>
        /// Command to locate the jzIntv emulator.
        /// </summary>
        public static readonly VisualRelayCommand LocateJzIntvCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.JzIntv))
        {
            UniqueId = UniqueNameBase + ".LocateJzIntvCommand",
            Name = "Locate jzIntv",
        };

        #endregion // LocateJzIntvCommand

        #region ResetJzIntvCommand

        /// <summary>
        /// Command to reset the jzIntv emulator location.
        /// </summary>
        public static readonly VisualRelayCommand ResetJzIntvCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.JzIntv))
        {
            UniqueId = UniqueNameBase + ".ResetJzIntvCommand",
            Name = "Reset jzIntv Location",
        };

        #endregion // ResetJzIntvCommand

        #region LocateExecCommand

        /// <summary>
        /// Command to locate the EXEC ROM.
        /// </summary>
        public static readonly VisualRelayCommand LocateExecCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.Exec))
        {
            UniqueId = UniqueNameBase + ".LocateExecCommand",
            Name = "Locate EXEC ROM",
        };

        #endregion // LocateExecCommand

        #region ResetExecCommand

        /// <summary>
        /// Command to reset the EXEC ROM location.
        /// </summary>
        public static readonly VisualRelayCommand ResetExecCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.Exec))
        {
            UniqueId = UniqueNameBase + ".ResetExecCommand",
            Name = "Reset EXEC ROM File Location",
        };

        #endregion // ResetExecCommand

        #region LocateGromCommand

        /// <summary>
        /// Command to locate the GROM ROM.
        /// </summary>
        public static readonly VisualRelayCommand LocateGromCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.Grom))
        {
            UniqueId = UniqueNameBase + ".LocateGromCommand",
            Name = "Locate GROM ROM File",
        };

        #endregion // LocateGromCommand

        #region ResetGromCommand

        /// <summary>
        /// Command to reset the GROM ROM location.
        /// </summary>
        public static readonly VisualRelayCommand ResetGromCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.Grom))
        {
            UniqueId = UniqueNameBase + ".ResetGromCommand",
            Name = "Reset GROM ROM File Location",
        };

        #endregion // ResetGromCommand

        #region LocateEcsCommand

        /// <summary>
        /// Command to locate the ECS ROM.
        /// </summary>
        public static readonly VisualRelayCommand LocateEcsCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.Ecs))
        {
            UniqueId = UniqueNameBase + ".LocateEcsCommand",
            Name = "Locate ECS ROM File",
        };

        #endregion // LocateEcsCommand

        #region ResetEcsCommand

        /// <summary>
        /// Command to reset the ECS ROM location.
        /// </summary>
        public static readonly VisualRelayCommand ResetEcsCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.Ecs))
        {
            UniqueId = UniqueNameBase + ".ResetEcsCommand",
            Name = "Reset ECS ROM File Location", 
        };

        #endregion // ResetEcsCommand

        #region SelectKeyboardConfigFileCommand

        /// <summary>
        /// Command to locate the default keyboard hackfile.
        /// </summary>
        public static readonly VisualRelayCommand SelectKeyboardConfigFileCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.KeyboardConfig))
        {
            UniqueId = UniqueNameBase + ".SelectKeyboardConfigFileCommand",
            Name = "Set Default Keyboard Hackfile",
        };

        #endregion // SelectKeyboardConfigFileCommand

        #region ClearKeyboardConfigFileCommand

        /// <summary>
        /// Command to reset the default keyboard hackfile.
        /// </summary>
        public static readonly VisualRelayCommand ClearKeyboardConfigFileCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.KeyboardConfig))
        {
            UniqueId = UniqueNameBase + ".ClearKeyboardConfigFileCommand",
            Name = "Clear Default Keyboard Hackfile",
        };

        #endregion // ClearKeyboardConfigFileCommand

        #region SelectClassicGameController0ConfigFileCommand

        /// <summary>
        /// Command to locate the jzIntv emulator.
        /// </summary>
        public static readonly VisualRelayCommand SelectClassicGameController0ConfigFileCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.Cgc0Config))
        {
            UniqueId = UniqueNameBase + ".SelectClassicGameController0ConfigFileCommand",
            Name = "Set Classic Game Controller 0 (Master Component) Configuration File",
        };

        #endregion // SelectClassicGameController0ConfigFileCommand

        #region ClearClassicGameController0ConfigFileCommand

        /// <summary>
        /// Command to reset the Classic Game Controller 0 file.
        /// </summary>
        public static readonly VisualRelayCommand ClearClassicGameController0ConfigFileCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.Cgc0Config))
        {
            UniqueId = UniqueNameBase + ".ClearClassicGameController0ConfigFileCommand",
            Name = "Clear Classic Game Controller 0 (Master Component) Configuration File",
        };

        #endregion // ClearClassicGameController0ConfigFileCommand

        #region SelectClassicGameController1ConfigFileCommand

        /// <summary>
        /// Command to locate the jzIntv emulator.
        /// </summary>
        public static readonly VisualRelayCommand SelectClassicGameController1ConfigFileCommand = new VisualRelayCommand(p => OnSelectEmulatorFile(p, EmulatorFile.Cgc1Config))
        {
            UniqueId = UniqueNameBase + ".SelectClassicGameController1ConfigFileCommand",
            Name = "Set Classic Game Controller 1 (ECS) Configuration File",
        };

        #endregion // SelectClassicGameController1ConfigFileCommand

        #region ClearClassicGameController1ConfigFileCommand

        /// <summary>
        /// Command to reset the Classic Game Controller 1 config file.
        /// </summary>
        public static readonly VisualRelayCommand ClearClassicGameController1ConfigFileCommand = new VisualRelayCommand(p => OnClearEmulatorFile(p, EmulatorFile.Cgc1Config))
        {
            UniqueId = UniqueNameBase + ".ClearClassicGameController1ConfigFileCommand",
            Name = "Clear Classic Game Controller 1 (ECS) Configuration File",
        };

        #endregion // ClearClassicGameController1ConfigFileCommand

        /// <summary>
        /// Resolves a path setting to a file system path. A path setting could be stored as a simple file path, or as a URI.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <returns>The path setting to resolve to an absolute file system path.</returns>
        /// <remarks>Should this be moved to PathUtils?</remarks>
        internal static string ResolvePathSetting(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                path = OSResolvePathFromSettings(path);
                Uri appUri = null;
                if (Uri.TryCreate(path, UriKind.Absolute, out appUri))
                {
                    path = appUri.FixUpUriPath();
                }
            }
            return path;
        }

        /// <summary>
        /// Checks to see if the given path is valid.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns><c>true</c>, if path is valid and the file described by the path exists, <c>false</c> otherwise.</returns>
        /// <remarks>NOTE: This path must already be resolved to be a file system path, not a URI / NSUrl representation of a file path.</remarks>
        internal static bool IsPathValid(string path)
        {
            var isValid = !string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path);
            return isValid;
        }

        /// <summary>
        /// Determines whether the path to the emulator is valid.
        /// </summary>
        /// <returns><c>true</c>, if the absolute path to the jzIntv emulator path valid is valid, <c>false</c> otherwise.</returns>
        /// <remarks>NOTE: This method does NOT determine if the path is actually a real jzIntv executable -- only that the file exists.</remarks>
        internal static bool IsEmulatorPathValid()
        {
            var path = ResolvePathSetting(Properties.Settings.Default.EmulatorPath);
            var isValid = IsRomPathValid(path);
            return isValid;
        }

        /// <summary>
        /// Determines whether the path to the EXEC ROM is valid.
        /// </summary>
        /// <param name="path">Absolute path to the EXEC ROM.</param>
        /// <returns><c>true</c>, if EXEC rom path appears to be valid, <c>false</c> otherwise.</returns>
        /// <remarks>This checks only that the file exists, and appears to be a ROM. Note that for .bin format ROMs, that check is not very reliable.</remarks>
        internal static bool IsExecRomPathvalid(string path)
        {
            var execRomPath = ResolvePathSetting(path);
            if (string.IsNullOrEmpty(execRomPath) && IsEmulatorPathValid())
            {
                var emulatorDirectory = System.IO.Path.GetDirectoryName(ResolvePathSetting(Properties.Settings.Default.EmulatorPath));
                execRomPath = System.IO.Path.Combine(emulatorDirectory, "EXEC.bin"); 
            }
            var isValid = IsRomPathValid(execRomPath);
            return isValid;
        }

        /// <summary>
        /// Determines whether the path to the GROM ROM is valid.
        /// </summary>
        /// <param name="path">Absolute path to the GROM ROM.</param>
        /// <returns><c>true</c>, if GROM rom path appears to be valid, <c>false</c> otherwise.</returns>
        /// <remarks>This checks only that the file exists, and appears to be a ROM. Note that for .bin format ROMs, that check is not very reliable.</remarks>
        internal static bool IsGromRomPathValid(string path)
        {
            var gromRomPath = ResolvePathSetting(path);
            if (string.IsNullOrEmpty(gromRomPath) && IsEmulatorPathValid())
            {
                var emulatorDirectory = System.IO.Path.GetDirectoryName(ResolvePathSetting(Properties.Settings.Default.EmulatorPath));
                gromRomPath = System.IO.Path.Combine(emulatorDirectory, "GROM.bin"); 
            }
            var isValid = IsRomPathValid(gromRomPath);
            return isValid;
        }

        /// <summary>
        /// Determines whether the path to the ECS ROM is valid.
        /// </summary>
        /// <param name="path">Absolute path to the ECS ROM.</param>
        /// <returns><c>true</c>, if ECS rom path appears to be valid, <c>false</c> otherwise.</returns>
        /// <remarks>This checks only that the file exists, and appears to be a ROM. Note that for .bin format ROMs, that check is not very reliable.</remarks>
        internal static bool IsEcsRomPathValid()
        {
            var ecsRomPath = ResolvePathSetting(Properties.Settings.Default.EcsRomPath);
            if (string.IsNullOrEmpty(ecsRomPath) && IsEmulatorPathValid())
            {
                var emulatorDirectory = System.IO.Path.GetDirectoryName(ResolvePathSetting(Properties.Settings.Default.EmulatorPath));
                ecsRomPath = System.IO.Path.Combine(emulatorDirectory, "ECS.bin"); 
            }
            var isValid = IsPathValid(ecsRomPath);
            if (isValid)
            {
                // Ensure that this at least appears to be a valid ROM.
                var rom = Rom.Create(ecsRomPath, null);
                isValid = (rom != null) && rom.IsValid && (rom.Format == RomFormat.Bin);
            }
            return isValid;
        }

        /// <summary>
        /// Determines if the given ROM path is valid.
        /// </summary>
        /// <param name="path">Absolute path to the ROM to check.</param>
        /// <returns><c>true</c> if the path is valid and indicates a likely .bin format ROM.</returns>
        internal static bool IsRomPathValid(string path)
        {
            var isValid = IsPathValid(path);
            if (isValid)
            {
                // Ensure that this at least appears to be a valid ROM.
                var rom = Rom.Create(path, null);
                isValid = (rom != null) && rom.IsValid && (rom.Format == RomFormat.Bin);
            }
            return isValid;
        }

        /// <summary>
        /// Determines whether the required jzIntv emulator files are valid.
        /// </summary>
        /// <param name="includeEcsCheck">If set to <c>true</c> include a check for the ECS ROM.</param>
        /// <returns><c>true</c>, if required emulator paths valid are valid, <c>false</c> otherwise.</returns>
        /// <remarks>The EXEC and GROM ROMs MUST be locatable. The ECS ROM will only be strictly required if so indicated.</remarks>
        internal static bool AreRequiredEmulatorPathsValid(bool includeEcsCheck)
        {
            return IsEmulatorPathValid() && IsExecRomPathvalid(Properties.Settings.Default.ExecRomPath) && IsGromRomPathValid(Properties.Settings.Default.GromRomPath) && (!includeEcsCheck || IsEcsRomPathValid());
        }

        /// <summary>
        /// Gets a string describing the general state of the jzIntv configuration.
        /// </summary>
        /// <param name="includeEcsCheck">If set to <c>true</c> include ECS ROM check result in the status.</param>
        /// <returns>A string describing the configuration status.</returns>
        internal static string GetConfigurationStatus(bool includeEcsCheck)
        {
            var message = "jzIntv Configuration: ";

            if (!IsEmulatorPathValid())
            {
                message += "Not configured.";
            }
            else
            {
                var missingFiles = new List<string>();
                if (!IsExecRomPathvalid(Properties.Settings.Default.ExecRomPath))
                {
                    missingFiles.Add("EXEC.bin");
                }
                if (!IsGromRomPathValid(Properties.Settings.Default.GromRomPath))
                {
                    missingFiles.Add("GROM.bin");
                }
                if (includeEcsCheck && !IsEcsRomPathValid())
                {
                    missingFiles.Add("ECS.bin");
                }

                if (missingFiles.Any())
                {
                    message += "Missing: " + string.Join(", ", missingFiles);
                }
                else
                {
                    message += "Ready";
                }
            }
            return message;
        }

        private static void OnSelectEmulatorFile(object parameter, EmulatorFile whichFile)
        {
            var viewModel = parameter as JzIntvSettingsPageViewModel;
            var selectionPrompt = string.Empty;
            switch (whichFile)
            {
                case EmulatorFile.JzIntv:
                    selectionPrompt = "Locate jzIntv";
                    break;
                case EmulatorFile.Exec:
                    selectionPrompt = "Locate EXEC";
                    break;
                case EmulatorFile.Grom:
                    selectionPrompt = "Locate GROM";
                    break;
                case EmulatorFile.Ecs:
                    selectionPrompt = "Locate ECS";
                    break;
                case EmulatorFile.KeyboardConfig:
                    selectionPrompt = "Select a keyboard \"hackfile\"";
                    break;
                case EmulatorFile.Cgc0Config:
                    selectionPrompt = "Select a \"hackfile\" for Classic Game Controller 1 (Master Component)";
                    break;
                case EmulatorFile.Cgc1Config:
                    selectionPrompt = "Select a \"hackfile\" for Classic Game Controller 2 (ECS)";
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(selectionPrompt))
            {
                var browser = FileDialogHelpers.Create();
                browser.EnsureFileExists = true;
                browser.Title = selectionPrompt;
                if ((whichFile == EmulatorFile.JzIntv) && !string.IsNullOrEmpty(PathUtils.ProgramSuffix))
                {
                    browser.AddFilter("jzIntv", new[] { PathUtils.ProgramSuffix });
                }
                var result = browser.ShowDialog();
                if (result == FileBrowserDialogResult.Ok)
                {
                    var path = ResolvePathForSettings(browser.FileNames.First());
                    switch (whichFile)
                    {
                        case EmulatorFile.JzIntv:
                            viewModel.EmulatorPath = path;
                            break;
                        case EmulatorFile.Exec:
                            viewModel.ExecRomPath = path;
                            break;
                        case EmulatorFile.Grom:
                            viewModel.GromRomPath = path;
                            break;
                        case EmulatorFile.Ecs:
                            viewModel.EcsRomPath = path;
                            break;
                        case EmulatorFile.KeyboardConfig:
                            viewModel.DefaultKeyboardConfigPath = path;
                            break;
                        case EmulatorFile.Cgc0Config:
                            viewModel.JzIntvCgc0ConfigPath = path;
                            break;
                        case EmulatorFile.Cgc1Config:
                            viewModel.JzIntvCgc1ConfigPath = path;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                OSMessageBox.Show("Requested to select unrecognized file: " + whichFile, "Unrecognized File Selection");
            }
        }

        private static void OnClearEmulatorFile(object parameter, EmulatorFile whichFile)
        {
            CommandLineArgumentHelpers.CheckCommandLineArgumentStrings();
            var viewModel = parameter as JzIntvSettingsPageViewModel;
            switch (whichFile)
            {
                case EmulatorFile.JzIntv:
                    viewModel.EmulatorPath = null;
                    break;
                case EmulatorFile.Exec:
                    viewModel.ExecRomPath = null;
                    break;
                case EmulatorFile.Grom:
                    viewModel.GromRomPath = null;
                    break;
                case EmulatorFile.Ecs:
                    viewModel.EcsRomPath = null;
                    break;
                case EmulatorFile.KeyboardConfig:
                    viewModel.DefaultKeyboardConfigPath = null;
                    break;
                case EmulatorFile.Cgc0Config:
                    viewModel.JzIntvCgc1ConfigPath = null;
                    break;
                case EmulatorFile.Cgc1Config:
                    viewModel.JzIntvCgc1ConfigPath = null;
                    break;
                default:
                    OSMessageBox.Show("Requested to clear unrecognized emulator file: " + whichFile, "Unrecognized Emulator File");
                    break;
            }
       }

        #region ResetDisplayResolutionCommand

        /// <summary>
        /// Command to reset the ECS ROM location.
        /// </summary>
        public static readonly VisualRelayCommand ResetDisplayResolutionCommand = new VisualRelayCommand(p => Properties.Settings.Default.DisplaySize = null)
        {
            UniqueId = UniqueNameBase + ".ResetDisplayResolutionCommand",
            Name = "Reset Resolution",
        };

        #endregion // ResetDisplayResolutionCommand

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            yield break;
            //            // A NULL target is allowed for the case of an empty list.
            //            if (((target is INTV.Shared.ViewModel.ProgramDescriptionViewModel) || (target == null)) && (context is INTV.Shared.ViewModel.RomListViewModel))
            //            {
            //                yield return CreateContextMenuCommand(null, DownloadCommand, IntellicartViewModel, null, 0.022);
            //            }
        }

        /// <inheritdoc />
        protected override void AddCommands()
        {
            AddPlatformCommands();
        }

        /// <summary>
        /// Adds the platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();

        #endregion // CommandGroup
    }
}
