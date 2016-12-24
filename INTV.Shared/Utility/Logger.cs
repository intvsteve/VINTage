// <copyright file="Logger.cs" company="INTV Funhouse">
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
using System.Text;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// A super-simplistic logger, helpful, perhaps, for some debugging.
    /// </summary>
    public class Logger
    {
        private readonly object _locker = new object();

        /// <summary>
        /// Creates the logger.
        /// </summary>
        /// <param name="logPath">The logger's text file path.</param>
        public Logger(string logPath)
        {
            Path = logPath;
            var directory = System.IO.Path.GetDirectoryName(logPath);
            System.IO.Directory.CreateDirectory(directory);
        }

        /// <summary>
        /// Gets the path for the log file.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Logs the message to the log file. The current time is prepended to the message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            lock (_locker)
            {
                try
                {
                    var entry = new StringBuilder(PathUtils.GetTimeString());
                    entry.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, ": {0}", message).AppendLine();
                    System.IO.File.AppendAllText(Path, entry.ToString());
                }
                catch (Exception)
                {
                    // don't want the logger dragging us down
                }
            }
        }
    }
}
