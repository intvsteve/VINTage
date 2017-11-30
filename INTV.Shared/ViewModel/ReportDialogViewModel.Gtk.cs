// <copyright file="ReportDialogViewModel.Gtk.cs" company="INTV Funhouse">
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

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class ReportDialogViewModel
    {
        private static void PlatformCopyToClipboard(object parameter)
        {
            try
            {
                var atom = Gdk.Atom.Intern("CLIPBOARD", true);
                Gtk.Clipboard.Get(atom).Text = ((ReportDialogViewModel)parameter).ReportText;
            }
            catch (System.Exception e)
            {
                // This happened in VirtualBox running on Mac...
                var message = string.Format(Resources.Strings.ReportDialog_CopyToClipboard_Failed_MessageFormat, e.Message);
                PlatformPanic(message, Resources.Strings.ReportDialog_CopyToClipboard_Failed_Title);
            }
        }
        
        // Directly use OS-specific dialog. The fancier OSMessageBox may cause a crash if we report a crash while we're reporting a crash. Because of its fancy fanciness.
        private static void PlatformPanic(string message, string title)
        {
            using (var dialog = new Gtk.MessageDialog(null, Gtk.DialogFlags.Modal, Gtk.MessageType.Error, Gtk.ButtonsType.Ok, "{0}", message))
            {
                dialog.Title = title;
                dialog.Run();
            }
        }

        private void Initialize()
        {
        }
    }
}
