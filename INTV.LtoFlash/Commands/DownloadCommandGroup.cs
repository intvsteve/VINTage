// <copyright file="DownloadCommandGroup.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using INTV.Core.Model;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Model;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Commands to send data to LTO Flash!.
    /// </summary>
    public partial class DownloadCommandGroup : CommandGroup
    {
        /// <summary>
        /// The command group.
        /// </summary>
        internal static readonly DownloadCommandGroup Group = new DownloadCommandGroup();

        private const string UniqueNameBase = "INTV.LtoFlash.Commands.DownloadCommandGroup";

        private DownloadCommandGroup()
            : base(UniqueNameBase, "Home", 0.1)
        {
            TabName = "Home"; // TODO: Change this to use parent visuals? Localize?
        }

        #region FileSystemCommandGroup

        /// <summary>
        /// Pseudo-command for the File System commands group.
        /// </summary>
        public static readonly VisualDeviceCommand FileSystemCommandGroup = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FileSystemCommandGroup",
            Name = Resources.Strings.FileSystemCommandGroup_Name,
            LargeIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/filesystem_32xLG.png"),
            Weight = 0.0
        };

        #endregion // FileSystemCommandGroup

        #region DownloadAndPlayCommand

        /// <summary>
        /// The command to load a program ROM for immediate execution on the Locutus board.
        /// </summary>
        public static readonly VisualDeviceCommand DownloadAndPlayCommand = new VisualDeviceCommand(DownloadAndPlay, CanDownloadAndPlay)
        {
            UniqueId = UniqueNameBase + ".DownloadAndPlayCommand",
            Name = Resources.Strings.DownloadAndPlayCommand_Name,
            MenuItemName = Resources.Strings.DownloadAndPlayCommand_MenuItemName,
            ContextMenuItemName = Resources.Strings.DownloadAndPlayCommand_MenuItemName,
            ToolTip = Resources.Strings.DownloadAndPlayCommand_TipDescription,
            ToolTipTitle = Resources.Strings.DownloadAndPlayCommand_MenuItemName,
            ToolTipDescription = Resources.Strings.DownloadAndPlayCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_32xLG_color.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_16xLG_color.png"),
            KeyboardShortcutKey = "p",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
            Weight = 0,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.DownloadAndPlayProtocolCommands
        };

        /// <summary>
        /// Download and play a program on a Locutus board.
        /// </summary>
        /// <param name="device">The Locutus device to send the program to.</param>
        /// <param name="program">The program to send.</param>
        internal static void DownloadAndPlay(Device device, INTV.Core.Model.Program.ProgramDescription program)
        {
            var rom = program.GetRom();
            var canPlayOnDevice = rom.CanExecuteOnDevice(device.UniqueId);
            if (!canPlayOnDevice)
            {
                var message = string.Format(Resources.Strings.DownloadAndPlayCommand_IncompatibleRom_Message_Format, program.Name);
                canPlayOnDevice = OSMessageBox.Show(message, Resources.Strings.DownloadAndPlayCommand_IncompatibleRom_Title, OSMessageBoxButton.YesNo) == OSMessageBoxResult.Yes;
            }
            if (canPlayOnDevice)
            {
                device.DownloadAndPlay(rom, program.Name, (m, e) => DownloadAndPlayErrorHandler(m, e, program.Name));
            }
        }

        private static void DownloadAndPlay(object parameter)
        {
            // stupid Mac NSToolbarItem doesn't stay disabled correctly (TODO: CHECK IF THIS WAS BUG IN DEVICE STATE)
            if (CanDownloadAndPlay(parameter))
            {
                var ltoFlashViewModel = CompositionHelpers.Container.GetExport<LtoFlashViewModel>().Value;
                var programDescriptionViewModel = parameter as INTV.Shared.ViewModel.ProgramDescriptionViewModel;
                var programViewModel = parameter as ProgramViewModel;

                // If we don't have a valid ROM to send via the parameter, try the current ROM selection.
                // This is to support the main menu / toolbar invocation of the command vs. context menu.
                if ((programDescriptionViewModel == null) && (programViewModel == null))
                {
                    var context = parameter as LtoFlashViewModel ?? ltoFlashViewModel;
                    if (context != null)
                    {
                        // Remember... the LtoFlashViewModel's CurrentSelection is the hacky-ish tracker of
                        // the RomListViewModel's CurrentSelection.
                        programDescriptionViewModel = context.CurrentSelection.FirstOrDefault();
                    }
                }
                if ((programDescriptionViewModel != null) || (programViewModel != null))
                {
                    var program = programDescriptionViewModel == null ? programViewModel.ProgramDescription : programDescriptionViewModel.Model;
                    if ((program != null) && (program.Rom != null) && !program.Rom.Validate())
                    {
                        OSMessageBox.Show(string.Format(Resources.Strings.DownloadAndPlayCommand_Missing_Message_Format, program.Name, program.Rom.RomPath), string.Format(Resources.Strings.DownloadAndPlayCommand_Failed_Title_Format, program.Name));
                    }
                    else
                    {
                        DownloadAndPlay(ltoFlashViewModel.ActiveLtoFlashDevice.Device, program);
                    }
                }
            }
        }

        private static bool CanDownloadAndPlay(object parameter)
        {
            // Stupid XAML designer.
            var canExecute = SingleInstanceApplication.Instance != null;
            if (canExecute)
            {
                var ltoFlashComponent = CompositionHelpers.Container.GetExport<LtoFlashViewModel>();
                var ltoFlashViewModel = ltoFlashComponent == null ? null : ltoFlashComponent.Value;
                canExecute = (ltoFlashViewModel != null) && ltoFlashViewModel.CanExecuteCommand(DownloadAndPlayCommand);
                if (canExecute)
                {
                    canExecute = (ltoFlashViewModel.CurrentSelection != null) && ltoFlashViewModel.CurrentSelection.Any();
                    if (!canExecute)
                    {
                        canExecute = (ltoFlashViewModel.HostPCMenuLayout.CurrentSelection != null) && (ltoFlashViewModel.HostPCMenuLayout.CurrentSelection is ProgramViewModel);
                    }
                }
            }
            return canExecute;
        }

        private static bool DownloadAndPlayErrorHandler(string errorMessage, System.Exception exception, string programName)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadAndPlayCommand_Failed_Message_Format, programName, errorMessage);
            OSMessageBox.Show(
                message,
                string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadAndPlayCommand_Failed_Title_Format, programName),
                SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null,
                (r) => { });
            return true;
        }

        #endregion // DownloadAndPlayCommand

        #region DownloadAndPlayPromptCommand

        /// <summary>
        /// The command to browse for a ROM, then load it for immediate execution on the Locutus board.
        /// </summary>
        public static readonly VisualDeviceCommand DownloadAndPlayPromptCommand = new VisualDeviceCommand(DownloadAndPlayPrompt, CanDownloadAndPlayPrompt)
        {
            UniqueId = UniqueNameBase + ".DownloadAndPlayPromptCommand",
            Name = Resources.Strings.DownloadAndPlayPromptCommand_Name,
            ToolTip = Resources.Strings.DownloadAndPlayPromptCommand_TipDescription,
            ToolTipTitle = Resources.Strings.DownloadAndPlayPromptCommand_Name,
            ToolTipDescription = Resources.Strings.DownloadAndPlayPromptCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_prompt_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/download_play_prompt_16xLG.png"),
            Weight = 0.01,
            KeyboardShortcutKey = "P",
            KeyboardShortcutModifiers = OSModifierKeys.Menu,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.DownloadAndPlayProtocolCommands
        };

        private static void DownloadAndPlayPrompt(object parameter)
        {
            // stupid Mac NSToolbarItem doesn't stay disabled correctly
            if (CanDownloadAndPlayPrompt(parameter))
            {
                var ltoFlashViewModel = parameter as LtoFlashViewModel;
                var selectedFile = INTV.Shared.Model.IRomHelpers.BrowseForRoms(false, Resources.Strings.DownloadAndPlayCommand_BrowseDialogPrompt).FirstOrDefault();
                if (selectedFile != null)
                {
                    var rom = selectedFile.GetRomFromPath();
                    if (rom != null)
                    {
                        var fileName = System.IO.Path.GetFileName(rom.RomPath);
                        var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                        var canPlayOnDevice = rom.CanExecuteOnDevice(device.UniqueId);
                        if (!canPlayOnDevice)
                        {
                            var message = string.Format(Resources.Strings.DownloadAndPlayCommand_IncompatibleRom_Message_Format, fileName);
                            canPlayOnDevice = OSMessageBox.Show(message, Resources.Strings.DownloadAndPlayCommand_IncompatibleRom_Title, OSMessageBoxButton.YesNo) == OSMessageBoxResult.Yes;
                        }
                        if (canPlayOnDevice)
                        {
                            device.DownloadAndPlay(rom, fileName, (m, e) => DownloadAndPlayErrorHandler(m, e, fileName));
                        }
                    }
                    else
                    {
                        var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadAndPlayPromptCommand_Failed_MessageFormat, selectedFile);
                        OSMessageBox.Show(message, string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.DownloadAndPlayPromptCommand_Failed_Title));
                    }
                }
            }
        }

        private static bool CanDownloadAndPlayPrompt(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            bool canExecute = ltoFlashViewModel.CanExecuteCommand(DownloadAndPlayPromptCommand);
            return canExecute;
        }

        #endregion // DownloadAndPlayPromptCommand

        #region SyncHostToDeviceCommand

        /// <summary>
        /// The command to synchronize files in the menu layout to Locutus.
        /// </summary>
        public static readonly VisualDeviceCommand SyncHostToDeviceCommand = new VisualDeviceCommand(SyncHostToDevice, CanSyncHostToDevice)
        {
            UniqueId = UniqueNameBase + ".SyncHostToDeviceCommand",
            Name = Resources.Strings.SyncHostToDeviceCommand_Name,
            MenuItemName = Resources.Strings.SyncHostToDeviceCommand_MenuItemName,
            ToolTip = Resources.Strings.SyncHostToDeviceCommand_TipTitle,
            ToolTipTitle = Resources.Strings.SyncHostToDeviceCommand_Name,
            ToolTipDescription = Resources.Strings.SyncHostToDeviceCommand_TipDescription,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_to_device_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_to_device_16xLG.png"),
            Weight = 0.03,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SyncHostToDeviceProtocolCommands
        };

        private static void SyncHostToDevice(object parameter)
        {
            // stupid Mac NSToolbarItem doesn't stay disabled correctly
            if (CanSyncDeviceToHost(parameter))
            {
                var ltoFlashViewModel = parameter as LtoFlashViewModel;
                var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                var configuration = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
                var menuLayout = ltoFlashViewModel.HostPCMenuLayout.MenuLayout;
                ltoFlashViewModel.HostPCMenuLayout.OverlayText = Resources.Strings.SyncHostToDeviceCommand_OverlayText;
                ltoFlashViewModel.HostPCMenuLayout.ShowOverlay = true;
                device.SyncHostToDevice(menuLayout, (c, p, r) => SyncHostToDeviceCompleteHandler(c, p, (Tuple<FileSystem, IDictionary<string, FailedOperationException>>)r, configuration, ltoFlashViewModel), (m, e) => SyncHostToDeviceErrorHandler(m, e, ltoFlashViewModel));
            }
        }

        private static bool CanSyncHostToDevice(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var canExecute = ltoFlashViewModel.CanExecuteCommand(SyncHostToDeviceCommand);
            var toolTip = Resources.Strings.SyncHostToDeviceCommand_TipTitle;
            if (!canExecute && (ltoFlashViewModel != null))
            {
                var deviceConnected = ltoFlashViewModel.ActiveLtoFlashDevice != null && ltoFlashViewModel.ActiveLtoFlashDevice.IsValid && (ltoFlashViewModel.ActiveLtoFlashDevice.Device != null);
                if (deviceConnected)
                {
                    if (ltoFlashViewModel.ActiveLtoFlashDevice.IsPowerOn)
                    {
                        toolTip += Resources.Strings.SyncHostToDeviceCommand_Unavailable_PowerOn;
                    }
                    else
                    {
                        toolTip += Resources.Strings.SyncHostToDeviceCommand_Unavailable_Generic;
                    }
                }
                else
                {
                    toolTip += Resources.Strings.SyncHostToDeviceCommand_Unavailable_NoDevice;
                }
            }
            SyncHostToDeviceCommand.ToolTip = toolTip;

            return canExecute;
        }

        private static void SyncHostToDeviceCompleteHandler(bool cancelled, bool didShowProgress, Tuple<FileSystem, IDictionary<string, FailedOperationException>> result, Configuration configuration, LtoFlashViewModel ltoFlashViewModel)
        {
            if (!cancelled)
            {
                ltoFlashViewModel.ResetCachedFileSystemsCompareResult();
                var menuLayout = ltoFlashViewModel.HostPCMenuLayout.MenuLayout;
                var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                device.FileSystem = result.Item1;
                menuLayout.Save(configuration.GetMenuLayoutPath(device.UniqueId));
                ltoFlashViewModel.HostPCMenuLayout.ClearItemStates(ltoFlashViewModel.AttachedPeripherals);
            }
            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new System.Action(() => SyncHostToDeviceCompleteDialog(cancelled, didShowProgress, ltoFlashViewModel, result == null ? null : result.Item2)));
        }

        private static void SyncHostToDeviceCompleteDialog(bool cancelled, bool didShowProgress, LtoFlashViewModel ltoFlashViewModel, IDictionary<string, FailedOperationException> warnings)
        {
            ltoFlashViewModel.HostPCMenuLayout.ShowOverlay = false;
            bool showDialog = true;
            if (cancelled && (ltoFlashViewModel.ActiveLtoFlashDevice.Device.FileSystem.Status == LfsDirtyFlags.None))
            {
                showDialog = false;
            }
            if (showDialog)
            {
                var title = cancelled ? Resources.Strings.SyncHostToDeviceCommandCancelled_Title : Resources.Strings.SyncHostToDeviceCommandComplete_Title;
                var message = string.Empty;
                if (cancelled)
                {
                    if (ltoFlashViewModel.ActiveLtoFlashDevice.Device.FileSystem.Status == LfsDirtyFlags.None)
                    {
                        message = Resources.Strings.SyncHostToDeviceCommandCancelledNoChanges_Message;
                    }
                    else
                    {
                        message = Resources.Strings.SyncHostToDeviceCommandCancelled_Message;
                    }
                }
                else
                {
                    message = Resources.Strings.SyncHostToDeviceCommandComplete_Message;
                }
                var result = OSMessageBoxResult.Yes;
                if ((warnings != null) && warnings.Any())
                {
                    var reportDialog = ReportDialog.Create(title, message);
                    reportDialog.ShowSendEmailButton = false;
                    var warningsBuilder = new System.Text.StringBuilder(Resources.Strings.SyncHostToDeviceCommand_WarningMessage).AppendLine().AppendLine();
                    foreach (var warning in warnings)
                    {
                        warningsBuilder.AppendFormat(Resources.Strings.SyncHostToDeviceCommand_WarningFormat, warning.Key, warning.Value.Message).AppendLine().AppendLine();
                    }
                    if (SingleInstanceApplication.SharedSettings.ShowDetailedErrors)
                    {
                        warningsBuilder.AppendLine(Resources.Strings.SyncHostToDeviceCommand_WarningDetailHeader).AppendLine("------------------------------------------------");
                        foreach (var exception in warnings.Values)
                        {
                            warningsBuilder.AppendLine(exception.ToString()).AppendLine();
                        }
                    }
                    reportDialog.ReportText = warningsBuilder.ToString();
                    reportDialog.TextWrapping = OSTextWrapping.Wrap;
                    reportDialog.ShowDialog(Resources.Strings.OK);
                }
                else
                {
                    var buttons = cancelled ? OSMessageBoxButton.YesNo : OSMessageBoxButton.OK;
                    result = OSMessageBox.Show(message, title, buttons);
                }
                if (cancelled && (result == OSMessageBoxResult.Yes) && CanSyncHostToDevicePreview(ltoFlashViewModel))
                {
                    SyncHostToDevicePreview(ltoFlashViewModel);
                }
            }
            ltoFlashViewModel.ActiveLtoFlashDevice.Device.GetFileSystemStatistics(GetFileSystemStatisticsErrorHandler);
        }

        private static bool SyncHostToDeviceErrorHandler(string errorMessage, System.Exception exception, LtoFlashViewModel ltoFlashViewModel)
        {
            ltoFlashViewModel.HostPCMenuLayout.ShowOverlay = false;
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.SyncHostToDeviceCommand_Failed_Message_Format, errorMessage);
            ////OSMessageBox.Show(message, Resources.Strings.SyncHostToDeviceCommand_Failed_Title, (r) => { });
            var reportCompleteDialog = INTV.Shared.View.ReportDialog.Create(Resources.Strings.SyncHostToDeviceCommand_Failed_Title, message);
            if (SingleInstanceApplication.SharedSettings.ShowDetailedErrors)
            {
                reportCompleteDialog.Exception = exception;
            }
            var reportText = new System.Text.StringBuilder(errorMessage);
            reportCompleteDialog.ReportText = reportText.AppendLine().AppendLine().Append(Resources.Strings.SyncHostToDeviceCommand_Failed_Resolution).ToString();
            reportCompleteDialog.TextWrapping = OSTextWrapping.Wrap;
            reportCompleteDialog.BeginInvokeDialog(Resources.Strings.OK, null);
            ltoFlashViewModel.ClearItemStates(ltoFlashViewModel.AttachedPeripherals);
            return true;
        }

        #endregion // SyncHostToDeviceCommand

        #region SyncHostToDevicePreviewCommand

        /// <summary>
        /// The command to preview changes that will be made by the synchronize to LTO Flash! command.
        /// </summary>
        public static readonly VisualDeviceCommand SyncHostToDevicePreviewCommand = new VisualDeviceCommand(SyncHostToDevicePreview, CanSyncHostToDevicePreview)
        {
            UniqueId = UniqueNameBase + ".SyncHostToDevicePreviewCommand",
            Name = Resources.Strings.SyncHostToDevicePreviewCommand_Name,
            ToolTip = Resources.Strings.SyncHostToDevicePreviewCommand_Tip,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_to_device_preview_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_to_device_preview_16xLG.png"),
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.RetrieveFileSystemProtocolCommands
        };

        private static void SyncHostToDevicePreview(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            device.RetrieveFileSystem(Resources.Strings.SyncHostToDevicePreviewCommand_Title, (c, p, r) => SyncHostToDevicePreviewCompleteHandler(ltoFlashViewModel), SyncHostToDevicePreviewErrorHandler);
        }

        private static bool CanSyncHostToDevicePreview(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel.CanExecuteCommand(SyncHostToDevicePreviewCommand);
        }

        private static void SyncHostToDevicePreviewCompleteHandler(LtoFlashViewModel ltoFlashViewModel)
        {
            var fileSystem = ltoFlashViewModel.ActiveLtoFlashDevice.Device.FileSystem;
            ltoFlashViewModel.HostPCMenuLayout.HighlightDifferencesFromDeviceFileSystem(fileSystem, MenuLayoutSynchronizationMode.ToLtoFlash);
        }

        private static bool SyncHostToDevicePreviewErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.SyncPreviewCommand_Failed_Message_Format, errorMessage);
            OSMessageBox.Show(message, Resources.Strings.SyncPreviewCommand_Failed_Title, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, Resources.Strings.SyncPreviewCommand_Failed_Resolution, (r) => { });
            return true;
        }

        #endregion // SyncHostToDevicePreviewCommand

        #region SyncDeviceToHostCommand

        /// <summary>
        /// The command to synchronize files in the menu layout to Locutus.
        /// </summary>
        public static readonly VisualDeviceCommand SyncDeviceToHostCommand = new VisualDeviceCommand(SyncDeviceToHost, CanSyncDeviceToHost)
        {
            UniqueId = UniqueNameBase + ".SyncDeviceToHostCommand",
            Name = Resources.Strings.SyncDeviceToHostCommand_Name,
            MenuItemName = Resources.Strings.SyncDeviceToHostCommand_MenuItemName,
            ToolTip = Resources.Strings.SyncDeviceToHostCommand_Tip,
            ToolTipTitle = Resources.Strings.SyncDeviceToHostCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SyncDeviceToHostCommand_Tip,
            ToolTipIcon = VisualRelayCommand.DefaultToolTipIcon,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_from_device_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_from_device_16xLG.png"),
            Weight = 0.033,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.SyncFromDeviceProtocolCommands
        };

        private static void SyncDeviceToHost(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            SyncDeviceToHost(ltoFlashViewModel, false);
        }

        private static bool CanSyncDeviceToHost(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel.CanExecuteCommand(SyncDeviceToHostCommand);
        }

        private static void SyncDeviceToHost(LtoFlashViewModel ltoFlashViewModel, bool ignoreInconsistentFileSystem)
        {
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            var configuration = INTV.Shared.Utility.SingleInstanceApplication.Instance.GetConfiguration<Configuration>();
            var menuLayout = ltoFlashViewModel.HostPCMenuLayout.MenuLayout;
            ltoFlashViewModel.HostPCMenuLayout.OverlayText = Resources.Strings.SyncDeviceToHostCommand_OverlayText;
            ltoFlashViewModel.HostPCMenuLayout.ShowOverlay = true;
            device.SyncDeviceToHost(menuLayout, ignoreInconsistentFileSystem, (c, p, r) => SyncDeviceToHostCompleteHandler(c, p, (FileSystemSyncErrors)r, configuration, ltoFlashViewModel), (m, e) => SyncDeviceToHostErrorHandler(m, e, ltoFlashViewModel));
        }

        private static void SyncDeviceToHostCompleteHandler(bool cancelled, bool didShowProgress, FileSystemSyncErrors syncErrors, Configuration configuration, LtoFlashViewModel ltoFlashViewModel)
        {
            if (!cancelled && (syncErrors.Data != null))
            {
                ltoFlashViewModel.ResetCachedFileSystemsCompareResult();
                var menuLayout = (MenuLayout)syncErrors.Data;
                ltoFlashViewModel.HostPCMenuLayout.MenuLayout = menuLayout;
                var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                menuLayout.Save(configuration.GetMenuLayoutPath(device.UniqueId));
                ltoFlashViewModel.HostPCMenuLayout.ClearItemStates(ltoFlashViewModel.AttachedPeripherals);
            }
            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new System.Action(() => SyncDeviceToHostCompleteDialog(cancelled, didShowProgress, ltoFlashViewModel, syncErrors)));
        }

        private static void SyncDeviceToHostCompleteDialog(bool cancelled, bool didShowProgress, LtoFlashViewModel ltoFlashViewModel, FileSystemSyncErrors syncErrors)
        {
            ltoFlashViewModel.HostPCMenuLayout.ShowOverlay = ltoFlashViewModel.HostPCMenuLayout.Items.Count == 0;
            bool showDialog = true;
            var title = string.Empty;
            var message = string.Empty;
            var icon = OSMessageBoxIcon.None;

            if (syncErrors.Any)
            {
                icon = OSMessageBoxIcon.Information;
                title = Resources.Strings.SyncDeviceToHostCommandComplete_WarningTitle;
                var briefReport = new System.Text.StringBuilder().AppendLine(Resources.Strings.SyncDeviceToHostCommandComplete_WarningsMessage);
                if (syncErrors.FailedToCreateEntries.Any())
                {
                    briefReport.AppendFormat("  " + Resources.Strings.SyncErrors_MissingEntriesFormat, syncErrors.FailedToCreateEntries.Count).AppendLine();
                }
                if (syncErrors.UnableToRetrieveForks.Any())
                {
                    briefReport.Append("  ").AppendFormat(Resources.Strings.SyncErrors_UnableToRetrieveForksFormat, syncErrors.UnableToRetrieveForks.Count).AppendLine();
                }
                if (syncErrors.OrphanedForks.Any())
                {
                    briefReport.Append("  ").AppendFormat(Resources.Strings.SyncErrors_OrphanedForksFormat, syncErrors.OrphanedForks.Count).AppendLine();
                }
                if (syncErrors.UnsupportedForks.Any())
                {
                    briefReport.Append("  ").AppendFormat(Resources.Strings.SyncErrors_UnsupportedForksFormat, syncErrors.UnsupportedForks.Count).AppendLine();
                    briefReport.AppendLine().AppendLine(Resources.Strings.SyncErrors_UnsupportedForksInfo);
                }
                briefReport.AppendLine().AppendLine(Resources.Strings.SyncDeviceToHostCommandComplete_DireWarning);
                message = briefReport.ToString();
                syncErrors.RecordErrors(Configuration.Instance.ErrorLogDirectory, "SYNC_LTOFLASH_TO_COMPUTER", "SyncDeviceToHost" + PathUtils.GetTimeString() + ".txt");
            }
            else
            {
                if (cancelled && (ltoFlashViewModel.ActiveLtoFlashDevice.Device.FileSystem.Status == LfsDirtyFlags.None))
                {
                    showDialog = false;
                }
                else if (!cancelled && !didShowProgress)
                {
                    showDialog = false;
                }
                if (showDialog)
                {
                    title = cancelled ? Resources.Strings.SyncDeviceToHostCommandCancelled_Title : Resources.Strings.SyncDeviceToHostCommandComplete_Title;
                    if (cancelled)
                    {
                        message = Resources.Strings.SyncDeviceToHostCommandCancelled_Message;
                    }
                    else
                    {
                        message = Resources.Strings.SyncDeviceToHostCommandComplete_Message;
                    }
                }
            }
            if (showDialog)
            {
                OSMessageBox.Show(message, title, OSMessageBoxButton.OK, icon);
            }
            ltoFlashViewModel.ActiveLtoFlashDevice.Device.GetFileSystemStatistics(GetFileSystemStatisticsErrorHandler);
        }

        private static bool SyncDeviceToHostErrorHandler(string errorMessage, System.Exception exception, LtoFlashViewModel viewModel)
        {
            viewModel.HostPCMenuLayout.ShowOverlay = false;
            var showInconsistentFileInfo = ((exception != null) && (exception is InconsistentFileSystemException)) || (exception.InnerException is InconsistentFileSystemException);
            if (showInconsistentFileInfo)
            {
                OSMessageBox.Show(
                    Resources.Strings.DeviceMultistageCommand_UpdatingMenuLayout_InconsistentStateErrorMessage,
                    Resources.Strings.FileSystem_Inconsistent_Title,
                    null,
                    OSMessageBoxButton.YesNo,
                    OSMessageBoxIcon.Exclamation,
                    (result) =>
                    {
                        if (result == OSMessageBoxResult.Yes)
                        {
                            SingleInstanceApplication.MainThreadDispatcher.BeginInvoke(new System.Action(() => SyncDeviceToHost(viewModel, true)));
                        }
                    });
            }
            else
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.SyncDeviceToHostCommand_Failed_Message_Format, errorMessage);
                OSMessageBox.Show(message, Resources.Strings.SyncDeviceToHostCommand_Failed_Title, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, (r) => { });
            }
            return true;
        }

        #endregion // SyncDeviceToHostCommand

        #region SyncDeviceToHostPreviewCommand

        /// <summary>
        /// The command to preview changes that will be made by the synchronize to LTO Flash! command.
        /// </summary>
        public static readonly VisualDeviceCommand SyncDeviceToHostPreviewCommand = new VisualDeviceCommand(SyncDeviceToHostPreview, CanSyncHostToDevicePreview)
        {
            UniqueId = UniqueNameBase + ".SyncDeviceToHostPreviewCommand",
            Name = Resources.Strings.SyncDeviceToHostPreviewCommand_Name,
            ToolTip = Resources.Strings.SyncDeviceToHostPreviewCommand_Tip,
            LargeIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_from_device_preview_32xLG.png"),
            SmallIcon = typeof(DeviceCommandGroup).LoadImageResource("Resources/Images/sync_from_device_preview_16xLG.png"),
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.RetrieveFileSystemProtocolCommands
        };

        private static void SyncDeviceToHostPreview(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
            device.RetrieveFileSystem(Resources.Strings.SyncDeviceToHostPreviewCommand_Title, (c, p, r) => SyncDeviceToHostPreviewCompleteHandler(ltoFlashViewModel), SyncDeviceToHostPreviewErrorHandler);
        }

        private static bool CanSyncDeviceToHostPreview(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel.CanExecuteCommand(SyncDeviceToHostPreviewCommand);
        }

        private static void SyncDeviceToHostPreviewCompleteHandler(LtoFlashViewModel ltoFlashViewModel)
        {
            var fileSystem = ltoFlashViewModel.ActiveLtoFlashDevice.Device.FileSystem;
            ltoFlashViewModel.HostPCMenuLayout.HighlightDifferencesFromDeviceFileSystem(fileSystem, MenuLayoutSynchronizationMode.FromLtoFlash);
        }

        private static bool SyncDeviceToHostPreviewErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.SyncPreviewCommand_Failed_Message_Format, errorMessage);
            OSMessageBox.Show(message, Resources.Strings.SyncPreviewCommand_Failed_Title, SingleInstanceApplication.SharedSettings.ShowDetailedErrors ? exception : null, Resources.Strings.SyncPreviewCommand_Failed_Resolution, (r) => { });
            return true;
        }

        #endregion // SyncDeviceToHostPreviewCommand

        #region SyncClearChangesPreviewCommand

        /// <summary>
        /// The command to clear the changes preview.
        /// </summary>
        public static readonly VisualDeviceCommand SyncClearChangesPreviewCommand = new VisualDeviceCommand(SyncClearChangesPreview, CanSyncClearChangesPreview)
        {
            UniqueId = UniqueNameBase + ".SyncClearChangesPreviewCommand",
            Name = Resources.Strings.SyncClearPreviewChangesCommand_Name,
            ToolTip = Resources.Strings.SyncClearPreviewChangesCommand_Tip,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = Enumerable.Empty<INTV.LtoFlash.Model.Commands.ProtocolCommandId>()
        };

        private static void SyncClearChangesPreview(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            ltoFlashViewModel.HostPCMenuLayout.ClearItemStates(ltoFlashViewModel.AttachedPeripherals);
        }

        private static bool CanSyncClearChangesPreview(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel != null; // ltoFlashViewModel.CanExecuteCommand(SyncClearChangesPreviewCommand);
        }

        #endregion // SyncClearChangesPreviewCommand

        /// <summary>
        /// Error reporting function to use if a problem occurs getting file system statistics.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <returns><c>true</c> if the error should be considered handled. This function always returns <c>true</c>.</returns>
        internal static bool GetFileSystemStatisticsErrorHandler(string errorMessage, System.Exception exception)
        {
            var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.GetFileSystemStatisticsCommand_Failed_Message_Format, errorMessage);
            INTV.Shared.View.OSMessageBox.Show(message, Resources.Strings.GetFileSystemStatisticsCommand_Failed_Title);
            return true;
        }

        #region CommandGroup

        /// <inheritdoc/>
        public override IEnumerable<ICommand> CreateContextMenuCommands(object target, object context)
        {
            // A NULL target is allowed for the case of an empty list.
            if (((target is INTV.Shared.ViewModel.ProgramDescriptionViewModel) || (target == null)) && (context is INTV.Shared.ViewModel.RomListViewModel))
            {
                yield return CreateContextMenuCommand(target, DownloadAndPlayCommand, context);
            }
            else if (((target is FileNodeViewModel) || (target == null)) && (context is MenuLayoutViewModel))
            {
                yield return CreateContextMenuCommand(target, DownloadAndPlayCommand, context);
            }
        }

        /// <inheritdoc/>
        protected override object GetContextForCommand(object target, ICommand command, object context)
        {
            var relayCommand = command as RelayCommand;
            if (relayCommand != null)
            {
                if (relayCommand.UniqueId == DownloadAndPlayCommand.UniqueId)
                {
                    return target;
                }
            }
            return base.GetContextForCommand(target, command, context);
        }

        /// <inheritdoc/>
        protected override void AddCommands()
        {
            CommandList.Add(DownloadAndPlayCommand);
            CommandList.Add(DownloadAndPlayPromptCommand);
            CommandList.Add(SyncHostToDeviceCommand);
            CommandList.Add(SyncDeviceToHostCommand);
            CommandList.Add(SyncHostToDevicePreviewCommand);
            CommandList.Add(SyncDeviceToHostPreviewCommand);
            CommandList.Add(SyncClearChangesPreviewCommand);
            AddPlatformCommands();
        }

        #endregion // CommandGroup

        /// <summary>
        /// Platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();
    }
}
