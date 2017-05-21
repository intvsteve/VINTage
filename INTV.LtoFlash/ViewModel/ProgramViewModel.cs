// <copyright file="ProgramViewModel.cs" company="INTV Funhouse">
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
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
using INTV.Shared.ViewModel;

using IntvColor = INTV.Core.Model.Stic.Color;

#if WIN
using OSImage = System.Windows.Media.ImageSource;
#elif MAC
#if __UNIFIED__
using OSImage = AppKit.NSImage;
#else
using OSImage = MonoMac.AppKit.NSImage;
#endif // __UNIFIED__
#endif // WIN

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// ViewModel for a Program.
    /// </summary>
    public partial class ProgramViewModel : FileNodeViewModel
    {
        #region UI Strings

        /// <summary>
        /// The tip strip for the field displaying the manual associated with a program.
        /// </summary>
        public static readonly string ManualTip = Resources.Strings.MenuLayout_ManualTip;

        /// <summary>
        /// The tip strip for the button to browse to select a manual for the program.
        /// </summary>
        public static readonly string SetManualTip = Resources.Strings.MenuLayout_SetManualTip;

        /// <summary>
        /// The tip strip for the button to remove a manual's association with a program.
        /// </summary>
        public static readonly string RemoveManualTip = Resources.Strings.MenuLayout_RemoveManualTip;

        #endregion // UI Strings

        #region SetManualCommand

        /// <summary>
        /// The command to execute to associate a manual to a program.
        /// </summary>
        public static readonly RelayCommand SetManualCommand = new RelayCommand(SetManual, CanSetManual)
        {
            PreferredParameterType = typeof(ProgramViewModel)
        };

        internal static bool CanSetManual(object parameter)
        {
            return (parameter != null) && (parameter is ProgramViewModel);
        }

        internal static void SetManual(object parameter)
        {
            var program = (ProgramViewModel)parameter;
            var newManual = BrowseForSupportFile(program, ProgramFileKind.ManualText);
            if (!string.IsNullOrEmpty(newManual))
            {
                if (AcceptManual(newManual))
                {
                    program.Manual = newManual;
                }
                else
                {
                    OSMessageBox.Show(RomListViewModel.InvalidManualMessage, RomListViewModel.InvalidManualTitle, OSMessageBoxButton.OK, OSMessageBoxIcon.Error);
                }
            }
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion // SetManualCommand

        #region RemoveManualCommand

        /// <summary>
        /// The command to execute to remove a manual's association from a program.
        /// </summary>
        public static readonly RelayCommand RemoveManualCommand = new RelayCommand(RemoveManual, CanRemoveManual)
        {
            PreferredParameterType = typeof(ProgramViewModel)
        };

        internal static bool CanRemoveManual(object parameter)
        {
            var program = parameter as ProgramViewModel;
            return (program != null) && !string.IsNullOrWhiteSpace(program.Manual);
        }

        internal static void RemoveManual(object parameter)
        {
            var program = (ProgramViewModel)parameter;
            program.Manual = null;
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion // RemoveManualCommand

        private static Dictionary<Tuple<IntvColor, ProgramSupportFileState>, OSImage> _icons = new Dictionary<Tuple<IntvColor, ProgramSupportFileState>, OSImage>();
        private OSImage _icon;

        #region Properties

        /// <summary>
        /// Gets the underlying IProgramDescription used to create this object.
        /// </summary>
        public ProgramDescription ProgramDescription
        {
            get { return (Model == null) ? null : ((Program)Model).Description; }
        }

        /// <inheritdoc />
        public override OSImage Icon
        {
            get
            {
                OSImage icon = null;
                var color = Color.IntvColor;
                var state = GetSupportFileState(ProgramFileKind.Rom);
                var key = new Tuple<IntvColor, ProgramSupportFileState>(Color.IntvColor, state);
                if (!_icons.TryGetValue(key, out icon))
                {
                    icon = this.LoadImageResource(MakeIconResourceName(Color.IntvColor));
                    _icons[key] = icon;
                    _icon = icon;
                }
                return icon;
            }

            protected set
            {
                AssignAndUpdateProperty(IconPropertyName, value, ref _icon, (p, v) => RaisePropertyChanged(IconTipStripPropertyName));
            }
        }

        /// <inheritdoc />
        public override bool IsOpen
        {
            get { return false; }
            set { }
        }

        /// <inheritdoc />
        public override string IconTipStrip
        {
            get
            {
                string tipStrip = null;
                var state = GetSupportFileState(ProgramFileKind.Rom);
                switch (state)
                {
                    case ProgramSupportFileState.PresentButModified:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                                tipStrip = Resources.Strings.MenuLayout_ModifiedRomTip;
                                break;
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tipStrip = Resources.Strings.MenuLayout_WillUpdateFile_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.Missing:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                                tipStrip = Resources.Strings.MenuLayout_MissingRomTip;
                                break;
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tipStrip = Resources.Strings.MenuLayout_MissingItem_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.Deleted:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                                tipStrip = Resources.Strings.MenuLayout_MissingRomTip;
                                break;
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tipStrip = Resources.Strings.MenuLayout_WillDeleteFile_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.New:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                tipStrip = Resources.Strings.MenuLayout_WillAddFile_Tip;
                                break;
                        }
                        break;
                    case ProgramSupportFileState.RequiredPeripheralNotAttached:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                                tipStrip = Resources.Strings.MenuLayout_RequiredPeripheralNotAttached_Tip;
                                break;
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                // Not needed. When doing a preview TO LTO Flash!, ROM is compatible or not - there's no "limbo" state.
                                break;
                        }
                        break;
                    case ProgramSupportFileState.RequiredPeripheralAvailable:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                                tipStrip = Resources.Strings.MenuLayout_RequiredPeripheralAttached_Tip;
                                break;
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                break;
                        }
                        break;
                    case ProgramSupportFileState.RequiredPeripheralIncompatible:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                                tipStrip = string.Format(Resources.Strings.MenuLayout_IncompatibleDevice_Tip_Format, ProgramDescription.Rom.GetTargetDeviceUniqueId());
                                break;
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                                tipStrip = string.Format(Resources.Strings.MenuLayout_IncompatibleDeviceNotCopied_Tip_Format, ProgramDescription.Rom.GetTargetDeviceUniqueId());
                                break;
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                break;
                        }
                        break;
                    case ProgramSupportFileState.RequiredPeripheralUnknown:
                        switch (LtoFlashViewModel.SyncMode)
                        {
                            case MenuLayoutSynchronizationMode.RomList:
                                tipStrip = string.Format(Resources.Strings.MenuLayout_UnknownDevice_Tip_Format, ProgramDescription.Rom.GetTargetDeviceUniqueId());
                                break;
                            case MenuLayoutSynchronizationMode.ToLtoFlash:
                            case MenuLayoutSynchronizationMode.FromLtoFlash:
                                // Not needed. When doing a preview TO LTO Flash!, ROM is compatible or not - there's no "limbo" state.
                                break;
                        }
                        break;
                    case ProgramSupportFileState.None:
                        break;
                }
                return tipStrip;
            }
        }

        /// <inheritdoc />
        public override bool HasSupportFiles
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override string Manual
        {
            get { return GetDefaultManualPath(); }
            set { UpdateProperty("Manual", (p, v) => SetDefaultManualPath(v), value, GetDefaultManualPath()); }
        }

        /// <inheritdoc />
        public override string SaveData
        {
            get { return GetDefaultSavePath(); }
            set { UpdateProperty("SaveData", (p, v) => SetDefaultSavePath(v), value, GetDefaultSavePath()); }
        }

        #endregion // Properties

        /// <summary>
        /// Refreshes the validation state (and therefore icon) of the item.
        /// </summary>
        /// <param name="peripherals">The peripherals attached to the system, used for compatibility checks.</param>
        public void RefreshValidationState(IEnumerable<IPeripheral> peripherals)
        {
            if (ProgramDescription != null)
            {
                var currentState = State;
                var currentTipStrip = IconTipStrip;
                State = ProgramDescription.Files.ValidateSupportFile(ProgramFileKind.Rom, ProgramDescription.Crc, ProgramDescription, peripherals, Configuration.Instance.ConnectedPeripheralsHistory, true);
                if (currentState != State)
                {
                    RaisePropertyChanged(IconPropertyName);
                }
                if (currentTipStrip != IconTipStrip)
                {
                    RaisePropertyChanged(IconTipStripPropertyName);
                }
            }
        }

        /// <summary>
        /// Shows a dialog to select a support file for the program.
        /// </summary>
        /// <param name="program">The program for which a support file is being selected.</param>
        /// <param name="kind">The kind of support file to browse for.</param>
        /// <returns>The selected support file.</returns>
        internal static string BrowseForSupportFile(ProgramViewModel program, ProgramFileKind kind)
        {
            string filter = null;
            string prompt = null;

            switch (kind)
            {
                case ProgramFileKind.ManualText:
                    filter = RomListViewModel.SelectManualFilter;
                    prompt = string.Format(System.Globalization.CultureInfo.CurrentCulture, RomListViewModel.SelectManualPromptFormat, program.ProgramDescription.Name);
                    break;
                case ProgramFileKind.SaveData:
                    filter = RomListViewModel.SelectJlpSaveDataFilter;
                    prompt = string.Format(System.Globalization.CultureInfo.CurrentCulture, RomListViewModel.SelectJlpSavePromptFormat, program.ProgramDescription.Name);
                    break;
                default:
                    ErrorReporting.ReportNotImplementedError("ProgramViewModel.BrowseForSupportFile");
                    break;
            }

            string supportFilePath = null;
            var fileBrowser = FileDialogHelpers.Create();
            fileBrowser.IsFolderBrowser = false;
            fileBrowser.AddFilter(filter, kind.FileExtensions());
            fileBrowser.AddFilter(FileDialogHelpers.AllFilesFilter, new string[] { ".*" });
            fileBrowser.Title = prompt;
            fileBrowser.EnsureFileExists = true;
            fileBrowser.EnsurePathExists = true;
            fileBrowser.Multiselect = false;
            var result = fileBrowser.ShowDialog();
            if (result == FileBrowserDialogResult.Ok)
            {
                supportFilePath = fileBrowser.FileNames.First();
            }
            return supportFilePath;
        }

        /// <inheritdoc />
        internal override OSImage GetIconForColor(INTV.Core.Model.Stic.Color color)
        {
            OSImage icon = null;
            var state = GetSupportFileState(ProgramFileKind.Rom);
            var key = new Tuple<IntvColor, ProgramSupportFileState>(color, state);
            if (!_icons.TryGetValue(key, out icon))
            {
                icon = this.LoadImageResource(MakeIconResourceName(color));
                _icons[key] = icon;
            }
            if (icon == null)
            {
                ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "GetIconForColor", "ProgramViewModel");
            }
            if (icon == null)
            {
                ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "GetIconForColor", "ProgramViewModel");
            }
            return icon;
        }

        /// <summary>
        /// Induce the View to re-request the icon's tip strip.
        /// </summary>
        /// <remarks>In the case of ROM incompatibilities, we want to show a different tip strip in some icon modes, e.g. when previewing changes
        /// to send to Locutus vs. basic viewing. Because the icon may not change state, provide a way to force tip strips to update.</remarks>
        internal void RefreshTipStrip()
        {
            RaisePropertyChanged(IconTipStripPropertyName);
        }

        /// <inheritdoc />
        protected override void OnModelChanged()
        {
            base.OnModelChanged();
            Manual = GetDefaultManualPath();
            SaveData = GetDefaultSavePath();
        }

        /// <inheritdoc />
        protected override void OnColorChanged(INTV.Core.Model.Stic.Color color)
        {
            OSImage icon = GetIconForColor(color);
            Icon = icon;
            if (Icon == null)
            {
                ErrorReporting.ReportError<InvalidOperationException>(ReportMechanism.Default, "OnColorChanged", "ProgramViewModel");
            }
        }

        private string MakeIconResourceName(IntvColor color)
        {
            var state = GetSupportFileState(ProgramFileKind.Rom);
            var iconString = string.Empty;
            switch (state)
            {
                case ProgramSupportFileState.Missing:
                    iconString += "missing_";
                    break;
                case ProgramSupportFileState.PresentButModified:
                    iconString += "modified_";
                    break;
                case ProgramSupportFileState.New:
                    iconString += "added_";
                    break;
                case ProgramSupportFileState.Deleted:
                    iconString += "deleted_";
                    break;
                case ProgramSupportFileState.RequiredPeripheralIncompatible:
                    iconString += "incompatible_";
                    break;
                case ProgramSupportFileState.RequiredPeripheralUnknown:
                    iconString += "unknown_";
                    break;
                case ProgramSupportFileState.RequiredPeripheralAvailable:
                case ProgramSupportFileState.RequiredPeripheralNotAttached:
                    // Don't decorate device-specific ROMs that don't need it.
                    ////iconString += "devicespecific_";
                    break;
                case ProgramSupportFileState.None:
                case ProgramSupportFileState.PresentAndUnchanged:
                default:
                    break;
            }
            if ((color != IntvColor.NotAColor) && MenuLayoutViewModel.Colors.Contains(color))
            {
                iconString += color.ToString() + "_";
            }
            return "Resources/Images/document_" + iconString + "16xLG.png";
        }

        private ProgramSupportFileState GetSupportFileState(ProgramFileKind kind)
        {
            var state = ProgramSupportFileState.None;
            switch (LtoFlashViewModel.SyncMode)
            {
                case MenuLayoutSynchronizationMode.None:
                    break;
                case MenuLayoutSynchronizationMode.RomList:
                    if (ProgramDescription != null)
                    {
                        state = ProgramDescription.Files.GetSupportFileState(kind);
                    }
                    break;
                case MenuLayoutSynchronizationMode.ToLtoFlash:
                case MenuLayoutSynchronizationMode.FromLtoFlash:
                    state = State;
                    break;
            }
            return state;
        }

        private string GetDefaultManualPath()
        {
            string path = null;
            if (ProgramDescription != null)
            {
                path = ProgramDescription.Files.DefaultManualTextPath;
            }
            else
            {
                var program = (Program)Model;
                if ((program != null) && (program.FileSystem.Origin == FileSystemOrigin.LtoFlash) && (program.Manual != null))
                {
                    path = Resources.Strings.ProgramFork_Present;
                }
            }
            return path;
        }

        private void SetDefaultManualPath(string path)
        {
            if (ProgramDescription != null)
            {
                ProgramDescription.Files.DefaultManualTextPath = path;
            }
        }

        private string GetDefaultSavePath()
        {
            string path = null;
            if (ProgramDescription != null)
            {
                path = ProgramDescription.Files.DefaultSaveDataPath;
            }
            else
            {
                var program = (Program)Model;
                if ((program != null) && (program.FileSystem.Origin == FileSystemOrigin.LtoFlash) && (program.JlpFlash != null))
                {
                    path = Resources.Strings.ProgramFork_Present;
                }
            }
            return path;
        }

        private void SetDefaultSavePath(string path)
        {
            if (ProgramDescription != null)
            {
                ProgramDescription.Files.DefaultSaveDataPath = path;
            }
        }
    }
}
