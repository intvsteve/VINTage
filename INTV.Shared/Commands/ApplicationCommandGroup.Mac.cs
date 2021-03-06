﻿// <copyright file="ApplicationCommandGroup.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Commands
{
    /// <summary>
    /// Mac-specific commands and behaviors for the ApplicationCommandGroup.
    /// </summary>
    public partial class ApplicationCommandGroup
    {
        #region ShowFTDIWarningCommand

        /// <summary>
        /// Command to show a warning that the FTDI driver is unstable.
        /// </summary>
        public static readonly RelayCommand ShowFTDIWarningCommand = new RelayCommand(OnShowFTDIWarningCommand)
        {
            UniqueId = UniqueNameBase + ".ShowFTDIWarningCommand"
        };

        private static void OnShowFTDIWarningCommand(object parameter)
        {
            SingleInstanceApplication.Instance.BeginInvokeOnMainThread(() =>
                {
                    var recommendedVersion = SingleInstanceApplication.AppInfo.RecommendedOSVersion;
                    if ((OSVersion.Current < recommendedVersion) && Properties.Settings.Default.ShowFTDIWarning)
                    {
                        var buttonLabels = new Dictionary<OSMessageBoxButton, string>
                        {
                            { OSMessageBoxButton.OK, Resources.Strings.OKButton_Text },
                            { OSMessageBoxButton.YesNo, Resources.Strings.OKDoNotShowAgainButton_Text }
                        };
                        var result = OSMessageBox.Show(Resources.Strings.FTDIWarning_Message, Resources.Strings.FTDIWarning_Title, OSMessageBoxButton.YesNo, buttonLabels, OSMessageBoxIcon.Exclamation);
                        Properties.Settings.Default.ShowFTDIWarning = result == OSMessageBoxResult.Yes;
                    }
                });
        }

        #endregion // ShowFTDIWarningCommand

        #region CommandGroup

        /// <summary>
        /// General data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return null; }
        }

        /// <summary>
        /// Attaches an event handler for a visual's 'Activated' event.
        /// </summary>
        /// <param name="command">The command whose <see cref=">ICommand.Execute"/> method should be called from the visual's 'Activated' event handler.</param>
        /// <param name="visual">The visual that will execute the given <paramref name="command"/>.</param>
        /// <remarks>In most cases, the MonoMac bindings to controls and other visual entities have added convenience 'Activated' events. These are simpler
        /// to use in C# than the traditional Cocoa 'Action' callbacks. This stock implementation handles NSControl, NSMenuItem, and NSToolbarItem.</remarks>
        protected override void AttachActivateHandler(RelayCommand command, NSObject visual)
        {
            if (command.UniqueId == CheckForUpdatesCommand.UniqueId)
            {
                base.AttachActivateHandler(command, visual);
            }

            // no-op for others
        }

        #endregion // CommandGroup

        #region ICommandGroup

        /// <summary>
        /// Creates the visual for a command, if applicable.
        /// </summary>
        /// <param name="command">The command for which a visual must be created.</param>
        /// <returns>The visual for the command.</returns>
        public override NSObject CreateVisualForCommand(ICommand command)
        {
            NSObject visual = null;
            if (command == SettingsDialogCommand)
            {
                var menu = RootCommandGroup.ApplicationMenuCommand.MenuItem.NativeMenuItem.Submenu;
                var item = menu.ItemWithTag((int)StandardMenuCommandId.Preferences);
                item.RepresentedObject = new NSObjectWrapper<ICommand>(command);
                item.Activated += HandleActivated; // doesn't use standard approach
                visual = item;
            }
            else
            {
                visual = base.CreateVisualForCommand(command);
            }
            return visual;
        }

        #endregion // ICommandGroup

        private void HandleActivated(object sender, System.EventArgs e)
        {
            OnShowSettingsDialog(null);
        }
    }
}
