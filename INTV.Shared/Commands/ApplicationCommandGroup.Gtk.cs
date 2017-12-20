// <copyright file="ApplicationCommandGroup.Gtk.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.Commands
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class ApplicationCommandGroup
    {
        #region AboutApplicationCommand

        /// <summary>
        /// The application "About" command.
        /// </summary>
        public static readonly VisualRelayCommand AboutApplicationCommand = new VisualRelayCommand(OnAbout)
        {
            // TODO: put strings in resources.
            UniqueId = UniqueNameBase + ".AboutApplicationCommand",
            Name = "About",
            Weight = 1.0,
            MenuParent = RootCommandGroup.HelpMenuCommand
        };

        private static void OnAbout(object parameter)
        {
            // TODO: put strings in resources.
            var aboutDialog = new Gtk.AboutDialog();
            aboutDialog.Version = SingleInstanceApplication.Version;

            var entryAssembly = Assembly.GetEntryAssembly();

            var attribute = entryAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                aboutDialog.ProgramName = ((AssemblyTitleAttribute)attribute).Title;
            }
            attribute = entryAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault();
            if (attribute != null)
            {
                aboutDialog.Copyright = ((AssemblyCopyrightAttribute)attribute).Copyright;
            }

            var metadataAttributes = entryAssembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false).Cast<AssemblyMetadataAttribute>();

            // Too wordy.
            ////attribute = entryAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0];
            ////var description = ((AssemblyDescriptionAttribute)attribute).Description;
            var metadataAttribute = metadataAttributes.FirstOrDefault(a => a.Key == INTV.Core.Utility.ResourceHelpers.AboutKey);
            var description = metadataAttribute == null ? string.Empty : metadataAttribute.Value;

            attribute = entryAssembly.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false).FirstOrDefault();
            var trademark = attribute == null ? string.Empty : ((AssemblyTrademarkAttribute)attribute).Trademark;

            var comments = description;
            if (string.IsNullOrEmpty(comments))
            {
                comments = trademark;
            }
            else if (!string.IsNullOrEmpty(trademark))
            {
                comments += "\n\n" + trademark;
            }
            if (!string.IsNullOrEmpty(comments))
            {
                aboutDialog.Comments = comments;
            }

            metadataAttribute = metadataAttributes.FirstOrDefault(a => a.Key == INTV.Core.Utility.ResourceHelpers.WebsiteKey);
            aboutDialog.Website = metadataAttribute == null ? "http://www.intvfunhouse.com/intvfunhouse/" : metadataAttribute.Value;

            metadataAttribute = metadataAttributes.FirstOrDefault(a => a.Key == INTV.Core.Utility.ResourceHelpers.WebsiteNameKey);
            aboutDialog.WebsiteLabel = metadataAttribute == null ? "INTV Funhouse" : metadataAttribute.Value;

            var authors = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var authorAttributes = assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false).Cast<AssemblyMetadataAttribute>().Where(a => a.Key == INTV.Core.Utility.ResourceHelpers.AuthorKey);
                authors.AddRange(authorAttributes.Select(a => a.Value));
            }
            var weightedAuthors = authors.GroupBy(a => a);
            aboutDialog.Authors = weightedAuthors.Select(wa => wa.Key).ToArray();

#if SCALE_DOWN
            var image = SingleInstanceApplication.MainWindowType.LoadImageResource(SingleInstanceApplication.SplashScreenResource);
            aboutDialog.Logo = image.ScaleSimple(Convert.ToInt32(image.Width * 0.75), Convert.ToInt32(image.Height * 0.75), Gdk.InterpType.Bilinear);
#else
            aboutDialog.Logo = SingleInstanceApplication.MainWindowType.LoadImageResource(SingleInstanceApplication.SplashScreenResource);
#endif
            try
            {
                // BUG: Should we rely on the file being there and trust nobody hacks it? Anyone care?
                var programDir = SingleInstanceApplication.Instance.ProgramDirectory;
                var licensePath = System.IO.Path.Combine(programDir, "license.txt");
                aboutDialog.License = System.IO.File.ReadAllText(licensePath);
            }
            catch (Exception)
            {
            }

            var result = aboutDialog.Run();
            VisualHelpers.Close(aboutDialog);
        }

        #endregion // AboutApplicationCommand

        #region CommandGroup

        /// <summary>
        /// Gets the general data context (parameter data) used for command execution for commands in the group.
        /// </summary>
        public override object Context
        {
            get { return null; }
        }

        #endregion // CommandGroup

        #region ICommandGroup

        /// <summary>
        /// Creates a menu item for a command.
        /// </summary>
        /// <param name="command">The command for which a menu item must be created.</param>
        /// <returns>The menu item.</returns>
        public override OSMenuItem CreateMenuItemForCommand(ICommand command)
        {
            var menuItem = OSMenuItem.Empty;
            var visualCommand = command as VisualRelayCommand;
            switch (visualCommand.UniqueId)
            {
                case UniqueNameBase + ".ExitCommand":
                    menuItem = visualCommand.CreateMenuItemForCommand(Gtk.Stock.Lookup(Gtk.Stock.Quit), true);
                    break;
                case UniqueNameBase + ".AboutApplicationCommand":
                    menuItem = visualCommand.CreateMenuItemForCommand(Gtk.Stock.Lookup(Gtk.Stock.About), true);
                    break;
                default:
                    menuItem = base.CreateMenuItemForCommand(command);
                    break;
            }
            return menuItem;
        }

        #endregion // ICommandGroup

        #region CommandGroup

        /// <summary>
        /// GTK-specific command group additions.
        /// </summary>
        partial void AddPlatformCommands()
        {
            // TODO: put strings in resources.
            CheckForUpdatesCommand.MenuParent = RootCommandGroup.HelpMenuCommand;
            SettingsDialogCommand.MenuParent = RootCommandGroup.EditMenuCommand;
            SettingsDialogCommand.MenuItemName = "Preferences...";
            SettingsDialogCommand.Weight = 1.0;
            CommandList.Add(SettingsDialogCommand.CreateSeparator(CommandLocation.Before));

            ExitCommand.MenuItemName = "Quit";
            ExitCommand.SmallIcon = OSImage.Empty;
            ExitCommand.MenuParent = RootCommandGroup.FileMenuCommand;
            ExitCommand.KeyboardShortcutKey = "q";
            ExitCommand.KeyboardShortcutModifiers = OSModifierKeys.Menu;
            CommandList.Add(ExitCommand);
            CommandList.Add(ExitCommand.CreateSeparator(CommandLocation.Before));

            ShowOnlineHelpCommand.MenuParent = RootCommandGroup.HelpMenuCommand;
            CommandList.Add(ShowOnlineHelpCommand);
            CommandList.Add(ShowOnlineHelpCommand.CreateSeparator(CommandLocation.After));

            CommandList.Add(AboutApplicationCommand);
            CommandList.Add(AboutApplicationCommand.CreateSeparator(CommandLocation.Before));
        }

        #endregion // CommandGroup
    }
}
