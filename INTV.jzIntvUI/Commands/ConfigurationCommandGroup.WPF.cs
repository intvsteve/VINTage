// <copyright file="ConfigurationCommandGroup.WPF.cs" company="INTV Funhouse">
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

using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;

namespace INTV.JzIntvUI.Commands
{
    /// <summary>
    /// WPF-specific implementation.
    /// </summary>
    public partial class ConfigurationCommandGroup
    {
        #region ConfigurationRibbonGroupCommand

        /// <summary>
        /// Command to act as grouper for jzIntv configuration commands.
        /// </summary>
        public static readonly VisualRelayCommand ConfigurationRibbonGroupCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ConfigurationRibbonGroupCommand",
            Name = Resources.Strings.ConfigurationRibbonGroupCommand_Name,
            LargeIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/settings_32xMD.png"),
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/settings_16xLG.png"),
            VisualParent = JzIntvLauncherCommandGroup.JzIntvRibbonTabCommand,
            Weight = 0.1,
            UseXamlResource = true
        };

        #endregion // ConfigurationRibbonGroupCommand

        #region SetEnableIntellivoiceCommand

        /// <summary>
        /// Command to configure jzIntv Intellivoice setting. (All the work is in the bindings to settings in the visual.)
        /// </summary>
        public static readonly VisualRelayCommand SetEnableIntellivoiceCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEnableIntellivoiceCommand",
            Name = Resources.Strings.SettingsPage_General_EnableIntellivoice_Label,
            ToolTip = Resources.Strings.SettingsPage_General_EnableIntellivoice_Tip,
            ToolTipTitle = Resources.Strings.SettingsPage_General_EnableIntellivoice_Label,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/intellivoice_16xLG.png"),
            VisualParent = ConfigurationRibbonGroupCommand,
            Weight = 0.0,
            UseXamlResource = true
        };

        #endregion // SetEnableIntellivoiceCommand

        #region SetEnableEcsCommand

        /// <summary>
        /// Command to configure jzIntv ECS setting. (All the work is in the bindings to settings in the visual.)
        /// </summary>
        public static readonly VisualRelayCommand SetEnableEcsCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEnableEcsCommand",
            Name = Resources.Strings.SettingsPage_General_EnableECS_Label,
            ToolTip = Resources.Strings.SettingsPage_General_EnableEcs_Tip,
            ToolTipTitle = Resources.Strings.SettingsPage_General_EnableECS_Label,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/ecs_16xLG.png"),
            VisualParent = ConfigurationRibbonGroupCommand,
            Weight = 0.1,
            UseXamlResource = true
        };

        #endregion // SetEnableEcsCommand

        #region SetEnableJlpCommand

        /// <summary>
        /// Command to configure jzIntv JLP setting. (All the work is in the bindings to settings in the visual.)
        /// </summary>
        public static readonly VisualRelayCommand SetEnableJlpCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".SetEnableJlpCommand",
            Name = Resources.Strings.SettingsPage_General_JLPFeatures_Label,
            ToolTip = Resources.Strings.SettingsPage_General_EnableJlp_Tip,
            ToolTipTitle = Resources.Strings.SettingsPage_General_JLPFeatures_Label,
            SmallIcon = typeof(JzIntvLauncherCommandGroup).LoadImageResource("Resources/Images/jlp_9xSM.png"),
            VisualParent = ConfigurationRibbonGroupCommand,
            Weight = 0.2,
            UseXamlResource = true
        };

        #endregion // SetEnableJlpCommand

        #region MuteSoundCommand

        /// <summary>
        /// Command to configure jzIntv ECS setting. (All the work is in the bindings to settings in the visual.)
        /// </summary>
        public static readonly VisualRelayCommand MuteSoundCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".MuteSoundCommand",
            Name = Resources.Strings.SettingsPage_MuteSound_Label,
            ToolTip = Resources.Strings.Configuration_MuteSound_Tip,
            ToolTipTitle = Resources.Strings.SettingsPage_MuteSound_Label,
            VisualParent = ConfigurationRibbonGroupCommand,
            Weight = 0.3,
            UseXamlResource = true
        };

        #endregion // MuteSoundCommand

        #region ShowFullscreenCommand

        /// <summary>
        /// Command to configure jzIntv windowed / full-screen setting. (All the work is in the bindings to settings in the visual.)
        /// </summary>
        public static readonly VisualRelayCommand ShowFullscreenCommand = new VisualRelayCommand(INTV.Shared.ComponentModel.RelayCommand.NoOp)
        {
            UniqueId = UniqueNameBase + ".ShowFullscreenCommand",
            Name = Resources.Strings.Configuration_EnableFullscreen,
            ToolTip = Resources.Strings.Configuration_FullscreenMode_Tip,
            ToolTipTitle = Resources.Strings.Configuration_EnableFullscreen,
            VisualParent = ConfigurationRibbonGroupCommand,
            Weight = 0.4,
            UseXamlResource = true
        };

        #endregion // ShowFullscreenCommand

        private static string OSResolvePathFromSettings(string path)
        {
            return path;
        }

        private static string ResolvePathForSettings(string path)
        {
            return path;
        }

        private static void JzIntvSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var status = GetConfigurationStatusString(false);
            var configureGroupName = string.Format(Resources.Strings.ConfigurationRibbonGroupCommand_Name_Format, status);
            ConfigurationRibbonGroupCommand.Name = configureGroupName;
        }

        #region CommandGroup

        /// <summary>
        /// WPF-specific command setup.
        /// </summary>
        partial void AddPlatformCommands()
        {
            OpenSettingsDialogCommand.VisualParent = ConfigurationRibbonGroupCommand;
            OpenSettingsDialogCommand.Weight = 0.9;
            OpenSettingsDialogCommand.UseXamlResource = true;

            // Set up a handler to update group text with changes in configuration status.
            Properties.Settings.Default.PropertyChanged += JzIntvSettingsChanged;
            JzIntvSettingsChanged(null, null);

            CommandList.Add(ConfigurationRibbonGroupCommand);
            CommandList.Add(SetEnableIntellivoiceCommand);
            CommandList.Add(SetEnableEcsCommand);
            CommandList.Add(SetEnableJlpCommand);
            CommandList.Add(MuteSoundCommand.CreateRibbonSeparator(CommandLocation.Before));
            CommandList.Add(MuteSoundCommand);
            CommandList.Add(ShowFullscreenCommand);
            CommandList.Add(OpenSettingsDialogCommand.CreateRibbonSeparator(CommandLocation.Before));
            CommandList.Add(OpenSettingsDialogCommand);
        }

        #endregion // CommandGroup
    }
}
