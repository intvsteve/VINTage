// <copyright file="ClickHyperlinkBehavior.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using INTV.Shared.ComponentModel;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements a command to handle clicking the link in a Hyperlink control.
    /// </summary>
    public static class ClickHyperlinkBehavior
    {
        #region ClickHyperlinkCommand

        /// <summary>
        /// The command to open a hyperlink.
        /// </summary>
        public static readonly ICommand ClickHyperlinkCommand = new RelayCommand(OnClickHyperlink)
        {
            UniqueId = "INTV.Shared.Behavior.ClickHyperlinkBehavior.ClickHyperlinkCommand"
        };

        private static void OnClickHyperlink(object parameter)
        {
            var uri = parameter as Uri;
            System.Diagnostics.Process.Start(uri.AbsoluteUri);
        }

        #endregion // ClickHyperlinkCommand
    }
}
