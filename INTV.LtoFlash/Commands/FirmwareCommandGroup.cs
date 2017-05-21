// <copyright file="FirmwareCommandGroup.cs" company="INTV Funhouse">
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

using System.Linq;
using INTV.LtoFlash.Model;
using INTV.LtoFlash.ViewModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.LtoFlash.Commands
{
    /// <summary>
    /// Firmware commands.
    /// </summary>
    public partial class FirmwareCommandGroup : INTV.Shared.Commands.CommandGroup
    {
        /// <summary>
        /// Firmware upgrade file extensions.
        /// </summary>
        public static readonly string[] UpgradeFileExtensions = new[] { ".upg" };

        /// <summary>
        /// The command group.
        /// </summary>
        internal static readonly FirmwareCommandGroup Group = new FirmwareCommandGroup();

        private const string UniqueNameBase = "INTV.LtoFlash.Commands.FirmwareCommandGroup";

        private FirmwareCommandGroup()
            : base(UniqueNameBase, Resources.Strings.Firmware_GroupHeader, 0.1)
        {
        }

        #region FirmwareGroupCommand

        /// <summary>
        /// Pseudo-command for the Firmware grouping of commands.
        /// </summary>
        public static readonly VisualDeviceCommand FirmwareGroupCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FirmwareGroupCommand",
            Name = Resources.Strings.Firmware_GroupHeader,
            LargeIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/firmware_32xLG.png"),
            SmallIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/firmware_16xLG.png"),
            Weight = 0.1
        };

        #endregion // FirmwareGroupCommand

        #region FactoryFirmwareCommand

        /// <summary>
        /// Pseudo-command used to display factory-installed firmware version.
        /// </summary>
        public static readonly VisualDeviceCommand FactoryFirmwareCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".FactoryFirmwareCommand",
            Name = Resources.Strings.FactoryFirmwareCommand_Name,
            ToolTip = Resources.Strings.FactoryFirmwareCommand_TipDescription,
            ToolTipTitle = Resources.Strings.FactoryFirmwareCommand_TipTitle,
            ToolTipDescription = Resources.Strings.FactoryFirmwareCommand_TipDescription,
            ToolTipIcon = INTV.Shared.ComponentModel.VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.1
        };

        #endregion // FactoryFirmwareCommand

        #region SecondaryFirmwareCommand

        /// <summary>
        /// Pseudo-command used to display factory-installed firmware version.
        /// </summary>
        public static readonly VisualDeviceCommand SecondaryFirmwareCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SecondaryFirmwareCommand",
            Name = Resources.Strings.SecondaryFirmwareCommand_Name,
            ToolTip = Resources.Strings.SecondaryFirmwareCommand_TipDescription,
            ToolTipTitle = Resources.Strings.SecondaryFirmwareCommand_TipTitle,
            ToolTipDescription = Resources.Strings.SecondaryFirmwareCommand_TipDescription,
            ToolTipIcon = INTV.Shared.ComponentModel.VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.11
        };

        #endregion // FactoryFirmwareCommand

        #region CurrentFirmwareCommand

        /// <summary>
        /// Pseudo-command used to display current firmware version.
        /// </summary>
        public static readonly VisualDeviceCommand CurrentFirmwareCommand = new VisualDeviceCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".CurrentFirmwareCommand",
            Name = Resources.Strings.CurrentFirmwareCommand_Name,
            ToolTip = Resources.Strings.CurrentFirmwareCommand_TipDescription,
            ToolTipTitle = Resources.Strings.CurrentFirmwareCommand_TipTitle,
            ToolTipDescription = Resources.Strings.CurrentFirmwareCommand_TipDescription,
            ToolTipIcon = INTV.Shared.ComponentModel.VisualRelayCommand.DefaultToolTipIcon,
            Weight = 0.12
        };

        #endregion // CurrentFirmwareCommand

        #region UpdateFirmware Command

        /// <summary>
        /// The command to load new firmware onto Locutus.
        /// </summary>
        public static readonly VisualDeviceCommand UpdateFirmwareCommand = new VisualDeviceCommand(OnUpdateFirmware, CanUpdateFirmware)
        {
            UniqueId = UniqueNameBase + ".UpdateFirmwareCommand",
            Name = Resources.Strings.Firmware_UpdateFirmwareCommand_MenuItemName,
            ToolTip = Resources.Strings.UpdateFirmwareCommand_TipDescription,
            LargeIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/update-firmware_32xLG.png"),
            SmallIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/update-firmware_16xLG.png"),
            Weight = 0,
            ////VisualParent = RootCommandGroup.RootCommand,
            MenuParent = FirmwareGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.UpdateFirmwareProtocolCommands,
        };

        /// <summary>
        /// Prompts to confirm a firmware update operation and then executes the update if so instructed.
        /// </summary>
        /// <param name="device">The target device of the firmware upgrade.</param>
        /// <param name="firmwareFile">Absolute path to the file containing the firmware update bits.</param>
        /// <param name="additionalMessagePrefix">Additional optional message prefix.</param>
        /// <param name="additionalMessageSuffix">Additional optional message suffix.</param>
        internal static void UpdateFirmware(Device device, string firmwareFile, string additionalMessagePrefix, string additionalMessageSuffix)
        {
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(firmwareFile) || !System.IO.File.Exists(firmwareFile))
            {
                message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.UpdateFirmwareCommand_FileNotFoundErrorMessageFormat, firmwareFile);
                OSMessageBox.Show(message, Resources.Strings.UpdateFirmwareCommand_FileNotFoundErrorTitle);
                return;
            }
            var fileInfo = new System.IO.FileInfo(firmwareFile);
            if ((fileInfo.Length > Device.TotalRAMSize) || (fileInfo.Length < MinimumFirmwareUpdateFileSize))
            {
                var badFileSizetitle = Resources.Strings.UpdateFirmwareCommand_FileTooLargeErrorTitle;
                var messageFormat = Resources.Strings.UpdateFirmwareCommand_FileTooLargeErrorMessageFormat;
                if (fileInfo.Length < MinimumFirmwareUpdateFileSize)
                {
                    badFileSizetitle = Resources.Strings.UpdateFirmwareCommand_FileTooSmallErrorTitle;
                    messageFormat = Resources.Strings.UpdateFirmwareCommand_FileTooSmallErrorMessageFormat;
                }
                message = string.Format(System.Globalization.CultureInfo.CurrentCulture, messageFormat, firmwareFile, fileInfo.Length);
                OSMessageBox.Show(message, badFileSizetitle);
                return;
            }

            var firmwareRevisions = device.FirmwareRevisions;
            int currentVersion = firmwareRevisions.Current & ~FirmwareRevisions.SecondaryMask;
            int primaryVersion = firmwareRevisions.Primary;
            bool isValidFirmwareFile;
            int updateVersion = ExtractFirmwareUpdateVersion(firmwareFile, out isValidFirmwareFile);
            string title = string.Empty;
            var dialogButtons = OSMessageBoxButton.YesNo;
            var icon = OSMessageBoxIcon.Question;

            var updateMode = FirmwareUpdateMode.None;

            var flags = (updateVersion == currentVersion) ? 0 : FirmwareRevisions.SecondaryMask;

            if (firmwareRevisions.Secondary != FirmwareRevisions.UnavailableFirmwareVersion)
            {
                var secondaryVersion = firmwareRevisions.Secondary & ~FirmwareRevisions.SecondaryMask;
                if (updateVersion == secondaryVersion)
                {
                    updateMode = FirmwareUpdateMode.ReapplySecondary;
                }
                else if (updateVersion == primaryVersion)
                {
                    updateMode = FirmwareUpdateMode.RevertToPrimary;
                }
                else if (updateVersion > secondaryVersion)
                {
                    updateMode = FirmwareUpdateMode.UpgradeSecondary;
                }
                else if (updateVersion < secondaryVersion)
                {
                    updateMode = FirmwareUpdateMode.DowngradeSecondary;
                }
            }
            else
            {
                // No secondary firmware -- still running primary.
                if (updateVersion > primaryVersion)
                {
                    updateMode = FirmwareUpdateMode.UpgradeSecondary;
                }
                else if (updateVersion < primaryVersion)
                {
                    updateMode = FirmwareUpdateMode.DowngradeSecondary;
                }
                ////else // proposed update is same as primary, so do nothing
            }

            var currentVersionString = FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Current, false);
            var updateVersionString = FirmwareRevisions.FirmwareVersionToString(updateVersion | flags, false);

            switch (updateMode)
            {
                case FirmwareUpdateMode.None:
                    title = Resources.Strings.FirmwareUpdate_SameAsFactory_Title;
                    message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FirmwareUpdate_SameAsFactory_Message, currentVersionString);
                    icon = OSMessageBoxIcon.Information;
                    dialogButtons = OSMessageBoxButton.OK;
                    break;
                case FirmwareUpdateMode.UpgradeSecondary:
                    flags |= FirmwareRevisions.SecondaryMask;
                    title = Resources.Strings.FirmwareUpdate_Title;
                    message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FirmwareUpdate_MessageFormat, currentVersionString, updateVersionString);
                    icon = OSMessageBoxIcon.Question;
                    break;
                case FirmwareUpdateMode.DowngradeSecondary:
                    flags |= FirmwareRevisions.SecondaryMask;
                    title = Resources.Strings.FirmwareUpdate_Downgrade_Title;
                    message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FirmwareUpdate_Downgrade_MessageFormat, currentVersionString, updateVersionString);
                    icon = OSMessageBoxIcon.Exclamation;
                    break;
                case FirmwareUpdateMode.ReapplySecondary:
                    flags |= FirmwareRevisions.SecondaryMask;
                    title = Resources.Strings.FirmwareUpdate_Reapply_Title;
                    message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FirmwareUpdate_Reapply_Message, currentVersionString);
                    break;
                case FirmwareUpdateMode.RevertToPrimary:
                    title = Resources.Strings.FirmwareRestore_Title;
                    message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.FirmwareUpdate_SameAsFactoryRestore_Message, FirmwareRevisions.FirmwareVersionToString(firmwareRevisions.Primary, false));
                    break;
            }

            if (!string.IsNullOrEmpty(additionalMessagePrefix))
            {
                message = additionalMessagePrefix + System.Environment.NewLine + System.Environment.NewLine + message;
            }

            if (!string.IsNullOrEmpty(additionalMessageSuffix))
            {
                message += System.Environment.NewLine + System.Environment.NewLine + additionalMessageSuffix;
            }

            var warningDialogResult = OSMessageBoxResult.No;
            try
            {
                SingleInstanceApplication.Instance.IsBusy = true;
                warningDialogResult = OSMessageBox.Show(message, title, dialogButtons, icon, OSMessageBoxResult.No);
            }
            finally
            {
                SingleInstanceApplication.Instance.IsBusy = false;
            }
            if (warningDialogResult == OSMessageBoxResult.Yes)
            {
                // Actually apply the firmware update!
                if (updateMode == FirmwareUpdateMode.RevertToPrimary)
                {
                    device.RemoveSecondaryFirmware(FirmwareRestoreCompleteHandler, (m, e) => ErrorHandler(Model.Commands.ProtocolCommandId.FirmwareEraseSecondary, m, e));
                }
                else
                {
                    device.UpdateFirmware(firmwareFile, updateVersion | flags, (c, p, r) => FirmwareUpdateCompleteHandler(c, p, r, firmwareFile), (m, e) => ErrorHandler(Model.Commands.ProtocolCommandId.FirmwareProgramSecondary, m, e));
                }
            }
        }

        private static void OnUpdateFirmware(object viewModel)
        {
            var fileBrowser = INTV.Shared.Utility.FileDialogHelpers.Create();
            fileBrowser.IsFolderBrowser = false;
            if (!fileBrowser.IsFolderBrowser)
            {
                fileBrowser.AddFilter(Resources.Strings.FirmwareUpdate_FileType, UpgradeFileExtensions);
                fileBrowser.AddFilter(Resources.Strings.FirmwareUpdate_SelectAllFilesFilter, new string[] { ".*" });
            }
            fileBrowser.Title = Resources.Strings.FirmwareUpdate_Prompt;
            fileBrowser.EnsureFileExists = true;
            fileBrowser.EnsurePathExists = true;
            fileBrowser.Multiselect = false;
            var result = fileBrowser.ShowDialog();
            if (result == INTV.Shared.Utility.FileBrowserDialogResult.Ok)
            {
                // validate firmware file
                var firmwareFile = fileBrowser.FileNames.First();
                var ltoFlashViewModel = viewModel as LtoFlashViewModel;
                UpdateFirmware(ltoFlashViewModel.ActiveLtoFlashDevice.Device, firmwareFile, null, null);
            }
        }

        private static bool CanUpdateFirmware(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            return ltoFlashViewModel.CanExecuteCommand(UpdateFirmwareCommand);
        }

        private static void FirmwareUpdateCompleteHandler(bool cancelled, bool didShowProgress, object result, string firmwareFile)
        {
            var title = string.Empty;
            var message = string.Empty;
            var dialogButtons = OSMessageBoxButton.OK;
            if (cancelled)
            {
                title = Resources.Strings.Firmware_UpdateFirmwareCommand_Cancelled_Title;
                message = Resources.Strings.Firmware_UpdateFirmwareCommand_CancelledMessage;
            }
            else
            {
                var firmwareVersion = (FirmwareRevisions)result;
                title = Resources.Strings.Firmware_UpdateFirmwareCommand_Complete_Title;
                message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.Firmware_UpdateFirmwareCommand_CompleteMessageFormat, FirmwareRevisions.FirmwareVersionToString(firmwareVersion.Current, false));
            }

            var updateNotesFile = System.IO.Path.ChangeExtension(firmwareFile, ".txt");
            var showReadme = System.IO.File.Exists(updateNotesFile);
            if (System.IO.File.Exists(updateNotesFile))
            {
                message += System.Environment.NewLine + System.Environment.NewLine + Resources.Strings.FirmwareUpdate_ShowReadmePrompt;
                dialogButtons = OSMessageBoxButton.YesNo;
            }

            // FIXME On Mac, OK and Yes are the same value, which is different than in Windows.
            // As a result, we can't rely on the dialog buttons mode to give a distinct answer,
            // so we double-check the return value *AND* whether readme exists. Ideally, the message
            // box result would be distinct. This means changing implementation of OSMessageBox and
            // all the other cases where the code calls RunModal() and simply casts the return value.
            var yesOrOK = OSMessageBox.Show(message, title, dialogButtons) == OSMessageBoxResult.Yes;
            if (showReadme && System.IO.File.Exists(updateNotesFile) && yesOrOK)
            {
                try
                {
                    RunExternalProgram.OpenInDefaultProgram(updateNotesFile);
                }
                catch (System.InvalidOperationException)
                {
                    // Silently fail.
                }
            }
        }

        private const long FirmwareUpdateVersionOffset = 0xC0;
        private const int FirmwareVersionSizeInBytes = 3;
        private const int FirmwareVersionUnreleasedFlag = 1 << 0;
        private const int FirmwareRevisionNeedsDoublingAfter = 0x08A2;
        private const int MinimumFirmwareUpdateFileSize = 0x1E004;

        /// <summary>
        /// Extracts the firmware update version.
        /// </summary>
        /// <param name="filePath">Absolute path to the file from which a firmware version number should be extracted.</param>
        /// <param name="isValidFirmwareFile">Receives a value indicating whether <paramref name="filePath"/> is valid firmware file.</param>
        /// <returns>The firmware update version, or <see cref="FirmwareRevisions.UnavailableFirmwareVersion"/> if none is found.</returns>
        internal static int ExtractFirmwareUpdateVersion(string filePath, out bool isValidFirmwareFile)
        {
            var updateVersion = FirmwareRevisions.UnavailableFirmwareVersion;
            try
            {
                using (var fileStream = FileUtilities.OpenFileStream(filePath))
                {
                    fileStream.Seek(FirmwareUpdateVersionOffset, System.IO.SeekOrigin.Begin);
                    var versionBuffer = new byte[4];
                    fileStream.Read(versionBuffer, 0, FirmwareVersionSizeInBytes);
                    updateVersion = System.BitConverter.ToInt32(versionBuffer, 0);
                    if (updateVersion > FirmwareRevisionNeedsDoublingAfter)
                    {
                        updateVersion *= 2;
                    }
                }
                isValidFirmwareFile = true;
            }
            catch (System.Exception)
            {
                // Something went wrong, so treat as invalid.
                isValidFirmwareFile = false;
                updateVersion = FirmwareRevisions.UnavailableFirmwareVersion;
            }
            return updateVersion;
        }

        #endregion // UpdateFirmware Command

        #region RestoreFirmware Command

        /// <summary>
        /// The command to restore firmware to the factory default.
        /// </summary>
        public static readonly VisualDeviceCommand RestoreFirmwareCommand = new VisualDeviceCommand(OnRestoreFirmware, CanRestoreFirmware)
        {
            UniqueId = UniqueNameBase + ".RestoreFirmwareCommand",
            Name = Resources.Strings.Firmware_RestoreFirmwareCommand_MenuItemName,
            ToolTip = Resources.Strings.RestoreFirmwareCommand_TipDescription,
            LargeIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/restore-firmware_32xLG.png"),
            SmallIcon = typeof(FirmwareCommandGroup).LoadImageResource("Resources/Images/restore-firmware_16xLG.png"),
            Weight = 0.15,
            MenuParent = FirmwareGroupCommand,
            PreferredParameterType = typeof(LtoFlashViewModel),
            RequiredProtocolCommands = DeviceHelpers.RemoveSecondaryFirmwareProtocolCommands
        };

        private static void OnRestoreFirmware(object viewModel)
        {
            var warningDialogResult = OSMessageBox.Show(
                Resources.Strings.FirmwareRestore_Message,
                Resources.Strings.FirmwareRestore_Title,
                OSMessageBoxButton.YesNo,
                OSMessageBoxIcon.Exclamation,
                OSMessageBoxResult.No);
            if (warningDialogResult == OSMessageBoxResult.Yes)
            {
                // Actually reset the device to factory default!
                var ltoFlashViewModel = viewModel as LtoFlashViewModel;
                if (ltoFlashViewModel.ActiveLtoFlashDevice.IsValid)
                {
                    var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                    device.RemoveSecondaryFirmware(FirmwareRestoreCompleteHandler, (m, e) => ErrorHandler(Model.Commands.ProtocolCommandId.FirmwareEraseSecondary, m, e));
                }
            }
        }

        private static bool CanRestoreFirmware(object parameter)
        {
            var ltoFlashViewModel = parameter as LtoFlashViewModel;
            var canExecute = ltoFlashViewModel.CanExecuteCommand(RestoreFirmwareCommand);
            if (canExecute)
            {
                var device = ltoFlashViewModel.ActiveLtoFlashDevice.Device;
                canExecute = device.FirmwareRevisions != null;
                if (canExecute)
                {
                    var secondaryVersion = device.FirmwareRevisions.Secondary;
                    canExecute = secondaryVersion != FirmwareRevisions.UnavailableFirmwareVersion;
                }
            }
            return canExecute;
        }

        private static void FirmwareRestoreCompleteHandler(bool cancelled, bool didShowProgress, object result)
        {
            var title = string.Empty;
            var message = string.Empty;
            if (cancelled)
            {
                title = Resources.Strings.Firmware_RestoreFirmwareCommand_Cancelled_Title;
                message = Resources.Strings.Firmware_RestoreFirmwareCommand_CancelledMesssage;
            }
            else
            {
                var firmwareVersion = (FirmwareRevisions)result;
                title = Resources.Strings.Firmware_RestoreFirmwareCommand_Complete_Title;
                message = string.Format(System.Globalization.CultureInfo.CurrentCulture, Resources.Strings.Firmware_RestoreFirmwareCommand_CompleteMessageFormat, FirmwareRevisions.FirmwareVersionToString(firmwareVersion.Current, false));
            }
            OSMessageBox.Show(message, title);
        }

        private enum FirmwareUpdateMode
        {
            /// <summary>
            /// No firmware update.
            /// </summary>
            None,

            /// <summary>
            /// Upgrading the secondary firmware.
            /// </summary>
            UpgradeSecondary,

            /// <summary>
            /// Downgrading the secondary firmware.
            /// </summary>
            DowngradeSecondary,

            /// <summary>
            /// Re-applying the same secondary firmware.
            /// </summary>
            ReapplySecondary,

            /// <summary>
            /// Removing the secondary firmware, which reverts to the primary.
            /// </summary>
            RevertToPrimary
        }

        #endregion // RestoreFirmware Command

        /// <summary>
        /// Report firmware command errors to the user.
        /// </summary>
        /// <param name="command">The firmware-related command that failed.</param>
        /// <param name="errorMessage">The message generated from the command execution code.</param>
        /// <param name="exception">The exception that caused the error, if applicable.</param>
        /// <returns><c>true</c> if the error was reported to the user.</returns>
        internal static bool ErrorHandler(INTV.LtoFlash.Model.Commands.ProtocolCommandId command, string errorMessage, System.Exception exception)
        {
            var handled = false;
            var title = string.Empty;
            var messageFormat = string.Empty;
            switch (command)
            {
                case Model.Commands.ProtocolCommandId.FirmwareEraseSecondary:
                    handled = true;
                    title = Resources.Strings.RestoreFirmwareCommand_Failed_Title;
                    messageFormat = Resources.Strings.RestoreFirmwareCommand_Failed_Message_Format;
                    break;
                case Model.Commands.ProtocolCommandId.FirmwareProgramSecondary:
                    handled = true;
                    title = Resources.Strings.Firmware_UpdateFirmwareCommand_Failed_Title;
                    messageFormat = Resources.Strings.Firmware_UpdateFirmwareCommand_Failed_Message_Format;
                    break;
                case Model.Commands.ProtocolCommandId.FirmwareGetRevisions:
                    handled = true;
                    title = Resources.Strings.GetFirmwareRevisionsCommand_Failed_Title;
                    messageFormat = Resources.Strings.GetFirmwareRevisionsCommand_Failed_Message_Format;
                    break;
                case Model.Commands.ProtocolCommandId.FirmwareValidateImageInRam:
                    handled = true;
                    title = Resources.Strings.ProtocolCommandId_FirmwareValidateImageInRam_Title;
                    messageFormat = Resources.Strings.ProtocolCommandId_FirmwareValidateImageInRam_Failed;
                    break;
            }
            if (handled)
            {
                var message = string.Format(System.Globalization.CultureInfo.CurrentCulture, messageFormat, errorMessage);
                OSMessageBox.Show(message, title);
            }
            return handled;
        }

        #region CommandGroup

        /// <inheritdoc />
        protected override void AddCommands()
        {
            CommandList.Add(UpdateFirmwareCommand);
            CommandList.Add(RestoreFirmwareCommand);
            AddPlatformCommands();
        }

        #endregion // CommandGroup

        /// <summary>
        /// Add platform-specific commands.
        /// </summary>
        partial void AddPlatformCommands();
    }
}
